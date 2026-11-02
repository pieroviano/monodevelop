using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	public partial class ProjectSelectorDialog
	{
		private global::Gtk.Box vbox4;
		private global::Gtk.Label labelTitle;
		private global::MonoDevelop.Ide.Gui.Components.ProjectSelectorWidget selector;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox4 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.labelTitle = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Select a project or solution:"));
			DesignerLayout.Pack (this.vbox4, this.labelTitle);
			this.selector = new global::MonoDevelop.Ide.Gui.Components.ProjectSelectorWidget ();
			this.selector.ShowCheckboxes = false;
			this.selector.CascadeCheckboxSelection = false;
			DesignerLayout.Pack (this.vbox4, this.selector, true);
			DesignerLayout.Pack (content, this.vbox4, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (492, 466);
			this.selector.SelectionChanged += new global::System.EventHandler (this.OnSelectorSelectionChanged);
			this.selector.ActiveChanged += new global::System.EventHandler (this.OnSelectorActiveChanged);
		}
	}
}
