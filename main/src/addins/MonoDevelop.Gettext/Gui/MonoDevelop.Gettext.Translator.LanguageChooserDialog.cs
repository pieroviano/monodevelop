
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Gettext.Translator
{
	internal partial class LanguageChooserDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box vbox3;
		private global::Gtk.Frame frame1;
		private global::Gtk.Box GtkAlignment2;
		private global::Gtk.Box tableKnown;
		private global::Gtk.Box vbox4;
		private global::Gtk.Label label3;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView languageTreeView;
		private global::Gtk.Box vbox5;
		private global::Gtk.CheckButton checkbuttonUseCoutry;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TreeView countryTreeView;
		private global::Gtk.CheckButton radiobuttonKnown;
		private global::Gtk.Frame frame2;
		private global::Gtk.Box GtkAlignment3;
		private global::Gtk.Box hboxUser;
		private global::Gtk.Label label2;
		private global::Gtk.Entry entryLocale;
		private global::Gtk.CheckButton radiobuttonCustom;
		private global::Gtk.Button button1;
		private global::Gtk.Button buttonOK;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Create New Localization");
			this.Modal = true;
			this.Resizable = false;
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.VBox (16);
			DesignerLayout.Margin (this.vbox2, 8);
			this.vbox3 = DesignerLayout.VBox (8);
			this.frame1 = new global::Gtk.Frame ();
			this.radiobuttonKnown = new global::Gtk.CheckButton ();
			this.radiobuttonKnown.Label = global::Mono.Unix.Catalog.GetString ("_Known Language");
			this.radiobuttonKnown.UseUnderline = true;
			this.frame1.LabelWidget = this.radiobuttonKnown;
			this.GtkAlignment2 = DesignerLayout.VBox (0);
			this.GtkAlignment2.MarginStart = 12;
			this.tableKnown = DesignerLayout.HBox (6);
			this.vbox4 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox4, 6);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.WidthRequest = 170;
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("_Language:");
			this.label3.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.label3);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.HeightRequest = 200;
			this.languageTreeView = new global::Gtk.TreeView ();
			this.languageTreeView.HeadersVisible = false;
			this.scrolledwindow1.Child = this.languageTreeView;
			DesignerLayout.Pack (this.vbox4, this.scrolledwindow1, true);
			DesignerLayout.Pack (this.tableKnown, this.vbox4);
			this.vbox5 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox5, 6);
			this.checkbuttonUseCoutry = new global::Gtk.CheckButton ();
			this.checkbuttonUseCoutry.WidthRequest = 220;
			this.checkbuttonUseCoutry.Label = global::Mono.Unix.Catalog.GetString ("U_se Country Code");
			this.checkbuttonUseCoutry.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.checkbuttonUseCoutry);
			this.scrolledwindow2 = new global::Gtk.ScrolledWindow ();
			this.countryTreeView = new global::Gtk.TreeView ();
			this.countryTreeView.HeadersVisible = false;
			this.scrolledwindow2.Child = this.countryTreeView;
			DesignerLayout.Pack (this.vbox5, this.scrolledwindow2, true);
			DesignerLayout.Pack (this.tableKnown, this.vbox5, true);
			DesignerLayout.Pack (this.GtkAlignment2, this.tableKnown, true);
			this.tableKnown.Hexpand = true;
			this.frame1.Child = this.GtkAlignment2;
			DesignerLayout.Pack (this.vbox3, this.frame1, true);
			this.frame2 = new global::Gtk.Frame ();
			this.radiobuttonCustom = new global::Gtk.CheckButton ();
			this.radiobuttonCustom.Label = global::Mono.Unix.Catalog.GetString ("_User Defined Locale");
			this.radiobuttonCustom.Group = this.radiobuttonKnown;
			this.radiobuttonCustom.UseUnderline = true;
			this.frame2.LabelWidget = this.radiobuttonCustom;
			this.GtkAlignment3 = DesignerLayout.VBox (0);
			this.GtkAlignment3.MarginStart = 12;
			this.hboxUser = DesignerLayout.HBox (6);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("L_ocale:");
			this.label2.UseUnderline = true;
			DesignerLayout.Pack (this.hboxUser, this.label2);
			this.entryLocale = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hboxUser, this.entryLocale, true);
			DesignerLayout.Pack (this.GtkAlignment3, this.hboxUser, true);
			this.hboxUser.Hexpand = true;
			this.frame2.Child = this.GtkAlignment3;
			DesignerLayout.Pack (this.vbox3, this.frame2);
			DesignerLayout.Pack (this.vbox2, this.vbox3, true);
			DesignerLayout.Pack (content, this.vbox2);
			this.button1 = new global::Gtk.Button ();
			this.button1.UseUnderline = true;
			this.button1.Label = "_Cancel";
			this.buttonOK = new global::Gtk.Button ();
			this.buttonOK.UseUnderline = true;
			this.buttonOK.Label = "_Add";
			DesignerLayout.AddActionButton (this, this.button1, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOK, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (466, 415);
			this.label3.MnemonicWidget = this.languageTreeView;
			this.label2.MnemonicWidget = this.entryLocale;
			this.checkbuttonUseCoutry.Toggled += new global::System.EventHandler (this.ChangeSensitivity);
			this.entryLocale.Changed += new global::System.EventHandler (this.OnEntryLocaleChanged);

		}
	}
}
