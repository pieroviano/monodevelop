//
// MonoDevelop.Components.Docking.cs
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
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Xwt.Motion;
using MonoDevelop.Core;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components.Docking
{
	class DockFrame: ContainerWidget, IAnimatable
	{
		public event EventHandler<EventArgs> LayoutChanged;

		/// <summary>Raised after the frame has been allocated (GTK 4 has no size-allocate signal).</summary>
		internal event EventHandler FrameSizeAllocated;

		internal const double ItemDockCenterArea = 0.4;
		internal const int GroupDockSeparatorSize = 40;
		
		internal bool ShadedSeparators = true;
		
		DockContainer container;
		
		int handleSize = 1;
		int handlePadding = 0;
		int defaultItemWidth = 300;
		int defaultItemHeight = 250;
		uint autoShowDelay = 400;
		uint autoHideDelay = 500;
		
		SortedDictionary<string,DockLayout> layouts = new SortedDictionary<string,DockLayout> ();
		List<DockFrameTopLevel> topLevels = new List<DockFrameTopLevel> ();
		string currentLayout;
		int compactGuiLevel = 3;
		
		DockBar dockBarTop, dockBarBottom, dockBarLeft, dockBarRight;
		Box mainBox;
		DockVisualStyle defaultStyle;
		Gtk.Widget overlayWidget;

		public DockFrame ()
		{
			this.SetCommonAttributes ("DockFrame", null, null);

			dockBarTop = new DockBar (this, Gtk.PositionType.Top);
			dockBarTop.SetCommonAttributes ("DockFrame.TopBar", null, null);

			dockBarBottom = new DockBar (this, Gtk.PositionType.Bottom);
			dockBarBottom.SetCommonAttributes ("DockFrame.BottomBar", null, null);

			dockBarLeft = new DockBar (this, Gtk.PositionType.Left);
			dockBarLeft.SetCommonAttributes ("DockFrame.LeftBar", null, null);

			dockBarRight = new DockBar (this, Gtk.PositionType.Right);
			dockBarRight.SetCommonAttributes ("DockFrame.RightBar", null, null);

			container = new DockContainer (this);
			container.SetCommonAttributes ("DockFrame.Main", null, null);
			container.Hexpand = container.Vexpand = true;

			var hbox = new Box (Orientation.Horizontal, 0);
			hbox.SetShouldIgnore (true);
			hbox.Vexpand = true;

			hbox.Append (dockBarLeft);
			hbox.Append (container);
			hbox.Append (dockBarRight);
			mainBox = new Box (Orientation.Vertical, 0);
			mainBox.SetShouldIgnore (true);

			mainBox.Append (dockBarTop);
			mainBox.Append (hbox);
			mainBox.Append (dockBarBottom);
			AddChild (mainBox);

			// A press anywhere in the frame (not handled by a child) minimizes the auto-hidden pads
			this.AddClickHandler (c => MinimizeAllAutohidden ());

			CompactGuiLevel = 2;
			UpdateDockbarsVisibility ();

			DefaultVisualStyle = new DockVisualStyle ();
		}

		public bool DockbarsVisible {
			get {
				return !OverlayWidgetVisible;
			}
		}


		
		/// <summary>
		/// Compactness level of the gui, from 1 (not compact) to 5 (very compact).
		/// </summary>
		public int CompactGuiLevel {
			get { return compactGuiLevel; }
			set {
				compactGuiLevel = value;
/*				switch (compactGuiLevel) {
					case 1: handleSize = 6; break;
					case 2: 
					case 3: handleSize = IsWindows ? 4 : 6; break;
					case 4:
					case 5: handleSize = 3; break;
				}
*/				handlePadding = 0;
				dockBarTop.OnCompactLevelChanged ();
				dockBarBottom.OnCompactLevelChanged ();
				dockBarLeft.OnCompactLevelChanged ();
				dockBarRight.OnCompactLevelChanged ();
				container.RelayoutWidgets ();
			}
		}

		internal bool OverlayWidgetVisible { get; set; }

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				this.AbortAnimation ("ShowOverlayWidget");
				this.AbortAnimation ("HideOverlayWidget");
			}
			base.Dispose (disposing);
		}

		public void AddOverlayWidget (Widget widget, bool animate = false)
		{
			RemoveOverlayWidget (false);

			this.overlayWidget = widget;
			AddChild (widget);

			OverlayWidgetVisible = true;
			MinimizeAllAutohidden ();
			if (animate) {
				currentOverlayPosition = Math.Max (0, Height);
				this.Animate (
					"ShowOverlayWidget",
					ShowOverlayWidgetAnimation,
					finished: (a, b) => {
						mainBox.Hide ();
					},
					easing: Easing.CubicOut);
			} else {
				currentOverlayPosition = 0;
				mainBox.Hide ();
				QueueResize ();
			}

			UpdateDockbarsVisibility ();
		}

		public void RemoveOverlayWidget (bool animate = false)
		{
			this.AbortAnimation ("ShowOverlayWidget");
			this.AbortAnimation ("HideOverlayWidget");
			OverlayWidgetVisible = false;

			mainBox.Show ();

			if (overlayWidget != null) {
				if (animate) {
					currentOverlayPosition = 0;
					this.Animate (
						"HideOverlayWidget",
						HideOverlayWidgetAnimation,
						finished: (a,b) => {
							if (overlayWidget != null) {
								RemoveChild (overlayWidget);
								overlayWidget = null;
							}
						},
						easing: Easing.SinOut);
				} else {
					RemoveChild (overlayWidget);
					overlayWidget = null;
					QueueResize ();
				}
			}

			UpdateDockbarsVisibility ();
		}

		int currentOverlayPosition;

		void UpdateDockbarsVisibility ()
		{
			dockBarTop.UpdateVisibility ();
			dockBarBottom.UpdateVisibility ();
			dockBarLeft.UpdateVisibility ();
			dockBarRight.UpdateVisibility ();
		}

		void ShowOverlayWidgetAnimation (double value)
		{
			currentOverlayPosition = (int)((double)Height * (1f - value));
			QueueAllocate ();
		}

		void HideOverlayWidgetAnimation (double value)
		{
			currentOverlayPosition = (int)((double)Height * value);
			QueueAllocate ();
		}

		void IAnimatable.BatchBegin ()
		{
		}

		void IAnimatable.BatchCommit ()
		{
		}

		// Registered region styles. We are using a list instead of a dictionary because
		// the registering order is important
		List<Tuple<string,DockVisualStyle>> regionStyles = new List<Tuple<string, DockVisualStyle>> ();

		// Styles specific to items
		Dictionary<string,DockVisualStyle> stylesById = new Dictionary<string, DockVisualStyle> ();

		public DockVisualStyle DefaultVisualStyle {
			get {
				return defaultStyle;
			}
			set {
				defaultStyle = DockVisualStyle.CreateDefaultStyle ();
				defaultStyle.CopyValuesFrom (value);
			}
		}

		/// <summary>
		/// Sets the style for a region of the dock frame
		/// </summary>
		/// <param name='regionPosition'>
		/// A region is a collection with the format: "ItemId1/Position1;ItemId2/Position2..."
		/// ItemId is the id of a dock item. Position is one of the values of the DockPosition enumeration
		/// </param>
		/// <param name='style'>
		/// Style.
		/// </param>
		public void SetRegionStyle (string regionPosition, DockVisualStyle style)
		{
			// Remove any old region style and add it
			regionStyles.RemoveAll (s => s.Item1 == regionPosition);
			if (style != null)
				regionStyles.Add (new Tuple<string,DockVisualStyle> (regionPosition, style));
		}

		public void SetDockItemStyle (string itemId, DockVisualStyle style)
		{
			if (style != null)
				stylesById [itemId] = style;
			else
				stylesById.Remove (itemId);
		}

		internal void UpdateRegionStyle (DockObject obj)
		{
			obj.VisualStyle = GetRegionStyleForObject (obj);
		}

		/// <summary>
		/// Gets the style for a dock object, which will inherit values from all region/style definitions
		/// </summary>
		internal DockVisualStyle GetRegionStyleForObject (DockObject obj)
		{
			DockVisualStyle mergedStyle = null;
			if (obj is DockGroupItem) {
				DockVisualStyle s;
				if (stylesById.TryGetValue (((DockGroupItem)obj).Id, out s)) {
					mergedStyle = DefaultVisualStyle.Clone ();
					mergedStyle.CopyValuesFrom (s);
				}
			}
			foreach (var e in regionStyles) {
				if (InRegion (e.Item1, obj)) {
					if (mergedStyle == null)
						mergedStyle = DefaultVisualStyle.Clone ();
					mergedStyle.CopyValuesFrom (e.Item2);
				}
			}
			return mergedStyle ?? DefaultVisualStyle;
		}

		internal DockVisualStyle GetRegionStyleForItem (DockItem item)
		{
			DockVisualStyle s;
			if (stylesById.TryGetValue (item.Id, out s)) {
				var ds = DefaultVisualStyle.Clone ();
				ds.CopyValuesFrom (s);
				return ds;
			}
			return DefaultVisualStyle;
		}

		/// <summary>
		/// Gets the style assigned to a specific position of the layout
		/// </summary>
		/// <returns>
		/// The region style for position.
		/// </returns>
		/// <param name='parentGroup'>
		/// Group which contains the position
		/// </param>
		/// <param name='childIndex'>
		/// Index of the position inside the group
		/// </param>
		/// <param name='insertingPosition'>
		/// If true, the position will be inserted (meaning that the objects in childIndex will be shifted 1 position)
		/// </param>
		internal DockVisualStyle GetRegionStyleForPosition (DockGroup parentGroup, int childIndex, bool insertingPosition)
		{
			DockVisualStyle mergedStyle = null;
			foreach (var e in regionStyles) {
				if (InRegion (e.Item1, parentGroup, childIndex, insertingPosition)) {
					if (mergedStyle == null)
						mergedStyle = DefaultVisualStyle.Clone ();
					mergedStyle.CopyValuesFrom (e.Item2);
				}
			}
			return mergedStyle ?? DefaultVisualStyle;
		}

		internal bool InRegion (string location, DockObject obj)
		{
			if (obj.ParentGroup == null)
				return false;
			return InRegion (location, obj.ParentGroup, obj.ParentGroup.GetObjectIndex (obj), false);
		}

		internal bool InRegion (string location, DockGroup objToFindParent, int objToFindIndex, bool insertingPosition)
		{
			// Checks if the object is in the specified region.
			// A region is a collection with the format: "ItemId1/Position1;ItemId2/Position2..."
			string[] positions = location.Split (';');
			foreach (string pos in positions) {
				// We individually check each entry in the region specification
				int i = pos.IndexOf ('/');
				if (i == -1) continue;
				string id = pos.Substring (0,i).Trim ();
				DockGroup g = container.Layout.FindGroupContaining (id);
				if (g != null) {
					DockPosition dpos;
					try {
						dpos = (DockPosition) Enum.Parse (typeof(DockPosition), pos.Substring(i+1).Trim(), true);
					}
					catch {
						continue;
					}

					var refItem = g.FindDockGroupItem (id);
					if (InRegion (g, dpos, refItem, objToFindParent, objToFindIndex, insertingPosition))
						return true;
				}
			}
			return false;
		}

		bool InRegion (DockGroup grp, DockPosition pos, DockObject refObject, DockGroup objToFindParent, int objToFindIndex, bool insertingPosition)
		{
			if (grp == null)
				return false;

			if (grp.Type == DockGroupType.Tabbed) {
				if (pos != DockPosition.Center &&  pos != DockPosition.CenterBefore)
					return InRegion (grp.ParentGroup, pos, grp, objToFindParent, objToFindIndex, insertingPosition);
			}
			if (grp.Type == DockGroupType.Horizontal) {
				if (pos != DockPosition.Left && pos != DockPosition.Right)
					return InRegion (grp.ParentGroup, pos, grp, objToFindParent, objToFindIndex, insertingPosition);
			}
			if (grp.Type == DockGroupType.Vertical) {
				if (pos != DockPosition.Top && pos != DockPosition.Bottom)
					return InRegion (grp.ParentGroup, pos, grp, objToFindParent, objToFindIndex, insertingPosition);
			}

			bool foundAtLeftSide = true;
			bool findingLeft = pos == DockPosition.Left || pos == DockPosition.Top || pos == DockPosition.CenterBefore;

			if (objToFindParent == grp) {
				// Check positions beyond the current range of items
				if (objToFindIndex < 0 && findingLeft)
					return true;
				if (objToFindIndex >= grp.Objects.Count && !findingLeft)
					return true;
			}

			for (int n=0; n<grp.Objects.Count; n++) {
				var ob = grp.Objects[n];

				bool foundRefObject = ob == refObject;
				bool foundTargetObject = objToFindParent == grp && objToFindIndex == n;

				if (foundRefObject) {
					// Found the reference object, but if insertingPosition=true it is in the position that the new item will have,
					// so this position still has to be considered to be at the left side
					if (foundTargetObject && insertingPosition)
						return foundAtLeftSide == findingLeft;
					foundAtLeftSide = false;
				}
				else if (foundTargetObject)
					return foundAtLeftSide == findingLeft;
				else if (ob is DockGroup) {
					DockGroup gob = (DockGroup)ob;
					if (gob == objToFindParent || ObjectHasAncestor (objToFindParent, gob))
						return foundAtLeftSide == findingLeft;
				}
			}
			return InRegion (grp.ParentGroup, pos, grp, objToFindParent, objToFindIndex, insertingPosition);
		}

		bool ObjectHasAncestor (DockObject obj, DockGroup ancestorToFind)
		{
			return obj != null && (obj.ParentGroup == ancestorToFind || ObjectHasAncestor (obj.ParentGroup, ancestorToFind));
		}
		
		public DockBar ExtractDockBar (PositionType pos)
		{
			DockBar db = new DockBar (this, pos);
			switch (pos) {
				case PositionType.Left: db.OriginalBar = dockBarLeft; dockBarLeft = db; break;
				case PositionType.Top: db.OriginalBar = dockBarTop; dockBarTop = db; break;
				case PositionType.Right: db.OriginalBar = dockBarRight; dockBarRight = db; break;
				case PositionType.Bottom: db.OriginalBar = dockBarBottom; dockBarBottom = db; break;
			}
			return db;
		}
		
		internal DockBar GetDockBar (PositionType pos)
		{
			switch (pos) {
				case Gtk.PositionType.Top: return dockBarTop;
				case Gtk.PositionType.Bottom: return dockBarBottom;
				case Gtk.PositionType.Left: return dockBarLeft;
				case Gtk.PositionType.Right: return dockBarRight;
			}
			return null;
		}
		
		internal DockContainer Container {
			get { return container; }
		}

		public int HandleSize {
			get {
				return handleSize;
			}
			set {
				handleSize = value;
			}
		}
		
		public int HandlePadding {
			get {
				return handlePadding;
			}
			set {
				handlePadding = value;
			}
		}

		public int DefaultItemWidth {
			get {
				return defaultItemWidth;
			}
			set {
				defaultItemWidth = value;
			}
		}

		public int DefaultItemHeight {
			get {
				return defaultItemHeight;
			}
			set {
				defaultItemHeight = value;
			}
		}
		
		internal int TotalHandleSize {
			get { return handleSize + handlePadding*2; }
		}

		internal int TotalSensitiveHandleSize {
			get { return 6; }
		}
		
		public DockItem AddItem (string id)
		{
			foreach (DockItem dit in container.Items) {
				if (dit.Id == id) {
					if (dit.IsPositionMarker) {
						dit.IsPositionMarker = false;
						return dit;
					}
					throw new InvalidOperationException ("An item with id '" + id + "' already exists.");
				}
			}
			
			DockItem it = new DockItem (this, id);
			container.Items.Add (it);
			return it;
		}
		
		public void RemoveItem (DockItem it)
		{
			if (container.Layout != null)
				container.Layout.RemoveItemRec (it);
			foreach (DockGroup grp in layouts.Values)
				grp.RemoveItemRec (it);
			container.Items.Remove (it);
		}
		
		public DockItem GetItem (string id)
		{
			foreach (DockItem it in container.Items) {
				if (it.Id == id) {
					if (!it.IsPositionMarker)
					    return it;
					else
					    return null;
				}
			}
			return null;
		}
		
		public IEnumerable<DockItem> GetItems ()
		{
			return container.Items;
		}
		
		bool LoadLayout (string layoutName)
		{
			DockLayout dl;
			if (!layouts.TryGetValue (layoutName, out dl))
				return false;

			var focus = GetActiveWidget ();

			container.LoadLayout (dl);

			// Keep the currently focused widget when switching layouts
			if (focus != null && focus.IsRealized && focus.Visible)
				DockItem.SetFocus (focus);

			return true;
		}

		Gtk.Widget GetActiveWidget ()
		{
			Gtk.Widget widget = this;
			while (widget.FocusChild != null)
				widget = widget.FocusChild;
			return widget;
		}

		public void CreateLayout (string name)
		{
			CreateLayout (name, false);
		}
		
		public void DeleteLayout (string name)
		{
			layouts.Remove (name);
		}
		
		public void CreateLayout (string name, bool copyCurrent)
		{
			DockLayout dl;
			if (container.Layout == null || !copyCurrent) {
				dl = GetDefaultLayout ();
			} else {
				container.StoreAllocation ();
				dl = (DockLayout) container.Layout.Clone ();
			}
			dl.Name = name;
			layouts [name] = dl;
		}
		
		public string CurrentLayout {
			get {
				return currentLayout;
			}
			set {
				if (currentLayout == value)
					return;
				if (LoadLayout (value)) {
					currentLayout = value;
				}
			}
		}
		
		public bool HasLayout (string id)
		{
			return layouts.ContainsKey (id);
		}
		
		public string[] Layouts {
			get {
				if (layouts.Count == 0)
					return new string [0];
				string[] arr = new string [layouts.Count];
				layouts.Keys.CopyTo (arr, 0);
				return arr;
			}
		}

		public uint AutoShowDelay {
			get {
				return autoShowDelay;
			}
			set {
				autoShowDelay = value;
			}
		}

		public uint AutoHideDelay {
			get {
				return autoHideDelay;
			}
			set {
				autoHideDelay = value;
			}
		}
		
		public void SaveLayouts (string file)
		{
			using (XmlTextWriter w = new XmlTextWriter (file, System.Text.Encoding.UTF8)) {
				w.Formatting = Formatting.Indented;
				SaveLayouts (w);
			}
		}
		
		public void SaveLayouts (XmlWriter writer)
		{
			if (container.Layout != null)
				container.Layout.StoreAllocation ();
			writer.WriteStartElement ("layouts");
			foreach (DockLayout la in layouts.Values)
				la.Write (writer);
			writer.WriteEndElement ();
		}
		
		public void LoadLayouts (string file)
		{
			using (XmlReader r = new XmlTextReader (new System.IO.StreamReader (file))) {
				LoadLayouts (r);
			}
		}
		
		public void LoadLayouts (XmlReader reader)
		{
			layouts.Clear ();
			container.Clear ();
			currentLayout = null;
			
			reader.MoveToContent ();
			if (reader.IsEmptyElement) {
				reader.Skip ();
				return;
			}
			reader.ReadStartElement ("layouts");
			reader.MoveToContent ();
			while (reader.NodeType != XmlNodeType.EndElement) {
				if (reader.NodeType == XmlNodeType.Element) {
					DockLayout layout = DockLayout.Read (this, reader);
					layout.AllocationChanged += LayoutAllocationChanged;
					layouts.Add (layout.Name, layout);
				}
				else
					reader.Skip ();
				reader.MoveToContent ();
			}
			reader.ReadEndElement ();
			container.RelayoutWidgets ();
		}

		void LayoutAllocationChanged (object sender, EventArgs e)
		{
			LayoutChanged?.Invoke (this, EventArgs.Empty);
		}

		internal void UpdateTitle (DockItem item)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null)
				return;
			
			gitem.ParentGroup.UpdateTitle (item);
			dockBarTop.UpdateTitle (item);
			dockBarBottom.UpdateTitle (item);
			dockBarLeft.UpdateTitle (item);
			dockBarRight.UpdateTitle (item);
		}
		
		internal void UpdateStyles ()
		{
			container.ReloadStyles ();
		}

		internal void UpdateStyle (DockItem item)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null)
				return;
			
			gitem.ParentGroup.UpdateStyle (item);
			dockBarTop.UpdateStyle (item);
			dockBarBottom.UpdateStyle (item);
			dockBarLeft.UpdateStyle (item);
			dockBarRight.UpdateStyle (item);
		}
		
		internal void Present (DockItem item, bool giveFocus)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null)
				return;
			
			gitem.ParentGroup.Present (item, giveFocus);
		}
		
		internal bool GetVisible (DockItem item)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null)
				return false;
			return gitem.VisibleFlag;
		}
		
		internal bool GetVisible (DockItem item, string layoutName)
		{
			DockLayout dl;
			if (!layouts.TryGetValue (layoutName, out dl))
				return false;
			
			DockGroupItem gitem = dl.FindDockGroupItem (item.Id);
			if (gitem == null)
				return false;
			return gitem.VisibleFlag;
		}
		
		internal void SetVisible (DockItem item, bool visible)
		{
			if (container.Layout == null)
				return;
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			
			if (gitem == null) {
				if (visible) {
					// The item is not present in the layout. Add it now.
					if (!string.IsNullOrEmpty (item.DefaultLocation))
						gitem = AddDefaultItem (container.Layout, item);
						
					if (gitem == null) {
						// No default position
						gitem = new DockGroupItem (this, item);
						container.Layout.AddObject (gitem);
					}
				} else
					return; // Already invisible
			}
			gitem.SetVisible (visible);
			container.RelayoutWidgets ();
		}
		
		internal DockItemStatus GetStatus (DockItem item)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null)
				return DockItemStatus.Dockable;
			return gitem.Status;
		}
		
		internal void SetStatus (DockItem item, DockItemStatus status)
		{
			DockGroupItem gitem = container.FindDockGroupItem (item.Id);
			if (gitem == null) {
				item.DefaultStatus = status;
				return;
			}
			gitem.StoreAllocation ();
			gitem.Status = status;
			container.RelayoutWidgets ();
		}

		internal void SetDockLocation (DockItem item, string placement)
		{
			bool vis = item.Visible;
			DockItemStatus stat = item.Status;
			item.ResetMode ();
			container.Layout.RemoveItemRec (item);
			AddItemAtLocation (container.Layout, item, placement, vis, stat);
		}
		
		DockLayout GetDefaultLayout ()
		{
			DockLayout group = new DockLayout (this);
			
			// Add items which don't have relative defaut positions
			
			List<DockItem> todock = new List<DockItem> ();
			foreach (DockItem item in container.Items) {
				if (string.IsNullOrEmpty (item.DefaultLocation)) {
					DockGroupItem dgt = new DockGroupItem (this, item);
					dgt.SetVisible (item.DefaultVisible);
					group.AddObject (dgt);
				}
				else
					todock.Add (item);
			}
			
			// Add items with relative positions.
			int lastCount = 0;
			while (lastCount != todock.Count) {
				lastCount = todock.Count;
				for (int n=0; n<todock.Count; n++) {
					DockItem it = todock [n];
					if (AddDefaultItem (group, it) != null) {
						todock.RemoveAt (n);
						n--;
					}
				}
			}
			
			// Items which could not be docked because of an invalid default location
			foreach (DockItem item in todock) {
				DockGroupItem dgt = new DockGroupItem (this, item);
				dgt.SetVisible (false);
				group.AddObject (dgt);
			}
//			group.Dump ();
			return group;
		}
		
		DockGroupItem AddDefaultItem (DockGroup grp, DockItem it)
		{
			return AddItemAtLocation (grp, it, it.DefaultLocation, it.DefaultVisible, it.DefaultStatus);
		}
		
		DockGroupItem AddItemAtLocation (DockGroup grp, DockItem it, string location, bool visible, DockItemStatus status)
		{
			string[] positions = location.Split (';');
			foreach (string pos in positions) {
				int i = pos.IndexOf ('/');
				if (i == -1) continue;
				string id = pos.Substring (0,i).Trim ();
				DockGroup g = grp.FindGroupContaining (id);
				if (g != null) {
					DockPosition dpos;
					try {
						dpos = (DockPosition) Enum.Parse (typeof(DockPosition), pos.Substring(i+1).Trim(), true);
					}
					catch {
						continue;
					}
					DockGroupItem dgt = g.AddObject (it, dpos, id);
					dgt.SetVisible (visible);
					dgt.Status = status;
					return dgt;
				}
			}
			return null;
		}
		
		internal void AddTopLevel (DockFrameTopLevel w, int x, int y, int width, int height)
		{
			ValidateWindowBounds (ref x, ref y, ref width, ref height);

			// GTK 4 cannot position top level windows: auto-hide pads are always laid out inside the frame
			w.X = x;
			w.Y = y;
			AddChild (w);
			w.Size = new Size (width, height);
			topLevels.Add (w);
		}

		void ValidateWindowBounds (ref int x, ref int y, ref int w, ref int h)
		{
			x = Math.Max (0, x);
			y = Math.Max (0, y);
			w = Math.Min (mainBox.Width - x, w);
			h = Math.Min (mainBox.Height - y, h);
		}

		internal void RemoveTopLevel (DockFrameTopLevel w)
		{
			RemoveChild (w);
			topLevels.Remove (w);
			QueueResize ();
		}

		public Gdk.Rectangle GetCoordinates (Gtk.Widget w)
		{
			if (!w.TranslateCoordinates (this, 0, 0, out double px, out double py))
				return new Gdk.Rectangle (0,0,0,0);

			return new Gdk.Rectangle ((int)px, (int)py, w.Width, w.Height);
		}
		
		internal void ShowPlaceholder (DockItem draggedItem)
		{
			container.ShowPlaceholder (draggedItem);
		}
		
		internal void DockInPlaceholder (DockItem item)
		{
			container.DockInPlaceholder (item);
		}
		
		internal void HidePlaceholder ()
		{
			container.HidePlaceholder ();
		}
		
		/// <param name="pointerX">Pointer X in <see cref="DockContainer"/> coordinates.</param>
		/// <param name="pointerY">Pointer Y in <see cref="DockContainer"/> coordinates.</param>
		internal void UpdatePlaceholder (DockItem item, Gdk.Size size, bool allowDocking, int pointerX, int pointerY)
		{
			container.UpdatePlaceholder (item, size, allowDocking, pointerX, pointerY);
		}
		
		internal DockBarItem BarDock (Gtk.PositionType pos, DockItem item, int size)
		{
			return GetDockBar (pos).AddItem (item, size);
		}
		
		internal AutoHideBox AutoShow (DockItem item, DockBar bar, int size)
		{
			AutoHideBox aframe = new AutoHideBox (this, item, bar.Position, size);
			Gdk.Size sTop = GetBarFrameSize (dockBarTop);
			Gdk.Size sBot = GetBarFrameSize (dockBarBottom);
			Gdk.Size sLeft = GetBarFrameSize (dockBarLeft);
			Gdk.Size sRgt = GetBarFrameSize (dockBarRight);

			int x,y,w,h;
			if (bar == dockBarLeft || bar == dockBarRight) {
				h = Height - sTop.Height - sBot.Height;
				w = size;
				y = sTop.Height;
				if (bar == dockBarLeft)
					x = sLeft.Width;
				else
					x = Width - size - sRgt.Width;
			} else {
				w = Width - sLeft.Width - sRgt.Width;
				h = size;
				x = sLeft.Width;
				if (bar == dockBarTop)
					y = sTop.Height;
				else
					y = Height - size - sBot.Height;
			}

			AddTopLevel (aframe, x, y, w, h);
			aframe.AnimateShow ();

			return aframe;
		}
		
		internal void UpdateSize (DockBar bar, AutoHideBox aframe)
		{
			Gdk.Size sTop = GetBarFrameSize (dockBarTop);
			Gdk.Size sBot = GetBarFrameSize (dockBarBottom);
			Gdk.Size sLeft = GetBarFrameSize (dockBarLeft);
			Gdk.Size sRgt = GetBarFrameSize (dockBarRight);
			
			if (bar == dockBarLeft || bar == dockBarRight) {
				aframe.Size = new Gdk.Size (aframe.RequestedWidth, Height - sTop.Height - sBot.Height);
				if (bar == dockBarRight)
					aframe.X = Width - aframe.Width - sRgt.Width;
			} else {
				aframe.Size = new Gdk.Size (Width - sLeft.Width - sRgt.Width, aframe.RequestedHeight);
				if (bar == dockBarBottom)
					aframe.Y = Height - aframe.Height - sBot.Height;
			}
		}
		
		Gdk.Size GetBarFrameSize (DockBar bar)
		{
			if (bar.OriginalBar != null)
				bar = bar.OriginalBar;
			if (!bar.Visible)
				return new Gdk.Size (0,0);
			Gtk.Requisition req = bar.GetRequisition ();
			return new Gdk.Size (req.Width, req.Height);
		}
		
		internal void AutoHide (DockItem item, AutoHideBox widget, bool animate)
		{
			if (animate) {
				widget.Hidden += delegate {
					if (!widget.Disposed)
						AutoHide (item, widget, false);
				};
				widget.AnimateHide ();
			}
			else {
				// The widget may already be removed from the parent. RemoveFromParent unmaps the widget first
				// (removing it from its parent causes it to unrealize without unmapping).
				DockUtil.RemoveFromParent (item.Widget);
				DockUtil.RemoveFromParent (item.TitleTab);
				RemoveTopLevel (widget);

				widget.Disposed = true;
				widget.Destroy ();
			}
		}

		// Only the main box defines the size of the frame: auto-hide pads and the overlay widget are laid out on top of it.
		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			mainBox.Measure (orientation, for_size, out minimum, out natural, out _, out _);
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			if (mainBox.Visible)
				mainBox.SizeAllocate (new Gdk.Rectangle (0, 0, width, height), -1);

			foreach (DockFrameTopLevel tl in topLevels) {
				Requisition r = tl.GetRequisition ();
				tl.SizeAllocate (new Gdk.Rectangle (tl.X, tl.Y, r.Width, r.Height), -1);
			}
			if (overlayWidget != null)
				overlayWidget.SizeAllocate (new Gdk.Rectangle (0, currentOverlayPosition, width, height), -1);

			FrameSizeAllocated?.Invoke (this, EventArgs.Empty);
		}

		internal void MinimizeAllAutohidden ()
		{
			foreach (var it in GetItems ()) {
				if (it.Visible && it.Status == DockItemStatus.AutoHide)
					it.Minimize ();
			}
		}

		static internal bool IsWindows {
			get { return System.IO.Path.DirectorySeparatorChar == '\\'; }
		}

		internal static Cairo.Color ToCairoColor (Gdk.RGBA color)
		{
			return new Cairo.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		protected override bool OnFocus (DirectionType direction)
		{
			// If there's an overlay widget, that's all we can focus
			if (overlayWidget != null && overlayWidget.Visible) {
				overlayWidget.ChildFocus (direction);
				return true;
			}

			return base.OnFocus (direction);
		}
	}

	public class DockStyle
	{
		public const string Default = "Default";
		public const string Browser = "Browser";
	}
	
	
	internal delegate void DockDelegate (DockItem item);
	
}
