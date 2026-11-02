using MonoDevelop.Components.Designer;

using System;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using MonoDevelop.Components;

namespace MonoDevelop.Deployment.Gui
{

	internal partial class EditPackageDialog : Gtk.Dialog
	{
		PackageBuilder target;
		Package package;
		
		public EditPackageDialog (Package package)
		{
			this.Build();
			
			this.package = package;
			target = package.PackageBuilder.Clone ();
			this.Title = target.Description;
			

			entryName.Text = package.Name;
			
			DesignerLayout.Pack (targetBox, new PackageBuilderEditor (target), true);
			
			entrySelector.Fill (target, null);
			entrySelector.SetSelection (target.RootSolutionItem, target.GetChildEntries ());
			
			DeployContext ctx = target.CreateDeployContext ();
			if (ctx == null)
				pageFiles.Hide ();
			else
				ctx.Dispose ();
		}

		protected virtual void OnEntryNameChanged(object sender, System.EventArgs e)
		{
			okbutton.Sensitive = entryName.Text.Length > 0;
		}

		protected virtual void OnOkbuttonClicked(object sender, System.EventArgs e)
		{
			string msg = target.Validate ();
			if (!string.IsNullOrEmpty (msg)) {
				MonoDevelop.Ide.MessageService.ShowError (this, msg);
				return;
			}
			package.Name = entryName.Text;
			package.PackageBuilder = target.Clone ();
			Respond (Gtk.ResponseType.Ok);
		}

		protected virtual void OnEntrySelectorSelectionChanged(object sender, System.EventArgs e)
		{
			SolutionFolderItem ce = entrySelector.GetSelectedEntry ();
			if (ce != null)
				target.SetSolutionItem (ce, entrySelector.GetSelectedChildren ());
		}

		protected virtual void OnNotebookSwitchPage(object o, Gtk.SwitchPageArgs args)
		{
			if (args.PageNum == 2) {
				fileListView.Fill (target);
			}
		}
	}
}
