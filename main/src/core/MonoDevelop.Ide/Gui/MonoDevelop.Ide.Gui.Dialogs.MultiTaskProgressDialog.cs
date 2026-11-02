// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal partial class MultiTaskProgressDialog
	{
		private global::Gtk.Label title;
		private global::Gtk.ScrolledWindow progressScroll;
		private global::Gtk.TreeView progressTreeView;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow detailsScroll;
		private global::Gtk.TextView detailsTextView;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonClose;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Progress");
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;
			this.title = DesignerLayout.Label (null, true);
			DesignerLayout.Pack (content, this.title);
			this.progressTreeView = new global::Gtk.TreeView ();
			this.progressScroll = DesignerLayout.Scrolled (this.progressTreeView);
			this.progressScroll.MinContentWidth = 400;
			this.progressScroll.MinContentHeight = 150;
			this.progressScroll.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (content, this.progressScroll, true);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Details:"));
			this.label1.MarginTop = 2;
			this.label1.MarginBottom = 2;
			DesignerLayout.Pack (content, this.label1);
			this.detailsTextView = new global::Gtk.TextView ();
			this.detailsTextView.Editable = false;
			this.detailsTextView.CursorVisible = false;
			this.detailsTextView.WrapMode = global::Gtk.WrapMode.WordChar;
			this.detailsScroll = DesignerLayout.Scrolled (this.detailsTextView);
			this.detailsScroll.MinContentHeight = 120;
			this.detailsScroll.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (content, this.detailsScroll, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonClose = DesignerLayout.Button ("_Close");
			DesignerLayout.AddActionButton (this, this.buttonClose, global::Gtk.ResponseType.Close);
			this.SetDefaultSize (491, 418);
			this.title.Visible = false;
			// TODO GTK4: DeleteActivated must take Gtk.CloseRequestArgs and return the "handled" state through args.RetVal.
			this.CloseRequest += new global::Gtk.CloseRequestHandler (this.DeleteActivated);
			this.buttonCancel.Clicked += new global::System.EventHandler (this.OnCancel);
			this.buttonClose.Clicked += new global::System.EventHandler (this.OnClose);
		}
	}
}
