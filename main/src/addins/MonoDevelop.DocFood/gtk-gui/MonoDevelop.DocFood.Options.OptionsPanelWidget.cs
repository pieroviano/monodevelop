
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.DocFood.Options
{
	internal partial class OptionsPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.CheckButton checkbutton1;
		private global::Gtk.Frame frame1;
		private global::Gtk.Box GtkAlignment;
		private global::Gtk.Box vbox3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeview1;
		private global::Gtk.Box hbox1;
		private global::Gtk.Button button1;
		private global::Gtk.Button button2;
		private global::Gtk.Label GtkLabel2;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.checkbutton1 = new global::Gtk.CheckButton ();
			this.checkbutton1.Label = global::Mono.Unix.Catalog.GetString ("Enable automatic documentation");
			this.checkbutton1.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.checkbutton1);
			this.frame1 = new global::Gtk.Frame ();
			this.GtkLabel2 = new global::Gtk.Label ((string)null);
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Macros</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel2;
			this.GtkAlignment = DesignerLayout.VBox (0);
			this.GtkAlignment.MarginStart = 12;
			this.vbox3 = DesignerLayout.VBox (6);
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.treeview1 = new global::Gtk.TreeView ();
			this.GtkScrolledWindow.Child = this.treeview1;
			DesignerLayout.Pack (this.vbox3, this.GtkScrolledWindow, true);
			this.hbox1 = DesignerLayout.HBox (6);
			this.button1 = new global::Gtk.Button ();
			this.button1.UseUnderline = true;
			this.button1.Label = "_Add";
			DesignerLayout.Pack (this.hbox1, this.button1);
			this.button2 = new global::Gtk.Button ();
			this.button2.UseUnderline = true;
			this.button2.Label = "_Remove";
			DesignerLayout.Pack (this.hbox1, this.button2);
			DesignerLayout.Pack (this.vbox3, this.hbox1);
			DesignerLayout.Pack (this.GtkAlignment, this.vbox3, true);
			this.vbox3.Hexpand = true;
			this.frame1.Child = this.GtkAlignment;
			DesignerLayout.Pack (this.vbox1, this.frame1, true);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
