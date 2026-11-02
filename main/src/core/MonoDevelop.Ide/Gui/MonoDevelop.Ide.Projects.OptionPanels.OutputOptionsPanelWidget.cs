using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class OutputOptionsPanelWidget
	{
		private global::Gtk.Box vbox66;
		private global::Gtk.Box vbox67;
		private global::Gtk.Label label93;
		private global::Gtk.Box hbox57;
		private global::Gtk.Label label91;
		private global::Gtk.Box vbox69;
		private global::Gtk.Grid table10;
		private global::Gtk.Entry assemblyNameEntry;
		private global::Gtk.Label label98;
		private global::Gtk.Label label99;
		private global::MonoDevelop.Components.FolderEntry outputPathEntry;

		protected virtual void Build ()
		{
			this.vbox66 = DesignerLayout.VBox (12);
			this.vbox67 = DesignerLayout.VBox (6);
			this.label93 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Output</b>"), true);
			DesignerLayout.Pack (this.vbox67, this.label93);
			this.hbox57 = DesignerLayout.HBox (0);
			this.label91 = new global::Gtk.Label ((string)null);
			this.label91.WidthRequest = 18;
			DesignerLayout.Pack (this.hbox57, this.label91);
			this.vbox69 = DesignerLayout.VBox (6);
			this.table10 = DesignerLayout.Grid (6, 6);
			this.label98 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Assembly _name:"));
			this.label98.UseUnderline = true;
			DesignerLayout.Attach (this.table10, this.label98, 0, 0);
			this.assemblyNameEntry = new global::Gtk.Entry ();
			this.assemblyNameEntry.Editable = true;
			DesignerLayout.Attach (this.table10, this.assemblyNameEntry, 1, 0, 1, 1, true);
			this.label99 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Output _path:"));
			this.label99.UseUnderline = true;
			DesignerLayout.Attach (this.table10, this.label99, 0, 1);
			this.outputPathEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Attach (this.table10, this.outputPathEntry, 1, 1, 1, 1, true);
			DesignerLayout.Pack (this.vbox69, this.table10, true);
			DesignerLayout.Pack (this.hbox57, this.vbox69, true);
			DesignerLayout.Pack (this.vbox67, this.hbox57, true);
			DesignerLayout.Pack (this.vbox66, this.vbox67, true);
			DesignerLayout.SetContent (this, this.vbox66);
			this.label98.MnemonicWidget = this.assemblyNameEntry;
		}
	}
}
