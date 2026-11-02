// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class BuildMessagePanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Grid table4;
		private global::Gtk.ComboBoxText comboboxErrorPadAfter;
		private global::Gtk.ComboBoxText comboboxJumpToFirst;
		private global::Gtk.ComboBoxText comboboxMessageBubbles;
		private global::Gtk.Label label3;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Box hbox4;
		private global::Gtk.Label label10;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.table4 = DesignerLayout.Grid (6, 6);
			this.comboboxJumpToFirst = new global::Gtk.ComboBoxText ();
			this.comboboxJumpToFirst.AppendText (global::Mono.Unix.Catalog.GetString ("Never"));
			this.comboboxJumpToFirst.Active = 0;
			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Jump to first error or warning:"));
			DesignerLayout.Attach (this.table4, this.label6, 0, 0);
			DesignerLayout.Attach (this.table4, this.comboboxJumpToFirst, 1, 0);
			this.comboboxErrorPadAfter = new global::Gtk.ComboBoxText ();
			this.comboboxErrorPadAfter.AppendText (global::Mono.Unix.Catalog.GetString ("Never"));
			this.comboboxErrorPadAfter.Active = 0;
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Show error pad:"));
			DesignerLayout.Attach (this.table4, this.label3, 0, 1);
			DesignerLayout.Attach (this.table4, this.comboboxErrorPadAfter, 1, 1);
			this.comboboxMessageBubbles = new global::Gtk.ComboBoxText ();
			this.comboboxMessageBubbles.AppendText (global::Mono.Unix.Catalog.GetString ("Never"));
			this.comboboxMessageBubbles.Active = 0;
			this.label5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Show message bubbles:"));
			DesignerLayout.Attach (this.table4, this.label5, 0, 2);
			DesignerLayout.Attach (this.table4, this.comboboxMessageBubbles, 1, 2);
			DesignerLayout.Pack (this.vbox1, this.table4);
			this.hbox4 = DesignerLayout.HBox (6);
			this.label10 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("    "));
			DesignerLayout.Pack (this.hbox4, this.label10);
			DesignerLayout.Pack (this.vbox1, this.hbox4);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
