using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.CodeTemplates
{
	internal partial class CodeTemplatePanelWidget
	{
		private global::Gtk.Paned vpaned1;
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeviewCodeTemplates;
		private global::Gtk.Box vbox2;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonEdit;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Box vbox1;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label3;
		private global::Gtk.Fixed fixed1;
		private global::Gtk.CheckButton checkbuttonWhiteSpaces;
		private global::Gtk.ScrolledWindow scrolledwindow1;

		protected virtual void Build ()
		{
			this.vpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Vertical);
			this.vpaned1.CanFocus = true;
			this.vpaned1.Position = 127;
			this.vpaned1.ResizeStartChild = false;
			this.hbox1 = DesignerLayout.HBox (6);
			this.treeviewCodeTemplates = new global::Gtk.TreeView ();
			this.treeviewCodeTemplates.CanFocus = true;
			this.treeviewCodeTemplates.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.treeviewCodeTemplates);
			DesignerLayout.Pack (this.hbox1, this.GtkScrolledWindow, true);
			this.vbox2 = DesignerLayout.VBox (6);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox2, this.buttonAdd);
			this.buttonEdit = DesignerLayout.Button ("_Edit");
			DesignerLayout.Pack (this.vbox2, this.buttonEdit);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox2, this.buttonRemove);
			DesignerLayout.Pack (this.hbox1, this.vbox2);
			this.vpaned1.StartChild = this.hbox1;
			this.vbox1 = DesignerLayout.VBox (6);
			this.hbox2 = DesignerLayout.HBox (6);
			this.label3 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Preview:"));
			DesignerLayout.Pack (this.hbox2, this.label3);
			this.fixed1 = new global::Gtk.Fixed ();
			DesignerLayout.Pack (this.hbox2, this.fixed1, true);
			this.checkbuttonWhiteSpaces = new global::Gtk.CheckButton ();
			this.checkbuttonWhiteSpaces.CanFocus = true;
			this.checkbuttonWhiteSpaces.Label = global::Mono.Unix.Catalog.GetString ("S_how whitespaces");
			this.checkbuttonWhiteSpaces.UseUnderline = true;
			DesignerLayout.Pack (this.hbox2, this.checkbuttonWhiteSpaces);
			DesignerLayout.Pack (this.vbox1, this.hbox2);
			this.scrolledwindow1 = DesignerLayout.Scrolled (null);
			this.scrolledwindow1.CanFocus = true;
			DesignerLayout.Pack (this.vbox1, this.scrolledwindow1, true);
			this.vpaned1.EndChild = this.vbox1;
			DesignerLayout.SetContent (this, this.vpaned1);
		}
	}
}
