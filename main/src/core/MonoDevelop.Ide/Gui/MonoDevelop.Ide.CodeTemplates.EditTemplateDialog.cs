using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.CodeTemplates
{
	partial class EditTemplateDialog
	{
		private global::Gtk.Paned hpaned1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Grid table2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry entryDescription;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBoxText comboboxentryMime;
		private global::Gtk.Box hbox2;
		private global::Gtk.Entry entryShortcut1;
		private global::Gtk.Label label5;
		private global::Gtk.ComboBoxText comboboxentryGroups;
		private global::Gtk.Box hbox5;
		private global::Gtk.CheckButton checkbuttonExpansion;
		private global::Gtk.CheckButton checkbuttonSurroundWith;
		private global::Gtk.Label label1;
		private global::Gtk.Label label3;
		private global::Gtk.Box vbox3;
		private global::Gtk.Box hbox3;
		private global::Gtk.Label label6;
		private global::Gtk.Fixed fixed1;
		private global::Gtk.CheckButton checkbuttonWhiteSpaces;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.Box vbox4;
		private global::Gtk.ComboBoxText comboboxVariables;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TreeView treeviewVariable;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = "";
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;
			this.hpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal);
			this.hpaned1.ResizeStartChild = false;
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.table2 = DesignerLayout.Grid (6, 6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.entryDescription = new global::Gtk.Entry ();
			this.entryDescription.Editable = true;
			DesignerLayout.Pack (this.hbox1, this.entryDescription, true);
			this.comboboxentryMime = global::Gtk.ComboBoxText.NewWithEntry ();
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Mime:"), false, this.comboboxentryMime);
			DesignerLayout.Pack (this.hbox1, this.label2);
			DesignerLayout.Pack (this.hbox1, this.comboboxentryMime);
			DesignerLayout.Attach (this.table2, this.hbox1, 1, 1, hexpand: true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.entryShortcut1 = new global::Gtk.Entry ();
			this.entryShortcut1.Text = global::Mono.Unix.Catalog.GetString (" ");
			this.entryShortcut1.Editable = true;
			DesignerLayout.Pack (this.hbox2, this.entryShortcut1, true);
			this.comboboxentryGroups = global::Gtk.ComboBoxText.NewWithEntry ();
			this.label5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Group:"), false, this.comboboxentryGroups);
			DesignerLayout.Pack (this.hbox2, this.label5);
			DesignerLayout.Pack (this.hbox2, this.comboboxentryGroups);
			DesignerLayout.Attach (this.table2, this.hbox2, 1, 0, hexpand: true);
			this.hbox5 = DesignerLayout.HBox (6);
			this.checkbuttonExpansion = new global::Gtk.CheckButton ();
			this.checkbuttonExpansion.Label = global::Mono.Unix.Catalog.GetString ("Is _expandable template");
			this.checkbuttonExpansion.UseUnderline = true;
			DesignerLayout.Pack (this.hbox5, this.checkbuttonExpansion, true);
			this.checkbuttonSurroundWith = new global::Gtk.CheckButton ();
			this.checkbuttonSurroundWith.Label = global::Mono.Unix.Catalog.GetString ("Is _surround with template");
			this.checkbuttonSurroundWith.UseUnderline = true;
			DesignerLayout.Pack (this.hbox5, this.checkbuttonSurroundWith, true);
			DesignerLayout.Attach (this.table2, this.hbox5, 1, 2, hexpand: true);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Shortcut:"), false, this.entryShortcut1);
			DesignerLayout.Attach (this.table2, this.label1, 0, 0);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Description:"), false, this.entryDescription);
			this.label3.Xalign = 1F;
			this.label3.Halign = global::Gtk.Align.End;
			DesignerLayout.Attach (this.table2, this.label3, 0, 1);
			DesignerLayout.Pack (this.vbox2, this.table2);
			this.vbox3 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.hbox3 = DesignerLayout.HBox (6);
			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Template Text:"));
			DesignerLayout.Pack (this.hbox3, this.label6);
			this.fixed1 = new global::Gtk.Fixed ();
			DesignerLayout.Pack (this.hbox3, this.fixed1, true);
			this.checkbuttonWhiteSpaces = new global::Gtk.CheckButton ();
			this.checkbuttonWhiteSpaces.Label = global::Mono.Unix.Catalog.GetString ("S_how whitespaces");
			this.checkbuttonWhiteSpaces.UseUnderline = true;
			DesignerLayout.Pack (this.hbox3, this.checkbuttonWhiteSpaces);
			DesignerLayout.Pack (this.vbox3, this.hbox3);
			this.scrolledwindow1 = DesignerLayout.Scrolled (null);
			DesignerLayout.Pack (this.vbox3, this.scrolledwindow1, true);
			DesignerLayout.Pack (this.vbox2, this.vbox3, true);
			this.hpaned1.StartChild = this.vbox2;
			this.vbox4 = DesignerLayout.VBox (6);
			this.comboboxVariables = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.vbox4, this.comboboxVariables);
			this.treeviewVariable = new global::Gtk.TreeView ();
			this.scrolledwindow2 = DesignerLayout.Scrolled (this.treeviewVariable);
			DesignerLayout.Pack (this.vbox4, this.scrolledwindow2, true);
			this.hpaned1.EndChild = this.vbox4;
			DesignerLayout.Pack (content, this.hpaned1, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (859, 494);
		}
	}
}
