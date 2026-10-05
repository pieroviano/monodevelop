//
// PopoverHelper.cs
//
// Copyright (c) Microsoft Corp.
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

namespace MonoDevelop.Components
{
	/// <summary>
	/// Anchored popups (GTK 4 cannot position toplevels, so popup windows become <see cref="Gtk.Popover"/>s
	/// parented to a widget and pointing at a rectangle in that widget's coordinates).
	/// <code>
	/// var pop = PopoverHelper.Show (editor, new Gdk.Rectangle (x, y, 1, lineHeight), content, Gtk.PositionType.Bottom);
	/// ...
	/// pop.Popdown (); // unparented automatically when transient
	/// </code>
	/// </summary>
	public static class PopoverHelper
	{
		/// <summary>Creates (not shown) a popover attached to <paramref name="anchor"/>.</summary>
		/// <param name="transient">When true the popover is unparented (and becomes collectable) once closed.</param>
		public static Gtk.Popover Create (Gtk.Widget anchor, Gtk.Widget content, Gtk.PositionType position = Gtk.PositionType.Bottom, bool autohide = true, bool hasArrow = false, bool transient = true)
		{
			if (anchor == null)
				throw new ArgumentNullException (nameof (anchor));
			var pop = new Gtk.Popover {
				Child = content,
				Position = position,
				Autohide = autohide,
				HasArrow = hasArrow,
			};
			pop.Parent = anchor;
			if (transient)
				pop.Closed += (s, e) => GLib.Idle.Add (() => { Detach (pop); return false; });
			return pop;
		}

		/// <summary>Creates and pops up a popover pointing at <paramref name="area"/> (anchor coordinates; null = whole anchor).</summary>
		public static Gtk.Popover Show (Gtk.Widget anchor, Gdk.Rectangle? area, Gtk.Widget content, Gtk.PositionType position = Gtk.PositionType.Bottom, bool autohide = true, bool hasArrow = false, bool transient = true)
		{
			var pop = Create (anchor, content, position, autohide, hasArrow, transient);
			ShowAt (pop, area, position);
			return pop;
		}

		/// <summary>Re-points an existing popover (e.g. tooltip following the mouse) and pops it up.</summary>
		public static void ShowAt (this Gtk.Popover popover, Gdk.Rectangle? area, Gtk.PositionType? position = null)
		{
			if (area.HasValue) {
				var r = area.Value;
				popover.PointingTo = new Gdk.Rectangle (r.X, r.Y, Math.Max (1, r.Width), Math.Max (1, r.Height));
			}
			if (position.HasValue)
				popover.Position = position.Value;
			popover.Popup ();
		}

		/// <summary>Moves a popover to another anchor widget.</summary>
		public static void Reparent (this Gtk.Popover popover, Gtk.Widget anchor)
		{
			if (popover.Parent == anchor)
				return;
			if (popover.Parent != null)
				popover.Unparent ();
			popover.Parent = anchor;
		}

		/// <summary>Pops down and unparents a popover (it can be collected afterwards).</summary>
		public static void Detach (this Gtk.Popover popover)
		{
			if (popover == null || popover.Handle == IntPtr.Zero)
				return;
			if (popover.Visible)
				popover.Popdown ();
			if (popover.Parent != null)
				popover.Unparent ();
		}

		/// <summary>Converts a rectangle in <paramref name="from"/> coordinates to <paramref name="to"/> coordinates.</summary>
		public static Gdk.Rectangle TranslateRectangle (this Gtk.Widget from, Gtk.Widget to, Gdk.Rectangle rect)
		{
			if (from == to || to == null)
				return rect;
			if (!from.TranslateCoordinates (to, rect.X, rect.Y, out double x, out double y))
				return rect;
			return new Gdk.Rectangle ((int)Math.Round (x), (int)Math.Round (y), rect.Width, rect.Height);
		}
	}
}
