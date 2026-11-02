using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class GeneralProjectOptionsWidget
	{
		private global::Gtk.Box vbox40;
		private global::Gtk.Box vbox47;
		private global::Gtk.Label informationHeaderLabel;
		private global::Gtk.Box hbox29;
		private global::Gtk.Label label55;
		private global::Gtk.Box vbox46;
		private global::Gtk.Grid table11;
		private global::Gtk.Label defaultNamespaceLabel;
		private global::Gtk.Label descriptionLabel;
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry entryVersion;
		private global::Gtk.CheckButton checkSolutionVersion;
		private global::Gtk.Label label1;
		private global::Gtk.Label nameLabel;
		private global::Gtk.Entry projectDefaultNamespaceEntry;
		private global::Gtk.Entry projectNameEntry;
		private global::Gtk.ScrolledWindow scrolledwindow5;
		private global::Gtk.TextView projectDescriptionTextView;
		private global::Gtk.Box msbuildOptionsSection;
		private global::Gtk.Label label51;

		protected virtual void Build ()
		{
			this.vbox40 = DesignerLayout.VBox (12);
			this.vbox47 = DesignerLayout.VBox (6);
			this.informationHeaderLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Project Information</b>"), true);
			this.informationHeaderLabel.UseUnderline = true;
			DesignerLayout.Pack (this.vbox47, this.informationHeaderLabel);
			this.hbox29 = DesignerLayout.HBox (0);
			this.label55 = new global::Gtk.Label ((string)null);
			this.label55.WidthRequest = 18;
			DesignerLayout.Pack (this.hbox29, this.label55);
			this.vbox46 = DesignerLayout.VBox (6);
			this.table11 = DesignerLayout.Grid (6, 6);
			this.nameLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Name:"));
			this.nameLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table11, this.nameLabel, 0, 0);
			this.projectNameEntry = new global::Gtk.Entry ();
			this.projectNameEntry.Editable = true;
			DesignerLayout.Attach (this.table11, this.projectNameEntry, 1, 0, 1, 1, true);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Version:"));
			DesignerLayout.Attach (this.table11, this.label1, 0, 1);
			this.hbox1 = DesignerLayout.HBox (6);
			this.entryVersion = new global::Gtk.Entry ();
			this.entryVersion.Editable = true;
			DesignerLayout.Pack (this.hbox1, this.entryVersion, true);
			this.checkSolutionVersion = new global::Gtk.CheckButton ();
			this.checkSolutionVersion.Label = global::Mono.Unix.Catalog.GetString ("Get version from parent solution");
			this.checkSolutionVersion.UseUnderline = true;
			DesignerLayout.Pack (this.hbox1, this.checkSolutionVersion);
			DesignerLayout.Attach (this.table11, this.hbox1, 1, 1);
			this.descriptionLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Description:"));
			this.descriptionLabel.UseUnderline = true;
			this.descriptionLabel.Yalign = 0F;
			this.descriptionLabel.Valign = global::Gtk.Align.Start;
			DesignerLayout.Attach (this.table11, this.descriptionLabel, 0, 2);
			this.projectDescriptionTextView = new global::Gtk.TextView ();
			this.scrolledwindow5 = DesignerLayout.Scrolled (this.projectDescriptionTextView);
			this.scrolledwindow5.WidthRequest = 350;
			this.scrolledwindow5.HeightRequest = 100;
			DesignerLayout.Attach (this.table11, this.scrolledwindow5, 1, 2, 1, 1, true, true);
			this.defaultNamespaceLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Default Namespace:"));
			DesignerLayout.Attach (this.table11, this.defaultNamespaceLabel, 0, 3);
			this.projectDefaultNamespaceEntry = new global::Gtk.Entry ();
			this.projectDefaultNamespaceEntry.Editable = true;
			DesignerLayout.Attach (this.table11, this.projectDefaultNamespaceEntry, 1, 3);
			DesignerLayout.Pack (this.vbox46, this.table11, true);
			DesignerLayout.Pack (this.hbox29, this.vbox46, true);
			DesignerLayout.Pack (this.vbox47, this.hbox29, true);
			DesignerLayout.Pack (this.vbox40, this.vbox47, true);
			this.msbuildOptionsSection = DesignerLayout.HBox (0);
			this.label51 = new global::Gtk.Label ((string)null);
			this.label51.WidthRequest = 18;
			DesignerLayout.Pack (this.msbuildOptionsSection, this.label51);
			DesignerLayout.Pack (this.vbox40, this.msbuildOptionsSection);
			DesignerLayout.SetContent (this, this.vbox40);
			this.descriptionLabel.MnemonicWidget = this.scrolledwindow5;
			this.nameLabel.MnemonicWidget = this.projectNameEntry;
			this.checkSolutionVersion.Toggled += new global::System.EventHandler (this.OnCheckSolutionVersionClicked);
		}
	}
}
