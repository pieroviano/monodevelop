using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Components
{
	public partial class FolderListSelector
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.Box vbox1;
		private global::MonoDevelop.Components.FolderEntry folderentry;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView dirList;
		private global::Gtk.Box vbox2;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Button buttonUpdate;
		private global::Gtk.Button buttonUp;
		private global::Gtk.Button buttonDown;

		protected virtual void Build ()
		{
			this.hbox1 = DesignerLayout.HBox (6);
			this.vbox1 = DesignerLayout.VBox (6);
			this.folderentry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Pack (this.vbox1, this.folderentry);
			this.dirList = new global::Gtk.TreeView ();
			this.dirList.CanFocus = true;
			this.dirList.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.dirList);
			DesignerLayout.Pack (this.vbox1, this.GtkScrolledWindow, true);
			DesignerLayout.Pack (this.hbox1, this.vbox1, true);
			this.vbox2 = DesignerLayout.VBox (6);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox2, this.buttonAdd);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox2, this.buttonRemove);
			this.buttonUpdate = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Update"));
			DesignerLayout.Pack (this.vbox2, this.buttonUpdate);
			this.buttonUp = DesignerLayout.Button ("Go _Up");
			DesignerLayout.Pack (this.vbox2, this.buttonUp);
			this.buttonDown = DesignerLayout.Button ("Go _Down");
			DesignerLayout.Pack (this.vbox2, this.buttonDown);
			DesignerLayout.Pack (this.hbox1, this.vbox2);
			DesignerLayout.SetContent (this, this.hbox1);
			this.folderentry.PathChanged += new global::System.EventHandler (this.OnFolderentryPathChanged);
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.buttonUpdate.Clicked += new global::System.EventHandler (this.OnButtonUpdateClicked);
			this.buttonUp.Clicked += new global::System.EventHandler (this.OnButtonUpClicked);
			this.buttonDown.Clicked += new global::System.EventHandler (this.OnButtonDownClicked);
		}
	}
}
