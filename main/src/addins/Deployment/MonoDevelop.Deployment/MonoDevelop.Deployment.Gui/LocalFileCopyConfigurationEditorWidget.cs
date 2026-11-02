
using System;
using MonoDevelop.Deployment;
using MonoDevelop.Deployment.Targets;

namespace MonoDevelop.Deployment.Gui
{
	internal partial class LocalFileCopyConfigurationEditorWidget : MonoDevelop.Components.BinWidget
	{
		LocalFileCopyConfiguration config;
		
		public LocalFileCopyConfigurationEditorWidget (LocalFileCopyConfiguration config)
		{
			this.Build();
			this.config = config;
			folderEntry.Path = config.TargetDirectory;
		}

		protected virtual void OnFolderEntryPathChanged(object sender, System.EventArgs e)
		{
			config.TargetDirectory = folderEntry.Path;
		}
	}
}
