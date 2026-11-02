// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	public partial class KeyBindingsPanel
	{
		private global::Gtk.Box vbox;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label labelScheme;
		private global::Gtk.ComboBoxText schemeCombo;
		private global::Gtk.Separator hseparator2;
		private global::Gtk.Box hbox2;
		private global::MonoDevelop.Components.SearchEntry searchEntry;
		private global::Gtk.Label label1;
		private global::Gtk.Box globalWarningBox;
		private global::Gtk.Frame frame1;
		private global::Gtk.Box warningBox;
		private global::MonoDevelop.Components.ImageView imageWarning;
		private global::Gtk.Label label2;
		private global::MonoDevelop.Components.MenuButton conflicButton;
		private global::Gtk.ScrolledWindow scrolledwindow;
		private global::Gtk.TreeView keyTreeView;
		private global::Gtk.Label labelMessage;
		private global::Gtk.Box hbox;
		private global::Gtk.Label labelEditBinding;
		private global::Gtk.Entry accelEntry;
		private global::Gtk.Button updateButton;
		private global::Gtk.Button addButton;

		protected virtual void Build ()
		{
			this.vbox = DesignerLayout.VBox (6);
			this.hbox1 = DesignerLayout.HBox (6);
			this.labelScheme = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Scheme:"));
			DesignerLayout.Pack (this.hbox1, this.labelScheme);
			this.schemeCombo = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.schemeCombo);
			DesignerLayout.Pack (this.vbox, this.hbox1);
			this.hseparator2 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox, this.hseparator2);
			this.hbox2 = DesignerLayout.HBox (6);
			this.hbox2.SetSizeRequest (300, -1);
			this.searchEntry = new global::MonoDevelop.Components.SearchEntry ();
			this.searchEntry.SetSizeRequest (250, -1);
			this.searchEntry.ForceFilterButtonVisible = true;
			this.searchEntry.HasFrame = true;
			this.searchEntry.RoundedShape = true;
			this.searchEntry.IsCheckMenu = false;
			this.searchEntry.ActiveFilterID = 0;
			this.searchEntry.Ready = false;
			DesignerLayout.Pack (this.hbox2, this.searchEntry);
			this.label1 = new global::Gtk.Label ((string)null);
			DesignerLayout.Pack (this.hbox2, this.label1, true);
			DesignerLayout.Pack (this.vbox, this.hbox2);
			this.globalWarningBox = DesignerLayout.VBox (6);
			this.frame1 = new global::Gtk.Frame ();
			this.warningBox = DesignerLayout.Margin (DesignerLayout.HBox (6), 2);
			this.imageWarning = new global::MonoDevelop.Components.ImageView ();
			this.imageWarning.IconId = "gtk-dialog-warning";
			this.imageWarning.IconSize = global::MonoDevelop.Components.StockIconSize.Menu;
			DesignerLayout.Pack (this.warningBox, this.imageWarning);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("The current scheme has conflicting key bindings"));
			DesignerLayout.Pack (this.warningBox, this.label2, true);
			this.conflicButton = new global::MonoDevelop.Components.MenuButton ();
			this.conflicButton.UseUnderline = false;
			this.conflicButton.UseMarkup = false;
			DesignerLayout.Pack (this.warningBox, this.conflicButton);
			this.frame1.Child = this.warningBox;
			DesignerLayout.Pack (this.globalWarningBox, this.frame1);
			DesignerLayout.Pack (this.vbox, this.globalWarningBox);
			this.keyTreeView = new global::Gtk.TreeView ();
			this.keyTreeView.EnableSearch = false;
			this.scrolledwindow = DesignerLayout.Scrolled (this.keyTreeView);
			DesignerLayout.Pack (this.vbox, this.scrolledwindow, true);
			this.labelMessage = DesignerLayout.Label (null, true);
			this.labelMessage.Ellipsize = Pango.EllipsizeMode.End;
			DesignerLayout.Pack (this.vbox, this.labelMessage);
			this.hbox = DesignerLayout.HBox (6);
			this.labelEditBinding = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Edit Binding"));
			DesignerLayout.Pack (this.hbox, this.labelEditBinding);
			this.accelEntry = new global::Gtk.Entry ();
			this.accelEntry.Editable = true;
			DesignerLayout.Pack (this.hbox, this.accelEntry, true);
			this.updateButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Apply"));
			DesignerLayout.Pack (this.hbox, this.updateButton);
			this.addButton = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Add"));
			DesignerLayout.Pack (this.hbox, this.addButton);
			DesignerLayout.Pack (this.vbox, this.hbox);
			DesignerLayout.SetContent (this, this.vbox);
			this.globalWarningBox.Visible = false;
			this.labelMessage.Visible = false;
		}
	}
}
