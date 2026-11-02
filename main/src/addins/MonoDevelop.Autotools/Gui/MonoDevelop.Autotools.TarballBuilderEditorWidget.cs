
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Autotools
{
	public partial class TarballBuilderEditorWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label2;
		private global::MonoDevelop.Components.FolderEntry folderEntry;
		private global::Gtk.CheckButton radioUseExisting;
		private global::Gtk.CheckButton radioGenerate;
		private global::Gtk.Box boxGenerate;
		private global::Gtk.Box boxConfig;
		private global::Gtk.Label label4;
		private global::Gtk.Label label3;
		private global::Gtk.ComboBoxText comboConfigs;
		private global::Gtk.Box boxConfig1;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Box hbox3;
		private global::Gtk.Label label8;
		private global::Gtk.CheckButton rbAutotools;
		private global::Gtk.Label label9;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label1;
		private global::Gtk.CheckButton rbSimple;
		private global::Gtk.Label label7;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box hbox4;
		private global::Gtk.Button autofooProperties;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Deploy directory:");
			DesignerLayout.Pack (this.hbox1, this.label2);
			this.folderEntry = new global::MonoDevelop.Components.FolderEntry ();
			this.folderEntry.BrowserTitle = "Target Directory";
			DesignerLayout.Pack (this.hbox1, this.folderEntry, true);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			this.radioUseExisting = new global::Gtk.CheckButton ();
			this.radioUseExisting.Label = global::Mono.Unix.Catalog.GetString ("Use existing Makefiles");
			this.radioUseExisting.Active = true;
			this.radioUseExisting.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.radioUseExisting);
			this.radioGenerate = new global::Gtk.CheckButton ();
			this.radioGenerate.Label = global::Mono.Unix.Catalog.GetString ("Generate new Makefiles");
			this.radioGenerate.Group = this.radioUseExisting;
			this.radioGenerate.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.radioGenerate);
			this.boxGenerate = DesignerLayout.VBox (6);
			this.boxConfig = DesignerLayout.HBox (6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.WidthRequest = 24;
			DesignerLayout.Pack (this.boxConfig, this.label4);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Default configuration:");
			DesignerLayout.Pack (this.boxConfig, this.label3);
			this.comboConfigs = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.boxConfig, this.comboConfigs, true);
			DesignerLayout.Pack (this.boxGenerate, this.boxConfig);
			this.boxConfig1 = DesignerLayout.HBox (6);
			this.label5 = new global::Gtk.Label ((string)null);
			this.label5.WidthRequest = 24;
			DesignerLayout.Pack (this.boxConfig1, this.label5);
			this.label6 = new global::Gtk.Label ((string)null);
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the kind of Makefile to generate:");
			DesignerLayout.Pack (this.boxConfig1, this.label6);
			DesignerLayout.Pack (this.boxGenerate, this.boxConfig1);
			this.hbox3 = DesignerLayout.HBox (6);
			this.label8 = new global::Gtk.Label ((string)null);
			this.label8.WidthRequest = 24;
			DesignerLayout.Pack (this.hbox3, this.label8);
			this.rbAutotools = new global::Gtk.CheckButton ();
			this.rbAutotools.Active = true;
			this.rbAutotools.UseUnderline = true;
			this.label9 = new global::Gtk.Label ((string)null);
			this.label9.WidthRequest = 550;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Autotools based Makefile</b>\nGenerates a Makefile structure based on Autotools, with the standard targets and configuration scripts.");
			this.label9.UseMarkup = true;
			this.label9.Wrap = true;
			this.rbAutotools.Child = this.label9;
			DesignerLayout.Pack (this.hbox3, this.rbAutotools, true);
			DesignerLayout.Pack (this.boxGenerate, this.hbox3);
			this.hbox2 = DesignerLayout.HBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.WidthRequest = 24;
			DesignerLayout.Pack (this.hbox2, this.label1);
			this.rbSimple = new global::Gtk.CheckButton ();
			this.rbSimple.Group = this.rbAutotools;
			this.rbSimple.UseUnderline = true;
			this.label7 = new global::Gtk.Label ((string)null);
			this.label7.WidthRequest = 550;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Simple Makefile</b>\nGenerates set of Makefiles with the most common targets, and a configuration script that does a basic check of package dependencies.");
			this.label7.UseMarkup = true;
			this.label7.Wrap = true;
			this.rbSimple.Child = this.label7;
			DesignerLayout.Pack (this.hbox2, this.rbSimple, true);
			DesignerLayout.Pack (this.boxGenerate, this.hbox2);
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment1.MarginStart = 24;
			this.alignment1.MarginTop = 8;
			this.alignment1.MarginBottom = 8;
			this.hbox4 = DesignerLayout.HBox (6);
			this.autofooProperties = new global::Gtk.Button ();
			this.autofooProperties.UseUnderline = true;
			this.autofooProperties.Label = global::Mono.Unix.Catalog.GetString ("Edit configure switches");
			DesignerLayout.Pack (this.hbox4, this.autofooProperties);
			DesignerLayout.Pack (this.alignment1, this.hbox4, true);
			this.hbox4.Hexpand = true;
			DesignerLayout.Pack (this.boxGenerate, this.alignment1);
			DesignerLayout.Pack (this.vbox2, this.boxGenerate);
			DesignerLayout.SetContent (this, this.vbox2);
			this.folderEntry.PathChanged += new global::System.EventHandler (this.OnFolderEntryPathChanged);
			this.radioUseExisting.Toggled += new global::System.EventHandler (this.OnRadioUseExistingClicked);
			this.radioGenerate.Toggled += new global::System.EventHandler (this.OnRadioGenerateClicked);
			this.comboConfigs.Changed += new global::System.EventHandler (this.OnComboConfigsChanged);
			this.rbAutotools.Toggled += new global::System.EventHandler (this.OnRbAutotoolsToggled);
			this.rbSimple.Toggled += new global::System.EventHandler (this.OnRbSimpleToggled);
			this.autofooProperties.Clicked += new global::System.EventHandler (this.OnAutofooPropertiesClicked);
		}
	}
}
