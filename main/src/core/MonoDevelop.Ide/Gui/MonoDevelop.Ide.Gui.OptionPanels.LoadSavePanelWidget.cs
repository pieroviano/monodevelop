// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class LoadSavePanelWidget
	{
		private global::Gtk.Box vbox17;
		private global::Gtk.Box vbox26;
		private global::Gtk.Label locationLabel;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.Box vbox18;
		private global::Gtk.Label loadLabel;
		private global::Gtk.Box hbox14;
		private global::Gtk.Box hbox10;
		private global::Gtk.Box vbox65;
		private global::Gtk.CheckButton loadUserDataCheckButton;
		private global::Gtk.CheckButton openStartWindowRadioButton;
		private global::Gtk.CheckButton loadPrevProjectRadioButton;
		private global::Gtk.CheckButton emptyEnvironmentRadioButton;
		private global::Gtk.Box vbox19;
		private global::Gtk.Label saveLabel;
		private global::Gtk.Box hbox11;
		private global::Gtk.Box vbox20;
		private global::Gtk.CheckButton createBackupCopyCheckButton;

		protected virtual void Build ()
		{
			this.vbox17 = DesignerLayout.VBox (6);
			this.vbox26 = DesignerLayout.VBox (6);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			this.folderEntry.DisplayAsRelativePath = false;
			this.locationLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Default _Solution location"), false, this.folderEntry);
			this.locationLabel.Yalign = 0F;
			DesignerLayout.Pack (this.vbox26, this.locationLabel);
			DesignerLayout.Pack (this.vbox26, this.folderEntry);
			DesignerLayout.Pack (this.vbox17, this.vbox26);
			this.vbox18 = DesignerLayout.VBox (6);
			this.loadLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Load</b>"), true);
			this.loadLabel.Yalign = 0F;
			DesignerLayout.Pack (this.vbox18, this.loadLabel);
			this.hbox14 = DesignerLayout.HBox (6);
			this.hbox14.MarginStart = 24;
			this.hbox14.MarginEnd = 24;
			this.hbox10 = DesignerLayout.HBox (6);
			this.vbox65 = DesignerLayout.VBox (6);
			this.loadUserDataCheckButton = new global::Gtk.CheckButton ();
			this.loadUserDataCheckButton.Label = global::Mono.Unix.Catalog.GetString ("Load user-specific settings with the document");
			this.loadUserDataCheckButton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox65, this.loadUserDataCheckButton);
			// Startup options
			var startSectionVbox = DesignerLayout.VBox (6);
			var startSectionLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Start</b>"), true);
			startSectionLabel.Yalign = 0F;
			DesignerLayout.Pack (startSectionVbox, startSectionLabel);
			var startContentHbox = DesignerLayout.HBox (0);
			DesignerLayout.Pack (startSectionVbox, startContentHbox);
			var startContentVbox = DesignerLayout.VBox (6);
			startContentVbox.MarginStart = 24;
			startContentVbox.MarginEnd = 24;
			DesignerLayout.Pack (startContentHbox, startContentVbox);
			this.openStartWindowRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("_Always show me the Start Window"));
			this.openStartWindowRadioButton.Name = "openStartWindowCheckButton";
			DesignerLayout.Pack (startContentVbox, this.openStartWindowRadioButton);
			this.loadPrevProjectRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("_Load previous solution on startup"), this.openStartWindowRadioButton);
			DesignerLayout.Pack (startContentVbox, this.loadPrevProjectRadioButton);
			this.emptyEnvironmentRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("_Show empty environment"), this.openStartWindowRadioButton);
			this.emptyEnvironmentRadioButton.Name = "emptyEnvironmentCheckButton";
			DesignerLayout.Pack (startContentVbox, this.emptyEnvironmentRadioButton);
			DesignerLayout.Pack (this.hbox10, this.vbox65);
			DesignerLayout.Pack (this.hbox14, this.hbox10);
			DesignerLayout.Pack (this.vbox18, this.hbox14);
			DesignerLayout.Pack (this.vbox17, startSectionVbox);
			DesignerLayout.Pack (this.vbox17, this.vbox18);
			this.vbox19 = DesignerLayout.VBox (6);
			this.saveLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Save</b>"), true);
			this.saveLabel.Yalign = 0F;
			DesignerLayout.Pack (this.vbox19, this.saveLabel);
			this.hbox11 = DesignerLayout.HBox (6);
			this.vbox20 = DesignerLayout.VBox (6);
			this.vbox20.MarginStart = 24;
			this.vbox20.MarginEnd = 24;
			this.createBackupCopyCheckButton = new global::Gtk.CheckButton ();
			this.createBackupCopyCheckButton.Label = global::Mono.Unix.Catalog.GetString ("Always create backup copy");
			this.createBackupCopyCheckButton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox20, this.createBackupCopyCheckButton);
			DesignerLayout.Pack (this.hbox11, this.vbox20);
			DesignerLayout.Pack (this.vbox19, this.hbox11);
			DesignerLayout.Pack (this.vbox17, this.vbox19);
			DesignerLayout.SetContent (this, this.vbox17);
		}
	}
}
