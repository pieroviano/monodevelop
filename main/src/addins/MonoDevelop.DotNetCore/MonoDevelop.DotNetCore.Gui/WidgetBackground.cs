//
// WidgetBackground.cs
//
// Sets the background colour of a GTK 4 widget through a private CSS provider
// (replaces the GTK 2 Widget.ModifyBg call on an EventBox).
//

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MonoDevelop.DotNetCore.Gui
{
	static class WidgetBackground
	{
		sealed class Entry
		{
			public Gtk.CssProvider Provider;
			public string CssClass;
		}

		static readonly ConditionalWeakTable<Gtk.Widget, Entry> table = new ConditionalWeakTable<Gtk.Widget, Entry> ();
		static int counter;

		public static void Set (Gtk.Widget widget, Xwt.Drawing.Color color)
		{
			if (!table.TryGetValue (widget, out var entry)) {
				entry = new Entry {
					Provider = new Gtk.CssProvider (),
					CssClass = "md-dotnetcore-bg-" + (++counter),
				};
				table.Add (widget, entry);
				widget.AddCssClass (entry.CssClass);
#pragma warning disable CS0612, CS0618 // StyleContext.AddProvider is deprecated in GTK 4 but is the only per-widget provider API
				widget.StyleContext.AddProvider (entry.Provider, (uint)Gtk.StyleProviderPriority.Application);
#pragma warning restore CS0612, CS0618
			}
			entry.Provider.LoadFromString ("." + entry.CssClass + " { background-color: " + ToCss (color) + "; }");
		}

		static string ToCss (Xwt.Drawing.Color color)
		{
			return string.Format (CultureInfo.InvariantCulture, "rgba({0},{1},{2},{3})",
				(int)Math.Round (color.Red * 255), (int)Math.Round (color.Green * 255), (int)Math.Round (color.Blue * 255), color.Alpha);
		}
	}
}
