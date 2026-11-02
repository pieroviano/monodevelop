//
// RoundedFrame.cs
//
// Authors:
//   Aaron Bockover <abockover@novell.com>
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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
using Cairo;

using MonoDevelop.Components.Theming;

namespace MonoDevelop.Components
{
	public class RoundedFrame : BinWidget
	{
		private Theme theme;

		private int frame_width = 3;

		private bool fill_color_set;
		private Cairo.Color fill_color;
		private bool draw_border = true;
		private Pattern fill_pattern;

		// Ugh, this is to avoid the GLib.MissingIntPtrCtorException seen by some; BGO #552169
		protected RoundedFrame (IntPtr ptr) : base(ptr)
		{
		}

		public RoundedFrame ()
		{
			Padding = new Gtk.Border { Left = (short)frame_width, Right = (short)frame_width, Top = (short)frame_width, Bottom = (short)frame_width };
		}

		public void SetFillColor (Cairo.Color color)
		{
			fill_color = color;
			fill_color_set = true;
			QueueDraw ();
		}

		public void UnsetFillColor ()
		{
			fill_color_set = false;
			QueueDraw ();
		}

		public Pattern FillPattern {
			get { return fill_pattern; }
			set {
				fill_pattern = value;
				QueueDraw ();
			}
		}

		public bool DrawBorder {
			get { return draw_border; }
			set {
				draw_border = value;
				QueueDraw ();
			}
		}

		#region Gtk.Widget Overrides

		protected override void OnRealized ()
		{
			base.OnRealized ();
			theme = MonoDevelop.Components.Theming.ThemeEngine.CreateTheme (this);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (theme != null && Child != null && Child.Visible)
				snapshot.DrawCairo (this, DrawFrame);
			base.OnSnapshot (snapshot);
		}

		private void DrawFrame (Cairo.Context cr)
		{
			int childWidth = Math.Max (1, Width - frame_width * 2);
			int childHeight = Math.Max (1, Height - frame_width * 2);
			int x = 0;
			int y = -frame_width - 1;
			int width = childWidth + 2 * frame_width;
			int height = childHeight + 3 * frame_width;

			Gdk.Rectangle rect = new Gdk.Rectangle (x, y, width, height);

			theme.Context.ShowStroke = draw_border;

			if (fill_color_set) {
				theme.DrawFrameBackground (cr, rect, fill_color);
				theme.DrawFrameBorder (cr, rect);
			} else if (fill_pattern != null) {
				theme.DrawFrameBackground (cr, rect, fill_pattern);
			} else {
				theme.DrawFrameBackground (cr, rect, true);
				theme.DrawFrameBorder (cr, rect);
			}
		}

		#endregion
	}
}
