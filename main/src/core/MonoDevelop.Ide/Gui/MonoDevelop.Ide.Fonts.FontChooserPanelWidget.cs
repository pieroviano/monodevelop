using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Fonts
{
	public partial class FontChooserPanelWidget
	{
		private global::Gtk.Box mainBox;

		protected virtual void Build ()
		{
			this.mainBox = DesignerLayout.VBox (6);
			DesignerLayout.SetContent (this, this.mainBox);
		}
	}
}
