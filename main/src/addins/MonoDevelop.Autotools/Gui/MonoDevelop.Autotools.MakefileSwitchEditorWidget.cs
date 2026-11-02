
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Autotools
{
	public partial class MakefileSwitchEditorWidget
	{
		private global::Gtk.Box dialog1_VBox;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Box alignment2;
		private global::Gtk.Label label2;
		private global::Gtk.Box alignment4;
		private global::Gtk.Box hbox1;
		private global::Gtk.Button addBtn;
		private global::Gtk.Button remBtn;
		private global::Gtk.Box alignment3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView itemTv;

		protected virtual void Build ()
		{
			this.dialog1_VBox = DesignerLayout.VBox (0);
			DesignerLayout.Margin (this.dialog1_VBox, 11);
			this.alignment1 = DesignerLayout.VBox (0);
			this.vbox2 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Switch list</b>");
			this.label1.UseMarkup = true;
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.alignment2 = DesignerLayout.VBox (0);
			DesignerLayout.Margin (this.alignment2, 3);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Use the following list to define new switches usable with autotools configure script. Each switch is mapped to a define that you can use to do conditional compilation in your source files.");
			this.label2.Wrap = true;
			this.label2.WidthChars = 78;
			DesignerLayout.Pack (this.alignment2, this.label2, true);
			this.label2.Hexpand = true;
			DesignerLayout.Pack (this.vbox2, this.alignment2);
			this.alignment4 = DesignerLayout.VBox (0);
			this.hbox1 = DesignerLayout.HBox (6);
			DesignerLayout.Margin (this.hbox1, 3);
			this.addBtn = new global::Gtk.Button ();
			this.addBtn.UseUnderline = true;
			this.addBtn.Label = "_Add";
			DesignerLayout.Pack (this.hbox1, this.addBtn);
			this.remBtn = new global::Gtk.Button ();
			this.remBtn.UseUnderline = true;
			this.remBtn.Label = "_Remove";
			DesignerLayout.Pack (this.hbox1, this.remBtn);
			DesignerLayout.Pack (this.alignment4, this.hbox1, true);
			this.hbox1.Hexpand = true;
			DesignerLayout.Pack (this.vbox2, this.alignment4);
			this.alignment3 = DesignerLayout.VBox (0);
			this.alignment3.MarginBottom = 8;
			DesignerLayout.Margin (this.alignment3, 3);
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.itemTv = new global::Gtk.TreeView ();
			this.GtkScrolledWindow.Child = this.itemTv;
			DesignerLayout.Pack (this.alignment3, this.GtkScrolledWindow, true);
			this.GtkScrolledWindow.Hexpand = true;
			DesignerLayout.Pack (this.vbox2, this.alignment3, true);
			DesignerLayout.Pack (this.alignment1, this.vbox2, true);
			this.vbox2.Hexpand = true;
			DesignerLayout.Pack (this.dialog1_VBox, this.alignment1, true);
			DesignerLayout.SetContent (this, this.dialog1_VBox);
			this.addBtn.Clicked += new global::System.EventHandler (this.OnAddBtnClicked);
			this.remBtn.Clicked += new global::System.EventHandler (this.OnRemBtnClicked);
		}
	}
}
