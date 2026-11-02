using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class RuntimeOptionsPanelWidget
	{
		private global::Gtk.Box vbox81;
		private global::Gtk.Box hbox68;
		private global::Gtk.Label label114;
		private global::Gtk.ComboBoxText runtimeVersionCombo;

		protected virtual void Build ()
		{
			this.vbox81 = DesignerLayout.VBox (12);
			this.hbox68 = DesignerLayout.HBox (7);
			this.label114 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Target _framework:"));
			this.label114.UseUnderline = true;
			DesignerLayout.Pack (this.hbox68, this.label114);
			this.runtimeVersionCombo = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox68, this.runtimeVersionCombo);
			DesignerLayout.Pack (this.vbox81, this.hbox68);
			DesignerLayout.SetContent (this, this.vbox81);
		}
	}
}
