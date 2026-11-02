// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	partial class HighlightingPanel
	{
		private global::Gtk.Box vbox5;
		private global::Gtk.TextView textview1;
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView styleTreeview;
		private global::Gtk.Box vbox1;
		private global::Gtk.Button addButton;
		private global::Gtk.Button removeButton;
		private global::Gtk.Button buttonOpenFolder;

		protected virtual void Build ()
		{
			this.vbox5 = DesignerLayout.VBox (6);
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.Buffer.Text = global::Mono.Unix.Catalog.GetString ("The following color theme formats are supported: Visual Studio (.vssettings), Xamarin Studio (.json), TextMate (.tmTheme). Changes in the theme folder require a restart.");
			this.textview1.Sensitive = false;
			this.textview1.Editable = false;
			this.textview1.CursorVisible = false;
			this.textview1.AcceptsTab = false;
			this.textview1.WrapMode = global::Gtk.WrapMode.Word;
			this.textview1.PixelsBelowLines = 6;
			this.textview1.PixelsInsideWrap = 3;
			DesignerLayout.Pack (this.vbox5, this.textview1);

			this.hbox1 = DesignerLayout.HBox (6);
			this.styleTreeview = new global::Gtk.TreeView ();
			this.styleTreeview.HeadersVisible = false;
			this.scrolledwindow1 = DesignerLayout.Scrolled (this.styleTreeview);
			DesignerLayout.Pack (this.hbox1, this.scrolledwindow1, true);

			this.vbox1 = DesignerLayout.VBox (6);
			this.addButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Add"));
			DesignerLayout.Pack (this.vbox1, this.addButton);
			this.removeButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Remove"));
			DesignerLayout.Pack (this.vbox1, this.removeButton);
			this.buttonOpenFolder = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Open Themes Folder"));
			DesignerLayout.Pack (this.vbox1, this.buttonOpenFolder);
			DesignerLayout.Pack (this.hbox1, this.vbox1);
			DesignerLayout.Pack (this.vbox5, this.hbox1, true);

			DesignerLayout.SetContent (this, this.vbox5);
		}
	}
}
