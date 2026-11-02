using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide
{
	internal partial class SelectEncodingsDialog
	{
		private global::Gtk.Grid table5;
		private global::Gtk.Label label106;
		private global::Gtk.Label label107;
		private global::Gtk.ScrolledWindow scrolledwindow10;
		private global::Gtk.TreeView listAvail;
		private global::Gtk.ScrolledWindow scrolledwindow11;
		private global::Gtk.TreeView listSelected;
		private global::Gtk.Box vbox74;
		private global::Gtk.Label label108;
		private global::Gtk.Button btnAdd;
		private global::MonoDevelop.Components.ImageView imageAdd;
		private global::Gtk.Button btnRemove;
		private global::MonoDevelop.Components.ImageView imageRemove;
		private global::Gtk.Label label109;
		private global::Gtk.Box vbox75;
		private global::Gtk.Button btnUp;
		private global::MonoDevelop.Components.ImageView imageUp;
		private global::Gtk.Button btnDown;
		private global::MonoDevelop.Components.ImageView imageDown;
		private global::Gtk.Button cancelbutton1;
		private global::Gtk.Button okbutton1;

		private static global::Gtk.Button CreateIconButton (global::MonoDevelop.Components.ImageView image, string iconId)
		{
			image.IconSize = StockIconSize.Button;
			image.IconId = iconId;
			var button = new global::Gtk.Button ();
			button.Child = image;
			return button;
		}

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Select Text Encodings");
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 6);
			this.table5 = DesignerLayout.Margin (DesignerLayout.Grid (6, 12), 6);
			this.label106 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Available encodings:"));
			this.label106.WidthChars = 20;
			this.label106.Valign = global::Gtk.Align.Start;
			DesignerLayout.Attach (this.table5, this.label106, 0, 0);
			this.label107 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Encodings shown in menu:"));
			this.label107.WidthChars = 20;
			this.label107.Valign = global::Gtk.Align.Start;
			DesignerLayout.Attach (this.table5, this.label107, 2, 0);
			this.listAvail = new global::Gtk.TreeView ();
			this.scrolledwindow10 = DesignerLayout.Scrolled (this.listAvail);
			DesignerLayout.Attach (this.table5, this.scrolledwindow10, 0, 1, 1, 1, true, true);
			this.vbox74 = DesignerLayout.VBox (6);
			this.vbox74.Valign = global::Gtk.Align.Center;
			this.label108 = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.vbox74, this.label108);
			this.imageAdd = new global::MonoDevelop.Components.ImageView ();
			this.btnAdd = CreateIconButton (this.imageAdd, "gtk-add");
			DesignerLayout.Pack (this.vbox74, this.btnAdd);
			this.imageRemove = new global::MonoDevelop.Components.ImageView ();
			this.btnRemove = CreateIconButton (this.imageRemove, "gtk-remove");
			DesignerLayout.Pack (this.vbox74, this.btnRemove);
			this.label109 = DesignerLayout.Label (null);
			DesignerLayout.Pack (this.vbox74, this.label109);
			DesignerLayout.Attach (this.table5, this.vbox74, 1, 1);
			this.listSelected = new global::Gtk.TreeView ();
			this.scrolledwindow11 = DesignerLayout.Scrolled (this.listSelected);
			DesignerLayout.Attach (this.table5, this.scrolledwindow11, 2, 1, 1, 1, true, true);
			this.vbox75 = DesignerLayout.VBox (6);
			this.vbox75.Valign = global::Gtk.Align.Center;
			this.imageUp = new global::MonoDevelop.Components.ImageView ();
			this.btnUp = CreateIconButton (this.imageUp, "gtk-go-up");
			DesignerLayout.Pack (this.vbox75, this.btnUp);
			this.imageDown = new global::MonoDevelop.Components.ImageView ();
			this.btnDown = CreateIconButton (this.imageDown, "gtk-go-down");
			DesignerLayout.Pack (this.vbox75, this.btnDown);
			DesignerLayout.Attach (this.table5, this.vbox75, 3, 1);
			DesignerLayout.Pack (content, this.table5, true);
			this.cancelbutton1 = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.cancelbutton1, global::Gtk.ResponseType.Cancel);
			this.okbutton1 = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.okbutton1, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (700, 450);
			this.Response += new global::Gtk.ResponseHandler (this.OnRespond);
			this.btnUp.Clicked += new global::System.EventHandler (this.OnUpClicked);
			this.btnDown.Clicked += new global::System.EventHandler (this.OnDownClicked);
			this.btnAdd.Clicked += new global::System.EventHandler (this.OnAddClicked);
			this.btnRemove.Clicked += new global::System.EventHandler (this.OnRemoveClicked);
		}
	}
}
