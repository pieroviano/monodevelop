// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal partial class AddinLoadErrorDialog
	{
		private global::Gtk.Box hbox1;
		private global::MonoDevelop.Components.ImageView iconError;
		private global::Gtk.Box vbox4;
		private global::Gtk.Label label4;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView errorTree;
		private global::Gtk.Label messageLabel;
		private global::Gtk.Button noButton;
		private global::Gtk.Button yesButton;
		private global::Gtk.Button closeButton;

		protected virtual void Build ()
		{
			this.Title = "";
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;
			this.hbox1 = DesignerLayout.Margin (DesignerLayout.HBox (12), 6);
			this.iconError = new global::MonoDevelop.Components.ImageView ();
			this.iconError.Xalign = 0F;
			this.iconError.Yalign = 0F;
			this.iconError.IconId = "gtk-dialog-error";
			this.iconError.IconSize = global::MonoDevelop.Components.StockIconSize.Dialog;
			DesignerLayout.Pack (this.hbox1, this.iconError);
			this.vbox4 = DesignerLayout.VBox (6);
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("The following extensions could not be started:"));
			this.label4.Yalign = 0F;
			DesignerLayout.Pack (this.vbox4, this.label4);
			this.errorTree = new global::Gtk.TreeView ();
			this.errorTree.HeadersVisible = false;
			this.scrolledwindow1 = DesignerLayout.Scrolled (this.errorTree);
			DesignerLayout.Pack (this.vbox4, this.scrolledwindow1, true);
			this.messageLabel = DesignerLayout.Label (null);
			this.messageLabel.Yalign = 0F;
			this.messageLabel.WidthChars = 60;
			this.messageLabel.MaxWidthChars = 60;
			this.messageLabel.Wrap = true;
			DesignerLayout.Pack (this.vbox4, this.messageLabel);
			DesignerLayout.Pack (this.hbox1, this.vbox4, true);
			DesignerLayout.Pack (content, this.hbox1, true);
			this.noButton = DesignerLayout.Button ("_No");
			DesignerLayout.AddActionButton (this, this.noButton, global::Gtk.ResponseType.No);
			this.yesButton = DesignerLayout.Button ("_Yes");
			DesignerLayout.AddActionButton (this, this.yesButton, global::Gtk.ResponseType.Yes);
			this.closeButton = DesignerLayout.Button ("_Close");
			DesignerLayout.AddActionButton (this, this.closeButton, global::Gtk.ResponseType.Close);
			this.closeButton.Visible = false;
			this.SetDefaultSize (575, 350);
		}
	}
}
