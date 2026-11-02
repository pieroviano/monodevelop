
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class LocalFileCopyConfigurationEditorWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::MonoDevelop.Components.FolderEntry folderEntry;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Target directory:");
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Pack (this.vbox2, this.folderEntry);
			DesignerLayout.SetContent (this, this.vbox2);
			this.folderEntry.PathChanged += new global::System.EventHandler (this.OnFolderEntryPathChanged);
		}
	}
}
