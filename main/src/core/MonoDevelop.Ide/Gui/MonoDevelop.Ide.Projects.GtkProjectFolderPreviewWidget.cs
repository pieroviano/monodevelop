using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class GtkProjectFolderPreviewWidget
	{
		private global::Gtk.Box mainVBox;
		private global::Gtk.Box previewLabelHBox;
		private global::Gtk.Label previewLabel;
		private global::Gtk.Label previewPaddingLabel;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView folderTreeView;

		protected virtual void Build ()
		{
			this.mainVBox = DesignerLayout.VBox (6);
			this.previewLabelHBox = DesignerLayout.HBox (6);
			this.previewLabel = DesignerLayout.Label (string.Format ("<span weight='bold' foreground='#555555'>{0}</span>", global::Mono.Unix.Catalog.GetString ("PREVIEW")), true);
			DesignerLayout.Pack (this.previewLabelHBox, this.previewLabel);
			this.previewPaddingLabel = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.previewLabelHBox, this.previewPaddingLabel, true);
			DesignerLayout.Pack (this.mainVBox, this.previewLabelHBox);
			this.folderTreeView = new global::Gtk.TreeView ();
			this.folderTreeView.EnableSearch = false;
			this.folderTreeView.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.folderTreeView, false);
			this.GtkScrolledWindow.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (this.mainVBox, this.GtkScrolledWindow, true);
			DesignerLayout.SetContent (this, this.mainVBox);
		}
	}
}
