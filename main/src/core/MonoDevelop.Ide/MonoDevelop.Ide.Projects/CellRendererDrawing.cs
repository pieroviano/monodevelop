//
// CellRendererDrawing.cs
//
// Shared drawing helpers of the custom template/language cell renderers (GTK 4 port: Cairo + Pango on a Snapshot).
//

using System;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Ide.Projects
{
	static class CellRendererDrawing
	{
		/// <summary>Whether the cell is drawn selected (GTK 2 <c>StateType.Selected</c>/<c>Active</c>).</summary>
		public static bool IsSelected (CellRendererState flags)
		{
			return (flags & CellRendererState.Selected) != 0;
		}

		/// <summary>Text colour of a cell: the theme selection colour when selected, otherwise the widget's foreground.</summary>
		public static Cairo.Color GetTextColor (Widget widget, CellRendererState flags)
		{
			if (IsSelected (flags))
				return Styles.BaseSelectionTextColor.ToCairoColor ();
			var color = widget.StyleColor;
			if ((flags & CellRendererState.Insensitive) != 0)
				color.Alpha *= 0.5f;
			return color.ToCairoColor ();
		}

		/// <summary>Draws <paramref name="layout"/> at the given position with <paramref name="color"/>.</summary>
		public static void DrawLayout (Cairo.Context ctx, Cairo.Color color, int x, int y, Pango.Layout layout)
		{
			ctx.MoveTo (x, y);
			ctx.SetSourceColor (color);
			ctx.ShowLayout (layout);
		}
	}

	/// <summary>
	/// Per-widget CSS (replaces the GTK 2 <c>ModifyBg</c>/<c>ModifyFg</c>/<c>ModifyFont</c>): each styled widget gets a
	/// private CSS class and provider.
	/// </summary>
	static class WidgetCss
	{
		sealed class Entry
		{
			public Gtk.CssProvider Provider;
			public string CssClass;
		}

		static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, Entry> table = new System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, Entry> ();
		static int counter;

		/// <summary>Sets the background colour of <paramref name="widget"/>.</summary>
		public static void SetBackground (Gtk.Widget widget, Xwt.Drawing.Color color)
		{
			Set (widget, "background-color: " + ToCss (color) + ";");
		}

		/// <summary>Sets the foreground colour of <paramref name="widget"/> (and of its descendants).</summary>
		public static void SetForeground (Gtk.Widget widget, Xwt.Drawing.Color color)
		{
			Set (widget, "color: " + ToCss (color) + ";");
		}

		/// <summary>Applies the CSS declarations <paramref name="declarations"/> to <paramref name="widget"/> only; replaces earlier ones.</summary>
		public static void Set (Gtk.Widget widget, string declarations)
		{
			if (!table.TryGetValue (widget, out var entry)) {
				entry = new Entry {
					Provider = new Gtk.CssProvider (),
					CssClass = "md-projects-" + (++counter),
				};
				table.Add (widget, entry);
				widget.AddCssClass (entry.CssClass);
#pragma warning disable CS0612, CS0618 // StyleContext.AddProvider is deprecated in GTK 4 but is the only per-widget provider API
				widget.StyleContext.AddProvider (entry.Provider, (uint)Gtk.StyleProviderPriority.Application);
#pragma warning restore CS0612, CS0618
			}
			entry.Provider.LoadFromString ("." + entry.CssClass + " { " + declarations + " }");
		}

		static string ToCss (Xwt.Drawing.Color color)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "rgba({0},{1},{2},{3})",
				(int)Math.Round (color.Red * 255), (int)Math.Round (color.Green * 255), (int)Math.Round (color.Blue * 255), color.Alpha);
		}
	}
}
