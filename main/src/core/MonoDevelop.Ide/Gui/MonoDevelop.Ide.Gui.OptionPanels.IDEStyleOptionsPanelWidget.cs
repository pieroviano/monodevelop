// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	public partial class IDEStyleOptionsPanelWidget
	{
		private global::Gtk.Box vbox13;
		private global::Gtk.Grid table1;
		private global::Gtk.ComboBoxText comboLanguage;
		private global::Gtk.ComboBoxText comboTheme;
		private global::Gtk.Label label2;
		private global::Gtk.Label labelTheme;
		private global::Gtk.Separator separatorRestart;
		private global::Gtk.Grid tableRestart;
		private global::Gtk.Button btnRestart;
		private global::MonoDevelop.Components.ImageView imageRestart;
		private global::Gtk.Label labelRestart;

		protected virtual void Build ()
		{
			this.vbox13 = DesignerLayout.VBox (6);
			this.table1 = DesignerLayout.Grid (6, 6);
			this.comboLanguage = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboLanguage, 1, 0);
			this.comboTheme = new global::Gtk.ComboBoxText ();
			DesignerLayout.Attach (this.table1, this.comboTheme, 1, 1);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("User Interface Language:"));
			DesignerLayout.Attach (this.table1, this.label2, 0, 0);
			this.labelTheme = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("User Interface Theme:"));
			DesignerLayout.Attach (this.table1, this.labelTheme, 0, 1);
			DesignerLayout.Pack (this.vbox13, this.table1);
			this.separatorRestart = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox13, this.separatorRestart);
			this.tableRestart = DesignerLayout.Grid (6, 6);
			this.btnRestart = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Restart {0}:"));
			DesignerLayout.Attach (this.tableRestart, this.btnRestart, 1, 1);
			this.imageRestart = new global::MonoDevelop.Components.ImageView ();
			this.imageRestart.IconId = "md-information";
			this.imageRestart.IconSize = global::MonoDevelop.Components.StockIconSize.Menu;
			DesignerLayout.Attach (this.tableRestart, this.imageRestart, 0, 0);
			this.labelRestart = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("These preferences will take effect next time you start {0}"));
			DesignerLayout.Attach (this.tableRestart, this.labelRestart, 1, 0, 2);
			DesignerLayout.Pack (this.vbox13, this.tableRestart);
			DesignerLayout.SetContent (this, this.vbox13);
			this.btnRestart.Clicked += new global::System.EventHandler (this.RestartClicked);
		}
	}
}
