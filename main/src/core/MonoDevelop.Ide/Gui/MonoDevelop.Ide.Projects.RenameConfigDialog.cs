using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class RenameConfigDialog
	{
		private global::Gtk.Box vbox79;
		private global::Gtk.Box hbox63;
		private global::Gtk.Label label106;
		private global::Gtk.Entry nameEntry;
		private global::Gtk.CheckButton renameChildrenCheck;
		private global::Gtk.Button button9;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Rename Configuration");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox79 = DesignerLayout.Margin (DesignerLayout.VBox (6), 7);
			this.hbox63 = DesignerLayout.HBox (6);
			this.label106 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("New name:"));
			DesignerLayout.Pack (this.hbox63, this.label106);
			this.nameEntry = new global::Gtk.Entry ();
			this.nameEntry.Editable = true;
			DesignerLayout.Pack (this.hbox63, this.nameEntry, true);
			DesignerLayout.Pack (this.vbox79, this.hbox63);
			this.renameChildrenCheck = new global::Gtk.CheckButton ();
			this.renameChildrenCheck.Label = global::Mono.Unix.Catalog.GetString ("Rename configurations in all solution items");
			this.renameChildrenCheck.Active = true;
			this.renameChildrenCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox79, this.renameChildrenCheck);
			DesignerLayout.Pack (content, this.vbox79, true);
			this.button9 = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.button9, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.None);
			this.SetDefaultSize (371, 149);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
