// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class TextStylePolicyPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.SpinButton columnWidthSpin;
		private global::Gtk.Label label5;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label6;
		private global::Gtk.ComboBoxText lineEndingCombo;
		private global::Gtk.Label label3;
		private global::Gtk.Box vbox2;
		private global::Gtk.Grid table1;
		private global::Gtk.SpinButton indentWidthSpin;
		private global::Gtk.Label label7;
		private global::Gtk.Label label9;
		private global::Gtk.SpinButton tabWidthSpin;
		private global::Gtk.CheckButton tabsToSpaceCheck;
		private global::Gtk.CheckButton tabsAfterNonTabsCheck;
		private global::Gtk.CheckButton removeTrailingWhitespaceCheck;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.columnWidthSpin = new global::Gtk.SpinButton (0, 1000, 1);
			this.columnWidthSpin.Adjustment.PageIncrement = 10;
			this.columnWidthSpin.ClimbRate = 1;
			this.columnWidthSpin.Numeric = true;
			this.columnWidthSpin.Value = 120;
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Desired _file width:");
			this.label1.UseUnderline = true;
			this.label1.MnemonicWidget = this.columnWidthSpin;
			DesignerLayout.Pack (this.hbox1, this.label1);
			DesignerLayout.Pack (this.hbox1, this.columnWidthSpin);
			this.label5 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("columns"));
			DesignerLayout.Pack (this.hbox1, this.label5);
			DesignerLayout.Pack (this.vbox1, this.hbox1);
			this.hbox2 = DesignerLayout.HBox (6);
			this.lineEndingCombo = new global::Gtk.ComboBoxText ();
			this.label6 = new global::Gtk.Label ((string)null);
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("_Line endings:");
			this.label6.UseUnderline = true;
			this.label6.MnemonicWidget = this.lineEndingCombo;
			DesignerLayout.Pack (this.hbox2, this.label6);
			DesignerLayout.Pack (this.hbox2, this.lineEndingCombo);
			DesignerLayout.Pack (this.vbox1, this.hbox2);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Whitespace</b>"), true);
			this.label3.MarginTop = 6;
			DesignerLayout.Pack (this.vbox1, this.label3);
			this.vbox2 = DesignerLayout.VBox (6);
			this.vbox2.MarginStart = 12;
			this.table1 = DesignerLayout.Grid (6, 6);
			this.tabWidthSpin = new global::Gtk.SpinButton (1, 50, 1);
			this.tabWidthSpin.Adjustment.PageIncrement = 2;
			this.tabWidthSpin.ClimbRate = 1;
			this.tabWidthSpin.Numeric = true;
			this.tabWidthSpin.Value = 4;
			this.indentWidthSpin = new global::Gtk.SpinButton (1, 50, 1);
			this.indentWidthSpin.Adjustment.PageIncrement = 2;
			this.indentWidthSpin.ClimbRate = 1;
			this.indentWidthSpin.Numeric = true;
			this.indentWidthSpin.Value = 4;
			this.label7 = new global::Gtk.Label ((string)null);
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("_Tab width:");
			this.label7.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.label7, 0, 0);
			DesignerLayout.Attach (this.table1, this.tabWidthSpin, 1, 0);
			this.label9 = new global::Gtk.Label ((string)null);
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("_Indent width:");
			this.label9.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.label9, 0, 1);
			DesignerLayout.Attach (this.table1, this.indentWidthSpin, 1, 1);
			DesignerLayout.Pack (this.vbox2, this.table1);
			this.tabsToSpaceCheck = new global::Gtk.CheckButton ();
			this.tabsToSpaceCheck.Label = global::Mono.Unix.Catalog.GetString ("_Convert tabs to spaces");
			this.tabsToSpaceCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.tabsToSpaceCheck);
			this.tabsAfterNonTabsCheck = new global::Gtk.CheckButton ();
			this.tabsAfterNonTabsCheck.Label = global::Mono.Unix.Catalog.GetString ("_Allow tabs after non-tabs");
			this.tabsAfterNonTabsCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.tabsAfterNonTabsCheck);
			this.removeTrailingWhitespaceCheck = new global::Gtk.CheckButton ();
			this.removeTrailingWhitespaceCheck.Label = global::Mono.Unix.Catalog.GetString ("_Remove trailing whitespace");
			this.removeTrailingWhitespaceCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.removeTrailingWhitespaceCheck);
			DesignerLayout.Pack (this.vbox1, this.vbox2);
			DesignerLayout.SetContent (this, this.vbox1);
			this.label7.MnemonicWidget = this.tabWidthSpin;
			this.label9.MnemonicWidget = this.tabWidthSpin;
			this.columnWidthSpin.ValueChanged += new global::System.EventHandler (this.UpdateState);
			this.lineEndingCombo.Changed += new global::System.EventHandler (this.UpdateState);
			this.tabWidthSpin.ValueChanged += new global::System.EventHandler (this.UpdateState);
			this.indentWidthSpin.ValueChanged += new global::System.EventHandler (this.UpdateState);
			this.tabsToSpaceCheck.Toggled += new global::System.EventHandler (this.UpdateState);
			this.tabsAfterNonTabsCheck.Toggled += new global::System.EventHandler (this.UpdateState);
			this.removeTrailingWhitespaceCheck.Toggled += new global::System.EventHandler (this.UpdateState);
		}
	}
}
