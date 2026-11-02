// 
// RoundButton.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
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
using System;
using MonoDevelop.Components;
using Cairo;
using MonoDevelop.Ide;
using System.Reflection;

namespace MonoDevelop.Components.MainToolbar
{
	class RoundButton : CairoWidget
	{
		const int height = 32;

		enum ButtonState
		{
			Normal,
			Prelight,
			Selected,
			Insensitive
		}

		Xwt.Drawing.Image btnNormal, btnInactive, btnHover, btnPressed;

		Xwt.Drawing.Image iconRunNormal, iconRunDisabled;
		Xwt.Drawing.Image iconStopNormal, iconStopDisabled;
		Xwt.Drawing.Image iconBuildNormal, iconBuildDisabled;

		bool hovering, pressed;

		public RoundButton ()
		{
			SetSizeRequest (height, height);

			btnNormal = Xwt.Drawing.Image.FromResource (GetType (), "btn-execute-normal-32.png");
			btnInactive = Xwt.Drawing.Image.FromResource (GetType (), "btn-execute-disabled-32.png");
			btnPressed = Xwt.Drawing.Image.FromResource (GetType (), "btn-execute-pressed-32.png");
			btnHover = Xwt.Drawing.Image.FromResource (GetType (), "btn-execute-hover-32.png");

			iconRunNormal = Xwt.Drawing.Image.FromResource (GetType (), "ico-execute-normal-32.png");
			iconRunDisabled = Xwt.Drawing.Image.FromResource (GetType (), "ico-execute-disabled-32.png");

			iconStopNormal = Xwt.Drawing.Image.FromResource (GetType (), "ico-stop-normal-32.png");
			iconStopDisabled = Xwt.Drawing.Image.FromResource (GetType (), "ico-stop-disabled-32.png");

			iconBuildNormal = Xwt.Drawing.Image.FromResource (GetType (), "ico-build-normal-32.png");
			iconBuildDisabled = Xwt.Drawing.Image.FromResource (GetType (), "ico-build-disabled-32.png");

			this.AddMotionHandler (
				(x, y) => SetHover (IsInside (x, y)),
				(x, y) => SetHover (IsInside (x, y)),
				() => {
					SetHover (false);
					pressed = false;
					QueueDraw ();
				});
			this.AddClickHandler (
				c => {
					if (c.Button == 1 && IsInside (c.X, c.Y)) {
						pressed = true;
						c.Claim ();
						QueueDraw ();
					}
				},
				c => {
					bool wasPressed = pressed;
					pressed = false;
					hovering = IsInside (c.X, c.Y);
					QueueDraw ();
					if (wasPressed)
						OnClicked (EventArgs.Empty);
				}, 1);
			// GTK 4 doesn't redraw on sensitivity changes of a custom drawn widget
			StateFlagsChanged += (o, args) => QueueDraw ();
		}

		void SetHover (bool value)
		{
			if (hovering != value) {
				hovering = value;
				QueueDraw ();
			}
		}

		ButtonState State {
			get {
				if (!Sensitive)
					return ButtonState.Insensitive;
				if (pressed)
					return ButtonState.Selected;
				return hovering ? ButtonState.Prelight : ButtonState.Normal;
			}
		}

		bool IsInside (double x, double y)
		{
			var xr = x - Width / 2;
			var yr = y - Height / 2;
			return Math.Sqrt (xr * xr + yr * yr) <= height / 2;
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = orientation == Gtk.Orientation.Horizontal ? (int) btnNormal.Size.Width : (int) btnNormal.Size.Height + 2;
			minimum_baseline = natural_baseline = -1;
		}

		Xwt.Drawing.Image GetIcon()
		{
			switch (icon) {
			case OperationIcon.Stop:
				return State == ButtonState.Insensitive ? iconStopDisabled : iconStopNormal;
			case OperationIcon.Run:
				return State == ButtonState.Insensitive ? iconRunDisabled : iconRunNormal;
			case OperationIcon.Build:
				return State == ButtonState.Insensitive ? iconBuildDisabled : iconBuildNormal;
			}
			throw new InvalidOperationException ();
		}

		OperationIcon icon;
		public OperationIcon Icon {
			get { return icon; }
			set {
				if (value != icon) {
					icon = value;
					QueueDraw ();
				}
			}
		}

		protected override void OnDraw (Cairo.Context context, int width, int height)
		{
			DrawBackground (context, new Gdk.Rectangle (0, 0, width, height), 15, State);
			var icon = GetIcon();
			context.DrawImage (this, icon, Math.Max (0, (width - icon.Width) / 2), Math.Max (0, (height - icon.Height) / 2));
		}

		void DrawBackground (Cairo.Context context, Gdk.Rectangle region, int radius, ButtonState state)
		{
			int x = region.X + region.Width / 2;
			int y = region.Y + region.Height / 2;

			var img = btnNormal;
			switch (state) {
			case ButtonState.Insensitive:
				img = btnInactive; break;
			case ButtonState.Prelight:
				img = btnHover; break;
			case ButtonState.Selected:
				img = btnPressed; break;
			}

			x -= (int) img.Width / 2;
			y -= (int) img.Height / 2;

			context.DrawImage (this, img, x, y);
		}

		public event EventHandler Clicked;

		protected virtual void OnClicked (EventArgs e)
		{
			EventHandler handler = this.Clicked;
			if (handler != null)
				handler (this, e);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (!disposing)
				return;

			DisposeImage (ref btnNormal);
			DisposeImage (ref btnInactive);
			DisposeImage (ref btnPressed);
			DisposeImage (ref btnHover);
			DisposeImage (ref iconRunNormal);
			DisposeImage (ref iconRunDisabled);
			DisposeImage (ref iconStopNormal);
			DisposeImage (ref iconStopDisabled);
			DisposeImage (ref iconBuildNormal);
			DisposeImage (ref iconBuildDisabled);
		}

		static void DisposeImage (ref Xwt.Drawing.Image image)
		{
			if (image != null) {
				image.Dispose ();
				image = null;
			}
		}
	}
}
