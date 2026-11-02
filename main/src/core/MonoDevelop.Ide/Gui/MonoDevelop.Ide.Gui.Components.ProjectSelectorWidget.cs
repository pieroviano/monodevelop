// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Components
{
	public partial class ProjectSelectorWidget
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tree;

		protected virtual void Build ()
		{
			this.tree = new global::Gtk.TreeView ();
			this.tree.CanFocus = true;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.tree);
			DesignerLayout.SetContent (this, this.GtkScrolledWindow);
		}
	}
}
