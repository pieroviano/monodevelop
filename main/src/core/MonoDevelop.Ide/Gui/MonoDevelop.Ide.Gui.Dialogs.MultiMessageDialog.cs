// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.Dialogs
{
	public partial class MultiMessageDialog
	{
		private global::Gtk.ScrolledWindow scrolled;
		private global::Gtk.Box msgBox;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			var content = DesignerLayout.ContentArea (this, 2);
			this.msgBox = DesignerLayout.Margin (DesignerLayout.VBox (12), 6);
			this.scrolled = DesignerLayout.Scrolled (this.msgBox, false);
			this.scrolled.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (content, this.scrolled, true);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (563, 346);
		}
	}
}
