// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Packaging.Gui
{
	public partial class GtkCrossPlatformLibraryProjectTemplateWizardPageWidget
	{
		private global::Gtk.Box mainHBox;
		private global::Gtk.Box leftBorderEventBox;
		private global::Gtk.Box configurationVBox;
		private global::Gtk.Box configurationTopEventBox;
		private global::Gtk.Box configurationTableEventBox;
		private global::Gtk.Grid configurationTable;
		private global::Gtk.Label descriptionLabel;
		private global::Gtk.Entry descriptionTextBox;
		private global::Gtk.Label implementationLabel;
		private global::Gtk.Box nameEventBox;
		private global::Gtk.Label nameLabel;
		private global::Gtk.Entry nameTextBox;
		private global::Gtk.Box organizationInfoEventBox;
		private global::Gtk.Label paddingLabelSharedCode;
		private global::Gtk.Label paddingLabelTargetPlatforms;
		private global::Gtk.Label paddingLabelXaml;
		private global::Gtk.Box sharedCodeSeparator;
		private global::Gtk.Box sharedCodeVBox;
		private global::Gtk.Box usePortableClassLibraryHBox;
		private global::Gtk.CheckButton sharedProjectRadioButton;
		private global::Gtk.Box usePortableLibraryInfoVBox;
		private global::Gtk.Label usePortableLibraryInfoIconPaddingLabel;
		private global::Gtk.Box usePortableLibraryInfoEventBox;
		private global::Gtk.Box useSharedLibraryHBox;
		private global::Gtk.CheckButton portableClassLibraryRadioButton;
		private global::Gtk.Box useSharedLibraryInfoVBox;
		private global::Gtk.Label useSharedLibraryInfoIconPaddingLabel;
		private global::Gtk.Box useSharedLibraryInfoEventBox;
		private global::Gtk.Label targetPlatformsLabel;
		private global::Gtk.Box targetPlatformsSeparator;
		private global::Gtk.Box targetPlatformsVBox;
		private global::Gtk.CheckButton androidCheckButton;
		private global::Gtk.CheckButton iOSCheckButton;
		private global::Gtk.Box configurationBottomEventBox;
		private global::Gtk.Box backgroundLargeImageEventBox;
		private global::Gtk.Box backgroundLargeImageVBox;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.Packaging.Gui.GtkCrossPlatformLibraryProjectTemplateWizardPageWidget";
			this.mainHBox = DesignerLayout.HBox (0);
			this.mainHBox.Name = "mainHBox";

			this.leftBorderEventBox = DesignerLayout.VBox (0);
			this.leftBorderEventBox.WidthRequest = 30;
			this.leftBorderEventBox.Name = "leftBorderEventBox";
			DesignerLayout.Pack (this.mainHBox, this.leftBorderEventBox);

			this.configurationVBox = DesignerLayout.VBox (0);
			this.configurationVBox.WidthRequest = 440;
			this.configurationVBox.Name = "configurationVBox";

			this.configurationTopEventBox = DesignerLayout.VBox (0);
			this.configurationTopEventBox.Name = "configurationTopEventBox";
			DesignerLayout.Pack (this.configurationVBox, this.configurationTopEventBox, true);

			this.configurationTableEventBox = DesignerLayout.VBox (0);
			this.configurationTableEventBox.Name = "configurationTableEventBox";
			this.configurationTable = DesignerLayout.Grid (7, 6);
			this.configurationTable.Name = "configurationTable";

			// Row 0: name
			this.nameLabel = CreateRowLabel ("nameLabel", global::Mono.Unix.Catalog.GetString ("Name:"), 0.5F);
			DesignerLayout.Attach (this.configurationTable, this.nameLabel, 0, 0);
			this.nameTextBox = CreateEntry ("nameTextBox");
			DesignerLayout.Attach (this.configurationTable, this.nameTextBox, 1, 0);
			this.nameEventBox = CreateInfoBox ("nameEventBox");
			DesignerLayout.Attach (this.configurationTable, this.nameEventBox, 2, 0);

			// Row 1: description
			this.descriptionLabel = CreateRowLabel ("descriptionLabel", global::Mono.Unix.Catalog.GetString ("Description:"), 0.5F);
			DesignerLayout.Attach (this.configurationTable, this.descriptionLabel, 0, 1);
			this.descriptionTextBox = CreateEntry ("descriptionTextBox");
			DesignerLayout.Attach (this.configurationTable, this.descriptionTextBox, 1, 1);
			this.organizationInfoEventBox = CreateInfoBox ("organizationInfoEventBox");
			DesignerLayout.Attach (this.configurationTable, this.organizationInfoEventBox, 2, 1);

			// Row 2: target platforms separator
			this.targetPlatformsSeparator = CreateSeparator ("targetPlatformsSeparator");
			DesignerLayout.Attach (this.configurationTable, this.targetPlatformsSeparator, 0, 2, 3);

			// Rows 3-4: target platforms
			this.targetPlatformsLabel = CreateRowLabel ("targetPlatformsLabel", global::Mono.Unix.Catalog.GetString ("Target Platforms:"), 0.8F);
			DesignerLayout.Attach (this.configurationTable, this.targetPlatformsLabel, 0, 3);
			this.targetPlatformsVBox = DesignerLayout.VBox (2);
			this.targetPlatformsVBox.Name = "targetPlatformsVBox";
			this.androidCheckButton = CreateCheck ("androidCheckButton", global::Mono.Unix.Catalog.GetString ("Android"));
			DesignerLayout.Pack (this.targetPlatformsVBox, this.androidCheckButton);
			this.iOSCheckButton = CreateCheck ("iOSCheckButton", global::Mono.Unix.Catalog.GetString ("iOS"));
			DesignerLayout.Pack (this.targetPlatformsVBox, this.iOSCheckButton);
			DesignerLayout.Attach (this.configurationTable, this.targetPlatformsVBox, 1, 3, 1, 2);
			this.paddingLabelTargetPlatforms = CreatePaddingLabel ("paddingLabelTargetPlatforms", 132, -1);
			DesignerLayout.Attach (this.configurationTable, this.paddingLabelTargetPlatforms, 0, 4);

			// Row 5: shared code separator
			this.sharedCodeSeparator = CreateSeparator ("sharedCodeSeparator");
			DesignerLayout.Attach (this.configurationTable, this.sharedCodeSeparator, 0, 5, 3);

			// Rows 6-7: implementation
			this.implementationLabel = CreateRowLabel ("implementationLabel", global::Mono.Unix.Catalog.GetString ("Implementation:"), 0.8F);
			DesignerLayout.Attach (this.configurationTable, this.implementationLabel, 0, 6);
			this.sharedCodeVBox = DesignerLayout.VBox (2);
			this.sharedCodeVBox.Name = "sharedCodeVBox";

			this.usePortableClassLibraryHBox = DesignerLayout.HBox (0);
			this.usePortableClassLibraryHBox.Name = "usePortableClassLibraryHBox";
			this.sharedProjectRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Platform specific"));
			this.sharedProjectRadioButton.CanFocus = true;
			this.sharedProjectRadioButton.Name = "sharedProjectRadioButton";
			this.sharedProjectRadioButton.Active = true;
			DesignerLayout.Pack (this.usePortableClassLibraryHBox, this.sharedProjectRadioButton);
			this.usePortableLibraryInfoVBox = DesignerLayout.VBox (0);
			this.usePortableLibraryInfoVBox.Name = "usePortableLibraryInfoVBox";
			this.usePortableLibraryInfoIconPaddingLabel = CreatePaddingLabel ("usePortableLibraryInfoIconPaddingLabel", -1, 1);
			DesignerLayout.Pack (this.usePortableLibraryInfoVBox, this.usePortableLibraryInfoIconPaddingLabel);
			this.usePortableLibraryInfoEventBox = CreateInfoBox ("usePortableLibraryInfoEventBox");
			DesignerLayout.Pack (this.usePortableLibraryInfoVBox, this.usePortableLibraryInfoEventBox);
			DesignerLayout.Pack (this.usePortableClassLibraryHBox, this.usePortableLibraryInfoVBox);
			DesignerLayout.Pack (this.sharedCodeVBox, this.usePortableClassLibraryHBox);

			this.useSharedLibraryHBox = DesignerLayout.HBox (0);
			this.useSharedLibraryHBox.Name = "useSharedLibraryHBox";
			this.portableClassLibraryRadioButton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Single for all platforms"), this.sharedProjectRadioButton);
			this.portableClassLibraryRadioButton.CanFocus = true;
			this.portableClassLibraryRadioButton.Name = "portableClassLibraryRadioButton";
			DesignerLayout.Pack (this.useSharedLibraryHBox, this.portableClassLibraryRadioButton);
			this.useSharedLibraryInfoVBox = DesignerLayout.VBox (0);
			this.useSharedLibraryInfoVBox.Name = "useSharedLibraryInfoVBox";
			this.useSharedLibraryInfoIconPaddingLabel = CreatePaddingLabel ("useSharedLibraryInfoIconPaddingLabel", -1, 1);
			DesignerLayout.Pack (this.useSharedLibraryInfoVBox, this.useSharedLibraryInfoIconPaddingLabel);
			this.useSharedLibraryInfoEventBox = CreateInfoBox ("useSharedLibraryInfoEventBox");
			DesignerLayout.Pack (this.useSharedLibraryInfoVBox, this.useSharedLibraryInfoEventBox);
			DesignerLayout.Pack (this.useSharedLibraryHBox, this.useSharedLibraryInfoVBox);
			DesignerLayout.Pack (this.sharedCodeVBox, this.useSharedLibraryHBox);
			DesignerLayout.Attach (this.configurationTable, this.sharedCodeVBox, 1, 6, 2, 2);

			// Rows 7-8: paddings
			this.paddingLabelSharedCode = CreatePaddingLabel ("paddingLabelSharedCode", 132, -1);
			DesignerLayout.Attach (this.configurationTable, this.paddingLabelSharedCode, 0, 7);
			this.paddingLabelXaml = CreatePaddingLabel ("paddingLabelXaml", 132, 5);
			DesignerLayout.Attach (this.configurationTable, this.paddingLabelXaml, 0, 8);

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

		static global::Gtk.Label CreateRowLabel (string name, string text, float yalign)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.MarginStart = 5;
			label.MarginEnd = 5;
			label.Xalign = 1F;
			label.Yalign = yalign;
			label.Halign = global::Gtk.Align.End;
			label.LabelProp = text;
			label.Justify = global::Gtk.Justification.Right;
			return label;
		}

		static global::Gtk.Label CreatePaddingLabel (string name, int width, int height)
		{
			var label = new global::Gtk.Label ((string)null);
			label.WidthRequest = width;
			label.HeightRequest = height;
			label.Name = name;
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

		static global::Gtk.CheckButton CreateCheck (string name, string label)
		{
			var check = new global::Gtk.CheckButton ();
			check.CanFocus = true;
			check.Name = name;
			check.Label = label;
			check.Active = true;
			check.UseUnderline = true;
			return check;
		}

		static global::Gtk.Box CreateInfoBox (string name)
		{
			var box = DesignerLayout.VBox (0);
			box.WidthRequest = 16;
			box.HeightRequest = 16;
			box.Name = name;
			return box;
		}

		static global::Gtk.Box CreateSeparator (string name)
		{
			var box = DesignerLayout.VBox (0);
			box.HeightRequest = 1;
			box.MarginTop = 10;
			box.MarginBottom = 10;
			box.Name = name;
			return box;
		}
	}
}
