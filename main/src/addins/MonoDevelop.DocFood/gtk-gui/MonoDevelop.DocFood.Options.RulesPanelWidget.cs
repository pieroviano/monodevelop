
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.DocFood.Options
{
	internal partial class RulesPanelWidget
	{
		private global::Gtk.Box vbox4;
		private global::Gtk.Label label2;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.Box hbox2;
		private global::Gtk.Button button3;

		protected virtual void Build ()
		{
			this.vbox4 = DesignerLayout.VBox (6);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("The rules are defined using XML. Click HELP for a full explaination.");
			DesignerLayout.Pack (this.vbox4, this.label2);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			DesignerLayout.Pack (this.vbox4, this.scrolledwindow1, true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.button3 = new global::Gtk.Button ();
			this.button3.UseUnderline = true;
			this.button3.Label = "_Help";
			DesignerLayout.Pack (this.hbox2, this.button3);
			DesignerLayout.Pack (this.vbox4, this.hbox2);
			DesignerLayout.SetContent (this, this.vbox4);
		}
	}
}
