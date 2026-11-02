//
// PlaceholderWindow.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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

using Gdk;
using Gtk;
using MonoDevelop.Components.Docking;
using MonoDevelop.Ide;
using System.Collections.Generic;
using MonoDevelop.Ide.Gui;
using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Shell;

namespace MonoDevelop.Components.DockNotebook
{
	/// <summary>
	/// Drag and drop of a document tab out of its tab strip: shows where the tab would be dropped (an outline over the
	/// notebook under the pointer) and moves the document there when the button is released. Dropping it outside of
	/// every notebook moves the document to a new floating window.
	/// </summary>
	/// <remarks>
	/// GTK 4 windows can't be positioned by the application and coordinates can't be translated between windows, so
	/// the target notebook is searched among the notebooks of the window that contains the tab strip.
	/// </remarks>
	class PlaceholderWindow
	{
		List<DockNotebook> allNotebooks;
		readonly TabStrip source;
		readonly DockNotebookTab frame;
		bool destroyed;
		bool controlPressed;
		EventControllerKey keyController;
		Gtk.Window keyWindow;
		DockNotebook outlinedNotebook;

		/// <summary>Raised when the drag has finished, because the tab was dropped or the drag was canceled.</summary>
		public event EventHandler Destroyed;

		static PlaceholderWindow ()
		{
			IdeApp.Workbench.ActiveDocumentChanged += delegate {
				var doc = IdeApp.Workbench.ActiveDocument;
				if (doc == null)
					return;
				var workspaceWindow = doc.Window as SdiWorkspaceWindow;
				var rootWindow = workspaceWindow?.GetToplevelWindow () as DockWindow;
				if (rootWindow == null)
					return;

				rootWindow.Title = DefaultWorkbench.GetTitle (workspaceWindow);
			};
		}

		public PlaceholderWindow (DockNotebookTab tab, TabStrip source)
		{
			this.frame = tab;
			this.source = source;
			IdeApp.Workbench.LockActiveWindowChangeEvent ();

			var windowStack = IdeApp.CommandService.TopLevelWindowStack.ToArray ();
			allNotebooks = DockNotebook.AllNotebooks.ToList ();
			allNotebooks.Sort (delegate(DockNotebook x, DockNotebook y) {
				var ix = Array.IndexOf (windowStack, x.GetToplevelWindow ());
				var iy = Array.IndexOf (windowStack, y.GetToplevelWindow ());
				if (ix == -1) ix = int.MaxValue;
				if (iy == -1) iy = int.MaxValue;
				return ix.CompareTo (iy);
			});

			// Ctrl disables the placement in a notebook, Escape cancels the drag
			keyWindow = source.GetToplevelWindow ();
			if (keyWindow != null) {
				keyController = keyWindow.AddKeyPressHandler (
					(keyval, keycode, state) => {
						var key = (Gdk.Key)keyval;
						if (key == Gdk.Key.Escape) {
							Destroy ();
							return true;
						}
						UpdateControl (state, keyval, true);
						return false;
					},
					(keyval, keycode, state) => UpdateControl (state, keyval, false),
					PropagationPhase.Capture);
			}
		}

		int controlKeyMask;

		void UpdateControl (Gdk.ModifierType state, uint keyval, bool pressed)
		{
			var key = (Gdk.Key)keyval;
			int mask = key == Gdk.Key.Control_L ? 1 : key == Gdk.Key.Control_R ? 2 : 0;
			if (mask == 0)
				return;
			if (pressed)
				controlKeyMask |= mask;
			else
				controlKeyMask &= ~mask;
			controlPressed = controlKeyMask != 0;
			UpdatePosition ();
		}

		DockNotebook hoverNotebook;

		bool CanPlaceInHoverNotebook ()
		{
			return !controlPressed && hoverNotebook != null;
		}

		/// <summary>Cancels the drag, hiding the outline.</summary>
		public void Destroy ()
		{
			if (destroyed)
				return;
			destroyed = true;

			ShowOutline (null, null);
			if (keyController != null && keyWindow != null)
				keyWindow.RemoveController (keyController);
			keyController = null;
			keyWindow = null;
			IdeApp.Workbench.UnlockActiveWindowChangeEvent ();
			Destroyed?.Invoke (this, EventArgs.Empty);
		}

		double curX, curY;

		public void UpdatePosition ()
		{
			if (!destroyed)
				MovePosition (curX, curY);
		}

		/// <summary>Updates the drop target. The position is in the coordinates of the source tab strip.</summary>
		public void MovePosition (double x, double y)
		{
			this.curX = x;
			this.curY = y;

			ShowPlaceholder (x, y);
		}

		void ShowOutline (DockNotebook notebook, Gdk.Rectangle? rect)
		{
			if (outlinedNotebook != null && outlinedNotebook != notebook)
				outlinedNotebook.ShowDropPlaceholder (null);
			outlinedNotebook = notebook;
			notebook?.ShowDropPlaceholder (rect);
		}

		public void ShowPlaceholder (double x, double y)
		{
			hoverNotebook = null;
			double nx = 0, ny = 0;

			// TODO: Handle z-ordering of floating windows.
			foreach (var notebook in allNotebooks) {
				if (!notebook.IsRealized)
					continue;

				if (!source.TranslateCoordinates (notebook, x, y, out double nx2, out double ny2))
					continue;
				if (nx2 >= 0 && nx2 <= notebook.Width && ny2 >= 0 && ny2 <= notebook.Height) {
					hoverNotebook = notebook;
					nx = nx2;
					ny = ny2;
					break;
				}
			}

			if (CanPlaceInHoverNotebook ()) {
				var container = hoverNotebook.Container;
				int width = hoverNotebook.Width, height = hoverNotebook.Height;
				var targetTabCount = hoverNotebook.TabCount;
				var overTabStrip = ny <= hoverNotebook.BarHeight;

				if (hoverNotebook.Tabs.Contains (frame))
					targetTabCount--; // Current is going to be removed, so it doesn't count

				if (targetTabCount > 0 && nx <= width / 3 && !overTabStrip) {
					if (container.AllowLeftInsert) {
						ShowOutline (hoverNotebook, new Gdk.Rectangle (0, 0, width / 2, height));
						placementDelegate = delegate(DockNotebook arg1, DockNotebookTab tab, Rectangle allocation2, int x2, int y2) {
							var window = (SdiWorkspaceWindow)tab.Content;
							container.InsertLeft (window);
							window.SelectWindow ();
						};
						return;
					}
				}

				if (targetTabCount > 0 && nx >= width - width / 3 && !overTabStrip) {
					if (container.AllowRightInsert) {
						ShowOutline (hoverNotebook, new Gdk.Rectangle (width / 2, 0, width / 2, height));
						placementDelegate = delegate(DockNotebook arg1, DockNotebookTab tab, Rectangle allocation2, int x2, int y2) {
							var window = (SdiWorkspaceWindow)tab.Content;
							container.InsertRight (window);
							window.SelectWindow ();
						};
						return;
					}
				}

				ShowOutline (hoverNotebook, new Gdk.Rectangle (0, 0, width, height));
				if (!hoverNotebook.Tabs.Contains (frame))
					placementDelegate = PlaceInHoverNotebook;
				else
					placementDelegate = null;
				return;
			}

			ShowOutline (null, null);
			placementDelegate = PlaceInFloatingFrame;
		}

		static void PlaceInFloatingFrame (DockNotebook notebook, DockNotebookTab tab, Rectangle allocation, int ox, int oy)
		{
			var newWindow = new DockWindow ();
			var newNotebook = newWindow.Container.GetFirstNotebook ();
			var newTab = newNotebook.AddTab ();

			var workspaceWindow = (SdiWorkspaceWindow)tab.Content;
			newTab.Content = workspaceWindow;
			newWindow.Title = DefaultWorkbench.GetTitle (workspaceWindow);

			workspaceWindow.SetDockNotebook (newNotebook, newTab);
			// GTK 4 windows can't be positioned, only the size is set
			newWindow.SetDefaultSize (w, h);
			newWindow.Present ();
			DockNotebook.ActiveNotebook = newNotebook;
		}

		const int w = 640;
		const int h = 480;

		void PlaceInHoverNotebook (DockNotebook notebook, DockNotebookTab tab, Rectangle allocation, int ox, int oy)
		{
			var window = (SdiWorkspaceWindow)tab.Content;
			var newTab = hoverNotebook.AddTab (window);
			window.SetDockNotebook (hoverNotebook, newTab);
			window.SelectWindow ();
		}

		Action<DockNotebook, DockNotebookTab, Rectangle, int, int> placementDelegate;

		public void PlaceWindow (DockNotebook notebook)
		{
			try {
				IdeApp.Workbench.LockActiveWindowChangeEvent ();
				var allocation = new Rectangle (0, 0, w, h);
				Destroy ();

				if (placementDelegate != null) {
					var tab = notebook.CurrentTab;
					notebook.RemoveTab (tab.Index, true);
					placementDelegate (notebook, tab, allocation, (int)curX, (int)curY);
				} else {
					((SdiWorkspaceWindow)frame.Content).SelectWindow ();
				}
			} finally {
				IdeApp.Workbench.UnlockActiveWindowChangeEvent ();
			}
		}
	}
}
