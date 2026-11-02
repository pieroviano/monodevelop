// Construction helpers for the hand-ported GTK 4 Build () methods of the former GUI designer files in this folder.
namespace MonoDevelop.VBNetBinding
{
	static class DesignerLayout
	{
		public static Gtk.Box VBox (int spacing = 6)
		{
			return new Gtk.Box (Gtk.Orientation.Vertical, spacing);
		}

		public static Gtk.Box HBox (int spacing = 6)
		{
			return new Gtk.Box (Gtk.Orientation.Horizontal, spacing);
		}

		public static Gtk.Grid Grid ()
		{
			return new Gtk.Grid { RowSpacing = 6, ColumnSpacing = 6 };
		}

		/// <summary>Left aligned label, optionally with markup.</summary>
		public static Gtk.Label Label (string text, bool markup = false)
		{
			var label = new Gtk.Label ((string)null) { Xalign = 0f, Halign = Gtk.Align.Start };
			if (markup)
				label.UseMarkup = true;
			label.LabelProp = text;
			return label;
		}

		/// <summary>Attaches a caption label in column 0 and the editor widget in column 1 of <paramref name="row"/>.</summary>
		public static void AttachRow (Gtk.Grid grid, int row, Gtk.Label caption, Gtk.Widget editor)
		{
			grid.Attach (caption, 0, row, 1, 1);
			editor.Hexpand = true;
			grid.Attach (editor, 1, row, 1, 1);
		}

		/// <summary>Bold section header followed by <paramref name="content"/> indented by 18 pixels.</summary>
		public static void AppendSection (Gtk.Box box, Gtk.Label header, Gtk.Widget content)
		{
			box.Append (header);
			content.MarginStart = 18;
			box.Append (content);
		}
	}
}
