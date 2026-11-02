using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CombineEntryConfigurationsPanelWidget
	{
		private global::Gtk.Box vbox75;
		private global::Gtk.Box hbox61;
		private global::Gtk.ScrolledWindow scrolledwindow12;
		private global::Gtk.TreeView configsList;
		private global::Gtk.Box vbox76;
		private global::Gtk.Button addButton;
		private global::Gtk.Button copyButton;
		private global::Gtk.Button removeButton;
		private global::Gtk.Button renameButton;

		protected virtual void Build ()
		{
			this.vbox75 = DesignerLayout.VBox (0);
			this.hbox61 = DesignerLayout.HBox (6);
			this.configsList = new global::Gtk.TreeView ();
			this.scrolledwindow12 = DesignerLayout.Scrolled (this.configsList);
			DesignerLayout.Pack (this.hbox61, this.scrolledwindow12, true);
			this.vbox76 = DesignerLayout.VBox (6);
			this.addButton = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.vbox76, this.addButton);
			this.copyButton = DesignerLayout.Button ("_Copy");
			DesignerLayout.Pack (this.vbox76, this.copyButton);
			this.removeButton = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.vbox76, this.removeButton);
			this.renameButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Rename"));
			DesignerLayout.Pack (this.vbox76, this.renameButton);
			DesignerLayout.Pack (this.hbox61, this.vbox76);
			DesignerLayout.Pack (this.vbox75, this.hbox61, true);
			DesignerLayout.SetContent (this, this.vbox75);
		}
	}
}
