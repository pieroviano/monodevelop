// ErrorListPad.cs
//  
// Author:
//       Todd Berman <tberman@sevenl.net>
//       David Makovský <yakeen@sannyas-on.net>
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2004 Todd Berman
// Copyright (c) 2006 David Makovský
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
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Ide.Gui.Content;

using Gtk;
using System.Text;
using MonoDevelop.Components.Docking;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using System.Linq;
using MonoDevelop.Components.AutoTest;
using System.ComponentModel;
using MonoDevelop.Ide.BuildOutputView;
using System.Threading.Tasks;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Instrumentation;

namespace MonoDevelop.Ide.Gui.Pads
{
	class ErrorListPad : PadContent
	{
		SizeNotifyingPaned control;
		ScrolledWindow sw;
		PadTreeView view;
		BuildOutputViewContent buildOutputViewContent;
		BuildOutput buildOutput;
		LogView logView;
		TreeStore store;
		TreeModelFilter filter;
		TreeModelSort sort;
		ToggleButton errorBtn, warnBtn, msgBtn;
		Button buildLogBtn;
		ToggleButton logBtn;
		Label errorBtnLbl, warnBtnLbl, msgBtnLbl, logBtnLbl;
		MonoDevelop.Components.SearchEntry searchEntry;
		string currentSearchPattern = null;
		Hashtable tasks = new Hashtable ();
		int errorCount;
		int warningCount;
		int infoCount;
		bool initialLogShow = true;


		Xwt.Drawing.Image iconWarning;
		Xwt.Drawing.Image iconError;
		Xwt.Drawing.Image iconInfo;
		Xwt.Drawing.Image iconEmpty;

		static readonly string restoreID = "Monodevelop.ErrorListColumns";
		public readonly ConfigurationProperty<bool> ShowErrors = ConfigurationProperty.Create ("SharpDevelop.TaskList.ShowErrors", true);
		public readonly ConfigurationProperty<bool> ShowWarnings = ConfigurationProperty.Create ("SharpDevelop.TaskList.ShowWarnings", true);
		public readonly ConfigurationProperty<bool> ShowMessages = ConfigurationProperty.Create ("SharpDevelop.TaskList.ShowMessages", true);
		public readonly ConfigurationProperty<double> LogSeparatorPosition = ConfigurationProperty.Create ("SharpDevelop.TaskList.LogSeparatorPosition", 0.5d);
		public readonly ConfigurationProperty<bool> OutputViewVisible = ConfigurationProperty.Create ("SharpDevelop.TaskList.OutputViewVisible", false);

		static class DataColumns
		{
			internal const int Type = 0;
			internal const int Read = 1;
			internal const int Task = 2;
			internal const int Description = 3;
		}

		static class VisibleColumns
		{
			internal const int Type = 0;
			internal const int Marked = 1;
			internal const int Line = 2;
			internal const int Description = 3;
			internal const int File = 4;
			internal const int Project = 5;
			internal const int Path = 6;
			internal const int Category = 7;
		}

		static class Counters
		{
			public static Counter BuildLogShown = InstrumentationService.CreateCounter ("Build log opened", "Build Output", id: "ErrorListPad.BuildLogShown");
		}

		public override Control Control {
			get {
				if (control == null)
					CreateControl ();
				return control;
			}
		}

		public override string Id {
			get { return "MonoDevelop.Ide.Gui.Pads.ErrorListPad"; }
		}

		ToggleButton MakeButton (string image, string name, bool active, out Label label)
		{
			var btnBox = MakeHBox (image, out label);

			var btn = new ToggleButton { Name = name, Active = active };
			btn.Child = btnBox;

			return btn;
		} 

		Button MakeButton (string image, string name, out Label label) 
		{
			var btnBox = MakeHBox (image, out label);

			var btn = new Button { Name = name };
			btn.Child = btnBox;

			return btn;
		}

		Box MakeHBox (string image, out Label label)
		{
			var btnBox = new Box (Orientation.Horizontal, 2);
			btnBox.SetShouldIgnore (true);
			var imageView = new ImageView (image, MonoDevelop.Components.StockIconSize.Menu);
			imageView.SetShouldIgnore (true);
			btnBox.Append (imageView);

			label = new Label ();
			label.SetShouldIgnore (true);
			btnBox.Append (label);

			return btnBox;
		}

		protected override void Initialize (IPadWindow window)
		{
			window.Title = GettextCatalog.GetString ("Errors");

			DockItemToolbar toolbar = window.GetToolbar (DockPositionType.Top);

			errorBtn = MakeButton (Stock.Error, "toggleErrors", ShowErrors, out errorBtnLbl);
			errorBtn.SetCommonAttributes ("ErrorPad.ErrorButton", null, null);

			errorBtn.Toggled += FilterChanged;
			errorBtn.TooltipText = GettextCatalog.GetString ("Show Errors");
			errorBtn.SetCommonAttributes (null, null, GettextCatalog.GetString ("Show Errors"));
			UpdateErrorsNum ();
			toolbar.Add (errorBtn);

			warnBtn = MakeButton (Stock.Warning, "toggleWarnings", ShowWarnings, out warnBtnLbl);
			warnBtn.SetCommonAttributes ("ErrorPad.WarningButton", null, null);
			warnBtn.Toggled += FilterChanged;
			warnBtn.TooltipText = GettextCatalog.GetString ("Show Warnings");
			warnBtn.SetCommonAttributes (null, null, GettextCatalog.GetString ("Show Warnings"));
			UpdateWarningsNum ();
			toolbar.Add (warnBtn);

			msgBtn = MakeButton (Stock.Information, "toggleMessages", ShowMessages, out msgBtnLbl);
			msgBtn.SetCommonAttributes ("ErrorPad.MessageButton", null, null);
			msgBtn.Toggled += FilterChanged;
			msgBtn.TooltipText = GettextCatalog.GetString ("Show Messages");
			msgBtn.SetCommonAttributes (null, null, GettextCatalog.GetString ("Show Messages"));
			UpdateMessagesNum ();
			toolbar.Add (msgBtn);

			var sep = new Separator (Orientation.Vertical);
			sep.SetShouldIgnore (true);
			toolbar.Add (sep);

			logBtn = MakeButton ("md-message-log", "toggleBuildOutput", false, out logBtnLbl);
			logBtn.SetCommonAttributes ("ErrorPad.LogButton", null, null);
			logBtn.TooltipText = GettextCatalog.GetString ("Build Output");
			logBtn.SetCommonAttributes (null, null, GettextCatalog.GetString ("Build Output"));
			logBtnLbl.Text = GettextCatalog.GetString ("Build Output");
			logBtn.SetTitle (logBtnLbl.Text);
			logBtn.Toggled += HandleTextLogToggled;
			toolbar.Add (logBtn);

			if (BuildOutput.IsFeatureEnabled) {
				buildLogBtn = MakeButton ("md-message-log", "toggleBuildOutput", out logBtnLbl);
				buildLogBtn.SetCommonAttributes ("ErrorPad.BuildLogButton", null, null);
				buildLogBtn.TooltipText = GettextCatalog.GetString ("Structured Build Output");
				buildLogBtn.SetCommonAttributes (null, null, GettextCatalog.GetString ("Structured Build Output"));

				logBtnLbl.Text = GettextCatalog.GetString ("Structured Build Output");
				buildLogBtn.SetTitle (logBtnLbl.Text);

				buildLogBtn.Clicked += HandleBinLogClicked;
				toolbar.Add (buildLogBtn);
			}

			buildOutput = new BuildOutput ();

			//Dummy widget to take all space between "Build Output" button and SearchEntry
			var spacer = new Box (Orientation.Horizontal, 0);
			spacer.SetShouldIgnore (true);
			toolbar.Add (spacer, true);

			searchEntry = new MonoDevelop.Components.SearchEntry ();
			searchEntry.SetLabel (GettextCatalog.GetString ("Search"));
			searchEntry.SetCommonAttributes ("ErrorPad.Search", null, null);
			searchEntry.SetCommonAttributes (null, null, GettextCatalog.GetString ("Search the error data"));
			searchEntry.Entry.Changed += searchPatternChanged;
			searchEntry.WidthRequest = 200;
			searchEntry.Visible = true;
			toolbar.Add (searchEntry);


			UpdatePadIcon ();

			IdeApp.ProjectOperations.StartBuild += OnBuildStarted;
			IdeApp.ProjectOperations.StartClean += OnBuildStarted;
		}

		void searchPatternChanged (object sender, EventArgs e)
		{
			currentSearchPattern = searchEntry.Entry.Text;
			filter.Refilter ();
		}

		void CreateControl ()
		{
			control = new SizeNotifyingPaned ();

			store = new Gtk.TreeStore (typeof (Xwt.Drawing.Image), // image - type
									   typeof (bool),       // read?
									   typeof (TaskListEntry),       // read? -- use Pango weight
									   typeof (string));
			SemanticModelAttribute modelAttr = new SemanticModelAttribute ("store__Type", "store__Read", "store__Task", "store__Description");
			TypeDescriptor.AddAttributes (store, modelAttr);

			TreeModelFilterVisibleFunc filterFunct = new TreeModelFilterVisibleFunc (FilterTasks);
			filter = (TreeModelFilter) store.FilterNew (null);
			filter.VisibleFunc = filterFunct;
			
			sort = new TreeModelSort (filter);
			sort.SetSortFunc (VisibleColumns.Type, SeverityIterSort);
			sort.SetSortFunc (VisibleColumns.Project, ProjectIterSort);
			sort.SetSortFunc (VisibleColumns.File, FileIterSort);
			sort.SetSortFunc (VisibleColumns.Category, CategoryIterSort);

			view = new PadTreeView (sort);
			view.Selection.Mode = SelectionMode.Multiple;
			view.ShowExpanders = true;
			// TODO GTK4: TreeView.RulesHint (alternating row colours) has no binding

			view.DoPopupMenu += ShowPopup;

			AddColumns ();
			LoadColumnsVisibility ();
			view.Columns [VisibleColumns.Type].SortColumnId = VisibleColumns.Type;
			view.Columns [VisibleColumns.Project].SortColumnId = VisibleColumns.Project;
			view.Columns [VisibleColumns.File].SortColumnId = VisibleColumns.File;
			view.Columns [VisibleColumns.Category].SortColumnId = VisibleColumns.Category;

			sw = new MonoDevelop.Components.CompactScrolledWindow ();
			sw.Child = view;
			IdeServices.TaskService.Errors.TasksRemoved += ShowResults;
			IdeServices.TaskService.Errors.TasksAdded += TaskAdded;
			IdeServices.TaskService.Errors.TasksChanged += TaskChanged;
			IdeServices.TaskService.Errors.CurrentLocationTaskChanged += HandleTaskServiceErrorsCurrentLocationTaskChanged;

			IdeApp.Workspace.FirstWorkspaceItemOpened += OnCombineOpen;
			IdeApp.Workspace.LastWorkspaceItemClosed += OnCombineClosed;

			view.RowActivated += OnRowActivated;

			iconWarning = ImageService.GetIcon (Ide.Gui.Stock.Warning, MonoDevelop.Components.StockIconSize.Menu);
			iconError = ImageService.GetIcon (Ide.Gui.Stock.Error, MonoDevelop.Components.StockIconSize.Menu);
			iconInfo = ImageService.GetIcon (Ide.Gui.Stock.Information, MonoDevelop.Components.StockIconSize.Menu);
			iconEmpty = ImageService.GetIcon (Ide.Gui.Stock.Empty, MonoDevelop.Components.StockIconSize.Menu);

			control.StartChild = sw;
			control.ResizeStartChild = true;
			control.ShrinkStartChild = false;

			logView = new LogView { Name = "buildOutput" };
			control.EndChild = logView;
			control.ResizeEndChild = false;
			control.ShrinkEndChild = false;


			control.Allocated += HandleControlSizeAllocated;
			control.AddNotification ("position", HandleSeparatorPositionChanged);

			logView.Visible = OutputViewVisible;
			logBtn.Active = OutputViewVisible;

			// Load existing tasks
			foreach (TaskListEntry t in IdeServices.TaskService.Errors) {
				AddTask (t);
			}
		}

		public override void Dispose ()
		{
			errorBtn.Toggled -= FilterChanged;
			warnBtn.Toggled -= FilterChanged;
			msgBtn.Toggled -= FilterChanged;
			logBtn.Toggled -= HandleTextLogToggled;
			if (BuildOutput.IsFeatureEnabled)
				buildLogBtn.Clicked -= HandleBinLogClicked;
			searchEntry.Entry.Changed -= searchPatternChanged;

			IdeApp.Workspace.FirstWorkspaceItemOpened -= OnCombineOpen;
			IdeApp.Workspace.LastWorkspaceItemClosed -= OnCombineClosed;
			IdeApp.ProjectOperations.StartBuild -= OnBuildStarted;
			IdeApp.ProjectOperations.StartClean -= OnBuildStarted;

			IdeServices.TaskService.Errors.TasksRemoved -= ShowResults;
			IdeServices.TaskService.Errors.TasksAdded -= TaskAdded;
			IdeServices.TaskService.Errors.TasksChanged -= TaskChanged;
			IdeServices.TaskService.Errors.CurrentLocationTaskChanged -= HandleTaskServiceErrorsCurrentLocationTaskChanged;

			buildOutput?.Dispose ();
			buildOutputViewContent?.Dispose ();
			buildOutputDoc?.Close ();

			// Set the model to null as it makes Gtk clean up faster
			if (view != null) {
				view.RowActivated -= OnRowActivated;
				view.Model = null;
			}

			if (control != null) {
				control.Allocated -= HandleControlSizeAllocated;
			}

			if (control != null) {
				control.RemoveNotification ("position", HandleSeparatorPositionChanged);
			}

			base.Dispose ();
		}

		void HandleSeparatorPositionChanged (object o, GLib.NotifyArgs args)
		{
			var width = control.AllocatedWidth;
			if (!initialLogShow && OutputViewVisible && width > 0) {
				var val = (double)((double)control.Position / (double)width);
				LogSeparatorPosition.Value = val;
			}
		}

		void HandleControlSizeAllocated (int width, int height)
		{
			if (initialLogShow && OutputViewVisible) {
				SetInitialOutputViewSize (width);
				initialLogShow = false;
			}
		}

		public ProgressMonitor GetBuildProgressMonitor ()
		{ 
			if (control == null)
				CreateControl ();

			var monitor = new AggregatedProgressMonitor ();
			monitor.AddFollowerMonitor (buildOutput.GetProgressMonitor ());
			monitor.AddFollowerMonitor (logView.GetProgressMonitor ());

			return monitor;
		}

		void HandleTaskServiceErrorsCurrentLocationTaskChanged (object sender, EventArgs e)
		{
			if (IdeServices.TaskService.Errors.CurrentLocationTask == null) {
				view.Selection.UnselectAll ();
				return;
			}
			TreeIter it;
			if (!view.Model.GetIterFirst (out it))
				return;
			do {
				TaskListEntry t = (TaskListEntry)view.Model.GetValue (it, DataColumns.Task);
				if (t == IdeServices.TaskService.Errors.CurrentLocationTask) {
					view.Selection.SelectIter (it);
					view.ScrollToCell (view.Model.GetPath (it), view.Columns [0], false, 0, 0);
					it = filter.ConvertIterToChildIter (sort.ConvertIterToChildIter (it));
					store.SetValue (it, DataColumns.Read, true);
					return;
				}
			} while (view.Model.IterNext (ref it));
		}

		internal void SelectTaskListEntry (TaskListEntry taskListEntry)
		{
			TreeIter iter;
			if (!view.Model.GetIterFirst (out iter))
				return;
			do {
				var t = (TaskListEntry)view.Model.GetValue (iter, DataColumns.Task);
				if (t == taskListEntry) {
					view.Selection.SelectIter (iter);
					view.ScrollToCell (view.Model.GetPath (iter), view.Columns [0], false, 0, 0);
					return;
				}
			} while (view.Model.IterNext (ref iter));
		}

		void LoadColumnsVisibility ()
		{
			var columns = PropertyService.Get (restoreID, string.Join (";", Enumerable.Repeat ("TRUE", view.Columns.Length)));
			var tokens = columns.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (view.Columns.Length == tokens.Length) {
				for (int i = 0; i < tokens.Length; i++) {
					bool visible;
					if (bool.TryParse (tokens [i], out visible))
						view.Columns [i].Visible = visible;
				}
			}
		}

		void ShowPopup (ClickInfo evt)
		{
			var menu = new ContextMenu ();

			var help = new ContextMenuItem (GettextCatalog.GetString ("Go to _Reference"));
			help.Clicked += OnShowReference;
			menu.Add (help);

			if (BuildOutput.IsFeatureEnabled) {
				var goBuild = new ContextMenuItem (GettextCatalog.GetString ("Go to _Log"));
				goBuild.Clicked += async (s, e) => await OnGoToLog (s, e);
				menu.Add (goBuild);
			}

			var jump = new ContextMenuItem (GettextCatalog.GetString ("_Go to Task"));
			jump.Clicked += OnTaskJumpto;
			menu.Add (jump);

			var columnsMenu = new ColumnSelectorMenu (view,
			                                          restoreID,
			                                          GettextCatalog.GetString ("Type"),
			                                          GettextCatalog.GetString ("Validity"));

			menu.Add (new SeparatorContextMenuItem ());

			var copy = new ContextMenuItem (GettextCatalog.GetString ("_Copy"));
			copy.Clicked += OnTaskCopied;
			menu.Add (copy);


			menu.Add (new SeparatorContextMenuItem ());

			var columns = new ContextMenuItem (GettextCatalog.GetString ("Columns"));
			columns.SubMenu = columnsMenu;
			menu.Add (columns);

			help.Sensitive = copy.Sensitive = jump.Sensitive =
				view.Selection != null &&
				view.Selection.CountSelectedRows () > 0 &&
				view.IsAColumnVisible ();

			// Disable Help and Go To if multiple rows selected.
			if (help.Sensitive && view.Selection.CountSelectedRows () > 1) {
				help.Sensitive = false;
				jump.Sensitive = false;
			}

			string dummyString;
			help.Sensitive &= GetSelectedErrorReference (out dummyString);

			menu.Show (view, evt);
		}

		async Task OnGoToLog (object o, EventArgs args)
		{
			var rows = view.Selection.GetSelectedRows ();
			if (!rows.Any ())
				return;

			TreeIter iter, sortedIter;
			if (view.Model.GetIter (out sortedIter, rows [0])) {
				iter = filter.ConvertIterToChildIter (sort.ConvertIterToChildIter (sortedIter));

				store.SetValue (iter, DataColumns.Read, true);

				TaskListEntry task = store.GetValue (iter, DataColumns.Task) as TaskListEntry;
				if (task != null) {
					await OpenBuildOutputViewDocument ();
					if (task.Severity == TaskSeverity.Error) {
						await buildOutputViewContent.GoToError (task.Message, task.GetProjectWithExtension ());
					} else if (task.Severity == TaskSeverity.Warning) {
						await buildOutputViewContent.GoToWarning (task.Message, task.GetProjectWithExtension ());
					} else if (task.Severity == TaskSeverity.Information) {
						await buildOutputViewContent.GoToMessage (task.Message, task.GetProjectWithExtension ());
					}
				}
			}
		}

		TaskListEntry SelectedTask {
			get {
				TreeIter iter;
				var rows = view.Selection.GetSelectedRows ();
				if (rows.Any () && view.Model.GetIter (out iter, rows[0]))
					return view.Model.GetValue (iter, DataColumns.Task) as TaskListEntry;
				return null; // no one selected
			}
		}

		IEnumerable<TaskListEntry> GetSelectedTasks ()
		{
			TreeIter iter;
			foreach (var row in view.Selection.GetSelectedRows ()) {
				if (view.Model.GetIter (out iter, row)) {
					var task = view.Model.GetValue (iter, DataColumns.Task) as TaskListEntry;
					if (task != null)
						yield return task;
				}
			}
		}

		[CommandHandler (EditCommands.Copy)]
		protected void OnCopy ()
		{
			OnTaskCopied (null, null);
		}

		void OnTaskCopied (object o, EventArgs args)
		{
			var selectedTasks = GetSelectedTasks ().ToArray ();
			if (!selectedTasks.Any ())
				return;

			var text = new StringBuilder ();

			for (int i = 0; i < selectedTasks.Length; i++) {
				if (i > 0)
					text.Append (Environment.NewLine);

				TaskListEntry task = selectedTasks [i];
				if (!string.IsNullOrEmpty (task.FileName)) {
					text.Append (task.FileName);
					if (task.Line >= 1) {
						text.Append ("(").Append (task.Column);
						if (task.Column >= 0)
							text.Append (",").Append (task.Column);
						text.Append (")");
					}
					text.Append (": ");
				}
				text.Append (task.Severity.ToString ());
				if (!string.IsNullOrEmpty (task.Code)) {
					text.Append (" ").Append (task.Code);
				}
				text.Append (": ");
				text.Append (task.Description);
				if (task.WorkspaceObject != null)
					text.Append (" (").Append (task.WorkspaceObject.Name).Append (")");

				if (!string.IsNullOrEmpty (task.Category)) {
					text.Append (" ").Append (task.Category);
				}
			}

			ClipboardHelper.SetText (text.ToString ());
			ClipboardHelper.SetText (text.ToString (), primary: true);
		}

		void OnShowReference (object o, EventArgs args)
		{
			string reference = null;
			if (GetSelectedErrorReference (out reference) && reference != null)
				IdeServices.DesktopService.ShowUrl (reference);
		}

		bool GetSelectedErrorReference (out string reference)
		{
			string webRequest = "http://google.com/search?q=";
			TaskListEntry task = SelectedTask;
			if (task != null && task.HasDocumentationLink ()) {
				reference = task.DocumentationLink;
				return true;
			}
			if (task != null && !string.IsNullOrEmpty (task.HelpKeyword)) {
				reference = webRequest + System.Web.HttpUtility.UrlEncode (task.HelpKeyword);
				return true;
			}
			if (task != null && !string.IsNullOrEmpty (task.Code)) {
				reference = webRequest + System.Web.HttpUtility.UrlEncode (task.Code);
				return true;
			}
			reference = null;
			return false;
		}

		void OnTaskJumpto (object o, EventArgs args)
		{
			var rows = view.Selection.GetSelectedRows ();
			if (!rows.Any ())
				return;

			TreeIter iter, sortedIter;
			if (view.Model.GetIter (out sortedIter, rows [0])) {
				iter = filter.ConvertIterToChildIter (sort.ConvertIterToChildIter (sortedIter));
				store.SetValue (iter, DataColumns.Read, true);
				TaskListEntry task = store.GetValue (iter, DataColumns.Task) as TaskListEntry;
				if (task != null) {
					IdeServices.TaskService.ShowStatus (task);
					task.JumpToPosition ();
					IdeServices.TaskService.Errors.CurrentLocationTask = task;
					IdeApp.Workbench.ActiveLocationList = IdeServices.TaskService.Errors;
				}
			}
		}

		void AddColumns ()
		{
			CellRendererImage iconRender = new CellRendererImage ();

			Gtk.CellRendererToggle toggleRender = new Gtk.CellRendererToggle ();
			toggleRender.Toggled += new ToggledHandler (ItemToggled);

			TreeViewColumn col;
			col = view.AppendColumn ("!", iconRender, "image", DataColumns.Type);

			col = view.AppendColumn ("", toggleRender);
			col.SetCellDataFunc (toggleRender, new Gtk.TreeCellDataFunc (ToggleDataFunc));

			col = view.AppendColumn (GettextCatalog.GetString ("Line"), view.TextRenderer);
			col.SetCellDataFunc (view.TextRenderer, new Gtk.TreeCellDataFunc (LineDataFunc));

			var descriptionCellRenderer = new DescriptionCellRendererText ();
			view.RegisterRenderForFontChanges (descriptionCellRenderer);
			var descriptionCol = view.AppendColumn (GettextCatalog.GetString ("Description"), descriptionCellRenderer);
			descriptionCol.SetCellDataFunc (descriptionCellRenderer, new Gtk.TreeCellDataFunc (DescriptionDataFunc));
			descriptionCol.Resizable = true;
			descriptionCellRenderer.PreferedMaxWidth = IdeApp.Workbench.RootWindow.AllocatedWidth / 3;
			descriptionCellRenderer.WrapWidth = descriptionCellRenderer.PreferedMaxWidth;
			descriptionCellRenderer.WrapMode = Pango.WrapMode.Word;

			col = view.AppendColumn (GettextCatalog.GetString ("File"), view.TextRenderer);
			col.SetCellDataFunc (view.TextRenderer, new Gtk.TreeCellDataFunc (FileDataFunc));
			col.Resizable = true;
			
			col = view.AppendColumn (GettextCatalog.GetString ("Project"), view.TextRenderer);
			col.SetCellDataFunc (view.TextRenderer, new Gtk.TreeCellDataFunc (ProjectDataFunc));
			col.Resizable = true;
			
			col = view.AppendColumn (GettextCatalog.GetString ("Path"), view.TextRenderer);
			col.SetCellDataFunc (view.TextRenderer, new Gtk.TreeCellDataFunc (PathDataFunc));
			col.Resizable = true;

			col = view.AppendColumn (GettextCatalog.GetString ("Category"), view.TextRenderer);
			col.SetCellDataFunc (view.TextRenderer, new Gtk.TreeCellDataFunc (CategoryDataFunc));
			col.Resizable = true;
		}

		static void ToggleDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererToggle toggleRenderer = (Gtk.CellRendererToggle)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			if (task == null) {
				toggleRenderer.Visible = false;
				return;
			}
			toggleRenderer.Active = task.Completed;
		}
		
		static void LineDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererText textRenderer = (Gtk.CellRendererText)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			if (task == null) {
				textRenderer.Text = "";
				return;
			}
			SetText (textRenderer, model, iter, task, task.Line != 0 ? task.Line.ToString () : "");
		}

		static void DescriptionDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			var textRenderer = (CellRendererText)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			var text = model.GetValue (iter, DataColumns.Description) as string;
			if (task == null) {
				if (model.IterParent (out iter, iter)) {
					task = model.GetValue (iter, DataColumns.Task) as TaskListEntry;
					if (task == null) {
						textRenderer.Text = "";
						return;
					}
				} else {
					textRenderer.Text = "";
					return;
				}
			}
			SetText (textRenderer, model, iter, task, text);
		}

		static void FileDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererText textRenderer = (Gtk.CellRendererText)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			if (task == null) {
				textRenderer.Text = "";
				return;
			}

			SetText (textRenderer, model, iter, task, task.GetFile ());
		}
		
		static void ProjectDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererText textRenderer = (Gtk.CellRendererText)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			if (task == null) {
				textRenderer.Text = "";
				return;
			}
			SetText (textRenderer, model, iter, task, task.GetProject ());
		}
		
		static void PathDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererText textRenderer = (Gtk.CellRendererText)cell;
			TaskListEntry task = model.GetValue (iter, DataColumns.Task) as TaskListEntry; 
			if (task == null) {
				textRenderer.Text = "";
				return;
			}
			SetText (textRenderer, model, iter, task, task.GetPath ());
		}

		static void CategoryDataFunc (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.ITreeModel model, Gtk.TreeIter iter)
		{
			Gtk.CellRendererText textRenderer = (Gtk.CellRendererText)cell;
			var task = model.GetValue (iter, DataColumns.Task) as TaskListEntry;
			if (task == null) {
				textRenderer.Text = "";
				return;
			}
			SetText (textRenderer, model, iter, task, task.Category ?? "");
		}
		
		static void SetText (CellRendererText textRenderer, ITreeModel model, TreeIter iter, TaskListEntry task, string text)
		{
			textRenderer.Text = text;
			textRenderer.Weight = (int)((bool)model.GetValue (iter, DataColumns.Read) ? Pango.Weight.Normal : Pango.Weight.Bold);
			textRenderer.Strikethrough = task.Completed;
		}
		
		void OnCombineOpen(object sender, EventArgs e)
		{
			Clear();
		}
		
		void OnCombineClosed(object sender, EventArgs e)
		{
			Clear();

			buildOutput.Dispose ();
			if (buildOutputViewContent != null) {
				buildOutputViewContent.Dispose ();
				buildOutputViewContent = null;
			}

			buildOutputDoc?.Close ();

			buildOutput = new BuildOutput ();
		}

		void OnBuildStarted (object sender, EventArgs e)
		{
			buildOutput.Clear ();
		}
		
		void OnRowActivated (object o, RowActivatedArgs args)
		{
			OnTaskJumpto (null, null);
		}
		
		public CompilerResults CompilerResults = null;
		
		void FilterChanged (object sender, EventArgs e)
		{
			
			ShowErrors.Value = errorBtn.Active;
			ShowWarnings.Value = warnBtn.Active;
			ShowMessages.Value = msgBtn.Active;
			
			filter.Refilter ();
		}

		internal void SetFilter (bool showErrors, bool showWarnings, bool showMessages)
		{
			errorBtn.Active = showErrors;
			warnBtn.Active = showWarnings;
			msgBtn.Active = showMessages;
		}


		bool FilterTasks (ITreeModel model, TreeIter iter)
		{
			bool canShow = false;

			try {
				TaskListEntry task = store.GetValue (iter, DataColumns.Task) as TaskListEntry;
				if (task == null)
					return true;
				if (task.Severity == TaskSeverity.Error && errorBtn.Active) canShow = true;
				else if (task.Severity == TaskSeverity.Warning && warnBtn.Active) canShow = true;
				else if (task.Severity == TaskSeverity.Information && msgBtn.Active) canShow = true;

				if (canShow && !string.IsNullOrWhiteSpace (currentSearchPattern)) {
					canShow = (task.Description != null && task.Description.IndexOf (currentSearchPattern, StringComparison.OrdinalIgnoreCase) != -1) ||
						(task.Code != null && task.Code.IndexOf (currentSearchPattern, StringComparison.OrdinalIgnoreCase) != -1) ||
						(task.FileName != null && task.FileName.FileName.IndexOf (currentSearchPattern, StringComparison.OrdinalIgnoreCase) != -1) ||
						(task.WorkspaceObject != null && task.WorkspaceObject.Name != null && task.WorkspaceObject.Name.IndexOf (currentSearchPattern, StringComparison.OrdinalIgnoreCase) != -1) ||
						(task.Category != null && task.Category.IndexOf (currentSearchPattern, StringComparison.OrdinalIgnoreCase) != -1);
				}
			} catch {
				//Not yet fully added
				return false;
			}
			
			return canShow;
		}

		public void ShowResults (object sender, EventArgs e)
		{
			Clear();

			AddTasks (IdeServices.TaskService.Errors);
		}

		private void Clear()
		{
			errorCount = warningCount = infoCount = 0;
			if (view.IsRealized)
				view.ScrollToPoint (0, 0);
			store.Clear ();
			tasks.Clear ();
			UpdateErrorsNum ();
			UpdateWarningsNum ();
			UpdateMessagesNum ();
			UpdatePadIcon ();
		}
		
		void TaskChanged (object sender, TaskEventArgs e)
		{
			this.view.QueueDraw ();
		}
	
		void TaskAdded (object sender, TaskEventArgs e)
		{
			AddTasks (e.Tasks);
		}
		
		public void AddTasks (IEnumerable<TaskListEntry> tasks)
		{
			int n = 1;
			foreach (TaskListEntry t in tasks) {
				AddTaskInternal (t);
				if ((n++ % 100) == 0) {
					// Adding many tasks is a bit slow, so refresh the
					// ui at every block of 100.
					DispatchService.RunPendingEvents ();
				}
			}
			filter.Refilter ();
		}
		
		public void AddTask (TaskListEntry t)
		{
			AddTaskInternal (t);
			filter.Refilter ();
		}

		void AddTaskInternal (TaskListEntry t)
		{
			if (tasks.Contains (t)) return;

			Xwt.Drawing.Image stock;
			
			switch (t.Severity) {
				case TaskSeverity.Error:
					stock = iconError;
					errorCount++;
					UpdateErrorsNum ();
					break; 
				case TaskSeverity.Warning:
					stock = iconWarning;
					warningCount++;
					UpdateWarningsNum ();	
					break;
				default:
					stock = iconInfo;
					infoCount++;
					UpdateMessagesNum ();
					break;
			}
			
			tasks [t] = t;

			var indexOfNewLine = t.Description.IndexOfAny (new [] { '\n', '\r' });
			if (indexOfNewLine != -1) {
				var iter = store.AppendValues (stock, false, t, t.Description.Remove (indexOfNewLine));
				store.AppendValues (iter, iconEmpty, false, null, t.Description);
			} else {
				store.AppendValues (stock, false, t, t.Description);
			}

			UpdatePadIcon ();
		}

		void UpdateErrorsNum () 
		{
			errorBtnLbl.Text = " " + string.Format(GettextCatalog.GetPluralString("{0} Error", "{0} Errors", errorCount), errorCount);
			errorBtn.SetTitle (errorBtnLbl.Text);
		}

		void UpdateWarningsNum ()
		{
			warnBtnLbl.Text = " " + string.Format(GettextCatalog.GetPluralString("{0} Warning", "{0} Warnings", warningCount), warningCount);
			warnBtn.SetTitle (warnBtnLbl.Text);
		}

		void UpdateMessagesNum ()
		{
			msgBtnLbl.Text = " " + string.Format(GettextCatalog.GetPluralString("{0} Message", "{0} Messages", infoCount), infoCount);
			msgBtn.SetTitle (msgBtnLbl.Text);
		}

		void UpdatePadIcon ()
		{
			if (errorCount > 0)
				Window.Icon = "md-errors-list-has-errors";
			else if (warningCount > 0)
				Window.Icon = "md-errors-list-has-warnings";
			else
				Window.Icon = "md-errors-list";
		}
		
		private void ItemToggled (object o, ToggledArgs args)
		{
			Gtk.TreeIter iter;

			if (view.Model.GetIterFromString (out iter, args.Path)) {
				TaskListEntry task = (TaskListEntry)view.Model.GetValue (iter, DataColumns.Task);
				task.Completed = !task.Completed;
				IdeServices.TaskService.FireTaskToggleEvent (this, new TaskEventArgs (task));
			}
		}

		static int SeverityIterSort(ITreeModel model, TreeIter a, TreeIter z)
		{
			TaskListEntry aTask = model.GetValue(a, DataColumns.Task) as TaskListEntry,
			     zTask = model.GetValue(z, DataColumns.Task) as TaskListEntry;
			     
			return (aTask != null && zTask != null) ?
			       aTask.Severity.CompareTo(zTask.Severity) :
			       0;
		}
		
		static int ProjectIterSort (ITreeModel model, TreeIter a, TreeIter z)
		{
			TaskListEntry aTask = model.GetValue (a, DataColumns.Task) as TaskListEntry,
			     zTask = model.GetValue (z, DataColumns.Task) as TaskListEntry;
			     
			return (aTask != null && zTask != null) ?
			       string.Compare (aTask.GetProject (), zTask.GetProject (), StringComparison.Ordinal) :
			       0;
		}
		
		static int FileIterSort (ITreeModel model, TreeIter a, TreeIter z)
		{
			TaskListEntry aTask = model.GetValue (a, DataColumns.Task) as TaskListEntry,
			     zTask = model.GetValue (z, DataColumns.Task) as TaskListEntry;
			     
			return (aTask != null && zTask != null) ?
			       aTask.FileName.CompareTo (zTask.FileName) :
			       0;
		}

		static int CategoryIterSort (ITreeModel model, TreeIter a, TreeIter z)
		{
			TaskListEntry aTask = model.GetValue (a, DataColumns.Task) as TaskListEntry,
				 zTask = model.GetValue (z, DataColumns.Task) as TaskListEntry;

			return (aTask?.Category != null && zTask?.Category != null) ?
			       string.Compare (aTask.Category, zTask.Category, StringComparison.Ordinal) :
			       0;
		}

		internal void FocusOutputView ()
		{
			HandleBinLogClicked (this, EventArgs.Empty);
		}

		void HandleTextLogToggled (object sender, EventArgs e)
		{
			var visible = logBtn.Active;
			OutputViewVisible.Value = visible;
			logView.Visible = visible;

			SetInitialOutputViewSize (control.AllocatedWidth);

			if (visible) {
				Counters.BuildLogShown.Inc (1);
			}
		}

		void SetInitialOutputViewSize (int controlWidth)
		{
			double relPos = LogSeparatorPosition;
			int pos = (int)(controlWidth * relPos);
			pos = Math.Max(30, Math.Min(pos, controlWidth - 30));
			control.Position = pos;
		}

		Document buildOutputDoc;
		void HandleBinLogClicked (object sender, EventArgs e)
		{
			if (BuildOutput.IsFeatureEnabled) {
				OpenBuildOutputViewDocument ().Ignore ();
			}
		}

		async Task OpenBuildOutputViewDocument () 
		{
			if (buildOutputViewContent == null) {
				buildOutputViewContent = new BuildOutputViewContent (buildOutput);
				buildOutputDoc = await IdeApp.Workbench.OpenDocument (buildOutputViewContent, true);
				buildOutputDoc.Closed += BuildOutputDocClosed;
			} else if (buildOutputDoc != null) {
				buildOutputDoc.Select ();
			}
		}

		void BuildOutputDocClosed (object sender, EventArgs e)
		{
			buildOutputViewContent?.Dispose ();
			buildOutputDoc.Closed -= BuildOutputDocClosed;
			buildOutputViewContent = null;
			buildOutputDoc = null;
		}

		class DescriptionCellRendererText : CellRendererText
		{
			public int PreferedMaxWidth { get; set; }

			protected override void OnGetPreferredWidth (Widget widget, out int minimum_size, out int natural_size)
			{
				int originalWrapWidth = WrapWidth;
				WrapWidth = -1;
				// Calculate the width with WrapWidth=-1 which gives us the
				// width of the text in one line (without wrapping)
				base.OnGetPreferredWidth (widget, out int oneLineMinimum, out int oneLineNatural);
				WrapWidth = originalWrapWidth;
				// The height is computed by GTK from the final column width (OnGetPreferredHeightForWidth),
				// so only the width is limited here.
				minimum_size = Math.Min (oneLineMinimum, PreferedMaxWidth);
				natural_size = Math.Min (oneLineNatural, PreferedMaxWidth);
			}
		}

		/// <summary>Paned that reports its size allocation (GTK 4 has no size-allocate signal).</summary>
		sealed class SizeNotifyingPaned : Paned
		{
			public SizeNotifyingPaned () : base (Orientation.Horizontal)
			{
			}

			/// <summary>Raised after each allocation with the new width and height.</summary>
			public event Action<int, int> Allocated;

			protected override void OnSizeAllocate (int width, int height, int baseline)
			{
				base.OnSizeAllocate (width, height, baseline);
				Allocated?.Invoke (width, height);
			}
		}
	}

	internal static class TaskListEntryExtensions
	{
		public static string GetPath (this TaskListEntry task)
		{
			if (task.WorkspaceObject != null)
				return FileService.AbsoluteToRelativePath (task.WorkspaceObject.BaseDirectory, task.FileName);

			return task.FileName;
		}

		public static string GetProject (this TaskListEntry task)
		{
			return (task != null && task.WorkspaceObject is SolutionFolderItem) ? task.WorkspaceObject.Name : string.Empty;
		}

		public static string GetProjectWithExtension (this TaskListEntry task)
		{
			return (task != null && task.WorkspaceObject is SolutionItem) ? Path.GetFileName (((SolutionItem)task.WorkspaceObject).FileName) : string.Empty;
		}

		public static string GetFile (this TaskListEntry task)
		{
			string tmpPath = "";
			string fileName = "";
			try {
				tmpPath = GetPath (task);
				fileName = Path.GetFileName (tmpPath);
			} catch (Exception) {
				fileName = tmpPath;
			}
			return fileName;
		}
	}
}
