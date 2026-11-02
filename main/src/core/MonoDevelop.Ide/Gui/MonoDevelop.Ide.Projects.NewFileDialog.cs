using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
// The scrolledInfo size-allocated hook is gone with GTK 4: the info labels wrap inside the
// horizontally non-scrolling window, so no explicit width request is needed.
namespace MonoDevelop.Ide.Projects
{
	internal partial class NewFileDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Paned hpaned1;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView catView;
		private global::Gtk.Paned panedTemplates;
		private global::Gtk.Box boxTemplates;
		private global::Gtk.ScrolledWindow scrolledInfo;
		private global::Gtk.Box boxInfo;
		private global::Gtk.Label labelTemplateTitle;
		private global::Gtk.Label infoLabel;
		private global::Gtk.Label label1;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label2;
		private global::Gtk.Entry nameEntry;
		private global::Gtk.Box boxProject;
		private global::Gtk.Box hbox3;
		private global::Gtk.CheckButton projectAddCheckbox;
		private global::Gtk.ComboBoxText projectAddCombo;
		private global::Gtk.Box hbox4;
		private global::Gtk.Label projectPathLabel;
		private global::MonoDevelop.Components.FolderEntry projectFolderEntry;
		private global::Gtk.Button cancelButton;
		private global::Gtk.Button okButton;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("New File");
			var content = DesignerLayout.ContentArea (this, 8);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.hpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal);
			this.hpaned1.Vexpand = true;
			this.catView = new global::Gtk.TreeView ();
			this.catView.HeadersVisible = false;
			this.scrolledwindow1 = DesignerLayout.Scrolled (this.catView);
			this.scrolledwindow1.WidthRequest = 160;
			this.hpaned1.StartChild = this.scrolledwindow1;
			this.hpaned1.ResizeStartChild = false;
			this.panedTemplates = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal);
			this.boxTemplates = DesignerLayout.VBox (6);
			this.panedTemplates.StartChild = this.boxTemplates;
			this.panedTemplates.ResizeStartChild = false;
			this.boxInfo = DesignerLayout.Margin (DesignerLayout.VBox (6), 3);
			this.labelTemplateTitle = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Console Project</b>"), true);
			this.labelTemplateTitle.WidthRequest = 145;
			this.labelTemplateTitle.Wrap = true;
			DesignerLayout.Pack (this.boxInfo, this.labelTemplateTitle);
			this.infoLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Creates a new C# Project"));
			this.infoLabel.WidthRequest = 145;
			this.infoLabel.Yalign = 0F;
			this.infoLabel.Valign = global::Gtk.Align.Start;
			this.infoLabel.Wrap = true;
			DesignerLayout.Pack (this.boxInfo, this.infoLabel, true);
			this.scrolledInfo = DesignerLayout.Scrolled (this.boxInfo);
			this.scrolledInfo.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			this.panedTemplates.EndChild = this.scrolledInfo;
			this.panedTemplates.ResizeEndChild = false;
			this.hpaned1.EndChild = this.panedTemplates;
			DesignerLayout.Pack (this.vbox2, this.hpaned1, true);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.WidthRequest = 1;
			this.label1.HeightRequest = 1;
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.hbox2 = DesignerLayout.HBox (6);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Name:"));
			DesignerLayout.Pack (this.hbox2, this.label2);
			this.nameEntry = new global::Gtk.Entry ();
			this.nameEntry.Editable = true;
			DesignerLayout.Pack (this.hbox2, this.nameEntry, true);
			DesignerLayout.Pack (this.vbox2, this.hbox2);
			this.boxProject = DesignerLayout.VBox (6);
			this.hbox3 = DesignerLayout.HBox (6);
			this.projectAddCheckbox = new global::Gtk.CheckButton ();
			this.projectAddCheckbox.Label = global::Mono.Unix.Catalog.GetString ("_Add to project:");
			this.projectAddCheckbox.UseUnderline = true;
			DesignerLayout.Pack (this.hbox3, this.projectAddCheckbox);
			this.projectAddCombo = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox3, this.projectAddCombo, true);
			DesignerLayout.Pack (this.boxProject, this.hbox3);
			this.hbox4 = DesignerLayout.HBox (6);
			this.projectPathLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Path:"));
			DesignerLayout.Pack (this.hbox4, this.projectPathLabel);
			this.projectFolderEntry = new global::MonoDevelop.Components.FolderEntry ();
			this.projectFolderEntry.DisplayAsRelativePath = false;
			DesignerLayout.Pack (this.hbox4, this.projectFolderEntry, true);
			DesignerLayout.Pack (this.boxProject, this.hbox4);
			DesignerLayout.Pack (this.vbox2, this.boxProject);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.cancelButton = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.cancelButton, global::Gtk.ResponseType.Cancel);
			this.okButton = DesignerLayout.Button ("_New");
			DesignerLayout.AddActionButton (this, this.okButton, global::Gtk.ResponseType.None);
			this.SetDefaultSize (718, 524);
			this.boxProject.Visible = false;
		}
	}
}
