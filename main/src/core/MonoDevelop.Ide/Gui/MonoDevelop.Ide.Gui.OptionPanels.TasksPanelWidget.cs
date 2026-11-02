// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class TasksPanelWidget
	{
		private global::Gtk.Box vbox6;
		private global::Gtk.Box hbox2;
		private global::Gtk.Box vbox7;
		private global::Gtk.Label labelTokens;
		private global::Gtk.ScrolledWindow scrolledwindow3;
		private global::Gtk.TreeView tokensTreeView;
		private global::Gtk.Box vbox14;
		private global::Gtk.Box vboxPriority;
		private global::Gtk.Label label112;
		private global::Gtk.Entry entryToken;
		private global::Gtk.Label label113;
		private global::Gtk.Box hbuttonbox2;
		private global::Gtk.Button buttonChange;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Label label;
		private global::Gtk.Separator hseparator2;
		private global::Gtk.Box frame1;
		private global::Gtk.Grid table6;
		private global::Gtk.ColorButton colorbuttonHighPrio;
		private global::Gtk.ColorButton colorbuttonLowPrio;
		private global::Gtk.ColorButton colorbuttonNormalPrio;
		private global::Gtk.Label label10;
		private global::Gtk.Label label11;
		private global::Gtk.Label label12;
		private global::Gtk.Label label9;

		protected virtual void Build ()
		{
			this.vbox6 = DesignerLayout.VBox (12);
			this.hbox2 = DesignerLayout.HBox (8);
			this.vbox7 = DesignerLayout.VBox (0);
			this.tokensTreeView = new global::Gtk.TreeView ();
			this.tokensTreeView.HeadersVisible = false;
			this.labelTokens = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Token List:"), false, this.tokensTreeView);
			this.labelTokens.Yalign = 0F;
			DesignerLayout.Pack (this.vbox7, this.labelTokens);
			this.scrolledwindow3 = DesignerLayout.Scrolled (this.tokensTreeView);
			this.scrolledwindow3.MinContentWidth = 200;
			DesignerLayout.Pack (this.vbox7, this.scrolledwindow3, true);
			DesignerLayout.Pack (this.hbox2, this.vbox7, true);
			this.vbox14 = DesignerLayout.VBox (4);
			this.vboxPriority = DesignerLayout.VBox (4);
			this.entryToken = new global::Gtk.Entry ();
			this.entryToken.Editable = true;
			this.entryToken.MaxLength = 50;
			this.label112 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("_Name:"), false, this.entryToken);
			DesignerLayout.Pack (this.vboxPriority, this.label112);
			DesignerLayout.Pack (this.vboxPriority, this.entryToken);
			this.label113 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Priority:"));
			DesignerLayout.Pack (this.vboxPriority, this.label113);
			DesignerLayout.Pack (this.vbox14, this.vboxPriority);
			this.hbuttonbox2 = DesignerLayout.HBox (6);
			this.hbuttonbox2.Halign = global::Gtk.Align.End;
			this.buttonChange = DesignerLayout.Button ("_Edit");
			DesignerLayout.Pack (this.hbuttonbox2, this.buttonChange);
			this.buttonRemove = DesignerLayout.Button ("_Remove");
			DesignerLayout.Pack (this.hbuttonbox2, this.buttonRemove);
			this.buttonAdd = DesignerLayout.Button ("_Add");
			DesignerLayout.Pack (this.hbuttonbox2, this.buttonAdd);
			DesignerLayout.Pack (this.vbox14, this.hbuttonbox2);
			this.label = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<i><b>Note:</b> Only Letters, Digits and Underscore are allowed.</i>"), true);
			this.label.Xalign = 0.5F;
			this.label.Yalign = 0F;
			this.label.MarginTop = 12;
			this.label.MarginBottom = 12;
			this.label.Wrap = true;
			this.label.Justify = global::Gtk.Justification.Center;
			DesignerLayout.Pack (this.vbox14, this.label);
			DesignerLayout.Pack (this.hbox2, this.vbox14, true);
			DesignerLayout.Pack (this.vbox6, this.hbox2, true);
			this.hseparator2 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox6, this.hseparator2);
			this.frame1 = DesignerLayout.VBox (0);
			this.label9 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("<b>Task Priorities Foreground Colors</b>"), true);
			this.label9.Xalign = 0.5F;
			this.label9.Halign = global::Gtk.Align.Center;
			DesignerLayout.Pack (this.frame1, this.label9);
			this.table6 = DesignerLayout.Grid (4, 6);
			this.table6.MarginStart = 12;
			this.table6.MarginTop = 4;
			this.colorbuttonHighPrio = new global::Gtk.ColorButton ();
			DesignerLayout.Attach (this.table6, this.colorbuttonHighPrio, 1, 0);
			this.colorbuttonNormalPrio = new global::Gtk.ColorButton ();
			DesignerLayout.Attach (this.table6, this.colorbuttonNormalPrio, 1, 1);
			this.colorbuttonLowPrio = new global::Gtk.ColorButton ();
			DesignerLayout.Attach (this.table6, this.colorbuttonLowPrio, 1, 2);
			this.label10 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("High"));
			this.label10.Xalign = 1F;
			this.label10.Halign = global::Gtk.Align.End;
			DesignerLayout.Attach (this.table6, this.label10, 0, 0);
			this.label11 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Normal"));
			this.label11.Xalign = 1F;
			this.label11.Halign = global::Gtk.Align.End;
			DesignerLayout.Attach (this.table6, this.label11, 0, 1);
			this.label12 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Low"));
			this.label12.Xalign = 1F;
			this.label12.Halign = global::Gtk.Align.End;
			this.label12.Justify = global::Gtk.Justification.Center;
			DesignerLayout.Attach (this.table6, this.label12, 0, 2);
			DesignerLayout.Pack (this.frame1, this.table6);
			DesignerLayout.Pack (this.vbox6, this.frame1);
			DesignerLayout.SetContent (this, this.vbox6);
		}
	}
}
