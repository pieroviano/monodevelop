// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.VBNetBinding
{
	public partial class ImportsOptionsPanelWidget
	{
		private global::Gtk.Grid table3;
		private global::Gtk.Button cmdAdd;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeview1;
		private global::Gtk.Entry txtImport;
		private global::Gtk.Box vbox1;
		private global::Gtk.Button cmdRemove;

		protected virtual void Build ()
		{
			this.table3 = DesignerLayout.Grid ();
			this.txtImport = new global::Gtk.Entry ();
			this.txtImport.CanFocus = true;
			this.txtImport.Hexpand = true;
			this.table3.Attach (this.txtImport, 0, 0, 1, 1);
			this.cmdAdd = new global::Gtk.Button ();
			this.cmdAdd.CanFocus = true;
			this.cmdAdd.UseUnderline = true;
			this.cmdAdd.Label = global::Mono.Unix.Catalog.GetString ("Add");
			this.table3.Attach (this.cmdAdd, 1, 0, 1, 1);
			this.treeview1 = new global::Gtk.TreeView ();
			this.treeview1.CanFocus = true;
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.HasFrame = true;
			this.GtkScrolledWindow.Hexpand = true;
			this.GtkScrolledWindow.Vexpand = true;
			this.GtkScrolledWindow.Child = this.treeview1;
			this.table3.Attach (this.GtkScrolledWindow, 0, 1, 1, 1);
			this.vbox1 = DesignerLayout.VBox ();
			this.vbox1.Valign = global::Gtk.Align.Start;
			this.cmdRemove = new global::Gtk.Button ();
			this.cmdRemove.CanFocus = true;
			this.cmdRemove.UseUnderline = true;
			this.cmdRemove.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			this.vbox1.Append (this.cmdRemove);
			this.table3.Attach (this.vbox1, 1, 1, 1, 1);
			this.Child = this.table3;
			this.cmdRemove.Clicked += new global::System.EventHandler (this.OnCmdRemoveClicked);
			this.txtImport.Changed += new global::System.EventHandler (this.OnTxtImportChanged);
			this.cmdAdd.Clicked += new global::System.EventHandler (this.OnCmdAddClicked);
		}
	}
}
