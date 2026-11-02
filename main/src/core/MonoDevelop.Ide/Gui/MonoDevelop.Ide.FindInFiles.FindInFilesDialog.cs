using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.FindInFiles
{
	public partial class FindInFilesDialog
	{
		private global::Gtk.Box vbox2;

		private global::Gtk.Box hbox3;

		private global::Gtk.ToggleButton toggleFindInFiles;

		private global::Gtk.Separator vseparator1;

		private global::Gtk.ToggleButton toggleReplaceInFiles;

		private global::Gtk.Box hbox1;

		private global::Gtk.Grid tableFindAndReplace;

		private global::Gtk.ComboBoxText comboboxentryFind;

		private global::Gtk.Box hbox2;

		private global::Gtk.ComboBoxText comboboxScope;

		private global::Gtk.Label labelFind;

		private global::Gtk.Label labelScope;

		private global::Gtk.Grid table1;

		private global::Gtk.CheckButton checkbuttonCaseSensitive;

		private global::Gtk.CheckButton checkbuttonRegexSearch;

		private global::Gtk.CheckButton checkbuttonWholeWordsOnly;

		private global::Gtk.Button buttonStop;

		private global::Gtk.Button buttonClose;

		private global::Gtk.Button buttonSearch;

		protected virtual void Build ()
		{
			this.DestroyWithParent = true;
			var content = DesignerLayout.ContentArea (this, 6);
			content.Spacing = 6;
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);
			this.hbox3 = DesignerLayout.HBox (6);
			this.toggleFindInFiles = new global::Gtk.ToggleButton {
				Label = global::Mono.Unix.Catalog.GetString ("Find in Files"),
				TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Switch to Find in Files"),
				Active = true,
				UseUnderline = true,
				HasFrame = false,
			};
			DesignerLayout.Pack (this.hbox3, this.toggleFindInFiles);
			this.vseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Vertical);
			DesignerLayout.Pack (this.hbox3, this.vseparator1);
			this.toggleReplaceInFiles = new global::Gtk.ToggleButton {
				Label = global::Mono.Unix.Catalog.GetString ("Replace in Files"),
				TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Switch to Replace in Files"),
				UseUnderline = true,
				HasFrame = false,
			};
			this.toggleReplaceInFiles.Group = this.toggleFindInFiles;
			DesignerLayout.Pack (this.hbox3, this.toggleReplaceInFiles);
			DesignerLayout.Pack (this.vbox2, this.hbox3);
			this.hbox1 = DesignerLayout.HBox (6);
			this.tableFindAndReplace = DesignerLayout.Grid (6, 6);
			this.comboboxentryFind = global::Gtk.ComboBoxText.NewWithEntry ();
			DesignerLayout.Attach (this.tableFindAndReplace, this.comboboxentryFind, 1, 0, hexpand: true);
			this.hbox2 = DesignerLayout.HBox (6);
			this.comboboxScope = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox2, this.comboboxScope);
			DesignerLayout.Attach (this.tableFindAndReplace, this.hbox2, 1, 1);
			this.labelFind = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Find:"));
			this.labelFind.UseUnderline = true;
			DesignerLayout.Attach (this.tableFindAndReplace, this.labelFind, 0, 0);
			this.labelScope = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Look in:"));
			this.labelScope.UseUnderline = true;
			DesignerLayout.Attach (this.tableFindAndReplace, this.labelScope, 0, 1);
			DesignerLayout.Pack (this.hbox1, this.tableFindAndReplace, true);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			this.table1 = DesignerLayout.Grid (6, 6);
			this.checkbuttonCaseSensitive = new global::Gtk.CheckButton ();
			this.checkbuttonCaseSensitive.Label = global::Mono.Unix.Catalog.GetString ("C_ase sensitive");
			this.checkbuttonCaseSensitive.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonCaseSensitive, 0, 0, hexpand: true);
			this.checkbuttonRegexSearch = new global::Gtk.CheckButton ();
			this.checkbuttonRegexSearch.Label = global::Mono.Unix.Catalog.GetString ("Rege_x search");
			this.checkbuttonRegexSearch.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonRegexSearch, 0, 2, hexpand: true);
			this.checkbuttonWholeWordsOnly = new global::Gtk.CheckButton ();
			this.checkbuttonWholeWordsOnly.Label = global::Mono.Unix.Catalog.GetString ("_Whole words only");
			this.checkbuttonWholeWordsOnly.UseUnderline = true;
			DesignerLayout.Attach (this.table1, this.checkbuttonWholeWordsOnly, 0, 1, hexpand: true);
			DesignerLayout.Pack (this.vbox2, this.table1);
			DesignerLayout.Pack (content, this.vbox2);
			this.buttonStop = DesignerLayout.Button ("_Stop");
			DesignerLayout.AddActionButton (this, this.buttonStop, (global::Gtk.ResponseType)0);
			this.buttonClose = DesignerLayout.Button ("_Close");
			DesignerLayout.AddActionButton (this, this.buttonClose, global::Gtk.ResponseType.Close);
			this.buttonSearch = DesignerLayout.Button ("_Find");
			DesignerLayout.AddActionButton (this, this.buttonSearch, (global::Gtk.ResponseType)0);
			this.SetDefaultSize (456, 348);
			this.labelFind.MnemonicWidget = this.comboboxentryFind;
			this.labelScope.MnemonicWidget = this.comboboxScope;
		}
	}
}
