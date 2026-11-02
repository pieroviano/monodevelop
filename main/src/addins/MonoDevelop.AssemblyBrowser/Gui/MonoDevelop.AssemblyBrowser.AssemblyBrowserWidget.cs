// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.AssemblyBrowser
{
	public partial class AssemblyBrowserWidget
	{
		private global::Gtk.Box vbox1;

		private global::Gtk.Paned hpaned1;

		private global::Gtk.Box treeViewPlaceholder;

		private global::Gtk.Box vbox3;

		private global::Gtk.Notebook notebook1;

		private global::Gtk.Box documentationScrolledWindow;

		private global::Gtk.Box searchWidget;

		private global::Gtk.ScrolledWindow scrolledwindow1;

		private global::Gtk.TreeView searchTreeview;

		protected virtual void Build()
		{
			this.Name = "MonoDevelop.AssemblyBrowser.AssemblyBrowserWidget";
			this.vbox1 = new global::Gtk.Box(global::Gtk.Orientation.Vertical, 2);
			this.vbox1.Name = "vbox1";
			this.hpaned1 = new global::MonoDevelop.Components.HPanedThin();
			this.hpaned1.CanFocus = true;
			this.hpaned1.Name = "hpaned1";
			this.hpaned1.Position = 271;
			this.hpaned1.Hexpand = true;
			this.hpaned1.Vexpand = true;
			this.treeViewPlaceholder = new global::Gtk.Box(global::Gtk.Orientation.Vertical, 0);
			this.treeViewPlaceholder.Name = "treeViewPlaceholder";
			this.hpaned1.StartChild = this.treeViewPlaceholder;
			this.hpaned1.ResizeStartChild = false;
			this.vbox3 = new global::Gtk.Box(global::Gtk.Orientation.Vertical, 6);
			this.vbox3.Name = "vbox3";
			this.notebook1 = new global::Gtk.Notebook();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.ShowBorder = false;
			this.notebook1.Vexpand = true;
			this.documentationScrolledWindow = new global::Gtk.Box(global::Gtk.Orientation.Horizontal, 6);
			this.documentationScrolledWindow.Name = "documentationScrolledWindow";
			this.notebook1.AppendPage(this.documentationScrolledWindow, null);
			this.searchWidget = new global::Gtk.Box(global::Gtk.Orientation.Vertical, 6);
			this.searchWidget.Name = "searchWidget";
			this.searchTreeview = new global::Gtk.TreeView();
			this.searchTreeview.CanFocus = true;
			this.searchTreeview.Name = "searchTreeview";
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.HasFrame = true;
			this.scrolledwindow1.Hexpand = true;
			this.scrolledwindow1.Vexpand = true;
			this.scrolledwindow1.Child = this.searchTreeview;
			this.searchWidget.Append(this.scrolledwindow1);
			this.notebook1.AppendPage(this.searchWidget, null);
			this.vbox3.Append(this.notebook1);
			this.hpaned1.EndChild = this.vbox3;
			this.vbox1.Append(this.hpaned1);
			this.Child = this.vbox1;
		}
	}
}
