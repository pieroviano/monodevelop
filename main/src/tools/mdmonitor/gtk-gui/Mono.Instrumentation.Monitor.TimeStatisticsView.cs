// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace Mono.Instrumentation.Monitor
{
	public partial class TimeStatisticsView
	{
		private global::Gtk.Box vbox3;
		private global::Gtk.Box hbox3;
		private global::Gtk.Button buttonUpdate;
		private global::Gtk.CheckButton checkShowCats;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeView;

		protected virtual void Build ()
		{
			this.vbox3 = DesignerLayout.VBox (6);
			this.hbox3 = DesignerLayout.HBox (6);
			this.buttonUpdate = DesignerLayout.IconButton ("view-refresh-symbolic", global::Mono.Unix.Catalog.GetString ("_Refresh"));
			this.buttonUpdate.CanFocus = true;
			DesignerLayout.Pack (this.hbox3, this.buttonUpdate);
			this.checkShowCats = new global::Gtk.CheckButton ();
			this.checkShowCats.CanFocus = true;
			this.checkShowCats.Label = global::Mono.Unix.Catalog.GetString ("Show Categories");
			this.checkShowCats.UseUnderline = true;
			DesignerLayout.Pack (this.hbox3, this.checkShowCats);
			DesignerLayout.Pack (this.vbox3, this.hbox3);
			this.treeView = new global::Gtk.TreeView ();
			this.treeView.CanFocus = true;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.treeView);
			DesignerLayout.Pack (this.vbox3, this.GtkScrolledWindow, true);
			this.Child = this.vbox3;
			this.buttonUpdate.Clicked += new global::System.EventHandler (this.OnButtonUpdateClicked);
			this.checkShowCats.Toggled += new global::System.EventHandler (this.OnCheckShowCatsToggled);
		}
	}
}
