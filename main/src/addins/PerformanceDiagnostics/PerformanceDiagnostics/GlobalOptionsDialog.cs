using System;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;

namespace PerformanceDiagnosticsAddIn
{
	public class GlobalOptionsPanel : OptionsPanel
	{
		FolderEntry folderEntry = new FolderEntry ();
		//Gtk.CheckButton uiCountersCheck;

		public override Control CreatePanelWidget()
		{
			var vbox = new Gtk.Box (Gtk.Orientation.Vertical, 6);

			var generalSectionLabel = new Gtk.Label ((string)null);
			generalSectionLabel.UseMarkup = true;
			generalSectionLabel.LabelProp = "<b>" + GettextCatalog.GetString ("General") + "</b>";
			generalSectionLabel.Xalign = 0;
			generalSectionLabel.Halign = Gtk.Align.Start;
			vbox.Append (generalSectionLabel);

			var outputDirectoryHBox = new Gtk.Box (Gtk.Orientation.Horizontal, 6);
			outputDirectoryHBox.MarginTop = outputDirectoryHBox.MarginBottom = outputDirectoryHBox.MarginStart = outputDirectoryHBox.MarginEnd = 10;
			var outputDirectoryLabel = new Gtk.Label (GettextCatalog.GetString ("Output directory:"));
			outputDirectoryLabel.Xalign = 0;
			outputDirectoryHBox.Append (outputDirectoryLabel);
			folderEntry.Path = Options.OutputPath.Value;
			folderEntry.SetEntryAccessibleTitleUIElement (outputDirectoryLabel);
			folderEntry.Hexpand = true;
			outputDirectoryHBox.Append (folderEntry);

			vbox.Append (outputDirectoryHBox);

			//if (Options.HasMemoryLeakFeature) {
			//	uiCountersCheck = new Gtk.CheckButton (GettextCatalog.GetString ("Show UI Widget counters in the toolbar"));
			//	uiCountersCheck.Active = Options.ShowUICounters.Value;

			//	vbox.PackStart (uiCountersCheck, false, false, 0);
			//}

			return vbox;
		}

		public override void ApplyChanges()
		{
			Options.OutputPath.Value = folderEntry.Path;

			//if (Options.HasMemoryLeakFeature)
				//Options.ShowUICounters.Value = uiCountersCheck.Active;
		}
	}
}

