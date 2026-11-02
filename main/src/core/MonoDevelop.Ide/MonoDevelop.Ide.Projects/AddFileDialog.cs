// 
// AddFileDialog.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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

using System;
using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Components.Extensions;
using MonoDevelop.Ide.Extensions;
using MonoDevelop.Components;
using Gtk;
using MonoDevelop.Core;


namespace MonoDevelop.Ide.Projects
{
	/// <summary>
	/// Dialog which allows selecting files to be added to a project
	/// </summary>
	public class AddFileDialog: SelectFileDialog<AddFileDialogData>
	{
		public AddFileDialog (string title)
		{
			Title = title;
			Action = MonoDevelop.Components.FileChooserAction.Open;
			data.SelectMultiple = true;
		}
		
		/// <summary>
		/// Build actions from which the user can select the one to apply to the new file.
		/// </summary>
		public string[] BuildActions {
			get { return data.BuildActions; }
			set { data.BuildActions = value; }
		}
		
		/// <summary>
		/// Selected build action.
		/// </summary>
		public string OverrideAction {
			get { return data.OverrideAction; }
		}
		
		protected override bool RunDefault ()
		{
			FileSelector fdiag  = new FileSelector (data.Title);

			// TODO GTK4: GtkFileChooser has no show-hidden property (data.ShowHidden is not applied)
			
			// add a choice that can be used to override the default build action (GTK 4 file choosers have no extra widget)
			const string buildActionChoice = "build-action";
			const string defaultActionId = "";
			var actions = (data.BuildActions ?? new string[0]).Where (a => a != "--").ToList ();
			var optionIds = new List<string> { defaultActionId };
			var optionLabels = new List<string> { GettextCatalog.GetString ("Default build action") };
			optionIds.AddRange (actions);
			optionLabels.AddRange (actions);
			fdiag.AddChoice (buildActionChoice, GettextCatalog.GetString ("Override default build action"), optionIds.ToArray (), optionLabels.ToArray ());
			fdiag.SetChoice (buildActionChoice, defaultActionId);
			
			SetDefaultProperties (fdiag);
			
			int result;
			
			try {
				result = MessageService.RunCustomDialog (fdiag, data.TransientFor ?? MessageService.RootWindow);
				GetDefaultProperties (fdiag);
				var chosen = fdiag.GetChoice (buildActionChoice);
				data.OverrideAction = string.IsNullOrEmpty (chosen) ? null : chosen;
				return result == (int) Gtk.ResponseType.Ok;
			} finally {
				fdiag.Destroy ();
				fdiag.Dispose ();
			}
		}
	}
}
