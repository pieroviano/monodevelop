//
// HoverCloseButton.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using MonoDevelop.Components;

namespace MonoDevelop.SourceEditor
{
	class HoverCloseButton : Gtk.Widget
	{
		bool hovered;

		public HoverCloseButton ()
		{
			SetSizeRequest (16, 16);
			Halign = Align.Center;
			Valign = Align.Center;
			this.AddMotionHandler (null, (x, y) => SetHovered (true), () => SetHovered (false));
			this.AddClickHandler (e => {
				if (e.Button == 1 && hovered)
					click = true;
			}, e => {
				if (click && hovered)
					OnClicked (EventArgs.Empty);
				click = false;
			});
		}

		void SetHovered (bool value)
		{
			hovered = value;
			QueueDraw ();
		}

		bool click;

		public event EventHandler Clicked;

		protected virtual void OnClicked (EventArgs e)
		{
			var handler = Clicked;
			if (handler != null)
				handler (this, e);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			snapshot.DrawCairo (this, cr => DrawCloseButton (cr, new Gdk.Point (Width / 2, Height / 2), hovered, 1.0, 0));
		}

		protected override void OnMeasure (Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = 16;
			minimum_baseline = natural_baseline = -1;
		}

		static void DrawCloseButton (Cairo.Context context, Gdk.Point center, bool hovered, double opacity, double animationProgress)
		{
			if (hovered) {
				double radius = 6;
				context.Arc (center.X, center.Y, radius, 0, Math.PI * 2);
				context.SetSourceRGBA (.6, .6, .6, opacity);
				context.Fill ();

				context.SetSourceRGBA (0.95, 0.95, 0.95, opacity);
				context.LineWidth = 2;

				context.MoveTo (center.X - 3, center.Y - 3);
				context.LineTo (center.X + 3, center.Y + 3);
				context.MoveTo (center.X - 3, center.Y + 3);
				context.LineTo (center.X + 3, center.Y - 3);
				context.Stroke ();
			} else {
				double lineColor = .63 - .1 * animationProgress;
				double fillColor = .74;

				double heightMod = Math.Max (0, 1.0 - animationProgress * 2);
				context.MoveTo (center.X - 3, center.Y - 3 * heightMod);
				context.LineTo (center.X + 3, center.Y + 3 * heightMod);
				context.MoveTo (center.X - 3, center.Y + 3 * heightMod);
				context.LineTo (center.X + 3, center.Y - 3 * heightMod);

				context.LineWidth = 2;
				context.SetSourceRGBA (lineColor, lineColor, lineColor, opacity);
				context.Stroke ();

				if (animationProgress > 0.5) {
					double partialProg = (animationProgress - 0.5) * 2;
					context.MoveTo (center.X - 3, center.Y);
					context.LineTo (center.X + 3, center.Y);

					context.LineWidth = 2 - partialProg;
					context.SetSourceRGBA (lineColor, lineColor, lineColor, opacity);
					context.Stroke ();


					double radius = partialProg * 3.5;

					// Background
					context.Arc (center.X, center.Y, radius, 0, Math.PI * 2);
					context.SetSourceRGBA (fillColor, fillColor, fillColor, opacity);
					context.Fill ();

					// Inset shadow
					// FIXME: VV: Remove gradient features
					using (var lg = new Cairo.LinearGradient (0, center.Y - 5, 0, center.Y)) {
						context.Arc (center.X, center.Y + 1, radius, 0, Math.PI * 2);
						lg.AddColorStop (0, new Cairo.Color (0, 0, 0, 0.2 * opacity));
						lg.AddColorStop (1, new Cairo.Color (0, 0, 0, 0));
						context.SetSource (lg);
						context.Stroke ();
					}

					// Outline
					context.Arc (center.X, center.Y, radius, 0, Math.PI * 2);
					context.SetSourceRGBA (lineColor, lineColor, lineColor, opacity);
					context.Stroke ();
				}
			}
		}
	}
}

