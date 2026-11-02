//
// FileSelectorDialog.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Ide.Codons;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Core;
using Mono.Addins;
using MonoDevelop.Projects.Text;
using Gtk;
using MonoDevelop.Ide.Gui;
using System.Text;

#pragma warning disable 612

namespace MonoDevelop.Ide.Gui.Dialogs
{
	class FileSelectorDialog: FileSelector
	{
		int selectOption;
		int firstEncIndex;

		Gtk.Label encodingLabel;
		Gtk.ComboBoxText encodingMenu;
		Gtk.Label viewerLabel;
		Gtk.ComboBoxText viewerSelector;
		Gtk.CheckButton closeWorkspaceCheck;
		Gtk.Widget extraWidget;
		List<FileViewer> currentViewers = new List<FileViewer> ();

		// The GTK 4 file chooser has no selection-changed notification: the selection is polled while the dialog is mapped
		const uint selectionPollInterval = 250;
		uint selectionPollTimeout;
		string lastSelection;
		string lastFolder;

		/// <summary>Raised when the folder shown by the chooser changes (GTK 4 has no current-folder-changed signal; it is polled).</summary>
		public event EventHandler CurrentFolderChanged;

		public FileSelectorDialog (string title): this (title, Gtk.FileChooserAction.Open)
		{
		}

		public FileSelectorDialog (string title, Gtk.FileChooserAction action): base (title, action)
		{
			// Add the text encoding selector
			var table = DesignerLayout.Grid (6, 6);
			DesignerLayout.Margin (table, 6);

			encodingLabel = new Label (GettextCatalog.GetString ("_Character Coding:"));
			encodingLabel.UseUnderline = true;
			encodingLabel.Xalign = 0;
			DesignerLayout.Attach (table, encodingLabel, 0, 0);

			encodingMenu = new Gtk.ComboBoxText ();
			encodingLabel.MnemonicWidget = encodingMenu;
			FillEncodings ();
			DesignerLayout.Attach (table, encodingMenu, 1, 0, hexpand: true);

			// Add the viewer selector
			viewerLabel = new Label (GettextCatalog.GetString ("Open With:"));
			viewerLabel.Xalign = 0;
			DesignerLayout.Attach (table, viewerLabel, 0, 1);

			var box = DesignerLayout.HBox (6);
			viewerSelector = new Gtk.ComboBoxText ();
			DesignerLayout.Pack (box, viewerSelector, true);
			closeWorkspaceCheck = new CheckButton { Label = GettextCatalog.GetString ("Close current workspace") };
			closeWorkspaceCheck.Active = true;
			DesignerLayout.Pack (box, closeWorkspaceCheck);
			DesignerLayout.Attach (table, box, 1, 1, hexpand: true);
			FillViewers ();
			viewerSelector.Changed += OnViewerChanged;

			// The GTK 2 extra widget lives below the chooser in the dialog content area
			extraWidget = table;
			DesignerLayout.Pack (ContentArea, extraWidget);

			if (action == Gtk.FileChooserAction.SelectFolder)
				ShowEncodingSelector = false;

			if (action != Gtk.FileChooserAction.Open)
				closeWorkspaceCheck.Visible = ShowViewerSelector = false;

			Mapped += OnDialogMapped;
			Unmapped += OnDialogUnmapped;
		}

		/// <summary>First selected file or empty when nothing is selected.</summary>
		string FirstSelectedFile {
			get {
				var files = Filenames;
				return files.Length > 0 ? files [0] : string.Empty;
			}
		}

		void OnDialogMapped (object sender, EventArgs args)
		{
			if (selectionPollTimeout != 0)
				return;
			lastSelection = string.Join ("\n", Filenames);
			lastFolder = CurrentFolderPath;
			selectionPollTimeout = GLib.Timeout.Add (selectionPollInterval, () => {
				var folder = CurrentFolderPath;
				if (folder != lastFolder) {
					lastFolder = folder;
					CurrentFolderChanged?.Invoke (this, EventArgs.Empty);
				}
				var selection = string.Join ("\n", Filenames);
				if (selection != lastSelection) {
					lastSelection = selection;
					OnSelectionChanged ();
				}
				return true;
			});
		}

		void OnDialogUnmapped (object sender, EventArgs args)
		{
			if (selectionPollTimeout != 0) {
				GLib.Source.Remove (selectionPollTimeout);
				selectionPollTimeout = 0;
			}
		}

		public TextEncoding SelectedEncoding {
			get {
				if (!ShowEncodingSelector)
					return null;
				int history = encodingMenu.Active;
				if (history < firstEncIndex || history == selectOption)
					return null;
				return TextEncoding.ConversionEncodings [history - firstEncIndex];
			}
			set {
				for (int n=0; n < TextEncoding.ConversionEncodings.Length; n++) {
					if (TextEncoding.ConversionEncodings [n] == value) {
						encodingMenu.Active = n + firstEncIndex;
						return;
					}
				}
				encodingMenu.Active = 0;
			}
		}

		public bool ShowEncodingSelector {
			get { return encodingLabel.Visible; }
			set { encodingLabel.Visible = encodingMenu.Visible = value; }
		}

		public bool ShowViewerSelector {
			get { return viewerLabel.Visible; }
			set { viewerLabel.Visible = viewerSelector.Visible = value; }
		}

		public bool CloseCurrentWorkspace {
			get { return closeWorkspaceCheck.Active; }
		}

		void FillEncodings ()
		{
			selectOption = -1;
			encodingMenu.Changed -= EncodingChanged;
			encodingMenu.RemoveAll ();

			// Don't show the auto-detection option when saving
			if (Action != Gtk.FileChooserAction.Save) {
				encodingMenu.AppendText (GettextCatalog.GetString ("Auto Detected"));
				firstEncIndex = 1;
			} else
				firstEncIndex = 0;

			foreach (var textEncoding in TextEncoding.ConversionEncodings) {
				var enc = textEncoding.Encoding;
				encodingMenu.AppendText (enc.EncodingName + " (" + enc.WebName + ")");
			}

			encodingMenu.AppendText (GettextCatalog.GetString ("Add or Remove..."));
			selectOption = firstEncIndex + TextEncoding.ConversionEncodings.Length;
			encodingMenu.Active = 0;
			encodingMenu.Changed += EncodingChanged;
		}

		void EncodingChanged (object s, EventArgs args)
		{
			if (encodingMenu.Active == selectOption) {
				using (var dlg = new SelectEncodingsDialog ())
					MessageService.ShowCustomDialog (dlg, this);
				FillEncodings ();
			}
		}

		void FillViewers ()
		{
			viewerSelector.Changed -= OnViewerChanged;
			viewerSelector.RemoveAll ();
			currentViewers.Clear ();

			try {
				var filename = FirstSelectedFile;
				if (filename.Length == 0 || System.IO.Directory.Exists (filename))
					return;

				int selected = -1;
				int i = 0;

				if (IdeServices.ProjectService.IsWorkspaceItemFile (filename) || IdeServices.ProjectService.IsSolutionItemFile (filename)) {
					viewerSelector.AppendText (GettextCatalog.GetString ("Solution Workbench"));
					currentViewers.Add (null);

					if (closeWorkspaceCheck.Visible)
						closeWorkspaceCheck.Active = true;

					// Default exe/dll to AssemblyBrowser, solutions/projects to Solution Workbench.
					// HACK: Couldn't make it a generic SolutionItemFile based conditional, .csproj fits under this category also.
					if (!(filename.EndsWith (".exe", StringComparison.OrdinalIgnoreCase) || filename.EndsWith (".dll", StringComparison.OrdinalIgnoreCase)))
						selected = 0;
					i++;
				}

				foreach (FileViewer vw in IdeServices.DisplayBindingService.GetFileViewers (filename, null).Result) {
					if (!vw.IsExternal) {
						viewerSelector.AppendText (vw.Title);
						currentViewers.Add (vw);

						if (vw.CanUseAsDefault && selected == -1)
							selected = i;

						i++;
					}
				}

				if (selected == -1)
					selected = 0;

				viewerSelector.Active = selected;
				viewerLabel.Sensitive = viewerSelector.Sensitive = currentViewers.Count > 1;
			} finally {
				viewerSelector.Changed += OnViewerChanged;
			}
		}

		void OnViewerChanged (object s, EventArgs args)
		{
			UpdateExtraWidgets ();
		}

		void UpdateExtraWidgets ()
		{
			var filename = FirstSelectedFile;
			if (filename.Length == 0 || System.IO.Directory.Exists (filename)) {
				encodingLabel.Sensitive = encodingMenu.Sensitive = false;
				viewerLabel.Sensitive = viewerSelector.Sensitive = false;
				closeWorkspaceCheck.Visible = false;
				return;
			}

			if (IdeServices.ProjectService.IsWorkspaceItemFile (filename) || IdeServices.ProjectService.IsSolutionItemFile (filename)) {
				encodingLabel.Sensitive = encodingMenu.Sensitive = (SelectedViewer != null);
				closeWorkspaceCheck.Visible = viewerLabel.Visible && IdeApp.Workspace.IsOpen;
			}
			else {
				encodingLabel.Sensitive = encodingMenu.Sensitive = true;
				closeWorkspaceCheck.Visible = false;
			}

			viewerLabel.Sensitive = viewerSelector.Sensitive = currentViewers.Count > 1;
		}

		public FileViewer SelectedViewer {
			get {
				int active = viewerSelector.Active;
				if (active < 0 || active >= currentViewers.Count)
					return null;
				return currentViewers [active];
			}
		}

		void OnSelectionChanged ()
		{
			if (extraWidget == null || this.Action != Gtk.FileChooserAction.Open)
				return;

			UpdateExtraWidgets ();
			FillViewers ();
		}

		public override void Destroy ()
		{
			OnDialogUnmapped (this, EventArgs.Empty);
			base.Destroy ();
		}
	}
}

#pragma warning restore 612
