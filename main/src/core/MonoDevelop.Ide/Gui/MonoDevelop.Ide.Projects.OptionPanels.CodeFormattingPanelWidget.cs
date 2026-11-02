using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CodeFormattingPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Box hbox1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tree;
		private global::Gtk.Box boxButtons;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Button buttonEdit;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("This is a summary of all file types used in the project or solution:"));
			DesignerLayout.Pack (this.vbox1, this.label1);
			this.hbox1 = DesignerLayout.HBox (6);
			this.tree = new global::Gtk.TreeView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.tree);
			DesignerLayout.Pack (this.hbox1, this.GtkScrolledWindow, true);
			this.boxButtons = DesignerLayout.VBox (6);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.boxButtons, this.buttonAdd);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.boxButtons, this.buttonRemove);
			this.buttonEdit = DesignerLayout.Button ("_Edit");
			DesignerLayout.Pack (this.boxButtons, this.buttonEdit);
			DesignerLayout.Pack (this.hbox1, this.boxButtons);
			DesignerLayout.Pack (this.vbox1, this.hbox1, true);
			DesignerLayout.SetContent (this, this.vbox1);
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.buttonEdit.Clicked += new global::System.EventHandler (this.OnButtonEditClicked);
		}
	}
}
