using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class CombineInformationWidget
	{
		private global::Gtk.Box vbox86;
		private global::Gtk.Grid table14;
		private global::Gtk.Label descLabel;
		private global::Gtk.ScrolledWindow scrolledwindow14;
		private global::Gtk.TextView descView;
		private global::Gtk.Entry versEntry;
		private global::Gtk.Label versLabel;

		protected virtual void Build ()
		{
			this.vbox86 = DesignerLayout.VBox (0);
			this.table14 = DesignerLayout.Grid (6, 6);
			this.versLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Version:"));
			this.versLabel.UseUnderline = true;
			DesignerLayout.Attach (this.table14, this.versLabel, 0, 0);
			this.versEntry = new global::Gtk.Entry ();
			this.versEntry.Editable = true;
			DesignerLayout.Attach (this.table14, this.versEntry, 1, 0, 1, 1, true);
			this.descLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Description:"));
			this.descLabel.UseUnderline = true;
			this.descLabel.Yalign = 0F;
			this.descLabel.Valign = global::Gtk.Align.Start;
			DesignerLayout.Attach (this.table14, this.descLabel, 0, 1);
			this.descView = new global::Gtk.TextView ();
			this.scrolledwindow14 = DesignerLayout.Scrolled (this.descView);
			this.scrolledwindow14.WidthRequest = 350;
			this.scrolledwindow14.HeightRequest = 100;
			DesignerLayout.Attach (this.table14, this.scrolledwindow14, 1, 1, 1, 1, true, true);
			DesignerLayout.Pack (this.vbox86, this.table14, true);
			DesignerLayout.SetContent (this, this.vbox86);
			this.descLabel.MnemonicWidget = this.scrolledwindow14;
			this.versLabel.MnemonicWidget = this.versEntry;
		}
	}
}
