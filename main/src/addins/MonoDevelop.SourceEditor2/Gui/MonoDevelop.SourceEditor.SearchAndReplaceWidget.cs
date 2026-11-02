// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor
{
	partial class SearchAndReplaceWidget
	{
		private global::Gtk.Grid table;
		private global::Gtk.Button buttonSearchMode;
		private global::Gtk.Image searchButtonModeArrow;
		private global::Gtk.Box hbox1;
		private global::MonoDevelop.Components.SearchEntry searchEntry;
		private global::Gtk.Button buttonSearchBackward;
		private global::MonoDevelop.Components.ImageView image2;
		private global::Gtk.Button buttonSearchForward;
		private global::MonoDevelop.Components.ImageView image3;
		private global::Gtk.Button closeButton;
		private global::MonoDevelop.Components.ImageView image4;
		private global::Gtk.Box hbox2;
		private global::Gtk.Entry entryReplace;
		private global::Gtk.Button buttonReplace;
		private global::MonoDevelop.Components.ImageView image5;
		private global::Gtk.Button buttonReplaceAll;
		private global::Gtk.Box hbox3;
		private global::MonoDevelop.Components.ImageView image6;
		private global::Gtk.Label label1;

		protected virtual void Build ()
		{
			this.table = DesignerLayout.Grid (0, 0);
			DesignerLayout.Margin (this.table, 4);

			this.buttonSearchMode = new global::Gtk.Button ();
			this.searchButtonModeArrow = new global::Gtk.Image ();
			this.searchButtonModeArrow.IconName = "pan-down-symbolic";
			this.buttonSearchMode.Child = this.searchButtonModeArrow;
			DesignerLayout.Attach (this.table, this.buttonSearchMode, 0, 0);

			this.hbox1 = DesignerLayout.HBox (0);
			this.searchEntry = new global::MonoDevelop.Components.SearchEntry ();
			this.searchEntry.ForceFilterButtonVisible = false;
			this.searchEntry.HasFrame = true;
			this.searchEntry.RoundedShape = false;
			this.searchEntry.IsCheckMenu = false;
			this.searchEntry.ActiveFilterID = 0;
			this.searchEntry.Ready = false;
			DesignerLayout.Pack (this.hbox1, this.searchEntry, true);
			this.buttonSearchBackward = new global::Gtk.Button ();
			this.buttonSearchBackward.HasFrame = false;
			this.image2 = new global::MonoDevelop.Components.ImageView ("gtk-go-up", global::MonoDevelop.Components.StockIconSize.Menu);
			this.buttonSearchBackward.Child = this.image2;
			DesignerLayout.Pack (this.hbox1, this.buttonSearchBackward);
			this.buttonSearchForward = new global::Gtk.Button ();
			this.buttonSearchForward.HasFrame = false;
			this.image3 = new global::MonoDevelop.Components.ImageView ("gtk-go-down", global::MonoDevelop.Components.StockIconSize.Menu);
			this.buttonSearchForward.Child = this.image3;
			DesignerLayout.Pack (this.hbox1, this.buttonSearchForward);
			this.closeButton = new global::Gtk.Button ();
			this.closeButton.HasFrame = false;
			this.image4 = new global::MonoDevelop.Components.ImageView ("gtk-close", global::MonoDevelop.Components.StockIconSize.Menu);
			this.closeButton.Child = this.image4;
			DesignerLayout.Pack (this.hbox1, this.closeButton);
			DesignerLayout.Attach (this.table, this.hbox1, 1, 0, hexpand: true);

			this.hbox2 = DesignerLayout.HBox (0);
			this.entryReplace = new global::Gtk.Entry ();
			this.entryReplace.Editable = true;
			DesignerLayout.Pack (this.hbox2, this.entryReplace, true);
			this.buttonReplace = new global::Gtk.Button ();
			this.buttonReplace.HasFrame = false;
			this.image5 = new global::MonoDevelop.Components.ImageView ("gtk-find-and-replace", global::MonoDevelop.Components.StockIconSize.Menu);
			this.buttonReplace.Child = this.image5;
			DesignerLayout.Pack (this.hbox2, this.buttonReplace);
			this.buttonReplaceAll = new global::Gtk.Button ();
			this.buttonReplaceAll.HasFrame = false;
			this.hbox3 = DesignerLayout.HBox (6);
			this.image6 = new global::MonoDevelop.Components.ImageView ("gtk-find-and-replace", global::MonoDevelop.Components.StockIconSize.Menu);
			DesignerLayout.Pack (this.hbox3, this.image6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("All"));
			DesignerLayout.Pack (this.hbox3, this.label1);
			this.buttonReplaceAll.Child = this.hbox3;
			DesignerLayout.Pack (this.hbox2, this.buttonReplaceAll);
			DesignerLayout.Attach (this.table, this.hbox2, 1, 1, hexpand: true);

			DesignerLayout.SetContent (this, this.table);
		}
	}
}
