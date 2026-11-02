using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	public partial class ProjectFileSelectorDialog
	{
		private global::Gtk.Paned hpaned1;
		private global::Gtk.Box vbox3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView projectTree;
		private global::Gtk.Box vbox4;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView fileList;
		private global::Gtk.Box hbox2;
		private global::Gtk.Button AddFileButton;
		private global::MonoDevelop.Components.ImageView imageAdd;
		private global::Gtk.Box typeBox;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBoxText fileTypeCombo;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Select Project File...");
			var content = DesignerLayout.ContentArea (this, 2);
			this.hpaned1 = DesignerLayout.Margin (new global::Gtk.Paned (global::Gtk.Orientation.Horizontal), 6);
			this.vbox3 = DesignerLayout.VBox (6);
			this.projectTree = new global::Gtk.TreeView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.projectTree);
			DesignerLayout.Pack (this.vbox3, this.GtkScrolledWindow, true);
			this.hpaned1.StartChild = this.vbox3;
			this.hpaned1.ResizeStartChild = false;
			this.vbox4 = DesignerLayout.VBox (6);
			this.fileList = new global::Gtk.TreeView ();
			this.GtkScrolledWindow1 = DesignerLayout.Scrolled (this.fileList);
			DesignerLayout.Pack (this.vbox4, this.GtkScrolledWindow1, true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.AddFileButton = new global::Gtk.Button ();
			this.AddFileButton.TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Add existing files to the project");
			this.AddFileButton.HasFrame = false;
			this.imageAdd = new global::MonoDevelop.Components.ImageView ();
			this.imageAdd.IconSize = StockIconSize.Menu;
			this.imageAdd.IconId = "gtk-add";
			this.AddFileButton.Child = this.imageAdd;
			DesignerLayout.Pack (this.hbox2, this.AddFileButton);
			// The file type selector is packed at the end of the row.
			DesignerLayout.Pack (this.hbox2, DesignerLayout.Label (null), true);
			this.typeBox = DesignerLayout.HBox (6);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("File _type:"));
			this.label2.UseUnderline = true;
			DesignerLayout.Pack (this.typeBox, this.label2);
			this.fileTypeCombo = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.typeBox, this.fileTypeCombo);
			DesignerLayout.Pack (this.hbox2, this.typeBox);
			DesignerLayout.Pack (this.vbox4, this.hbox2);
			this.hpaned1.EndChild = this.vbox4;
			this.hpaned1.Vexpand = true;
			DesignerLayout.Pack (content, this.hpaned1, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (522, 416);
			this.label2.MnemonicWidget = this.fileTypeCombo;
			this.AddFileButton.Clicked += new global::System.EventHandler (this.OnAddFileButtonClicked);
		}
	}
}
