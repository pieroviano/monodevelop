using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class ImportProjectPolicyDialog
	{
		private global::Gtk.Box vbox5;
		private global::Gtk.Label label7;
		private global::Gtk.Entry entryName;
		private global::Gtk.Label label8;
		private global::MonoDevelop.Ide.Gui.Components.ProjectSelectorWidget selector;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox5 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.label7 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Policy Name:"));
			DesignerLayout.Pack (this.vbox5, this.label7);
			this.entryName = new global::Gtk.Entry ();
			this.entryName.Editable = true;
			DesignerLayout.Pack (this.vbox5, this.entryName);
			this.label8 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Select the project or solution from which to import the policies:"));
			DesignerLayout.Pack (this.vbox5, this.label8);
			this.selector = new global::MonoDevelop.Ide.Gui.Components.ProjectSelectorWidget ();
			this.selector.ShowCheckboxes = false;
			this.selector.CascadeCheckboxSelection = false;
			DesignerLayout.Pack (this.vbox5, this.selector, true);
			DesignerLayout.Pack (content, this.vbox5, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (501, 447);
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.selector.SelectionChanged += new global::System.EventHandler (this.OnSelectorSelectionChanged);
		}
	}
}
