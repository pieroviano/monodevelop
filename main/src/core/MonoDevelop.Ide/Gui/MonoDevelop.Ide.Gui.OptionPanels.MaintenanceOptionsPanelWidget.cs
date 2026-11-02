// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class MaintenanceOptionsPanelWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.CheckButton checkInstr;
		private global::Gtk.CheckButton checkAutoTest;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			this.checkInstr = new global::Gtk.CheckButton ();
			this.checkInstr.Label = global::Mono.Unix.Catalog.GetString ("Enable MonoDevelop Instrumentation");
			this.checkInstr.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.checkInstr);
			this.checkAutoTest = new global::Gtk.CheckButton ();
			this.checkAutoTest.Label = global::Mono.Unix.Catalog.GetString ("Enable automated test support");
			this.checkAutoTest.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.checkAutoTest);
			DesignerLayout.SetContent (this, this.vbox2);
		}
	}
}
