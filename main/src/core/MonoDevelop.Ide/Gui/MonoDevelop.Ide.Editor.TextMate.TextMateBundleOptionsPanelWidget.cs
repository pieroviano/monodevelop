using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Editor.TextMate
{
	public partial class TextMateBundleOptionsPanelWidget
	{
		private global::Gtk.Box vbox4;

		private global::Gtk.Box vbox5;

		private global::Gtk.TextView textview1;

		private global::Gtk.Box hbox1;

		private global::Gtk.ScrolledWindow scrolledwindow1;

		private global::Gtk.TreeView bundleTreeview;

		private global::Gtk.Box vbox1;

		private global::Gtk.Button addButton;

		private global::Gtk.Button removeButton;

		protected virtual void Build()
		{
			this.vbox4 = DesignerLayout.VBox (6);
			this.vbox5 = DesignerLayout.VBox (6);
			this.textview1 = new global::Gtk.TextView();
			this.textview1.Buffer.Text = global::Mono.Unix.Catalog.GetString(@"Language bundles can provide new editor themes, code snippets, code completion items and other information to improve the editing experience of specific languages. Visual Studio for Mac supports: <b>TextMate (.tmBundle)</b> and <b>Sublime 3 (.sublime)</b> package files.");
			this.textview1.Sensitive = false;
			this.textview1.Editable = false;
			this.textview1.CursorVisible = false;
			this.textview1.AcceptsTab = false;
			this.textview1.WrapMode = global::Gtk.WrapMode.Word;
			this.textview1.PixelsBelowLines = 6;
			this.textview1.PixelsInsideWrap = 3;
			DesignerLayout.Pack (this.vbox5, this.textview1);
			this.hbox1 = DesignerLayout.HBox (6);
			this.bundleTreeview = new global::Gtk.TreeView();
			this.bundleTreeview.CanFocus = true;
			this.bundleTreeview.HeadersVisible = false;
			this.scrolledwindow1 = DesignerLayout.Scrolled (this.bundleTreeview);
			this.scrolledwindow1.CanFocus = true;
			DesignerLayout.Pack (this.hbox1, this.scrolledwindow1, true);
			this.vbox1 = DesignerLayout.VBox (6);
			this.addButton = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox1, this.addButton);
			this.removeButton = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox1, this.removeButton);
			DesignerLayout.Pack (this.hbox1, this.vbox1);
			DesignerLayout.Pack (this.vbox5, this.hbox1, true);
			DesignerLayout.Pack (this.vbox4, this.vbox5, true);
			DesignerLayout.SetContent (this, this.vbox4);
		}
	}
}
