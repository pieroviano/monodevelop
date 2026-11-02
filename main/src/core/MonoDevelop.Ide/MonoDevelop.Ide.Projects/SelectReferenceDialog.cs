// SelectReferenceDialog.cs
//  
// Author:
//       Todd Berman <tberman@sevenl.net>
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2004 Todd Berman
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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
using System.Linq;

using MonoDevelop.Projects;
using MonoDevelop.Core;

using Gtk;
using SearchEntry = MonoDevelop.Components.SearchEntry;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Components.Commands;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoDevelop.Ide.Projects
{
	internal interface IReferencePanel
	{
		void SetProject (DotNetProject configureProject);
		void SignalRefChange (ProjectReference refInfo, bool newState);
		void SetFilter (string filter);
	}
	
	internal partial class SelectReferenceDialog: Gtk.Dialog
	{
		ListStore refTreeStore;
		
		PackageReferencePanel packageRefPanel;
		PackageReferencePanel allRefPanel;
		ProjectReferencePanel projectRefPanel;
		AssemblyReferencePanel assemblyRefPanel;
		DotNetProject configureProject;
		List<IReferencePanel> panels = new List<IReferencePanel> ();
		Notebook mainBook;
		CombinedBox combinedBox;
		SearchEntry filterEntry;

		Dictionary<FilePath,List<FilePath>> recentFiles;
		bool recentFilesModified = false;

		static FilePath RecentAssembliesFile = UserProfile.Current.CacheDir.Combine ("RecentAssemblies2.txt");
		const int RecentFileListSize = 75;
		
		const int NameColumn = 0;
		const int SecondaryNameColumn = 1;
		const int TypeNameColumn = 2;
		const int LocationColumn = 3;
		const int ProjectReferenceColumn = 4;
		const int IconColumn = 5;
		
		public ProjectReferenceCollection ReferenceInformations {
			get {
				ProjectReferenceCollection referenceInformations = new ProjectReferenceCollection();
				Gtk.TreeIter looping_iter;
				if (!refTreeStore.GetIterFirst (out looping_iter)) {
					return referenceInformations;
				}
				do {
					referenceInformations.Add ((ProjectReference) refTreeStore.GetValue(looping_iter, ProjectReferenceColumn));
				} while (refTreeStore.IterNext (ref looping_iter));
				return referenceInformations;
			}
		}

		protected void OnMainBookSwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			filterEntry.Sensitive = args.PageNum != 3;
		}
		
		public void SetProject (DotNetProject configureProject)
		{
			this.configureProject = configureProject;
			foreach (var p in panels)
				p.SetProject (configureProject);

			((ListStore) ReferencesTreeView.Model).Clear ();

			if (configureProject == null)
				return;

			foreach (ProjectReference refInfo in configureProject.References)
				AppendReference (refInfo);

			OnChanged (null, null);
		}
		
		TreeIter AppendReference (ProjectReference refInfo)
		{
			foreach (var p in panels)
				p.SignalRefChange (refInfo, true);
			
			switch (refInfo.ReferenceType) {
				case ReferenceType.Assembly:
					return AddAssemplyReference (refInfo);
				case ReferenceType.Project:
					return AddProjectReference (refInfo);
				case ReferenceType.Package:
					return AddPackageReference (refInfo);
				default:
					return TreeIter.Zero;
			}
		}

		TreeIter AddAssemplyReference (ProjectReference refInfo)
		{
			string txt = GLib.Markup.EscapeText (System.IO.Path.GetFileName (refInfo.Reference));
			string secondaryTxt = GLib.Markup.EscapeText (System.IO.Path.GetFullPath (refInfo.HintPath));
			return refTreeStore.AppendValues (txt, secondaryTxt, GetTypeText (refInfo), System.IO.Path.GetFullPath (refInfo.Reference), refInfo, ImageService.GetIcon ("md-empty-file-icon", StockIconSize.Dnd));
		}

		TreeIter AddProjectReference (ProjectReference refInfo)
		{
			Solution c = configureProject.ParentSolution;
			if (c == null) return TreeIter.Zero;

			Project p = refInfo.ResolveProject (c);
			if (p == null) return TreeIter.Zero;

			string txt = GLib.Markup.EscapeText (System.IO.Path.GetFileName (refInfo.Reference));
			string secondaryTxt = GLib.Markup.EscapeText (p.BaseDirectory.ToString ());
			return refTreeStore.AppendValues (txt, secondaryTxt, GetTypeText (refInfo), p.BaseDirectory.ToString (), refInfo, ImageService.GetIcon ("md-project", StockIconSize.Dnd));
		}

		TreeIter AddPackageReference (ProjectReference refInfo)
		{
			string txt = GLib.Markup.EscapeText (System.IO.Path.GetFileNameWithoutExtension (refInfo.Reference));
			string secondaryTxt = string.Empty;
			int i = refInfo.Reference.IndexOf (',');
			if (i != -1) {
				txt = GLib.Markup.EscapeText (txt.Substring (0, i));
				secondaryTxt = GLib.Markup.EscapeText (refInfo.Reference.Substring (i + 1).Trim ());
			}
			return refTreeStore.AppendValues (txt, secondaryTxt, GetTypeText (refInfo), refInfo.Reference, refInfo, ImageService.GetIcon ("md-package", StockIconSize.Dnd));
		}

		public SelectReferenceDialog ()
		{
			Build ();

			combinedBox = new CombinedBox ();
			mainBook = new Notebook { Vexpand = true, Hexpand = true };

			mainBook.SetActionWidget (combinedBox, PackType.End);

			alignment1.Append (mainBook);

			// Ctrl+F (Cmd+F on Mac) moves the focus to the filter entry
			this.AddKeyPressHandler (HandleKeyPressed, phase: PropagationPhase.Capture);

			Unmapped += (sender, e) => {
				SaveRecentFiles ();
				recentFiles = null;
			};

			filterEntry = combinedBox.FilterEntry;

			boxRefs.WidthRequest = 200;
			
			refTreeStore = new ListStore (typeof (string), typeof(string), typeof(string), typeof(string), typeof(ProjectReference), typeof(Xwt.Drawing.Image));
			ReferencesTreeView.Model = refTreeStore;

			TreeViewColumn col = new TreeViewColumn ();
			col.Title = GettextCatalog.GetString("Reference");
			CellRendererImage crp = new CellRendererImage ();
			crp.Yalign = 0f;
			col.PackStart (crp, false);
			col.AddAttribute (crp, "image", IconColumn);
			var text_render = new CustomSelectedReferenceCellRenderer ();
			col.PackStart (text_render, true);
			col.AddAttribute (text_render, "text-markup", NameColumn);
			col.AddAttribute (text_render, "secondary-text-markup", SecondaryNameColumn);
			text_render.Ellipsize = Pango.EllipsizeMode.End;
			
			ReferencesTreeView.AppendColumn (col);
//			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Type"), new CellRendererText (), "text", TypeNameColumn);
//			ReferencesTreeView.AppendColumn (GettextCatalog.GetString ("Location"), new CellRendererText (), "text", LocationColumn);
			
			projectRefPanel = new ProjectReferencePanel (this);
			packageRefPanel = new PackageReferencePanel (this, false);
			allRefPanel = new PackageReferencePanel (this, true);
			assemblyRefPanel = new AssemblyReferencePanel (this);
			panels.Add (allRefPanel);
			panels.Add (packageRefPanel);
			panels.Add (projectRefPanel);
			panels.Add (assemblyRefPanel);
			
			//mainBook.RemovePage (mainBook.CurrentPage);
			
			var tab = CreateTabLabel (GettextCatalog.GetString ("_All"));
			mainBook.AppendPage (allRefPanel, tab);
			
			tab = CreateTabLabel (GettextCatalog.GetString ("_Packages"));
			mainBook.AppendPage (packageRefPanel, tab);
			
			tab = CreateTabLabel (GettextCatalog.GetString ("Pro_jects"));
			mainBook.AppendPage (projectRefPanel, tab);
			
			tab = CreateTabLabel (GettextCatalog.GetString (".Net A_ssembly"));
			mainBook.AppendPage (assemblyRefPanel, tab);
			
			mainBook.Page = 0;
			
			var w = selectedHeader.FirstChild;
			selectedHeader.Remove (w);
			HeaderBox header = new HeaderBox (1, 0, 1, 1);
			header.SetPadding (6, 6, 6, 6);
			header.GradientBackground = true;
			header.Child = w;
			selectedHeader.Append (header);
			
			RemoveReferenceButton.CanFocus = true;
			ReferencesTreeView.Selection.Changed += new EventHandler (OnChanged);
			OnChanged (null, null);
			InsertFilterEntry ();
		}
		
		void InsertFilterEntry ()
		{
			filterEntry.Activated += HandleFilterEntryActivated;
			filterEntry.Changed += delegate {
				foreach (var p in panels)
					p.SetFilter (filterEntry.Query);
			};
		}

		static Box CreateTabLabel (string text)
		{
			var tab = new Box (Orientation.Horizontal, 3);
			tab.MarginStart = tab.MarginEnd = tab.MarginTop = tab.MarginBottom = 3;
			tab.Append (new Label (text) { UseUnderline = true, Hexpand = true });
			return tab;
		}

		void HandleFilterEntryActivated (object sender, EventArgs e)
		{
			mainBook.GrabFocus ();
			mainBook.ChildFocus (DirectionType.TabForward);
		}

		bool HandleKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			bool complete;
			var accels = KeyBindingManager.AccelsFromKey (keyval, keycode, state, out complete);
			if (complete && accels.Any (a => a.Equals (combinedBox.SearchKeyShortcut))) {
				filterEntry.GrabFocusEntry ();
				return false;
			}

			return false;
		}

		void OnChanged (object o, EventArgs e)
		{
			if (ReferencesTreeView.Selection.CountSelectedRows () > 0)
				RemoveReferenceButton.Sensitive = true;
			else
				RemoveReferenceButton.Sensitive = false;
		}
		
		string GetTypeText (ProjectReference pref)
		{
			switch (pref.ReferenceType) {
				case ReferenceType.Package: return GettextCatalog.GetString ("Package");
				case ReferenceType.Assembly: return GettextCatalog.GetString ("Assembly");
				case ReferenceType.Project: return GettextCatalog.GetString ("Project");
				default: return "";
			}
		}

		public void RemoveReference (ReferenceType referenceType, string reference)
		{
			TreeIter iter = FindReference (referenceType, reference);
			if (iter.Equals (TreeIter.Zero))
				return;
			refTreeStore.Remove (ref iter);
		}
		
		public void AddReference (ProjectReference pref)
		{
			TreeIter iter = FindReference (pref.ReferenceType, pref.Reference);
			if (!iter.Equals (TreeIter.Zero))
				return;
			
			TreeIter ni = AppendReference (pref);
			if (!ni.Equals (TreeIter.Zero))
				ReferencesTreeView.ScrollToCell (refTreeStore.GetPath (ni), null, false, 0, 0);
		}
		
		TreeIter FindReference (ReferenceType referenceType, string reference)
		{
			TreeIter looping_iter;
			if (refTreeStore.GetIterFirst (out looping_iter)) {
				do {
					ProjectReference pref = (ProjectReference) refTreeStore.GetValue (looping_iter, ProjectReferenceColumn);
					if (pref.Reference == reference && pref.ReferenceType == referenceType) {
						return looping_iter;
					}
				} while (refTreeStore.IterNext (ref looping_iter));
			}
			return TreeIter.Zero;
		}
		
		protected void RemoveReference (object sender, EventArgs e)
		{
			TreeIter iter;
			ITreeModel mdl;
			if (ReferencesTreeView.Selection.GetSelected (out mdl, out iter)) {
				ProjectReference pref = (ProjectReference)refTreeStore.GetValue (iter, ProjectReferenceColumn);
				foreach (var p in panels)
					p.SignalRefChange (pref, false);
				TreeIter newIter = iter;
				if (refTreeStore.IterNext (ref newIter)) {
					ReferencesTreeView.Selection.SelectIter (newIter);
					refTreeStore.Remove (ref iter);
				} else {
					TreePath path = refTreeStore.GetPath (iter);
					if (path.Prev ()) {
						ReferencesTreeView.Selection.SelectPath (path);
						refTreeStore.Remove (ref iter);
					} else {
						refTreeStore.Remove (ref iter);
					}
				}
			}
		}

		protected void OnReferencesTreeViewRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			Respond (ResponseType.Ok);
		}
		
		public void RegisterFileReference (FilePath file, FilePath solutionFile = default (FilePath))
		{
			LoadRecentFiles ();

			if (solutionFile.IsNullOrEmpty)
				solutionFile = FilePath.Empty;
			else
				solutionFile = solutionFile.CanonicalPath;

			List<FilePath> files;
			if (!recentFiles.TryGetValue (solutionFile, out files))
				files = recentFiles [solutionFile] = new List<FilePath> ();
			if (files.Contains (file))
				return;
			recentFilesModified = true;
			files.Add (file);
			if (files.Count > RecentFileListSize)
				files.RemoveAt (0);
		}
		
		public IEnumerable<FilePath> GetRecentFileReferences (FilePath solutionFile = default(FilePath))
		{
			LoadRecentFiles ();
			IEnumerable<FilePath> result = null;
			List<FilePath> list;
			if (recentFiles.TryGetValue (FilePath.Empty, out list))
				result = list;
			if (recentFiles.TryGetValue (solutionFile, out list))
				result = result != null ? result.Concat (list) : list;
			return result ?? new FilePath[0];
		}
		
		void LoadRecentFiles ()
		{
			if (recentFiles != null)
				return;
			
			recentFilesModified = false;
			recentFiles = new Dictionary<FilePath, List<FilePath>> ();

			if (File.Exists (RecentAssembliesFile)) {
				try {
					FilePath solutionPath = FilePath.Empty;
					List<FilePath> list = new List<FilePath> ();
					recentFiles [FilePath.Empty] = list;
					foreach (var line in File.ReadAllLines (RecentAssembliesFile)) {
						if (line.StartsWith ("# ", StringComparison.Ordinal)) {
							solutionPath = (FilePath)line.Substring (2);
							list = recentFiles [solutionPath] = new List<FilePath> ();
							continue;
						}
						list.Add ((FilePath) line);
					}

				}
				catch (Exception ex) {
					LoggingService.LogError ("Error while loading recent assembly files list", ex);
				}
			}
		}
		
		void SaveRecentFiles ()
		{
			if (!recentFilesModified)
				return;
			try {
				using (var sw = new StreamWriter (RecentAssembliesFile)) {
					List<FilePath> files;
					if (recentFiles.TryGetValue (FilePath.Empty, out files))
						SaveRecentFiles (sw, FilePath.Empty, files);
					foreach (var p in recentFiles.Where (e => e.Key != FilePath.Empty))
						SaveRecentFiles (sw, p.Key, p.Value);
				}
			} catch (Exception ex) {
				LoggingService.LogError ("Error while saving recent assembly files list", ex);
			}
			recentFilesModified = false;
		}

		void SaveRecentFiles (StreamWriter sw, FilePath solPath, List<FilePath> files)
		{
			var existing = files.Where (f => File.Exists (f)).ToArray ();
			if (existing.Length == 0)
				return;

			if (!solPath.IsNullOrEmpty) {
				if (!File.Exists (solPath))
					return;
				sw.WriteLine ("# " + solPath);
			}
			foreach (var f in existing)
				sw.WriteLine (f);
		}
		
		public override void Destroy ()
		{
			SaveRecentFiles ();
			recentFiles = null;
			base.Destroy ();
		}
	}

	class CombinedBox: Gtk.Box
	{
		SearchEntry filterEntry;
		KeyboardShortcut searchShortcut;

		public SearchEntry FilterEntry {
			get { return filterEntry; }
		}

		public CombinedBox () : base (Orientation.Horizontal, 0)
		{
			filterEntry = new SearchEntry ();
			filterEntry.WidthRequest = 180;
			filterEntry.Ready = true;
			filterEntry.ForceFilterButtonVisible = true;
			filterEntry.Entry.CanFocus = true;
			filterEntry.RoundedShape = true;
			filterEntry.HasFrame = true;
			Append (filterEntry);

			SearchKeyShortcut = new KeyboardShortcut (
				Gdk.Key.f, Platform.IsMac? Gdk.ModifierType.MetaMask : Gdk.ModifierType.ControlMask
			);
		}

		public KeyboardShortcut SearchKeyShortcut {
			get { return searchShortcut; }
			set {
				searchShortcut = value;
				var bindingLabel = KeyBindingManager.BindingToDisplayLabel (new KeyBinding (searchShortcut), false);
				filterEntry.EmptyMessage = GettextCatalog.GetString ("Search ({0})", bindingLabel);
			}
		}

	}

	class CustomSelectedReferenceCellRenderer : CellRendererText
	{
		Pango.Layout layout;
		string markup;
		string secondarymarkup;

		[GLib.Property ("text-markup")]
		public string TextMarkup {
			get { return markup; }
			set {
				markup = value;
				if (!string.IsNullOrEmpty (secondarymarkup))
					Markup = markup + " " + secondarymarkup;
				else
					Markup = markup;
			}
		}

		[GLib.Property ("secondary-text-markup")]
		public string SecondaryTextMarkup {
			get { return secondarymarkup; }
			set {
				secondarymarkup = value;
				if (!string.IsNullOrEmpty (secondarymarkup))
					Markup = markup + " " + secondarymarkup;
				else
					Markup = markup;
			}
		}

		protected override void OnSnapshot (Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			SetupLayout (widget, flags);

			int w, h;
			layout.GetPixelSize (out w, out h);

			const int textXOffset = 2; // Shift text up slightly in the row.
			int tx = cell_area.X + (int)Xpad;
			int ty = cell_area.Y + (cell_area.Height - h) / 2 - textXOffset;

			int textPixelWidth = cell_area.Width - ((int)Xpad * 2) ;
			layout.Width = (int)(textPixelWidth * Pango.Scale.PangoScale);

			var color = CellRendererDrawing.GetTextColor (widget, flags);
			snapshot.DrawCairo (cell_area, ctx => CellRendererDrawing.DrawLayout (ctx, color, tx, ty, layout));
		}

		protected override void OnGetPreferredHeight (Widget widget, out int minimum_size, out int natural_size)
		{
			base.OnGetPreferredHeight (widget, out minimum_size, out natural_size);
			minimum_size = natural_size = GetLayoutHeight (widget);
		}

		protected override void OnGetPreferredHeightForWidth (Widget widget, int width, out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeightForWidth (widget, width, out minimum_height, out natural_height);
			minimum_height = natural_height = GetLayoutHeight (widget);
		}

		int GetLayoutHeight (Widget widget)
		{
			SetupLayout (widget);

			int layoutWidth = 0;
			int height;
			layout.GetPixelSize (out layoutWidth, out height);
			return height;
		}

		void SetupLayout (Widget widget, CellRendererState flags = 0)
		{
			if (layout == null || layout.Context != widget.PangoContext) {
				if (layout != null)
					layout.Dispose ();
				layout = new Pango.Layout (widget.PangoContext);
				layout.Ellipsize = Ellipsize;
			}

			string newmarkup = TextMarkup;
			if (!string.IsNullOrEmpty (SecondaryTextMarkup)) {
				if (Platform.IsMac && flags.HasFlag (CellRendererState.Selected))
					newmarkup += "\n<span foreground='" + Gui.Styles.SecondarySelectionTextColor.ToHexString (false) + "'><small>" + SecondaryTextMarkup + "</small></span>";
				else
					newmarkup += "\n<span foreground='" + Gui.Styles.SecondaryTextColor.ToHexString (false) + "'><small>" + SecondaryTextMarkup + "</small></span>";
			}

			layout.SetMarkup (newmarkup);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && layout != null) {
				layout.Dispose ();
				layout = null;
			}
			base.Dispose (disposing);
		}
	}
}

