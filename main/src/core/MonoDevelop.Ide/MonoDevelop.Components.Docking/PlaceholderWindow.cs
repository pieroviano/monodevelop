//
// PlaceholderWindow.cs
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

namespace MonoDevelop.Components.Docking
{
	/// <summary>
	/// Outline shown over the dock container while a pad is dragged: it marks where the pad would be docked.
	/// GTK 4 can't position top level windows, so the placeholder is a widget laid out by the <see cref="DockContainer"/>.
	/// </summary>
	internal class PlaceholderWindow: Gtk.Widget
	{
		uint anim;
		Gdk.Rectangle target;
		Gdk.Rectangle current;
		bool allowDocking;

		public bool AllowDocking {
			get {
				return allowDocking;
			}
			set {
				allowDocking = value;
			}
		}

		/// <summary>Current outline, in the coordinates of the dock container.</summary>
		public Gdk.Rectangle Rect {
			get { return current; }
		}

		public PlaceholderWindow (DockFrame frame)
		{
			// The outline must never take the input away from the pads
			CanTarget = false;
			Visible = false;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && anim != 0) {
				GLib.Source.Remove (anim);
				anim = 0;
			}
			base.Dispose (disposing);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = 0;
			minimum_baseline = natural_baseline = -1;
		}

		/// <summary>Allocates the outline inside its parent (called by the dock container).</summary>
		internal void AllocateOverlay (int parentWidth, int parentHeight)
		{
			if (!Visible)
				return;
			var r = current;
			r.Intersect (new Gdk.Rectangle (0, 0, parentWidth, parentHeight), out r);
			if (r.Width > 0 && r.Height > 0)
				SizeAllocate (r, -1);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int w = Width, h = Height;
			if (w <= 0 || h <= 0)
				return;

			snapshot.DrawCairo (this, cr => {
				var color = new Cairo.Color (0.17, 0.55, 0.79);
				cr.SetSourceColor (new Cairo.Color (color.R, color.G, color.B, 0.15));
				cr.Rectangle (0, 0, w, h);
				cr.Fill ();

				// 2 pixels wide frame
				cr.SetSourceColor (new Cairo.Color (color.R, color.G, color.B, 0.6));
				cr.LineWidth = 2;
				cr.Rectangle (1, 1, w - 2, h - 2);
				cr.Stroke ();
			});
		}

		public void Relocate (int x, int y, int w, int h, bool animate)
		{
			var rect = new Gdk.Rectangle (x, y, w, h);
			if (rect.Equals (target))
				return;
			target = rect;

			if (anim != 0) {
				GLib.Source.Remove (anim);
				anim = 0;
			}

			current = target;
			if (animate && w < 150 && h < 150) {
				const int sa = 7;
				current = new Gdk.Rectangle (x - sa, y - sa, w + sa * 2, h + sa * 2);
				anim = GLib.Timeout.Add (10, RunAnimation);
			}
			Parent?.QueueAllocate ();
			QueueDraw ();
		}

		bool RunAnimation ()
		{
			if (current.X != target.X) {
				current = new Gdk.Rectangle (current.X + 1, current.Y + 1, current.Width - 2, current.Height - 2);
				Parent?.QueueAllocate ();
				QueueDraw ();
				return true;
			}
			anim = 0;
			return false;
		}

		public DockDelegate DockDelegate { get; private set; }
		public Gdk.Rectangle DockRect { get; private set; }

		public void SetDockInfo (DockDelegate dockDelegate, Gdk.Rectangle rect)
		{
			DockDelegate = dockDelegate;
			DockRect = rect;
		}
	}

	/// <summary>Title of the dragged pad, shown next to the pointer while a pad is dragged.</summary>
	class PadTitleWindow: BinWidget
	{
		Gdk.Rectangle rect;

		public PadTitleWindow (DockFrame frame, DockItem draggedItem)
		{
			CanTarget = false;
			// Shown on the first Relocate, when its position is known
			Visible = false;

			var box = new Box (Orientation.Horizontal, 3);
			if (draggedItem.Icon != null)
				box.Append (new ImageView (draggedItem.Icon));
			var la = new Label ();
			la.UseMarkup = true;
			la.LabelMarkup = draggedItem.Label;
			box.Append (la);

			CustomFrame f = new CustomFrame ();
			f.SetPadding (12, 12, 12, 12);
			f.SetMargins (1, 1, 1, 1);
			f.BackgroundColor = Ide.Gui.Styles.DockBarBackground.ToGdkColor ();
			f.Child = box;

			Child = f;
		}

		internal void Relocate (int x, int y, int w, int h)
		{
			rect = new Gdk.Rectangle (x, y, w, h);
			Visible = true;
			Parent?.QueueAllocate ();
		}

		internal void AllocateOverlay (int parentWidth, int parentHeight)
		{
			if (!Visible)
				return;
			var r = rect;
			r.X = Math.Max (0, Math.Min (r.X, parentWidth - r.Width));
			r.Y = Math.Max (0, Math.Min (r.Y, parentHeight - r.Height));
			if (r.Width > 0 && r.Height > 0)
				SizeAllocate (r, -1);
		}
	}
}
