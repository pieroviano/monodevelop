using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Execution
{
	internal partial class CustomExecutionModeManagerDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView listModes;
		private global::Gtk.Box vbox3;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Button buttonEdit;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Custom Execution Modes");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.hbox1 = DesignerLayout.HBox (6);
			this.listModes = new global::Gtk.TreeView ();
			this.listModes.CanFocus = true;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.listModes);
			DesignerLayout.Pack (this.hbox1, this.GtkScrolledWindow, true);
			this.vbox3 = DesignerLayout.VBox (6);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox3, this.buttonAdd);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox3, this.buttonRemove);
			this.buttonEdit = DesignerLayout.Button ("_Edit");
			DesignerLayout.Pack (this.vbox3, this.buttonEdit);
			DesignerLayout.Pack (this.hbox1, this.vbox3);
			DesignerLayout.Pack (this.vbox2, this.hbox1, true);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonOk = DesignerLayout.Button ("_Close");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Close, true);
			this.SetDefaultSize (645, 409);
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.buttonEdit.Clicked += new global::System.EventHandler (this.OnButtonEditClicked);
		}
	}
}
