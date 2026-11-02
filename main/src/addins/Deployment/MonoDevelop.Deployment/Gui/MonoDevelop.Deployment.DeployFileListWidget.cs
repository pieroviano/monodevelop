
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment
{
	internal partial class DeployFileListWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label labelFiles;
		private global::Gtk.ComboBoxText comboConfigs;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView fileList;
		private global::Gtk.Box hbox2;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.labelFiles = new global::Gtk.Label ((string)null);
			this.labelFiles.Xalign = 0F;
			this.labelFiles.LabelProp = global::Mono.Unix.Catalog.GetString ("The following files will be included in the package:");
			DesignerLayout.Pack (this.hbox1, this.labelFiles);
			this.comboConfigs = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboConfigs);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.fileList = new global::Gtk.TreeView ();
			this.fileList.EnableGridLines = global::Gtk.TreeViewGridLines.Horizontal;
			this.scrolledwindow1.Child = this.fileList;
			DesignerLayout.Pack (this.vbox2, this.scrolledwindow1, true);
			this.hbox2 = DesignerLayout.HBox (6);
			DesignerLayout.Pack (this.vbox2, this.hbox2);
			DesignerLayout.SetContent (this, this.vbox2);
			this.comboConfigs.Changed += new global::System.EventHandler (this.OnComboConfigsChanged);
		}
	}
}
