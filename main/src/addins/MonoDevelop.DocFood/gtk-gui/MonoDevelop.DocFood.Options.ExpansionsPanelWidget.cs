
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.DocFood.Options
{
	internal partial class ExpansionsPanelWidget
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TextView textview1;
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Box hbox3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeviewAcronyms;
		private global::Gtk.Box vbox2;
		private global::Gtk.Button button1;
		private global::Gtk.Button button2;

		protected virtual void Build ()
		{
			this.hbox1 = DesignerLayout.HBox (6);
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.Buffer.Text = "All words that consist of only consonants (like sql) and/or upper case letters (like HTML) are treated as acronyms. However the acronym recognition can be improved by defining acronyms.";
			this.textview1.Editable = false;
			this.textview1.CursorVisible = false;
			this.textview1.WrapMode = ((global::Gtk.WrapMode)(2));
			this.GtkScrolledWindow1.Child = this.textview1;
			DesignerLayout.Pack (this.hbox1, this.GtkScrolledWindow1, true);
			this.vbox1 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("_Acronyms:");
			this.label1.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.label1);
			this.hbox3 = DesignerLayout.HBox (6);
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.treeviewAcronyms = new global::Gtk.TreeView ();
			this.GtkScrolledWindow.Child = this.treeviewAcronyms;
			DesignerLayout.Pack (this.hbox3, this.GtkScrolledWindow, true);
			this.vbox2 = DesignerLayout.VBox (6);
			this.button1 = new global::Gtk.Button ();
			this.button1.UseUnderline = true;
			this.button1.Label = "_Add";
			DesignerLayout.Pack (this.vbox2, this.button1);
			this.button2 = new global::Gtk.Button ();
			this.button2.UseUnderline = true;
			this.button2.Label = "_Remove";
			DesignerLayout.Pack (this.vbox2, this.button2);
			DesignerLayout.Pack (this.hbox3, this.vbox2);
			DesignerLayout.Pack (this.vbox1, this.hbox3);
			DesignerLayout.Pack (this.hbox1, this.vbox1);
			DesignerLayout.SetContent (this, this.hbox1);
			this.label1.MnemonicWidget = this.treeviewAcronyms;
		}
	}
}
