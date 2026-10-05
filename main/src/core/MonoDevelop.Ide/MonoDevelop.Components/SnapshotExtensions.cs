//
// SnapshotExtensions.cs
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
	/// OnSnapshot → Cairo bridge (replaces GTK 2 <c>OnExposeEvent</c> + <c>CairoHelper.Create (GdkWindow)</c>).
	/// <code>
	/// protected override void OnSnapshot (Gtk.Snapshot snapshot)
	/// {
	///     snapshot.DrawCairo (this, cr => { cr.Rectangle (0, 0, Width, Height); cr.Fill (); });
	///     base.OnSnapshot (snapshot); // draws children
	/// }
	/// </code>
	/// </summary>
	public static class SnapshotExtensions
	{
		/// <summary>Graphene rect from integer bounds.</summary>
		public static Graphene.Rect ToGrapheneRect (this Gdk.Rectangle r)
		{
			return Graphene.Rect.Alloc ().Init (r.X, r.Y, r.Width, r.Height);
		}

		/// <summary>Graphene rect from double bounds.</summary>
		public static Graphene.Rect CreateGrapheneRect (double x, double y, double width, double height)
		{
			return Graphene.Rect.Alloc ().Init ((float)x, (float)y, (float)Math.Max (0, width), (float)Math.Max (0, height));
		}

		/// <summary>Opens a Cairo context covering <paramref name="bounds"/> (widget coordinates); dispose it when done.</summary>
		public static Cairo.Context CreateCairoContext (this Gtk.Snapshot snapshot, Gdk.Rectangle bounds)
		{
			return snapshot.AppendCairo (bounds.ToGrapheneRect ());
		}

		/// <summary>Opens a Cairo context covering the whole widget; dispose it when done.</summary>
		public static Cairo.Context CreateCairoContext (this Gtk.Snapshot snapshot, Gtk.Widget widget)
		{
			return snapshot.CreateCairoContext (new Gdk.Rectangle (0, 0, widget.Width, widget.Height));
		}

		/// <summary>Runs <paramref name="draw"/> on a Cairo context covering the whole widget and disposes it.</summary>
		public static void DrawCairo (this Gtk.Snapshot snapshot, Gtk.Widget widget, Action<Cairo.Context> draw)
		{
			if (widget.Width <= 0 || widget.Height <= 0)
				return;
			using (var cr = snapshot.CreateCairoContext (widget))
				draw (cr);
		}

		/// <summary>Runs <paramref name="draw"/> on a Cairo context covering <paramref name="bounds"/> and disposes it.</summary>
		public static void DrawCairo (this Gtk.Snapshot snapshot, Gdk.Rectangle bounds, Action<Cairo.Context> draw)
		{
			if (bounds.Width <= 0 || bounds.Height <= 0)
				return;
			using (var cr = snapshot.CreateCairoContext (bounds))
				draw (cr);
		}
	}

	/// <summary>
	/// Custom-drawn leaf widget: override <see cref="OnDraw"/> with Cairo (replaces GTK 2 DrawingArea/ExposeEvent subclasses).
	/// Children (if any are parented by a subclass) are drawn after <see cref="OnDraw"/>.
	/// </summary>
	public abstract class CairoWidget : ContainerWidget
	{
		protected CairoWidget ()
		{
		}

		protected CairoWidget (IntPtr raw) : base (raw)
		{
		}

		/// <summary>Draws the widget; (0,0) is the widget's top-left corner.</summary>
		protected abstract void OnDraw (Cairo.Context cr, int width, int height);

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int w = Width, h = Height;
			if (w > 0 && h > 0) {
				using (var cr = snapshot.CreateCairoContext (new Gdk.Rectangle (0, 0, w, h)))
					OnDraw (cr, w, h);
			}
			base.OnSnapshot (snapshot);
		}
	}
}
