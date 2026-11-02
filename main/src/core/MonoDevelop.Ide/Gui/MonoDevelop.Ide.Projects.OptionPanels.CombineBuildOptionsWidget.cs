using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CombineBuildOptionsWidget
	{
		private global::Gtk.Box vbox68;
		private global::Gtk.Label label73;
		private global::Gtk.Box hbox46;
		private global::Gtk.Label label74;
		private global::MonoDevelop.Components.FolderEntry folderEntry;

		protected virtual void Build ()
		{
			this.vbox68 = DesignerLayout.Margin (DesignerLayout.VBox (0), 12);
			this.label73 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Output Directory</b>"), true);
			DesignerLayout.Pack (this.vbox68, this.label73);
			this.hbox46 = DesignerLayout.HBox (6);
			this.hbox46.MarginTop = 6;
			this.label74 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("    "));
			DesignerLayout.Pack (this.hbox46, this.label74);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Pack (this.hbox46, this.folderEntry, true);
			DesignerLayout.Pack (this.vbox68, this.hbox46);
			DesignerLayout.SetContent (this, this.vbox68);
		}
	}
}
