using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class AddExternalFileDialog
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.Box vbox3;
		private global::MonoDevelop.Components.ImageView iconQuestion;
		private global::Gtk.Box vbox2;
		private global::Gtk.Label labelTitle;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box vbox4;
		private global::Gtk.CheckButton radioKeep;
		private global::Gtk.Label labelKeep;
		private global::Gtk.CheckButton radioCopy;
		private global::Gtk.Label label4;
		private global::Gtk.CheckButton radioMove;
		private global::Gtk.Label label5;
		private global::Gtk.CheckButton radioLink;
		private global::Gtk.Label label6;
		private global::Gtk.CheckButton checkApplyAll;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Add File to Folder");
			var content = DesignerLayout.ContentArea (this, 2);
			this.hbox1 = DesignerLayout.Margin (DesignerLayout.HBox (6), 9);
			this.vbox3 = DesignerLayout.VBox (6);
			this.iconQuestion = new global::MonoDevelop.Components.ImageView ();
			this.iconQuestion.IconId = "gtk-dialog-question";
			this.iconQuestion.IconSize = StockIconSize.Dialog;
			DesignerLayout.Pack (this.vbox3, this.iconQuestion);
			DesignerLayout.Pack (this.hbox1, this.vbox3);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (18), 3);
			this.labelTitle = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("The file {0} is outside the target directory. What would you like to do?"), true);
			this.labelTitle.WidthRequest = 450;
			this.labelTitle.Wrap = true;
			DesignerLayout.Pack (this.vbox2, this.labelTitle);
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment1.MarginStart = 20;
			this.vbox4 = DesignerLayout.VBox (6);
			this.radioKeep = new global::Gtk.CheckButton ();
			this.radioKeep.CanFocus = true;
			this.labelKeep = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Keep</b> the file in its the current subdirectory ({0})"), true);
			this.labelKeep.WidthRequest = 376;
			this.labelKeep.Wrap = true;
			this.radioKeep.Child = this.labelKeep;
			DesignerLayout.Pack (this.vbox4, this.radioKeep);
			this.radioCopy = new global::Gtk.CheckButton ();
			this.radioCopy.CanFocus = true;
			this.radioCopy.Group = this.radioKeep;
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Copy</b> the file to the directory"), true);
			this.radioCopy.Child = this.label4;
			DesignerLayout.Pack (this.vbox4, this.radioCopy);
			this.radioMove = new global::Gtk.CheckButton ();
			this.radioMove.CanFocus = true;
			this.radioMove.Group = this.radioKeep;
			this.label5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Move</b> the file to the directory"), true);
			this.radioMove.Child = this.label5;
			DesignerLayout.Pack (this.vbox4, this.radioMove);
			this.radioLink = new global::Gtk.CheckButton ();
			this.radioLink.CanFocus = true;
			this.radioLink.Group = this.radioKeep;
			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Add a link</b> to the file"), true);
			this.radioLink.Child = this.label6;
			DesignerLayout.Pack (this.vbox4, this.radioLink);
			DesignerLayout.Pack (this.alignment1, this.vbox4);
			DesignerLayout.Pack (this.vbox2, this.alignment1);
			this.checkApplyAll = new global::Gtk.CheckButton ();
			this.checkApplyAll.CanFocus = true;
			this.checkApplyAll.Label = global::Mono.Unix.Catalog.GetString ("Use the same action for all selected files");
			this.checkApplyAll.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.checkApplyAll);
			DesignerLayout.Pack (this.hbox1, this.vbox2);
			DesignerLayout.Pack (content, this.hbox1, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (536, 286);
			this.radioKeep.Visible = false;
			this.checkApplyAll.Visible = false;
		}
	}
}
