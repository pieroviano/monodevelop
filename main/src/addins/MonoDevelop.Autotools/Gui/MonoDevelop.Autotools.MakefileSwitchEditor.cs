
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Autotools
{
	public partial class MakefileSwitchEditor
	{
		private global::Gtk.Box align;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Switch editor");
			var content = DesignerLayout.ContentArea (this, 2);
			this.align = DesignerLayout.VBox (0);
			DesignerLayout.Pack (content, this.align, true);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "_Cancel";
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (400, 300);
		}
	}
}
