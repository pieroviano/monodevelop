
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class InstallDialog
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label1;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Label label2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry nameEntry;
		private global::Gtk.Button button19;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Install Project");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox1 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox1, 12);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Directory prefix:");
			DesignerLayout.Pack (this.vbox1, this.label1);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			this.folderEntry.BrowserTitle = "Select Directory";
			DesignerLayout.Pack (this.vbox1, this.folderEntry);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Application name (to use as subdirectory name in the installation prefix):");
			DesignerLayout.Pack (this.vbox1, this.label2);
			this.hbox1 = DesignerLayout.HBox (0);
			this.nameEntry = new global::Gtk.Entry ();
			this.nameEntry.WidthRequest = 275;
			DesignerLayout.Pack (this.hbox1, this.nameEntry);
			DesignerLayout.Pack (this.vbox1, this.hbox1);
			DesignerLayout.Pack (content, this.vbox1);
			this.button19 = new global::Gtk.Button ();
			this.button19.UseUnderline = true;
			this.button19.Label = "_Cancel";
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.button19, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (453, 193);
			this.folderEntry.PathChanged += new global::System.EventHandler (this.OnFolderEntryPathChanged);
			this.nameEntry.Changed += new global::System.EventHandler (this.OnNameEntryChanged);
		}
	}
}
