using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
// The former Alignment containers (alignment1, alignment2, selectedHeader) are single-child boxes.
// The Delete key handling of ReferencesTreeView is an EventControllerKey (key release).
namespace MonoDevelop.Ide.Projects
{
	internal partial class SelectReferenceDialog
	{
		private global::Gtk.Box vbox5;
		private global::Gtk.Paned hpaned1;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box alignment2;
		private global::Gtk.Box boxRefs;
		private global::Gtk.Box selectedHeader;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label114;
		private global::Gtk.Button RemoveReferenceButton;
		private global::MonoDevelop.Components.ImageView imageAdd;
		private global::Gtk.Box hbox4;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TreeView ReferencesTreeView;
		private global::Gtk.Button cancelbutton;
		private global::Gtk.Button okbutton;

		protected virtual void Build ()
		{
			this.SetSizeRequest (640, 520);
			this.Title = global::Mono.Unix.Catalog.GetString ("References");
			this.DestroyWithParent = true;
			var content = DesignerLayout.ContentArea (this, 6);
			this.vbox5 = DesignerLayout.Margin (DesignerLayout.VBox (12), 6);
			this.hpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal);
			this.hpaned1.Vexpand = true;
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment1.MarginEnd = 3;
			this.hpaned1.StartChild = this.alignment1;
			this.hpaned1.ResizeStartChild = false;
			this.hpaned1.ShrinkStartChild = false;
			this.alignment2 = DesignerLayout.VBox (0);
			this.alignment2.MarginStart = 3;
			this.boxRefs = DesignerLayout.VBox (0);
			this.selectedHeader = DesignerLayout.VBox (0);
			this.hbox2 = DesignerLayout.HBox (6);
			this.label114 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Selected references:"));
			DesignerLayout.Pack (this.hbox2, this.label114, true);
			this.RemoveReferenceButton = new global::Gtk.Button ();
			this.RemoveReferenceButton.TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Remove");
			this.RemoveReferenceButton.FocusOnClick = false;
			this.RemoveReferenceButton.HasFrame = false;
			this.imageAdd = new global::MonoDevelop.Components.ImageView ();
			this.imageAdd.IconId = "gtk-delete";
			this.imageAdd.IconSize = StockIconSize.Menu;
			this.RemoveReferenceButton.Child = this.imageAdd;
			DesignerLayout.Pack (this.hbox2, this.RemoveReferenceButton);
			DesignerLayout.Pack (this.selectedHeader, this.hbox2);
			DesignerLayout.Pack (this.boxRefs, this.selectedHeader);
			this.hbox4 = DesignerLayout.HBox (12);
			this.hbox4.HeightRequest = 150;
			this.ReferencesTreeView = new global::Gtk.TreeView ();
			this.ReferencesTreeView.HeadersVisible = false;
			this.scrolledwindow2 = DesignerLayout.Scrolled (this.ReferencesTreeView);
			DesignerLayout.Pack (this.hbox4, this.scrolledwindow2, true);
			DesignerLayout.Pack (this.boxRefs, this.hbox4, true);
			DesignerLayout.Pack (this.alignment2, this.boxRefs, true);
			this.hpaned1.EndChild = this.alignment2;
			this.hpaned1.ShrinkEndChild = false;
			DesignerLayout.Pack (this.vbox5, this.hpaned1, true);
			DesignerLayout.Pack (content, this.vbox5, true);
			this.cancelbutton = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.cancelbutton, global::Gtk.ResponseType.Cancel);
			this.okbutton = DesignerLayout.Button ("_OK");
			DesignerLayout.AddActionButton (this, this.okbutton, global::Gtk.ResponseType.Ok, true);
			this.SetDefaultSize (889, 551);
			this.RemoveReferenceButton.Clicked += new global::System.EventHandler (this.RemoveReference);
			this.ReferencesTreeView.AddKeyPressHandler (null, (keyval, keycode, state) => {
				if (keyval == (uint)global::Gdk.Key.Delete)
					this.RemoveReference (null, null);
			});
			this.ReferencesTreeView.RowActivated += new global::Gtk.RowActivatedHandler (this.OnReferencesTreeViewRowActivated);
		}
	}
}
