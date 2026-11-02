using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	public partial class BaseDirectoryPanelWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::MonoDevelop.Components.FolderEntry folderentry;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Location of Files</b>"), true);
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.WidthRequest = 12;
			DesignerLayout.Pack (this.hbox1, this.label2);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Root directory:"));
			DesignerLayout.Pack (this.hbox1, this.label3);
			this.folderentry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Pack (this.hbox1, this.folderentry, true);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			DesignerLayout.SetContent (this, this.vbox2);
		}
	}
}
