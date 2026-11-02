using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.ExternalTools
{
	public partial class ExternalToolPanelWidget
	{
		private global::Gtk.Box vbox32;

		private global::Gtk.Box hbox21;

		private global::Gtk.ScrolledWindow scrolledwindow4;

		private global::Gtk.TreeView toolListBox;

		private global::Gtk.Box buttons;

		private global::Gtk.Button addButton;

		private global::Gtk.Button removeButton;

		private global::Gtk.Label label34;

		private global::Gtk.Button moveUpButton;

		private global::Gtk.Button moveDownButton;

		private global::Gtk.Grid table2;

		private global::Gtk.Label argumentLabel;

		private global::MonoDevelop.Components.FileEntry browseButton;

		private global::Gtk.Label commandLabel;

		private global::Gtk.Label defaultKeyLabel;

		private global::Gtk.Grid table3;

		private global::Gtk.Entry argumentTextBox;

		private global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton tagSelectorArgs;

		private global::Gtk.Grid table4;

		private global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton tagSelectorPath;

		private global::Gtk.Entry workingDirTextBox;

		private global::Gtk.Grid table5;

		private global::Gtk.Entry defaultKeyTextBox;

		private global::Gtk.Box keyBindingInfoEventBox;

		private global::MonoDevelop.Components.ImageView defaultKeyInfoIcon;

		private global::Gtk.Label label2;

		private global::Gtk.Label titleLabel;

		private global::Gtk.Entry titleTextBox;

		private global::Gtk.Label workingDirLabel;

		private global::Gtk.Grid table1;

		private global::Gtk.CheckButton promptArgsCheckBox;

		private global::Gtk.CheckButton saveCurrentFileCheckBox;

		private global::Gtk.CheckButton useOutputPadCheckBox;

		protected virtual void Build ()
		{
			this.vbox32 = DesignerLayout.VBox (12);
			this.hbox21 = DesignerLayout.HBox (6);
			this.toolListBox = new global::Gtk.TreeView ();
			this.scrolledwindow4 = DesignerLayout.Scrolled (this.toolListBox);
			this.scrolledwindow4.MinContentWidth = 200;
			this.scrolledwindow4.MinContentHeight = 150;
			DesignerLayout.Pack (this.hbox21, this.scrolledwindow4, true);
			this.buttons = DesignerLayout.VBox (6);
			this.addButton = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.buttons, this.addButton);
			this.removeButton = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.buttons, this.removeButton);
			this.label34 = DesignerLayout.Label ("    ");
			this.label34.Yalign = 0F;
			DesignerLayout.Pack (this.buttons, this.label34, true);
			this.moveUpButton = DesignerLayout.Button ("Go _Up");
			DesignerLayout.Pack (this.buttons, this.moveUpButton);
			this.moveDownButton = DesignerLayout.Button ("Go _Down");
			DesignerLayout.Pack (this.buttons, this.moveDownButton);
			DesignerLayout.Pack (this.hbox21, this.buttons);
			DesignerLayout.Pack (this.vbox32, this.hbox21, true);

			this.table2 = DesignerLayout.Grid (6, 6);
			this.titleLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Title:"));
			this.titleLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table2, this.titleLabel, 0, 0);
			this.titleTextBox = new global::Gtk.Entry ();
			this.titleTextBox.Editable = true;
			DesignerLayout.Attach (this.table2, this.titleTextBox, 1, 0, hexpand: true);

			this.commandLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Command:"));
			this.commandLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table2, this.commandLabel, 0, 1);
			this.browseButton = new global::MonoDevelop.Components.FileEntry ();
			DesignerLayout.Attach (this.table2, this.browseButton, 1, 1, hexpand: true);

			this.argumentLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Arguments:"));
			this.argumentLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table2, this.argumentLabel, 0, 2);
			this.table3 = DesignerLayout.Grid (6, 4);
			this.argumentTextBox = new global::Gtk.Entry ();
			this.argumentTextBox.Editable = true;
			DesignerLayout.Attach (this.table3, this.argumentTextBox, 0, 0, hexpand: true);
			this.tagSelectorArgs = new global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton ();
			DesignerLayout.Attach (this.table3, this.tagSelectorArgs, 1, 0);
			DesignerLayout.Attach (this.table2, this.table3, 1, 2, hexpand: true);

			this.workingDirLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Working directory:"));
			this.workingDirLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table2, this.workingDirLabel, 0, 3);
			this.table4 = DesignerLayout.Grid (6, 4);
			this.tagSelectorPath = new global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton ();
			DesignerLayout.Attach (this.table4, this.tagSelectorPath, 1, 0);
			this.workingDirTextBox = new global::Gtk.Entry ();
			this.workingDirTextBox.Editable = true;
			DesignerLayout.Attach (this.table4, this.workingDirTextBox, 0, 0, hexpand: true);
			DesignerLayout.Attach (this.table2, this.table4, 1, 3, hexpand: true);

			this.defaultKeyLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Key Binding:"));
			DesignerLayout.Attach (this.table2, this.defaultKeyLabel, 0, 4);
			this.table5 = DesignerLayout.Grid (6, 6);
			this.defaultKeyTextBox = new global::Gtk.Entry ();
			this.defaultKeyTextBox.Editable = true;
			DesignerLayout.Attach (this.table5, this.defaultKeyTextBox, 0, 0, hexpand: true);
			this.keyBindingInfoEventBox = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 0);
			this.keyBindingInfoEventBox.SetSizeRequest (16, 16);
			this.defaultKeyInfoIcon = new global::MonoDevelop.Components.ImageView ();
			this.defaultKeyInfoIcon.IconId = "md-warning";
			this.defaultKeyInfoIcon.IconSize = global::MonoDevelop.Components.StockIconSize.Menu;
			this.keyBindingInfoEventBox.Append (this.defaultKeyInfoIcon);
			DesignerLayout.Attach (this.table5, this.keyBindingInfoEventBox, 1, 0);
			this.label2 = new global::Gtk.Label ((string)null);
			DesignerLayout.Attach (this.table5, this.label2, 2, 0, hexpand: true);
			DesignerLayout.Attach (this.table2, this.table5, 1, 4, hexpand: true);
			DesignerLayout.Pack (this.vbox32, this.table2);

			this.table1 = DesignerLayout.Grid (6, 6);
			this.promptArgsCheckBox = new global::Gtk.CheckButton ();
			this.promptArgsCheckBox.Label = global::Mono.Unix.Catalog.GetString ("_Prompt for arguments");
			this.promptArgsCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.promptArgsCheckBox, 0, 0, hexpand: true);
			this.saveCurrentFileCheckBox = new global::Gtk.CheckButton ();
			this.saveCurrentFileCheckBox.Label = global::Mono.Unix.Catalog.GetString ("_Save current file");
			this.saveCurrentFileCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.saveCurrentFileCheckBox, 1, 0, hexpand: true);
			this.useOutputPadCheckBox = new global::Gtk.CheckButton ();
			this.useOutputPadCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Use _output window");
			this.useOutputPadCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.useOutputPadCheckBox, 0, 1, hexpand: true);
			DesignerLayout.Pack (this.vbox32, this.table1);

			DesignerLayout.SetContent (this, this.vbox32);
			this.titleLabel.MnemonicWidget = this.titleTextBox;
		}
	}
}
