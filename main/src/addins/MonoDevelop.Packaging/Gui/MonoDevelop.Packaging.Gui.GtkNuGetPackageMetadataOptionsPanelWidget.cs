// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Packaging.Gui
{
	public partial class GtkNuGetPackageMetadataOptionsPanelWidget
	{
		private global::Gtk.Notebook notebook;
		private global::Gtk.Grid generalTable;
		private global::Gtk.Label packageAuthorsLabel;
		private global::Gtk.Entry packageAuthorsTextBox;
		private global::Gtk.Box packageDescriptionLabelVBox;
		private global::Gtk.Label packageDescriptionLabel;
		private global::Gtk.Label packageDescriptionPaddingLabel;
		private global::Gtk.ScrolledWindow packageDescriptionScrolledWindow;
		private global::Gtk.TextView packageDescriptionTextView;
		private global::Gtk.Label packageIdLabel;
		private global::Gtk.Entry packageIdTextBox;
		private global::Gtk.Label packageVersionLabel;
		private global::Gtk.Entry packageVersionTextBox;
		private global::Gtk.Label generalTabPageLabel;
		private global::Gtk.Grid detailsTable;
		private global::Gtk.Label packageCopyrightLabel;
		private global::Gtk.Entry packageCopyrightTextBox;
		private global::Gtk.CheckButton packageDevelopmentDependencyCheckBox;
		private global::Gtk.Label packageDevelopmentDependencyLabel;
		private global::Gtk.Label packageIconUrlLabel;
		private global::Gtk.Entry packageIconUrlTextBox;
		private global::Gtk.Box packageLanguageHBox;
		private global::Gtk.ComboBoxText packageLanguageComboBox;
		private global::Gtk.Label packageLanguageLabel;
		private global::Gtk.Label packageLicenseUrlLabel;
		private global::Gtk.Entry packageLicenseUrlTextBox;
		private global::Gtk.Label packageOwnersLabel;
		private global::Gtk.Entry packageOwnersTextBox;
		private global::Gtk.Label packageProjectUrlLabel;
		private global::Gtk.Entry packageProjectUrlTextBox;
		private global::Gtk.Box packageReleaseNotesLabelVBox;
		private global::Gtk.Label packageReleaseNotesLabel;
		private global::Gtk.Label packageReleaseNotesPaddingLabel;
		private global::Gtk.ScrolledWindow packageReleaseNotesScrolledWindow;
		private global::Gtk.TextView packageReleaseNotesTextView;
		private global::Gtk.CheckButton packageRequireLicenseAcceptanceCheckBox;
		private global::Gtk.Label packageRequireLicenseAcceptanceLabel;
		private global::Gtk.Label packageSummaryLabel;
		private global::Gtk.Entry packageSummaryTextBox;
		private global::Gtk.Label packageTagsLabel;
		private global::Gtk.Entry packageTagsTextBox;
		private global::Gtk.Label packageTitleLabel;
		private global::Gtk.Entry packageTitleTextBox;
		private global::Gtk.Label detailsTabPageLabel;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.Packaging.Gui.GtkNuGetPackageMetadataOptionsPanelWidget";
			this.notebook = new global::Gtk.Notebook ();
			this.notebook.CanFocus = true;
			this.notebook.Name = "notebook";

			// General tab
			this.generalTable = DesignerLayout.Grid (6, 6);
			this.generalTable.Name = "generalTable";
			DesignerLayout.Margin (this.generalTable, 10);

			this.packageIdLabel = CreateLabel ("packageIdLabel", global::Mono.Unix.Catalog.GetString ("ID:"));
			DesignerLayout.Attach (this.generalTable, this.packageIdLabel, 0, 0);
			this.packageIdTextBox = CreateEntry ("packageIdTextBox");
			DesignerLayout.Attach (this.generalTable, this.packageIdTextBox, 1, 0);

			this.packageVersionLabel = CreateLabel ("packageVersionLabel", global::Mono.Unix.Catalog.GetString ("Version:"));
			DesignerLayout.Attach (this.generalTable, this.packageVersionLabel, 0, 1);
			this.packageVersionTextBox = CreateEntry ("packageVersionTextBox");
			DesignerLayout.Attach (this.generalTable, this.packageVersionTextBox, 1, 1);

			this.packageAuthorsLabel = CreateLabel ("packageAuthorsLabel", global::Mono.Unix.Catalog.GetString ("Authors:"));
			DesignerLayout.Attach (this.generalTable, this.packageAuthorsLabel, 0, 2);
			this.packageAuthorsTextBox = CreateEntry ("packageAuthorsTextBox");
			DesignerLayout.Attach (this.generalTable, this.packageAuthorsTextBox, 1, 2);

			this.packageDescriptionLabelVBox = DesignerLayout.VBox (6);
			this.packageDescriptionLabelVBox.Name = "packageDescriptionLabelVBox";
			this.packageDescriptionLabelVBox.Valign = global::Gtk.Align.Start;
			this.packageDescriptionLabel = CreateLabel ("packageDescriptionLabel", global::Mono.Unix.Catalog.GetString ("Description:"));
			this.packageDescriptionLabel.WidthRequest = 164;
			DesignerLayout.Pack (this.packageDescriptionLabelVBox, this.packageDescriptionLabel);
			this.packageDescriptionPaddingLabel = CreateLabel ("packageDescriptionPaddingLabel", null);
			DesignerLayout.Pack (this.packageDescriptionLabelVBox, this.packageDescriptionPaddingLabel);
			DesignerLayout.Attach (this.generalTable, this.packageDescriptionLabelVBox, 0, 3);
			this.packageDescriptionTextView = CreateTextView ("packageDescriptionTextView");
			this.packageDescriptionScrolledWindow = DesignerLayout.Scrolled (this.packageDescriptionTextView);
			this.packageDescriptionScrolledWindow.Name = "packageDescriptionScrolledWindow";
			DesignerLayout.Attach (this.generalTable, this.packageDescriptionScrolledWindow, 1, 3, vexpand: true);

			this.notebook.AppendPage (this.generalTable, null);
			this.generalTabPageLabel = new global::Gtk.Label ((string)null);
			this.generalTabPageLabel.Name = "generalTabPageLabel";
			this.generalTabPageLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("General");
			this.notebook.SetTabLabel (this.generalTable, this.generalTabPageLabel);

			// Details tab
			this.detailsTable = DesignerLayout.Grid (6, 6);
			this.detailsTable.Name = "detailsTable";
			DesignerLayout.Margin (this.detailsTable, 10);

			this.packageOwnersLabel = CreateLabel ("packageOwnersLabel", global::Mono.Unix.Catalog.GetString ("Owners:"));
			DesignerLayout.Attach (this.detailsTable, this.packageOwnersLabel, 0, 0);
			this.packageOwnersTextBox = CreateEntry ("packageOwnersTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageOwnersTextBox, 1, 0);

			this.packageCopyrightLabel = CreateLabel ("packageCopyrightLabel", global::Mono.Unix.Catalog.GetString ("Copyright:"));
			DesignerLayout.Attach (this.detailsTable, this.packageCopyrightLabel, 0, 1);
			this.packageCopyrightTextBox = CreateEntry ("packageCopyrightTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageCopyrightTextBox, 1, 1);

			this.packageTitleLabel = CreateLabel ("packageTitleLabel", global::Mono.Unix.Catalog.GetString ("Title:"));
			DesignerLayout.Attach (this.detailsTable, this.packageTitleLabel, 0, 2);
			this.packageTitleTextBox = CreateEntry ("packageTitleTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageTitleTextBox, 1, 2);

			this.packageSummaryLabel = CreateLabel ("packageSummaryLabel", global::Mono.Unix.Catalog.GetString ("Summary:"));
			DesignerLayout.Attach (this.detailsTable, this.packageSummaryLabel, 0, 3);
			this.packageSummaryTextBox = CreateEntry ("packageSummaryTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageSummaryTextBox, 1, 3);

			this.packageProjectUrlLabel = CreateLabel ("packageProjectUrlLabel", global::Mono.Unix.Catalog.GetString ("Project URL:"));
			DesignerLayout.Attach (this.detailsTable, this.packageProjectUrlLabel, 0, 4);
			this.packageProjectUrlTextBox = CreateEntry ("packageProjectUrlTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageProjectUrlTextBox, 1, 4);

			this.packageIconUrlLabel = CreateLabel ("packageIconUrlLabel", global::Mono.Unix.Catalog.GetString ("Icon URL:"));
			DesignerLayout.Attach (this.detailsTable, this.packageIconUrlLabel, 0, 5);
			this.packageIconUrlTextBox = CreateEntry ("packageIconUrlTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageIconUrlTextBox, 1, 5);

			this.packageLicenseUrlLabel = CreateLabel ("packageLicenseUrlLabel", global::Mono.Unix.Catalog.GetString ("License URL:"));
			DesignerLayout.Attach (this.detailsTable, this.packageLicenseUrlLabel, 0, 6);
			this.packageLicenseUrlTextBox = CreateEntry ("packageLicenseUrlTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageLicenseUrlTextBox, 1, 6);

			this.packageRequireLicenseAcceptanceLabel = CreateLabel ("packageRequireLicenseAcceptanceLabel", global::Mono.Unix.Catalog.GetString ("Require License Acceptance:"));
			DesignerLayout.Attach (this.detailsTable, this.packageRequireLicenseAcceptanceLabel, 0, 7);
			this.packageRequireLicenseAcceptanceCheckBox = CreateCheck ("packageRequireLicenseAcceptanceCheckBox");
			DesignerLayout.Attach (this.detailsTable, this.packageRequireLicenseAcceptanceCheckBox, 1, 7);

			this.packageDevelopmentDependencyLabel = CreateLabel ("packageDevelopmentDependencyLabel", global::Mono.Unix.Catalog.GetString ("Development Dependency:"));
			DesignerLayout.Attach (this.detailsTable, this.packageDevelopmentDependencyLabel, 0, 8);
			this.packageDevelopmentDependencyCheckBox = CreateCheck ("packageDevelopmentDependencyCheckBox");
			DesignerLayout.Attach (this.detailsTable, this.packageDevelopmentDependencyCheckBox, 1, 8);

			this.packageTagsLabel = CreateLabel ("packageTagsLabel", global::Mono.Unix.Catalog.GetString ("Tags:"));
			DesignerLayout.Attach (this.detailsTable, this.packageTagsLabel, 0, 9);
			this.packageTagsTextBox = CreateEntry ("packageTagsTextBox");
			DesignerLayout.Attach (this.detailsTable, this.packageTagsTextBox, 1, 9);

			this.packageLanguageLabel = CreateLabel ("packageLanguageLabel", global::Mono.Unix.Catalog.GetString ("Language:"));
			DesignerLayout.Attach (this.detailsTable, this.packageLanguageLabel, 0, 10);
			this.packageLanguageHBox = DesignerLayout.HBox (6);
			this.packageLanguageHBox.Name = "packageLanguageHBox";
			this.packageLanguageComboBox = new global::Gtk.ComboBoxText ();
			this.packageLanguageComboBox.Name = "packageLanguageComboBox";
			this.packageLanguageComboBox.Hexpand = true;
			DesignerLayout.Pack (this.packageLanguageHBox, this.packageLanguageComboBox);
			DesignerLayout.Attach (this.detailsTable, this.packageLanguageHBox, 1, 10);

			this.packageReleaseNotesLabelVBox = DesignerLayout.VBox (6);
			this.packageReleaseNotesLabelVBox.Name = "packageReleaseNotesLabelVBox";
			this.packageReleaseNotesLabelVBox.Valign = global::Gtk.Align.Start;
			this.packageReleaseNotesLabel = CreateLabel ("packageReleaseNotesLabel", global::Mono.Unix.Catalog.GetString ("Release Notes:"));
			DesignerLayout.Pack (this.packageReleaseNotesLabelVBox, this.packageReleaseNotesLabel);
			this.packageReleaseNotesPaddingLabel = CreateLabel ("packageReleaseNotesPaddingLabel", null);
			DesignerLayout.Pack (this.packageReleaseNotesLabelVBox, this.packageReleaseNotesPaddingLabel);
			DesignerLayout.Attach (this.detailsTable, this.packageReleaseNotesLabelVBox, 0, 11);
			this.packageReleaseNotesTextView = CreateTextView ("packageReleaseNotesTextView");
			this.packageReleaseNotesScrolledWindow = DesignerLayout.Scrolled (this.packageReleaseNotesTextView);
			this.packageReleaseNotesScrolledWindow.Name = "packageReleaseNotesScrolledWindow";
			DesignerLayout.Attach (this.detailsTable, this.packageReleaseNotesScrolledWindow, 1, 11, vexpand: true);

			this.notebook.AppendPage (this.detailsTable, null);
			this.detailsTabPageLabel = new global::Gtk.Label ((string)null);
			this.detailsTabPageLabel.Name = "detailsTabPageLabel";
			this.detailsTabPageLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Details");
			this.notebook.SetTabLabel (this.detailsTable, this.detailsTabPageLabel);

			DesignerLayout.SetContent (this, this.notebook);
			this.generalTabPageLabel.Visible = false;
		}

		static global::Gtk.Label CreateLabel (string name, string text)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.Xalign = 0F;
			label.Halign = global::Gtk.Align.Start;
			if (text != null)
				label.LabelProp = text;
			return label;
		}

		static global::Gtk.Entry CreateEntry (string name)
		{
			var entry = new global::Gtk.Entry ();
			entry.CanFocus = true;
			entry.Name = name;
			entry.Editable = true;
			entry.Hexpand = true;
			return entry;
		}

		static global::Gtk.CheckButton CreateCheck (string name)
		{
			var check = new global::Gtk.CheckButton ();
			check.CanFocus = true;
			check.Name = name;
			check.Label = "";
			check.UseUnderline = true;
			check.Halign = global::Gtk.Align.Start;
			return check;
		}

		static global::Gtk.TextView CreateTextView (string name)
		{
			var textView = new global::Gtk.TextView ();
			textView.CanFocus = true;
			textView.Name = name;
			textView.AcceptsTab = false;
			textView.WrapMode = global::Gtk.WrapMode.Word;
			return textView;
		}
	}
}
