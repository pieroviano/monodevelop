// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	partial class MarkerPanel
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label GtkLabel9;
		private global::Gtk.Box vbox3;
		private global::Gtk.CheckButton showLineNumbersCheckbutton;
		private global::Gtk.CheckButton highlightMatchingBracketCheckbutton;
		private global::Gtk.CheckButton highlightCurrentLineCheckbutton;
		private global::Gtk.CheckButton showRulerCheckbutton;
		private global::Gtk.CheckButton enableAnimationCheckbutton1;
		private global::Gtk.CheckButton enableHighlightUsagesCheckbutton;
		private global::Gtk.CheckButton drawIndentMarkersCheckbutton;
		private global::Gtk.CheckButton enableQuickDiffCheckbutton;
		private global::Gtk.CheckButton showProcedureLineSeparatorsCheckbutton;
		private global::Gtk.Grid table1;
		private global::Gtk.CheckButton checkbuttonLineEndings;
		private global::Gtk.CheckButton checkbuttonSpaces;
		private global::Gtk.CheckButton checkbuttonTabs;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBoxText showWhitespacesCombobox;

		static global::Gtk.CheckButton Check (global::Gtk.Box parent, string label)
		{
			var check = new global::Gtk.CheckButton ();
			check.Label = label;
			check.UseUnderline = true;
			DesignerLayout.Pack (parent, check);
			return check;
		}

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.GtkLabel9 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>General</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel9);

			this.vbox3 = DesignerLayout.VBox (6);
			this.vbox3.MarginStart = 12;
			this.showLineNumbersCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Show line numbers"));
			this.highlightMatchingBracketCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Highlight matching braces"));
			this.highlightCurrentLineCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("Highlight _current line"));
			this.showRulerCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("Show _column ruler"));
			this.enableAnimationCheckbutton1 = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Enable animations"));
			this.enableHighlightUsagesCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("Highlight _identifier references"));
			this.drawIndentMarkersCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Show indentation guides"));
			this.enableQuickDiffCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Visualize changed lines"));
			this.showProcedureLineSeparatorsCheckbutton = Check (this.vbox3, global::Mono.Unix.Catalog.GetString ("_Show procedure line separators"));

			this.table1 = DesignerLayout.Grid (6, 6);
			this.showWhitespacesCombobox = new global::Gtk.ComboBoxText ();
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Show invisible characters:"), false, this.showWhitespacesCombobox);
			DesignerLayout.Attach (this.table1, this.label1, 0, 0);
			DesignerLayout.Attach (this.table1, this.showWhitespacesCombobox, 1, 0, 3);
			this.checkbuttonSpaces = new global::Gtk.CheckButton ();
			this.checkbuttonSpaces.Label = global::Mono.Unix.Catalog.GetString ("Include _Spaces");
			this.checkbuttonSpaces.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonSpaces, 1, 1, 3);
			this.checkbuttonTabs = new global::Gtk.CheckButton ();
			this.checkbuttonTabs.Label = global::Mono.Unix.Catalog.GetString ("Include Tabs");
			this.checkbuttonTabs.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonTabs, 1, 2, 3);
			this.checkbuttonLineEndings = new global::Gtk.CheckButton ();
			this.checkbuttonLineEndings.Label = global::Mono.Unix.Catalog.GetString ("Include Line Endings");
			this.checkbuttonLineEndings.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonLineEndings, 1, 3, 3);
			DesignerLayout.Pack (this.vbox3, this.table1);

			DesignerLayout.Pack (this.vbox1, this.vbox3);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
