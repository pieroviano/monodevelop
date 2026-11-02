
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	public partial class FileCopyConfigurationSelector
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBoxText comboHandlers;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Box editorBox;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Target:");
			DesignerLayout.Pack (this.hbox1, this.label1);
			this.comboHandlers = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboHandlers, true);
			DesignerLayout.Pack (this.vbox1, this.hbox1);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox1, this.hseparator1);
			this.editorBox = DesignerLayout.VBox (0);
			DesignerLayout.Pack (this.vbox1, this.editorBox, true);
			DesignerLayout.SetContent (this, this.vbox1);
			this.comboHandlers.Changed += new global::System.EventHandler (this.OnComboHandlersChanged);
		}
	}
}
