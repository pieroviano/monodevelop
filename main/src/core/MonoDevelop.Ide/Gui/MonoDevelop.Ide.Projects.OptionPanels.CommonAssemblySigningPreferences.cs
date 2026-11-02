using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CommonAssemblySigningPreferences
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.CheckButton signAssemblyCheckbutton;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label strongNameFileLabel;
		private global::MonoDevelop.Components.FileEntry strongNameFileEntry;
		private global::Gtk.CheckButton delaySignCheckbutton;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.signAssemblyCheckbutton = new global::Gtk.CheckButton ();
			this.signAssemblyCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Sign this assembly");
			this.signAssemblyCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.signAssemblyCheckbutton);
			this.hbox1 = DesignerLayout.HBox (6);
			this.strongNameFileLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("S_trong Name File:"));
			this.strongNameFileLabel.UseUnderline = true;
			DesignerLayout.Pack (this.hbox1, this.strongNameFileLabel);
			this.strongNameFileEntry = new global::MonoDevelop.Components.FileEntry ();
			DesignerLayout.Pack (this.hbox1, this.strongNameFileEntry, true);
			DesignerLayout.Pack (this.vbox1, this.hbox1);
			this.delaySignCheckbutton = new global::Gtk.CheckButton ();
			this.delaySignCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Delay sign assembly");
			this.delaySignCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.delaySignCheckbutton);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
