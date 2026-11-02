using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Execution
{
	internal partial class MonoExecutionParametersPreview
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.Label labelOps;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Options Preview");
			this.Modal = true;
			var content = DesignerLayout.ContentArea (this, 2);
			this.labelOps = new global::Gtk.Label ((string)null);
			this.labelOps.Xalign = 0F;
			this.labelOps.Yalign = 0F;
			this.labelOps.Halign = global::Gtk.Align.Start;
			this.labelOps.Valign = global::Gtk.Align.Start;
			this.labelOps.UseMarkup = true;
			this.labelOps.Wrap = true;
			this.labelOps.Selectable = true;
			this.labelOps.WidthChars = 50;
			DesignerLayout.Margin (this.labelOps, 6);
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.labelOps, false);
			this.GtkScrolledWindow.HscrollbarPolicy = global::Gtk.PolicyType.Never;
			DesignerLayout.Pack (content, this.GtkScrolledWindow, true);
			this.buttonOk = DesignerLayout.Button ("_Close");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Close, true);
			this.SetDefaultSize (501, 341);
		}
	}
}
