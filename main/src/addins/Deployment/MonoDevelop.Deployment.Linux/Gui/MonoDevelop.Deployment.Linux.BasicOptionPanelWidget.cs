
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Linux
{
	public partial class BasicOptionPanelWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box boxExe;
		private global::Gtk.CheckButton checkScript;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label3;
		private global::Gtk.Label label2;
		private global::Gtk.Entry entryScript;
		private global::Gtk.CheckButton checkDesktop;
		private global::Gtk.Box boxLibrary;
		private global::Gtk.CheckButton checkPcFile;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox2, 6);
			this.boxExe = DesignerLayout.VBox (6);
			this.checkScript = new global::Gtk.CheckButton ();
			this.checkScript.Label = global::Mono.Unix.Catalog.GetString ("Generate launch script");
			DesignerLayout.Pack (this.boxExe, this.checkScript);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.WidthRequest = 24;
			DesignerLayout.Pack (this.hbox1, this.label3);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Script name:");
			DesignerLayout.Pack (this.hbox1, this.label2);
			this.entryScript = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox1, this.entryScript, true);
			DesignerLayout.Pack (this.boxExe, this.hbox1);
			this.checkDesktop = new global::Gtk.CheckButton ();
			this.checkDesktop.Label = global::Mono.Unix.Catalog.GetString ("Generate .desktop file");
			DesignerLayout.Pack (this.boxExe, this.checkDesktop);
			DesignerLayout.Pack (this.vbox2, this.boxExe);
			this.boxLibrary = DesignerLayout.VBox (6);
			this.checkPcFile = new global::Gtk.CheckButton ();
			this.checkPcFile.Label = global::Mono.Unix.Catalog.GetString ("Generate .pc file for the library");
			DesignerLayout.Pack (this.boxLibrary, this.checkPcFile);
			DesignerLayout.Pack (this.vbox2, this.boxLibrary);
			DesignerLayout.SetContent (this, this.vbox2);
			this.checkScript.Toggled += new global::System.EventHandler (this.OnCheckScriptClicked);
		}
	}
}
