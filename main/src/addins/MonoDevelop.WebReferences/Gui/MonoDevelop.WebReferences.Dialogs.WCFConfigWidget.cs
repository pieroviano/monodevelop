// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.WebReferences.Dialogs
{
	public partial class WCFConfigWidget
	{
		private global::Gtk.Box dialog1_VBox;
		private global::Gtk.Grid wcfOptions;
		private global::Gtk.ComboBoxText dictionaryCollection;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.ComboBoxText listAccess;
		private global::Gtk.ComboBoxText listAsync;
		private global::Gtk.ComboBoxText listCollection;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.WebReferences.Dialogs.WCFConfigWidget";
			this.dialog1_VBox = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 0);
			this.dialog1_VBox.Name = "dialog1_VBox";
			this.dialog1_VBox.MarginTop = this.dialog1_VBox.MarginBottom = this.dialog1_VBox.MarginStart = this.dialog1_VBox.MarginEnd = 2;

			this.wcfOptions = new global::Gtk.Grid ();
			this.wcfOptions.Name = "wcfOptions";
			this.wcfOptions.RowSpacing = 6;
			this.wcfOptions.ColumnSpacing = 6;

			this.label2 = CreateLabel ("label2", global::Mono.Unix.Catalog.GetString ("Collection Mapping:"));
			this.wcfOptions.Attach (this.label2, 0, 0, 1, 1);
			this.listCollection = new global::Gtk.ComboBoxText ();
			this.listCollection.Name = "listCollection";
			this.listCollection.Hexpand = true;
			this.wcfOptions.Attach (this.listCollection, 1, 0, 1, 1);

			this.label1 = CreateLabel ("label1", global::Mono.Unix.Catalog.GetString ("Dictionary type:"));
			this.wcfOptions.Attach (this.label1, 0, 1, 1, 1);
			this.dictionaryCollection = new global::Gtk.ComboBoxText ();
			this.dictionaryCollection.Name = "dictionaryCollection";
			this.wcfOptions.Attach (this.dictionaryCollection, 1, 1, 1, 1);

			this.label3 = CreateLabel ("label3", global::Mono.Unix.Catalog.GetString ("Access level:"));
			this.wcfOptions.Attach (this.label3, 0, 2, 1, 1);
			this.listAccess = new global::Gtk.ComboBoxText ();
			this.listAccess.AppendText (global::Mono.Unix.Catalog.GetString ("Public"));
			this.listAccess.AppendText (global::Mono.Unix.Catalog.GetString ("Internal"));
			this.listAccess.Name = "listAccess";
			this.listAccess.Active = 0;
			this.listAccess.Hexpand = true;
			this.wcfOptions.Attach (this.listAccess, 1, 2, 1, 1);

			this.label4 = CreateLabel ("label4", global::Mono.Unix.Catalog.GetString ("Generate Asynchronous:"));
			this.wcfOptions.Attach (this.label4, 0, 3, 1, 1);
			this.listAsync = new global::Gtk.ComboBoxText ();
			this.listAsync.AppendText (global::Mono.Unix.Catalog.GetString ("No"));
			this.listAsync.AppendText (global::Mono.Unix.Catalog.GetString ("Async"));
			this.listAsync.Name = "listAsync";
			this.listAsync.Active = 0;
			this.wcfOptions.Attach (this.listAsync, 1, 3, 1, 1);

			this.dialog1_VBox.Append (this.wcfOptions);
			this.Child = this.dialog1_VBox;
		}

		static global::Gtk.Label CreateLabel (string name, string text)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.Xalign = 0F;
			label.Halign = global::Gtk.Align.Start;
			label.LabelProp = text;
			return label;
		}
	}
}
