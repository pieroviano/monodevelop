// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class AuthorInformationPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.CheckButton checkCustom;
		private global::Gtk.Grid infoTable;
		private global::Gtk.Entry companyEntry;
		private global::Gtk.Entry copyrightEntry;
		private global::Gtk.Entry emailEntry;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Entry nameEntry;
		private global::Gtk.Entry trademarkEntry;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.checkCustom = new global::Gtk.CheckButton ();
			this.checkCustom.Label = global::Mono.Unix.Catalog.GetString ("_Use custom author information for this solution");
			this.checkCustom.UseUnderline = true;
			DesignerLayout.Pack (this.vbox1, this.checkCustom);
			this.infoTable = DesignerLayout.Grid (6, 6);
			this.infoTable.MarginStart = 24;
			this.nameEntry = new global::Gtk.Entry ();
			this.nameEntry.Editable = true;
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Name:"), false, this.nameEntry);
			DesignerLayout.Attach (this.infoTable, this.label2, 0, 0);
			DesignerLayout.Attach (this.infoTable, this.nameEntry, 1, 0, hexpand: true);
			this.emailEntry = new global::Gtk.Entry ();
			this.emailEntry.Editable = true;
			this.label4 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Email:"), false, this.emailEntry);
			DesignerLayout.Attach (this.infoTable, this.label4, 0, 1);
			DesignerLayout.Attach (this.infoTable, this.emailEntry, 1, 1, hexpand: true);
			this.copyrightEntry = new global::Gtk.Entry ();
			this.copyrightEntry.Editable = true;
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Copyright:"), false, this.copyrightEntry);
			DesignerLayout.Attach (this.infoTable, this.label3, 0, 2);
			DesignerLayout.Attach (this.infoTable, this.copyrightEntry, 1, 2, hexpand: true);
			this.companyEntry = new global::Gtk.Entry ();
			this.companyEntry.Editable = true;
			this.label5 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("C_ompany:"), false, this.companyEntry);
			DesignerLayout.Attach (this.infoTable, this.label5, 0, 3);
			DesignerLayout.Attach (this.infoTable, this.companyEntry, 1, 3, hexpand: true);
			this.trademarkEntry = new global::Gtk.Entry ();
			this.trademarkEntry.Editable = true;
			this.label6 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Trademark:"), false, this.trademarkEntry);
			DesignerLayout.Attach (this.infoTable, this.label6, 0, 4);
			DesignerLayout.Attach (this.infoTable, this.trademarkEntry, 1, 4, hexpand: true);
			DesignerLayout.Pack (this.vbox1, this.infoTable);
			DesignerLayout.SetContent (this, this.vbox1);
			this.checkCustom.Toggled += new global::System.EventHandler (this.UseDefaultToggled);
		}
	}
}
