// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	partial class SelectFileFormatDialog
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.Box vbox3;
		private global::MonoDevelop.Components.ImageView iconWarning;
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Label labelMessage;
		private global::Gtk.Label labelWarnings;
		private global::Gtk.Label label6;
		private global::Gtk.Grid table1;
		private global::Gtk.Label labelCurrentFormat;
		private global::Gtk.ComboBoxText comboNewFormat;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Select File Format");
			var content = DesignerLayout.ContentArea (this, 2);
			this.hbox1 = DesignerLayout.Margin (DesignerLayout.HBox (6), 6);
			this.vbox3 = DesignerLayout.VBox (6);
			this.iconWarning = new global::MonoDevelop.Components.ImageView ();
			this.iconWarning.IconId = "gtk-dialog-warning";
			this.iconWarning.IconSize = global::MonoDevelop.Components.StockIconSize.Dialog;
			DesignerLayout.Pack (this.vbox3, this.iconWarning);
			DesignerLayout.Pack (this.hbox1, this.vbox3);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>File Format Incompatibility Detected</b>"), true);
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.labelMessage = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("The current file format for project {0} is not compatible with some settings of the project."));
			DesignerLayout.Pack (this.vbox2, this.labelMessage);
			this.labelWarnings = DesignerLayout.Label (null);
			this.labelWarnings.WidthChars = 60;
			this.labelWarnings.MaxWidthChars = 60;
			this.labelWarnings.Wrap = true;
			DesignerLayout.Pack (this.vbox2, this.labelWarnings);
			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Please select a new file format for the solution:"));
			DesignerLayout.Pack (this.vbox2, this.label6);
			this.table1 = DesignerLayout.Grid (6, 6);
			this.table1.MarginStart = 18;
			this.labelCurrentFormat = DesignerLayout.Label ("MonoDevelop");
			this.labelCurrentFormat.MarginTop = 3;
			this.labelCurrentFormat.MarginBottom = 3;
			DesignerLayout.Attach (this.table1, this.labelCurrentFormat, 1, 0);
			this.comboNewFormat = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboNewFormat, 1, 1, hexpand: true);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Current Format:"));
			DesignerLayout.Attach (this.table1, this.label3, 0, 0);
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("New Format:"));
			DesignerLayout.Attach (this.table1, this.label4, 0, 1);
			DesignerLayout.Pack (this.vbox2, this.table1);
			DesignerLayout.Pack (this.hbox1, this.vbox2);
			DesignerLayout.Pack (content, this.hbox1);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (638, 246);
		}
	}
}
