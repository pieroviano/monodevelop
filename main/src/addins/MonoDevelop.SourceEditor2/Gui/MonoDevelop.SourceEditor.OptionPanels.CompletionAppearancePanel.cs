// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	partial class CompletionAppearancePanel
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.CheckButton filterByBrowsableCheckbutton;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.filterByBrowsableCheckbutton = new global::Gtk.CheckButton ();
			this.filterByBrowsableCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Show EditorBrowsable.Advanced members");
			this.filterByBrowsableCheckbutton.Active = true;
			this.filterByBrowsableCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.filterByBrowsableCheckbutton);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
