// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.RegexToolkit
{
	public partial class RegexToolkitWidget
	{
		private global::Gtk.Box vbox2;

		private global::Gtk.Paned vpaned1;

		private global::Gtk.Box vbox1;

		private global::Gtk.Box hbox1;

		private global::Gtk.Box vbox6;

		private global::Gtk.Label label8;

		private global::Gtk.Entry entryRegEx;

		private global::Gtk.Box vbox3;

		private global::Gtk.CheckButton checkbuttonReplace;

		private global::Gtk.Entry entryReplace;

		private global::Gtk.Box hbox7;

		private global::Gtk.CheckButton expandMatches;

		private global::Gtk.Box vbox4;

		private global::Gtk.Label label10;

		private global::Gtk.ScrolledWindow scrolledwindow5;

		private global::Gtk.TreeView optionsTreeview;

		private global::Gtk.Box HelpWidget;

		private global::Gtk.Label label9;

		private global::Gtk.Box hbox5;

		private global::Gtk.ScrolledWindow scrolledwindow1;

		private global::Gtk.TextView inputTextview;

		private global::Gtk.Box hbox4;

		private global::Gtk.Notebook notebook2;

		private global::Gtk.ScrolledWindow scrolledwindow2;

		private global::Gtk.TreeView resultsTreeview;

		private global::Gtk.Label label3;

		private global::Gtk.ScrolledWindow scrolledwindow4;

		private global::Gtk.TextView replaceResultTextview;

		private global::Gtk.Label label4;

		static global::Gtk.Box NewBox (global::Gtk.Orientation orientation, string name, int margin = 0)
		{
			var box = new global::Gtk.Box (orientation, 6);
			box.Name = name;
			if (margin != 0) {
				box.MarginTop = box.MarginBottom = box.MarginStart = box.MarginEnd = margin;
			}
			return box;
		}

		static global::Gtk.Label NewLabel (string name, string text, bool markup)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.Xalign = 0F;
			label.Halign = global::Gtk.Align.Start;
			label.LabelProp = text;
			label.UseMarkup = markup;
			label.UseUnderline = true;
			return label;
		}

		static global::Gtk.ScrolledWindow NewScrolled (string name, global::Gtk.Widget child)
		{
			var sw = new global::Gtk.ScrolledWindow ();
			sw.CanFocus = true;
			sw.Name = name;
			sw.HasFrame = true;
			sw.Hexpand = true;
			sw.Vexpand = true;
			sw.Child = child;
			return sw;
		}

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.RegexToolkit.RegexToolkitWidget";
			this.vbox2 = NewBox (global::Gtk.Orientation.Vertical, "vbox2");
			this.vpaned1 = new global::Gtk.Paned (global::Gtk.Orientation.Vertical);
			this.vpaned1.CanFocus = true;
			this.vpaned1.Name = "vpaned1";
			this.vpaned1.Position = 359;
			this.vpaned1.MarginTop = this.vpaned1.MarginBottom = this.vpaned1.MarginStart = this.vpaned1.MarginEnd = 6;
			this.vpaned1.Hexpand = true;
			this.vpaned1.Vexpand = true;
			this.vbox1 = NewBox (global::Gtk.Orientation.Vertical, "vbox1");
			this.hbox1 = NewBox (global::Gtk.Orientation.Horizontal, "hbox1");
			this.vbox6 = NewBox (global::Gtk.Orientation.Vertical, "vbox6", 6);
			this.vbox6.Hexpand = true;
			this.label8 = NewLabel ("label8", global::Mono.Unix.Catalog.GetString ("Regular Expression"), false);
			this.vbox6.Append (this.label8);
			this.entryRegEx = new global::Gtk.Entry ();
			this.entryRegEx.CanFocus = true;
			this.entryRegEx.Name = "entryRegEx";
			this.entryRegEx.Editable = true;
			this.vbox6.Append (this.entryRegEx);
			this.vbox3 = NewBox (global::Gtk.Orientation.Vertical, "vbox3");
			this.checkbuttonReplace = new global::Gtk.CheckButton ();
			this.checkbuttonReplace.CanFocus = true;
			this.checkbuttonReplace.Name = "checkbuttonReplace";
			this.checkbuttonReplace.Label = global::Mono.Unix.Catalog.GetString ("Replace");
			this.checkbuttonReplace.Active = true;
			this.checkbuttonReplace.UseUnderline = true;
			this.vbox3.Append (this.checkbuttonReplace);
			this.entryReplace = new global::Gtk.Entry ();
			this.entryReplace.CanFocus = true;
			this.entryReplace.Name = "entryReplace";
			this.entryReplace.Editable = true;
			this.vbox3.Append (this.entryReplace);
			this.hbox7 = NewBox (global::Gtk.Orientation.Horizontal, "hbox7");
			this.expandMatches = new global::Gtk.CheckButton ();
			this.expandMatches.CanFocus = true;
			this.expandMatches.Name = "expandMatches";
			this.expandMatches.Label = global::Mono.Unix.Catalog.GetString ("Expand matches");
			this.expandMatches.Active = true;
			this.expandMatches.UseUnderline = true;
			this.expandMatches.MarginTop = this.expandMatches.MarginBottom = this.expandMatches.MarginStart = this.expandMatches.MarginEnd = 3;
			this.hbox7.Append (this.expandMatches);
			this.vbox3.Append (this.hbox7);
			this.vbox6.Append (this.vbox3);
			this.hbox1.Append (this.vbox6);
			this.vbox4 = NewBox (global::Gtk.Orientation.Vertical, "vbox4");
			this.label10 = NewLabel ("label10", global::Mono.Unix.Catalog.GetString ("Options:"), true);
			this.vbox4.Append (this.label10);
			this.optionsTreeview = new global::Gtk.TreeView ();
			this.optionsTreeview.CanFocus = true;
			this.optionsTreeview.Name = "optionsTreeview";
			this.scrolledwindow5 = NewScrolled ("scrolledwindow5", this.optionsTreeview);
			this.vbox4.Append (this.scrolledwindow5);
			this.hbox1.Append (this.vbox4);
			this.vbox1.Append (this.hbox1);
			this.HelpWidget = NewBox (global::Gtk.Orientation.Vertical, "HelpWidget", 6);
			this.HelpWidget.Vexpand = true;
			this.label9 = NewLabel ("label9", global::Mono.Unix.Catalog.GetString ("Input:"), true);
			this.HelpWidget.Append (this.label9);
			this.hbox5 = NewBox (global::Gtk.Orientation.Horizontal, "hbox5");
			this.hbox5.Vexpand = true;
			this.inputTextview = new global::Gtk.TextView ();
			this.inputTextview.CanFocus = true;
			this.inputTextview.Name = "inputTextview";
			this.scrolledwindow1 = NewScrolled ("scrolledwindow1", this.inputTextview);
			this.hbox5.Append (this.scrolledwindow1);
			this.HelpWidget.Append (this.hbox5);
			this.vbox1.Append (this.HelpWidget);
			this.vpaned1.StartChild = this.vbox1;
			this.vpaned1.ResizeStartChild = false;
			this.hbox4 = NewBox (global::Gtk.Orientation.Horizontal, "hbox4", 6);
			this.notebook2 = new global::Gtk.Notebook ();
			this.notebook2.CanFocus = true;
			this.notebook2.Name = "notebook2";
			this.notebook2.Hexpand = true;
			this.notebook2.Vexpand = true;
			this.resultsTreeview = new global::Gtk.TreeView ();
			this.resultsTreeview.CanFocus = true;
			this.resultsTreeview.Name = "resultsTreeview";
			this.scrolledwindow2 = NewScrolled ("scrolledwindow2", this.resultsTreeview);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Matches");
			this.notebook2.AppendPage (this.scrolledwindow2, this.label3);
			this.replaceResultTextview = new global::Gtk.TextView ();
			this.replaceResultTextview.CanFocus = true;
			this.replaceResultTextview.Name = "replaceResultTextview";
			this.replaceResultTextview.Editable = false;
			this.replaceResultTextview.CursorVisible = false;
			this.scrolledwindow4 = NewScrolled ("scrolledwindow4", this.replaceResultTextview);
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Replace");
			this.notebook2.AppendPage (this.scrolledwindow4, this.label4);
			this.hbox4.Append (this.notebook2);
			this.vpaned1.EndChild = this.hbox4;
			this.vbox2.Append (this.vpaned1);
			this.Child = this.vbox2;
			this.label8.MnemonicWidget = this.inputTextview;
			this.label10.MnemonicWidget = this.optionsTreeview;
			this.label9.MnemonicWidget = this.inputTextview;
		}
	}
}
