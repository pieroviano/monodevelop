// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	internal partial class GeneralOptionsPanel
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label GtkLabel13;
		private global::Gtk.Box vbox4;
		private global::Gtk.Box hbox3;
		private global::Gtk.Label fixed2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBoxText comboboxLineEndings;
		private global::Gtk.Label GtkLabel14;
		private global::Gtk.Box vbox5;
		private global::Gtk.CheckButton foldingCheckbutton;
		private global::Gtk.CheckButton foldregionsCheckbutton;
		private global::Gtk.CheckButton foldCommentsCheckbutton;
		private global::Gtk.Label GtkLabel15;
		private global::Gtk.Box vbox6;
		private global::Gtk.CheckButton antiAliasingCheckbutton;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);

			this.GtkLabel13 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Coding</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel13);
			this.vbox4 = DesignerLayout.VBox (6);
			this.vbox4.MarginStart = 12;
			this.hbox3 = DesignerLayout.HBox (6);
			this.fixed2 = new global::Gtk.Label ();
			this.fixed2.MarginStart = 6;
			this.fixed2.MarginEnd = 6;
			DesignerLayout.Pack (this.hbox3, this.fixed2);
			DesignerLayout.Pack (this.vbox4, this.hbox3);
			this.hbox1 = DesignerLayout.HBox (6);
			this.comboboxLineEndings = new global::Gtk.ComboBoxText ();
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Line ending conversion:"), false, this.comboboxLineEndings);
			DesignerLayout.Pack (this.hbox1, this.label1);
			DesignerLayout.Pack (this.hbox1, this.comboboxLineEndings);
			DesignerLayout.Pack (this.vbox4, this.hbox1);
			DesignerLayout.Pack (this.vbox1, this.vbox4);

			this.GtkLabel14 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Code Folding</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel14);
			this.vbox5 = DesignerLayout.VBox (6);
			this.vbox5.MarginStart = 12;
			this.foldingCheckbutton = new global::Gtk.CheckButton ();
			this.foldingCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Enable code _folding");
			this.foldingCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.foldingCheckbutton);
			this.foldregionsCheckbutton = new global::Gtk.CheckButton ();
			this.foldregionsCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Fold #_regions by default");
			this.foldregionsCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.foldregionsCheckbutton);
			this.foldCommentsCheckbutton = new global::Gtk.CheckButton ();
			this.foldCommentsCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Fold _comments by default");
			this.foldCommentsCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.foldCommentsCheckbutton);
			DesignerLayout.Pack (this.vbox1, this.vbox5);

			this.GtkLabel15 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Appearance</b>"), true);
			DesignerLayout.Pack (this.vbox1, this.GtkLabel15);
			this.vbox6 = DesignerLayout.VBox (6);
			this.vbox6.MarginStart = 12;
			this.antiAliasingCheckbutton = new global::Gtk.CheckButton ();
			this.antiAliasingCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Use anti aliasing");
			this.antiAliasingCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox6, this.antiAliasingCheckbutton);
			DesignerLayout.Pack (this.vbox1, this.vbox6);

			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
