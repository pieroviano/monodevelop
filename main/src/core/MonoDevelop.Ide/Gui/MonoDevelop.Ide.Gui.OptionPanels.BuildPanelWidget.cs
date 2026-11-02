// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	partial class BuildPanelWidget
	{
		private global::Gtk.Box vbox66;
		private global::Gtk.CheckButton buildBeforeRunCheckBox;
		private global::Gtk.CheckButton runWithWarningsCheckBox;
		private global::Gtk.CheckButton parallelBuildCheckbox;
		private global::Gtk.CheckButton buildBeforeTestCheckBox;
		private global::Gtk.CheckButton skipBuildingUnmodifiedProjectsCheckbox;
		private global::Gtk.ComboBoxText verbosityCombo;
		private global::Gtk.Label buildAndRunOptionsLabel;
		private global::Gtk.Box hbox44;
		private global::Gtk.Label label71;
		private global::Gtk.Box vbox67;
		private global::Gtk.CheckButton saveChangesRadioButton;
		private global::Gtk.CheckButton promptChangesRadioButton;
		private global::Gtk.CheckButton noSaveRadioButton;

		protected virtual void Build()
		{
			this.vbox66 = DesignerLayout.VBox (6);
			this.buildBeforeRunCheckBox = new global::Gtk.CheckButton();
			this.buildBeforeRunCheckBox.Label = global::Mono.Unix.Catalog.GetString("Build project before running");
			this.buildBeforeRunCheckBox.UseUnderline = true;
			DesignerLayout.Pack (this.vbox66, this.buildBeforeRunCheckBox);
			this.runWithWarningsCheckBox = new global::Gtk.CheckButton();
			this.runWithWarningsCheckBox.Label = global::Mono.Unix.Catalog.GetString("Run project if build completed with warnings");
			this.runWithWarningsCheckBox.UseUnderline = true;
			DesignerLayout.Pack (this.vbox66, this.runWithWarningsCheckBox);
			this.parallelBuildCheckbox = new global::Gtk.CheckButton();
			this.parallelBuildCheckbox.Label = global::Mono.Unix.Catalog.GetString("Enable parallel build of projects");
			this.parallelBuildCheckbox.UseUnderline = true;
			DesignerLayout.Pack (this.vbox66, this.parallelBuildCheckbox);
			this.buildBeforeTestCheckBox = new global::Gtk.CheckButton();
			this.buildBeforeTestCheckBox.Label = global::Mono.Unix.Catalog.GetString("Build project before executing unit tests");
			this.buildBeforeTestCheckBox.UseUnderline = true;
			DesignerLayout.Pack (this.vbox66, this.buildBeforeTestCheckBox);
			// Log verbosity settings
			var logContainer = DesignerLayout.HBox (6);
			this.verbosityCombo = new global::Gtk.ComboBoxText ();
			var label = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Log _verbosity:"), false, this.verbosityCombo);
			label.MarginStart = 6;
			label.MarginEnd = 6;
			DesignerLayout.Pack (logContainer, label);
			this.verbosityCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Quiet"));
			this.verbosityCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Minimal"));
			this.verbosityCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Normal"));
			this.verbosityCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Detailed"));
			this.verbosityCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Diagnostic"));
			this.verbosityCombo.Active = 2;
			this.verbosityCombo.MarginStart = 6;
			this.verbosityCombo.MarginEnd = 6;
			DesignerLayout.Pack (logContainer, this.verbosityCombo);
			DesignerLayout.Pack (this.vbox66, logContainer);
			this.skipBuildingUnmodifiedProjectsCheckbox = new global::Gtk.CheckButton();
			this.skipBuildingUnmodifiedProjectsCheckbox.Label = global::Mono.Unix.Catalog.GetString("Skip building unmodified projects");
			this.skipBuildingUnmodifiedProjectsCheckbox.UseUnderline = true;
			DesignerLayout.Pack (this.vbox66, this.skipBuildingUnmodifiedProjectsCheckbox);
			this.buildAndRunOptionsLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString("<b>File Save Options Before Building</b>"), true);
			this.buildAndRunOptionsLabel.Yalign = 0F;
			this.buildAndRunOptionsLabel.MarginTop = 6;
			this.buildAndRunOptionsLabel.MarginBottom = 6;
			DesignerLayout.Pack (this.vbox66, this.buildAndRunOptionsLabel);
			this.hbox44 = DesignerLayout.HBox (6);
			this.label71 = DesignerLayout.Label ("    ");
			this.label71.Yalign = 0F;
			DesignerLayout.Pack (this.hbox44, this.label71);
			this.vbox67 = DesignerLayout.VBox (6);
			this.saveChangesRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString("_Save changes to open documents"));
			DesignerLayout.Pack (this.vbox67, this.saveChangesRadioButton);
			this.promptChangesRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString("_Prompt to save changes to open documents"), this.saveChangesRadioButton);
			DesignerLayout.Pack (this.vbox67, this.promptChangesRadioButton);
			this.noSaveRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString("_Don\'t save changes to open documents "), this.saveChangesRadioButton);
			DesignerLayout.Pack (this.vbox67, this.noSaveRadioButton);
			DesignerLayout.Pack (this.hbox44, this.vbox67);
			DesignerLayout.Pack (this.vbox66, this.hbox44);
			DesignerLayout.SetContent (this, this.vbox66);
		}
	}
}
