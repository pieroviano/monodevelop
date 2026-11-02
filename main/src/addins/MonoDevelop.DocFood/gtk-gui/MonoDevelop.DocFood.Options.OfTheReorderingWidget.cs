
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.DocFood.Options
{
	internal partial class OfTheReorderingWidget
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TextView textview1;
		private global::Gtk.Box vbox5;
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Box hbox3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeviewAcronyms;
		private global::Gtk.Box vbox2;
		private global::Gtk.Button button2;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label2;
		private global::Gtk.Entry entry1;
		private global::Gtk.Button button1;
		private global::Gtk.Box vbox3;
		private global::Gtk.Label label3;
		private global::Gtk.Box hbox4;
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		private global::Gtk.TreeView treeviewAcronyms1;
		private global::Gtk.Box vbox4;
		private global::Gtk.Button button3;
		private global::Gtk.Box hbox5;
		private global::Gtk.Label label4;
		private global::Gtk.Entry entry2;
		private global::Gtk.Button button4;

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
			this.vbox5 = DesignerLayout.VBox (6);
			this.vbox1 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("\"of the\" reordering words:");
			this.label1.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.label1);
			this.hbox3 = DesignerLayout.HBox (6);
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.treeviewAcronyms = new global::Gtk.TreeView ();
			this.GtkScrolledWindow.Child = this.treeviewAcronyms;
			DesignerLayout.Pack (this.hbox3, this.GtkScrolledWindow, true);
			this.vbox2 = DesignerLayout.VBox (6);
			this.button2 = new global::Gtk.Button ();
			this.button2.UseUnderline = true;
			this.button2.Label = "_Remove";
			DesignerLayout.Pack (this.vbox2, this.button2);
			DesignerLayout.Pack (this.hbox3, this.vbox2);
			DesignerLayout.Pack (this.vbox1, this.hbox3, true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("A_cronym:");
			this.label2.UseUnderline = true;
			DesignerLayout.Pack (this.hbox2, this.label2);
			this.entry1 = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox2, this.entry1, true);
			this.button1 = new global::Gtk.Button ();
			this.button1.UseUnderline = true;
			this.button1.Label = "_Add";
			DesignerLayout.Pack (this.hbox2, this.button1);
			DesignerLayout.Pack (this.vbox1, this.hbox2);
			DesignerLayout.Pack (this.vbox5, this.vbox1, true);
			this.vbox3 = DesignerLayout.VBox (6);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Prefix words for the reordering:");
			this.label3.UseUnderline = true;
			DesignerLayout.Pack (this.vbox3, this.label3);
			this.hbox4 = DesignerLayout.HBox (6);
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.treeviewAcronyms1 = new global::Gtk.TreeView ();
			this.GtkScrolledWindow2.Child = this.treeviewAcronyms1;
			DesignerLayout.Pack (this.hbox4, this.GtkScrolledWindow2, true);
			this.vbox4 = DesignerLayout.VBox (6);
			this.button3 = new global::Gtk.Button ();
			this.button3.UseUnderline = true;
			this.button3.Label = "_Remove";
			DesignerLayout.Pack (this.vbox4, this.button3);
			DesignerLayout.Pack (this.hbox4, this.vbox4);
			DesignerLayout.Pack (this.vbox3, this.hbox4, true);
			this.hbox5 = DesignerLayout.HBox (6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("A_cronym:");
			this.label4.UseUnderline = true;
			DesignerLayout.Pack (this.hbox5, this.label4);
			this.entry2 = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox5, this.entry2, true);
			this.button4 = new global::Gtk.Button ();
			this.button4.UseUnderline = true;
			this.button4.Label = "_Add";
			DesignerLayout.Pack (this.hbox5, this.button4);
			DesignerLayout.Pack (this.vbox3, this.hbox5);
			DesignerLayout.Pack (this.vbox5, this.vbox3, true);
			DesignerLayout.Pack (this.hbox1, this.vbox5);
			DesignerLayout.SetContent (this, this.hbox1);
			this.label1.MnemonicWidget = this.treeviewAcronyms;
			this.label3.MnemonicWidget = this.treeviewAcronyms;
		}
	}
}
