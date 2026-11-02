// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor
{
	partial class GotoLineNumberWidget
	{
		private global::Gtk.Box hbox1;
		private global::Gtk.Entry entryLineNumber;
		private global::Gtk.Button buttonGoToLine;
		private global::MonoDevelop.Components.ImageView image2;
		private global::Gtk.Button closeButton;
		private global::MonoDevelop.Components.ImageView image1;

		protected virtual void Build ()
		{
			this.hbox1 = DesignerLayout.HBox (0);
			this.entryLineNumber = new global::Gtk.Entry ();
			this.entryLineNumber.Editable = true;
			DesignerLayout.Pack (this.hbox1, this.entryLineNumber, true);

			this.buttonGoToLine = new global::Gtk.Button ();
			this.buttonGoToLine.HasFrame = false;
			this.image2 = new global::MonoDevelop.Components.ImageView ("gtk-jump-to", global::MonoDevelop.Components.StockIconSize.Menu);
			this.buttonGoToLine.Child = this.image2;
			DesignerLayout.Pack (this.hbox1, this.buttonGoToLine);

			this.closeButton = new global::Gtk.Button ();
			this.closeButton.HasFrame = false;
			this.image1 = new global::MonoDevelop.Components.ImageView ("gtk-close", global::MonoDevelop.Components.StockIconSize.Menu);
			this.closeButton.Child = this.image1;
			DesignerLayout.Pack (this.hbox1, this.closeButton);

			DesignerLayout.SetContent (this, this.hbox1);
		}
	}
}
