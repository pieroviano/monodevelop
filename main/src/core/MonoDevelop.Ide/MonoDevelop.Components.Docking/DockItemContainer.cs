//
// DockItemContainer.cs
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
using Gtk;

using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components.Docking
{
	class DockItemContainer: BinWidget
	{
		DockItem item;
		Widget widget;
		CustomFrame borderFrame;
		Box contentBox;
		Box mainBox;

		public DockItemContainer (DockFrame frame, DockItem item)
		{
			this.item = item;
			item.LabelChanged += UpdateAccessibilityLabel;

			mainBox = new Box (Orientation.Vertical, 0);
			mainBox.SetShouldIgnore (false);
			UpdateAccessibilityLabel (null, null);
			Child = mainBox;

			mainBox.Append (item.GetToolbar (DockPositionType.Top).Container);

			var hbox = new Box (Orientation.Horizontal, 0);
			hbox.SetTitle ("Hbox");
			hbox.Vexpand = true;
			hbox.Append (item.GetToolbar (DockPositionType.Left).Container);

			contentBox = new Box (Orientation.Horizontal, 0);
			hbox.SetTitle ("Content");
			contentBox.Hexpand = true;
			contentBox.Vexpand = true;
			hbox.Append (contentBox);

			hbox.Append (item.GetToolbar (DockPositionType.Right).Container);

			mainBox.Append (hbox);

			mainBox.Append (item.GetToolbar (DockPositionType.Bottom).Container);
		}

		void UpdateAccessibilityLabel (object sender, EventArgs args)
		{
			mainBox.SetTitle (Core.GettextCatalog.GetString ("{0} Pad", item.Label));
		}

		DockVisualStyle visualStyle;

		public DockVisualStyle VisualStyle {
			get { return visualStyle; }
			set { visualStyle = value; UpdateVisualStyle (); }
		}

		/// <summary>Raised when the container is added to or removed from a parent widget.</summary>
		internal event EventHandler ParentChanged;

		internal void NotifyParentChanged ()
		{
			ParentChanged?.Invoke (this, EventArgs.Empty);
		}

		void OnClickDock (object s, EventArgs a)
		{
			if (item.Status == DockItemStatus.AutoHide || item.Status == DockItemStatus.Floating)
				item.Status = DockItemStatus.Dockable;
			else
				item.Status = DockItemStatus.AutoHide;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				item.LabelChanged -= UpdateAccessibilityLabel;
			base.Dispose (disposing);
		}

		public void UpdateContent ()
		{
			if (widget != null)
				DockUtil.RemoveFromParent (widget);
			widget = item.Content;

			if (item.DrawFrame) {
				if (borderFrame == null) {
					borderFrame = new CustomFrame (1, 1, 1, 1);
					borderFrame.Hexpand = borderFrame.Vexpand = true;
					contentBox.Append (borderFrame);
				}
				if (widget != null) {
					widget.Hexpand = widget.Vexpand = true;
					borderFrame.Child = widget;
				}
			}
			else if (widget != null) {
				if (borderFrame != null) {
					contentBox.Remove (borderFrame);
					borderFrame = null;
				}
				widget.Hexpand = widget.Vexpand = true;
				contentBox.Append (widget);
			}
			UpdateVisualStyle ();
		}

		void UpdateVisualStyle ()
		{
			if (VisualStyle != null) {
				if (widget != null)
					SetTreeStyle (widget);

				item.GetToolbar (DockPositionType.Top).SetStyle (VisualStyle);
				item.GetToolbar (DockPositionType.Left).SetStyle (VisualStyle);
				item.GetToolbar (DockPositionType.Right).SetStyle (VisualStyle);
				item.GetToolbar (DockPositionType.Bottom).SetStyle (VisualStyle);

				QueueDraw ();
			}
		}

		void SetTreeStyle (Gtk.Widget w)
		{
			if (w is Gtk.TreeView) {
				if (w.IsRealized)
					OnTreeRealized (w, null);
				else
					w.Realized += OnTreeRealized;
			}
			else {
				foreach (var cw in w.GetChildren ())
					SetTreeStyle (cw);
			}
		}

		void OnTreeRealized (object sender, EventArgs e)
		{
			var w = (Gtk.TreeView)sender;
			// A null colour restores the theme's tree background
			DockUtil.SetBackground (w, VisualStyle.TreeBackgroundColor);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (VisualStyle?.TabStyle == DockTabStyle.Normal && VisualStyle.PadBackgroundColor != null) {
				var color = VisualStyle.PadBackgroundColor.Value.ToCairoColor ();
				snapshot.DrawCairo (this, cr => {
					cr.SetSourceColor (color);
					cr.Rectangle (0, 0, Width, Height);
					cr.Fill ();
				});
			}
			base.OnSnapshot (snapshot);
		}
	}

	/// <summary>
	/// Single-child container with a border (margins) and inner padding; set the content with <see cref="BinWidget.Child"/>.
	/// </summary>
	class CustomFrame: BinWidget
	{
		int topMargin;
		int bottomMargin;
		int leftMargin;
		int rightMargin;

		int topPadding;
		int bottomPadding;
		int leftPadding;
		int rightPadding;

		Gdk.RGBA backgroundColor;
		Gdk.RGBA borderColor;
		bool backgroundColorSet, borderColorSet;

		public CustomFrame ()
		{
		}

		public CustomFrame (int topMargin, int bottomMargin, int leftMargin, int rightMargin)
		{
			SetMargins (topMargin, bottomMargin, leftMargin, rightMargin);
		}

		public void SetMargins (int topMargin, int bottomMargin, int leftMargin, int rightMargin)
		{
			this.topMargin = topMargin;
			this.bottomMargin = bottomMargin;
			this.leftMargin = leftMargin;
			this.rightMargin = rightMargin;
			UpdatePadding ();
		}

		public void SetPadding (int topPadding, int bottomPadding, int leftPadding, int rightPadding)
		{
			this.topPadding = topPadding;
			this.bottomPadding = bottomPadding;
			this.leftPadding = leftPadding;
			this.rightPadding = rightPadding;
			UpdatePadding ();
		}

		void UpdatePadding ()
		{
			Padding = new Gtk.Border {
				Left = (short)(leftMargin + leftPadding),
				Right = (short)(rightMargin + rightPadding),
				Top = (short)(topMargin + topPadding),
				Bottom = (short)(bottomMargin + bottomPadding),
			};
		}

		public bool GradientBackround { get; set; }

		public Gdk.RGBA BackgroundColor {
			get { return backgroundColor; }
			set { backgroundColor = value; backgroundColorSet = true; QueueDraw (); }
		}

		public Gdk.RGBA BorderColor {
			get { return borderColorSet ? borderColor : DefaultBorderColor; }
			set { borderColor = value; borderColorSet = true; QueueDraw (); }
		}

		Gdk.RGBA DefaultBorderColor {
			get {
				// Derived from the theme's text colour (replaces Style.Dark (StateType.Normal))
				var fg = StyleColor;
				return new Gdk.RGBA { Red = fg.Red, Green = fg.Green, Blue = fg.Blue, Alpha = 0.35f };
			}
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width <= 0 || height <= 0)
				return;

			if (backgroundColorSet) {
				var bcolor = backgroundColor.ToCairoColor ();
				snapshot.DrawCairo (this, cr => {
					if (GradientBackround) {
						// FIXME: VV: Remove gradient features
						using (Cairo.Gradient pat = new Cairo.LinearGradient (0, 0, 0, height)) {
							pat.AddColorStop (0, bcolor);
							Xwt.Drawing.Color gcol = bcolor.ToXwtColor ();
							gcol.Light -= 0.1;
							if (gcol.Light < 0)
								gcol.Light = 0;
							pat.AddColorStop (1, gcol.ToCairoColor ());
							cr.Rectangle (0, 0, width, height);
							cr.SetSource (pat);
							cr.Fill ();
						}
					} else {
						cr.Rectangle (0, 0, width, height);
						cr.SetSourceColor (bcolor);
						cr.Fill ();
					}
				});
			}

			base.OnSnapshot (snapshot);

			var border = BorderColor.ToCairoColor ();
			snapshot.DrawCairo (this, cr => {
				cr.SetSourceColor (border);

				if (topMargin > 0) {
					double y = topMargin / 2d;
					cr.LineWidth = topMargin;
					cr.MoveTo (0, y);
					cr.LineTo (width, y);
					cr.Stroke ();
				}

				if (bottomMargin > 0) {
					double y = height - bottomMargin / 2d;
					cr.LineWidth = bottomMargin;
					cr.MoveTo (0, y);
					cr.LineTo (width, y);
					cr.Stroke ();
				}

				if (leftMargin > 0) {
					double x = leftMargin / 2d;
					cr.LineWidth = leftMargin;
					cr.MoveTo (x, 0);
					cr.LineTo (x, height);
					cr.Stroke ();
				}

				if (rightMargin > 0) {
					double x = width - rightMargin / 2d;
					cr.LineWidth = rightMargin;
					cr.MoveTo (x, 0);
					cr.LineTo (x, height);
					cr.Stroke ();
				}
			});
		}
	}
}
