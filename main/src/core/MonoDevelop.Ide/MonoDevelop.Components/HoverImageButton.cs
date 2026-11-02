/* **************************************************************************
 *  HoverImageButton.cs
 *
 *  Copyright (C) 2007 Novell, Inc.
 *  Written by Aaron Bockover <abockover@novell.com>
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),
 *  to deal in the Software without restriction, including without limitation
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,
 *  and/or sell copies of the Software, and to permit persons to whom the
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 *  DEALINGS IN THE SOFTWARE.
 */

using System;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components
{
	class HoverImageButton : BinWidget
	{
		static Gdk.Cursor hand_cursor;

		StockIconSize icon_size = StockIconSize.Menu;
		string [] icon_names = { "image-missing" };
		Xwt.Drawing.Image normal_pixbuf;
		Xwt.Drawing.Image active_pixbuf;
		ImageView image;
		bool is_hovering;
		bool is_pressed;

		bool draw_focus = true;

		event EventHandler clicked;

		public event EventHandler Clicked {
			add { clicked += value; }
			remove { clicked -= value; }
		}

		public HoverImageButton ()
		{
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += OnPerformPress;

			this.SetRole (AtkCocoa.Roles.AXButton);

			Focusable = true;
			CanFocus = true;
			image = new ImageView () {
				Halign = Gtk.Align.Center,
				Valign = Gtk.Align.Center,
			};
			image.SetShouldIgnore (true);
			Child = image;

			if (hand_cursor == null)
				hand_cursor = new Gdk.Cursor ("pointer", null);

			this.AddMotionHandler (null, (x, y) => {
				Cursor = hand_cursor;
				is_hovering = true;
				UpdateImage ();
			}, () => {
				Cursor = null;
				is_hovering = false;
				UpdateImage ();
			});
			this.AddFocusHandler (() => { UpdateImage (); QueueDraw (); }, () => { UpdateImage (); QueueDraw (); });
			this.AddClickHandler (OnPressed, OnReleased, 1);
		}

		public HoverImageButton (StockIconSize size, string icon_name) : this (size, new string [] { icon_name })
		{
		}

		public HoverImageButton (StockIconSize size, params string [] icon_names) : this ()
		{
			this.icon_size = size;
			this.icon_names = icon_names;
		}

		public new void Activate ()
		{
			EventHandler handler = clicked;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}

		void OnPerformPress (object sender, EventArgs args)
		{
			Activate ();
		}

		protected override void OnRealized ()
		{
			base.OnRealized ();
			if (normal_pixbuf == null)
				LoadPixbufs ();
		}

		void OnPressed (ClickInfo click)
		{
			if (CanFocus)
				GrabFocus ();
			is_pressed = true;
			QueueDraw ();
		}

		void OnReleased (ClickInfo click)
		{
			is_pressed = false;
			QueueDraw ();
			Activate ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot);

			if (HasFocus && draw_focus) {
				snapshot.DrawCairo (this, cr => {
					cr.Rectangle (0.5, 0.5, Width - 1, Height - 1);
					cr.SetSourceColor (Styles.FocusColor.ToCairoColor ());
					cr.LineWidth = 1;
					cr.SetDash (new double [] { 1, 1 }, 0.5);
					cr.Stroke ();
				});
			}
		}

		void UpdateImage ()
		{
			image.Image = is_hovering || is_pressed || HasFocus
				? active_pixbuf : normal_pixbuf;
		}

		void LoadPixbufs ()
		{
			if (normal_pixbuf != null) {
				normal_pixbuf.Dispose ();
				normal_pixbuf = null;
			}

			if (active_pixbuf != null) {
				active_pixbuf.Dispose ();
				active_pixbuf = null;
			}

			for (int i = 0; i < icon_names.Length; i++) {
				try {
					normal_pixbuf = ImageService.GetIcon (icon_names[i], icon_size);
					active_pixbuf = normal_pixbuf;
					break;
				} catch {
				}
			}

			UpdateImage ();
		}

		public Xwt.Drawing.Image Pixbuf {
			get { return this.normal_pixbuf; }
			set {
				this.normal_pixbuf = value;
				active_pixbuf = normal_pixbuf;
				UpdateImage ();
			}
		}

		public string [] IconNames {
			get { return icon_names; }
			set {
				icon_names = value;
				LoadPixbufs ();
			}
		}

		public StockIconSize IconSize {
			get { return icon_size; }
			set {
				icon_size = value;
				LoadPixbufs ();
			}
		}

		public ImageView Image {
			get { return image; }
		}

		public bool DrawFocus {
			get { return draw_focus; }
			set {
				draw_focus = value;
				QueueDraw ();
			}
		}
	}
}
