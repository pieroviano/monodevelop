
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class PackagingFeatureWidget
	{
		private global::Gtk.Box box;

		protected virtual void Build ()
		{
			this.box = DesignerLayout.VBox (6);
			DesignerLayout.SetContent (this, this.box);
		}
	}
}
