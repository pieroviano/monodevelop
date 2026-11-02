// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace ILAsmBinding
{
	internal partial class CompilerParametersPanelWidget
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Box vbox3;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label86;
		private global::Gtk.ComboBox compileTargetCombo;
		private global::Gtk.CheckButton checkbuttonIncludeDebugInfo;
		private global::Gtk.Label GtkLabel1;

		protected virtual void Build ()
		{
			this.vbox1 = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 6);
			this.GtkLabel1 = new global::Gtk.Label ((string)null);
			this.GtkLabel1.Xalign = 0F;
			this.GtkLabel1.Halign = global::Gtk.Align.Start;
			this.GtkLabel1.UseMarkup = true;
			this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Code Generation</b>");
			this.vbox1.Append (this.GtkLabel1);
			this.vbox3 = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 6);
			this.vbox3.MarginStart = 12;
			this.hbox1 = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 6);
			this.label86 = new global::Gtk.Label ((string)null);
			this.label86.Xalign = 0F;
			this.label86.LabelProp = global::Mono.Unix.Catalog.GetString ("Compile _Target:");
			this.label86.UseUnderline = true;
			this.hbox1.Append (this.label86);
			this.compileTargetCombo = new global::Gtk.ComboBox ();
			this.label86.MnemonicWidget = this.compileTargetCombo;
			this.hbox1.Append (this.compileTargetCombo);
			this.vbox3.Append (this.hbox1);
			this.checkbuttonIncludeDebugInfo = new global::Gtk.CheckButton ();
			this.checkbuttonIncludeDebugInfo.CanFocus = true;
			this.checkbuttonIncludeDebugInfo.Label = global::Mono.Unix.Catalog.GetString ("Include debug information");
			this.checkbuttonIncludeDebugInfo.UseUnderline = true;
			this.vbox3.Append (this.checkbuttonIncludeDebugInfo);
			this.vbox1.Append (this.vbox3);
			this.Child = this.vbox1;
		}
	}
}
