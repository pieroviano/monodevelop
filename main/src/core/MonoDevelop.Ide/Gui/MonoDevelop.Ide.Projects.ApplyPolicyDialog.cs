using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.Ide.Projects
{
	internal partial class ApplyPolicyDialog
	{
		private global::Gtk.Box vbox3;
		private global::Gtk.Box vbox2;
		private global::Gtk.CheckButton radioCustom;
		private global::Gtk.Box boxCustom;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label label2;
		private global::Gtk.ComboBoxText combPolicies;
		private global::Gtk.CheckButton radioFile;
		private global::Gtk.Box boxFile;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label label3;
		private global::MonoDevelop.Components.FileEntry fileEntry;
		private global::Gtk.Box vbox4;
		private global::Gtk.Label labelChangesTitle;
		private global::Gtk.ScrolledWindow policiesScroll;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Apply Policies");
			var content = DesignerLayout.ContentArea (this, 2);
			this.vbox3 = DesignerLayout.Margin (DesignerLayout.VBox (16), 12);
			this.vbox2 = DesignerLayout.VBox (6);
			this.radioCustom = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("Apply stock or custom policy set"));
			this.radioCustom.CanFocus = true;
			this.radioCustom.UseUnderline = true;
			DesignerLayout.Pack (this.vbox2, this.radioCustom);
			this.boxCustom = DesignerLayout.VBox (0);
			this.boxCustom.MarginStart = 42;
			this.hbox1 = DesignerLayout.HBox (6);
			this.label2 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Policy:"));
			DesignerLayout.Pack (this.hbox1, this.label2);
			this.combPolicies = new global::Gtk.ComboBoxText ();
			DesignerLayout.Pack (this.hbox1, this.combPolicies);
			DesignerLayout.Pack (this.boxCustom, this.hbox1);
			DesignerLayout.Pack (this.vbox2, this.boxCustom);
			this.radioFile = new global::Gtk.CheckButton (global::Mono.Unix.Catalog.GetString ("Apply policies from file"));
			this.radioFile.CanFocus = true;
			this.radioFile.UseUnderline = true;
			this.radioFile.Group = this.radioCustom;
			DesignerLayout.Pack (this.vbox2, this.radioFile);
			this.boxFile = DesignerLayout.VBox (0);
			this.boxFile.MarginStart = 42;
			this.hbox2 = DesignerLayout.HBox (6);
			this.label3 = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("File:"));
			DesignerLayout.Pack (this.hbox2, this.label3);
			this.fileEntry = new global::MonoDevelop.Components.FileEntry ();
			this.fileEntry.BrowserTitle = "Select Policy Set File";
			DesignerLayout.Pack (this.hbox2, this.fileEntry, true);
			DesignerLayout.Pack (this.boxFile, this.hbox2);
			DesignerLayout.Pack (this.vbox2, this.boxFile);
			DesignerLayout.Pack (this.vbox3, this.vbox2);
			this.vbox4 = DesignerLayout.VBox (6);
			this.labelChangesTitle = DesignerLayout.Label (global::Mono.Unix.Catalog.GetString ("Policies to set or replace:"));
			DesignerLayout.Pack (this.vbox4, this.labelChangesTitle);
			this.policiesScroll = DesignerLayout.Scrolled (null);
			this.policiesScroll.CanFocus = true;
			DesignerLayout.Pack (this.vbox4, this.policiesScroll, true);
			DesignerLayout.Pack (this.vbox3, this.vbox4, true);
			DesignerLayout.Pack (content, this.vbox3, true);
			this.buttonCancel = DesignerLayout.Button ("_Cancel");
			DesignerLayout.AddActionButton (this, this.buttonCancel, global::Gtk.ResponseType.Cancel);
			this.buttonOk = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("_Apply policies"));
			DesignerLayout.AddActionButton (this, this.buttonOk, global::Gtk.ResponseType.None);
			this.SetDefaultSize (475, 325);
			this.radioCustom.Toggled += new global::System.EventHandler (this.OnRadioCustomToggled);
			this.combPolicies.Changed += new global::System.EventHandler (this.OnCombPoliciesChanged);
			this.fileEntry.PathChanged += new global::System.EventHandler (this.OnFileEntryPathChanged);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
