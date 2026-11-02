// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	partial class MonoRuntimePanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Grid table1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tree;
		private global::Gtk.TextView textview1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Button buttonDefault;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Label labelRunning;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.table1 = DesignerLayout.Grid (6, 6);
			this.textview1 = new global::Gtk.TextView ();
			this.textview1.Buffer.Text = global::Mono.Unix.Catalog.GetString ("If you have a parallel installation of Mono you can register it here, so you can use it for building and running projects. The <b>default runtime</b> is the .NET runtime to be used for building and running applications when none is specifically selected.");
			this.textview1.Sensitive = false;
			this.textview1.Editable = false;
			this.textview1.CursorVisible = false;
			this.textview1.AcceptsTab = false;
			this.textview1.WrapMode = global::Gtk.WrapMode.Word;
			this.textview1.PixelsBelowLines = 6;
			this.textview1.PixelsInsideWrap = 3;
			DesignerLayout.Attach (this.table1, this.textview1, 0, 0, hexpand: true);
			this.tree = new global::Gtk.TreeView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.tree);
			DesignerLayout.Attach (this.table1, this.GtkScrolledWindow, 0, 1, hexpand: true, vexpand: true);
			this.vbox2 = DesignerLayout.VBox (6);
			this.buttonDefault = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Set as Default"));
			DesignerLayout.Pack (this.vbox2, this.buttonDefault);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox2, this.buttonAdd);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox2, this.buttonRemove);
			DesignerLayout.Attach (this.table1, this.vbox2, 1, 1);
			DesignerLayout.Pack (this.vbox1, this.table1, true);
			this.labelRunning = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("label1"));
			DesignerLayout.Pack (this.vbox1, this.labelRunning);
			DesignerLayout.SetContent (this, this.vbox1);
			this.buttonDefault.Clicked += new global::System.EventHandler (this.OnButtonDefaultClicked);
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
		}
	}
}
