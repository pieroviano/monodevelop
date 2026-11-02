// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.VBNetBinding
{
	public partial class ProjectOptionsPanelWidget
	{
		private global::Gtk.Box vbox5;
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText cmbCodePage;
		private global::Gtk.ComboBoxText cmbOptionCompare;
		private global::Gtk.ComboBoxText cmbOptionExplicit;
		private global::Gtk.ComboBoxText cmbOptionInfer;
		private global::Gtk.ComboBoxText cmbOptionStrict;
		private global::Gtk.ComboBoxText compileTargetCombo;
		private global::Gtk.ComboBoxText entryMainClass;
		private global::MonoDevelop.Components.FileEntry iconEntry;
		private global::Gtk.Label label1;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Label label7;
		private global::Gtk.Label label8;
		private global::Gtk.Label label86;
		private global::Gtk.Label label88;
		private global::Gtk.ComboBoxText txtMyType;

		protected virtual void Build ()
		{
			this.vbox5 = DesignerLayout.VBox ();
			this.table1 = DesignerLayout.Grid ();

			this.label86 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Compile _Target:"));
			this.label86.UseUnderline = true;
			this.compileTargetCombo = new global::Gtk.ComboBoxText ();
			this.label86.MnemonicWidget = this.compileTargetCombo;
			DesignerLayout.AttachRow (this.table1, 0, this.label86, this.compileTargetCombo);

			this.label88 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Main Class:"));
			this.label88.UseUnderline = true;
			this.entryMainClass = global::Gtk.ComboBoxText.NewWithEntry ();
			this.label88.MnemonicWidget = this.entryMainClass;
			DesignerLayout.AttachRow (this.table1, 1, this.label88, this.entryMainClass);

			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Win32 Icon:"));
			this.iconEntry = new global::MonoDevelop.Components.FileEntry ();
			DesignerLayout.AttachRow (this.table1, 2, this.label3, this.iconEntry);

			this.label8 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Compiler Code Page:"));
			this.cmbCodePage = global::Gtk.ComboBoxText.NewWithEntry ();
			DesignerLayout.AttachRow (this.table1, 3, this.label8, this.cmbCodePage);

			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("My Type:"));
			this.txtMyType = global::Gtk.ComboBoxText.NewWithEntry ();
			DesignerLayout.AttachRow (this.table1, 4, this.label4, this.txtMyType);

			this.label5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Option Explicit:"));
			this.cmbOptionExplicit = new global::Gtk.ComboBoxText ();
			DesignerLayout.AttachRow (this.table1, 5, this.label5, this.cmbOptionExplicit);

			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Option Strict:"));
			this.cmbOptionStrict = new global::Gtk.ComboBoxText ();
			DesignerLayout.AttachRow (this.table1, 6, this.label6, this.cmbOptionStrict);

			this.label7 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Option Compare:"));
			this.cmbOptionCompare = new global::Gtk.ComboBoxText ();
			DesignerLayout.AttachRow (this.table1, 7, this.label7, this.cmbOptionCompare);

			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Option Infer:"));
			this.cmbOptionInfer = new global::Gtk.ComboBoxText ();
			DesignerLayout.AttachRow (this.table1, 8, this.label1, this.cmbOptionInfer);

			this.vbox5.Append (this.table1);
			this.Child = this.vbox5;
		}
	}
}
