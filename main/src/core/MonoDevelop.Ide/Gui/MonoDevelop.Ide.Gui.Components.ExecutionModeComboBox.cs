using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Components
{
	public partial class ExecutionModeComboBox
	{
		private global::Gtk.ComboBoxText comboMode;

		protected virtual void Build ()
		{
			this.comboMode = new global::Gtk.ComboBoxText ();
			DesignerLayout.SetContent (this, this.comboMode);
			this.comboMode.Changed += new global::System.EventHandler (this.OnComboModeChanged);
		}
	}
}
