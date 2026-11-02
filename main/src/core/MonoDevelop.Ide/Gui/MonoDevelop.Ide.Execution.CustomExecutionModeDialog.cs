using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Execution
{
	internal partial class CustomExecutionModeDialog
	{
		private global::Gtk.Box boxEditor;
		private global::Gtk.Box boxModeSelector;
		private global::Gtk.Box hbox5;
		private global::Gtk.Label label2;
		private global::MonoDevelop.Ide.Gui.Components.ExecutionModeComboBox comboTargetMode;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Notebook notebook;
		private global::Gtk.Box boxSave;
		private global::Gtk.Separator hseparator;
		private global::Gtk.CheckButton checkSave;
		private global::Gtk.Box boxName;
		private global::Gtk.Label label4;
		private global::Gtk.Entry entryModeName;
		private global::Gtk.Label label3;
		private global::Gtk.ComboBoxText comboStore;
		private global::Gtk.CheckButton checkPrompt;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Execution Arguments");
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 2);
			this.boxEditor = DesignerLayout.Margin (DesignerLayout.VBox (9), 6);
			this.boxModeSelector = DesignerLayout.VBox (6);
			this.hbox5 = DesignerLayout.HBox (6);
			this.label2 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Execution Mode:"));
			DesignerLayout.Pack (this.hbox5, this.label2);
			this.comboTargetMode = new global::MonoDevelop.Ide.Gui.Components.ExecutionModeComboBox ();
			DesignerLayout.Pack (this.hbox5, this.comboTargetMode, true);
			DesignerLayout.Pack (this.boxModeSelector, this.hbox5);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.boxModeSelector, this.hseparator1);
			DesignerLayout.Pack (this.boxEditor, this.boxModeSelector);
			this.notebook = new global::Gtk.Notebook ();
			DesignerLayout.Pack (this.boxEditor, this.notebook, true);
			this.boxSave = DesignerLayout.VBox (6);
			this.hseparator = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.boxSave, this.hseparator);
			this.checkSave = new global::Gtk.CheckButton ();
			this.checkSave.Label = global::Mono.Unix.Catalog.GetString ("Save this configuration as a custom execution mode");
			this.checkSave.UseUnderline = true;
			DesignerLayout.Pack (this.boxSave, this.checkSave);
			this.boxName = DesignerLayout.HBox (6);
			this.label4 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Custom Mode Name:"));
			DesignerLayout.Pack (this.boxName, this.label4);
			this.entryModeName = new global::Gtk.Entry ();
			this.entryModeName.Editable = true;
			DesignerLayout.Pack (this.boxName, this.entryModeName, true);
			this.label3 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Available for:"));
			DesignerLayout.Pack (this.boxName, this.label3);
			this.comboStore = new global::Gtk.ComboBoxText ();
			this.comboStore.AppendText (global::Mono.Unix.Catalog.GetString ("Only this project"));
			this.comboStore.AppendText (global::Mono.Unix.Catalog.GetString ("Only this solution"));
			this.comboStore.AppendText (global::Mono.Unix.Catalog.GetString ("All solutions"));
			this.comboStore.Active = 0;
			DesignerLayout.Pack (this.boxName, this.comboStore);
			DesignerLayout.Pack (this.boxSave, this.boxName);
			this.checkPrompt = new global::Gtk.CheckButton ();
			this.checkPrompt.Label = global::Mono.Unix.Catalog.GetString ("Always show the parameters window before running this custom mode");
			this.checkPrompt.UseUnderline = true;
			DesignerLayout.Pack (this.boxSave, this.checkPrompt);
			DesignerLayout.Pack (this.boxEditor, this.boxSave);
			DesignerLayout.Pack (content, this.boxEditor, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (655, 525);
			this.boxName.Visible = false;
			this.checkPrompt.Visible = false;
			this.comboTargetMode.SelectionChanged += new global::System.EventHandler (this.OnComboTargetModeSelectionChanged);
			this.checkSave.Toggled += new global::System.EventHandler (this.OnCheckSaveToggled);
			this.entryModeName.Changed += new global::System.EventHandler (this.OnEntryModeNameChanged);
		}
	}
}
