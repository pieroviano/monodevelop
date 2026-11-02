// 
// NewProjectOptionsWidget.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2011 Novell, Inc.
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
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using System.Text;

namespace MonoDevelop.Ide.Projects
{
	class NewProjectOptionsWidget : MonoDevelop.Components.BinWidget
	{
		Entry nameEntry;
		FileEntry locationEntry;
		Entry solutionNameEntry;
		CheckButton createSolutionDirectoryCheck;
		Label saveInLabel;
		Label solutionNameLabel;
		
		string basePath;
		bool showSolutionOptions = true;
		string lastName;
		
		public NewProjectOptionsWidget ()
		{
			var table = new Grid {
				ColumnSpacing = 4,
				RowSpacing = 4,
			};
			int row = 0;
			
			var header = new Label (GettextCatalog.GetString ("<b>Project Options</b>")) {
				Xalign = 0,
				UseMarkup = true,
			};
			header.Hexpand = true;
			table.Attach (header, 0, row, 3, 1);
			
			row++;
			
			nameEntry = new Entry ();
			nameEntry.Changed += NameChanged;
			var nameLabel = new Label (GettextCatalog.GetString ("N_ame:")) {
				UseUnderline = true,
				MnemonicWidget = nameEntry,
				Xalign = 0,
			};
			table.Attach (nameLabel, 0, row, 1, 1);
			nameEntry.Hexpand = true;
			table.Attach (nameEntry, 1, row, 2, 1);
			
			row++;
			
			locationEntry = new FileEntry ();
			locationEntry.PathChanged += delegate {
				UpdateState ();
			};
			var locationLabel = new Label (GettextCatalog.GetString ("L_ocation:")) {
				UseUnderline = true,
				MnemonicWidget = locationEntry,
				Xalign = 0,
			};
			table.Attach (locationLabel, 0, row, 1, 1);
			locationEntry.Hexpand = true;
			table.Attach (locationEntry, 1, row, 2, 1);
			
			solutionNameEntry = new Entry ();
			solutionNameEntry.Sensitive = false;
			solutionNameEntry.Changed += delegate {
				UpdateState ();
			};
			solutionNameLabel = new Label (GettextCatalog.GetString ("_Solution name:")) {
				UseUnderline = true,
				MnemonicWidget = solutionNameEntry,
				Xalign = 0,
			};
			createSolutionDirectoryCheck = new CheckButton {
				Label = GettextCatalog.GetString ("_Create directory for solution"),
				UseUnderline = true,
			};
			createSolutionDirectoryCheck.Toggled += delegate {
				solutionNameEntry.Sensitive = createSolutionDirectoryCheck.Active;
				UpdateState ();
			};
			
			row++;
			
			table.Attach (solutionNameLabel, 0, row, 1, 1);
			solutionNameEntry.Hexpand = true;
			table.Attach (solutionNameEntry, 1, row, 1, 1);
			table.Attach (createSolutionDirectoryCheck, 2, row, 1, 1);
			
			row++;
			
			saveInLabel = new Label () {
				Xalign = 0,
			};
			table.Attach (saveInLabel, 1, row, 2, 1);
			
			Child = table;
		}

		void NameChanged (object sender, EventArgs e)
		{
			string newName = nameEntry.Text;
			string solutionName = solutionNameEntry.Text;
			if (string.IsNullOrEmpty (solutionName) || solutionName == (lastName ?? ""))
				solutionNameEntry.Text = newName;
			lastName = newName;
			UpdateState ();
		}
		
		public new string Name {
			get {
				return nameEntry.Text;
			}
			set {
				nameEntry.Text = value ?? "";
				UpdateState ();
			}
		}
		
		public string Location {
			get {
				return locationEntry.Path;
			}
			set {
				locationEntry.Path = value ?? "";
				UpdateState ();
			}
		}
		
		public string SolutionLocation {
			get {
				FilePath path = Location;
				if (CreateSolutionDirectory)
					return path.Combine (GetValidDir (SolutionName));
				else
					return path.Combine (GetValidDir (Name));
			}
		}
		
		public string ProjectLocation {
			get {
				FilePath path = Location;
				if (CreateSolutionDirectory)
					path = path.Combine (GetValidDir (SolutionName));
				return path.Combine (GetValidDir (Name));
			}
		}
		
		string GetValidDir (string name)
		{
			if (name == null)
				return "";
			name = name.Trim ();
			var sb = new StringBuilder ();
			for (int n=0; n<name.Length; n++) {
				char c = name [n];
				if (Array.IndexOf (FilePath.GetInvalidPathChars (), c) != -1)
					continue;
				if (c == System.IO.Path.DirectorySeparatorChar || c == System.IO.Path.AltDirectorySeparatorChar || c == System.IO.Path.VolumeSeparatorChar)
					continue;
				sb.Append (c);
			}
			return sb.ToString ();
		}
		
		public string SolutionName {
			get {
				return solutionNameEntry.Text;
			}
			set {
				solutionNameEntry.Text = value ?? "";
				UpdateState ();
			}
		}
		
		public bool CreateSolutionDirectory {
			get {
				return createSolutionDirectoryCheck.Active && createSolutionDirectoryCheck.Sensitive;
			}
			set {
				createSolutionDirectoryCheck.Active = value;
				solutionNameEntry.Sensitive = createSolutionDirectoryCheck.Active;
				UpdateState ();
			}
		}
		
		public bool ShowSolutionOptions {
			get { return showSolutionOptions; }
			set {
				if (value == showSolutionOptions)
					return;
				solutionNameEntry.Visible = value;
				createSolutionDirectoryCheck.Visible = value;
				solutionNameLabel.Visible = value;
				UpdateState ();
			}
		}
		
		public string BasePath {
			get { return basePath; }
			set {
				basePath = value;
				if (string.IsNullOrEmpty (Location))
					Location = value;
			}
		}
		
		void UpdateState ()
		{
			bool ready = FileService.IsValidFileName (Name) && GetValidDir (Name).Length > 0
				&& FileService.IsValidPath (Location)
				&& (!ShowSolutionOptions || !CreateSolutionDirectory ||
					(FileService.IsValidFileName (Name) && GetValidDir (SolutionName).Length > 0));
			
			if (ready != Ready) {
				Ready = ready;
				var evt = ReadyChanged;
				if (evt != null)
					evt (this, EventArgs.Empty);
			}
			
			saveInLabel.Text = GettextCatalog.GetString ("Project will be saved at {0}", ProjectLocation);
		}
		
		public event EventHandler ReadyChanged;
		
		public bool Ready { get; private set; }
	}
}
