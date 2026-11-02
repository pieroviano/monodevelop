
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class EntrySelectionTree
	{
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView tree;

		protected virtual void Build ()
		{
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.tree = new global::Gtk.TreeView ();
			this.scrolledwindow1.Child = this.tree;
			DesignerLayout.SetContent (this, this.scrolledwindow1);
		}
	}
}
