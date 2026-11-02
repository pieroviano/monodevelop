// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace Mono.Instrumentation.Monitor
{
	public partial class CounterSelectorDialog
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeCounters;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Select Counter");
			this.DefaultWidth = 445;
			this.DefaultHeight = 540;
			var content = DesignerLayout.ContentArea (this, 2);
			this.treeCounters = new global::Gtk.TreeView ();
			this.treeCounters.CanFocus = true;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.treeCounters);
			DesignerLayout.Margin (this.GtkScrolledWindow, 9);
			DesignerLayout.Pack (content, this.GtkScrolledWindow, true);
			this.buttonCancel = DesignerLayout.AddActionButton (this, DesignerLayout.Button ("_Cancel"), global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.AddActionButton (this, DesignerLayout.Button ("_OK"), global::Gtk.ResponseType.Ok, true);
		}
	}
}
