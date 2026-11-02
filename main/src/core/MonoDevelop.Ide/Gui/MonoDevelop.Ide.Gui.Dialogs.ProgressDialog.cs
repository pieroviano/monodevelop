// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	public partial class ProgressDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label;
		private global::Gtk.Box hbox1;
		private global::Gtk.ProgressBar progressBar;
		private global::Gtk.Button btnCancel;
		private global::Gtk.Button btnClose;
		private global::Gtk.Expander expander;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView detailsTextView;
		private global::Gtk.Label expanderLabel;
		private global::Gtk.Button button103;

		protected virtual void Build ()
		{
			this.Title = "";
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 12);
			this.label = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.vbox2, this.label);
			this.hbox1 = DesignerLayout.HBox (6);
			this.progressBar = new global::Gtk.ProgressBar ();
			this.progressBar.Valign = global::Gtk.Align.Center;
			DesignerLayout.Pack (this.hbox1, this.progressBar, true);
			this.btnCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.Pack (this.hbox1, this.btnCancel);
			this.btnClose = DesignerLayout.Button ("_Close");
			DesignerLayout.Pack (this.hbox1, this.btnClose);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			this.expander = new global::Gtk.Expander (null);
			this.detailsTextView = new global::Gtk.TextView ();
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.detailsTextView);
			this.GtkScrolledWindow.MinContentHeight = 250;
			this.expander.Child = this.GtkScrolledWindow;
			this.expanderLabel = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Details"));
			this.expanderLabel.UseUnderline = true;
			this.expander.LabelWidget = this.expanderLabel;
			DesignerLayout.Pack (this.vbox2, this.expander);
			DesignerLayout.Pack (content, this.vbox2, true);
			// The designer's (hidden) action area button: kept as a field, never added since the action area was hidden.
			this.button103 = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("GtkButton"));
			this.SetDefaultSize (544, 170);
			this.btnClose.Visible = false;
			this.btnCancel.Clicked += new global::System.EventHandler (this.OnBtnCancelClicked);
			this.btnClose.Clicked += new global::System.EventHandler (this.OnBtnCloseClicked);
			this.expander.Activated += new global::System.EventHandler (this.OnExpander1Activated);
		}
	}
}
