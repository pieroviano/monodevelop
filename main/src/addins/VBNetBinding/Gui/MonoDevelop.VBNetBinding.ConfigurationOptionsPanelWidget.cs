// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.VBNetBinding
{
	public partial class ConfigurationOptionsPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label82;
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText cmbDebugType;
		private global::Gtk.ComboBoxText cmbDefineDEBUG;
		private global::Gtk.ComboBoxText cmbDefineTRACE;
		private global::Gtk.ComboBoxText cmbOptimize;
		private global::Gtk.Label label86;
		private global::Gtk.Label label87;
		private global::Gtk.Label label88;
		private global::Gtk.Label label94;
		private global::Gtk.Label label83;
		private global::Gtk.Grid table3;
		private global::Gtk.ComboBoxText cmbEnableWarnings;
		private global::Gtk.Label label91;
		private global::Gtk.Label label92;
		private global::Gtk.Label label93;
		private global::Gtk.Entry txtDontWarnAbout;
		private global::Gtk.Entry txtTreatAsError;
		private global::Gtk.Label label85;
		private global::Gtk.Grid table2;
		private global::Gtk.ComboBoxText cmbGenerateXmlDocumentation;
		private global::Gtk.ComboBoxText cmbRemoveIntegerChecks;
		private global::Gtk.Label label89;
		private global::Gtk.Label label90;
		private global::Gtk.Label label95;
		private global::Gtk.Label label96;
		private global::Gtk.Entry txtAdditionalArguments;
		private global::Gtk.Entry txtDefineConstants;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox ();

			// Optimization/Debug options
			this.label82 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Optimization/Debug options</b>"), true);
			this.table1 = DesignerLayout.Grid ();
			this.cmbOptimize = new global::Gtk.ComboBoxText ();
			this.cmbDefineDEBUG = new global::Gtk.ComboBoxText ();
			this.cmbDefineTRACE = new global::Gtk.ComboBoxText ();
			this.cmbDebugType = new global::Gtk.ComboBoxText ();
			this.label94 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Optimize:"));
			this.label87 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Define DEBUG:"));
			this.label88 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Define TRACE:"));
			this.label86 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Debug Type:"));
			DesignerLayout.AttachRow (this.table1, 0, this.label94, this.cmbOptimize);
			DesignerLayout.AttachRow (this.table1, 1, this.label87, this.cmbDefineDEBUG);
			DesignerLayout.AttachRow (this.table1, 2, this.label88, this.cmbDefineTRACE);
			DesignerLayout.AttachRow (this.table1, 3, this.label86, this.cmbDebugType);
			DesignerLayout.AppendSection (this.vbox1, this.label82, this.table1);

			// Warnings
			this.label83 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Warnings</b>"), true);
			this.table3 = DesignerLayout.Grid ();
			this.cmbEnableWarnings = new global::Gtk.ComboBoxText ();
			this.txtDontWarnAbout = new global::Gtk.Entry ();
			this.txtTreatAsError = new global::Gtk.Entry ();
			this.label92 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Enable Warnings:"));
			this.label93 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Don't warn about:"));
			this.label91 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Treat as error:"));
			DesignerLayout.AttachRow (this.table3, 0, this.label92, this.cmbEnableWarnings);
			DesignerLayout.AttachRow (this.table3, 1, this.label93, this.txtDontWarnAbout);
			DesignerLayout.AttachRow (this.table3, 2, this.label91, this.txtTreatAsError);
			DesignerLayout.AppendSection (this.vbox1, this.label83, this.table3);

			// Misc
			this.label85 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Misc</b>"), true);
			this.table2 = DesignerLayout.Grid ();
			this.cmbGenerateXmlDocumentation = new global::Gtk.ComboBoxText ();
			this.cmbRemoveIntegerChecks = new global::Gtk.ComboBoxText ();
			this.txtDefineConstants = new global::Gtk.Entry ();
			this.txtAdditionalArguments = new global::Gtk.Entry ();
			this.label90 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Generate XML documentation:"));
			this.label95 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Remove integer checks:"));
			this.label96 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Define constants:"));
			this.label89 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Additional compiler arguments:"));
			DesignerLayout.AttachRow (this.table2, 0, this.label90, this.cmbGenerateXmlDocumentation);
			DesignerLayout.AttachRow (this.table2, 1, this.label95, this.cmbRemoveIntegerChecks);
			DesignerLayout.AttachRow (this.table2, 2, this.label96, this.txtDefineConstants);
			DesignerLayout.AttachRow (this.table2, 3, this.label89, this.txtAdditionalArguments);
			DesignerLayout.AppendSection (this.vbox1, this.label85, this.table2);

			this.Child = this.vbox1;
		}
	}
}
