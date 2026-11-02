/*
 * ProjectOptionsPanelWidget.cs.
 *
 * Author:
 *   Rolf Bjarne Kvinge <RKvinge@novell.com>
 *
 * Copyright 2008 Novell, Inc. (http://www.novell.com)
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;

using MonoDevelop.Projects;
using MonoDevelop.Projects.Text;
 
using MonoDevelop.Core;

namespace MonoDevelop.VBNetBinding
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ProjectOptionsPanelWidget : MonoDevelop.Components.BinWidget
	{
		VBProject project;

		public ProjectOptionsPanelWidget (MonoDevelop.Projects.Project project)
		{
			this.Build();

			this.project = (VBProject) project;

			compileTargetCombo.AppendText (GettextCatalog.GetString ("Executable"));
			compileTargetCombo.AppendText (GettextCatalog.GetString ("Library"));
			compileTargetCombo.AppendText (GettextCatalog.GetString ("Executable with GUI"));
			compileTargetCombo.AppendText (GettextCatalog.GetString ("Module"));
			compileTargetCombo.Active = (int) this.project.CompileTarget;
			compileTargetCombo.Changed += delegate(object sender, EventArgs e) {
				entryMainClass.Sensitive = compileTargetCombo.Active != (int) CompileTarget.Library	&& compileTargetCombo.Active != (int) CompileTarget.Module;
			};

			txtMyType.AppendText ("WindowsForms");
			txtMyType.AppendText ("Windows");
			txtMyType.AppendText ("Console");
			switch (this.project.MyType) {
			case "WindowsForms":
				txtMyType.Active = 0;
				break;
			case "Windows":
				txtMyType.Active = 1;
				break;
			case "Console":
				txtMyType.Active = 2;
				break;
			case null:
			case "":
				break;
			default:
				txtMyType.AppendText (this.project.MyType);
				txtMyType.Active = 3;
				break;
			}

			cmbOptionCompare.AppendText ("Binary");
			cmbOptionCompare.AppendText ("Text");
			cmbOptionCompare.Active = this.project.BinaryOptionCompare ? 0 : 1;
				
			cmbOptionExplicit.AppendText ("On");
			cmbOptionExplicit.AppendText ("Off");
			cmbOptionExplicit.Active = this.project.OptionExplicit ? 0 : 1;
			
			cmbOptionInfer.AppendText ("On");
			cmbOptionInfer.AppendText ("Off");
			cmbOptionInfer.Active = this.project.OptionInfer ? 0 : 1;
			
			cmbOptionStrict.AppendText ("On");
			cmbOptionStrict.AppendText ("Off");
			cmbOptionStrict.Active = this.project.OptionStrict ? 0 : 1;

			// Codepage
			string foundEncoding = null;
			string currentCodepage = this.project.CodePage;
			foreach (TextEncoding e in TextEncoding.SupportedEncodings) {
				if (e.CodePage == -1)
					continue;
				if (e.Id == currentCodepage)
					foundEncoding = e.Id;
				cmbCodePage.AppendText (e.Id);
			}
			if (foundEncoding != null)
				((Gtk.Entry) cmbCodePage.Child).Text = foundEncoding;
			else if (!string.IsNullOrEmpty (currentCodepage))
				((Gtk.Entry) cmbCodePage.Child).Text = currentCodepage;
			
			((Gtk.Entry) entryMainClass.Child).Text = this.project.StartupObject;
			iconEntry.Path = this.project.ApplicationIcon;
		}
		
		public void StorePanelContents ()
		{
			project.BinaryOptionCompare = cmbOptionCompare.ActiveText == "Binary";
			project.OptionExplicit = cmbOptionExplicit.ActiveText == "On";
			project.OptionInfer = cmbOptionInfer.ActiveText == "On";
			project.OptionStrict = cmbOptionStrict.ActiveText == "On";
			project.MyType = txtMyType.ActiveText;
			project.StartupObject = entryMainClass.ActiveText;
			project.CodePage = ((Gtk.Entry) cmbCodePage.Child).Text;
			project.ApplicationIcon = iconEntry.Path;
			project.CompileTarget = (CompileTarget) compileTargetCombo.Active;
		}
	}
}
