
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class CommandDeployEditorWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Entry cmdEntry;
		private global::Gtk.Label label2;
		private global::Gtk.Entry argsEntry;
		private global::Gtk.CheckButton checkExternal;
		private global::Gtk.CheckButton checkDisposeExternal;

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Command:");
			DesignerLayout.Pack (this.vbox1, this.label1);
			this.cmdEntry = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.vbox1, this.cmdEntry);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Arguments:");
			DesignerLayout.Pack (this.vbox1, this.label2);
			this.argsEntry = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.vbox1, this.argsEntry);
			this.checkExternal = new global::Gtk.CheckButton ();
			this.checkExternal.Label = global::Mono.Unix.Catalog.GetString ("Run in external console");
			DesignerLayout.Pack (this.vbox1, this.checkExternal);
			this.checkDisposeExternal = new global::Gtk.CheckButton ();
			this.checkDisposeExternal.Label = global::Mono.Unix.Catalog.GetString ("Dispose console after running");
			DesignerLayout.Pack (this.vbox1, this.checkDisposeExternal);
			DesignerLayout.SetContent (this, this.vbox1);
			this.cmdEntry.Changed += new global::System.EventHandler (this.OnCmdEntryChanged);
			this.argsEntry.Changed += new global::System.EventHandler (this.OnArgsEntryChanged);
			this.checkExternal.Toggled += new global::System.EventHandler (this.OnCheckExternalClicked);
			this.checkDisposeExternal.Toggled += new global::System.EventHandler (this.OnCheckDisposeExternalClicked);
		}
	}
}
