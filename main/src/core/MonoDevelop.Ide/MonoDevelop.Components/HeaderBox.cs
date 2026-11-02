// 
// HeaderBox.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Single-child box with border lines (margins) and inner padding; set the content with <see cref="BinWidget.Child"/>.
	/// </summary>
	public class HeaderBox: BinWidget
	{
		int topMargin;
		int bottomMargin;
		int leftMargin;
		int rightMargin;

		int topPadding;
		int bottomPadding;
		int leftPadding;
		int rightPadding;
		Gdk.RGBA? backgroundColor;
		bool showTopShadow;
		bool useChildBackgroundColor;
		int shadowSize = 3;
		double shadowStrengh = 0.1;

		public HeaderBox ()
		{
		}

		public HeaderBox (int topMargin, int bottomMargin, int leftMargin, int rightMargin)
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

		public bool ShowTopShadow {
			get { return showTopShadow; }
			set {
				showTopShadow = value;
				QueueDraw ();
			}
		}

		public int ShadowSize {
			get {
				return shadowSize;
			}
			set {
				shadowSize = value;
				QueueDraw ();
			}
		}

		public double ShadowStrengh {
			get {
				return shadowStrengh;
			}
			set {
				shadowStrengh = value;
				QueueDraw ();
			}
		}

		public bool GradientBackground { get; set; }

		public Gdk.RGBA? BorderColor { get; set; }

		public Gdk.RGBA? BackgroundColor {
			get { return backgroundColor; }
			set {
				backgroundColor = value;
				QueueDraw ();
			}
		}

		public bool UseChildBackgroundColor {
			get {
				return useChildBackgroundColor;
			}
			set {
				useChildBackgroundColor = value;
				QueueDraw ();
			}
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width <= 0 || height <= 0)
				return;

			snapshot.DrawCairo (this, DrawBackground);
			base.OnSnapshot (snapshot);
			snapshot.DrawCairo (this, DrawBorders);
		}

		void DrawBackground (Cairo.Context cr)
		{
			int width = Width, height = Height;
			if (GradientBackground) {
				var gcol = Styles.BackgroundColor;
				cr.Rectangle (0, 0, width, height);
				// FIXME: VV: Remove gradient features
				using (Cairo.Gradient pat = new Cairo.LinearGradient (0, 0, 0, height)) {
					pat.AddColorStop (0, gcol.ToCairoColor ());
					gcol.Light -= 0.1;
					if (gcol.Light < 0)
						gcol.Light = 0;
					pat.AddColorStop (1, gcol.ToCairoColor ());
					cr.SetSource (pat);
					cr.Fill ();
				}
			} else if (BackgroundColor != null) {
				cr.Rectangle (0, 0, width, height);
				cr.SetSourceColor (BackgroundColor.Value.ToCairoColor ());
				cr.Fill ();
			} else if (useChildBackgroundColor && Child != null) {
				// GTK 4 cannot query a widget's theme background: use the IDE base (entry/list) background
				cr.Rectangle (0, 0, width, height);
				cr.SetSourceColor (Styles.BaseBackgroundColor.ToCairoColor ());
				cr.Fill ();
			}
		}

		void DrawBorders (Cairo.Context cr)
		{
			int width = Width, height = Height;
			cr.SetSourceColor (BorderColor != null ? BorderColor.Value.ToCairoColor () : Styles.BorderColor.ToCairoColor ());
			if (topMargin > 0)
				cr.Rectangle (0, 0, width, topMargin);
			if (bottomMargin > 0)
				cr.Rectangle (0, height - bottomMargin, width, bottomMargin);
			if (leftMargin > 0)
				cr.Rectangle (0, 0, leftMargin, height);
			if (rightMargin > 0)
				cr.Rectangle (width - rightMargin, 0, rightMargin, height);
			cr.Fill ();

			if (showTopShadow) {
				// FIXME: VV: Remove gradient features
				cr.Rectangle (0, 0, width, shadowSize);
				using (Cairo.Gradient pat = new Cairo.LinearGradient (0, 0, 0, shadowSize)) {
					pat.AddColorStop (0, new Cairo.Color (0, 0, 0, shadowStrengh));
					pat.AddColorStop (1, new Cairo.Color (0, 0, 0, 0));
					cr.SetSource (pat);
					cr.Fill ();
				}
			}
		}
	}
}
