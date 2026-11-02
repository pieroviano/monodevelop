
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Gettext
{
	internal partial class TranslationProjectOptionsDialog
	{
		private global::Gtk.Notebook notebook1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox4;
		private global::Gtk.Label label3;
		private global::Gtk.Entry entryPackageName;
		private global::Gtk.Frame frame1;
		private global::Gtk.Box GtkAlignment2;
		private global::Gtk.Grid table1;
		private global::Gtk.Entry entryRelPath;
		private global::MonoDevelop.Components.FolderEntry folderentrySystemPath;
		private global::Gtk.CheckButton radiobuttonRelPath;
		private global::Gtk.CheckButton radiobuttonSystemPath;
		private global::Gtk.Label GtkLabel4;
		private global::Gtk.Frame frame2;
		private global::Gtk.Box GtkAlignment3;
		private global::Gtk.Box vbox4;
		private global::Gtk.Label label4;
		private global::Gtk.Frame frame3;
		private global::Gtk.Box GtkAlignment4;
		private global::Gtk.Label labelInitString;
		private global::Gtk.Label GtkLabel6;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.TreeView treeviewProjectList;
		private global::Gtk.Label label2;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Translation Options");
			var content = DesignerLayout.ContentArea (this, 2);
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CurrentPage = 1;
			DesignerLayout.Margin (this.notebook1, 6);
			this.vbox2 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox2, 6);
			this.hbox4 = DesignerLayout.HBox (6);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("_Package name:");
			this.label3.UseUnderline = true;
			DesignerLayout.Pack (this.hbox4, this.label3);
			this.entryPackageName = new global::Gtk.Entry ();
			DesignerLayout.Pack (this.hbox4, this.entryPackageName, true);
			DesignerLayout.Pack (this.vbox2, this.hbox4);
			this.frame1 = new global::Gtk.Frame ();
			this.GtkLabel4 = new global::Gtk.Label ((string)null);
			this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Output</b>");
			this.GtkLabel4.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel4;
			this.GtkAlignment2 = DesignerLayout.VBox (0);
			this.GtkAlignment2.MarginStart = 12;
			this.table1 = DesignerLayout.Grid (0, 0);
			this.entryRelPath = new global::Gtk.Entry ();
			DesignerLayout.Attach (this.table1, this.entryRelPath, 1, 0, hexpand: true);
			this.folderentrySystemPath = new global::MonoDevelop.Components.FolderEntry ();
			this.folderentrySystemPath.Visible = false;
			DesignerLayout.Attach (this.table1, this.folderentrySystemPath, 1, 1);
			this.radiobuttonRelPath = new global::Gtk.CheckButton ();
			this.radiobuttonRelPath.Label = global::Mono.Unix.Catalog.GetString ("_Relative to output path:");
			this.radiobuttonRelPath.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.radiobuttonRelPath, 0, 0);
			this.radiobuttonSystemPath = new global::Gtk.CheckButton ();
			this.radiobuttonSystemPath.Label = global::Mono.Unix.Catalog.GetString ("_System path:");
			this.radiobuttonSystemPath.Group = this.radiobuttonRelPath;
			this.radiobuttonSystemPath.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.radiobuttonSystemPath, 0, 1);
			DesignerLayout.Pack (this.GtkAlignment2, this.table1, true);
			this.table1.Hexpand = true;
			this.frame1.Child = this.GtkAlignment2;
			DesignerLayout.Pack (this.vbox2, this.frame1);
			this.frame2 = new global::Gtk.Frame ();
			this.GtkLabel6 = new global::Gtk.Label ((string)null);
			this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Init String</b>");
			this.GtkLabel6.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel6;
			this.GtkAlignment3 = DesignerLayout.VBox (0);
			this.GtkAlignment3.MarginStart = 12;
			this.vbox4 = DesignerLayout.VBox (6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("The catalog initialization string should look like:");
			DesignerLayout.Pack (this.vbox4, this.label4);
			this.frame3 = new global::Gtk.Frame ();
			this.GtkAlignment4 = DesignerLayout.VBox (0);
			this.GtkAlignment4.MarginStart = 12;
			this.labelInitString = new global::Gtk.Label ((string)null);
			this.labelInitString.Xalign = 0F;
			DesignerLayout.Pack (this.GtkAlignment4, this.labelInitString, true);
			this.labelInitString.Hexpand = true;
			this.frame3.Child = this.GtkAlignment4;
			DesignerLayout.Pack (this.vbox4, this.frame3);
			DesignerLayout.Pack (this.GtkAlignment3, this.vbox4, true);
			this.vbox4.Hexpand = true;
			this.frame2.Child = this.GtkAlignment3;
			DesignerLayout.Pack (this.vbox2, this.frame2);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Settings");
			this.notebook1.AppendPage (this.vbox2, this.label1);
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			DesignerLayout.Margin (this.scrolledwindow1, 6);
			this.treeviewProjectList = new global::Gtk.TreeView ();
			this.scrolledwindow1.Child = this.treeviewProjectList;
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Include in Projects");
			this.notebook1.AppendPage (this.scrolledwindow1, this.label2);
			DesignerLayout.Pack (content, this.notebook1, true);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (content, this.hseparator1);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "_Cancel";
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (519, 426);
		}
	}
}
