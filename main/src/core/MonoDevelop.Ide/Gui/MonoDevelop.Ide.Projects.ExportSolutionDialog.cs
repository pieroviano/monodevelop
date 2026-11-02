using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class ExportSolutionDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Grid table;
		private global::Gtk.ComboBoxText comboFormat;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Label label2;
		private global::Gtk.Label label4;
		private global::Gtk.Label labelNewFormat;
		private global::Gtk.Label newFormatLabel;
		private global::Gtk.Button button51;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Export Solution");
			this.Resizable = false;
			var content = DesignerLayout.ContentArea (this, 6);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (12), 6);
			this.table = DesignerLayout.Grid (6, 6);
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Current format:"));
			DesignerLayout.Attach (this.table, this.label4, 0, 0);
			this.labelNewFormat = DesignerLayout.Label (null);
			DesignerLayout.Attach (this.table, this.labelNewFormat, 1, 0);
			this.newFormatLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("New format:"));
			DesignerLayout.Attach (this.table, this.newFormatLabel, 0, 1);
			this.comboFormat = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table, this.comboFormat, 1, 1);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Target folder:"));
			DesignerLayout.Attach (this.table, this.label2, 0, 2);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			this.folderEntry.DisplayAsRelativePath = false;
			DesignerLayout.Attach (this.table, this.folderEntry, 1, 2, 1, 1, true);
			DesignerLayout.Pack (this.vbox2, this.table);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.button51 = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.button51, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (496, 154);
		}
	}
}
