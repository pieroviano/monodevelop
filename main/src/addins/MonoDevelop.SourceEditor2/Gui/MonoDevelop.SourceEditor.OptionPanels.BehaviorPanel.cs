// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	partial class BehaviorPanel
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label GtkLabel5;
		private global::Gtk.Box vbox4;
		private global::Gtk.CheckButton autoInsertBraceCheckbutton;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label fixed1;
		private global::Gtk.CheckButton smartSemicolonPlaceCheckbutton;
		private global::Gtk.CheckButton checkbuttonFormatOnSave;
		private global::Gtk.CheckButton checkbuttonAutoSetSearchPatternCasing;
		private global::Gtk.CheckButton checkbuttonGenerateFormattingUndoStep;
		private global::Gtk.CheckButton checkbuttonEnableSelectionSurrounding;
		private global::Gtk.Label GtkLabel8;
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBoxText indentationCombobox;
		private global::Gtk.CheckButton tabAsReindentCheckbutton;
		private global::Gtk.CheckButton smartBackspaceCheckbutton;
		private global::Gtk.Label GtkLabel10;
		private global::Gtk.Box vbox5;
		private global::Gtk.Box hbox3;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBoxText controlLeftRightCombobox;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);

			this.GtkLabel5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Automatic behaviors</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel5);

			this.vbox4 = DesignerLayout.VBox (6);
			this.vbox4.MarginStart = 12;
			this.autoInsertBraceCheckbutton = new global::Gtk.CheckButton ();
			this.autoInsertBraceCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Insert matching brace");
			this.autoInsertBraceCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.autoInsertBraceCheckbutton);

			this.hbox2 = DesignerLayout.HBox (6);
			this.fixed1 = new global::Gtk.Label ();
			this.fixed1.MarginStart = 6;
			this.fixed1.MarginEnd = 6;
			DesignerLayout.Pack (this.hbox2, this.fixed1);
			this.smartSemicolonPlaceCheckbutton = new global::Gtk.CheckButton ();
			this.smartSemicolonPlaceCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Smart semicolon placement");
			this.smartSemicolonPlaceCheckbutton.Active = true;
			this.smartSemicolonPlaceCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.hbox2, this.smartSemicolonPlaceCheckbutton);
			DesignerLayout.Pack (this.vbox4, this.hbox2);

			this.checkbuttonFormatOnSave = new global::Gtk.CheckButton ();
			this.checkbuttonFormatOnSave.Label = global::Mono.Unix.Catalog.GetString ("_Format document on save");
			this.checkbuttonFormatOnSave.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.checkbuttonFormatOnSave);

			this.checkbuttonAutoSetSearchPatternCasing = new global::Gtk.CheckButton ();
			this.checkbuttonAutoSetSearchPatternCasing.Label = global::Mono.Unix.Catalog.GetString ("_Automatically set search pattern case sensitivity");
			this.checkbuttonAutoSetSearchPatternCasing.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.checkbuttonAutoSetSearchPatternCasing);

			this.checkbuttonGenerateFormattingUndoStep = new global::Gtk.CheckButton ();
			this.checkbuttonGenerateFormattingUndoStep.Label = global::Mono.Unix.Catalog.GetString ("_Generate additional undo steps for formatting");
			this.checkbuttonGenerateFormattingUndoStep.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.checkbuttonGenerateFormattingUndoStep);

			this.checkbuttonEnableSelectionSurrounding = new global::Gtk.CheckButton ();
			this.checkbuttonEnableSelectionSurrounding.Label = global::Mono.Unix.Catalog.GetString ("Enable _selection surrounding keys");
			this.checkbuttonEnableSelectionSurrounding.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.checkbuttonEnableSelectionSurrounding);
			DesignerLayout.Pack (this.vbox1, this.vbox4);

			this.GtkLabel8 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Indentation</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel8);

			this.vbox2 = DesignerLayout.VBox (6);
			this.vbox2.MarginStart = 12;
			this.hbox1 = DesignerLayout.HBox (6);
			this.indentationCombobox = new global::Gtk.ComboBoxText ();
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Indentation mode:"), false, this.indentationCombobox);
			DesignerLayout.Pack (this.hbox1, this.label1);
			DesignerLayout.Pack (this.hbox1, this.indentationCombobox);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			this.tabAsReindentCheckbutton = new global::Gtk.CheckButton ();
			this.tabAsReindentCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Interpret tab _keystroke as reindent command");
			this.tabAsReindentCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.tabAsReindentCheckbutton);
			this.smartBackspaceCheckbutton = new global::Gtk.CheckButton ();
			this.smartBackspaceCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Backspace removes indentation");
			this.smartBackspaceCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.smartBackspaceCheckbutton);
			DesignerLayout.Pack (this.vbox1, this.vbox2);

			this.GtkLabel10 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Navigation</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel10);

			this.vbox5 = DesignerLayout.VBox (6);
			this.vbox5.MarginStart = 12;
			this.hbox3 = DesignerLayout.HBox (6);
			this.controlLeftRightCombobox = new global::Gtk.ComboBoxText ();
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Word _break mode:"), false, this.controlLeftRightCombobox);
			DesignerLayout.Pack (this.hbox3, this.label2);
			DesignerLayout.Pack (this.hbox3, this.controlLeftRightCombobox);
			DesignerLayout.Pack (this.vbox5, this.hbox3);
			DesignerLayout.Pack (this.vbox1, this.vbox5);

			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
