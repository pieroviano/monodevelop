using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.StandardHeader
{
	public partial class StandardHeaderPolicyPanelWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView headerText;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView treeviewTemplates;
		private global::Gtk.CheckButton includeAutoCheck;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.hbox2 = DesignerLayout.HBox (6);
			this.headerText = new global::Gtk.TextView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.headerText);
			DesignerLayout.Pack (this.hbox2, this.GtkScrolledWindow, true);
			this.treeviewTemplates = new global::Gtk.TreeView ();
			this.GtkScrolledWindow1 = DesignerLayout.Scrolled (this.treeviewTemplates);
			DesignerLayout.Pack (this.hbox2, this.GtkScrolledWindow1, true);
			DesignerLayout.Pack (this.vbox2, this.hbox2, true);
			this.includeAutoCheck = new global::Gtk.CheckButton ();
			this.includeAutoCheck.Label = global::Mono.Unix.Catalog.GetString ("_Include standard header in new files");
			this.includeAutoCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.includeAutoCheck);
			DesignerLayout.SetContent (this, this.vbox2);
		}
	}
}
