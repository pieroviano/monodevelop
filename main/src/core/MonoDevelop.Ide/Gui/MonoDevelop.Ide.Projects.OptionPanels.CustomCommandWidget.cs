using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CustomCommandWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Separator hseparator2;
		private global::Gtk.Box hbox1;
		private global::Gtk.ComboBoxText comboType;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Grid tableData;
		private global::Gtk.Entry entryCommand;
		private global::Gtk.Entry entryName;
		private global::Gtk.Box hbox2;
		private global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton tagSelectorDirectory;
		private global::Gtk.Box hbox3;
		private global::Gtk.Button buttonBrowse;
		private global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton tagSelectorCommand;
		private global::Gtk.Label label1;
		private global::Gtk.Label label3;
		private global::Gtk.Label labelName;
		private global::Gtk.Entry workingdirEntry;
		private global::Gtk.Box boxData;
		private global::Gtk.CheckButton checkExternalCons;
		private global::Gtk.CheckButton checkPauseCons;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.hseparator2 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox1, this.hseparator2);
			this.hbox1 = DesignerLayout.HBox (6);
			this.comboType = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboType);
			// The remove button is packed at the end of the row.
			DesignerLayout.Pack (this.hbox1, DesignerLayout.Label (null), true);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.hbox1, this.buttonRemove);
			DesignerLayout.Pack (this.vbox1, this.hbox1);
			this.tableData = DesignerLayout.Grid (6, 6);
			this.labelName = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Name:"));
			DesignerLayout.Attach (this.tableData, this.labelName, 0, 0);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.Editable = true;
			DesignerLayout.Attach (this.tableData, this.entryName, 1, 0, 1, 1, true);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Command:"));
			DesignerLayout.Attach (this.tableData, this.label3, 0, 1);
			this.entryCommand = new global::Gtk.Entry ();
			this.entryCommand.Editable = true;
			DesignerLayout.Attach (this.tableData, this.entryCommand, 1, 1, 1, 1, true);
			this.hbox3 = DesignerLayout.HBox (6);
			this.buttonBrowse = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Browse..."));
			DesignerLayout.Pack (this.hbox3, this.buttonBrowse);
			this.tagSelectorCommand = new global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton ();
			DesignerLayout.Pack (this.hbox3, this.tagSelectorCommand);
			DesignerLayout.Attach (this.tableData, this.hbox3, 2, 1);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Working Directory:"));
			DesignerLayout.Attach (this.tableData, this.label1, 0, 2);
			this.workingdirEntry = new global::Gtk.Entry ();
			this.workingdirEntry.Editable = true;
			DesignerLayout.Attach (this.tableData, this.workingdirEntry, 1, 2, 1, 1, true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.tagSelectorDirectory = new global::MonoDevelop.Ide.Gui.Components.StringTagSelectorButton ();
			DesignerLayout.Pack (this.hbox2, this.tagSelectorDirectory);
			DesignerLayout.Attach (this.tableData, this.hbox2, 2, 2);
			DesignerLayout.Pack (this.vbox1, this.tableData);
			this.boxData = DesignerLayout.HBox (6);
			this.checkExternalCons = new global::Gtk.CheckButton ();
			this.checkExternalCons.Label = global::Mono.Unix.Catalog.GetString ("Run on e_xternal console");
			this.checkExternalCons.UseUnderline = true;
			DesignerLayout.Pack (this.boxData, this.checkExternalCons);
			this.checkPauseCons = new global::Gtk.CheckButton ();
			this.checkPauseCons.Label = global::Mono.Unix.Catalog.GetString ("Pause _console output");
			this.checkPauseCons.UseUnderline = true;
			DesignerLayout.Pack (this.boxData, this.checkPauseCons);
			DesignerLayout.Pack (this.vbox1, this.boxData);
			DesignerLayout.SetContent (this, this.vbox1);
			this.comboType.Changed += new global::System.EventHandler (this.OnComboTypeChanged);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.workingdirEntry.Changed += new global::System.EventHandler (this.OnWorkingdirEntryChanged);
			this.buttonBrowse.Clicked += new global::System.EventHandler (this.OnButtonBrowseClicked);
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.entryCommand.Changed += new global::System.EventHandler (this.OnEntryCommandChanged);
			this.checkExternalCons.Toggled += new global::System.EventHandler (this.OnCheckExternalConsClicked);
			this.checkPauseCons.Toggled += new global::System.EventHandler (this.OnCheckPauseConsClicked);
		}
	}
}
