//
// GtkDotNetCoreProjectTemplateWizardPageWidget.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2017 Xamarin Inc. (http://xamarin.com)
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

using System;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.DotNetCore.Templating;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.DotNetCore.Gui
{
	[System.ComponentModel.ToolboxItem (true)]
	partial class GtkDotNetCoreProjectTemplateWizardPageWidget : BinWidget
	{
		DotNetCoreProjectTemplateWizardPage wizardPage;
		Xwt.Drawing.Color backgroundColor;
		MonoDevelop.Components.ImageView backgroundImageView;
		Xwt.Drawing.Image backgroundImage;

		Box mainHBox;
		Box leftBorderEventBox;
		Box configurationVBox;
		Box configurationTopEventBox;
		Box configurationTableEventBox;
		Grid configurationTable;

		ComboBoxText targetFrameworkComboBox;
		Label targetFrameworkInformationLabel;
		Label targetFrameworkLabel;

		ComboBoxText authenticationComboBox;
		Label authenticationInformationLabel;
		Label authenticationLabel;

		Box configurationBottomEventBox;
		Box backgroundLargeImageEventBox;
		Box backgroundLargeImageVBox;

		public GtkDotNetCoreProjectTemplateWizardPageWidget (DotNetCoreProjectTemplateWizardPage wizardPage)
		{
			this.wizardPage = wizardPage;

			Build ();

			// Do not use a width request for the configuration box so the left hand side of the
			// wizard page can expand to fit its contents.
			configurationVBox.WidthRequest = -1;

			backgroundImage = Xwt.Drawing.Image.FromResource ("netcore-wizard-page.png");
			backgroundImageView = new MonoDevelop.Components.ImageView (backgroundImage) {
				Xalign = 1.0f,
				Yalign = 0.5f,
				Hexpand = true,
				Vexpand = true
			};
			backgroundLargeImageVBox.Append (backgroundImageView);

			backgroundColor = Styles.NewProjectDialog.ProjectConfigurationLeftHandBackgroundColor;
			WidgetBackground.Set (leftBorderEventBox, backgroundColor);
			WidgetBackground.Set (configurationTopEventBox, backgroundColor);
			WidgetBackground.Set (configurationTableEventBox, backgroundColor);
			WidgetBackground.Set (configurationBottomEventBox, backgroundColor);
			WidgetBackground.Set (backgroundLargeImageEventBox, backgroundColor);

			if (wizardPage.TargetFrameworks.Count > 1) {
				PopulateTargetFrameworks ();
				targetFrameworkComboBox.Changed += TargetFrameworkComboBoxChanged;
			}

			if (wizardPage.SupportedAuthentications.Count > 0) {
				PopulateAuthentications ();
				authenticationComboBox.Changed += AuthenticationsComboBoxChanged;
			}
		}

		void PopulateTargetFrameworks ()
		{
			foreach (TargetFramework framework in wizardPage.TargetFrameworks) {
				targetFrameworkComboBox.AppendText (framework.GetDisplayName ());
			}

			targetFrameworkComboBox.Active = wizardPage.SelectedTargetFrameworkIndex;
		}

		void TargetFrameworkComboBoxChanged (object sender, EventArgs e)
		{
			wizardPage.SelectedTargetFrameworkIndex = targetFrameworkComboBox.Active;
		}

		void PopulateAuthentications ()
		{
			foreach (var authentication in wizardPage.SupportedAuthentications) {
				authenticationComboBox.AppendText (authentication.Description);
			}

			authenticationComboBox.Active = wizardPage.SelectedAuthenticationIndex;
			authenticationInformationLabel.LabelProp = wizardPage.SupportedAuthentications [wizardPage.SelectedAuthenticationIndex].Information;
		}

		void AuthenticationsComboBoxChanged (object sender, EventArgs e)
		{
			wizardPage.SelectedAuthenticationIndex = authenticationComboBox.Active;
			authenticationInformationLabel.LabelProp = wizardPage.SupportedAuthentications [wizardPage.SelectedAuthenticationIndex].Information;
		}

		protected virtual void Build ()
		{
			Name = "MonoDevelop.DotNetCore.Gui.GtkDotNetCoreProjectTemplateWizardPageWidget";

			mainHBox = new Box (Orientation.Horizontal, 0) {
				Name = "mainHBox"
			};

			leftBorderEventBox = new Box (Orientation.Vertical, 0) {
				WidthRequest = 30,
				Name = "leftBorderEventBox"
			};
			mainHBox.Append (leftBorderEventBox);

			configurationVBox = new Box (Orientation.Vertical, 0) {
				WidthRequest = 440,
				Name = "configurationVBox"
			};

			configurationTopEventBox = new Box (Orientation.Vertical, 0) {
				Name = "configurationTopEventBox",
				Vexpand = true
			};
			configurationVBox.Append (configurationTopEventBox);

			var showFrameworkSelection = wizardPage.TargetFrameworks.Count > 1;
			var showAuthenticationSelection = wizardPage.SupportedAuthentications.Count > 0;

			// Create the table of configurable options
			configurationTable = new Grid {
				Name = "configurationTable",
				RowSpacing = 7,
				ColumnSpacing = 6
			};

			if (showFrameworkSelection)
				AddFrameworkSelection ();

			if (showAuthenticationSelection)
				AddAuthenticationSelection (showFrameworkSelection ? 2 : 0);

			configurationTableEventBox = new Box (Orientation.Vertical, 0) {
				Name = "configurationTableEventBox"
			};
			configurationTableEventBox.Append (configurationTable);
			configurationVBox.Append (configurationTableEventBox);

			configurationBottomEventBox = new Box (Orientation.Vertical, 0) {
				Name = "configurationBottomEventBox",
				Vexpand = true
			};
			configurationVBox.Append (configurationBottomEventBox);
			mainHBox.Append (configurationVBox);

			// Add the image
			backgroundLargeImageEventBox = new Box (Orientation.Vertical, 0) {
				Name = "backgroundLargeImageEventBox",
				Hexpand = true
			};
			backgroundLargeImageVBox = new Box (Orientation.Vertical, 0) {
				Name = "backgroundLargeImageVBox",
				Vexpand = true,
				Hexpand = true
			};
			backgroundLargeImageEventBox.Append (backgroundLargeImageVBox);
			mainHBox.Append (backgroundLargeImageEventBox);

			Child = mainHBox;
		}

		void AddFrameworkSelection()
		{
			targetFrameworkComboBox = new ComboBoxText ();
			targetFrameworkComboBox.WidthRequest = 350;
			targetFrameworkComboBox.Name = "targetFrameworkComboBox";
			configurationTable.Attach (targetFrameworkComboBox, 1, 1, 1, 1);

			targetFrameworkInformationLabel = new Label {
				WidthRequest = 350,
				Name = "targetFrameworkInformationLabel",
				MarginStart = 5,
				MarginEnd = 5,
				Xalign = 0F,
				Halign = Align.Start,
				LabelProp = GettextCatalog.GetString ("Select the target framework for your project."),
				Justify = Justification.Left,
				Wrap = true
			};
			configurationTable.Attach (targetFrameworkInformationLabel, 1, 0, 1, 1);

			targetFrameworkLabel = new Label {
				Name = "targetFrameworkLabel",
				MarginStart = 5,
				MarginEnd = 5,
				Xalign = 1F,
				Halign = Align.End,
				LabelProp = GettextCatalog.GetString ("Target Framework:"),
				Justify = Justification.Right
			};
			configurationTable.Attach (targetFrameworkLabel, 0, 1, 1, 1);
		}

		void AddAuthenticationSelection(int primaryRow)
		{
			authenticationComboBox = new ComboBoxText ();
			authenticationComboBox.WidthRequest = 350;
			authenticationComboBox.Name = "authenticationComboBox";
			configurationTable.Attach (authenticationComboBox, 1, primaryRow, 1, 1);

			authenticationInformationLabel = new Label {
				WidthRequest = 350,
				Name = "authenticationInformationLabel",
				MarginStart = 5,
				MarginEnd = 5,
				Xalign = 0F,
				Halign = Align.Start,
				Justify = Justification.Left,
				Wrap = true
			};
			configurationTable.Attach (authenticationInformationLabel, 1, primaryRow + 1, 1, 1);

			authenticationLabel = new Label {
				Name = "authenticationLabel",
				MarginStart = 5,
				MarginEnd = 5,
				Xalign = 1F,
				Halign = Align.End,
				LabelProp = GettextCatalog.GetString ("Authentication:"),
				Justify = Justification.Right
			};
			configurationTable.Attach (authenticationLabel, 0, primaryRow, 1, 1);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				backgroundImage?.Dispose ();
			base.Dispose (disposing);
		}
	}
}
