using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Execution
{
	internal partial class MonoExecutionParametersWidget
	{
		private global::Gtk.Box hbox1;
		private global::MonoDevelop.Components.PropertyGrid.PropertyGrid propertyGrid;
		private global::Gtk.Box vbox4;
		private global::Gtk.Button buttonReset;
		private global::Gtk.Button buttonPreview;

		protected virtual void Build ()
		{
			this.hbox1 = DesignerLayout.Margin (DesignerLayout.HBox (6), 6);
			this.propertyGrid = new global::MonoDevelop.Components.PropertyGrid.PropertyGrid ();
			this.propertyGrid.ShowToolbar = false;
			this.propertyGrid.ShowHelp = true;
			DesignerLayout.Pack (this.hbox1, this.propertyGrid, true);
			this.vbox4 = DesignerLayout.VBox (6);
			this.buttonReset = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Clear All Options"));
			this.buttonReset.CanFocus = true;
			DesignerLayout.Pack (this.vbox4, this.buttonReset);
			this.buttonPreview = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Preview Options"));
			this.buttonPreview.CanFocus = true;
			DesignerLayout.Pack (this.vbox4, this.buttonPreview);
			DesignerLayout.Pack (this.hbox1, this.vbox4);
			DesignerLayout.SetContent (this, this.hbox1);
			this.buttonReset.Clicked += new global::System.EventHandler (this.OnButtonResetClicked);
			this.buttonPreview.Clicked += new global::System.EventHandler (this.OnButtonPreviewClicked);
		}
	}
}
