// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Components
{
	public partial class PriorityList
	{
		private global::Gtk.Box hbox;
		private global::Gtk.ScrolledWindow scrolledWindow;
		private global::Gtk.TreeView treeview;
		private global::Gtk.Box controls;
		private global::Gtk.Button buttonUp;
		private global::Gtk.Button buttonDown;

		protected virtual void Build ()
		{
			this.hbox = DesignerLayout.HBox (6);
			this.treeview = new global::Gtk.TreeView ();
			this.treeview.CanFocus = true;
			this.treeview.HeadersVisible = false;
			this.scrolledWindow = DesignerLayout.Scrolled (this.treeview);
			DesignerLayout.Pack (this.hbox, this.scrolledWindow, true);
			this.controls = DesignerLayout.VBox (6);
			this.buttonUp = DesignerLayout.Button ("Go _Up");
			DesignerLayout.Pack (this.controls, this.buttonUp);
			this.buttonDown = DesignerLayout.Button ("Go _Down");
			DesignerLayout.Pack (this.controls, this.buttonDown);
			DesignerLayout.Pack (this.hbox, this.controls);
			DesignerLayout.SetContent (this, this.hbox);
			this.buttonUp.Clicked += new global::System.EventHandler (this.OnButtonUpClicked);
			this.buttonDown.Clicked += new global::System.EventHandler (this.OnButtonDownClicked);
		}
	}
}
