// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.PackageManagement
{
	internal partial class PackageSourcesWidget
	{
		private global::Gtk.Box mainVBox;
		private global::Gtk.Box packageSourceListHBox;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView packageSourcesTreeView;
		private global::Gtk.Box packageSourceHBox;
		private global::Gtk.Box bottomButtonBox;
		private global::Gtk.Button removeButton;
		private global::Gtk.Button addButton;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.PackageManagement.PackageSourcesWidget";
			this.mainVBox = DesignerLayout.VBox (6);
			this.mainVBox.Name = "mainVBox";

			this.packageSourceListHBox = DesignerLayout.HBox (6);
			this.packageSourceListHBox.Name = "packageSourceListHBox";
			this.packageSourcesTreeView = new global::Gtk.TreeView ();
			this.packageSourcesTreeView.CanFocus = true;
			this.packageSourcesTreeView.Name = "packageSourcesTreeView";
			this.packageSourcesTreeView.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.packageSourcesTreeView);
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			DesignerLayout.Pack (this.packageSourceListHBox, this.GtkScrolledWindow, true);
			DesignerLayout.Pack (this.mainVBox, this.packageSourceListHBox, true);

			this.packageSourceHBox = DesignerLayout.HBox (6);
			this.packageSourceHBox.Name = "packageSourceHBox";
			// Right aligned button box (former HButtonBox with LayoutStyle End)
			this.bottomButtonBox = DesignerLayout.HBox (5);
			this.bottomButtonBox.Name = "bottomButtonBox";
			this.bottomButtonBox.Hexpand = true;
			this.bottomButtonBox.Halign = global::Gtk.Align.End;
			DesignerLayout.Margin (this.bottomButtonBox, 5);

			this.removeButton = new global::Gtk.Button ();
			this.removeButton.CanFocus = true;
			this.removeButton.Name = "removeButton";
			this.removeButton.UseUnderline = true;
			this.removeButton.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			DesignerLayout.Pack (this.bottomButtonBox, this.removeButton);

			this.addButton = new global::Gtk.Button ();
			this.addButton.CanFocus = true;
			this.addButton.Name = "addButton";
			this.addButton.UseUnderline = true;
			this.addButton.Label = global::Mono.Unix.Catalog.GetString ("Add");
			DesignerLayout.Pack (this.bottomButtonBox, this.addButton);

			DesignerLayout.Pack (this.packageSourceHBox, this.bottomButtonBox, true);
			DesignerLayout.Pack (this.mainVBox, this.packageSourceHBox);

			DesignerLayout.SetContent (this, this.mainVBox);
		}
	}
}
