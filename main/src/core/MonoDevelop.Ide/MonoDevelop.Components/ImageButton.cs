//
// ImageButton.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components
{
	public class ImageButton: BinWidget
	{
		Xwt.Drawing.Image image;
		Xwt.Drawing.Image inactiveImage;
		ImageView imageWidget;
		bool hasInactiveImage;
		bool hover;
		bool pressed;

		public ImageButton ()
		{
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += HandlePress;

			this.SetRole (AtkCocoa.Roles.AXButton);

			imageWidget = new ImageView ();
			imageWidget.SetShouldIgnore (true);
			Child = imageWidget;

			this.AddMotionHandler (null, (x, y) => { hover = true; LoadImage (); }, () => { hover = false; LoadImage (); });
			this.AddClickHandler (OnPressed, OnReleased, 1);
			this.AddKeyPressHandler (OnKeyPressed, OnKeyReleased);
			this.AddFocusHandler (() => { hover = true; LoadImage (); QueueDraw (); }, () => { hover = false; LoadImage (); QueueDraw (); });
		}

		public Xwt.Drawing.Image Image {
			get { return image; }
			set {
				image = value;
				Xwt.Drawing.Image oldInactive = null;
				if (!hasInactiveImage) {
					oldInactive = inactiveImage;
					inactiveImage = image != null ? image.WithAlpha (0.5) : null;
				}
				LoadImage ();
				if (oldInactive != null)
					oldInactive.Dispose ();
			}
		}

		public Xwt.Drawing.Image InactiveImage {
			get { return hasInactiveImage ? inactiveImage : null; }
			set {
				if (!hasInactiveImage && inactiveImage != null)
					inactiveImage.Dispose ();
				hasInactiveImage = true;
				inactiveImage = value;
				LoadImage ();
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && !hasInactiveImage && inactiveImage != null) {
				inactiveImage.Dispose ();
				inactiveImage = null;
			}
			base.Dispose (disposing);
		}

		void LoadImage ()
		{
			if (image != null) {
				if (hover)
					imageWidget.Image = image;
				else
					imageWidget.Image = inactiveImage;
			} else {
				imageWidget.Image = null;
			}
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot);
			if (HasFocus && image != null) {
				snapshot.DrawCairo (this, cr => {
					cr.Rectangle (0.5, 0.5, Width - 1, Height - 1);
					cr.SetSourceColor (Styles.FocusColor.ToCairoColor ());
					cr.LineWidth = 1;
					cr.SetDash (new double [] { 1, 1 }, 0.5);
					cr.Stroke ();
				});
			}
		}

		void OnPressed (ClickInfo click)
		{
			pressed = image != null;
		}

		void OnReleased (ClickInfo click)
		{
			if (pressed && click.X >= 0 && click.Y >= 0 && click.X < Width && click.Y < Height) {
				click.Claim ();
				hover = false;
				LoadImage ();
				Clicked?.Invoke (this, EventArgs.Empty);
			}
		}

		bool OnKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			pressed = image != null;
			return false;
		}

		void OnKeyReleased (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			if (pressed && keyval == (uint)Gdk.Key.space) {
				LoadImage ();
				Clicked?.Invoke (this, EventArgs.Empty);
			}
		}

		void HandlePress (object o, EventArgs args)
		{
			Clicked?.Invoke (this, EventArgs.Empty);
		}

		public event EventHandler Clicked;
	}
}
