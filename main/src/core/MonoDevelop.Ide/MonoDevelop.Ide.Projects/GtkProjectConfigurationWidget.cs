//
// GtkProjectConfigurationWidget.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Core;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Ide.Projects
{
	[System.ComponentModel.ToolboxItem (true)]
	partial class GtkProjectConfigurationWidget : MonoDevelop.Components.BinWidget
	{
		FinalProjectConfigurationPage projectConfiguration;
		// rows of the grid created by Build (); extra controls are appended below them
		const int defaultTableRows = 8;
		int nextTableRow = defaultTableRows;
		Xwt.Drawing.Color separatorColor = Styles.NewProjectDialog.ProjectConfigurationSeparatorColor;
		Xwt.Drawing.Color leftHandBackgroundColor = Styles.NewProjectDialog.ProjectConfigurationLeftHandBackgroundColor;
		DrawingArea extraControlsSeparator;
		List<ExtraControlTableRow> extraControlRows = new List<ExtraControlTableRow> ();

		public GtkProjectConfigurationWidget ()
		{
			this.Build ();

			WidgetCss.SetBackground (solutionNameSeparator, separatorColor);
			WidgetCss.SetBackground (locationSeparator, separatorColor);

			WidgetCss.SetBackground (eventBox, Styles.NewProjectDialog.ProjectConfigurationRightHandBackgroundColor);

			WidgetCss.SetBackground (leftBorderEventBox, leftHandBackgroundColor);
			WidgetCss.SetBackground (projectConfigurationRightBorderEventBox, leftHandBackgroundColor);
			WidgetCss.SetBackground (projectConfigurationTopEventBox, leftHandBackgroundColor);
			WidgetCss.SetBackground (projectConfigurationTableEventBox, leftHandBackgroundColor);
			WidgetCss.SetBackground (projectConfigurationBottomEventBox, leftHandBackgroundColor);

			projectNameTextBox.ActivatesDefault = true;
			solutionNameTextBox.ActivatesDefault = true;
			locationTextBox.ActivatesDefault = true;

			projectNameTextBox.TruncateMultiline = true;
			solutionNameTextBox.TruncateMultiline = true;
			locationTextBox.TruncateMultiline = true;

			// Disable width request for the event box otherwise the New Project dialog
			// does not resize to fit. Set a width request for the project name text box
			// instead so the project folder preview widget does not take up more width.
			projectConfigurationTableEventBox.WidthRequest = -1;
			projectNameTextBox.WidthRequest = 335;

			RegisterEvents ();

			// Accessibility
			projectNameLabel.Accessible.Name = "ProjectConfigurationWidget.ProjectNameLabel";
			projectNameLabel.SetTitleFor (projectNameTextBox);

			projectNameTextBox.Accessible.Name = "ProjectConfigurationWidget.ProjectNameTextBox";
			projectNameTextBox.Accessible.Description = GettextCatalog.GetString ("Enter the name for the new project");
			projectNameTextBox.SetTitleUIElement (projectNameLabel);

			solutionNameLabel.Accessible.Name = "ProjectConfigurationWidget.SolutionNameLabel";
			solutionNameLabel.SetTitleFor (solutionNameTextBox);

			solutionNameTextBox.Accessible.Name = "ProjectConfigurationWidget.SolutionNameTextBox";
			solutionNameTextBox.Accessible.Description = GettextCatalog.GetString ("Enter the name for the new solution");
			solutionNameTextBox.SetTitleUIElement (solutionNameLabel);

			locationLabel.Accessible.Name = "ProjectConfigurationWidget.LocationLabel";
			locationLabel.SetTitleFor (locationTextBox, browseButton);

			locationTextBox.Accessible.Name = "ProjectConfigurationWidget.LocationTextBox";
			locationTextBox.Accessible.Description = GettextCatalog.GetString ("Enter the filepath for the new project");
			locationTextBox.SetTitleUIElement (locationLabel);

			browseButton.Accessible.Name = "ProjectConfigurationWidget.BrowseButton";
			browseButton.Accessible.Description = GettextCatalog.GetString ("Open a file selector to select the project path");
			browseButton.SetTitleUIElement (locationLabel);

			createProjectWithinSolutionDirectoryCheckBox.Accessible.Name = "ProjectConfigurationWidget.CreateProjectDirectoryCheckBox";
			createProjectWithinSolutionDirectoryCheckBox.Accessible.Description = GettextCatalog.GetString ("Select whether a folder for the new project should be created inside the solution folder");

			versionControlLabel.Accessible.Name = "ProjectConfigurationWidget.VersionControl";
			versionControlLabel.AddLinkedUIElement (useGitCheckBox);
			versionControlLabel.AddLinkedUIElement (createGitIgnoreFileCheckBox);

			useGitCheckBox.Accessible.Name = "ProjectConfigurationWidget.UseGitCheckBox";
			useGitCheckBox.Accessible.Description = GettextCatalog.GetString ("Select whether to use Git source control");

			createGitIgnoreFileCheckBox.Accessible.Name = "ProjectConfigurationWidget.CreateGitIgnoreCheckBox";
			createGitIgnoreFileCheckBox.Accessible.Description = GettextCatalog.GetString ("Select whether to create a .gitignore file to ignore common files");

			projectFolderPreviewWidget.Accessible.Name = "ProjectConfigurationWidget.ProjectFolderPreview";
			projectFolderPreviewWidget.SetLabel (GettextCatalog.GetString ("Folder Layout Preview"));
			projectFolderPreviewWidget.Accessible.Description = GettextCatalog.GetString ("A preview of the folder layout for the new project");
		}

		public void FocusFirstEntry ()
		{
			if (projectConfiguration != null) {
				if (projectConfiguration.IsProjectNameEnabled) {
					projectNameTextBox.GrabFocus ();
				} else if (projectConfiguration.IsSolutionNameEnabled) {
					solutionNameTextBox.GrabFocus ();
				}
			} else {
				projectNameTextBox.GrabFocus ();
			}
		}

		void RegisterEvents ()
		{
			locationTextBox.Changed += (sender, e) => OnLocationTextBoxChanged ();
			projectNameTextBox.TextInserted += ProjectNameTextInserted;
			projectNameTextBox.Changed += (sender, e) => OnProjectNameTextBoxChanged ();
			solutionNameTextBox.Changed += (sender, e) => OnSolutionNameTextBoxChanged ();
			createGitIgnoreFileCheckBox.Toggled += (sender, e) => OnCreateGitIgnoreFileCheckBoxClicked ();
			useGitCheckBox.Toggled += (sender, e) => OnUseGitCheckBoxClicked ();
			createProjectWithinSolutionDirectoryCheckBox.Toggled += (sender, e) => OnCreateProjectWithinSolutionDirectoryCheckBoxClicked ();
			browseButton.Clicked += (sender, e) => BrowseButtonClicked ();
		}

		void OnLocationTextBoxChanged ()
		{
			projectConfiguration.Location = locationTextBox.Text;
			projectFolderPreviewWidget.UpdateLocation ();
		}

		void ProjectNameTextInserted (object o, TextInsertedArgs args)
		{
			if (args.Text.IndexOf ('\r') >= 0) {
				var textBox = (Entry)o;
				textBox.Text = textBox.Text.Replace ("\r", string.Empty);
			}
		}

		void OnProjectNameTextBoxChanged ()
		{
			projectConfiguration.ProjectName = projectNameTextBox.Text;
			solutionNameTextBox.Text = projectConfiguration.SolutionName;
			projectFolderPreviewWidget.UpdateProjectName ();
			projectFolderPreviewWidget.UpdateSolutionName ();
		}

		void OnSolutionNameTextBoxChanged ()
		{
			projectConfiguration.SolutionName = solutionNameTextBox.Text;
			projectFolderPreviewWidget.UpdateSolutionName ();
		}

		void OnCreateGitIgnoreFileCheckBoxClicked ()
		{
			projectConfiguration.CreateGitIgnoreFile = createGitIgnoreFileCheckBox.Active;
			projectFolderPreviewWidget.ShowGitIgnoreFile ();
		}

		void OnUseGitCheckBoxClicked ()
		{
			projectConfiguration.UseGit = useGitCheckBox.Active;
			createGitIgnoreFileCheckBox.Sensitive = projectConfiguration.IsGitIgnoreEnabled;
			projectFolderPreviewWidget.ShowGitFolder ();
			projectFolderPreviewWidget.ShowGitIgnoreFile ();
		}

		void OnCreateProjectWithinSolutionDirectoryCheckBoxClicked ()
		{
			projectConfiguration.CreateProjectDirectoryInsideSolutionDirectory = createProjectWithinSolutionDirectoryCheckBox.Active;
			projectFolderPreviewWidget.Refresh ();
		}

		void BrowseButtonClicked ()
		{
			FilePath startingFolder = GetStartingFolder ();
			FilePath selectedFolder = BrowseForFolder (startingFolder);
			if (selectedFolder != null) {
				locationTextBox.Text = selectedFolder;
			}
		}

		FilePath GetStartingFolder ()
		{
			try {
				FilePath folder = locationTextBox.Text;

				if (!folder.IsNullOrEmpty && !folder.IsDirectory) {
					folder = folder.ParentDirectory;
					if (!folder.IsNullOrEmpty && !folder.IsDirectory)
						folder = FilePath.Null;
				}
				return folder;

			} catch (FileNotFoundException) {
			}

			return FilePath.Null;
		}

		FilePath BrowseForFolder (FilePath startingFolder)
		{
			var dialog = new SelectFolderDialog ();
			if (startingFolder != null)
				dialog.CurrentFolder = startingFolder;

			dialog.TransientFor = Root as Gtk.Window;

			if (dialog.Run ())
				return dialog.SelectedFile;
			return null;
		}

		public void Load (FinalProjectConfigurationPage projectConfiguration, IEnumerable<ProjectConfigurationControl> controls)
		{
			this.projectConfiguration = projectConfiguration;
			LoadWidget ();
			AddExtraControls (controls.ToList ());
		}

		void LoadWidget ()
		{
			projectFolderPreviewWidget.Load (projectConfiguration);
			solutionNameLabel.Text = GetSolutionNameLabel ();
			locationTextBox.Text = projectConfiguration.Location;
			projectNameTextBox.Text = projectConfiguration.ProjectName;
			solutionNameTextBox.Text = projectConfiguration.SolutionName;

			solutionNameTextBox.Sensitive = projectConfiguration.IsSolutionNameEnabled;
			projectNameTextBox.Sensitive = projectConfiguration.IsProjectNameEnabled;
			createProjectWithinSolutionDirectoryCheckBox.Sensitive = projectConfiguration.IsCreateProjectDirectoryInsideSolutionDirectoryEnabled;
			createProjectWithinSolutionDirectoryCheckBox.Active = projectConfiguration.CreateProjectDirectoryInsideSolutionDirectory;
			useGitCheckBox.Sensitive = projectConfiguration.IsUseGitEnabled;
			useGitCheckBox.Active = projectConfiguration.UseGit;
			createGitIgnoreFileCheckBox.Sensitive = projectConfiguration.IsGitIgnoreEnabled;
			createGitIgnoreFileCheckBox.Active = projectConfiguration.CreateGitIgnoreFile;
		}

		string GetSolutionNameLabel ()
		{
			if (projectConfiguration.IsWorkspace) {
				return GettextCatalog.GetString ("Workspace Name:");
			}
			return GettextCatalog.GetString ("Solution Name:");
		}

		void AddExtraControls (List<ProjectConfigurationControl> controls)
		{
			RemoveExistingExtraControls ();

			if (controls.Any ())
				AddExtraControlsSeparator ();

			foreach (ProjectConfigurationControl control in controls) {
				AddExtraControl (control);
			}
		}

		void RemoveExistingExtraControls ()
		{
			if (!extraControlRows.Any ())
				return;

			for (int i = extraControlRows.Count - 1; i >= 0; i--) {
				RemoveExtraControl (extraControlRows [i]);
			}

			RemoveExtraControlsSeparator ();
		}

		void RemoveExtraControl (ExtraControlTableRow extraRow)
		{
			if (extraRow.Label != null) {
				projectConfigurationTable.Remove (extraRow.Label);
							}

			if (extraRow.MainWidget.Parent == projectConfigurationTable)
				projectConfigurationTable.Remove (extraRow.MainWidget);

			if (extraRow.InformationTooltipWidget != null) {
				projectConfigurationTable.Remove (extraRow.InformationTooltipWidget);
				extraRow.InformationTooltip.Dispose ();
			}

			extraControlRows.Remove (extraRow);
			nextTableRow--;
		}

		void RemoveExtraControlsSeparator ()
		{
			if (extraControlsSeparator != null) {
				projectConfigurationTable.Remove (extraControlsSeparator);

				extraControlsSeparator = null;

				nextTableRow--;
			}
		}

		void AddExtraControlsSeparator ()
		{
			extraControlsSeparator = new DrawingArea ();
			extraControlsSeparator.HeightRequest = 1;
			extraControlsSeparator.Hexpand = true;
			extraControlsSeparator.Valign = Align.Start;
			extraControlsSeparator.MarginTop = 10;
			extraControlsSeparator.MarginBottom = 10;
			WidgetCss.SetBackground (extraControlsSeparator, separatorColor);
			projectConfigurationTable.Attach (extraControlsSeparator, 0, defaultTableRows, 3, 1);
			nextTableRow++;
		}

		void AddExtraControl (ProjectConfigurationControl control)
		{
			var extraRow = new ExtraControlTableRow {
				Row = nextTableRow,
				Label = new Label (control.Label ?? string.Empty) {
					MarginStart = 5,
					MarginEnd = 5,
					Xalign = 1,
					Halign = Align.End,
					Justify = Justification.Left
				},
				MainWidget = (Widget)control
			};

			CreateTooltip (control, extraRow);

			AddExtraControl (extraRow);
		}

		void CreateTooltip (ProjectConfigurationControl control, ExtraControlTableRow extraRow)
		{
			if (string.IsNullOrEmpty (control.InformationTooltip))
				return;

			var hbox = new Box (Orientation.Horizontal, 0);
			var paddingEventBox = new Box (Orientation.Horizontal, 0) { Hexpand = true };
			WidgetCss.SetBackground (paddingEventBox, leftHandBackgroundColor);
			hbox.Append (paddingEventBox);

			var tooltipEventBox = new Box (Orientation.Horizontal, 0) {
				HeightRequest = 16,
				WidthRequest = 16,
			};

			hbox.Append (tooltipEventBox);

			extraRow.InformationTooltipWidget = hbox;
			extraRow.InformationTooltip = CreateTooltip (tooltipEventBox, control.InformationTooltip);
		}

		EventBoxTooltip CreateTooltip (Box eventBox, string tooltipText)
		{
			Xwt.Drawing.Image image = ImageService.GetIcon ("md-help");
			WidgetCss.SetBackground (eventBox, leftHandBackgroundColor);
			eventBox.Append (new ImageView (image));

			return new EventBoxTooltip (eventBox) {
				ToolTip = tooltipText,
				Severity = TaskSeverity.Information
			};
		}

		void AddExtraControl (ExtraControlTableRow extraRow)
		{
			nextTableRow++;

			projectConfigurationTable.Attach (extraRow.Label, 0, extraRow.Row, 1, 1);

			// Allow control to extend all the way across if it has no information tooltip.
			int columnSpan = extraRow.InformationTooltipWidget == null ? 2 : 1;
			extraRow.MainWidget.Halign = Align.Fill;
			projectConfigurationTable.Attach (extraRow.MainWidget, 1, extraRow.Row, columnSpan, 1);

			if (extraRow.InformationTooltipWidget != null)
				projectConfigurationTable.Attach (extraRow.InformationTooltipWidget, 2, extraRow.Row, 1, 1);

			extraControlRows.Add (extraRow);
		}

		class ExtraControlTableRow
		{
			public int Row { get; set; }
			public Label Label { get; set; }
			public Widget MainWidget { get; set; }
			public Widget InformationTooltipWidget { get; set; }
			public EventBoxTooltip InformationTooltip { get; set; }
		}
	}
}

