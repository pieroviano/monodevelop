// 
// ButtonBar.cs
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
using System.Collections.Generic;
using MonoDevelop.Components;
using Cairo;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace MonoDevelop.Components.MainToolbar
{
	class ButtonBar : CairoWidget
	{
		const int SeparatorSpacing = 6;
		List<IButtonBarButton> buttons = new List<IButtonBarButton> ();
		public IEnumerable<IButtonBarButton> Buttons {
			set {
				buttons = value.ToList ();
				foreach (var item in buttons) {
					item.EnabledChanged += (o, e) => QueueDraw ();
					item.ImageChanged += (o, e) => QueueDraw ();
					item.VisibleChanged += (o, e) => {
						visibleButtons = null;
						QueueResize ();
					};
				}
				QueueResize ();
			}
		}

		public IEnumerable<ButtonBarGroup> Groups {
			set {
				var buttonList = new List<IButtonBarButton> ();
				foreach (var g in value) {
					buttonList.AddRange (g.Buttons);
				}

				Buttons = buttonList;
			}
		}

		Dictionary<IButtonBarButton, Gdk.Rectangle> allocations = new Dictionary<IButtonBarButton, Gdk.Rectangle> ();
		IButtonBarButton[] visibleButtons;

		Xwt.Drawing.Image[] btnNormalOriginal;
		Xwt.Drawing.Image[] btnPressedOriginal;
		Xwt.Drawing.Image[] btnNormal;
		Xwt.Drawing.Image[] btnPressed;

		IButtonBarButton pushedButton;
		int currentImagesHeight;

		public ButtonBar ()
		{
			btnNormalOriginal = new Xwt.Drawing.Image[] {
				Xwt.Drawing.Image.FromResource ("btn-debug-base-left-cap-normal.png"),
				Xwt.Drawing.Image.FromResource ("btn-debug-base-middle-cap-normal.png"),
				Xwt.Drawing.Image.FromResource ("btn-debug-base-right-cap-normal.png")
			};

			btnPressedOriginal = new Xwt.Drawing.Image[] {
				Xwt.Drawing.Image.FromResource ("btn-debug-base-left-cap-pressed.png"),
				Xwt.Drawing.Image.FromResource ("btn-debug-base-middle-cap-pressed.png"),
				Xwt.Drawing.Image.FromResource ("btn-debug-base-right-cap-pressed.png")
			};
			btnNormal = new Xwt.Drawing.Image[btnNormalOriginal.Length];
			btnPressed = new Xwt.Drawing.Image[btnNormalOriginal.Length];
			HasTooltip = true;

			this.AddMotionHandler (null, null, () => {
				pushedButton = null;
				pressed = false;
				QueueDraw ();
			});
			this.AddClickHandler (OnButtonPressed, OnButtonReleased, 1);
			StateFlagsChanged += (o, args) => QueueDraw ();
		}

		void ScaleImages (int newHeight)
		{
			if (currentImagesHeight == newHeight)
				return;

			currentImagesHeight = newHeight;
			for (int n=0; n<btnNormalOriginal.Length; n++) {
				if (btnNormal[n] != null)
					btnNormal[n].Dispose ();
				btnNormal[n] = ExpandImageVertically (btnNormalOriginal[n], newHeight);
			}
			for (int n=0; n<btnPressedOriginal.Length; n++) {
				if (btnPressed[n] != null)
					btnPressed[n].Dispose ();
				btnPressed[n] = ExpandImageVertically (btnPressedOriginal[n], newHeight);
			}
		}

		IButtonBarButton[] VisibleButtons {
			get {
				if (visibleButtons == null)
					visibleButtons = buttons.Where (b => b.Visible || b.IsSeparator).ToArray ();
				return visibleButtons;
			}
		}

		bool pressed;

		void OnButtonPressed (ClickInfo click)
		{
			pushedButton = VisibleButtons.FirstOrDefault (b => allocations.TryGetValue (b, out var r) && r.Contains ((int)click.X, (int)click.Y));
			if (pushedButton != null && pushedButton.Enabled) {
				pressed = true;
				click.Claim ();
				QueueDraw ();
			}
		}

		void OnButtonReleased (ClickInfo click)
		{
			if (pressed && pushedButton != null) {
				pushedButton.NotifyPushed ();
			}
			pressed = false;
			pushedButton = null;
			QueueDraw ();
		}

		protected override bool OnQueryTooltip (int x, int y, bool keyboard_tooltip, Gtk.Tooltip tooltip)
		{
			var button = VisibleButtons.FirstOrDefault (b => allocations.TryGetValue (b, out var r) && r.Contains (x, y));
			if (button != null) {
				tooltip.Text = button.Tooltip;
				tooltip.TipArea = allocations [button];
				return true;
			} else {
				return false;
			}
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			if (orientation == Gtk.Orientation.Horizontal)
				minimum = VisibleButtons.Sum (b => b.Visible ? (!b.IsSeparator ? (int)btnNormalOriginal[0].Width : SeparatorSpacing) : 0);
			else
				minimum = (int)btnNormalOriginal[0].Height;
			natural = minimum;
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnDraw (Cairo.Context context, int width, int height)
		{
			ScaleImages (height);

			double x = 0, y = 0;
			for (int i = 0; i < VisibleButtons.Length; i++) {
				bool nextIsSeparator = (i < VisibleButtons.Length - 1 && VisibleButtons[i + 1].IsSeparator) || i == VisibleButtons.Length - 1;
				bool lastWasSeparator = (i > 0 && VisibleButtons[i - 1].IsSeparator) || i == 0;
				IButtonBarButton button = VisibleButtons [i];
				if (button.IsSeparator) {
					if (!lastWasSeparator)
						x += SeparatorSpacing;
					continue;
				}
				Xwt.Drawing.Image[] images = pressed && pushedButton == button ? btnPressed : btnNormal;
				Xwt.Drawing.Image img = images [lastWasSeparator ? 0 : nextIsSeparator ? 2 : 1];
				context.DrawImage (this, img, x, y);

				allocations [button] = new Gdk.Rectangle ((int)x, (int)y, (int)img.Width, (int)img.Height);

				var icon = ImageService.GetIcon (button.Image, StockIconSize.Menu);
				if (!Sensitive || !button.Enabled)
					icon = icon.WithAlpha (0.4);
				context.DrawImage (this, icon, Math.Truncate (x + (img.Width - icon.Width) / 2), Math.Truncate (y + (img.Height - icon.Height) / 2));
				x += img.Width;
			}
		}

		Xwt.Drawing.Image ExpandImageVertically (Xwt.Drawing.Image img, int newHeight)
		{
			if (newHeight <= img.Height)
				return img;

			using (Xwt.Drawing.ImageBuilder ib = new Xwt.Drawing.ImageBuilder (img.Width, newHeight))
			{
				int h1 = (int)img.Height / 2;
				int h2 = (int)img.Height - h1;

				var ctx = ib.Context;
				ctx.Save ();
				ctx.Rectangle (0, 0, img.Width, h1);
				ctx.Clip ();
				ctx.DrawImage (img, 0, 0);
				ctx.Restore ();

				ctx.Save ();
				ctx.Rectangle (0, newHeight - h2, img.Width, h2);
				ctx.Clip ();
				ctx.DrawImage (img, 0, newHeight - img.Height);
				ctx.Restore ();

				ctx.Rectangle (0, h1, img.Width, newHeight - h1 - h2);
				ctx.Clip ();
				ctx.DrawImage (img, 0, 0, img.Width, newHeight);

				return ib.ToVectorImage ();
			}
		}

		public sealed class ClickEventArgs : EventArgs
		{
			public int Button { get; private set; }

			public ClickEventArgs (int button)
			{
				this.Button = button;
			}
		}
	}
}

