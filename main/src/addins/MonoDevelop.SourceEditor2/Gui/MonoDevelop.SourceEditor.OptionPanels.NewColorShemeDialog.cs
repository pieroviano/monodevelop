// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	partial class NewColorShemeDialog
	{
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText comboboxBaseStyle;
		private global::Gtk.Entry entryDescription;
		private global::Gtk.Entry entryName;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Create new color sheme");
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;

			this.table1 = DesignerLayout.Grid (6, 6);
			this.comboboxBaseStyle = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboboxBaseStyle, 1, 0, hexpand: true);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.Editable = true;
			DesignerLayout.Attach (this.table1, this.entryName, 1, 1, hexpand: true);
			this.entryDescription = new global::Gtk.Entry ();
			this.entryDescription.Editable = true;
			DesignerLayout.Attach (this.table1, this.entryDescription, 1, 2, hexpand: true);

			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Based on:"), false, this.comboboxBaseStyle);
			DesignerLayout.Attach (this.table1, this.label1, 0, 0);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Name:"), false, this.entryName);
			DesignerLayout.Attach (this.table1, this.label2, 0, 1);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Description:"), false, this.entryDescription);
			DesignerLayout.Attach (this.table1, this.label3, 0, 2);
			DesignerLayout.Pack (content, this.table1, true);

			this.buttonCancel = DesignerLayout.AddActionButton (this, DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Cancel")), global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.AddActionButton (this, DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_OK")), global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (393, 148);
		}
	}
}
