//
// WelcomePageBarButton.cs
//
// Author:
//       lluis <${AuthorEmail}>
//
// Copyright (c) 2012 lluis
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
using System;
using Gtk;
using MonoDevelop.Core;
using System.Xml.Linq;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Ide.WelcomePage
{
	public class WelcomePageBarButton: BinWidget
	{
		Xwt.ImageView image;
		Widget imageWidget;
		Gtk.Label label;

		Xwt.Drawing.Image imageNormal, imageHover;
		bool mouseOver;
		string actionLink;
		static Gdk.Cursor hand_cursor;

		public string FontFamily { get; set; }
		public string HoverColor { get; set; }
		public string Color { get; set; }
		public int FontSize { get; set; }
		protected string Text { get; set; }
		protected Pango.Weight FontWeight { get; set; }
		protected bool Bold { 
			get { return FontWeight == Pango.Weight.Bold; }
			set { FontWeight = value ? Pango.Weight.Bold : Pango.Weight.Normal; }
		}
		Gtk.Box box = new Gtk.Box (Gtk.Orientation.Horizontal, 0);

		public int IconTextSpacing {
			get { return box.Spacing; }
			set { box.Spacing = value; }
		}

		public bool MouseOver {
			get {
				return mouseOver;
			}
		}

		/// <summary>
		/// If false the window button isn't inserted into the page bar.
		/// </summary>
		public virtual bool IsVisible { 
			get { return true; }
		}

		public WelcomePageBarButton (string title, string href, string iconResource = null)
		{
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += HandlePress;

			this.SetRole (AtkCocoa.Roles.AXLink);

			this.SetTitle (title);
			if (!string.IsNullOrEmpty (href)) {
				this.SetUrl (href);
			}
			this.SetCommonAttributes (null, null, "Opens the link in a web browser");

			UpdateStyle ();

			this.Text = title;
			this.actionLink = href;
			if (!string.IsNullOrEmpty (iconResource)) {
				imageHover = Xwt.Drawing.Image.FromResource (iconResource);
				imageNormal = imageHover.WithAlpha (0.7);
			}

			box.SetShouldIgnore (true);

			IconTextSpacing = Styles.WelcomeScreen.Links.IconTextSpacing;
			image = new Xwt.ImageView ();
			label = CreateLabel ();
			imageWidget = image.ToGtkWidget ();

			label.SetShouldIgnore (true);
			imageWidget.SetShouldIgnore (true);

			box.Append (imageWidget);
			if (imageNormal == null)
				imageWidget.Visible = false;
			box.Append (label);
			Child = box;

			Update ();

			this.AddMotionHandler (null, (x, y) => {
				if (hand_cursor == null)
					hand_cursor = new Gdk.Cursor ("pointer", null);
				Cursor = hand_cursor;
				mouseOver = true;
				Update ();
			}, () => {
				Cursor = null;
				mouseOver = false;
				Update ();
			});
			this.AddClickHandler (null, click => {
				if (click.Button == 1 && click.X >= 0 && click.Y >= 0 && click.X < Width && click.Y < Height)
					OnClicked ();
			}, 1);
		}

		protected override void OnRoot ()
		{
			base.OnRoot ();
			Gui.Styles.Changed -= UpdateStyle;
			Gui.Styles.Changed += UpdateStyle;
		}

		protected override void OnUnroot ()
		{
			Gui.Styles.Changed -= UpdateStyle;
			base.OnUnroot ();
		}

		void UpdateStyle (object sender = null, EventArgs e = null)
		{
			OnUpdateStyle ();
			if (label != null) {
				box.Remove (label);
				label = CreateLabel ();
				box.Append (label);
				Update ();
			}
			QueueResize ();
		}

		protected virtual void OnUpdateStyle ()
		{
			FontFamily = Platform.IsMac ? Styles.WelcomeScreen.FontFamilyMac : Styles.WelcomeScreen.FontFamilyWindows;
			HoverColor = Styles.WelcomeScreen.Links.HoverColor;
			Color = Styles.WelcomeScreen.Links.Color;
			FontSize = Styles.WelcomeScreen.Links.FontSize;
			FontWeight = Pango.Weight.Bold;
		}

		protected virtual Label CreateLabel ()
		{
			return new Label ();
		}

		protected void SetImage (Xwt.Drawing.Image normal, Xwt.Drawing.Image hover)
		{
			imageHover = hover;
			imageNormal = normal;

			imageWidget.Visible = imageNormal != null;

			Update ();
		}

		void HandlePress (object sender, EventArgs args)
		{
			OnClicked ();
		}

		protected virtual void OnClicked ()
		{
			WelcomePageSection.DispatchLink (actionLink);
		}

		protected void Update ()
		{
			if (imageNormal != null)
				image.Image = mouseOver ? imageHover : imageNormal;
			var color = mouseOver ? HoverColor : Color;
			label.Markup = WelcomePageSection.FormatText (FontFamily, FontSize, FontWeight, color, Text);
		}
	}
}
