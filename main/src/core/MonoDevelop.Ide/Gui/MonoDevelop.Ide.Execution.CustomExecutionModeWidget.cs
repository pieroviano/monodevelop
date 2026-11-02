using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Execution
{
	internal partial class CustomExecutionModeWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Grid table1;
		private global::Gtk.Entry entryArgs;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Label label2;
		private global::Gtk.Label label4;
		private global::Gtk.Label label3;
		private global::MonoDevelop.Ide.Gui.Components.EnvVarList envVarList;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (9), 6);
			this.table1 = DesignerLayout.Grid (6, 6);
			this.entryArgs = new global::Gtk.Entry ();
			this.entryArgs.CanFocus = true;
			this.entryArgs.Editable = true;
			DesignerLayout.Attach (this.table1, this.entryArgs, 1, 0, hexpand: true);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			DesignerLayout.Attach (this.table1, this.folderEntry, 1, 1, hexpand: true);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Working Directory:"));
			DesignerLayout.Attach (this.table1, this.label2, 0, 1);
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Arguments:"));
			DesignerLayout.Attach (this.table1, this.label4, 0, 0);
			DesignerLayout.Pack (this.vbox2, this.table1);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Environment Variables:"));
			DesignerLayout.Pack (this.vbox2, this.label3);
			this.envVarList = new global::MonoDevelop.Ide.Gui.Components.EnvVarList ();
			this.envVarList.CanFocus = true;
			DesignerLayout.Pack (this.vbox2, this.envVarList, true);
			DesignerLayout.SetContent (this, this.vbox2);
		}
	}
}
