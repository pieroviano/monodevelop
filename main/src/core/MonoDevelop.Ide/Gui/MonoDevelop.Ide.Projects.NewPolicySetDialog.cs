using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class NewPolicySetDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.Entry entryName;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBoxText comboSets;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("New Policy");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 12);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Policy Name:"));
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.Editable = true;
			DesignerLayout.Pack (this.vbox2, this.entryName);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Copy Settings From:"));
			DesignerLayout.Pack (this.vbox2, this.label2);
			this.comboSets = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.vbox2, this.comboSets);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (429, 200);
		}
	}
}
