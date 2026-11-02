
// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Components.Designer;
namespace MonoDevelop.ChangeLogAddIn
{
	internal partial class ProjectOptionPanelWidget
	{
		private global::Gtk.Notebook notebook1;
		private global::Gtk.Box vbox2;
		private global::Gtk.CheckButton noneRadioButton;
		private global::Gtk.Label label3;
		private global::Gtk.CheckButton nearestRadioButton;
		private global::Gtk.Label label5;
		private global::Gtk.CheckButton oneChangeLogInProjectRootDirectoryRadioButton;
		private global::Gtk.Label label6;
		private global::Gtk.CheckButton oneChangeLogInEachDirectoryRadioButton;
		private global::Gtk.Label label7;
		private global::Gtk.Box alignment1;
		private global::Gtk.Box alignment2;
		private global::Gtk.CheckButton checkVersionControl;
		private global::Gtk.Box alignment3;
		private global::Gtk.CheckButton checkRequireOnCommit;
		private global::Gtk.Label label1;
		private global::Gtk.Box vbox1;
		private global::MonoDevelop.VersionControl.CommitMessageStylePanelWidget messageWidget;
		private global::Gtk.Label label2;

		protected virtual void Build ()
		{
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CurrentPage = 0;
			this.vbox2 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox2, 6);
			this.noneRadioButton = new global::Gtk.CheckButton ();
			this.noneRadioButton.Active = true;
			this.noneRadioButton.UseUnderline = true;
			this.label3 = new global::Gtk.Label ((string)null);
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Disable ChangeLog support</b>\nNo ChangeLog entries will be generated for this project.");
			this.label3.UseMarkup = true;
			this.noneRadioButton.Child = this.label3;
			DesignerLayout.Pack (this.vbox2, this.noneRadioButton);
			this.nearestRadioButton = new global::Gtk.CheckButton ();
			this.nearestRadioButton.Group = this.noneRadioButton;
			this.nearestRadioButton.UseUnderline = true;
			this.label5 = new global::Gtk.Label ((string)null);
			this.label5.WidthRequest = 500;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Update nearest ChangeLog</b>\nThe nearest ChangeLog file in the directory hierarchy will be updated (below the commit directory). If none is found, a warning message will be shown. ChangeLog files will never be automatically created.");
			this.label5.UseMarkup = true;
			this.label5.Wrap = true;
			this.nearestRadioButton.Child = this.label5;
			DesignerLayout.Pack (this.vbox2, this.nearestRadioButton);
			this.oneChangeLogInProjectRootDirectoryRadioButton = new global::Gtk.CheckButton ();
			this.oneChangeLogInProjectRootDirectoryRadioButton.Group = this.noneRadioButton;
			this.oneChangeLogInProjectRootDirectoryRadioButton.UseUnderline = true;
			this.label6 = new global::Gtk.Label ((string)null);
			this.label6.WidthRequest = 500;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Single project ChangeLog</b>\nAll changes made in the project files will be logged in a single ChangeLog file, located at the project root directory. The ChangeLog file will be created if it doesn't exist.");
			this.label6.UseMarkup = true;
			this.label6.Wrap = true;
			this.oneChangeLogInProjectRootDirectoryRadioButton.Child = this.label6;
			DesignerLayout.Pack (this.vbox2, this.oneChangeLogInProjectRootDirectoryRadioButton);
			this.oneChangeLogInEachDirectoryRadioButton = new global::Gtk.CheckButton ();
			this.oneChangeLogInEachDirectoryRadioButton.Group = this.noneRadioButton;
			this.oneChangeLogInEachDirectoryRadioButton.UseUnderline = true;
			this.label7 = new global::Gtk.Label ((string)null);
			this.label7.WidthRequest = 500;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>One ChangeLog in each directory</b>\nFile changes will be logged in a ChangeLog located at the file's directory. The ChangeLog file will be created if it doesn't exist.");
			this.label7.UseMarkup = true;
			this.label7.Wrap = true;
			this.oneChangeLogInEachDirectoryRadioButton.Child = this.label7;
			DesignerLayout.Pack (this.vbox2, this.oneChangeLogInEachDirectoryRadioButton);
			this.alignment1 = DesignerLayout.VBox (0);
			this.alignment2 = DesignerLayout.VBox (0);
			this.alignment2.MarginTop = 18;
			this.checkVersionControl = new global::Gtk.CheckButton ();
			this.checkVersionControl.Label = global::Mono.Unix.Catalog.GetString ("Integrate with _version control");
			this.checkVersionControl.UseUnderline = true;
			DesignerLayout.Pack (this.alignment2, this.checkVersionControl, true);
			this.checkVersionControl.Hexpand = true;
			DesignerLayout.Pack (this.alignment1, this.alignment2, true);
			this.alignment2.Hexpand = true;
			DesignerLayout.Pack (this.vbox2, this.alignment1);
			this.alignment3 = DesignerLayout.VBox (0);
			this.alignment3.MarginStart = 24;
			this.checkRequireOnCommit = new global::Gtk.CheckButton ();
			this.checkRequireOnCommit.Label = global::Mono.Unix.Catalog.GetString ("_Require ChangeLog entries for all files when committing");
			this.checkRequireOnCommit.UseUnderline = true;
			DesignerLayout.Pack (this.alignment3, this.checkRequireOnCommit, true);
			this.checkRequireOnCommit.Hexpand = true;
			DesignerLayout.Pack (this.vbox2, this.alignment3);
			this.label1 = new global::Gtk.Label ((string)null);
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("ChangeLog Generation");
			this.notebook1.AppendPage (this.vbox2, this.label1);
			this.vbox1 = DesignerLayout.VBox (6);
			DesignerLayout.Margin (this.vbox1, 9);
			this.messageWidget = new global::MonoDevelop.VersionControl.CommitMessageStylePanelWidget ();
			DesignerLayout.Pack (this.vbox1, this.messageWidget, true);
			this.label2 = new global::Gtk.Label ((string)null);
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Message Style");
			this.notebook1.AppendPage (this.vbox1, this.label2);
			DesignerLayout.SetContent (this, this.notebook1);
			this.noneRadioButton.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.nearestRadioButton.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.oneChangeLogInProjectRootDirectoryRadioButton.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.oneChangeLogInEachDirectoryRadioButton.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.checkVersionControl.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.checkRequireOnCommit.Toggled += new global::System.EventHandler (this.ValueChanged);
			this.messageWidget.Changed += new global::System.EventHandler (this.OnMessageWidgetChanged);
		}
	}
}
