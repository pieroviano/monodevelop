// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Packaging.Gui
{
	public partial class GtkPackagingProjectTemplateWizardPageWidget
	{
		private global::Gtk.Box mainHBox;
		private global::Gtk.Box leftBorderEventBox;
		private global::Gtk.Box configurationVBox;
		private global::Gtk.Box configurationTopEventBox;
		private global::Gtk.Box configurationTableEventBox;
		private global::Gtk.Grid configurationTable;
		private global::Gtk.Box bottomPadding;
		private global::Gtk.Box idEventBox;
		private global::Gtk.Box organizationInfoEventBox;
		private global::Gtk.Box versionEventBox;
		private global::Gtk.Label packageAuthorsLabel;
		private global::Gtk.Entry packageAuthorsTextBox;
		private global::Gtk.Label packageDescriptionLabel;
		private global::Gtk.Entry packageDescriptionTextBox;
		private global::Gtk.Label packageIdLabel;
		private global::Gtk.Entry packageIdTextBox;
		private global::Gtk.Box separator;
		private global::Gtk.Box topPadding;
		private global::Gtk.Box configurationBottomEventBox;
		private global::Gtk.Box backgroundLargeImageEventBox;
		private global::Gtk.Box backgroundLargeImageVBox;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.Packaging.Gui.GtkPackagingProjectTemplateWizardPageWidget";
			this.mainHBox = DesignerLayout.HBox (0);
			this.mainHBox.Name = "mainHBox";

			this.leftBorderEventBox = DesignerLayout.VBox (0);
			this.leftBorderEventBox.WidthRequest = 30;
			this.leftBorderEventBox.Name = "leftBorderEventBox";
			DesignerLayout.Pack (this.mainHBox, this.leftBorderEventBox);

			this.configurationVBox = DesignerLayout.VBox (0);
			this.configurationVBox.Name = "configurationVBox";

			this.configurationTopEventBox = DesignerLayout.VBox (0);
			this.configurationTopEventBox.Name = "configurationTopEventBox";
			DesignerLayout.Pack (this.configurationVBox, this.configurationTopEventBox, true);

			this.configurationTableEventBox = DesignerLayout.VBox (0);
			this.configurationTableEventBox.Name = "configurationTableEventBox";
			this.configurationTable = DesignerLayout.Grid (7, 6);
			this.configurationTable.Name = "configurationTable";

			// Row 0: top padding
			this.topPadding = DesignerLayout.VBox (0);
			this.topPadding.WidthRequest = 132;
			this.topPadding.HeightRequest = 0;
			this.topPadding.Name = "topPadding";
			DesignerLayout.Attach (this.configurationTable, this.topPadding, 0, 0);

			// Row 1: ID
			this.packageIdLabel = CreateRowLabel ("packageIdLabel", global::Mono.Unix.Catalog.GetString ("ID:"));
			DesignerLayout.Attach (this.configurationTable, this.packageIdLabel, 0, 1);
			this.packageIdTextBox = CreateEntry ("packageIdTextBox");
			this.packageIdTextBox.WidthRequest = 280;
			DesignerLayout.Attach (this.configurationTable, this.packageIdTextBox, 1, 1);
			this.idEventBox = CreateInfoBox ("idEventBox");
			DesignerLayout.Attach (this.configurationTable, this.idEventBox, 2, 1);

			// Row 2: separator and organization info
			this.separator = DesignerLayout.VBox (0);
			this.separator.HeightRequest = 1;
			this.separator.Name = "separator";
			DesignerLayout.Attach (this.configurationTable, this.separator, 0, 2, 3);
			this.organizationInfoEventBox = CreateInfoBox ("organizationInfoEventBox");
			this.versionEventBox = CreateInfoBox ("versionEventBox");
			DesignerLayout.Pack (this.organizationInfoEventBox, this.versionEventBox);
			DesignerLayout.Attach (this.configurationTable, this.organizationInfoEventBox, 2, 2);

			// Row 3: authors
			this.packageAuthorsLabel = CreateRowLabel ("packageAuthorsLabel", global::Mono.Unix.Catalog.GetString ("Author:"));
			DesignerLayout.Attach (this.configurationTable, this.packageAuthorsLabel, 0, 3);
			this.packageAuthorsTextBox = CreateEntry ("packageAuthorsTextBox");
			DesignerLayout.Attach (this.configurationTable, this.packageAuthorsTextBox, 1, 3);

			// Row 4: description
			this.packageDescriptionLabel = CreateRowLabel ("packageDescriptionLabel", global::Mono.Unix.Catalog.GetString ("Description:"));
			DesignerLayout.Attach (this.configurationTable, this.packageDescriptionLabel, 0, 4);
			this.packageDescriptionTextBox = CreateEntry ("packageDescriptionTextBox");
			DesignerLayout.Attach (this.configurationTable, this.packageDescriptionTextBox, 1, 4);

			// Row 5: bottom padding
			this.bottomPadding = DesignerLayout.VBox (0);
			this.bottomPadding.WidthRequest = 132;
			this.bottomPadding.HeightRequest = 0;
			this.bottomPadding.Name = "bottomPadding";
			DesignerLayout.Attach (this.configurationTable, this.bottomPadding, 0, 5);

			DesignerLayout.Pack (this.configurationTableEventBox, this.configurationTable);
			DesignerLayout.Pack (this.configurationVBox, this.configurationTableEventBox);

			this.configurationBottomEventBox = DesignerLayout.VBox (0);
			this.configurationBottomEventBox.Name = "configurationBottomEventBox";
			DesignerLayout.Pack (this.configurationVBox, this.configurationBottomEventBox, true);
			DesignerLayout.Pack (this.mainHBox, this.configurationVBox);

			this.backgroundLargeImageEventBox = DesignerLayout.VBox (0);
			this.backgroundLargeImageEventBox.Name = "backgroundLargeImageEventBox";
			this.backgroundLargeImageVBox = DesignerLayout.VBox (0);
			this.backgroundLargeImageVBox.Name = "backgroundLargeImageVBox";
			DesignerLayout.Pack (this.backgroundLargeImageEventBox, this.backgroundLargeImageVBox, true);
			DesignerLayout.Pack (this.mainHBox, this.backgroundLargeImageEventBox, true);

			DesignerLayout.SetContent (this, this.mainHBox);
		}

		static global::Gtk.Label CreateRowLabel (string name, string text)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.MarginStart = 5;
			label.MarginEnd = 5;
			label.Xalign = 1F;
			label.Halign = global::Gtk.Align.End;
			label.LabelProp = text;
			label.Justify = global::Gtk.Justification.Right;
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

		static global::Gtk.Box CreateInfoBox (string name)
		{
			var box = DesignerLayout.VBox (0);
			box.WidthRequest = 16;
			box.HeightRequest = 16;
			box.Name = name;
			return box;
		}
	}
}
