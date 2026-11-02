// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class AddInsPanelWidget
	{
		private global::Gtk.Box vbox72;
		private global::Gtk.Label label2;
		private global::Gtk.Box vbox5;
		private global::Gtk.CheckButton radioHour;
		private global::Gtk.CheckButton radioDay;
		private global::Gtk.CheckButton radioMonth;
		private global::Gtk.CheckButton radioNever;
		private global::Gtk.CheckButton checkUnstable;
		private global::Gtk.Box hbox47;
		private global::Gtk.Box boxUnstable;
		private global::Gtk.Box vbox6;
		private global::Gtk.CheckButton radioBeta;
		private global::Gtk.CheckButton radioAlpha;
		private global::Gtk.CheckButton radioTest;
		private global::Gtk.Box hbox3;
		private global::Gtk.Button buttonUpdateNow;

		protected virtual void Build ()
		{
			this.vbox72 = DesignerLayout.VBox (6);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Automatically check for updates:"));
			DesignerLayout.Pack (this.vbox72, this.label2);
			this.vbox5 = DesignerLayout.VBox (6);
			this.vbox5.MarginStart = 24;
			this.vbox5.MarginBottom = 6;
			this.radioHour = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Every hour"));
			this.radioHour.Active = true;
			DesignerLayout.Pack (this.vbox5, this.radioHour);
			this.radioDay = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Every day"), this.radioHour);
			DesignerLayout.Pack (this.vbox5, this.radioDay);
			this.radioMonth = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Every month"), this.radioHour);
			DesignerLayout.Pack (this.vbox5, this.radioMonth);
			this.radioNever = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Never"), this.radioHour);
			DesignerLayout.Pack (this.vbox5, this.radioNever);
			DesignerLayout.Pack (this.vbox72, this.vbox5);
			this.checkUnstable = new global::Gtk.CheckButton ();
			this.checkUnstable.Label = global::Mono.Unix.Catalog.GetString ("Install unstable developer updates");
			this.checkUnstable.Active = true;
			this.checkUnstable.UseUnderline = true;
			DesignerLayout.Pack (this.vbox72, this.checkUnstable);
			this.hbox47 = DesignerLayout.HBox (0);
			this.boxUnstable = DesignerLayout.VBox (0);
			this.boxUnstable.MarginStart = 24;
			this.vbox6 = DesignerLayout.VBox (6);
			this.radioBeta = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Beta updates (weekly)"));
			this.radioBeta.Active = true;
			DesignerLayout.Pack (this.vbox6, this.radioBeta);
			this.radioAlpha = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Alpha updates (very often, very unstable)"), this.radioBeta);
			DesignerLayout.Pack (this.vbox6, this.radioAlpha);
			this.radioTest = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("Test"), this.radioBeta);
			DesignerLayout.Pack (this.vbox6, this.radioTest);
			DesignerLayout.Pack (this.boxUnstable, this.vbox6, true);
			DesignerLayout.Pack (this.hbox47, this.boxUnstable, true);
			DesignerLayout.Pack (this.vbox72, this.hbox47);
			this.hbox3 = DesignerLayout.Margin (DesignerLayout.HBox (6), 12);
			this.buttonUpdateNow = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Check for Updates Now"));
			DesignerLayout.Pack (this.hbox3, this.buttonUpdateNow);
			DesignerLayout.Pack (this.vbox72, this.hbox3);
			DesignerLayout.SetContent (this, this.vbox72);
			this.radioTest.Visible = false;
			this.checkUnstable.Toggled += new global::System.EventHandler (this.OnCheckUnstableToggled);
			this.buttonUpdateNow.Clicked += new global::System.EventHandler (this.OnButtonUpdateNowClicked);
		}
	}
}
