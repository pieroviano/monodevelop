using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
// The former EventBox containers are plain boxes (their background is applied through CSS by the partial class).
namespace MonoDevelop.Ide.Projects
{
	internal partial class GtkProjectConfigurationWidget
	{
		private global::Gtk.Box mainHBox;
		private global::Gtk.Box leftBorderEventBox;
		private global::Gtk.Box projectConfigurationTableEventBox;
		private global::Gtk.Box projectConfigurationVBox;
		private global::Gtk.Box projectConfigurationTopEventBox;
		private global::Gtk.Grid projectConfigurationTable;
		private global::Gtk.Button browseButton;
		private global::Gtk.CheckButton createGitIgnoreFileCheckBox;
		private global::Gtk.CheckButton createProjectWithinSolutionDirectoryCheckBox;
		private global::Gtk.Label locationLabel;
		private global::Gtk.DrawingArea locationSeparator;
		private global::Gtk.Entry locationTextBox;
		private global::Gtk.Label projectNameLabel;
		private global::Gtk.Entry projectNameTextBox;
		private global::Gtk.Label solutionNameLabel;
		private global::Gtk.DrawingArea solutionNameSeparator;
		private global::Gtk.Entry solutionNameTextBox;
		private global::Gtk.CheckButton useGitCheckBox;
		private global::Gtk.Box versionControlLabelHBox;
		private global::Gtk.Label versionControlSpacerLabel;
		private global::Gtk.Label versionControlLabel;
		private global::Gtk.Box projectConfigurationBottomEventBox;
		private global::Gtk.Box projectConfigurationRightBorderEventBox;
		private global::Gtk.Box eventBox;
		private global::Gtk.Box previewProjectFolderVBox;
		private global::MonoDevelop.Ide.Projects.GtkProjectFolderPreviewWidget projectFolderPreviewWidget;

		static global::Gtk.Label CreateRightAlignedLabel (string text)
		{
			var label = DesignerLayout.Label (text);
			label.Xalign = 1F;
			label.Halign = global::Gtk.Align.End;
			label.MarginStart = 5;
			label.MarginEnd = 5;
			label.Justify = global::Gtk.Justification.Right;
			return label;
		}

		static global::Gtk.DrawingArea CreateSeparator ()
		{
			var separator = new global::Gtk.DrawingArea ();
			separator.HeightRequest = 1;
			separator.Hexpand = true;
			separator.Valign = global::Gtk.Align.Start;
			separator.MarginTop = 10;
			separator.MarginBottom = 10;
			return separator;
		}

		protected virtual void Build ()
		{
			this.mainHBox = DesignerLayout.HBox (0);
			this.leftBorderEventBox = DesignerLayout.VBox (0);
			this.leftBorderEventBox.WidthRequest = 30;
			DesignerLayout.Pack (this.mainHBox, this.leftBorderEventBox);
			this.projectConfigurationTableEventBox = DesignerLayout.VBox (0);
			this.projectConfigurationTableEventBox.WidthRequest = 561;
			this.projectConfigurationVBox = DesignerLayout.VBox (0);
			this.projectConfigurationTopEventBox = DesignerLayout.VBox (0);
			DesignerLayout.Pack (this.projectConfigurationVBox, this.projectConfigurationTopEventBox);
			this.projectConfigurationTable = DesignerLayout.Grid (7, 6);

			this.projectNameLabel = CreateRightAlignedLabel (global::Mono.Unix.Catalog.GetString ("Project Name:"));
			DesignerLayout.Attach (this.projectConfigurationTable, this.projectNameLabel, 0, 0);
			this.projectNameTextBox = new global::Gtk.Entry ();
			this.projectNameTextBox.Editable = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.projectNameTextBox, 1, 0, 1, 1, true);

			this.solutionNameLabel = CreateRightAlignedLabel (global::Mono.Unix.Catalog.GetString ("Solution Name:"));
			DesignerLayout.Attach (this.projectConfigurationTable, this.solutionNameLabel, 0, 1);
			this.solutionNameTextBox = new global::Gtk.Entry ();
			this.solutionNameTextBox.Editable = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.solutionNameTextBox, 1, 1);

			this.solutionNameSeparator = CreateSeparator ();
			DesignerLayout.Attach (this.projectConfigurationTable, this.solutionNameSeparator, 0, 2, 3);

			this.locationLabel = CreateRightAlignedLabel (global::Mono.Unix.Catalog.GetString ("Location:"));
			DesignerLayout.Attach (this.projectConfigurationTable, this.locationLabel, 0, 3);
			this.locationTextBox = new global::Gtk.Entry ();
			this.locationTextBox.Editable = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.locationTextBox, 1, 3);
			this.browseButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Browse..."));
			this.browseButton.MarginStart = 5;
			this.browseButton.MarginEnd = 5;
			DesignerLayout.Attach (this.projectConfigurationTable, this.browseButton, 2, 3);

			this.createProjectWithinSolutionDirectoryCheckBox = new global::Gtk.CheckButton ();
			this.createProjectWithinSolutionDirectoryCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Create a project directory within the solution directory.");
			this.createProjectWithinSolutionDirectoryCheckBox.Active = true;
			this.createProjectWithinSolutionDirectoryCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.createProjectWithinSolutionDirectoryCheckBox, 1, 4, 2);

			this.locationSeparator = CreateSeparator ();
			DesignerLayout.Attach (this.projectConfigurationTable, this.locationSeparator, 0, 5, 3);

			this.versionControlLabelHBox = DesignerLayout.HBox (0);
			this.versionControlSpacerLabel = new global::Gtk.Label ((string)null);
			this.versionControlSpacerLabel.WidthRequest = 24;
			this.versionControlSpacerLabel.Justify = global::Gtk.Justification.Right;
			DesignerLayout.Pack (this.versionControlLabelHBox, this.versionControlSpacerLabel);
			this.versionControlLabel = CreateRightAlignedLabel (global::Mono.Unix.Catalog.GetString ("Version Control:"));
			DesignerLayout.Pack (this.versionControlLabelHBox, this.versionControlLabel);
			DesignerLayout.Attach (this.projectConfigurationTable, this.versionControlLabelHBox, 0, 6);
			this.useGitCheckBox = new global::Gtk.CheckButton ();
			this.useGitCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Use git for version control.");
			this.useGitCheckBox.Active = true;
			this.useGitCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.useGitCheckBox, 1, 6);

			this.createGitIgnoreFileCheckBox = new global::Gtk.CheckButton ();
			this.createGitIgnoreFileCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Create a .gitignore file to ignore inessential files.");
			this.createGitIgnoreFileCheckBox.Active = true;
			this.createGitIgnoreFileCheckBox.UseUnderline = true;
			DesignerLayout.Attach (this.projectConfigurationTable, this.createGitIgnoreFileCheckBox, 1, 7, 2);

			DesignerLayout.Pack (this.projectConfigurationVBox, this.projectConfigurationTable);
			this.projectConfigurationBottomEventBox = DesignerLayout.VBox (0);
			DesignerLayout.Pack (this.projectConfigurationVBox, this.projectConfigurationBottomEventBox, true);
			DesignerLayout.Pack (this.projectConfigurationTableEventBox, this.projectConfigurationVBox, true);
			DesignerLayout.Pack (this.mainHBox, this.projectConfigurationTableEventBox, true);
			this.projectConfigurationRightBorderEventBox = DesignerLayout.VBox (0);
			this.projectConfigurationRightBorderEventBox.WidthRequest = 30;
			DesignerLayout.Pack (this.mainHBox, this.projectConfigurationRightBorderEventBox);
			this.eventBox = DesignerLayout.VBox (0);
			this.previewProjectFolderVBox = DesignerLayout.Margin (DesignerLayout.VBox (6), 20);
			this.projectFolderPreviewWidget = new global::MonoDevelop.Ide.Projects.GtkProjectFolderPreviewWidget ();
			this.projectFolderPreviewWidget.WidthRequest = 240;
			DesignerLayout.Pack (this.previewProjectFolderVBox, this.projectFolderPreviewWidget, true);
			DesignerLayout.Pack (this.eventBox, this.previewProjectFolderVBox, true);
			DesignerLayout.Pack (this.mainHBox, this.eventBox);
			DesignerLayout.SetContent (this, this.mainHBox);
		}
	}
}
