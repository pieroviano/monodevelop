// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal partial class NewLayoutDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox45;
		private global::Gtk.Label label72;
		private global::Gtk.Entry layoutName;
		private global::Gtk.Label validationMessage;
		private global::Gtk.Button button309;
		private global::Gtk.Button newButton;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("New Layout");
			this.Resizable = false;
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.hbox45 = DesignerLayout.HBox (6);
			this.label72 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Layout name:"));
			DesignerLayout.Pack (this.hbox45, this.label72);
			this.layoutName = new global::Gtk.Entry ();
			this.layoutName.SetSizeRequest (320, -1);
			this.layoutName.Editable = true;
			this.layoutName.ActivatesDefault = true;
			DesignerLayout.Pack (this.hbox45, this.layoutName, true);
			DesignerLayout.Pack (this.vbox2, this.hbox45);
			this.validationMessage = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.vbox2, this.validationMessage);
			DesignerLayout.Pack (content, this.vbox2);
			this.button309 = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.button309, global::Gtk.ResponseType.Cancel);
			this.newButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Create _Layout"));
			DesignerLayout.AddActionButton (this, this.newButton, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (459, 162);
		}
	}
}
