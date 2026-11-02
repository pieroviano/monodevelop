using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class DeleteConfigDialog
	{
		private global::Gtk.Box hbox64;
		private global::MonoDevelop.Components.ImageView imageQuestion;
		private global::Gtk.Box vbox80;
		private global::Gtk.Label label107;
		private global::Gtk.CheckButton deleteChildrenCheck;
		private global::Gtk.Button button11;
		private global::Gtk.Button button12;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Delete Configuration");
			var content = DesignerLayout.ContentArea (this, 0);
			this.hbox64 = DesignerLayout.Margin (DesignerLayout.HBox (12), 12);
			this.imageQuestion = new global::MonoDevelop.Components.ImageView ();
			this.imageQuestion.Yalign = 0F;
			this.imageQuestion.IconSize = StockIconSize.Dialog;
			this.imageQuestion.IconId = "gtk-dialog-question";
			DesignerLayout.Pack (this.hbox64, this.imageQuestion);
			this.vbox80 = DesignerLayout.VBox (6);
			this.label107 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Are you sure you want to delete this configuration?"));
			DesignerLayout.Pack (this.vbox80, this.label107);
			this.deleteChildrenCheck = new global::Gtk.CheckButton ();
			this.deleteChildrenCheck.Label = global::Mono.Unix.Catalog.GetString ("Delete also configurations in solution items");
			this.deleteChildrenCheck.Active = true;
			this.deleteChildrenCheck.UseUnderline = true;
			DesignerLayout.Pack (this.vbox80, this.deleteChildrenCheck);
			DesignerLayout.Pack (this.hbox64, this.vbox80, true);
			DesignerLayout.Pack (content, this.hbox64, true);
			this.button11 = DesignerLayout.Button ("_No");
			DesignerLayout.AddActionButton (this, this.button11, global::Gtk.ResponseType.No);
			this.button12 = DesignerLayout.Button ("_Yes");
			DesignerLayout.AddActionButton (this, this.button12, global::Gtk.ResponseType.Yes, true);
			this.SetDefaultSize (381, 128);
		}
	}
}
