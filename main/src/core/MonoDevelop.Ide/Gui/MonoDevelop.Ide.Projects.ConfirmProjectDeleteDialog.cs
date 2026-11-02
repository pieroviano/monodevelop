using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class ConfirmProjectDeleteDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.CheckButton radioDeleteAll;
		private global::Gtk.Box alignment1;
		private global::Gtk.Label labelProjectDir;
		private global::Gtk.CheckButton radioDeleteSel;
		private global::Gtk.Box alignment2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView fileList;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Delete Project");
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Please select which files and folders you want to delete:"));
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.radioDeleteAll = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("Delete the project file and the whole project directory:"));
			this.radioDeleteAll.CanFocus = true;
			this.radioDeleteAll.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.radioDeleteAll);
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment1.MarginStart = 30;
			this.labelProjectDir = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.alignment1, this.labelProjectDir);
			DesignerLayout.Pack (this.vbox2, this.alignment1);
			this.radioDeleteSel = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("Delete the following files and folders:"));
			this.radioDeleteSel.CanFocus = true;
			this.radioDeleteSel.UseUnderline = true;
			this.radioDeleteSel.Group = this.radioDeleteAll;
			DesignerLayout.Pack (this.vbox2, this.radioDeleteSel);
			this.alignment2 = DesignerLayout.VBox (0);
			this.alignment2.MarginStart = 30;
			this.fileList = new global::Gtk.TreeView ();
			this.fileList.CanFocus = true;
			this.fileList.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.fileList);
			DesignerLayout.Pack (this.alignment2, this.GtkScrolledWindow, true);
			DesignerLayout.Pack (this.vbox2, this.alignment2, true);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_Delete");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (512, 387);
			this.radioDeleteAll.Toggled += new global::System.EventHandler (this.OnRadioDeleteAllToggled);
		}
	}
}
