// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace Mono.Instrumentation.Monitor
{
	public partial class NewProfile
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Entry entryName;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("New View");
			this.DefaultWidth = 392;
			this.DefaultHeight = 136;
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("View Name:"));
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Editable = true;
			DesignerLayout.Pack (this.vbox2, this.entryName);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonCancel = DesignerLayout.AddActionButton (this, DesignerLayout.Button ("_Cancel"), global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.AddActionButton (this, DesignerLayout.Button ("_OK"), global::Gtk.ResponseType.Ok, true);
		}
	}
}
