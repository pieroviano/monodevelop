//
// GtkNewProjectDialogBackend.UI.cs
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

using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Templates;

namespace MonoDevelop.Ide.Projects
{
	partial class GtkNewProjectDialogBackend : IdeDialog
	{
		const int DefaultDialogWidth = 901;
		const int DefaultDialogHeight = 632;

		Gdk.RGBA categoriesBackgroundColor = Styles.NewProjectDialog.CategoriesBackgroundColor.ToGdkColor ();
		Gdk.RGBA templateListBackgroundColor = Styles.NewProjectDialog.TemplateListBackgroundColor.ToGdkColor ();

		Box centreVBox;
		Box templatesHBox;
		Button cancelButton;
		Button previousButton;
		Button nextButton;
		Label topBannerLabel;

		TreeView templateCategoriesTreeView;
		const int TemplateCategoryNameColumn = 0;
		const int TemplateCategoryIconColumn = 1;
		const int TemplateCategoryColumn = 2;
		TreeStore templateCategoriesTreeStore =
			new TreeStore(typeof (string), typeof (Xwt.Drawing.Image), typeof(TemplateCategory));
		TreeView templatesTreeView;
		// DO NOT REMOVE
		// This column is not used here, but is required for
		// external UI tests which need a plain string name inside the model
		// This is a reminder to not remove or abuse this column for other purposes
		const int TemplateNameColumn = 0;
		// DO NOT REMOVE
		const int TemplateIconColumn = 1;
		const int TemplateColumn = 2;
		const int TemplateOwnCategoryNameColumn = 3;
		const int TemplateA11yLanguageNameColumn = 4;
		TreeStore templatesTreeStore =
			new TreeStore(typeof (string), typeof (Xwt.Drawing.Image), typeof(SolutionTemplate), typeof (string), typeof (string));
		Box templateVBox;
		ImageView templateImage;
		Label templateNameLabel;
		Label templateDescriptionLabel;
		GtkProjectConfigurationWidget projectConfigurationWidget;
		GtkTemplateCellRenderer templateTextRenderer;
		GtkTemplateCategoryCellRenderer categoryTextRenderer;
		LanguageCellRenderer languageCellRenderer;

		static CssProvider treeViewProvider;

		static GtkNewProjectDialogBackend ()
		{
			UpdateStyles ();
			Styles.Changed += (sender, e) => UpdateStyles ();
		}

		static void UpdateStyles ()
		{
			var categoriesBackgroundColorHex = Styles.ColorGetHex (Styles.NewProjectDialog.CategoriesBackgroundColor);
			var templateListBackgroundColorHex = Styles.ColorGetHex (Styles.NewProjectDialog.TemplateListBackgroundColor);

			string css = "#templateCategoriesTreeView, #templateCategoriesTreeView.view { background-color: " + categoriesBackgroundColorHex + "; }\n" +
				"#templatesTreeView, #templatesTreeView.view { background-color: " + templateListBackgroundColorHex + "; }\n";

			if (treeViewProvider == null) {
				treeViewProvider = new CssProvider ();
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.AddProviderForDisplay (display, treeViewProvider, (uint)StyleProviderPriority.Application);
			}
			treeViewProvider.LoadFromString (css);
		}

		static Box CreateBox (Orientation orientation, string name, int spacing = 0)
		{
			var box = new Box (orientation, spacing);
			box.SetShouldIgnore (true);
			box.Name = name;
			return box;
		}

		void Build ()
		{
			DefaultWidth = DefaultDialogWidth;
			DefaultHeight = DefaultDialogHeight;

			Name = "wizard_dialog";
			Title = GettextCatalog.GetString ("New Project");

			projectConfigurationWidget = new GtkProjectConfigurationWidget ();
			projectConfigurationWidget.Name = "projectConfigurationWidget";

			// The dialog is not resizable, so keep it at least at its default size when the pages change.
			ContentArea.SetSizeRequest (DefaultDialogWidth, DefaultDialogHeight);

			// Top banner of dialog.
			var topLabelEventBox = CreateBox (Orientation.Horizontal, "topLabelEventBox");
			topLabelEventBox.HeightRequest = 52;
			WidgetCss.SetBackground (topLabelEventBox, Styles.NewProjectDialog.BannerBackgroundColor);

			var topBannerBottomEdgeLineEventBox = CreateBox (Orientation.Horizontal, "topBannerBottomEdgeLineEventBox");
			topBannerBottomEdgeLineEventBox.HeightRequest = 1;
			WidgetCss.SetBackground (topBannerBottomEdgeLineEventBox, Styles.NewProjectDialog.BannerLineColor);

			topBannerLabel = new Label ();
			topBannerLabel.Name = "topBannerLabel";
			topBannerLabel.Accessible.Name = "topBannerLabel";
			WidgetCss.Set (topBannerLabel, "font-size: 200%; font-weight: bold; color: " + Styles.ColorGetHex (Styles.NewProjectDialog.BannerForegroundColor) + ";");
			topBannerLabel.Valign = Align.Center;
			var topLabelHBox = CreateBox (Orientation.Horizontal, "topLabelHBox");
			topLabelHBox.Hexpand = true;
			topBannerLabel.MarginStart = 20;
			topBannerLabel.MarginEnd = 20;
			topLabelHBox.Append (topBannerLabel);
			topLabelEventBox.Append (topLabelHBox);

			ContentArea.Append (topLabelEventBox);
			ContentArea.Append (topBannerBottomEdgeLineEventBox);

			// Main templates section.
			centreVBox = CreateBox (Orientation.Vertical, "centreVBox");
			centreVBox.Vexpand = true;
			ContentArea.Append (centreVBox);
			templatesHBox = CreateBox (Orientation.Horizontal, "templatesHBox");
			templatesHBox.Vexpand = true;
			centreVBox.Append (templatesHBox);

			// Template categories.
			var templateCategoriesBgBox = CreateBox (Orientation.Vertical, "templateCategoriesVBox");
			WidgetCss.SetBackground (templateCategoriesBgBox, Styles.NewProjectDialog.CategoriesBackgroundColor);
			templateCategoriesBgBox.WidthRequest = 220;
			var templateCategoriesScrolledWindow = new ScrolledWindow ();
			templateCategoriesScrolledWindow.Name = "templateCategoriesScrolledWindow";
			templateCategoriesScrolledWindow.HscrollbarPolicy = PolicyType.Never;
			templateCategoriesScrolledWindow.Vexpand = true;

			// Template categories tree view.
			templateCategoriesTreeView = new TreeView ();
			templateCategoriesTreeView.Name = "templateCategoriesTreeView";
			templateCategoriesTreeView.Accessible.Name = "templateCategoriesTreeView";
			templateCategoriesTreeView.SetTitle (GettextCatalog.GetString ("Project Categories"));
			templateCategoriesTreeView.Accessible.Description = GettextCatalog.GetString ("Select the project category to see all possible project templates");
			templateCategoriesTreeView.HeadersVisible = false;
			templateCategoriesTreeView.Model = templateCategoriesTreeStore;
			templateCategoriesTreeView.SearchColumn = -1; // disable the interactive search
			templateCategoriesTreeView.ShowExpanders = false;

			templateCategoriesTreeView.AppendColumn (CreateTemplateCategoriesTreeViewColumn ());
			templateCategoriesScrolledWindow.Child = templateCategoriesTreeView;
			templateCategoriesBgBox.Append (templateCategoriesScrolledWindow);
			templatesHBox.Append (templateCategoriesBgBox);

			// Templates.
			var templatesBgBox = CreateBox (Orientation.Vertical, "templatesVBox");
			WidgetCss.SetBackground (templatesBgBox, Styles.NewProjectDialog.TemplateListBackgroundColor);
			templatesBgBox.WidthRequest = 400;
			templatesHBox.Append (templatesBgBox);
			var templatesScrolledWindow = new ScrolledWindow ();
			templatesScrolledWindow.Name = "templatesScrolledWindow";
			templatesScrolledWindow.HscrollbarPolicy = PolicyType.Never;
			templatesScrolledWindow.Vexpand = true;

			// Templates tree view.
			templatesTreeView = new TreeView ();
			templatesTreeView.Name = "templatesTreeView";
			templatesTreeView.Accessible.Name = "templatesTreeView";
			templatesTreeView.SetTitle (GettextCatalog.GetString ("Project Templates"));
			templatesTreeView.Accessible.Description = GettextCatalog.GetString ("Select the project template");
			templatesTreeView.HeadersVisible = false;
			templatesTreeView.Model = templatesTreeStore;
			templatesTreeView.SearchColumn = -1; // disable the interactive search
			templatesTreeView.ShowExpanders = false;
			templatesTreeView.AppendColumn (CreateTemplateListTreeViewColumn ());
			templatesScrolledWindow.Child = templatesTreeView;
			templatesBgBox.Append (templatesScrolledWindow);

			// Accessibilityy
			templateCategoriesTreeView.AddLinkedUIElement (templatesTreeView);

			// Template
			var templateEventBox = CreateBox (Orientation.Vertical, "templateEventBox");
			WidgetCss.SetBackground (templateEventBox, Styles.NewProjectDialog.TemplateBackgroundColor);
			templateEventBox.Hexpand = true;
			templatesHBox.Append (templateEventBox);
			templateVBox = CreateBox (Orientation.Vertical, "templateVBox", 10);
			templateVBox.Visible = false;
			templateVBox.Vexpand = true;
			DesignerLayout.Margin (templateVBox, 20);
			templateEventBox.Append (templateVBox);

			// Template large image.
			templateImage = new ImageView ();
			templateImage.SetShouldIgnore (true);
			templateImage.Name = "templateImage";
			templateImage.HeightRequest = 140;
			templateImage.WidthRequest = 240;
			templateImage.Halign = Align.Start;
			templateImage.MarginTop = 10;
			templateImage.MarginBottom = 10;
			templateVBox.Append (templateImage);

			// Template description.
			templateNameLabel = new Label ();
			templateNameLabel.Name = "templateNameLabel";
			templateNameLabel.Accessible.Name = "templateNameLabel";
			templateNameLabel.Accessible.Description = GettextCatalog.GetString ("The name of the selected template");
			templateNameLabel.WidthRequest = 240;
			templateNameLabel.Wrap = true;
			templateNameLabel.Xalign = 0;
			templateNameLabel.Halign = Align.Start;
			templateNameLabel.UseMarkup = true;
			templateNameLabel.Markup = MarkupTemplateName ("TemplateName");
			templateVBox.Append (templateNameLabel);
			templateDescriptionLabel = new Label ();
			templateDescriptionLabel.Name = "templateDescriptionLabel";
			templateDescriptionLabel.Accessible.Name = "templateDescriptionLabel";
			templateDescriptionLabel.SetLabel (GettextCatalog.GetString ("The description of the selected template"));
			templateDescriptionLabel.WidthRequest = 240;
			templateDescriptionLabel.Wrap = true;
			templateDescriptionLabel.Xalign = 0;
			templateDescriptionLabel.Halign = Align.Start;
			templateVBox.Append (templateDescriptionLabel);

			var tempLabel = new Label ();
			tempLabel.SetShouldIgnore (true);
			tempLabel.Vexpand = true;
			templateVBox.Append (tempLabel);

			templatesTreeView.AddLinkedUIElement (templateNameLabel);
			templatesTreeView.AddLinkedUIElement (templateDescriptionLabel);

			// Template - button separator.
			var templateSectionSeparatorEventBox = CreateBox (Orientation.Horizontal, "templateSectionSeparatorEventBox");
			templateSectionSeparatorEventBox.HeightRequest = 1;
			WidgetCss.SetBackground (templateSectionSeparatorEventBox, Styles.NewProjectDialog.TemplateSectionSeparatorColor);
			ContentArea.Append (templateSectionSeparatorEventBox);

			// Buttons at bottom of dialog.
			var bottomHBox = CreateBox (Orientation.Horizontal, "bottomHBox");
			ContentArea.Append (bottomHBox);

			// Cancel button - bottom left.
			var cancelButtonBox = CreateBox (Orientation.Horizontal, "cancelButtonBox");
			DesignerLayout.Margin (cancelButtonBox, 16);
			cancelButton = new Button ();
			cancelButton.Name = "cancelButton";
			cancelButton.Accessible.Name = "cancelButton";
			cancelButton.Accessible.Description = GettextCatalog.GetString ("Cancel the dialog");
			cancelButton.Label = GettextCatalog.GetString ("_Cancel");
			cancelButton.UseUnderline = true;
			cancelButtonBox.Append (cancelButton);
			bottomHBox.Append (cancelButtonBox);

			// Previous button - bottom right.
			var previousNextButtonBox = CreateBox (Orientation.Horizontal, "previousNextButtonBox", 9);
			DesignerLayout.Margin (previousNextButtonBox, 16);
			previousNextButtonBox.Hexpand = true;
			previousNextButtonBox.Halign = Align.End;
			bottomHBox.Append (previousNextButtonBox);

			previousButton = new Button ();
			previousButton.Name = "previousButton";
			previousButton.Accessible.Name = "previousButton";
			previousButton.Accessible.Description = GettextCatalog.GetString ("Return to the previous page");
			previousButton.Label = GettextCatalog.GetString ("Previous");
			previousButton.Sensitive = false;
			previousNextButtonBox.Append (previousButton);

			// Next button - bottom right.
			nextButton = new Button ();
			nextButton.Name = "nextButton";
			nextButton.Accessible.Name = "nextButton";
			nextButton.Accessible.Description = GettextCatalog.GetString ("Move to the next page");
			nextButton.Label = GettextCatalog.GetString ("Next");
			previousNextButtonBox.Append (nextButton);

			templatesTreeView.GrabFocus ();
			Resizable = false;
		}

		TreeViewColumn CreateTemplateCategoriesTreeViewColumn ()
		{
			var column = new TreeViewColumn ();

			categoryTextRenderer = new GtkTemplateCategoryCellRenderer ();
			categoryTextRenderer.Xpad = 17;
			categoryTextRenderer.Ypad = 0;
			categoryTextRenderer.CellBackgroundRgba = categoriesBackgroundColor;

			column.PackStart (categoryTextRenderer, true);
			column.AddAttribute (categoryTextRenderer, "markup", column: 0);

			column.SetCellDataFunc (categoryTextRenderer, SetTemplateCategoryCellData);

			return column;
		}

		TreeViewColumn CreateTemplateListTreeViewColumn ()
		{
			var column = new TreeViewColumn ();

			templateTextRenderer = new GtkTemplateCellRenderer ();
			templateTextRenderer.Xpad = 14;
			templateTextRenderer.Ypad = 0;
			templateTextRenderer.Ellipsize = Pango.EllipsizeMode.End;
			templateTextRenderer.CellBackgroundRgba = templateListBackgroundColor;

			column.PackStart (templateTextRenderer, true);
			column.AddAttribute (templateTextRenderer, "markup", column: 0);

			column.SetCellDataFunc (templateTextRenderer, SetTemplateTextCellData);

			languageCellRenderer = new LanguageCellRenderer ();
			languageCellRenderer.CellBackgroundRgba = templateListBackgroundColor;

			column.PackEnd (languageCellRenderer, false);
			column.SetCellDataFunc (languageCellRenderer, SetLanguageCellData);
			return column;
		}
	}
}
