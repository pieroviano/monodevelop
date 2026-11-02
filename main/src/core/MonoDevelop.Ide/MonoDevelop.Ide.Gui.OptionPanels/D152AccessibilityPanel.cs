//
// D152AccessibilityPanel.cs
//
// Author:
//       iain <>
//
// Copyright (c) 2017 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if MAC
using System;
using MonoDevelop.Components;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Projects;
using MonoDevelop.Ide.Gui.Dialogs;
using MonoDevelop.Core;

using Gtk;

using Foundation;

namespace MonoDevelop.Ide.Gui.OptionPanels
{
	public class D152AccessibilityPanel : OptionsPanel
	{
		D152AccessibilityPanelWidget widget;

		public override Control CreatePanelWidget ()
		{
			return widget = new D152AccessibilityPanelWidget ();
		}

		public override void ApplyChanges ()
		{
			widget.Save ();
		}
	}

	class D152AccessibilityPanelWidget : Gtk.Box
	{
		bool originalSetting;
		CheckButton enabled;

		Separator separatorRestart;
		Grid tableRestart;
		Button btnRestart;
		ImageView imageRestart;
		Label labelRestart;

		static string EnabledKey = "com.monodevelop.AccessibilityEnabled";

		public D152AccessibilityPanelWidget () : base (Orientation.Vertical, 6)
		{
			NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;

			enabled = new CheckButton { Label = GettextCatalog.GetString ("Enable Accessibility") };
			enabled.Active = originalSetting = defaults.BoolForKey (EnabledKey);
			enabled.Visible = true;

			enabled.Toggled += ShowQuitOption;
			Append (enabled);

			separatorRestart = new Separator (Orientation.Horizontal);
			Append (this.separatorRestart);

			tableRestart = DesignerLayout.Grid (6, 6);

			btnRestart = new Button ();
			btnRestart.CanFocus = true;
			btnRestart.UseUnderline = true;
			btnRestart.Clicked += RestartClicked;

			tableRestart.Attach (btnRestart, 1, 1, 1, 1);

			imageRestart = new global::MonoDevelop.Components.ImageView ();
			imageRestart.IconId = "md-information";
			imageRestart.IconSize = StockIconSize.Menu;

			tableRestart.Attach (imageRestart, 0, 0, 1, 1);
			labelRestart = new global::Gtk.Label ();
			tableRestart.Attach (labelRestart, 1, 0, 2, 1);

			Append (tableRestart);

			labelRestart.LabelProp = GettextCatalog.GetString ("These preferences will take effect next time you start {0}", BrandingService.ApplicationName);
			btnRestart.Label = GettextCatalog.GetString ("Restart {0}", BrandingService.ApplicationName);

		}

		public void Save ()
		{
			NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
			defaults.SetBool (enabled.Active, EnabledKey);
			defaults.Synchronize ();
		}

		void ShowQuitOption (object sender, EventArgs args)
		{
			if (enabled.Active != originalSetting) {
				tableRestart.Visible = true;
				separatorRestart.Visible = true;
			} else {
				tableRestart.Visible = false;
				separatorRestart.Visible = false;
			}
		}

		void RestartClicked (object sender, System.EventArgs e)
		{
			Save ();
			IdeApp.Restart (true).Ignore ();
		}
	}
}

#endif
