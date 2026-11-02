// CompletionListWindow.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;
using System.Collections.Generic;

using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Components;
using System.Linq;
using MonoDevelop.Ide.Editor.Extension;
using System.ComponentModel;
using System.Threading;
using Xwt.Drawing;

namespace MonoDevelop.Ide.CodeCompletion
{
	[Obsolete]
	class CompletionListWindowGtk : PopoverWindow, ICompletionView
	{
		const int declarationWindowMargin = 3;
		const int WindowWidth = 400;

		ICompletionViewEventSink eventSink;

		Gtk.Widget parsingMessage;
		int previousWidth = -1, previousHeight = -1;
		Entry previewEntry;

		ListWidget list;
		Widget footer;
		Gtk.Box vbox;

		Gtk.Scrollbar scrollbar;
		Gtk.Box scrollBox;
		Gtk.EventControllerScroll scrollController;

		public bool InCategoryMode {
			get => list.InCategoryMode;
			set => list.InCategoryMode = value;
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		/// <summary>
		/// Window the popover is anchored to. GTK 4 has no global coordinates: trigger positions passed to
		/// <see cref="Reposition(int,int,int,bool)"/> are relative to this window (the IDE main window when null).
		/// </summary>
		public Gtk.Window TransientFor { get; set; }

		public CompletionListWindowGtk ()
		{
			ShowArrow = false;
			vbox = new Gtk.Box (Gtk.Orientation.Vertical, 0);
			list = new ListWidget (this);
			scrollController = new Gtk.EventControllerScroll (Gtk.EventControllerScrollFlags.Vertical);
			scrollController.Scroll += OnScrolled;
			list.AddController (scrollController);

			scrollbar = new Gtk.Scrollbar (Gtk.Orientation.Vertical, list.VAdjustment);
			scrollbar.Name = "CompletionScrolledWindow"; // use a different CSS style for the scrollbar
			scrollBox = new Gtk.Box (Gtk.Orientation.Horizontal, 0) { Vexpand = true, Hexpand = true };
			scrollBox.Append (list);
			scrollBox.Append (scrollbar);
			list.VAdjustment.Changed += OnAdjustmentChanged;
			list.ButtonPressed += OnButtonPressed;
			vbox.Append (scrollBox);
			ContentBox.Child = vbox;
			Theme.CornerRadius = 0;
			Theme.Padding = 0;

			UpdateStyle ();
			Gui.Styles.Changed += HandleThemeChanged;
			IdeApp.Preferences.ColorScheme.Changed += HandleThemeChanged;

			if (IdeApp.Workbench != null)
				this.TransientFor = IdeApp.Workbench.RootWindow;
			Hidden += HandleHidden;
			list.SelectionChanged += OnSelectionChanged;
		}

		void OnAdjustmentChanged (object sender, EventArgs e)
		{
			// the scrolled window used to hide its scrollbar when everything fits
			var adj = list.VAdjustment;
			scrollbar.Visible = adj.Upper > adj.PageSize;
		}

		void HandleThemeChanged (object sender, EventArgs e)
		{
			UpdateStyle ();
		}

		void UpdateStyle ()
		{
			Theme.SetBackgroundColor (Gui.Styles.CodeCompletion.BackgroundColor.ToCairoColor ());
			Theme.ShadowColor = Gui.Styles.PopoverWindow.ShadowColor.ToCairoColor ();
			list.QueueDraw ();
		}

		public void Initialize (IListDataProvider provider, ICompletionViewEventSink eventSink)
		{
			list.DataProvider = provider;
			this.eventSink = eventSink;
		}

		public int SelectedItemIndex {
			get { return list.SelectedItemIndex; }
			set { list.SelectedItemIndex = value; }
		}

		public bool SelectionEnabled {
			get { return list.SelectionEnabled; }
			set { list.SelectionEnabled = value; }
		}

		public void ShowFilteredItems (CompletionListFilterResult filterResult)
		{
			list.ShowFilteredItems (filterResult);
		}

		public void ShowFooter (Widget w)
		{
			HideFooter ();
			vbox.Append (w);
			footer = w;
		}

		public void HideFooter ()
		{
			if (footer != null) {
				vbox.Remove (footer);
				footer = null;
			}
		}

		/// <summary>
		/// This method is used to set the completion window to its initial state.
		/// This is required for re-using the window object.
		/// </summary>
		protected internal virtual void ResetState ()
		{
			list.ResetState ();
		}

		protected int curXPos, curYPos;

		public void ResetSizes ()
		{
			var allocWidth = Width;
			if (IsRealized && !Visible) {
				allocWidth = list.WidthRequest = WindowWidth;
			}

			int width = Math.Max (allocWidth, list.WidthRequest + Theme.CornerRadius * 2);
			int height = Math.Max (Height, list.HeightRequest + 2 + (footer != null ? footer.Height : 0) + Theme.CornerRadius * 2);
			GetSizeRequest (out var currentWidth, out var currentHeight);
			if (currentWidth != width || currentHeight != height)
				SetSizeRequest (width, height);
		}

		void ReleaseObjects ()
		{
			list.ResetState ();
		}

		protected override void OnDestroyed ()
		{
			list.SelectionChanged -= OnSelectionChanged;
			list.VAdjustment.Changed -= OnAdjustmentChanged;
			scrollController.Scroll -= OnScrolled;
			list.ButtonPressed -= OnButtonPressed;
			Hidden -= HandleHidden;

			ReleaseObjects ();

			Gui.Styles.Changed -= HandleThemeChanged;
			IdeApp.Preferences.ColorScheme.Changed -= HandleThemeChanged;

			base.OnDestroyed ();
		}

		int currentTriggerX;
		int currentTriggerY;
		int currentTriggerHeight;
		bool shown;

		public void Reposition (int triggerX, int triggerY, int triggerHeight, bool force)
		{
			currentTriggerX = triggerX;
			currentTriggerY = triggerY;
			currentTriggerHeight = triggerHeight;
			Reposition (force);
		}

		public void Reposition (bool force)
		{
			X = currentTriggerX - TextOffset;
			Y = currentTriggerY;
			curXPos = X;
			curYPos = Y;
			if (!shown)
				return;

			int w = Width, h = Height;
			if (!force && previousHeight != h && previousWidth != w)
				return;
			previousHeight = h;
			previousWidth = w;

			// GTK places the popover below the trigger line and flips it above the line when there is no room.
			var anchor = TransientFor ?? IdeApp.Workbench?.RootWindow;
			if (anchor != null)
				ShowPopup (anchor, new Gdk.Rectangle (X, Y - currentTriggerHeight, 1, Math.Max (1, currentTriggerHeight)), PopupPosition.TopLeft);
		}

		void HandleHidden (object sender, EventArgs e)
		{
			ReleaseObjects ();
		}

		void ICompletionView.ResetState ()
		{
			previousWidth = previousHeight = -1;
			list.ResetState ();
		}

		void ICompletionView.Hide ()
		{
			shown = false;
			if (Visible)
				Popdown ();
		}

		Xwt.Rectangle ICompletionView.Allocation {
			get {
				return new Xwt.Rectangle (X, Y, Width, Height);
			}
		}

		public void MoveCursor (int relative)
		{
			list.MoveCursor (relative);
		}

		public void PageDown ()
		{
			var adj = list.VAdjustment;
			adj.Value = Math.Max (0, Math.Min (adj.Upper - adj.PageSize, adj.Value + adj.PageSize));
			MoveCursor (8);
		}

		public void PageUp ()
		{
			var adj = list.VAdjustment;
			adj.Value = Math.Max (0, adj.Value - adj.PageSize);
			MoveCursor (-8);
		}

		public void HideLoadingMessage ()
		{
			HideFooter ();
		}

		public void ShowLoadingMessage ()
		{
			if (parsingMessage == null) {
				var box = new Gtk.Box (Gtk.Orientation.Vertical, 0);
				box.Append (new Gtk.Separator (Gtk.Orientation.Horizontal));
				var hbox = new Gtk.Box (Gtk.Orientation.Horizontal, 3);
				hbox.MarginTop = hbox.MarginBottom = hbox.MarginStart = hbox.MarginEnd = 3;
				hbox.Append (new Components.ImageView ("md-parser", StockIconSize.Menu));
				var lab = new Gtk.Label (GettextCatalog.GetString ("Updating..."));
				lab.Xalign = 0;
				lab.Hexpand = true;
				hbox.Append (lab);
				box.Append (hbox);
				parsingMessage = box;
			}
			ShowFooter (parsingMessage);
		}

		void ICompletionView.Show ()
		{
			shown = true;
			Reposition (true);
			if (!Visible)
				Popup ();
		}

		public void ShowPreviewCompletionEntry ()
		{
			previewEntry = new Entry ();
			previewEntry.Changed += delegate (object sender, EventArgs e) {
				eventSink.OnPreviewCompletionEntryChanged (previewEntry.Text);
			};
			previewEntry.AddKeyPressHandler ((keyval, keycode, state) => {
				var key = (Gdk.Key)keyval;
				return eventSink.OnPreProcessPreviewCompletionEntryKey (KeyDescriptor.FromGtk (key, (char)Gdk.Keyval.ToUnicode (keyval), state));
			}, phase: Gtk.PropagationPhase.Capture);
			vbox.Append (previewEntry);

			previewEntry.Activated += (sender, e) => eventSink.OnPreviewCompletionEntryActivated ();
			// the entry needs the keyboard focus: an autohiding popover grabs it
			Autohide = true;
			previewEntry.AddFocusHandler (null, () => eventSink.OnPreviewCompletionEntryLostFocus ());
			GLib.Timeout.Add (10, delegate {
				previewEntry.GrabFocus ();
				return false;
			});
		}

		public bool RepositionDeclarationViewWindow (TooltipInformationWindow declarationviewwindow, int item)
		{
			if (!IsRealized)
				return false;

			Gdk.Rectangle rect = list.GetRowArea (item);
			if (rect.Width <= 0 || rect.Height <= 0 || rect.Y + rect.Height < (int)list.VAdjustment.Value || rect.Y > list.Height + (int)list.VAdjustment.Value)
				return false;

			int y = rect.Y + Theme.Padding - (int)list.VAdjustment.Value;
			declarationviewwindow.ShowPopup (this, new Gdk.Rectangle (0, Math.Min (Height, Math.Max (0, y)), Width, rect.Height), PopupPosition.Left);
			return true;
		}

		public void RepositionWindow (Xwt.Rectangle? r)
		{
			Gdk.Rectangle? rect = r != null ? (Gdk.Rectangle?)new Gdk.Rectangle ((int)r.Value.Left, (int)r.Value.Top, (int)r.Value.Width, (int)r.Value.Height) : null;
			RepositionWindow (rect);
		}

		void OnSelectionChanged (object o, EventArgs args)
		{
			eventSink.OnSelectedItemChanged ();
		}

		void OnScrolled (object o, Gtk.ScrollArgs args)
		{
			if (!scrollbar.Visible)
				return;

			var adj = list.VAdjustment;

			//This widget is a special case because it's always aligned to items as it scrolls.
			//Although this means we can't use the pixel deltas for true smooth scrolling, we
			//can still make use of the effective scrolling velocity by basing the calculation
			//on pixels and rounding to the nearest item.

			double dy = args.Dy;
			if (dy == 0)
				return;

			// mouse wheels report notches, touchpads report surface pixels
			var pixelDelta = scrollController.Unit == Gdk.ScrollUnit.Wheel ? dy * adj.StepIncrement : dy;
			double discreteItemDelta = System.Math.Round (pixelDelta);
			if (discreteItemDelta == 0.0)
				discreteItemDelta = dy > 0 ? 1.0 : -1.0;

			adj.AddValueClamped (discreteItemDelta);
			args.RetVal = true;

			eventSink.OnListScrolled ();
		}

		void OnButtonPressed (ClickInfo click)
		{
			if (click.Button == 1 && click.NPress == 2)
				eventSink.OnDoubleClick ();
		}

		//smaller lists get size reallocated after FillList, so we have to notify when the window size has changed
		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);
			eventSink?.OnAllocationChanged ();
		}

		public int TextOffset {
			get { return list.TextOffset + (int)Theme.CornerRadius; }
		}
	}
}
