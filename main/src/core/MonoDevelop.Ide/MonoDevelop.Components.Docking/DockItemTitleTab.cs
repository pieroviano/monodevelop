//
// DockItemTitleTab.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using Gtk;

using System;
using MonoDevelop.Ide.Gui;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide.Fonts;

namespace MonoDevelop.Components.Docking
{

	class DockItemTitleTab : BinWidget
	{
		static Xwt.Drawing.Image dockTabActiveBackImage = Xwt.Drawing.Image.FromResource ("padbar-active.9.png");
		static Xwt.Drawing.Image dockTabBackImage = Xwt.Drawing.Image.FromResource ("padbar-inactive.9.png");

		bool active;
		Gtk.Widget page;
		ExtendedLabel labelWidget;
		int labelWidth;
		int minWidth;
		DockVisualStyle visualStyle;
		ImageView tabIcon;
		Gtk.Box box;
		DockFrame frame;
		string label;
		ImageButton btnDock;
		ImageButton btnClose;
		Gtk.Box btnBox;
		DockItem item;
		bool allowPlaceholderDocking;
		bool mouseOver;
		Widget currentFocus = null; // Currently focused child
		bool placeholderActive;
		EventControllerKey dragKeyController;
		Gtk.Window dragKeyWindow;


		static Xwt.Drawing.Image pixClose;
		static Xwt.Drawing.Image pixAutoHide;
		static Xwt.Drawing.Image pixDock;

		static readonly Xwt.WidgetSpacing TabPadding;
		static readonly Xwt.WidgetSpacing TabActivePadding;

		internal event EventHandler<EventArgs> TabPressed;

		static DockItemTitleTab ()
		{
			pixClose = Xwt.Drawing.Image.FromResource ("pad-close-9.png");
			pixAutoHide = Xwt.Drawing.Image.FromResource ("pad-minimize-9.png");
			pixDock = Xwt.Drawing.Image.FromResource ("pad-dock-9.png");

			Xwt.Drawing.NinePatchImage tabBackImage9;
			if (dockTabBackImage is Xwt.Drawing.ThemedImage) {
				var img = ((Xwt.Drawing.ThemedImage)dockTabBackImage).GetImage (Xwt.Drawing.Context.GlobalStyles);
				tabBackImage9 = img as Xwt.Drawing.NinePatchImage;
			} else
				tabBackImage9 = dockTabBackImage as Xwt.Drawing.NinePatchImage;
			TabPadding = tabBackImage9.Padding;


			Xwt.Drawing.NinePatchImage tabActiveBackImage9;
			if (dockTabActiveBackImage is Xwt.Drawing.ThemedImage) {
				var img = ((Xwt.Drawing.ThemedImage)dockTabActiveBackImage).GetImage (Xwt.Drawing.Context.GlobalStyles);
				tabActiveBackImage9 = img as Xwt.Drawing.NinePatchImage;
			} else
				tabActiveBackImage9 = dockTabActiveBackImage as Xwt.Drawing.NinePatchImage;
			TabActivePadding = tabActiveBackImage9.Padding;
		}

		public DockItemTitleTab (DockItem item, DockFrame frame)
		{
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += HandlePress;
			actionHandler.PerformShowMenu += HandleShowMenu;

			UpdateRole (false, null);

			CanFocus = true;
			Focusable = true;
			this.item = item;
			this.frame = frame;
			UpdateVisualStyle ();

			this.AddClickHandler (OnButtonPress, OnButtonRelease);
			this.AddDragHandler (OnDragBegin, OnDragUpdate, OnDragEnd);
			this.AddMotionHandler (null, (x, y) => { mouseOver = true; UpdateBehavior (); }, OnLeave);
			this.AddFocusHandler (OnFocusIn, OnFocusOut);
			this.AddKeyPressHandler ((keyval, keycode, state) => {
				var key = (Gdk.Key)keyval;
				if (key == Gdk.Key.Return || key == Gdk.Key.KP_Enter || key == Gdk.Key.space) {
					TabPressed?.Invoke (this, EventArgs.Empty);
					return true;
				}
				return false;
			});
		}



		internal void UpdateRole (bool isTab, TabStrip strip)
		{
			Gtk.IAccessible fromAccessible = null, toAccessible = null;

			if (!isTab) {
				this.SetRole (AtkCocoa.Roles.AXGroup, "pad header");
				this.SetSubRole ("XAPadHeader");

				// Take the button accessibles back from the strip
				if (strip != null) {
					fromAccessible = strip;
					toAccessible = this;
				}
			} else {
				this.SetRole (AtkCocoa.Roles.AXRadioButton, "tab");
				this.SetSubRole ("");

				// Give the button accessibles to the strip
				if (strip != null) {
					fromAccessible = this;
					toAccessible = strip;
				}
			}

			if (fromAccessible != null && toAccessible != null) {
				fromAccessible.TransferAccessibleChild (toAccessible, btnDock);
				fromAccessible.TransferAccessibleChild (toAccessible, btnClose);
			}
		}

		public DockVisualStyle VisualStyle {
			get { return visualStyle; }
			set {
				visualStyle = value;
				UpdateVisualStyle ();
				QueueDraw ();
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				EndKeyTracking ();
			base.Dispose (disposing);
		}

		void UpdateVisualStyle ()
		{
			double inactiveIconAlpha;

			if (IdeApp.Preferences == null || IdeApp.Preferences.UserInterfaceTheme == Theme.Light)
				inactiveIconAlpha = 0.8;
			else
				inactiveIconAlpha = 0.6;

			if (labelWidget?.Visible == true && label != null) {
				if (visualStyle.UppercaseTitles.Value)
					labelWidget.Text = label.ToUpper ();
				else
					labelWidget.Text = label;
				labelWidget.UseMarkup = true;
				if (visualStyle.ExpandedTabs.Value)
					labelWidget.Xalign = 0.5f;

				if (!(Parent is TabStrip))
					labelWidget.Xalign = 0;
			}

			if (tabIcon != null) {
				tabIcon.Image = tabIcon.Image.WithAlpha (active ? 1.0 : inactiveIconAlpha);
				tabIcon.Visible = visualStyle.ShowPadTitleIcon.Value;
			}

			if (labelWidget?.Visible == true) {
				var font = IdeServices.FontService.SansFont.CopyModified (null, Pango.Weight.Bold);
				var color = (active ? visualStyle.PadTitleLabelColor.Value : visualStyle.InactivePadTitleLabelColor.Value);
				DockUtil.SetCss (labelWidget, string.Format (System.Globalization.CultureInfo.InvariantCulture,
					"font-family: \"{0}\"; font-weight: bold; font-size: 11px; color: {1};", font.Family, DockUtil.ToCss (color)));
			}

			labelWidth = this.GetNaturalRequisition ().Width + 1;

			if (visualStyle != null)
				HeightRequest = visualStyle.PadTitleHeight != null ? (int)(visualStyle.PadTitleHeight.Value) : -1;
		}

		public void SetLabel (Gtk.Widget page, Xwt.Drawing.Image icon, string label)
		{
			string labelNoSpaces = label != null ? label.Replace (' ', '-') : null;
			this.label = label;
			this.page = page;

			if (icon == null)
				icon = ImageService.GetIcon ("md-empty");

			if (box == null) {
				box = new Box (Gtk.Orientation.Horizontal, 0);
				box.SetShouldIgnore (true);

				tabIcon = new ImageView ();
				tabIcon.SetShouldIgnore (true);
				tabIcon.MarginStart = 3;
				tabIcon.MarginEnd = 1;
				box.Append (tabIcon);

				labelWidget = new ExtendedLabel (label);
				// Ignore the label because the title tab already contains its name
				labelWidget.SetShouldIgnore (true);
				labelWidget.UseMarkup = true;
				labelWidget.Hexpand = true;
				labelWidget.Valign = Align.Center;
				labelWidget.MarginEnd = 15;
				labelWidget.Ellipsize = Pango.EllipsizeMode.End;
				labelWidget.Overflow = Overflow.Hidden;
				box.Append (labelWidget);

				btnDock = new ImageButton ();
				btnDock.Image = pixAutoHide;
				btnDock.TooltipText = GettextCatalog.GetString ("Auto Hide");
				btnDock.CanFocus = true;
				//			btnDock.WidthRequest = btnDock.HeightRequest = 17;
				btnDock.Clicked += OnClickDock;
				btnDock.WidthRequest = btnDock.GetNaturalRequisition ().Width;
				btnDock.MarginStart = btnDock.MarginEnd = 3;
				UpdateDockButtonAccessibilityLabels ();

				btnClose = new ImageButton ();
				btnClose.Image = pixClose;
				btnClose.TooltipText = GettextCatalog.GetString ("Close");
				btnClose.CanFocus = true;
				//			btnClose.WidthRequest = btnClose.HeightRequest = 17;
				btnClose.WidthRequest = btnDock.GetNaturalRequisition ().Width;
				btnClose.MarginStart = btnClose.MarginEnd = 1;
				btnClose.Clicked += delegate {
					item.Visible = false;
				};

				btnBox = new Box (Gtk.Orientation.Horizontal, 0);
				btnBox.SetShouldIgnore (true);
				btnBox.Valign = Align.Center;
				btnBox.MarginStart = btnBox.MarginEnd = 3;
				btnBox.Append (btnDock);
				btnBox.Append (btnClose);
				box.Append (btnBox);

				Child = box;
			}

			tabIcon.Image = icon;

			string realLabel, realHelp;
			if (!string.IsNullOrEmpty (label)) {
				labelWidget.Visible = true;
				labelWidget.Name = label;
				btnDock.Name = string.Format ("btnDock_{0}", labelNoSpaces ?? string.Empty);
				btnClose.Name = string.Format ("btnClose_{0}", labelNoSpaces ?? string.Empty);
				realLabel = GettextCatalog.GetString ("Close {0}", label);
				realHelp = GettextCatalog.GetString ("Close the {0} pad", label);
			} else {
				labelWidget.Visible = false;
				realLabel = GettextCatalog.GetString ("Close pad");
				realHelp = GettextCatalog.GetString ("Close the pad");
			}

			btnClose.SetCommonAttributes (null, realLabel, realHelp);

			if (label != null) {
				this.SetCommonAttributes ($"DockTab.{labelNoSpaces}", label, GettextCatalog.GetString ("Switch to the {0} tab", label));
				this.SetTitle (label);
			}

			minWidth = tabIcon.GetNaturalRequisition ().Width + btnBox.GetNaturalRequisition ().Width + 10;

			UpdateBehavior ();
			UpdateVisualStyle ();
		}

		void UpdateDockButtonAccessibilityLabels ()
		{
			string realLabel;
			string realHelp;
			bool dockable = item.Status != DockItemStatus.Dockable;

			if (string.IsNullOrEmpty (label)) {
				if (dockable) {
					realLabel = GettextCatalog.GetString ("Dock pad");
					realHelp = GettextCatalog.GetString ("Dock the pad into the UI so it will not hide automatically");
				} else {
					realLabel = GettextCatalog.GetString ("Autohide pad");
					realHelp = GettextCatalog.GetString ("Automatically hide the pad when it loses focus");
				}
			} else {
				if (dockable) {
					realLabel = GettextCatalog.GetString ("Dock {0}", label);
					realHelp = GettextCatalog.GetString ("Dock the {0} pad into the UI so it will not hide automatically", label);
				} else {
					realLabel = GettextCatalog.GetString ("Autohide {0}", label);
					realHelp = GettextCatalog.GetString ("Automatically hide the {0} pad when it loses focus", label);
				}
			}
			btnDock.SetCommonAttributes (null, realLabel, realHelp);
		}

		void OnClickDock (object s, EventArgs a)
		{
			if (item.Status == DockItemStatus.AutoHide || item.Status == DockItemStatus.Floating)
				item.Status = DockItemStatus.Dockable;
			else
				item.Status = DockItemStatus.AutoHide;
		}

		public int LabelWidth {
			get { return labelWidth; }
		}

		public int MinWidth {
			get { return minWidth; }
		}

		public bool Active {
			get {
				return active;
			}
			set {
				if (active != value) {
					active = value;
					UpdateVisualStyle ();
					QueueResize ();
					QueueDraw ();
					UpdateBehavior ();
				}
			}
		}

		public Widget Page {
			get {
				return page;
			}
		}

		public void UpdateBehavior ()
		{
			if (btnClose == null)
				return;

			btnClose.Visible = (item.Behavior & DockItemBehavior.CantClose) == 0;
			btnDock.Visible = (item.Behavior & DockItemBehavior.CantAutoHide) == 0;

			if (active || mouseOver) {
				if (btnClose.Image == null)
					btnClose.Image = pixClose;
				if (item.Status == DockItemStatus.AutoHide || item.Status == DockItemStatus.Floating) {
					btnDock.Image = pixDock;
					btnDock.TooltipText = GettextCatalog.GetString ("Dock");
				} else {
					btnDock.Image = pixAutoHide;
					btnDock.TooltipText = GettextCatalog.GetString ("Auto Hide");
				}
			} else {
				btnDock.Image = null;
				btnClose.Image = null;
			}

			UpdateDockButtonAccessibilityLabels ();
		}

		bool tabPressed, tabActivated;
		double pressX, pressY;

		// The dock and close buttons handle their own clicks
		bool IsOverButton (double x, double y)
		{
			return DockUtil.IsOverWidget (this, btnDock, x, y) || DockUtil.IsOverWidget (this, btnClose, x, y);
		}

		void OnButtonPress (ClickInfo evnt)
		{
			if (IsOverButton (evnt.X, evnt.Y))
				return;

			if (evnt.TriggersContextMenu ()) {
				item.ShowDockPopupMenu (this, evnt);
			} else if (evnt.Button == 1) {
				if (evnt.NPress == 1) {
					tabPressed = true;
					pressX = evnt.X;
					pressY = evnt.Y;
					TabPressed?.Invoke (this, EventArgs.Empty);
				} else if (evnt.NPress == 2) {
					tabActivated = true;
				}
			}
		}

		void OnButtonRelease (ClickInfo evnt)
		{
			if (tabActivated) {
				tabActivated = false;
				if (!item.Behavior.HasFlag (DockItemBehavior.CantAutoHide)) {
					if (item.Status == DockItemStatus.AutoHide)
						item.Status = DockItemStatus.Dockable;
					else
						item.Status = DockItemStatus.AutoHide;
				}
			}
			tabPressed = false;
		}

		void OnFocusIn ()
		{
			mouseOver = true;
			UpdateBehavior ();
			QueueDraw ();
		}

		void OnFocusOut ()
		{
			if (currentFocus == null) {
				mouseOver = false;
				UpdateBehavior ();
			}
			QueueDraw ();
		}

		void HandlePress (object sender, EventArgs args)
		{
			TabPressed?.Invoke (this, EventArgs.Empty);
		}

		void HandleShowMenu (object sender, EventArgs args)
		{
			// Show the menu at the middle of the widget
			item.ShowDockPopupMenu (this, Width / 2, Height / 2);
		}

		// Drag of the tab: moves the pad using the dock placeholder

		void OnDragBegin (double x, double y)
		{
			pressX = x;
			pressY = y;
			// Presses on the buttons or with other buttons than the first one are handled elsewhere
			tabPressed = !IsOverButton (x, y);
		}

		void OnDragUpdate (double offsetX, double offsetY)
		{
			if (tabPressed && !item.Behavior.HasFlag (DockItemBehavior.NoGrip) && Math.Abs (offsetX) > 3 && Math.Abs (offsetY) > 3) {
				frame.ShowPlaceholder (item);
				CursorFromName = "move";
				StartKeyTracking ();
				allowPlaceholderDocking = true;
				placeholderActive = true;
				tabPressed = false;
			}
			if (placeholderActive)
				UpdatePlaceholder (pressX + offsetX, pressY + offsetY);
		}

		void OnDragEnd (double offsetX, double offsetY)
		{
			if (placeholderActive) {
				placeholderActive = false;
				frame.DockInPlaceholder (item);
				frame.HidePlaceholder ();
				Cursor = null;
				EndKeyTracking ();
			}
			tabPressed = false;
		}

		double lastPointerX, lastPointerY;

		void UpdatePlaceholder (double x, double y)
		{
			lastPointerX = x;
			lastPointerY = y;
			var container = frame.Container;
			if (!TranslateCoordinates (container, x, y, out double cx, out double cy))
				return;
			frame.UpdatePlaceholder (item, new Gdk.Size (Width, Height), allowPlaceholderDocking, (int)cx, (int)cy);
		}

		// While a pad is dragged, Ctrl disables docking and Escape cancels the drag.
		void StartKeyTracking ()
		{
			EndKeyTracking ();
			dragKeyWindow = frame.GetToplevelWindow ();
			if (dragKeyWindow == null)
				return;
			dragKeyController = dragKeyWindow.AddKeyPressHandler (HeaderKeyPress, HeaderKeyRelease, PropagationPhase.Capture);
		}

		void EndKeyTracking ()
		{
			if (dragKeyController != null && dragKeyWindow != null)
				dragKeyWindow.RemoveController (dragKeyController);
			dragKeyController = null;
			dragKeyWindow = null;
		}

		bool HeaderKeyPress (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			var key = (Gdk.Key)keyval;
			if (key == Gdk.Key.Control_L || key == Gdk.Key.Control_R) {
				allowPlaceholderDocking = false;
				UpdatePlaceholder (lastPointerX, lastPointerY);
				return false;
			}
			if (key == Gdk.Key.Escape) {
				placeholderActive = false;
				frame.HidePlaceholder ();
				Cursor = null;
				EndKeyTracking ();
				return true;
			}
			return false;
		}

		void HeaderKeyRelease (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			var key = (Gdk.Key)keyval;
			if (key == Gdk.Key.Control_L || key == Gdk.Key.Control_R) {
				allowPlaceholderDocking = true;
				UpdatePlaceholder (lastPointerX, lastPointerY);
			}
		}

		void OnLeave ()
		{
			mouseOver = false;
			UpdateBehavior ();
		}

		enum FocusWidget
		{
			None,
			Widget,
			DockButton,
			CloseButton
		};

		bool FocusCurrentWidget (DirectionType direction)
		{
			if (currentFocus == null) {
				return false;
			}

			return currentFocus.ChildFocus (direction);
		}

		bool MoveFocusToWidget (FocusWidget widget, DirectionType direction)
		{
			switch (widget) {
			case FocusWidget.Widget:
				GrabFocus ();
				currentFocus = null;
				return true;

			case FocusWidget.DockButton:
				currentFocus = btnDock;
				return btnDock.ChildFocus (direction);

			case FocusWidget.CloseButton:
				currentFocus = btnClose;
				return btnClose.ChildFocus (direction);

			case FocusWidget.None:
				break;
			}

			return false;
		}

		FocusWidget GetNextWidgetToFocus (FocusWidget widget, DirectionType direction)
		{
			FocusWidget nextSite;

			switch (widget) {
			case FocusWidget.CloseButton:
				switch (direction) {
				case DirectionType.TabForward:
				case DirectionType.Right:
				case DirectionType.Down:
					return FocusWidget.None;

				case DirectionType.TabBackward:
				case DirectionType.Left:
				case DirectionType.Up:
					if (btnDock.Image != null) {
						nextSite = FocusWidget.DockButton;
					} else {
						nextSite = FocusWidget.Widget;
					}
					return nextSite;
				}

				break;

			case FocusWidget.DockButton:
				switch (direction) {
				case DirectionType.TabForward:
				case DirectionType.Right:
				case DirectionType.Down:
					return btnClose.Image == null ? FocusWidget.None : FocusWidget.CloseButton;

				case DirectionType.TabBackward:
				case DirectionType.Left:
				case DirectionType.Up:
					return FocusWidget.Widget;
				}

				break;

			case FocusWidget.Widget:
				switch (direction) {
				case DirectionType.TabForward:
				case DirectionType.Right:
				case DirectionType.Down:
					if (btnDock.Image != null) {
						nextSite = FocusWidget.DockButton;
					} else if (btnClose.Image != null) {
						nextSite = FocusWidget.CloseButton;
					} else {
						nextSite = FocusWidget.None;
					}
					return nextSite;

				case DirectionType.TabBackward:
				case DirectionType.Left:
				case DirectionType.Up:
					return FocusWidget.None;
				}

				break;
			case FocusWidget.None:
				switch (direction) {
				case DirectionType.TabForward:
				case DirectionType.Right:
				case DirectionType.Down:
					return FocusWidget.Widget;

				case DirectionType.TabBackward:
				case DirectionType.Left:
				case DirectionType.Up:
					if (btnClose.Image != null) {
						nextSite = FocusWidget.CloseButton;
					} else if (btnDock.Image != null) {
						nextSite = FocusWidget.DockButton;
					} else {
						nextSite = FocusWidget.Widget;
					}
					return nextSite;
				}

				break;
			}

			return FocusWidget.None;
		}

		protected override bool OnFocus (DirectionType direction)
		{
			if (!FocusCurrentWidget (direction)) {
				FocusWidget focus = FocusWidget.None;

				if (currentFocus == btnClose) {
					focus = FocusWidget.CloseButton;
				} else if (currentFocus == btnDock) {
					focus = FocusWidget.DockButton;
				} else if (IsFocus) {
					focus = FocusWidget.Widget;
				}

				while ((focus = GetNextWidgetToFocus (focus, direction)) != FocusWidget.None) {
					if (MoveFocusToWidget (focus, direction)) {
						return true;
					}
				}

				// Clean up the icons because the focus-out handler has already been called
				// so we need the icons to hide again
				mouseOver = false;
				UpdateBehavior ();

				currentFocus = null;
				return false;
			}

			return true;
		}

		protected override void OnRealized ()
		{
			base.OnRealized ();
			UpdateVisualStyle ();
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum_baseline = natural_baseline = -1;
			if (Child == null) {
				minimum = natural = 0;
				return;
			}

			int leftRight = (int)(TabPadding.Left + TabPadding.Right);
			int topBottom = active ? (int)(TabActivePadding.Top + TabActivePadding.Bottom) : (int)(TabPadding.Top + TabPadding.Bottom);
			if (orientation == Gtk.Orientation.Horizontal) {
				MeasureChild (Child, orientation, -1, out minimum, out natural);
				minimum += leftRight;
				natural += leftRight;
			} else {
				MeasureChild (Child, orientation, -1, out minimum, out natural);
				minimum += topBottom;
				natural += topBottom;
			}
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			int leftPadding = (int)TabPadding.Left;
			int rightPadding = (int)TabPadding.Right;

			var rect = new Gdk.Rectangle (0, 0, width, height);
			rect.X += leftPadding;
			rect.Width -= leftPadding + rightPadding;
			if (rect.Width < 1) {
				rect.Width = 1;
			}

			if (Child != null) {
				var bottomPadding = active ? (int)TabActivePadding.Bottom : (int)TabPadding.Bottom;
				var topPadding = active ? (int)TabActivePadding.Top : (int)TabPadding.Top;
				int centerY = topPadding + ((height - bottomPadding - topPadding) / 2);
				MeasureChild (Child, Gtk.Orientation.Vertical, -1, out int childHeight, out _);
				rect.Y += centerY - (childHeight / 2);
				rect.Height = childHeight;
				AllocateChild (Child, rect);
			}
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0 && visualStyle != null) {
				snapshot.DrawCairo (this, ctx => {
					if (visualStyle.TabStyle == DockTabStyle.Normal)
						DrawAsBrowser (ctx, width, height);
					else
						DrawNormal (ctx, width, height);

					if (HasFocus && labelWidget != null && labelWidget.TranslateCoordinates (this, 0, 0, out double lx, out double ly)) {
						DockUtil.DrawFocusRectangle (ctx, new Gdk.Rectangle ((int)lx, (int)ly, labelWidget.Width, labelWidget.Height), this.GetForegroundColor ());
					}
				});
			}
			base.OnSnapshot (snapshot);
		}

		void DrawAsBrowser (Cairo.Context ctx, int width, int height)
		{
			bool first = true;
			bool last = true;

			if (Parent is TabStrip) {
				var tsb = (TabStrip) Parent;
				first = tsb.IsFirstTab (this);
				last = tsb.IsLastTab (this);
			}

			if (first && last) {
				ctx.Rectangle (0, 0, width, height);
				ctx.SetSourceColor (VisualStyle.PadBackgroundColor.Value.ToCairoColor ());
				ctx.Fill ();
			} else {
				var image = Active ? dockTabActiveBackImage : dockTabBackImage;
				image = image.WithSize (width, height);

				ctx.DrawImage (this, image, 0, 0);
			}
		}

		void DrawNormal (Cairo.Context ctx, int width, int height)
		{
			ctx.Rectangle (0, 1, width, height - 1);
			ctx.SetSourceColor (Styles.DockBarBackground.ToCairoColor ());
			ctx.Fill ();
		}
	}
}
