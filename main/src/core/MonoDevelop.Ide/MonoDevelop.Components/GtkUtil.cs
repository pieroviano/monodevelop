// GtkUtil.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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
//
//

using System;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using MonoDevelop.Core;

namespace MonoDevelop.Components
{
	/// <summary>
	/// GTK 2 stock icon sizes. GTK 4's <see cref="Gtk.IconSize"/> only has Inherit/Normal/Large, so MonoDevelop keeps
	/// its own enum (same numeric values as GTK 2's GtkIconSize) for icon lookups. Pixel sizes: <see cref="GtkUtil.GetSize(StockIconSize, out int, out int)"/>.
	/// </summary>
	public enum StockIconSize
	{
		Invalid = 0,
		Menu = 1,
		SmallToolbar = 2,
		LargeToolbar = 3,
		Button = 4,
		Dnd = 5,
		Dialog = 6,
	}

	public static class GtkUtil
	{
		static Xwt.Toolkit gtkToolkit;

		internal static Xwt.Toolkit GtkToolkit {
			get {
				if (gtkToolkit == null)
					gtkToolkit = Xwt.Toolkit.LoadedToolkits.FirstOrDefault (t => t.Type == Xwt.ToolkitType.Gtk4);
				return gtkToolkit;
			}
		}

		public static Cairo.Color ToCairoColor (this Gdk.RGBA color)
		{
			return new Cairo.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Xwt.Drawing.Color ToXwtColor (this Gdk.RGBA color)
		{
			return new Xwt.Drawing.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static string GetHex (this Gdk.RGBA color)
		{
			return String.Format ("#{0:x2}{1:x2}{2:x2}",
			                     (byte)(Clamp (color.Red) * 255),
			                     (byte)(Clamp (color.Green) * 255),
			                     (byte)(Clamp (color.Blue) * 255));
		}

		static double Clamp (double v) => v < 0 ? 0 : v > 1 ? 1 : v;

		/// <summary>Converts to <see cref="Gdk.RGBA"/> (GTK 4 has no Gdk.Color).</summary>
		public static Gdk.RGBA ToGdkColor (this Cairo.Color color)
		{
			return new Gdk.RGBA { Red = (float)color.R, Green = (float)color.G, Blue = (float)color.B, Alpha = (float)color.A };
		}

		/// <summary>Converts to <see cref="Gdk.RGBA"/> (GTK 4 has no Gdk.Color).</summary>
		public static Gdk.RGBA ToGdkColor (this Xwt.Drawing.Color color)
		{
			return new Gdk.RGBA { Red = (float)color.Red, Green = (float)color.Green, Blue = (float)color.Blue, Alpha = (float)color.Alpha };
		}

		public static Cairo.Color ToCairoColor (this Xwt.Drawing.Color color)
		{
			return new Cairo.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Xwt.Drawing.Color ToXwtColor (this Cairo.Color color)
		{
			return new Xwt.Drawing.Color (color.R, color.G, color.B, color.A);
		}

		/// <summary>
		/// Makes a color lighter or darker
		/// </summary>
		/// <param name='lightAmount'>
		/// Amount of lightness to add. If the value is positive, the color will be lighter,
		/// if negative it will be darker. Value must be between 0 and 1.
		/// </param>
		public static Gdk.RGBA AddLight (this Gdk.RGBA color, double lightAmount)
		{
			var c = color.ToXwtColor ();
			c.Light += lightAmount;
			return c.ToGdkColor ();
		}

		/// <summary>
		/// Makes a color lighter or darker
		/// </summary>
		/// <param name='lightAmount'>
		/// Amount of lightness to add. If the value is positive, the color will be lighter,
		/// if negative it will be darker. Value must be between 0 and 1.
		/// </param>
		public static HslColor AddLight (this HslColor color, double lightAmount)
		{
			color.L += lightAmount;
			return color;
		}

		public static Cairo.Color AddLight (this Cairo.Color color, double lightAmount)
		{
			var c = color.ToXwtColor ();
			c.Light += lightAmount;
			return c.ToCairoColor ();
		}

		/// <summary>
		/// Makes a color lighter or darker
		/// </summary>
		/// <param name='lightAmount'>
		/// Amount of lightness to add. If the value is positive, the color will be lighter,
		/// if negative it will be darker. Value must be between 0 and 1.
		/// </param>
		public static Xwt.Drawing.Color AddLight (this Xwt.Drawing.Color color, double lightAmount)
		{
			color.Light += lightAmount;
			return color;
		}

		/// <summary>Wraps a Cairo context (e.g. from <c>snapshot.CreateCairoContext (w)</c>) in an Xwt drawing context.</summary>
		public static Xwt.Drawing.Context CreateXwtContext (this Gtk.Widget w, Cairo.Context cr)
		{
			return GtkToolkit.WrapContext (w, cr);
		}

		public static Gtk.Widget ToGtkWidget (this Xwt.Widget widget)
		{
			return (Gtk.Widget) GtkToolkit.GetNativeWidget (widget);
		}

		public static void DrawImage (this Cairo.Context s, Gtk.Widget widget, Xwt.Drawing.Image image, double x, double y)
		{
			GtkToolkit.RenderImage (widget, s, image, x, y);
		}

		public static Xwt.Drawing.Image ToXwtImage (this Gdk.Pixbuf pix)
		{
			return GtkToolkit.WrapImage (pix);
		}

		public static Gdk.Pixbuf ToPixbuf (this Xwt.Drawing.Image image)
		{
			return (Gdk.Pixbuf)GtkToolkit.GetNativeImage (image);
		}

		public static Gdk.Pixbuf ToPixbuf (this Xwt.Drawing.Image image, StockIconSize size)
		{
			return (Gdk.Pixbuf)GtkToolkit.GetNativeImage (image.WithSize (size));
		}

		/// <summary>GTK 4 paintable for an Xwt image (for Gtk.Image.SetFromPaintable / Gtk.Picture).</summary>
		public static Gdk.Texture ToTexture (this Xwt.Drawing.Image image)
		{
			var pb = image.ToPixbuf ();
			return pb != null ? new Gdk.Texture (pb) : null;
		}

		public static Xwt.Drawing.Image WithSize (this Xwt.Drawing.Image image, StockIconSize size)
		{
			int w, h;
			size.GetSize (out w, out h);
			return image.WithSize (w, h);
		}

		public static Xwt.Size GetSize (this StockIconSize size)
		{
			int w, h;
			size.GetSize (out w, out h);
			return new Xwt.Size (w, h);
		}

		/// <summary>Pixel size of a stock icon size (GTK 2 defaults: menu/button/small toolbar 16, large toolbar 24, dnd 32, dialog 48).</summary>
		public static void GetSize (this StockIconSize size, out int width, out int height)
		{
			switch (size) {
			case StockIconSize.LargeToolbar: width = height = 24; break;
			case StockIconSize.Dnd: width = height = 32; break;
			case StockIconSize.Dialog: width = height = 48; break;
			case StockIconSize.Invalid: width = height = 0; break;
			default: width = height = 16; break;
			}
		}

		public static Gdk.Rectangle ToGdkRectangle (this Xwt.Rectangle rect)
		{
			return new Gdk.Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static Xwt.Rectangle ToXwtRectangle (this Gdk.Rectangle rect)
		{
			return new Xwt.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static Gdk.Point ToGdkPoint (this Xwt.Point point)
		{
			return new Gdk.Point ((int)point.X, (int)point.Y);
		}

		public static Xwt.Point ToXwtPoint (this Gdk.Point point)
		{
			return new Xwt.Point (point.X, point.Y);
		}

		#if MAC
		static Gdk.Point ConvertToGdkCoordinates (this AppKit.NSScreen screen, Gdk.Point screenPoint)
		{
			if (screen == null)
				return new Gdk.Point ();
			var macgeometry = screen.Frame;
			// Cocoa screen frames are relative to the main monitor 0.
			screenPoint.X = (int)Math.Abs (macgeometry.X - screenPoint.X);
			return screenPoint;
		}

		public static Gdk.Rectangle GetSceenBounds (this AppKit.NSView widget)
		{
			var frame = widget.Frame;
			var point = ConvertToGdkCoordinates (widget.Window?.Screen, new Gdk.Point ((int)frame.Location.X, (int)frame.Location.Y));
			frame.X = point.X;
			frame.Y = point.Y;
			return new Gdk.Rectangle ((int)frame.X, (int)frame.Y, (int)frame.Width, (int)frame.Height);
		}
		#endif

		public static Gdk.Rectangle GetSceenBounds (this Xwt.Widget widget)
		{
			var wbounds = widget.ScreenBounds.ToGdkRectangle ();
			#if MAC
			// Xwt.Widget.ScreenBounds is toolkit specific and Cocoa uses a different screen coordinate system.
			var view = widget.Surface.NativeWidget as AppKit.NSView;
			if (view != null) {
				var point = ConvertToGdkCoordinates (view.Window?.Screen, wbounds.Location);
				wbounds.X = point.X;
				wbounds.Y = point.Y;
			}
			#endif
			return wbounds;
		}

		public static Gdk.Point ToScreenCoordinates (this Xwt.Widget widget, Xwt.Point point)
		{
			var spoint = widget.ConvertToScreenCoordinates (point).ToGdkPoint ();
			#if MAC
			// Xwt.Widget.ScreenBounds is toolkit specific and Cocoa uses a different screen coordinate system.
			var view = widget.Surface.NativeWidget as AppKit.NSView;
			if (view != null) {
				spoint = ConvertToGdkCoordinates (view.Window?.Screen, spoint);
			}
			#endif
			return spoint;
		}

		public static Gdk.Rectangle ToScreenCoordinates (Xwt.Widget widget, Xwt.Rectangle rect)
		{
			return new Gdk.Rectangle (ToScreenCoordinates (widget, rect.Location), new Gdk.Size ((int)rect.Width, (int)rect.Height));
		}

		/// <summary>
		/// Converts <paramref name="p"/> (widget coordinates) to the coordinates of the widget's toplevel window.
		/// GTK 4 exposes no global screen coordinates, so this is the closest equivalent of the GTK 2 behaviour.
		/// </summary>
		public static Gdk.Point GetScreenCoordinates (this Gtk.Widget w, Gdk.Point p)
		{
			var root = w.Root as Gtk.Widget;
			if (root == null)
				return new Gdk.Point ();
			if (!w.TranslateCoordinates (root, p.X, p.Y, out double x, out double y))
				return new Gdk.Point ();
			return new Gdk.Point ((int)x, (int)y);
		}

		/// <summary>True if the row at (x, y) — widget coordinates — is selected.</summary>
		public static bool IsClickedNodeSelected (this Gtk.TreeView tree, int x, int y)
		{
			Gtk.TreePath path;
			tree.ConvertWidgetToBinWindowCoords (x, y, out int bx, out int by);
			if (tree.GetPathAtPos (bx, by, out path))
				return tree.Selection.PathIsSelected (path);

			return false;
		}

		/// <summary>Shows a tooltip with the full cell text when a text cell is truncated (GTK 4: via QueryTooltip).</summary>
		public static void EnableAutoTooltips (this Gtk.TreeView tree)
		{
			tree.HasTooltip = true;
			tree.QueryTooltip -= HandleTreeQueryTooltip;
			tree.QueryTooltip += HandleTreeQueryTooltip;
		}

		static void HandleTreeQueryTooltip (object o, QueryTooltipArgs args)
		{
			var tree = (Gtk.TreeView)o;
			int x = args.X, y = args.Y;
			if (!tree.GetTooltipContext (ref x, ref y, args.KeyboardMode, out ITreeModel model, out TreePath path, out TreeIter iter))
				return;

			tree.ConvertWidgetToBinWindowCoords (args.X, args.Y, out int bx, out int by);
			if (!tree.GetPathAtPos (bx, by, out TreePath p, out TreeViewColumn col) || col == null)
				return;

			col.CellSetCellData (model, iter, false, false);
			var area = tree.GetCellArea (path, col);
			int needed = 0;
			var texts = new List<string> ();
			foreach (var cr in col.Cells) {
				if (!cr.Visible)
					continue;
				cr.GetPreferredWidth (tree, out _, out int nat);
				needed += nat;
				if (cr is CellRendererText ct && !string.IsNullOrEmpty (ct.Text))
					texts.Add (ct.Text);
			}
			if (needed <= area.Width || texts.Count == 0)
				return;
			args.Tooltip.Text = string.Join (" ", texts);
			tree.SetTooltipCell (args.Tooltip, path, col, null);
			args.RetVal = true;
		}

		public static bool GetCellForegroundSet (this Gtk.CellRendererText cell)
		{
			GLib.Value property = cell.GetProperty ("foreground-set");
			bool result = (bool)property;
			property.Dispose ();
			return result;
		}

		public static void SetCellForegroundSet (this Gtk.CellRendererText cell, bool value)
		{
			GLib.Value val = new GLib.Value (value);
			cell.SetProperty ("foreground-set", val);
			val.Dispose ();
		}

		/// <summary>
		/// Puts <paramref name="newWidget"/> where <paramref name="oldWidget"/> is (Box, Paned, Grid, BinWidget, ContainerWidget parents),
		/// copying expand/align/margin settings, and removes <paramref name="oldWidget"/>.
		/// </summary>
		public static T ReplaceWithWidget<T> (this Gtk.Widget oldWidget, T newWidget, bool transferChildren = false) where T:Gtk.Widget
		{
			var parent = oldWidget.Parent;
			if (parent == null)
				throw new InvalidOperationException ();

			CopyLayoutProperties (oldWidget, newWidget);

			if (parent is Gtk.Box box) {
				box.InsertChildAfter (newWidget, oldWidget);
				box.Remove (oldWidget);
			} else if (parent is Gtk.Grid grid) {
				grid.QueryChild (oldWidget, out int col, out int row, out int w, out int h);
				grid.Remove (oldWidget);
				grid.Attach (newWidget, col, row, w, h);
			} else if (parent is Gtk.Paned paned) {
				if (oldWidget == paned.StartChild)
					paned.StartChild = newWidget;
				else
					paned.EndChild = newWidget;
			} else if (parent is BinWidget bin) {
				bin.Child = newWidget;
			} else if (parent is ContainerWidget container) {
				int index = container.Children.ToList ().IndexOf (oldWidget);
				container.RemoveChild (oldWidget);
				container.InsertChild (newWidget, index);
			} else
				throw new NotSupportedException ();

			if (transferChildren) {
				if (newWidget is Gtk.Paned panedNew && oldWidget is Gtk.Paned panedOld) {
					var c1 = panedOld.StartChild;
					var c2 = panedOld.EndChild;
					panedNew.ResizeStartChild = panedOld.ResizeStartChild;
					panedNew.ShrinkStartChild = panedOld.ShrinkStartChild;
					panedNew.ResizeEndChild = panedOld.ResizeEndChild;
					panedNew.ShrinkEndChild = panedOld.ShrinkEndChild;
					panedOld.StartChild = null;
					panedOld.EndChild = null;
					panedNew.StartChild = c1;
					panedNew.EndChild = c2;
				}
				else
					throw new NotSupportedException ();
			}

			newWidget.Visible = oldWidget.Visible;
			return newWidget;
		}

		static void CopyLayoutProperties (Gtk.Widget from, Gtk.Widget to)
		{
			to.Hexpand = from.Hexpand;
			to.Vexpand = from.Vexpand;
			to.Halign = from.Halign;
			to.Valign = from.Valign;
			to.MarginStart = from.MarginStart;
			to.MarginEnd = from.MarginEnd;
			to.MarginTop = from.MarginTop;
			to.MarginBottom = from.MarginBottom;
		}

		/// <summary>True when the default display composites (translucent windows are possible).</summary>
		public static bool ScreenSupportsARGB ()
		{
			var d = Gdk.Display.Default;
			return d != null && d.IsComposited && d.IsRgba;
		}

		/// <summary>
		/// Raises <paramref name="leaveHandler"/> when the pointer leaves the widget (child widgets count as inside,
		/// which is GTK 4's crossing semantics). Dispose the returned object to unsubscribe.
		/// </summary>
		public static IDisposable SubscribeLeaveEvent (this Gtk.Widget w, System.Action leaveHandler)
		{
			return new LeaveEventData (w, leaveHandler);
		}

		/// <summary>Depth-first enumeration of <paramref name="widget"/> and all its descendants.</summary>
		public static IEnumerable<Gtk.Widget> GetDescendants (this Gtk.Widget widget)
		{
			var widgets = new Stack<Widget> (new[] { widget });

			while (widgets.Count > 0) {
				var w = widgets.Pop ();
				yield return w;
				for (var c = w.LastChild; c != null; c = c.PrevSibling)
					widgets.Push (c);
			}
		}

		/// <summary>Direct children of <paramref name="widget"/> (GTK 4 sibling chain).</summary>
		public static IEnumerable<Gtk.Widget> GetChildren (this Gtk.Widget widget)
		{
			for (var c = widget.FirstChild; c != null; c = c.NextSibling)
				yield return c;
		}

		/// <summary>No-op on GTK 4 (GTK 4 entries already provide their own context menu).</summary>
		public static void UseNativeContextMenus (this Gtk.Window window)
		{
		}

		/// <summary>No-op on GTK 4 (GTK 4 entries already provide their own context menu).</summary>
		public static void UseNativeContextMenus (this Gtk.Entry entry)
		{
		}

		/// <summary>
		/// Shows the context menu for a TreeView.
		/// </summary>
		/// <returns><c>true</c>, if context menu was shown, <c>false</c> otherwise.</returns>
		/// <param name="tree">Gtk TreeView for which the context menu is shown</param>
		/// <param name="evt">The click that triggered the menu (widget coordinates), or <c>null</c> for keyboard invocation.</param>
		/// <param name="entrySet">Entry set with the command definitions</param>
		/// <param name="initialCommandTarget">Initial command target.</param>
		public static bool ShowContextMenu (this Gtk.TreeView tree, ClickInfo evt, Commands.CommandEntrySet entrySet,
			object initialCommandTarget = null)
		{
			if (evt == null) {
				var paths = tree.Selection.GetSelectedRows ();
				if (paths != null && paths.Length > 0) {
					var area = tree.GetCellArea (paths [0], tree.GetColumn (0));
					tree.ConvertBinWindowToWidgetCoords (area.X, area.Y, out int wx, out int wy);
					return Ide.IdeApp.CommandService.ShowContextMenu (tree, wx, wy, entrySet, initialCommandTarget);
				} else
					return Ide.IdeApp.CommandService.ShowContextMenu (tree, 0, 0, entrySet, initialCommandTarget);
			} else {
				return Ide.IdeApp.CommandService.ShowContextMenu (tree, (int)evt.X, (int)evt.Y, entrySet, initialCommandTarget);
			}
		}
	}

	class LeaveEventData: IDisposable
	{
		readonly Gtk.Widget widget;
		readonly Gtk.EventControllerMotion controller;

		public LeaveEventData (Gtk.Widget w, System.Action leaveHandler)
		{
			widget = w;
			controller = w.AddMotionHandler (null, null, leaveHandler);
		}

		public void Dispose ()
		{
			if (widget.Handle != IntPtr.Zero)
				widget.RemoveController (controller);
		}
	}
}
