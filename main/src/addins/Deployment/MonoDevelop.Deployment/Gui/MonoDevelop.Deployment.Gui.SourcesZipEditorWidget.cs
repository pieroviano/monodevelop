
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class SourcesZipEditorWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label4;
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText comboFormat;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry entryZip;
		private global::Gtk.ComboBoxText comboZip;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (12);
			DesignerLayout.Margin (this.vbox2, 6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the archive file name and format:");
			DesignerLayout.Pack (this.vbox2, this.label4);
			this.table1 = DesignerLayout.Grid (0, 0);
			this.comboFormat = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboFormat, 1, 0, hexpand: true);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Attach (this.table1, this.folderEntry, 1, 1);
			this.hbox1 = DesignerLayout.HBox (6);
			this.entryZip = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox1, this.entryZip, true);
			this.comboZip = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboZip);
			DesignerLayout.Attach (this.table1, this.hbox1, 1, 2);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("File:");
			DesignerLayout.Attach (this.table1, this.label1, 0, 2);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Target folder:");
			DesignerLayout.Attach (this.table1, this.label2, 0, 1);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("File format:");
			DesignerLayout.Attach (this.table1, this.label3, 0, 0);
			DesignerLayout.Pack (this.vbox2, this.table1);
			DesignerLayout.SetContent (this, this.vbox2);
			this.entryZip.Changed += new global::System.EventHandler (this.OnEntryZipChanged);
			this.comboZip.Changed += new global::System.EventHandler (this.OnComboZipChanged);
			this.folderEntry.PathChanged += new global::System.EventHandler (this.OnFolderEntryPathChanged);
			this.comboFormat.Changed += new global::System.EventHandler (this.OnComboFormatChanged);
		}
	}
}
