
using System;
using MonoDevelop.Projects;

namespace MonoDevelop.Autotools
{
	[System.ComponentModel.Category("widget")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class MakefileIntegrationFeatureWidget : MonoDevelop.Components.BinWidget
	{
		public MakefileIntegrationFeatureWidget (Project project)
		{
			this.Build();
		}
		
		public void Store ()
		{
		}
	}
}
