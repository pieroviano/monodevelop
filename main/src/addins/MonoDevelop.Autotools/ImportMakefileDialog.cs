using MonoDevelop.Components.Designer;

using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide;

namespace MonoDevelop.Autotools
{
	class ImportMakefileDialog: Gtk.Dialog
	{
		Entry nameEntry;
		MakefileOptionPanelWidget optionsWidget;
		Project project;
		
		public ImportMakefileDialog (Project project, MakefileData tmpData, string name)
		{
			this.TransientFor = IdeApp.Workbench.RootWindow;
			this.project = project;
			
			Title = GettextCatalog.GetString ("Makefile Project Import");
			Modal = true;
			
			Gtk.Box box = DesignerLayout.VBox (6);
			
			Gtk.Label lab = new Gtk.Label ();
			lab.Wrap = true;
			lab.Xalign = 0;
			lab.WidthRequest = 500;
			lab.Text = GettextCatalog.GetString (
				"{0} is going to create a project bound to a Makefile. Please enter the name you want to give to the new project.",
				BrandingService.ApplicationName
			);
			DesignerLayout.Pack (box, lab, false);
			
			Gtk.Box hb = DesignerLayout.HBox (6);
			DesignerLayout.Pack (hb, new Gtk.Label (GettextCatalog.GetString ("Project Name:")), false);
			nameEntry = new Gtk.Entry ();
			nameEntry.Text = name;
			DesignerLayout.Pack (hb, nameEntry, true);
			DesignerLayout.Pack (box, hb, false);
			
			DesignerLayout.Pack (box, new Gtk.Separator (Gtk.Orientation.Horizontal), false);
			
			optionsWidget = new MakefileOptionPanelWidget (this, project, tmpData);
			
			DesignerLayout.Pack (box, optionsWidget, false);
			DesignerLayout.Margin (box, 6);
			
			DesignerLayout.Pack (this.ContentArea, box, true);
			
			this.AddButton (GettextCatalog.GetString ("_Cancel"), ResponseType.Cancel);
			this.AddButton (GettextCatalog.GetString ("_OK"), ResponseType.Ok);
			
			optionsWidget.SetImportMode ();
		}
		
		public bool Store ()
		{
			if (nameEntry.Text.Length == 0) {
				MessageService.ShowError (GettextCatalog.GetString ("Please enter a valid project name"));
				return false;
			}
			project.Name = nameEntry.Text;
			if (!optionsWidget.ValidateChanges (project))
				return false;
			optionsWidget.Store (project);
			return true;
		}
	}
}
