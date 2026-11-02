
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.ChangeLogAddIn
{
	internal partial class AddLogEntryDialog
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Paned vpaned1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TreeView fileList;
		private global::Gtk.Box vbox3;
		private global::Gtk.Label label3;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TextView textview;
		private global::Gtk.Box boxNewFile;
		private global::MonoDevelop.Components.ImageView image36;
		private global::Gtk.Label label7;
		private global::Gtk.Box boxNoFile;
		private global::MonoDevelop.Components.ImageView image37;
		private global::Gtk.Label label8;
		private global::Gtk.Box hbox3;
		private global::Gtk.Box vbox4;
		private global::MonoDevelop.Components.ImageView image38;
		private global::Gtk.Label label9;
		private global::Gtk.Button button7;
		private global::Gtk.Button button119;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("ChangeLog");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox1 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox1, 6);
			this.vpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Vertical);
			this.vpaned1.Position = 116;
			this.vbox2 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Modified ChangeLog files:");
			DesignerLayout.Pack (this.vbox2, this.label1);
			this.scrolledwindow2 = new global::Gtk.ScrolledWindow ();
			this.fileList = new global::Gtk.TreeView ();
			this.fileList.HeadersVisible = false;
			this.scrolledwindow2.Child = this.fileList;
			DesignerLayout.Pack (this.vbox2, this.scrolledwindow2, true);
			this.vpaned1.StartChild = this.vbox2;
			this.vbox3 = DesignerLayout.VBox (6);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("ChangeLog entry:");
			DesignerLayout.Pack (this.vbox3, this.label3);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.textview = new global::Gtk.TextView ();
			this.scrolledwindow1.Child = this.textview;
			DesignerLayout.Pack (this.vbox3, this.scrolledwindow1, true);
			this.vpaned1.EndChild = this.vbox3;
			DesignerLayout.Pack (this.vbox1, this.vpaned1, true);
			this.boxNewFile = DesignerLayout.HBox (6);
			this.boxNewFile.Visible = false;
			this.image36 = new global::MonoDevelop.Components.ImageView ();
			this.image36.IconId = "gtk-new";
			this.image36.IconSize = global::MonoDevelop.Components.StockIconSize.SmallToolbar;
			DesignerLayout.Pack (this.boxNewFile, this.image36);
			this.label7 = new global::Gtk.Label ((string)null);
			this.label7.WidthRequest = 500;
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("This ChangeLog file does not exist and will be created.");
			this.label7.Wrap = true;
			DesignerLayout.Pack (this.boxNewFile, this.label7);
			DesignerLayout.Pack (this.vbox1, this.boxNewFile);
			this.boxNoFile = DesignerLayout.HBox (6);
			this.boxNoFile.Visible = false;
			this.image37 = new global::MonoDevelop.Components.ImageView ();
			this.image37.IconId = "gtk-dialog-warning";
			this.image37.IconSize = global::MonoDevelop.Components.StockIconSize.SmallToolbar;
			DesignerLayout.Pack (this.boxNoFile, this.image37);
			this.label8 = new global::Gtk.Label ((string)null);
			this.label8.WidthRequest = 500;
			this.label8.Xalign = 0F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("This ChangeLog file does not exist and will <b>not</b> be created.");
			this.label8.UseMarkup = true;
			this.label8.Wrap = true;
			DesignerLayout.Pack (this.boxNoFile, this.label8);
			DesignerLayout.Pack (this.vbox1, this.boxNoFile);
			this.hbox3 = DesignerLayout.HBox (6);
			this.vbox4 = DesignerLayout.VBox (6);
			this.image38 = new global::MonoDevelop.Components.ImageView ();
			this.image38.IconId = "gtk-dialog-info";
			this.image38.IconSize = global::MonoDevelop.Components.StockIconSize.SmallToolbar;
			DesignerLayout.Pack (this.vbox4, this.image38);
			DesignerLayout.Pack (this.hbox3, this.vbox4);
			this.label9 = new global::Gtk.Label ((string)null);
			this.label9.WidthRequest = 500;
			this.label9.Xalign = 0F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("To change the ChangeLog creation and update policies, open the options dialog of the project or solution and click on the 'ChangeLog Integration\" section.");
			this.label9.UseMarkup = true;
			this.label9.Wrap = true;
			DesignerLayout.Pack (this.hbox3, this.label9);
			DesignerLayout.Pack (this.vbox1, this.hbox3);
			DesignerLayout.Pack (content, this.vbox1, true);
			this.button7 = new global::Gtk.Button ();
			this.button7.UseUnderline = true;
			this.button7.Label = "_Cancel";
			this.button119 = new global::Gtk.Button ();
			this.button119.UseUnderline = true;
			this.button119.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.button7, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.button119, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (573, 510);
		}
	}
}
