// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.DesignerSupport.Toolbox
{
	internal partial class ComponentSelectorDialog
	{
		private global::Gtk.Box vbox2;

		private global::Gtk.Box hbox1;

		private global::Gtk.Label label1;

		private global::Gtk.ComboBoxText comboType;

		private global::Gtk.Separator vseparator1;

		private global::Gtk.Button button24;

		private global::Gtk.Box hbox2;

		private global::MonoDevelop.Components.ImageView imageview1;

		private global::Gtk.Label label2;

		private global::Gtk.ScrolledWindow scrolledwindow1;

		private global::Gtk.TreeView listView;

		private global::Gtk.CheckButton checkGroupByCat;

		private global::Gtk.Button buttonCancel;

		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.DesignerSupport.Toolbox.ComponentSelectorDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Toolbox Item Selector");
			this.vbox2 = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 6);
			this.vbox2.Name = "vbox2";
			this.vbox2.MarginTop = this.vbox2.MarginBottom = this.vbox2.MarginStart = this.vbox2.MarginEnd = 6;
			this.vbox2.Hexpand = true;
			this.vbox2.Vexpand = true;
			this.hbox1 = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 6);
			this.hbox1.Name = "hbox1";
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Type of component:");
			this.hbox1.Append (this.label1);
			this.comboType = new global::Gtk.ComboBoxText ();
			this.comboType.Name = "comboType";
			this.comboType.Hexpand = true;
			this.hbox1.Append (this.comboType);
			this.vseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Vertical);
			this.vseparator1.Name = "vseparator1";
			this.hbox1.Append (this.vseparator1);
			this.button24 = new global::Gtk.Button ();
			this.button24.CanFocus = true;
			this.button24.Name = "button24";
			this.hbox2 = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 2);
			this.hbox2.Name = "hbox2";
			this.imageview1 = new global::MonoDevelop.Components.ImageView ();
			this.imageview1.Name = "imageview1";
			this.imageview1.IconId = "gtk-add";
			this.imageview1.IconSize = global::MonoDevelop.Components.StockIconSize.Menu;
			this.hbox2.Append (this.imageview1);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Add Assembly...");
			this.label2.UseUnderline = true;
			this.hbox2.Append (this.label2);
			this.button24.Child = this.hbox2;
			this.hbox1.Append (this.button24);
			this.vbox2.Append (this.hbox1);
			this.listView = new global::Gtk.TreeView ();
			this.listView.CanFocus = true;
			this.listView.Name = "listView";
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.HasFrame = true;
			this.scrolledwindow1.Hexpand = true;
			this.scrolledwindow1.Vexpand = true;
			this.scrolledwindow1.Child = this.listView;
			this.vbox2.Append (this.scrolledwindow1);
			this.checkGroupByCat = new global::Gtk.CheckButton ();
			this.checkGroupByCat.CanFocus = true;
			this.checkGroupByCat.Name = "checkGroupByCat";
			this.checkGroupByCat.Label = global::Mono.Unix.Catalog.GetString ("Group by component category");
			this.checkGroupByCat.UseUnderline = true;
			this.vbox2.Append (this.checkGroupByCat);
			this.ContentArea.Append (this.vbox2);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("_Cancel");
			this.AddActionWidget (this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			this.AddActionWidget (this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (642, 433);
			this.comboType.Changed += new global::System.EventHandler (this.OnComboTypeChanged);
			this.button24.Clicked += new global::System.EventHandler (this.OnButton24Clicked);
			this.checkGroupByCat.Toggled += new global::System.EventHandler (this.OnCheckbutton1Clicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
