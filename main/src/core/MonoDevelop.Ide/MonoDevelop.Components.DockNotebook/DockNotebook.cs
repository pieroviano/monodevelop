// DragNotebook.cs
//
// Author:
//   Todd Berman  <tberman@off.net>
//
// Copyright (c) 2004 Todd Berman
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

using Gdk;
using Gtk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using MonoDevelop.Ide;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Components.Docking;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Shell;

namespace MonoDevelop.Components.DockNotebook
{
	delegate void TabsReorderedHandler (DockNotebookTab tab, int oldPlacement, int newPlacement);

	class DockNotebook : Gtk.Box, IShellNotebook
	{
		List<DockNotebookTab> pages = new List<DockNotebookTab> ();
		List<DockNotebookTab> pagesHistory = new List<DockNotebookTab> ();
		TabStrip tabStrip;
		BinWidget contentBox;
		ReadOnlyCollection<DockNotebookTab> pagesCol;
		const int MAX_LASTACTIVEWINDOWS = 10;

		DockNotebookTab currentTab;

		static DockNotebook activeNotebook;
		static List<DockNotebook> allNotebooks = new List<DockNotebook> ();

		public static event EventHandler ActiveNotebookChanged;
		public static event EventHandler NotebookChanged;

		public DockNotebook () : base (Gtk.Orientation.Vertical, 0)
		{
			pagesCol = new ReadOnlyCollection<DockNotebookTab> (pages);

			tabStrip = new TabStrip (this);

			Append (tabStrip);

			contentBox = new BinWidget ();
			contentBox.SetShouldIgnore (true);
			contentBox.Hexpand = true;
			contentBox.Vexpand = true;
			Append (contentBox);

			tabStrip.DropDownButton.Sensitive = false;

			tabStrip.DropDownButton.ContextMenuRequested = delegate {
				ContextMenu menu = new ContextMenu ();
				foreach (var tab in pages) {
					var item = new ContextMenuItem (tab.Markup ?? tab.Text);
					var locTab = tab;
					item.Clicked += (object sender, ContextMenuItemClickedEventArgs e) => {
						CurrentTab = locTab;
					};
					menu.Items.Add (item);
				}
				return menu;
			};

			// Files dropped on the notebook are opened
			var dropTarget = new Gtk.DropTarget (Gdk.FileList.GType, Gdk.DragAction.Copy);
			dropTarget.Dropped += OnFilesDropped;
			dropTarget.Enter += (o, args) => {
				// Bring this window to the front. Otherwise, the drop may end being done in another window that overlaps this one
				if (!Platform.IsWindows) {
					var window = this.GetToplevelWindow ();
					if (window is DockWindow)
						window.Present ();
				}
				args.RetVal = Gdk.DragAction.Copy;
			};
			AddController (dropTarget);

			// A press anywhere in the notebook (even inside the content) activates it
			this.AddClickHandler (c => ActiveNotebook = this, phase: PropagationPhase.Capture);

			allNotebooks.Add (this);
		}

		public static DockNotebook ActiveNotebook {
			get { return activeNotebook; }
			set {
				if (activeNotebook != value) {
					if (activeNotebook != null)
						activeNotebook.tabStrip.IsActiveNotebook = false;
					activeNotebook = value;
					if (activeNotebook != null)
						activeNotebook.tabStrip.IsActiveNotebook = true;
					if (ActiveNotebookChanged != null)
						ActiveNotebookChanged (null, EventArgs.Empty);
				}
			}
		}

		public static IEnumerable<DockNotebook> AllNotebooks {
			get { return allNotebooks; }
		}

		public event TabsReorderedHandler TabsReordered;
		public event EventHandler<TabEventArgs> TabClosed;
		public event EventHandler<TabEventArgs> TabPinned;
		public event EventHandler<TabEventArgs> TabActivated;

		public event EventHandler<TabEventArgs> PageAdded;
		public event EventHandler<TabEventArgs> PageRemoved;
		public event EventHandler SwitchPage;

		public event EventHandler PreviousButtonClicked {
			add { tabStrip.PreviousButton.Clicked += value; }
			remove { tabStrip.PreviousButton.Clicked -= value; }
		}

		public event EventHandler NextButtonClicked {
			add { tabStrip.NextButton.Clicked += value; }
			remove { tabStrip.NextButton.Clicked -= value; }
		}

		public bool PreviousButtonEnabled {
			get { return tabStrip.PreviousButton.Sensitive; }
			set { tabStrip.PreviousButton.Sensitive = value; }
		}

		public bool NextButtonEnabled {
			get { return tabStrip.NextButton.Sensitive; }
			set { tabStrip.NextButton.Sensitive = value; }
		}

		public bool NavigationButtonsVisible {
			get { return tabStrip.NavigationButtonsVisible; }
			set { tabStrip.NavigationButtonsVisible = value; }
		}

		public ReadOnlyCollection<DockNotebookTab> Tabs {
			get { return pagesCol; }
		}

		public DockNotebookTab CurrentTab {
			get { return currentTab; }
			set {
				if (currentTab != value) {
					currentTab = value;
					contentBox.Child = null;

					if (currentTab != null) {
						if (currentTab.Content != null) {
							contentBox.Child = currentTab.Content;
							// Focus the last child, as some editors like the JSON one have a dropdown at the top.
							contentBox.ChildFocus (DirectionType.Up);
						}
						pagesHistory.Remove (currentTab);
						pagesHistory.Insert (0, currentTab);
						if (pagesHistory.Count > MAX_LASTACTIVEWINDOWS)
							pagesHistory.RemoveAt (pagesHistory.Count - 1);
					}

					tabStrip.Update ();

					if (SwitchPage != null)
						SwitchPage (this, EventArgs.Empty);
				}
			}
		}

		public int CurrentTabIndex {
			get { return currentTab != null ? currentTab.Index : -1; }
			set {
				if (value > pages.Count - 1)
					CurrentTab = null;
				else
					CurrentTab = pages [value];
			}
		}

		void SelectLastActiveTab (int lastClosed)
		{
			if (pages.Count == 0) {
				CurrentTab = null;
				return;
			}

			while (pagesHistory.Count > 0 && pagesHistory [0].Content == null)
				pagesHistory.RemoveAt (0);

			if (pagesHistory.Count > 0)
				CurrentTab = pagesHistory [0];
			else {
				if (lastClosed + 1 < pages.Count)
					CurrentTab = pages [lastClosed + 1];
				else
					CurrentTab = pages [lastClosed - 1];
			}
		}

		public int TabCount {
			get { return pages.Count; }
		}

		public int BarHeight {
			get { return tabStrip.BarHeight; }
		}

		internal void InitSize ()
		{
			tabStrip.InitSize ();
		}

		async void OnFilesDropped (object o, Gtk.DroppedArgs args)
		{
			var fileList = args.Value.Val as Gdk.FileList;
			if (fileList == null) {
				args.RetVal = false;
				return;
			}

			var loadWorkspaceItems = !IsInsideTabStrip (args.X, args.Y);
			var files = new List<Ide.Gui.FileOpenInformation> ();

			var droppedFiles = new GLib.SList (fileList.Files.Handle, typeof (GLib.IFile), false, false);
			foreach (GLib.IFile droppedFile in droppedFiles) {
				string path = droppedFile.Path;
				if (string.IsNullOrEmpty (path))
					continue;

				var filePath = new FilePath (path);
				if (filePath.IsDirectory) {
					if (!loadWorkspaceItems) // skip directories when not loading solutions
						continue;
					filePath = Directory.EnumerateFiles (filePath).FirstOrDefault (p => Services.ProjectService.IsWorkspaceItemFile (p));
				}
				if (!filePath.IsNullOrEmpty) // skip empty paths
					files.Add (new Ide.Gui.FileOpenInformation (filePath, null, 0, 0, Ide.Gui.OpenDocumentOptions.DefaultInternal) { DockNotebook = this });
			}

			args.RetVal = files.Count > 0;

			if (files.Count > 0) {
				if (loadWorkspaceItems) {
					try {
						IdeApp.OpenFiles (files);
					} catch (Exception e) {
						LoggingService.LogError ($"Failed to open dropped files", e);
					}
				} else { // open workspace items as files
					foreach (var file in files) {
						try {
							await IdeApp.Workbench.OpenDocument (file).ConfigureAwait (false);
						} catch (Exception e) {
							LoggingService.LogError ($"unable to open file {file}", e);
						}
					}
				}
			}
		}

		bool IsInsideTabStrip (double pointerX, double pointerY)
		{
			if (tabStrip?.IsRealized != true)
				return false;
			if (!TranslateCoordinates (tabStrip, pointerX, pointerY, out double tabX, out double tabY))
				return false;
			return tabX >= 0 && tabY >= 0 && tabX < tabStrip.Width && tabY < tabStrip.Height;
		}

		public DockNotebookContainer Container {
			get {
				var container = (DockNotebookContainer)Parent;
				return container.MotherContainer () ?? container;
			}
		}

		/// <summary>
		/// Returns the next notebook in the same window
		/// </summary>
		public DockNotebook GetNextNotebook ()
		{
			return Container.GetNextNotebook (this);
		}

		/// <summary>
		/// Returns the previous notebook in the same window
		/// </summary>
		public DockNotebook GetPreviousNotebook ()
		{
			return Container.GetPreviousNotebook (this);
		}

		/// <summary>Shows the context menu of a tab: (notebook, tab index, click that requested the menu).</summary>
		public Action<DockNotebook, int, ClickInfo> DoPopupMenu { get; set; }

		public DockNotebookTab AddTab (Gtk.Widget content = null)
		{
			var t = InsertTab (-1);
			if (content != null)
				t.Content = content;
			return t;
		}

		public DockNotebookTab InsertTab (int index)
		{
			var tab = new DockNotebookTab (this, tabStrip);
			if (index == -1) {
				pages.Add (tab);
				tab.Index = pages.Count - 1;
			} else {
				pages.Insert (index, tab);
				tab.Index = index;
				UpdateIndexes (index + 1);
			}

			pagesHistory.Add (tab);

			if (pages.Count == 1)
				CurrentTab = tab;

			tabStrip.StartOpenAnimation ((DockNotebookTab)tab);
			tabStrip.Update ();
			tabStrip.DropDownButton.Sensitive = pages.Count > 0;

			PageAdded?.Invoke (this, new TabEventArgs { Tab = tab, });

			tab.OnChangingPinned = OnTabPinned;

			NotebookChanged?.Invoke (this, EventArgs.Empty);

			return tab;
		}

		void UpdateIndexes (int startIndex)
		{
			for (int n=startIndex; n < pages.Count; n++)
				((DockNotebookTab)pages [n]).Index = n;
		}

		void OnTabPinned (DockNotebookTab sender, bool value)
		{
			if (pages.Count == 1)
				return;

			var stickedPages = pages.Where (p => p.IsPinned);
			var normalPages = pages.Where (p => !p.IsPinned);

			if (value) {
				if (stickedPages.Any ())
					ReorderTab (sender, normalPages.MinValueOrDefault (s => s.Index) ?? stickedPages.MaxValueOrDefault (s => s.Index), false);
				 else
					ReorderTab (sender, pages.FirstOrDefault (), false);
			} else {
				ReorderTab (sender, stickedPages.MaxValueOrDefault (s => s.Index) ?? normalPages.MinValueOrDefault (s => s.Index), false);
			}
		}

		public DockNotebookTab GetTab (int n)
		{
			if (n < 0 || n >= pages.Count)
				return null;
			else
				return pages [n];
		}

		public void RemoveTab (int page, bool animate)
		{
			var tab = pages [page];
			if (animate)
				tabStrip.StartCloseAnimation ((DockNotebookTab)tab);
			pagesHistory.Remove (tab);
			if (pages.Count == 1)
				CurrentTab = null;
			else if (page == CurrentTabIndex)
				SelectLastActiveTab (page);
			pages.RemoveAt (page);
			UpdateIndexes (page);
			tabStrip.Update ();
			tabStrip.DropDownButton.Sensitive = pages.Count > 0;

			PageRemoved?.Invoke (this, new TabEventArgs { Tab = tab });

			NotebookChanged?.Invoke (this, EventArgs.Empty);
		}

		internal void ReorderTab (DockNotebookTab tab, DockNotebookTab targetTab, bool pinCheck = true)
		{
			if (pinCheck && tab.IsPinned != targetTab.IsPinned)
				return;

			if (tab == targetTab)
				return;
			int targetPos = targetTab.Index;
			if (tab.Index > targetTab.Index) {
				pages.RemoveAt (tab.Index);
				pages.Insert (targetPos, tab);
			} else {
				pages.Insert (targetPos + 1, tab);
				pages.RemoveAt (tab.Index);
			}
			if (TabsReordered != null)
				TabsReordered (tab, tab.Index, targetPos);
			UpdateIndexes (Math.Min (tab.Index, targetPos));
			tabStrip.Update ();
		}

		internal void OnCloseTab (DockNotebookTab tab)
		{
			if (TabClosed != null)
				TabClosed (this, new TabEventArgs () { Tab = tab });
		}

		internal void OnPinTab (DockNotebookTab tab)
		{
			if (TabPinned != null)
				TabPinned (this, new TabEventArgs () { Tab = tab });
		}

		internal void OnActivateTab (DockNotebookTab tab)
		{
			if (TabActivated != null)
				TabActivated (this, new TabEventArgs () { Tab = tab });
		}

		internal void ShowContent (DockNotebookTab tab)
		{
			if (tab == currentTab)
				contentBox.Child = tab.Content;
		}

		Gdk.Rectangle? dropPlaceholder;

		/// <summary>
		/// Shows (or hides, when null) the outline of the place where a dragged tab would be dropped.
		/// The rectangle is in the coordinates of this notebook.
		/// </summary>
		internal void ShowDropPlaceholder (Gdk.Rectangle? rect)
		{
			if (dropPlaceholder == rect)
				return;
			dropPlaceholder = rect;
			QueueDraw ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot);
			if (dropPlaceholder != null) {
				var r = dropPlaceholder.Value;
				snapshot.DrawCairo (new Gdk.Rectangle (0, 0, Width, Height), ctx => {
					ctx.SetSourceColor (new Cairo.Color (0.17, 0.55, 0.79, 0.4));
					ctx.Rectangle (r.X, r.Y, r.Width, r.Height);
					ctx.Fill ();
				});
			}
		}

		protected override void OnRoot ()
		{
			base.OnRoot ();
			if (!allNotebooks.Contains (this))
				allNotebooks.Add (this);
		}

		protected override void OnUnroot ()
		{
			allNotebooks.Remove (this);
			base.OnUnroot ();
		}

		/// <summary>Called when the notebook is destroyed.</summary>
		protected virtual void OnDestroyed ()
		{
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				allNotebooks.Remove (this);

				if (ActiveNotebook == this)
					ActiveNotebook = null;
				OnDestroyed ();
			}
			base.Dispose (disposing);
		}
	}

	class DockNotebookChangedArgs : EventArgs
	{
		public DockNotebook Notebook { get; private set; }
		public DockNotebookChangedArgs (DockNotebook notebook)
		{
			Notebook = notebook;
		}
	}
}
