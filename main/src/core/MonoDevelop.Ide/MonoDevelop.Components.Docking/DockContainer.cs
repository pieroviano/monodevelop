//
//
// Author:
//   Lluis Sanchez Gual
//

//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using System.Linq;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using GLib;

namespace MonoDevelop.Components.Docking
{
	/// <summary>
	/// Lays out the pads of a <see cref="DockLayout"/> inside the dock frame. The children (pad contents, title tabs,
	/// tab strips and splitters) are allocated explicitly from the layout, in this widget's coordinates.
	/// </summary>
	class DockContainer : ContainerWidget
	{
		DockLayout layout;
		DockFrame frame;

		List<TabStrip> notebooks = new List<TabStrip> ();
		List<DockItem> items = new List<DockItem> ();

		List<SplitterWidgetWrapper> splitters = new List<SplitterWidgetWrapper> ();

		volatile PlaceholderWindow placeholderWindow;
		volatile PadTitleWindow padTitleWindow;

		public DockContainer (DockFrame frame)
		{
			this.SetRole (AtkCocoa.Roles.AXSplitGroup);

			this.frame = frame;
		}

		internal DockGroupItem FindDockGroupItem (string id)
		{
			if (layout == null)
				return null;
			else
				return layout.FindDockGroupItem (id);
		}

		public List<DockItem> Items {
			get { return items; }
		}

		public DockLayout Layout {
			get { return layout; }
			set { layout = value; }
		}

		public void Clear ()
		{
			layout = null;
		}

		internal bool IsSwitchingLayout { get; set; }

		public void LoadLayout (DockLayout dl)
		{
			IsSwitchingLayout = true;
			HidePlaceholder ();

			// Sticky items currently selected in notebooks will remain
			// selected after switching the layout
			List<DockItem> sickyOnTop = new List<DockItem> ();
			foreach (DockItem it in items) {
				if ((it.Behavior & DockItemBehavior.Sticky) != 0) {
					DockGroupItem gitem = FindDockGroupItem (it.Id);
					if (gitem != null && gitem.ParentGroup.IsSelectedPage (it))
						sickyOnTop.Add (it);
				}
			}

			if (layout != null)
				layout.StoreAllocation ();
			layout = dl;
			layout.RestoreAllocation ();

			// Make sure items not present in this layout are hidden
			foreach (DockItem it in items) {
				if ((it.Behavior & DockItemBehavior.Sticky) != 0)
					it.Visible = it.StickyVisible;
				if (layout.FindDockGroupItem (it.Id) == null)
					it.HideWidget ();
			}

			RelayoutWidgets ();

			foreach (DockItem it in sickyOnTop)
				it.Present (false);

			IsSwitchingLayout = false;
		}

		public void StoreAllocation ()
		{
			if (layout != null)
				layout.StoreAllocation ();
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = 0;
			if (layout != null) {
				var req = layout.SizeRequest ();
				minimum = natural = orientation == Gtk.Orientation.Horizontal ? req.Width : req.Height;
			}
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			if (layout != null) {
				var rect = new Gdk.Rectangle (0, 0, width, height);
				layout.Size = -1;
				layout.SizeAllocate (rect);

				usedSplitters = 0;
				layout.DrawSeparators (null, null, 0, DrawSeparatorOperation.Allocate, null);
			}

			// Drag feedback is laid out over the pads
			placeholderWindow?.AllocateOverlay (width, height);
			padTitleWindow?.AllocateOverlay (width, height);
		}

		int usedSplitters;

		const int SplitterSize = 5;

		internal void AllocateSplitter (DockGroup grp, int index, Gdk.Rectangle a)
		{
			var s = splitters[usedSplitters++];
			if (a.Height > a.Width) {
				a.Width = SplitterSize;
				a.X -= (int)(SplitterSize / 2);
			}
			else {
				a.Height = SplitterSize;
				a.Y -= (int)(SplitterSize / 2);
			}
			s.SizeAllocate (a);
			s.Init (grp, index);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0) {
				var bg = Styles.DockFrameBackground.ToCairoColor ();
				snapshot.DrawCairo (this, cr => {
					cr.SetSourceColor (bg);
					cr.Rectangle (0, 0, width, height);
					cr.Fill ();
				});
			}

			base.OnSnapshot (snapshot);

			if (layout != null && width > 0 && height > 0) {
				snapshot.DrawCairo (this, cr => {
					layout.Draw (cr, null, 0);
				});
			}
		}

		protected override void OnChildAdded (Gtk.Widget child)
		{
			(child as DockItemContainer)?.NotifyParentChanged ();
		}

		protected override void OnChildRemoved (Gtk.Widget child)
		{
			(child as DockItemContainer)?.NotifyParentChanged ();
		}

		public void RelayoutWidgets ()
		{
			if (layout != null) {
				layout.AddRemoveWidgets ();
				LayoutWidgets ();
			}

			QueueResize ();
		}

		public void ReloadStyles ()
		{
			foreach (var item in Items)
				item.SetRegionStyle (frame.GetRegionStyleForItem (item));
			RelayoutWidgets ();
		}

		void LayoutWidgets ()
		{
			// Create the needed notebooks and place the widgets in there

			List<DockGroup> tabbedGroups = new List<DockGroup> ();
			GetTabbedGroups (layout, tabbedGroups);

			for (int n=0; n<tabbedGroups.Count; n++) {
				DockGroup grp = tabbedGroups [n];
				TabStrip ts;
				if (n < notebooks.Count) {
					ts = notebooks [n];
				}
				else {
					ts = new TabStrip (frame);
					notebooks.Add (ts);
					AddChild (ts);
				}
				frame.UpdateRegionStyle (grp);
				ts.VisualStyle = grp.VisualStyle;
				grp.UpdateNotebook (ts);
			}

			// Remove spare tab strips
			for (int n = notebooks.Count - 1; n >= tabbedGroups.Count; n--) {
				TabStrip ts = notebooks [n];
				notebooks.RemoveAt (n);
				ts.Clear ();
				RemoveChild (ts);
			}

			// Create and add the required splitters

			int reqSpliters = CountRequiredSplitters (layout);

			for (int n=0; n < splitters.Count; n++) {
				var s = splitters [n];
				if (s.Parent != null)
					RemoveChild (s.Widget);
			}

			// Hide the splitters that are not required

			for (int n=reqSpliters; n < splitters.Count; n++)
				splitters[n].Hide ();

			// Add widgets to the container

			layout.LayoutWidgets ();

			// Create and add the required splitters

			for (int n=0; n < reqSpliters; n++) {
				if (n < splitters.Count) {
					var s = splitters [n];
					if (!s.Visible)
						s.Show ();
					AddChild (s.Widget);
				} else {

					SplitterWidgetWrapper s = null;
#if MAC
					var widget = new SplitterMacHostWidget ();
#else
					var widget = new SplitterWidget ();
#endif
					s = new SplitterWidgetWrapper (widget);
					splitters.Add (s);
					s.Show ();
					AddChild (s.Widget);
				}
			}

			// Keep the drag feedback on top of the pads
			if (placeholderWindow != null) {
				RemoveChild (placeholderWindow);
				AddChild (placeholderWindow);
			}
			if (padTitleWindow != null) {
				RemoveChild (padTitleWindow);
				AddChild (padTitleWindow);
			}
		}

		void GetTabbedGroups (DockGroup grp, List<DockGroup> tabbedGroups)
		{
			if (grp.Type == DockGroupType.Tabbed) {
				if (grp.VisibleObjects.Count > 1)
					tabbedGroups.Add (grp);
				else
					grp.ResetNotebook ();
			}
			else {
				// Make sure it doesn't have a notebook bound to it
				grp.ResetNotebook ();
				foreach (DockObject ob in grp.Objects) {
					if (ob is DockGroup)
						GetTabbedGroups ((DockGroup) ob, tabbedGroups);
				}
			}
		}

		int CountRequiredSplitters (DockGroup grp)
		{
			if (grp.Type == DockGroupType.Tabbed)
				return 0;
			else {
				int num = grp.VisibleObjects.Count - 1;
				if (num < 0)
					return 0;
				foreach (var c in grp.VisibleObjects.OfType<DockGroup> ())
					num += CountRequiredSplitters (c);
				return num;
			}
		}

		internal void ShowPlaceholder (DockItem draggedItem)
		{
			HidePlaceholder ();
			padTitleWindow = new PadTitleWindow (frame, draggedItem);
			placeholderWindow = new PlaceholderWindow (frame);
			AddChild (placeholderWindow);
			AddChild (padTitleWindow);
		}

		/// <param name="pointerX">Pointer X in this container's coordinates.</param>
		/// <param name="pointerY">Pointer Y in this container's coordinates.</param>
		internal bool UpdatePlaceholder (DockItem item, Gdk.Size size, bool allowDocking, int pointerX, int pointerY)
		{
			try {
				Runtime.AssertMainThread ();

				var placeholderWindow = this.placeholderWindow;
				var padTitleWindow = this.padTitleWindow;

				if (placeholderWindow == null || padTitleWindow == null || !IsRealized)
					return false;

				int px = pointerX, py = pointerY;

				placeholderWindow.AllowDocking = allowDocking;

				// The title of the dragged pad follows the pointer
				var ts = padTitleWindow.GetNaturalRequisition ();
				padTitleWindow.Relocate (px - ts.Width / 2, py - ts.Height / 2, ts.Width, ts.Height);

				DockDelegate dockDelegate;
				Gdk.Rectangle rect;
				if (allowDocking && layout.GetDockTarget (item, px, py, out dockDelegate, out rect)) {
					placeholderWindow.Relocate (rect.X, rect.Y, rect.Width, rect.Height, true);
					placeholderWindow.Show ();
					placeholderWindow.SetDockInfo (dockDelegate, rect);
					return true;
				} else {
					int w, h;
					var gi = layout.FindDockGroupItem (item.Id);
					if (gi != null) {
						w = gi.Allocation.Width;
						h = gi.Allocation.Height;
					} else {
						w = item.DefaultWidth;
						h = item.DefaultHeight;
					}
					placeholderWindow.Relocate (px - w / 2, py - h / 2, w, h, false);
					placeholderWindow.Show ();
					placeholderWindow.AllowDocking = false;
				}
			} catch (Exception ex) {
				LoggingService.LogInternalError ("Updating the dock container placeholder failed", ex);
			}

			return false;
		}

		internal void DockInPlaceholder (DockItem item)
		{
			if (placeholderWindow == null || !placeholderWindow.Visible)
				return;

			try {
				if (placeholderWindow.AllowDocking && placeholderWindow.DockDelegate != null) {
					item.Status = DockItemStatus.Dockable;
					DockGroupItem dummyItem = new DockGroupItem (frame, new DockItem (frame, "__dummy"));
					DockGroupItem gitem = layout.FindDockGroupItem (item.Id);
					gitem.ParentGroup.ReplaceItem (gitem, dummyItem);
					placeholderWindow.DockDelegate (item);
					dummyItem.ParentGroup.Remove (dummyItem);
					RelayoutWidgets ();
				} else {
					DockGroupItem gi = FindDockGroupItem (item.Id);
					// GTK 4 windows can't be positioned, only the size of the floating window is kept
					var pr = placeholderWindow.Rect;
					gi.FloatRect = new Gdk.Rectangle (0, 0, pr.Width, pr.Height);
					item.Status = DockItemStatus.Floating;
				}
			} catch (Exception ex) {
				LoggingService.LogInternalError ("Updating the dock container placeholder failed", ex);
			}
		}

		internal void HidePlaceholder ()
		{
			if (placeholderWindow != null) {
				RemoveChild (placeholderWindow);
				placeholderWindow = null;
			}
			if (padTitleWindow != null) {
				RemoveChild (padTitleWindow);
				padTitleWindow = null;
			}
		}

		internal class SplitterWidget: Gtk.Widget, ISplitterWidget
		{
			bool dragging;
			double dragStartX, dragStartY;
			int dragPos;
			int dragSize;

			DockGroup dockGroup;
			int dockIndex;

			public Widget Widget => this;

			public SplitterWidget ()
			{
				this.SetRole (AtkCocoa.Roles.AXSplitter);

				// The splitter is invisible, it only provides a grab area over the separator
				this.AddDragHandler (OnDragBegin, OnDragUpdate, OnDragEnd);
			}

			public void Init (DockGroup grp, int index)
			{
				dockGroup = grp;
				dockIndex = index;
			}

			void ISplitterWidget.SizeAllocate (Gdk.Rectangle rect)
			{
				SizeAllocate (rect, -1);
			}

			protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
			{
				minimum = natural = 0;
				minimum_baseline = natural_baseline = -1;
			}

			protected override void OnSizeAllocate (int width, int height, int baseline)
			{
				bool vertical = height > width;
				this.SetOrientation (vertical ? Orientation.Vertical : Orientation.Horizontal);
				CursorFromName = vertical ? "col-resize" : "row-resize";
				base.OnSizeAllocate (width, height, baseline);
			}

			// Pointer position along the resize axis, in the coordinates of the dock container
			int GetContainerPointer (double x, double y)
			{
				if (Parent == null || !TranslateCoordinates (Parent, x, y, out double cx, out double cy))
					return 0;
				return (int)((dockGroup.Type == DockGroupType.Horizontal) ? cx : cy);
			}

			void OnDragBegin (double x, double y)
			{
				if (dockGroup == null)
					return;
				dragging = true;
				dragStartX = x;
				dragStartY = y;
				dragPos = GetContainerPointer (x, y);
				DockObject obj = dockGroup.VisibleObjects [dockIndex];
				dragSize = (dockGroup.Type == DockGroupType.Horizontal) ? obj.Allocation.Width : obj.Allocation.Height;
			}

			void OnDragEnd (double offsetX, double offsetY)
			{
				dragging = false;
			}

			void OnDragUpdate (double offsetX, double offsetY)
			{
				if (dragging) {
					int newpos = GetContainerPointer (dragStartX + offsetX, dragStartY + offsetY);
					if (newpos != dragPos) {
						int nsize = dragSize + (newpos - dragPos);
						dockGroup.ResizeItem (dockIndex, nsize);
					}
				}
			}
		}
	}

	enum DrawSeparatorOperation
	{
		Draw,
		Invalidate,
		Allocate,
		CollectAreas
	}
}
