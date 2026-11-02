// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.RegexToolkit
{
	internal partial class ElementHelpWidget
	{
		private global::Gtk.Box vbox4;

		private global::Gtk.Label label11;

		private global::Gtk.ScrolledWindow elementsscrolledwindow;

		private global::Gtk.TreeView elementsTreeview;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.RegexToolkit.ElementHelpWidget";
			this.vbox4 = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 6);
			this.vbox4.Name = "vbox4";
			this.label11 = new global::Gtk.Label ((string)null);
			this.label11.Name = "label11";
			this.label11.Xalign = 0F;
			this.label11.Halign = global::Gtk.Align.Start;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("_Elements:");
			this.label11.UseMarkup = true;
			this.label11.UseUnderline = true;
			this.vbox4.Append (this.label11);
			this.elementsscrolledwindow = new global::Gtk.ScrolledWindow ();
			this.elementsscrolledwindow.CanFocus = true;
			this.elementsscrolledwindow.Name = "elementsscrolledwindow";
			this.elementsscrolledwindow.HasFrame = true;
			this.elementsscrolledwindow.Hexpand = true;
			this.elementsscrolledwindow.Vexpand = true;
			this.elementsTreeview = new global::Gtk.TreeView ();
			this.elementsTreeview.CanFocus = true;
			this.elementsTreeview.Name = "elementsTreeview";
			this.elementsscrolledwindow.Child = this.elementsTreeview;
			this.vbox4.Append (this.elementsscrolledwindow);
			this.vbox4.Hexpand = true;
			this.vbox4.Vexpand = true;
			this.Child = this.vbox4;
			this.label11.MnemonicWidget = this.elementsTreeview;
		}
	}
}
