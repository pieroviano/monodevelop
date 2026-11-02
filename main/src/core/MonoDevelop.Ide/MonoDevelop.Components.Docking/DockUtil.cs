//
// DockUtil.cs
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
using System.Runtime.CompilerServices;

namespace MonoDevelop.Components.Docking
{
	/// <summary>
	/// GTK 4 helpers shared by the docking and dock notebook widgets (size requests, re-parenting, per-widget CSS,
	/// focus rectangles).
	/// </summary>
	static class DockUtil
	{
		/// <summary>Minimum size of <paramref name="widget"/> (replaces GTK 2 <c>Widget.SizeRequest ()</c>); 0x0 while the widget is hidden.</summary>
		public static Gtk.Requisition GetRequisition (this Gtk.Widget widget)
		{
			if (widget == null)
				return Gtk.Requisition.Zero;
			widget.GetPreferredSize (out var minimum, out _);
			return minimum;
		}

		/// <summary>Natural size of <paramref name="widget"/>; 0x0 while the widget is hidden.</summary>
		public static Gtk.Requisition GetNaturalRequisition (this Gtk.Widget widget)
		{
			if (widget == null)
				return Gtk.Requisition.Zero;
			widget.GetPreferredSize (out _, out var natural);
			return natural;
		}

		/// <summary>The window containing <paramref name="widget"/>, or null when it is not in a window.</summary>
		public static Gtk.Window GetToplevelWindow (this Gtk.Widget widget)
		{
			return widget?.Root as Gtk.Window;
		}

		/// <summary>Removes <paramref name="widget"/> from whatever container currently holds it.</summary>
		public static void RemoveFromParent (Gtk.Widget widget)
		{
			var parent = widget?.Parent;
			if (parent == null)
				return;
			if (widget.IsMapped)
				widget.Unmap ();
			if (parent is ContainerWidget container)
				container.RemoveChild (widget);
			else if (parent is Gtk.Box box)
				box.Remove (widget);
			else if (parent is Gtk.Window window)
				window.Child = null;
			else
				widget.Unparent ();
		}

		/// <summary>Position of the pointer relative to <paramref name="widget"/>, or false when it cannot be determined.</summary>
		public static bool TryGetPointer (this Gtk.Widget widget, out double x, out double y)
		{
			x = y = 0;
			var native = widget?.Native;
			var surface = native?.Surface;
			var pointer = widget?.Display?.DefaultSeat?.Pointer;
			if (surface == null || pointer == null)
				return false;
			surface.GetDevicePosition (pointer, out double sx, out double sy, out _);
			var nativeWidget = native as Gtk.Widget;
			if (nativeWidget == null)
				return false;
			// Surface coordinates include the client side decorations around the window widget
			native.GetSurfaceTransform (out double tx, out double ty);
			sx -= tx;
			sy -= ty;
			nativeWidget.TranslateCoordinates (widget, sx, sy, out x, out y);
			return true;
		}

		/// <summary>True when the point (x, y), in <paramref name="widget"/> coordinates, is over <paramref name="widget"/> or one of its descendants.</summary>
		public static bool IsOverWidget (Gtk.Widget container, Gtk.Widget widget, double x, double y)
		{
			if (container == null || widget == null || !widget.Visible)
				return false;
			var hit = container.Pick (x, y, Gtk.PickFlags.Default);
			return hit != null && (hit == widget || hit.IsAncestor (widget));
		}

		/// <summary>Draws a dotted focus rectangle (replaces <c>Gtk.Style.PaintFocus</c>).</summary>
		public static void DrawFocusRectangle (Cairo.Context cr, Gdk.Rectangle rect, Cairo.Color color)
		{
			if (rect.Width <= 0 || rect.Height <= 0)
				return;
			cr.Save ();
			cr.SetSourceColor (color);
			cr.LineWidth = 1;
			cr.SetDash (new double [] { 1, 1 }, 0);
			cr.Rectangle (rect.X + 0.5, rect.Y + 0.5, rect.Width - 1, rect.Height - 1);
			cr.Stroke ();
			cr.Restore ();
		}

		/// <summary>Foreground colour of the widget's current theme.</summary>
		public static Cairo.Color GetForegroundColor (this Gtk.Widget widget)
		{
			var c = widget.StyleColor;
			return new Cairo.Color (c.Red, c.Green, c.Blue, c.Alpha);
		}

		static readonly ConditionalWeakTable<Gtk.Widget, WidgetCss> cssTable = new ConditionalWeakTable<Gtk.Widget, WidgetCss> ();
		static int cssCounter;

		sealed class WidgetCss
		{
			public Gtk.CssProvider Provider;
			public string CssClass;
		}

		/// <summary>
		/// Sets (or clears, when null) the background colour of <paramref name="widget"/> through a private CSS provider
		/// (replaces <c>ModifyBg</c>/<c>ModifyBase</c>).
		/// </summary>
		public static void SetBackground (Gtk.Widget widget, Xwt.Drawing.Color? color)
		{
			SetCss (widget, color == null ? null : "background-color: " + ToCss (color.Value) + ";");
		}

		/// <summary>Applies the given CSS declarations only to <paramref name="widget"/> (null clears them).</summary>
		public static void SetCss (Gtk.Widget widget, string declarations)
		{
			if (!cssTable.TryGetValue (widget, out var css)) {
				if (declarations == null)
					return;
				css = new WidgetCss {
					Provider = new Gtk.CssProvider (),
					CssClass = "md-dock-" + (++cssCounter),
				};
				cssTable.Add (widget, css);
				widget.AddCssClass (css.CssClass);
				widget.StyleContext.AddProvider (css.Provider, (uint)Gtk.StyleProviderPriority.Application);
			}
			css.Provider.LoadFromString (declarations == null ? "" : "." + css.CssClass + " { " + declarations + " }");
		}

		public static string ToCss (Xwt.Drawing.Color color)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "rgba({0},{1},{2},{3})",
				(int)Math.Round (color.Red * 255), (int)Math.Round (color.Green * 255), (int)Math.Round (color.Blue * 255), color.Alpha);
		}
	}
}
