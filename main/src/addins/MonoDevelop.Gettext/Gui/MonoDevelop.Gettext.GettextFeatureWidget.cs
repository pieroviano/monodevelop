
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Gettext
{
	internal partial class GettextFeatureWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label4;
		private global::Gtk.Box hbox1;
		private global::Gtk.Box vbox3;
		private global::Gtk.Frame frame1;
		private global::Gtk.TreeView treeviewTranslations;
		private global::Gtk.Box vbox4;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox2, 8);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the languages you want to support (more languages can be added later):");
			DesignerLayout.Pack (this.vbox2, this.label4);
			this.hbox1 = DesignerLayout.HBox (6);
			this.vbox3 = DesignerLayout.VBox (6);
			this.frame1 = new global::Gtk.Frame ();
			this.treeviewTranslations = new global::Gtk.TreeView ();
			this.treeviewTranslations.WidthRequest = 200;
			this.frame1.Child = this.treeviewTranslations;
			DesignerLayout.Pack (this.vbox3, this.frame1, true);
			DesignerLayout.Pack (this.hbox1, this.vbox3, true);
			this.vbox4 = DesignerLayout.VBox (6);
			this.buttonAdd = new global::Gtk.Button ();
			this.buttonAdd.UseUnderline = true;
			this.buttonAdd.Label = "_Add";
			DesignerLayout.Pack (this.vbox4, this.buttonAdd);
			this.buttonRemove = new global::Gtk.Button ();
			this.buttonRemove.UseUnderline = true;
			this.buttonRemove.Label = "_Remove";
			DesignerLayout.Pack (this.vbox4, this.buttonRemove);
			DesignerLayout.Pack (this.hbox1, this.vbox4);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			DesignerLayout.SetContent (this, this.vbox2);
		}
	}
}
