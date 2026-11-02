
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.Autotools
{
	public partial class GenerateMakefilesDialog
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Label label2;
		private global::Gtk.Box vbox3;
		private global::Gtk.CheckButton rbSimple;
		private global::Gtk.Label label4;
		private global::Gtk.CheckButton rbAutotools;
		private global::Gtk.Label label5;
		private global::Gtk.Separator hseparator1;
		private global::Gtk.Label label3;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBoxText comboConfigs;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Generate Makefiles");
			this.Resizable = false;
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox2 = DesignerLayout.VBox (12);
			DesignerLayout.Margin (this.vbox2, 12);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the kind of Makefile you want to generate:");
			DesignerLayout.Pack (this.vbox2, this.label2);
			this.vbox3 = DesignerLayout.VBox (6);
			this.rbSimple = new global::Gtk.CheckButton ();
			this.rbSimple.UseUnderline = true;
			this.label4 = new global::Gtk.Label ((string)null);
			this.label4.WidthRequest = 550;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Simple Makefile</b>\nGenerates set of Makefiles with the most common targets, and a configuration script that does a basic check of package dependencies.");
			this.label4.UseMarkup = true;
			this.label4.Wrap = true;
			this.rbSimple.Child = this.label4;
			DesignerLayout.Pack (this.vbox3, this.rbSimple);
			this.rbAutotools = new global::Gtk.CheckButton ();
			this.rbAutotools.Group = this.rbSimple;
			this.rbAutotools.UseUnderline = true;
			this.label5 = new global::Gtk.Label ((string)null);
			this.label5.WidthRequest = 550;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Autotools based Makefile</b>\nGenerates a Makefile structure based on Autotools, with the standard targets and configuration scripts.");
			this.label5.UseMarkup = true;
			this.label5.Wrap = true;
			this.rbAutotools.Child = this.label5;
			DesignerLayout.Pack (this.vbox3, this.rbAutotools);
			DesignerLayout.Pack (this.vbox2, this.vbox3);
			this.hseparator1 = new global::Gtk.Separator (global::Gtk.Orientation.Horizontal);
			DesignerLayout.Pack (this.vbox2, this.hseparator1);
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.WidthRequest = 580;
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the configuration that the Makefile will build by default. Other configurations can be selected via the '--config' option of the generated configure script.");
			this.label3.Wrap = true;
			DesignerLayout.Pack (this.vbox2, this.label3);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Default configuration:");
			DesignerLayout.Pack (this.hbox1, this.label1);
			this.comboConfigs = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.comboConfigs);
			DesignerLayout.Pack (this.vbox2, this.hbox1);
			DesignerLayout.Pack (content, this.vbox2);
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "_Cancel";
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "_OK";
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.Ok);
			this.SetDefaultSize (614, 359);
		}
	}
}
