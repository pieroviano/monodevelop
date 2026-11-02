// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal partial class TipOfTheDayWindow
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box vbox3;
		private global::Gtk.Box hbox7;
		private global::MonoDevelop.Components.ImageView iconInfo;
		private global::Gtk.Label categoryLabel;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TextView tipTextview;
		private global::Gtk.Box hbox5;
		private global::Gtk.CheckButton noshowCheckbutton;
		private global::Gtk.Box hbuttonbox1;
		private global::Gtk.Button nextButton;
		private global::Gtk.Button closeButton;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Tip of the Day");
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.vbox3 = DesignerLayout.Margin (DesignerLayout.VBox (12), 6);
			this.hbox7 = DesignerLayout.HBox (5);
			this.iconInfo = new global::MonoDevelop.Components.ImageView ();
			this.iconInfo.Xalign = 0F;
			this.iconInfo.Yalign = 0F;
			this.iconInfo.IconId = "gtk-dialog-info";
			this.iconInfo.IconSize = global::MonoDevelop.Components.StockIconSize.Dialog;
			DesignerLayout.Pack (this.hbox7, this.iconInfo);
			this.categoryLabel = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Did you know...?"), true);
			this.categoryLabel.Wrap = true;
			DesignerLayout.Pack (this.hbox7, this.categoryLabel);
			DesignerLayout.Pack (this.vbox3, this.hbox7);
			this.tipTextview = new global::Gtk.TextView ();
			this.tipTextview.Buffer.Text = "Did you know that you can design lots of cool things with glade?";
			this.tipTextview.Editable = false;
			this.tipTextview.CursorVisible = false;
			this.tipTextview.WrapMode = global::Gtk.WrapMode.Word;
			this.scrolledwindow2 = DesignerLayout.Scrolled (this.tipTextview);
			DesignerLayout.Pack (this.vbox3, this.scrolledwindow2, true);
			DesignerLayout.Pack (this.vbox2, this.vbox3, true);
			this.hbox5 = DesignerLayout.Margin (DesignerLayout.HBox (12), 6);
			this.noshowCheckbutton = new global::Gtk.CheckButton ();
			this.noshowCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Show at startup");
			this.noshowCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.hbox5, this.noshowCheckbutton);
			this.hbuttonbox1 = DesignerLayout.HBox (10);
			this.hbuttonbox1.Halign = global::Gtk.Align.End;
			this.hbuttonbox1.Hexpand = true;
			this.nextButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Next Tip"));
			DesignerLayout.Pack (this.hbuttonbox1, this.nextButton);
			this.closeButton = DesignerLayout.Button ("_Close");
			DesignerLayout.Pack (this.hbuttonbox1, this.closeButton);
			DesignerLayout.Pack (this.hbox5, this.hbuttonbox1, true);
			DesignerLayout.Pack (this.vbox2, this.hbox5);
			DesignerLayout.SetContent (this, this.vbox2);
			this.DefaultWidget = this.closeButton;
			this.SetDefaultSize (500, 285);
		}
	}
}
