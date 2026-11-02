using System;
using System.Collections;
using System.Collections.Generic;

using Gtk;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using System.Threading.Tasks;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal class DirtyFilesDialog : IdeDialog
	{
		Button btnSaveAndQuit;
		Button btnQuit;
		Button btnCancel;
		TreeView tvFiles;
		TreeStore tsFiles;
		CellRendererToggle togRender;
		CellRendererText textRender;

		public DirtyFilesDialog () : this (IdeApp.Workbench.Documents, true, true)
		{
		}

		public DirtyFilesDialog (IReadOnlyList<Document> docs, bool closeWorkspace, bool groupByProject) :
			base (GettextCatalog.GetString ("Save Files"), IdeApp.Workbench.RootWindow, DialogFlags.Modal)
		{
			this.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles", (string)null, null);

			string description;
			if (closeWorkspace) {
				description = GettextCatalog.GetString ("Select which files should be saved before closing the workspace");
			} else {
				description = GettextCatalog.GetString ("Select which files should be saved before quitting the application");
			}
			this.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles", (string)null, description);

			tsFiles = new TreeStore (typeof(string), typeof(bool), typeof(Document), typeof(bool));
			tvFiles = new TreeView (tsFiles);
			TreeIter topCombineIter = TreeIter.Zero;
			Hashtable projectIters = new Hashtable ();

			tvFiles.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles.FileList", GettextCatalog.GetString ("Dirty Files"), GettextCatalog.GetString ("The list of files which have changes and need saving"));
			foreach (Document doc in docs) {
				if (!doc.IsDirty)
					continue;
				
				if (groupByProject && doc.Owner != null) {
					TreeIter projIter = TreeIter.Zero;
					if (projectIters.ContainsKey (doc.Owner))
						projIter = (TreeIter)projectIters [doc.Owner];
					else {
						if (topCombineIter.Equals (TreeIter.Zero))
							projIter = tsFiles.AppendValues (GettextCatalog.GetString ("Project: {0}", doc.Owner.Name), true, null, false);
						else
							projIter = tsFiles.AppendValues (topCombineIter, GettextCatalog.GetString ("Project: {0}", doc.Owner.Name), true, null, false);
						projectIters [doc.Owner] = projIter;
					}
					var name = doc.FilePath.IsNullOrEmpty ? doc.Name : doc.FilePath.ToRelative (doc.Owner.BaseDirectory).ToString ();
					tsFiles.AppendValues (projIter, name, true, doc);
				} else {
					tsFiles.AppendValues (doc.Name, true, doc);
				}
			}
			if (!topCombineIter.Equals (TreeIter.Zero)) {
				if (!tsFiles.IterHasChild (topCombineIter))
					tsFiles.Remove (ref topCombineIter); 
			}

			TreeViewColumn mainColumn = new TreeViewColumn ();
			mainColumn.Title = "header";
			
			togRender = new CellRendererToggle ();
			togRender.Toggled += toggled;
			mainColumn.PackStart (togRender, false);
			mainColumn.AddAttribute (togRender, "active", 1);
			mainColumn.AddAttribute (togRender, "inconsistent", 3);
			
			textRender = new CellRendererText ();
			mainColumn.PackStart (textRender, true);
			mainColumn.AddAttribute (textRender, "text", 0);

			tvFiles.AppendColumn (mainColumn);
			tvFiles.HeadersVisible = false;
			tvFiles.ExpandAll ();

			ScrolledWindow sc = new ScrolledWindow ();
			sc.Child = tvFiles;
			sc.HasFrame = true;
			sc.Hexpand = true;
			sc.Vexpand = true;
			DesignerLayout.Margin (sc, 6);
			DesignerLayout.Pack (this.ContentArea, sc, true);

			btnSaveAndQuit = DesignerLayout.Button (closeWorkspace ? GettextCatalog.GetString ("_Save and Quit") : GettextCatalog.GetString ("_Save and Close"));

			if (closeWorkspace) {
				description = GettextCatalog.GetString ("Save the selected files and close the workspace");
			} else {
				description = GettextCatalog.GetString ("Save the selected files and quit the application");
			}
			btnSaveAndQuit.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles.SaveAndQuit", (string)null, description);

			btnQuit = DesignerLayout.Button (closeWorkspace ? GettextCatalog.GetString ("_Quit") : GettextCatalog.GetString ("_Close"));
			if (closeWorkspace) {
				description = GettextCatalog.GetString ("Close the workspace");
			} else {
				description = GettextCatalog.GetString ("Quit the application");
			}
			btnQuit.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles.Quit", (string)null, description);

			btnCancel = DesignerLayout.Button (GettextCatalog.GetString ("_Cancel"));
			if (closeWorkspace) {
				description = GettextCatalog.GetString ("Cancel closing the workspace");
			} else {
				description = GettextCatalog.GetString ("Cancel quitting the application");
			}
			btnCancel.SetCommonAccessibilityAttributes ("Dialog.DirtyFiles.Cancel", (string)null, description);

			btnSaveAndQuit.Clicked += SaveAndQuit;
			btnQuit.Clicked += Quit;
			btnCancel.Clicked += Cancel;

			// The buttons respond on their own (SaveAndQuit saves first), so they are not response action widgets
			var buttonBar = DialogButtonBar.Add (this);
			DesignerLayout.Pack (buttonBar, btnCancel);
			DesignerLayout.Pack (buttonBar, btnQuit);
			DesignerLayout.Pack (buttonBar, btnSaveAndQuit);
			this.SetDefaultSize (300, 200);
		}

		public override void Destroy ()
		{
			btnSaveAndQuit.Clicked -= SaveAndQuit;
			btnQuit.Clicked -= Quit;
			btnCancel.Clicked -= Cancel;
			if (togRender != null) {
				togRender.Toggled -= toggled;
				togRender = null;
			}
			if (textRender != null) {
				textRender = null;
			}
			base.Destroy ();
		}
		
		async void SaveAndQuit (object o, EventArgs e)
		{
			Sensitive = false;

			List<Task> saveTasks = new List<Task> ();
			tsFiles.Foreach (delegate (ITreeModel model, TreePath path, TreeIter iter) {
				var doc = tsFiles.GetValue (iter, 2) as Document;
				if (doc == null)
					return false;
				if ((bool)tsFiles.GetValue (iter, 1))
					saveTasks.Add (doc.Save ());
				return false;
			});

			try {
				await Task.WhenAll (saveTasks);
			} finally {
				Sensitive = true;
			}
	
			Respond (Gtk.ResponseType.Ok);
			Hide ();
		}

		void Quit (object o, EventArgs e)
		{
			Respond (Gtk.ResponseType.Ok);
			Hide ();
		}

		void Cancel (object o, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
			Hide ();
		}

		void toggled (object o, ToggledArgs e)
		{
			TreeIter iter;
			tsFiles.GetIterFromString (out iter, e.Path);
			bool newsetting = !(bool)tsFiles.GetValue (iter, 1);
			tsFiles.SetValue (iter, 1, newsetting);
			if (tsFiles.IterHasChild (iter))
				ToggleChildren (iter, newsetting);
			
			TreeIter iterFirst;
			tsFiles.GetIterFirst (out iterFirst);
			if (tsFiles.IterHasChild (iterFirst))
				NewCheckStatus (iterFirst);
		}

		void NewCheckStatus (TreeIter iter)
		{
			if (tsFiles.IterHasChild (iter)) {
				TreeIter childIter;
				tsFiles.IterNthChild (out childIter, iter, 0);
				if (tsFiles.IterHasChild (childIter))
					NewCheckStatus (childIter);
				bool lastsetting = (bool)tsFiles.GetValue (childIter, 1);
				bool inconsistant = (bool)tsFiles.GetValue (childIter, 3);
				bool anytrue;
				anytrue = lastsetting;
				while (tsFiles.IterNext (ref childIter)) {
					if (tsFiles.IterHasChild (childIter))
						NewCheckStatus (childIter);
					bool newsetting = (bool)tsFiles.GetValue (childIter, 1);
					if (newsetting != lastsetting || (bool)tsFiles.GetValue (childIter, 3) == true)
						inconsistant = true;
					if (newsetting)
						anytrue = true;
					lastsetting = newsetting;
				}
				
				tsFiles.SetValue (iter, 3, inconsistant);
				tsFiles.SetValue (iter, 1, anytrue);
			}
		}

		void ToggleChildren (TreeIter iter, bool setting)
		{
			TreeIter newIter;
			tsFiles.IterNthChild (out newIter, iter, 0);
			do {
				tsFiles.SetValue (newIter, 1, setting);
				if (tsFiles.IterHasChild (newIter))
					ToggleChildren (newIter, setting);
			}
			while (tsFiles.IterNext (ref newIter));
		}
	}
}
