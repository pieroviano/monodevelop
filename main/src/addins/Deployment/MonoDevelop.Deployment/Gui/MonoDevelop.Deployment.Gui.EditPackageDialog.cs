
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Deployment.Gui
{
	internal partial class EditPackageDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Notebook notebook;
		private global::Gtk.Box targetBox;
		private global::Gtk.Box hbox4;
		private global::Gtk.Label label4;
		private global::Gtk.Entry entryName;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Label label2;
		private global::Gtk.Box vbox4;
		private global::Gtk.Label label6;
		private global::MonoDevelop.Deployment.Gui.EntrySelectionTree entrySelector;
		private global::Gtk.Label label1;
		private global::Gtk.Box pageFiles;
		private global::MonoDevelop.Deployment.DeployFileListWidget fileListView;
		private global::Gtk.Label label5;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button okbutton;

		protected virtual void Build ()
		{
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox2, 12);
			this.notebook = new global::Gtk.Notebook ();
			this.notebook.CurrentPage = 0;
			this.targetBox = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.targetBox, 12);
			this.hbox4 = DesignerLayout.HBox (6);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			DesignerLayout.Pack (this.hbox4, this.label4);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.WidthChars = 40;
			DesignerLayout.Pack (this.hbox4, this.entryName);
			DesignerLayout.Pack (this.targetBox, this.hbox4);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.targetBox, this.hseparator1);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Package Settings");
			this.notebook.AppendPage (this.targetBox, this.label2);
			this.vbox4 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox4, 12);
			this.label6 = new global::Gtk.Label ((string)null);
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the projects and solutions you want to include in the package:");
			DesignerLayout.Pack (this.vbox4, this.label6);
			this.entrySelector = new global::MonoDevelop.Deployment.Gui.EntrySelectionTree ();
			DesignerLayout.Pack (this.vbox4, this.entrySelector, true);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Projects/Solutions");
			this.notebook.AppendPage (this.vbox4, this.label1);
			this.pageFiles = DesignerLayout.HBox (6);
			DesignerLayout.Margin (this.pageFiles, 12);
			this.fileListView = new global::MonoDevelop.Deployment.DeployFileListWidget ();
			DesignerLayout.Pack (this.pageFiles, this.fileListView, true);
			this.label5 = new global::Gtk.Label ((string)null);
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Files");
			this.notebook.AppendPage (this.pageFiles, this.label5);
			DesignerLayout.Pack (this.vbox2, this.notebook, true);
			DesignerLayout.Pack (content, this.vbox2, true);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "_Cancel";
			this.okbutton = new global::Gtk.Button ();
			this.okbutton.UseUnderline = true;
			this.okbutton.Label = "_OK";
			var actions = DesignerLayout.ActionBox (this);
			DesignerLayout.AddResponseButton (this, actions, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			actions.Append (this.okbutton);
			this.SetDefaultSize (599, 450);
			this.notebook.SwitchPage += new global::Gtk.SwitchPageHandler (this.OnNotebookSwitchPage);
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.entrySelector.SelectionChanged += new global::System.EventHandler (this.OnEntrySelectorSelectionChanged);
			this.okbutton.Clicked += new global::System.EventHandler (this.OnOkbuttonClicked);
		}
	}
}
