using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class IncludeNewFilesDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeviewFiles;
		private global::Gtk.Box hbox1;
		private global::Gtk.Button buttonExcludeAll;
		private global::Gtk.Button buttonIncludeAll;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			var content = DesignerLayout.ContentArea (this, 6);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Choose files to include in the project"));
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.treeviewFiles = new global::Gtk.TreeView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.treeviewFiles);
			DesignerLayout.Pack (this.vbox2, this.GtkScrolledWindow, true);
			this.hbox1 = DesignerLayout.HBox (6);
			// Buttons are packed at the end of the row: spacer first, then include/exclude.
			DesignerLayout.Pack (this.hbox1, DesignerLayout.Label (null), true);
			this.buttonIncludeAll = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Include All"));
			DesignerLayout.Pack (this.hbox1, this.buttonIncludeAll);
			this.buttonExcludeAll = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Exclude All"));
			DesignerLayout.Pack (this.hbox1, this.buttonExcludeAll);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (410, 323);
		}
	}
}
