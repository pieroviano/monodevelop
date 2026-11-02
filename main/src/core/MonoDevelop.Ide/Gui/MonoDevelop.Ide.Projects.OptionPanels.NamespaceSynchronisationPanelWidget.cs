using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects.OptionPanels
{
	internal partial class NamespaceSynchronisationPanelWidget
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.CheckButton checkAssociateNamespacesDirectories;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box namespaceAssociationBox;
		private global::Gtk.CheckButton checkDefaultAsRoot;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Box vbox4;
		private global::Gtk.CheckButton radioFlat;
		private global::Gtk.CheckButton radioHierarch;
		private global::Gtk.Box hbox2;
		private global::Gtk.Frame previewFrame;
		private global::Gtk.Label GtkLabel6;
		private global::Gtk.CheckButton checkVSStyleResourceNames;
		private global::Gtk.Label label2;

		protected virtual void Build ()
		{
			this.vbox2 = DesignerLayout.VBox (12);
			this.checkAssociateNamespacesDirectories = new global::Gtk.CheckButton ();
			this.checkAssociateNamespacesDirectories.Label = global::Mono.Unix.Catalog.GetString ("_Associate namespaces with directory names");
			this.checkAssociateNamespacesDirectories.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.checkAssociateNamespacesDirectories);
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment1.MarginStart = 24;
			this.namespaceAssociationBox = DesignerLayout.VBox (12);
			this.checkDefaultAsRoot = new global::Gtk.CheckButton ();
			this.checkDefaultAsRoot.Label = global::Mono.Unix.Catalog.GetString ("Use _default namespace as root");
			this.checkDefaultAsRoot.UseUnderline = true;
			DesignerLayout.Pack (this.namespaceAssociationBox, this.checkDefaultAsRoot);
			this.hbox1 = DesignerLayout.HBox (6);
			this.label1 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Directory structure:"));
			this.label1.Yalign = 0F;
			this.label1.Valign = global::Gtk.Align.Start;
			DesignerLayout.Pack (this.hbox1, this.label1);
			this.vbox4 = DesignerLayout.VBox (6);
			this.radioFlat = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("_Flat"));
			this.radioFlat.UseUnderline = true;
			DesignerLayout.Pack (this.vbox4, this.radioFlat);
			this.radioHierarch = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("_Hierarchical"));
			this.radioHierarch.UseUnderline = true;
			this.radioHierarch.Group = this.radioFlat;
			DesignerLayout.Pack (this.vbox4, this.radioHierarch);
			DesignerLayout.Pack (this.hbox1, this.vbox4, true);
			DesignerLayout.Pack (this.namespaceAssociationBox, this.hbox1);
			this.hbox2 = DesignerLayout.HBox (6);
			this.previewFrame = new global::Gtk.Frame ((string)null);
			this.previewFrame.WidthRequest = 400;
			this.GtkLabel6 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("Preview"));
			this.previewFrame.LabelWidget = this.GtkLabel6;
			DesignerLayout.Pack (this.hbox2, this.previewFrame, true);
			DesignerLayout.Pack (this.namespaceAssociationBox, this.hbox2);
			DesignerLayout.Pack (this.alignment1, this.namespaceAssociationBox);
			DesignerLayout.Pack (this.vbox2, this.alignment1);
			this.checkVSStyleResourceNames = new global::Gtk.CheckButton ();
			this.label2 = new global::Gtk.Label (global::Mono.Unix.Catalog.GetString ("<b>Use _Visual Studio-style resource names</b>\nVisual Studio generates a default ID for embedded resources, instead of simply using the resource's filename."));
			this.label2.UseMarkup = true;
			this.label2.UseUnderline = true;
			this.label2.Wrap = true;
			this.label2.Xalign = 0F;
			this.checkVSStyleResourceNames.Child = this.label2;
			DesignerLayout.Pack (this.vbox2, this.checkVSStyleResourceNames);
			DesignerLayout.SetContent (this, this.vbox2);
			this.checkVSStyleResourceNames.Toggled += new global::System.EventHandler (this.UpdatePolicyNameList);
		}
	}
}
