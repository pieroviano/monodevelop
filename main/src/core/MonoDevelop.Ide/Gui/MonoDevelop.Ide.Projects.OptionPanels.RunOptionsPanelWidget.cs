using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	public partial class RunOptionsPanelWidget
	{
		private global::Gtk.Box vbox67;
		private global::Gtk.Box vbox69;
		private global::Gtk.Grid table10;
		private global::Gtk.Label label100;
		private global::Gtk.Entry parametersEntry;
		private global::Gtk.CheckButton externalConsoleCheckButton;
		private global::Gtk.CheckButton pauseConsoleOutputCheckButton;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Label label1;
		private global::MonoDevelop.Ide.Gui.Components.EnvVarList envVarList;

		protected virtual void Build ()
		{
			this.vbox67 = DesignerLayout.VBox (6);
			this.vbox69 = DesignerLayout.VBox (6);
			this.table10 = DesignerLayout.Grid (0, 6);
			this.label100 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Paramet_ers:"));
			this.label100.UseUnderline = true;
			DesignerLayout.Attach (this.table10, this.label100, 0, 0);
			this.parametersEntry = new global::Gtk.Entry ();
			this.parametersEntry.Editable = true;
			DesignerLayout.Attach (this.table10, this.parametersEntry, 1, 0, 1, 1, true);
			DesignerLayout.Pack (this.vbox69, this.table10);
			this.externalConsoleCheckButton = new global::Gtk.CheckButton ();
			this.externalConsoleCheckButton.Label = global::Mono.Unix.Catalog.GetString ("Run on e_xternal console");
			this.externalConsoleCheckButton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox69, this.externalConsoleCheckButton);
			this.pauseConsoleOutputCheckButton = new global::Gtk.CheckButton ();
			this.pauseConsoleOutputCheckButton.Label = global::Mono.Unix.Catalog.GetString ("Pause _console output");
			this.pauseConsoleOutputCheckButton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox69, this.pauseConsoleOutputCheckButton);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox69, this.hseparator1);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Environment Variables:"));
			DesignerLayout.Pack (this.vbox69, this.label1);
			this.envVarList = new global::MonoDevelop.Ide.Gui.Components.EnvVarList ();
			DesignerLayout.Pack (this.vbox69, this.envVarList, true);
			DesignerLayout.Pack (this.vbox67, this.vbox69, true);
			DesignerLayout.SetContent (this, this.vbox67);
			this.label100.MnemonicWidget = this.parametersEntry;
		}
	}
}
