using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CombineConfigurationPanelWidget
	{
		private global::Gtk.Box vbox74;
		private global::Gtk.Box hbox60;
		private global::Gtk.Label label104;
		private global::Gtk.ScrolledWindow scrolledwindow11;
		private global::Gtk.TreeView configsList;

		protected virtual void Build ()
		{
			this.vbox74 = DesignerLayout.VBox (6);
			this.hbox60 = DesignerLayout.HBox (6);
			this.label104 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Select a target configuration for each solution item:"));
			DesignerLayout.Pack (this.hbox60, this.label104);
			DesignerLayout.Pack (this.vbox74, this.hbox60);
			this.configsList = new global::Gtk.TreeView ();
			this.scrolledwindow11 = DesignerLayout.Scrolled (this.configsList);
			DesignerLayout.Pack (this.vbox74, this.scrolledwindow11, true);
			DesignerLayout.SetContent (this, this.vbox74);
		}
	}
}
