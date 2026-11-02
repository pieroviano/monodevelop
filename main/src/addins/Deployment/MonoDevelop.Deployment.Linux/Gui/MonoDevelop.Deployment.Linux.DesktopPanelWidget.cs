
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Linux
{
	public partial class DesktopPanelWidget
	{
		private global::Gtk.Notebook notebook2;
		private global::Gtk.Label label6;

		protected virtual void Build ()
		{
			this.notebook2 = new global::Gtk.Notebook ();
			this.notebook2.CurrentPage = 0;
			var w1 = new global::Gtk.Label ((string)null);
			w1.Visible = true;
			this.label6 = new global::Gtk.Label ((string)null);
			this.notebook2.AppendPage (w1, this.label6);
			DesignerLayout.SetContent (this, this.notebook2);
		}
	}
}
