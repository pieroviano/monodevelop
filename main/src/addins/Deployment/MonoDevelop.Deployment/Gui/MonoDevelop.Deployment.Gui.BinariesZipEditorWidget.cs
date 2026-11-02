
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class BinariesZipEditorWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label4;
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText comboConfiguration;
		private global::Gtk.ComboBoxText comboPlatform;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry entryZip;
		private global::Gtk.ComboBoxText comboZip;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label5;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (12);
			DesignerLayout.Margin (this.vbox2, 6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the archive file name and location:");
			DesignerLayout.Pack (this.vbox2, this.label4);
			this.table1 = DesignerLayout.Grid (0, 0);
			this.comboConfiguration = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboConfiguration, 1, 0);
			this.comboPlatform = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboPlatform, 1, 1);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Attach (this.table1, this.folderEntry, 1, 2, hexpand: true);
			this.hbox1 = DesignerLayout.HBox (6);
			this.entryZip = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox1, this.entryZip, true);
			this.comboZip = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboZip);
			DesignerLayout.Attach (this.table1, this.hbox1, 1, 3, hexpand: true);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Configuration:");
			DesignerLayout.Attach (this.table1, this.label1, 0, 0);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Target folder:");
			DesignerLayout.Attach (this.table1, this.label2, 0, 2);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("File:");
			DesignerLayout.Attach (this.table1, this.label3, 0, 3);
			this.label5 = new global::Gtk.Label ((string)null);
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Target platform:");
			DesignerLayout.Attach (this.table1, this.label5, 0, 1);
			DesignerLayout.Pack (this.vbox2, this.table1);
			DesignerLayout.SetContent (this, this.vbox2);
			this.entryZip.Changed += new global::System.EventHandler (this.OnEntryZipChanged);
			this.comboZip.Changed += new global::System.EventHandler (this.OnComboZipChanged);
			this.folderEntry.PathChanged += new global::System.EventHandler (this.OnFolderEntryPathChanged);
			this.comboPlatform.Changed += new global::System.EventHandler (this.OnComboPlatformChanged);
			this.comboConfiguration.Changed += new global::System.EventHandler (this.OnComboConfigurationChanged);
		}
	}
}
