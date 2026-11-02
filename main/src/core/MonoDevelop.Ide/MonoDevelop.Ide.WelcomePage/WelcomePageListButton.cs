//
// WelcomePageListButton.cs
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
using System.Collections.Generic;

using MonoDevelop.Core;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

using Gtk;

namespace MonoDevelop.Ide.WelcomePage
{
	public class WelcomePageListButton: CairoWidget
	{
		static Gdk.Cursor hand_cursor;
		string title, subtitle, actionUrl;
		protected Xwt.Drawing.Image icon;
		protected bool mouseOver;
		protected bool pinned;
		protected Gdk.Rectangle starRect;
		protected bool mouseOverStar;

		protected Pango.Weight TitleFontWeight { get; set; }

		static protected readonly Xwt.Drawing.Image starNormal;
		static protected readonly Xwt.Drawing.Image starNormalHover;
		static protected readonly Xwt.Drawing.Image starPinned;
		static protected readonly Xwt.Drawing.Image starPinnedHover;

		public event EventHandler PinClicked;

		public bool DrawLeftBorder { get; set; }
		public bool DrawRightBorder { get; set; }

		public int BorderPadding { get; set; }

		public int LeftTextPadding { get; set; }
		public int InternalPadding { get; set; }

		public string SmallTitleColor { get; set; }
		public string MediumTitleColor { get; set; }

		public string TitleFontFace { get; set; }
		public string SmallTitleFontFace { get; set; }

		public string HoverBackgroundColor { get; set; }
		public string HoverBorderColor { get; set; }

		public int TitleFontSize { get; set; }
		public int SmallTitleFontSize { get; set; }

		static WelcomePageListButton ()
		{
			starNormal = Xwt.Drawing.Image.FromResource ("unstar-16.png");
			starNormalHover = Xwt.Drawing.Image.FromResource ("unstar-hover-16.png");
			starPinned = Xwt.Drawing.Image.FromResource ("star-16.png");
			starPinnedHover = Xwt.Drawing.Image.FromResource ("star-hover-16.png");
		}

		ActionDelegate actionHandler;
		public WelcomePageListButton (string title, string subtitle, Xwt.Drawing.Image icon, string actionUrl)
		{
			actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += HandlePress;

			this.SetRole (AtkCocoa.Roles.AXButton);
			this.SetTitle (title);

			if (!actionUrl.StartsWith ("monodevelop://")) {
				this.SetCommonAttributes (null, null, string.Format ("Opens {0}", title));
			}

			this.title = title;
			this.subtitle = subtitle;
			this.icon = icon;
			this.actionUrl = actionUrl;

			WidthRequest = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.Width;
			HeightRequest = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.Height + 2;

			LeftTextPadding = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.TextLeftPadding;
			InternalPadding = Styles.WelcomeScreen.Pad.Padding;

			UpdateStyle ();

			UpdateActions ();

			this.AddMotionHandler ((x, y) => {
				var so = starRect.Contains ((int)x, (int)y);
				if (so != mouseOverStar) {
					mouseOverStar = so;
					QueueDraw ();
				}
			}, (x, y) => {
				if (hand_cursor == null)
					hand_cursor = new Gdk.Cursor ("pointer", null);
				Cursor = hand_cursor;
				mouseOver = true;
				if (AllowPinning) {
					this.SetAlternateUIVisible (true);
				}
				QueueDraw ();
			}, () => {
				Cursor = null;
				mouseOver = false;
				mouseOverStar = false;
				this.SetAlternateUIVisible (false);
				QueueDraw ();
			});
			this.AddClickHandler (null, click => {
				if (click.Button != 1)
					return;
				if (mouseOverStar && AllowPinning) {
					pinned = !pinned;
					QueueDraw ();
					PinClicked?.Invoke (this, EventArgs.Empty);
				} else if (mouseOver) {
					WelcomePageSection.DispatchLink (actionUrl);
				}
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

		void UpdateActions ()
		{
			// FIXME: Should the pinning star just be handled by an internal accessible element
			// rather than an alternate UI?
			if (AllowPinning) {
				actionHandler.PerformShowAlternateUI += HandleShowAlternateUI;
				actionHandler.PerformShowDefaultUI += HandleShowDefaultUI;
			} else {
				actionHandler.PerformShowAlternateUI -= HandleShowAlternateUI;
				actionHandler.PerformShowDefaultUI -= HandleShowDefaultUI;
			}
		}

		void UpdateStyle (object sender = null, EventArgs e = null)
		{
			OnUpdateStyle ();
			QueueDraw ();
		}

		protected virtual void OnUpdateStyle ()
		{
			SmallTitleColor = Styles.WelcomeScreen.Pad.SmallTitleColor;
			MediumTitleColor = Styles.WelcomeScreen.Pad.MediumTitleColor;

			TitleFontFace = Platform.IsMac ? Styles.WelcomeScreen.Pad.TitleFontFamilyMac : Styles.WelcomeScreen.Pad.TitleFontFamilyWindows;
			SmallTitleFontFace = Platform.IsMac ? Styles.WelcomeScreen.Pad.TitleFontFamilyMac : Styles.WelcomeScreen.Pad.TitleFontFamilyWindows;

			HoverBackgroundColor = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.HoverBackgroundColor;
			HoverBorderColor = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.HoverBorderColor;

			TitleFontSize = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.TitleFontSize;
			SmallTitleFontSize = Styles.WelcomeScreen.Pad.Solutions.SolutionTile.PathFontSize;
		}

		bool allowPinning;
		public bool AllowPinning {
			get {
				return allowPinning;
			}
			set {
				allowPinning = value;
				UpdateActions ();
			}
		}

		public bool Pinned {
			get { return pinned; }
			set {
				pinned = value;

				if (pinned) {
					this.SetTitle (string.Format ("{0}. This item is pinned", title));
				} else {
					this.SetTitle (title);
				}

				if (AllowPinning && mouseOver) {
					UpdatePinnedHelp ();
				}
				QueueDraw ();
			}
		}

		void HandlePress (object sender, EventArgs args)
		{
			// If alternate UI is shown then a press activates the pin
			if (AllowPinning && mouseOver) {
				Pinned = !pinned;

				QueueDraw ();
				PinClicked?.Invoke (this, EventArgs.Empty);
			} else {
				WelcomePageSection.DispatchLink (actionUrl);
			}
		}

		void HandleShowAlternateUI (object sender, EventArgs args)
		{
			mouseOver = true;
			this.SetAlternateUIVisible (true);
			if (!actionUrl.StartsWith ("monodevelop://", StringComparison.Ordinal)) {
				UpdatePinnedHelp ();
			}
			QueueDraw ();
		}

		void UpdatePinnedHelp ()
		{
			if (pinned) {
				this.SetCommonAttributes (null, null, string.Format ("Unpin {0}", title));
			} else {
				this.SetCommonAttributes (null, null, string.Format ("Pin {0}", title));
			}
		}

		void HandleShowDefaultUI (object sender, EventArgs args)
		{
			mouseOver = false;
			this.SetAlternateUIVisible (false);
			if (!actionUrl.StartsWith ("monodevelop://", StringComparison.Ordinal)) {
				this.SetCommonAttributes (null, null, string.Format ("Open {0}", title));
			}
			QueueDraw ();
		}

		protected virtual void DrawLayout (Cairo.Context ctx, Pango.Layout layout, string fontFace, int fontSize, Pango.Weight weight, string color, int tx, int ty)
		{
			ctx.MoveTo (tx, ty);
			Pango.CairoHelper.ShowLayout (ctx, layout);
		}

		protected virtual void DrawHoverBackground (Cairo.Context ctx)
		{
			var allocation = new Gdk.Rectangle (0, 0, Width, Height);
			if (BorderPadding <= 0) {
				ctx.Rectangle (allocation.X, allocation.Y, allocation.Width, allocation.Height);
				ctx.SetSourceColor (CairoExtensions.ParseColor (HoverBackgroundColor));
				ctx.Fill ();
				ctx.MoveTo (allocation.X, allocation.Y + 0.5);
				ctx.RelLineTo (allocation.Width, 0);
				ctx.MoveTo (allocation.X, allocation.Y + allocation.Height - 0.5);
				ctx.RelLineTo (allocation.Width, 0);
				if (DrawRightBorder) {
					ctx.MoveTo (allocation.Right + 0.5, allocation.Y + 0.5);
					ctx.LineTo (allocation.Right + 0.5, allocation.Bottom - 0.5);
				}
				if (DrawLeftBorder) {
					ctx.MoveTo (allocation.Left + 0.5, allocation.Y + 0.5);
					ctx.LineTo (allocation.Left + 0.5, allocation.Bottom - 0.5);
				}
				ctx.LineWidth = 1;
				ctx.SetSourceColor (CairoExtensions.ParseColor (HoverBorderColor));
				ctx.Stroke ();
			}
			else {
				Gdk.Rectangle region = allocation;
				region.Inflate (-BorderPadding, -BorderPadding);
				ctx.RoundedRectangle (region.X + 0.5, region.Y + 0.5, region.Width - 1, region.Height - 1, 3);
				ctx.SetSourceColor (CairoExtensions.ParseColor (HoverBackgroundColor));
				ctx.FillPreserve ();
				ctx.LineWidth = 1;
				ctx.SetSourceColor (CairoExtensions.ParseColor (HoverBorderColor));
				ctx.Stroke ();
			}
		}

		protected virtual void DrawIcon (Cairo.Context ctx)
		{
			int x = InternalPadding;
			int y = (Height - (int)icon.Height) / 2;
			ctx.DrawImage (this, icon, x, y);
			if (AllowPinning && (mouseOver || pinned)) {
				Xwt.Drawing.Image star;
				if (pinned) {
					if (mouseOverStar)
						star = starPinnedHover;
					else
						star = starPinned;
				}
				else {
					if (mouseOverStar)
						star = starNormalHover;
					else
						star = starNormal;
				}
				x = x + (int)icon.Width - (int)star.Width + 3;
				y = y + (int)icon.Height - (int)star.Height + 3;
				ctx.DrawImage (this, star, x, y);
				starRect = new Gdk.Rectangle (x, y, (int)star.Width, (int)star.Height);
			}
		}

		protected override void OnDraw (Cairo.Context ctx, int width, int height)
		{
			if (mouseOver)
				DrawHoverBackground (ctx);

			// Draw the icon
			DrawIcon (ctx);

			// Draw the text

			int textWidth = width - LeftTextPadding - InternalPadding * 2;

			using (var titleLayout = new Pango.Layout (PangoContext))
			{
				titleLayout.Width = Pango.Units.FromPixels (textWidth);
				titleLayout.Ellipsize = Pango.EllipsizeMode.End;
				titleLayout.SetMarkup (WelcomePageSection.FormatText (TitleFontFace, TitleFontSize, Pango.Weight.Bold, MediumTitleColor, title));

				Pango.Layout subtitleLayout = null;

				if (!string.IsNullOrEmpty (subtitle))
				{
					subtitleLayout = new Pango.Layout (PangoContext);
					subtitleLayout.Width = Pango.Units.FromPixels (textWidth);
					subtitleLayout.Ellipsize = Pango.EllipsizeMode.Start;
					subtitleLayout.SetMarkup (WelcomePageSection.FormatText (SmallTitleFontFace, SmallTitleFontSize, Pango.Weight.Normal, SmallTitleColor, subtitle));
				}

				int totalHeight = 0;
				int w, h1, h2 = 0;
				titleLayout.GetPixelSize (out w, out h1);
				totalHeight += h1;

				if (subtitleLayout != null)
				{
					totalHeight += Styles.WelcomeScreen.Pad.Solutions.SolutionTile.TitleBottomMargin;
					subtitleLayout.GetPixelSize (out w, out h2);
					totalHeight += h2;
				}

				int tx = InternalPadding + LeftTextPadding;
				int ty = (height - totalHeight) / 2;
				DrawLayout (ctx, titleLayout, TitleFontFace, TitleFontSize, Pango.Weight.Bold, MediumTitleColor, tx, ty);

				if (subtitleLayout != null)
				{
					ty += h1 + Styles.WelcomeScreen.Pad.Solutions.SolutionTile.TitleBottomMargin;
					DrawLayout (ctx, subtitleLayout, SmallTitleFontFace, SmallTitleFontSize, Pango.Weight.Normal, SmallTitleColor, tx, ty);
					subtitleLayout.Dispose ();
				}
			}
		}
	}
}
