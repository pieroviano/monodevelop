
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Linux
{
	public partial class MenuCategorySelectorDialog
	{
		private global::Gtk.Box vbox5;
		private global::Gtk.Label label19;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView tree;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Menu Category Selection");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox5 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox5, 6);
			this.label19 = new global::Gtk.Label ((string)null);
			this.label19.Xalign = 0F;
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the categories you want to assign to the desktop entry:");
			DesignerLayout.Pack (this.vbox5, this.label19);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.tree = new global::Gtk.TreeView ();
			this.scrolledwindow1.Child = this.tree;
			DesignerLayout.Pack (this.vbox5, this.scrolledwindow1, true);
			DesignerLayout.Pack (content, this.vbox5, true);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "_Cancel";
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (652, 484);
		}
	}
}
