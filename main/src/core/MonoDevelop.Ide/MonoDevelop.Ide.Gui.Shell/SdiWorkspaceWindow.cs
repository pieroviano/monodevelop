//  SdiWorkspaceWindow.cs
//
// Author:
//   Mike Krüger
//   Lluis Sanchez Gual
//
//  This file was derived from a file from #Develop 2.0
//
//  Copyright (C) 2001-2007 Mike Krüger <mkrueger@novell.com>
//  Copyright (C) 2006 Novell, Inc (http://www.novell.com)
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Components.DockNotebook;
using MonoDevelop.Ide.Extensions;
using MonoDevelop.Ide.Gui.Documents;

namespace MonoDevelop.Ide.Gui.Shell
{
	internal class SdiWorkspaceWindow : BinWidget, IWorkbenchWindow, ICommandDelegatorRouter
	{
		DefaultWorkbench workbench;
		DocumentController controller;
		GtkShellDocumentViewItem view;
		DockNotebookTab tab;
		DockNotebook tabControl;

		string titleHolder = "";

		bool showNotification;

		public event EventHandler CloseRequested;
		public event EventHandler<NotebookChangeEventArgs> NotebookChanged;
		public event EventHandler TitleChanged;

		public DockNotebook TabControl {
			get {
				return this.tabControl;
			}
		}

		public DocumentController DocumentController {
			get {
				return controller;
			}
		}

		public DockNotebookTab DockNotebookTab {
			get {
				return tab;
			}
		}

		public SdiWorkspaceWindow (DefaultWorkbench workbench, DocumentController controller, DockNotebook tabControl, DockNotebookTab tabLabel) : base ()
		{
			this.workbench = workbench;
			this.tabControl = tabControl;
			this.controller = controller;
			this.tab = tabLabel;

			// The previous WorkbenchWindow property assignement may end with a call to AttachViewContent,
			// which will add the content control to the subview notebook. In that case, we don't need to add it to box
			controller.DocumentTitleChanged += SetTitleEvent;
			controller.HasUnsavedChangesChanged += HandleDirtyChanged;

			SetTitleEvent ();

			// activate the notebook containing this window when it is clicked
			this.AddClickHandler (null, click => {
				var notebook = FindNotebook ();
				if (notebook != null)
					DockNotebook.ActiveNotebook = notebook;
			}, phase: Gtk.PropagationPhase.Capture);
		}

		SdiDragNotebook FindNotebook ()
		{
			for (var w = Parent; w != null; w = w.Parent) {
				if (w is SdiDragNotebook notebook)
					return notebook;
			}
			return null;
		}

		public void SetRootView (IShellDocumentViewItem view)
		{
			this.view = (GtkShellDocumentViewItem) view;
			Child = null;
			if (view != null) {
				var widget = (Gtk.Widget)view;
				widget.Hexpand = true;
				widget.Vexpand = true;
				Child = widget;
			}
		}

		public IShellDocumentViewContent CreateViewContent ()
		{
			return new GtkShellDocumentViewContent ();
		}

		public IShellDocumentViewContainer CreateViewContainer ()
		{
			return new GtkShellDocumentViewContainer ();
		}

		internal void SetDockNotebook (DockNotebook tabControl, DockNotebookTab tabLabel)
		{
			var oldNotebook = tabControl;
			this.tabControl = tabControl;
			this.tab = tabLabel;
			SetTitleEvent ();
			SetDockNotebookTabTitle ();

			if (oldNotebook != tabControl)
				NotebookChanged?.Invoke (this, new NotebookChangeEventArgs { OldNotebook = oldNotebook, NewNotebook = tabControl });
		}

		void HandleDirtyChanged (object sender, EventArgs e)
		{
			OnTitleChanged (null);
		}

		Document document;
		public Document Document {
			get {
				return document;
			}
			set {
				document = value;
			}
		}
		
		public bool ShowNotification {
			get {
				return showNotification;
			}
			set {
				if (showNotification != value) {
					showNotification = value;
					OnTitleChanged (null);
				}
			}
		}

		public string Title {
			get {
				return titleHolder;
			}
			set {
				titleHolder = value;
				OnTitleChanged (null);
			}
		}

		public void OnDeactivated ()
		{
		}

		public void OnActivated ()
		{
		}

		public void SelectWindow ()
		{
			var window = tabControl.Root as Gtk.Window;
			if (window != null) {
				if (window is DockWindow) {
					IdeServices.DesktopService.GrabDesktopFocus (window);
				}

				// Focusing the main window so hide all the autohide pads
				workbench.DockFrame.MinimizeAllAutohidden ();

#if MAC
				AppKit.NSWindow nswindow = MonoDevelop.Components.Mac.GtkMacInterop.GetNSWindow (window);
				if (nswindow != null) {
					// Don't change the first responder if the current one is already a child of the content view
					if (!(nswindow.FirstResponder is AppKit.NSView view) || !view.IsDescendantOf (nswindow.ContentView))
						nswindow.MakeFirstResponder (nswindow.ContentView);
					nswindow.MakeKeyAndOrderFront (nswindow);
				}
#endif
			}

			// The tab change must be done now to ensure that the content is created
			// before exiting this method.
			tabControl.CurrentTabIndex = tab.Index;

			// Focus the tab in the next iteration since presenting the window may take some time
			Application.Invoke ((o, args) => {
				DockNotebook.ActiveNotebook = tabControl;
				if (view != null)
					view.Load ().Ignore ();
			});
		}

		public bool ContentVisible => TabControl.CurrentTab?.Content == this;

		public bool CanMoveToNextNotebook ()
		{
			return TabControl.GetNextNotebook () != null || (TabControl.Container.AllowRightInsert && TabControl.TabCount > 1);
		}

		public bool CanMoveToPreviousNotebook ()
		{
			return TabControl.GetPreviousNotebook () != null || (TabControl.Container.AllowLeftInsert && TabControl.TabCount > 1);
		}

		public void MoveToNextNotebook ()
		{
			var nextNotebook = TabControl.GetNextNotebook ();
			if (nextNotebook == null) {
				if (TabControl.Container.AllowRightInsert) {
					TabControl.RemoveTab (tab.Index, true);
					TabControl.Container.InsertRight (this);
					SelectWindow ();
				}
				return;
			}

			TabControl.RemoveTab (tab.Index, true);
			var newTab = nextNotebook.AddTab ();
			newTab.Content = this;
			SetDockNotebook (nextNotebook, newTab);
			SelectWindow ();
		}

		public void MoveToPreviousNotebook ()
		{
			var nextNotebook = TabControl.GetPreviousNotebook ();
			if (nextNotebook == null) {
				if (TabControl.Container.AllowLeftInsert) {
					TabControl.RemoveTab (tab.Index, true);
					TabControl.Container.InsertLeft (this);
					SelectWindow ();
				}
				return;
			}

			TabControl.RemoveTab (tab.Index, true);
			var newTab = nextNotebook.AddTab ();
			newTab.Content = this;
			SetDockNotebook (nextNotebook, newTab);
			SelectWindow ();
		}

		public object ViewCommandHandler { get; set; }

		IShellNotebook IWorkbenchWindow.Notebook => tabControl;

		public void SetTitleEvent (object sender, EventArgs e)
		{
			SetTitleEvent ();
		}

		void SetTitleEvent ()
		{
			if (controller == null)
				return;

			var newTitle = controller.DocumentTitle;

			if (newTitle != Title)
				Title = newTitle;
		}

		public void RequestClose ()
		{
			CloseRequested?.Invoke (this, EventArgs.Empty);
		}

		public void Close ()
		{
			view?.DetachFromView ();
			DetachController ();
			if (Parent is ContainerWidget container)
				container.RemoveChild (this);
			else if (Parent is Gtk.Box box)
				box.Remove (this);
			else if (Parent != null)
				Unparent ();
		}

		void DetachController ()
		{
			controller.DocumentTitleChanged -= SetTitleEvent;
			controller.HasUnsavedChangesChanged -= HandleDirtyChanged;

			document = null;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				DetachController ();
			base.Dispose (disposing);
		}

		object ICommandDelegatorRouter.GetNextCommandTarget ()
		{
			return Parent;
		}
		
		object ICommandDelegatorRouter.GetDelegatedCommandTarget ()
		{
			// If command checks are flowing through this view, it means the view's notebook
			// is the active notebook.
			if (!(Root is Gtk.Window window))
				return Document;

			if (window.IsActive) {
				var notebook = FindNotebook ();
				if (notebook != null)
					DockNotebook.ActiveNotebook = notebook;
			}

			return Document;
		}

		void SetDockNotebookTabTitle ()
		{
			tab.Text = Title;
			tab.Notify = showNotification;
			tab.Dirty = controller.HasUnsavedChanges;
			if (!string.IsNullOrEmpty (controller.DocumentTitle)) {
				if (controller is FileDocumentController fileDocumentController)
					tab.Tooltip = fileDocumentController.FilePath;
				else
					tab.Tooltip = controller.DocumentTitle;
			}
			tab.Icon = controller.DocumentIcon?.WithSize (StockIconSize.Menu);
		}
		
		void OnTitleChanged(EventArgs e)
		{
			SetDockNotebookTabTitle ();
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}
	}
}
