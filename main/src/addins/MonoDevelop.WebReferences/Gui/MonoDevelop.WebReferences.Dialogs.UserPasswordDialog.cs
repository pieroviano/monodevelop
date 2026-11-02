// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.WebReferences.Dialogs
{
	internal partial class UserPasswordDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label labelServer;
		private global::Gtk.Grid table1;
		private global::Gtk.Entry entryPwd;
		private global::Gtk.Entry entryUser;
		private global::Gtk.Label label3;
		private global::Gtk.Label labelUser;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.WebReferences.Dialogs.UserPasswordDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Credentials");

			var content = this.ContentArea;
			content.Name = "dialog1_VBox";
			content.MarginTop = content.MarginBottom = content.MarginStart = content.MarginEnd = 2;

			this.vbox2 = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 6);
			this.vbox2.Name = "vbox2";
			this.vbox2.MarginTop = this.vbox2.MarginBottom = this.vbox2.MarginStart = this.vbox2.MarginEnd = 9;

			this.labelServer = new global::Gtk.Label ((string)null);
			this.labelServer.Name = "labelServer";
			this.labelServer.Xalign = 0F;
			this.labelServer.Halign = global::Gtk.Align.Start;
			this.vbox2.Append (this.labelServer);

			this.table1 = new global::Gtk.Grid ();
			this.table1.Name = "table1";
			this.table1.RowSpacing = 6;
			this.table1.ColumnSpacing = 6;

			this.labelUser = new global::Gtk.Label ((string)null);
			this.labelUser.Name = "labelUser";
			this.labelUser.Xalign = 0F;
			this.labelUser.Halign = global::Gtk.Align.Start;
			this.labelUser.LabelProp = global::Mono.Unix.Catalog.GetString ("User:");
			this.table1.Attach (this.labelUser, 0, 0, 1, 1);

			this.entryUser = new global::Gtk.Entry ();
			this.entryUser.CanFocus = true;
			this.entryUser.Name = "entryUser";
			this.entryUser.Editable = true;
			this.entryUser.Hexpand = true;
			this.table1.Attach (this.entryUser, 1, 0, 1, 1);

			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.Halign = global::Gtk.Align.Start;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Password:");
			this.table1.Attach (this.label3, 0, 1, 1, 1);

			this.entryPwd = new global::Gtk.Entry ();
			this.entryPwd.CanFocus = true;
			this.entryPwd.Name = "entryPwd";
			this.entryPwd.Editable = true;
			this.entryPwd.Visibility = false;
			this.entryPwd.Hexpand = true;
			this.table1.Attach (this.entryPwd, 1, 1, 1, 1);

			this.vbox2.Append (this.table1);
			content.Append (this.vbox2);

			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("_Cancel");
			this.AddActionWidget (this.buttonCancel, (int)global::Gtk.ResponseType.Cancel);

			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			this.AddActionWidget (this.buttonOk, (int)global::Gtk.ResponseType.Ok);
			this.DefaultWidget = this.buttonOk;

			this.DefaultWidth = 400;
			this.DefaultHeight = 174;
		}
	}
}
