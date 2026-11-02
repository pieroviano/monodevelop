using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class AddMimeTypeDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Entry entry;
		private global::Gtk.Box hbox1;
		private global::MonoDevelop.Components.ImageView image;
		private global::Gtk.Label labelDesc;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Add File Type");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Enter a file extension or a mime type name:"));
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.entry = new global::Gtk.Entry ();
			this.entry.CanFocus = true;
			this.entry.Editable = true;
			DesignerLayout.Pack (this.vbox2, this.entry);
			this.hbox1 = DesignerLayout.HBox (6);
			this.image = new global::MonoDevelop.Components.ImageView ();
			this.image.IconId = "gtk-dialog-error";
			this.image.IconSize = StockIconSize.Menu;
			DesignerLayout.Pack (this.hbox1, this.image);
			this.labelDesc = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.hbox1, this.labelDesc, true);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			DesignerLayout.Pack (content, this.vbox2);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_Add");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (400, 164);
			this.image.Visible = false;
			this.entry.Changed += new global::System.EventHandler (this.OnEntryChanged);
		}
	}
}
