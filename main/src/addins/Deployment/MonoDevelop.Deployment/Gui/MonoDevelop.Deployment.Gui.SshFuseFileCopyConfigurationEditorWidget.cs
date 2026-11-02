
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class SshFuseFileCopyConfigurationEditorWidget
	{
		private global::Gtk.Grid table1;
		private global::Gtk.Entry entryDirectory;
		private global::Gtk.Entry entryHostName;
		private global::Gtk.Entry entryUserName;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;

		protected virtual void Build ()
		{
			this.table1 = DesignerLayout.Grid (0, 0);
			this.entryDirectory = new global::Gtk.Entry ();
			DesignerLayout.Attach (this.table1, this.entryDirectory, 1, 1, hexpand: true);
			this.entryHostName = new global::Gtk.Entry ();
			DesignerLayout.Attach (this.table1, this.entryHostName, 1, 0, hexpand: true);
			this.entryUserName = new global::Gtk.Entry ();
			DesignerLayout.Attach (this.table1, this.entryUserName, 1, 2, hexpand: true);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Host name:");
			DesignerLayout.Attach (this.table1, this.label1, 0, 0);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Directory:");
			DesignerLayout.Attach (this.table1, this.label2, 0, 1);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Username:");
			DesignerLayout.Attach (this.table1, this.label3, 0, 2);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Note: the SSH key for this host must be installed on your system. If it is password protected, the password must be loaded into a running SSH authentication daemon, such as <i>ssh-agent</i> or <i>seahorse-agent</i>.");
			this.label4.UseMarkup = true;
			this.label4.Wrap = true;
			this.label4.Justify = ((global::Gtk.Justification)(3));
			DesignerLayout.Attach (this.table1, this.label4, 0, 3, 2, 1, hexpand: true);
			DesignerLayout.SetContent (this, this.table1);
			this.entryUserName.Changed += new global::System.EventHandler (this.UserNameChanged);
			this.entryHostName.Changed += new global::System.EventHandler (this.HostnameChanged);
			this.entryDirectory.Changed += new global::System.EventHandler (this.DirectoryChanged);
		}
	}
}
