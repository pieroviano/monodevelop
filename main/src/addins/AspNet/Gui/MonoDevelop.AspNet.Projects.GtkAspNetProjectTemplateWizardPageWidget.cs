// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.AspNet.Projects
{
	internal partial class GtkAspNetProjectTemplateWizardPageWidget
	{
		private global::Gtk.Box mainHBox;
		private global::Gtk.Box leftBorderEventBox;
		private global::Gtk.Box configurationVBox;
		private global::Gtk.Box configurationTopEventBox;
		private global::Gtk.Box configurationTableEventBox;
		private global::Gtk.Grid configurationTable;
		private global::Gtk.Box includeLabelVBox;
		private global::Gtk.Label includeLabelPadding;
		private global::Gtk.Label includeLabel;
		private global::Gtk.Box includeUnitTestProjectVBox;
		private global::Gtk.CheckButton includeTestProjectCheck;
		private global::Gtk.Box includeUnitTestProjectDescriptionHBox;
		private global::Gtk.Label includeUnitTestProjectDescriptionLeftHandPadding;
		private global::Gtk.Label includeUnitTestProjectDescriptionLabel;
		private global::Gtk.Box mvcVBox;
		private global::Gtk.CheckButton includeMvcCheck;
		private global::Gtk.Box mvcDescriptionHBox;
		private global::Gtk.Label mvcDescriptionLeftHandPadding;
		private global::Gtk.Label mvcDescriptionLabel;
		private global::Gtk.Label paddingLabel;
		private global::Gtk.Box testingLabelVBox;
		private global::Gtk.Label testingLabelPadding;
		private global::Gtk.Label testingLabel;
		private global::Gtk.Box testingSeparator;
		private global::Gtk.Box webApiVBox;
		private global::Gtk.CheckButton includeWebApiCheck;
		private global::Gtk.Box webApiDescriptionHBox;
		private global::Gtk.Label webApiDescriptionLeftHandPadding;
		private global::Gtk.Label webApiDescriptionLabel;
		private global::Gtk.Box webFormsVBox;
		private global::Gtk.CheckButton includeWebFormsCheck;
		private global::Gtk.Box webFormsDescriptionHBox;
		private global::Gtk.Label webFormsDescriptionLeftHandPadding;
		private global::Gtk.Label webFormsDescriptionLabel;
		private global::Gtk.Box configurationBottomEventBox;
		private global::Gtk.Box backgroundLargeImageEventBox;
		private global::Gtk.Box backgroundLargeImageVBox;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.AspNet.Projects.GtkAspNetProjectTemplateWizardPageWidget";
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

			// "Include:" label (row 0, column 0)
			this.includeLabelVBox = DesignerLayout.VBox (0);
			this.includeLabelVBox.Name = "includeLabelVBox";
			this.includeLabelPadding = CreatePaddingLabel ("includeLabelPadding", 0, 3);
			DesignerLayout.Pack (this.includeLabelVBox, this.includeLabelPadding);
			this.includeLabel = CreateRowLabel ("includeLabel", global::Mono.Unix.Catalog.GetString ("Include:"));
			DesignerLayout.Pack (this.includeLabelVBox, this.includeLabel);
			DesignerLayout.Attach (this.configurationTable, this.includeLabelVBox, 0, 0);

			// MVC (row 0, column 1)
			this.mvcVBox = CreateOptionBox ("mvc", global::Mono.Unix.Catalog.GetString ("MVC"),
				global::Mono.Unix.Catalog.GetString ("<span size=\'smaller\' color=\'grey\'>Modern programming model. Unit testable, choice of templating languages</span>"),
				out this.includeMvcCheck, out this.mvcDescriptionHBox, out this.mvcDescriptionLeftHandPadding, out this.mvcDescriptionLabel);
			DesignerLayout.Attach (this.configurationTable, this.mvcVBox, 1, 0);

			// Web Forms (row 1, column 1)
			this.webFormsVBox = CreateOptionBox ("webForms", global::Mono.Unix.Catalog.GetString ("Web Forms"),
				global::Mono.Unix.Catalog.GetString ("<span size=\'smaller\' color=\'grey\'>Stateful programming model similar to desktop applications</span>"),
				out this.includeWebFormsCheck, out this.webFormsDescriptionHBox, out this.webFormsDescriptionLeftHandPadding, out this.webFormsDescriptionLabel);
			DesignerLayout.Attach (this.configurationTable, this.webFormsVBox, 1, 1);

			// Web API (row 2, column 1)
			this.webApiVBox = CreateOptionBox ("webApi", global::Mono.Unix.Catalog.GetString ("Web API"),
				global::Mono.Unix.Catalog.GetString ("<span size=\'smaller\' color=\'grey\'>Framework for creating HTTP web services</span>"),
				out this.includeWebApiCheck, out this.webApiDescriptionHBox, out this.webApiDescriptionLeftHandPadding, out this.webApiDescriptionLabel);
			DesignerLayout.Attach (this.configurationTable, this.webApiVBox, 1, 2);

			// Separator (row 3, all columns)
			this.testingSeparator = DesignerLayout.VBox (0);
			this.testingSeparator.WidthRequest = 440;
			this.testingSeparator.HeightRequest = 1;
			this.testingSeparator.MarginTop = 10;
			this.testingSeparator.MarginBottom = 10;
			this.testingSeparator.Name = "testingSeparator";
			DesignerLayout.Attach (this.configurationTable, this.testingSeparator, 0, 3, 3);

			// "Testing:" label and unit test option (row 4)
			this.testingLabelVBox = DesignerLayout.VBox (0);
			this.testingLabelVBox.Name = "testingLabelVBox";
			this.testingLabelPadding = CreatePaddingLabel ("testingLabelPadding", 0, 3);
			DesignerLayout.Pack (this.testingLabelVBox, this.testingLabelPadding);
			this.testingLabel = CreateRowLabel ("testingLabel", global::Mono.Unix.Catalog.GetString ("Testing:"));
			DesignerLayout.Pack (this.testingLabelVBox, this.testingLabel);
			DesignerLayout.Attach (this.configurationTable, this.testingLabelVBox, 0, 4);

			this.includeUnitTestProjectVBox = CreateOptionBox ("includeUnitTestProject", global::Mono.Unix.Catalog.GetString ("Include Unit Test Project"),
				global::Mono.Unix.Catalog.GetString ("<span size=\'smaller\' color=\'grey\'>Add a Unit Test Project for testing the Web Project using NUnit</span>"),
				out this.includeTestProjectCheck, out this.includeUnitTestProjectDescriptionHBox, out this.includeUnitTestProjectDescriptionLeftHandPadding, out this.includeUnitTestProjectDescriptionLabel);
			DesignerLayout.Attach (this.configurationTable, this.includeUnitTestProjectVBox, 1, 4);

			this.paddingLabel = new global::Gtk.Label ((string)null);
			this.paddingLabel.WidthRequest = 132;
			this.paddingLabel.Name = "paddingLabel";
			DesignerLayout.Attach (this.configurationTable, this.paddingLabel, 0, 5);

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

		static global::Gtk.Label CreatePaddingLabel (string name, int width, int height)
		{
			var label = new global::Gtk.Label ((string)null);
			label.WidthRequest = width;
			label.HeightRequest = height;
			label.Name = name;
			return label;
		}

		static global::Gtk.Label CreateRowLabel (string name, string text)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.MarginStart = 5;
			label.MarginEnd = 5;
			label.Xalign = 1F;
			label.Yalign = 0F;
			label.LabelProp = text;
			label.Justify = global::Gtk.Justification.Right;
			return label;
		}

		static global::Gtk.Box CreateOptionBox (string name, string checkLabel, string description,
			out global::Gtk.CheckButton check, out global::Gtk.Box descriptionBox, out global::Gtk.Label leftHandPadding, out global::Gtk.Label descriptionLabel)
		{
			var box = DesignerLayout.VBox (0);
			box.Name = name + "VBox";

			check = new global::Gtk.CheckButton ();
			check.Name = name + "Check";
			check.Label = checkLabel;
			check.Active = true;
			check.UseUnderline = true;
			DesignerLayout.Pack (box, check);

			descriptionBox = DesignerLayout.HBox (0);
			descriptionBox.Name = name + "DescriptionHBox";
			leftHandPadding = CreatePaddingLabel (name + "DescriptionLeftHandPadding", 21, -1);
			DesignerLayout.Pack (descriptionBox, leftHandPadding);
			descriptionLabel = new global::Gtk.Label ((string)null);
			descriptionLabel.WidthRequest = 255;
			descriptionLabel.Name = name + "DescriptionLabel";
			descriptionLabel.Xalign = 0F;
			descriptionLabel.UseMarkup = true;
			descriptionLabel.LabelProp = description;
			descriptionLabel.Wrap = true;
			DesignerLayout.Pack (descriptionBox, descriptionLabel);
			DesignerLayout.Pack (box, descriptionBox);
			return box;
		}
	}
}
