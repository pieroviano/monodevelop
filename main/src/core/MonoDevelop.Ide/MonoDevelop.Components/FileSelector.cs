// FileSelector.cs
//
// Author:
//   John Luke  <jluke@cfl.rr.com>
//
// Copyright (c) 2004 John Luke
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
//
//

using System;
using Gtk;

using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace MonoDevelop.Components
{
	class FileSelector : FileChooserDialog
	{
		const string LastPathProperty = "MonoDevelop.FileSelector.LastPath";

		public FileSelector () : this (GettextCatalog.GetString ("Open file..."), Gtk.FileChooserAction.Open)
		{
		}

		public FileSelector (string title) : this (title, Gtk.FileChooserAction.Open)
		{
		}
		
		//for some reason GTK# needs this to create wrapper objects
		protected FileSelector (IntPtr ptr) : base (ptr) {}

		public FileSelector (string title, Gtk.FileChooserAction action) : base (title, null, action)
		{
			// Gtk.Stock is gone in GTK 4: use mnemonic labels.
			switch (action) {
				case Gtk.FileChooserAction.Open:
					AddButton (GettextCatalog.GetString ("_Cancel"), ResponseType.Cancel);
					AddButton (GettextCatalog.GetString ("_Open"), ResponseType.Ok);
					break;
				case Gtk.FileChooserAction.SelectFolder:
					AddButton (GettextCatalog.GetString ("_Cancel"), ResponseType.Cancel);
					AddButton (GettextCatalog.GetString ("Select Folder"), ResponseType.Ok);
					break;
				case Gtk.FileChooserAction.Save:
					AddButton (GettextCatalog.GetString ("_Cancel"), ResponseType.Cancel);
					AddButton (GettextCatalog.GetString ("_Save"), ResponseType.Ok);
					break;
				default:
					break;
			}
			DefaultResponse = ResponseType.Ok;

			CommonSetup ();
		}

		/// <summary>Sets the current folder from a local path (GTK 4 FileChooser takes a GLib.IFile).</summary>
		public bool SetCurrentFolder (string path)
		{
			if (string.IsNullOrEmpty (path))
				return false;
			return SetCurrentFolder (GLib.FileFactory.NewForPath (path));
		}

		/// <summary>Selects a local file (GTK 4 FileChooser takes a GLib.IFile).</summary>
		public bool SetFilename (string path)
		{
			if (string.IsNullOrEmpty (path))
				return false;
			return SetFile (GLib.FileFactory.NewForPath (path));
		}

		/// <summary>Local path of the current folder (null when none or not local).</summary>
		public string CurrentFolderPath {
			get { return CurrentFolder?.Path; }
		}

		/// <summary>Local paths of the selected files (replaces GTK 2 <c>Filenames</c>).</summary>
		public string[] Filenames {
			get {
				var model = Files;
				if (model == null)
					return new string [0];
				var n = (int)model.NItems;
				var list = new System.Collections.Generic.List<string> (n);
				for (uint i = 0; i < n; i++) {
					var obj = model.GetObject (i);
					var path = obj != null ? GLib.FileAdapter.GetObject (obj)?.Path : null;
					if (path != null)
						list.Add (path);
				}
				return list.ToArray ();
			}
		}

		void CommonSetup ()
		{
			// Restore the last active directory
			string last = PropertyService.Get<string> (LastPathProperty);
			if (last != null && last.Length > 0)
				this.SetCurrentFolder (last);
			else
				this.SetCurrentFolder (Environment.GetFolderPath (Environment.SpecialFolder.Personal));

			// add default project path as a MD bookmark
			string pathName = IdeApp.Preferences.ProjectsDefaultPath;

			if (FileService.IsDirectory (pathName)) {
				try {
					this.AddShortcutFolder (GLib.FileFactory.NewForPath (pathName));
				} catch {
					// This may fail if the folder is already registered, and the ShortcutFolders is not
					// giving the correct values, so there isn't another way to check it.
				}
			}

			// GTK 4 has no current-folder-changed signal: remember the folder when the dialog responds.
			this.Response += OnResponse;
		}

		void OnResponse (object o, ResponseArgs args)
		{
			var folder = CurrentFolderPath;
			if (!string.IsNullOrEmpty (folder))
				PropertyService.Set (LastPathProperty, folder);
		}
	}
}
