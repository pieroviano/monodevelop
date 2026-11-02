// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.PackageManagement
{
	internal partial class PackageManagementOptionsWidget
	{
		private global::Gtk.Box mainVBox;
		private global::Gtk.Box restorePackagesLabelHBox;
		private global::Gtk.Label restorePackagesLabel;
		private global::Gtk.Label restorePackagesPaddingLabel;
		private global::Gtk.CheckButton automaticPackageRestoreOnOpeningSolutionCheckBox;
		private global::Gtk.Box packageUpdatesLabelHBox;
		private global::Gtk.Label packageUpdatesLabel;
		private global::Gtk.Label packageUpdatesPaddingLabel;
		private global::Gtk.CheckButton checkForPackageUpdatesOnOpeningSolutionCheckBox;
		private global::Gtk.Label bottomLabel;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.PackageManagement.PackageManagementOptionsWidget";
			this.mainVBox = DesignerLayout.VBox (6);
			this.mainVBox.Name = "mainVBox";

			this.restorePackagesLabelHBox = DesignerLayout.HBox (6);
			this.restorePackagesLabelHBox.Name = "restorePackagesLabelHBox";
			this.restorePackagesLabel = new global::Gtk.Label ((string)null);
			this.restorePackagesLabel.Name = "restorePackagesLabel";
			this.restorePackagesLabel.UseMarkup = true;
			this.restorePackagesLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Package Restore</b>");
			DesignerLayout.Pack (this.restorePackagesLabelHBox, this.restorePackagesLabel);
			this.restorePackagesPaddingLabel = new global::Gtk.Label ((string)null);
			this.restorePackagesPaddingLabel.Name = "restorePackagesPaddingLabel";
			DesignerLayout.Pack (this.restorePackagesLabelHBox, this.restorePackagesPaddingLabel, true);
			DesignerLayout.Pack (this.mainVBox, this.restorePackagesLabelHBox);

			this.automaticPackageRestoreOnOpeningSolutionCheckBox = new global::Gtk.CheckButton ();
			this.automaticPackageRestoreOnOpeningSolutionCheckBox.CanFocus = true;
			this.automaticPackageRestoreOnOpeningSolutionCheckBox.Name = "automaticPackageRestoreOnOpeningSolutionCheckBox";
			this.automaticPackageRestoreOnOpeningSolutionCheckBox.Label = global::Mono.Unix.Catalog.GetString ("_Automatically restore packages when opening a solution.");
			this.automaticPackageRestoreOnOpeningSolutionCheckBox.UseUnderline = true;
			DesignerLayout.Margin (this.automaticPackageRestoreOnOpeningSolutionCheckBox, 10);
			DesignerLayout.Pack (this.mainVBox, this.automaticPackageRestoreOnOpeningSolutionCheckBox);

			this.packageUpdatesLabelHBox = DesignerLayout.HBox (6);
			this.packageUpdatesLabelHBox.Name = "packageUpdatesLabelHBox";
			this.packageUpdatesLabel = new global::Gtk.Label ((string)null);
			this.packageUpdatesLabel.Name = "packageUpdatesLabel";
			this.packageUpdatesLabel.UseMarkup = true;
			this.packageUpdatesLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Package Updates</b>");
			DesignerLayout.Pack (this.packageUpdatesLabelHBox, this.packageUpdatesLabel);
			this.packageUpdatesPaddingLabel = new global::Gtk.Label ((string)null);
			this.packageUpdatesPaddingLabel.Name = "packageUpdatesPaddingLabel";
			DesignerLayout.Pack (this.packageUpdatesLabelHBox, this.packageUpdatesPaddingLabel, true);
			DesignerLayout.Pack (this.mainVBox, this.packageUpdatesLabelHBox);

			this.checkForPackageUpdatesOnOpeningSolutionCheckBox = new global::Gtk.CheckButton ();
			this.checkForPackageUpdatesOnOpeningSolutionCheckBox.CanFocus = true;
			this.checkForPackageUpdatesOnOpeningSolutionCheckBox.Name = "checkForPackageUpdatesOnOpeningSolutionCheckBox";
			this.checkForPackageUpdatesOnOpeningSolutionCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Check for package _updates when opening a solution.");
			this.checkForPackageUpdatesOnOpeningSolutionCheckBox.UseUnderline = true;
			DesignerLayout.Margin (this.checkForPackageUpdatesOnOpeningSolutionCheckBox, 10);
			DesignerLayout.Pack (this.mainVBox, this.checkForPackageUpdatesOnOpeningSolutionCheckBox);

			this.bottomLabel = new global::Gtk.Label ((string)null);
			this.bottomLabel.Name = "bottomLabel";
			DesignerLayout.Pack (this.mainVBox, this.bottomLabel, true);

			DesignerLayout.SetContent (this, this.mainVBox);
		}
	}
}
