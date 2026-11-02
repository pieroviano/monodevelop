using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CustomCommandPanelWidget
	{
		private global::Gtk.Box vbox;
		private global::Gtk.Label label3;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.Box vboxCommands;

		protected virtual void Build ()
		{
			this.vbox = DesignerLayout.VBox (6);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("MonoDevelop can execute user specified commands or scripts before, after or as a replacement of common project operations. It is also possible to enter custom commands which will be available in the project or solution menu."));
			this.label3.WidthRequest = 470;
			this.label3.Wrap = true;
			DesignerLayout.Pack (this.vbox, this.label3);
			this.vboxCommands = DesignerLayout.VBox (0);
			this.scrolledwindow1 = DesignerLayout.Scrolled (this.vboxCommands, false);
			this.scrolledwindow1.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (this.vbox, this.scrolledwindow1, true);
			DesignerLayout.SetContent (this, this.vbox);
		}
	}
}
