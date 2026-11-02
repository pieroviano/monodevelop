// 
// FindInFilesDialog.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
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
using System.Threading;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components;
using Gtk;
using System.Collections.Generic;
using MonoDevelop.Ide.Gui.Content;
using System.Threading.Tasks;
using MonoDevelop.Components.AtkCocoaHelper;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace MonoDevelop.Ide.FindInFiles
{
	public enum PathMode {
		Absolute,
		Relative,
		Hidden
	}

	public partial class FindInFilesDialog : Gtk.Dialog
	{
		readonly bool writeScope = true;

		enum SearchScope {
			WholeWorkspace,
			CurrentProject,
			AllOpenFiles,
			Directories,
			CurrentDocument,
			Selection
		}

		CheckButton checkbuttonRecursively;
		ComboBoxText comboboxentryReplace;
		ComboBoxText comboboxentryPath;
		MonoDevelop.Components.SearchEntry searchentryFileMask;
		Button buttonBrowsePaths;
		Button buttonReplace;
		Label labelFileMask;
		Label labelReplace;
		Label labelPath;
		Gtk.Box hboxPath;

		Properties properties = null;
		bool replaceMode = false;
		bool isDestroyed;

		/// <summary>The text entry of a combo box created with <see cref="ComboBoxText.NewWithEntry"/>.</summary>
		static Entry GetEntry (ComboBoxText comboBox)
		{
			return (Entry) comboBox.Child;
		}

		static void SetButtonIcon (Button button, string stockIcon)
		{
			Label label = new Label (button.Label);
			var hbox = new Gtk.Box (Gtk.Orientation.Horizontal, 2) {
				Halign = Align.Center,
				Valign = Align.Center,
			};
			ImageView image = new ImageView ();

			image.Image = ImageService.GetIcon (stockIcon, StockIconSize.Menu);
			hbox.Append (image);
			hbox.Append (label);

			button.Child = hbox;
		}

		FindInFilesDialog (bool showReplace, string directory) : this (showReplace)
		{
			comboboxScope.Active = (int)SearchScope.Directories;
			GetEntry (comboboxentryPath).Text = directory;
			writeScope = false;
		}

		public static string FormatPatternToSelectionOption (string pattern, bool regex)
		{
			if (pattern == null)
				return null;
			if (regex) {
				var sb = new StringBuilder ();
				foreach (var ch in pattern) {
					if (!char.IsLetterOrDigit (ch))
						sb.Append ('\\');
					sb.Append (ch);
				}
				return sb.ToString ();
			}
			return pattern;
		}

		FindInFilesDialog (bool showReplace)
		{
			Build ();
			IdeTheme.ApplyTheme (this);

			// The dialog is at least 480 pixels wide.
			ContentArea.WidthRequest = 480;

			properties = PropertyService.Get ("MonoDevelop.FindReplaceDialogs.SearchOptions", new Properties ());
			SetButtonIcon (toggleReplaceInFiles, "gtk-find-and-replace");
			SetButtonIcon (toggleFindInFiles, "gtk-find");

			// If we have an active floating window, attach the dialog to it. Otherwise use the main IDE window.
			var current_toplevel = Gtk.Window.ListToplevels ().FirstOrDefault (x => x.IsActive);
			if (current_toplevel is Components.DockNotebook.DockWindow)
				TransientFor = current_toplevel;
			else
				TransientFor = IdeApp.Workbench.RootWindow;

			toggleReplaceInFiles.Active = showReplace;
			toggleFindInFiles.Active = !showReplace;

			toggleFindInFiles.Toggled += delegate {
				if (toggleFindInFiles.Active) {
					Title = GettextCatalog.GetString ("Find in Files");
					HideReplaceUI ();
				}
			};

			toggleReplaceInFiles.Toggled += delegate {
				if (toggleReplaceInFiles.Active) {
					Title = GettextCatalog.GetString ("Replace in Files");
					ShowReplaceUI ();
				}
			};

			buttonSearch.Clicked += HandleSearchClicked;
			buttonClose.Clicked += (sender, e) => Destroy ();
			CloseRequest += (o, args) => {
				Destroy ();
				args.RetVal = true;
			};
			DefaultWidget = buttonSearch;

			buttonStop.Clicked += ButtonStopClicked;

			var workspace = IdeApp.Workspace;
			if (workspace != null && workspace.GetAllSolutions ().Count() == 1) {
				comboboxScope.AppendText (GettextCatalog.GetString ("Whole solution"));
			} else {
				comboboxScope.AppendText (GettextCatalog.GetString ("All solutions"));
			}
			comboboxScope.AppendText (GettextCatalog.GetString ("Current project"));
			comboboxScope.AppendText (GettextCatalog.GetString ("All open files"));
			comboboxScope.AppendText (GettextCatalog.GetString ("Directories"));
			comboboxScope.AppendText (GettextCatalog.GetString ("Current document"));
			comboboxScope.AppendText (GettextCatalog.GetString ("Selection"));

			comboboxScope.Changed += HandleScopeChanged;

			InitFromProperties ();

			// The toggle state was set before the handlers were connected: apply it explicitly.
			if (showReplace) {
				Title = GettextCatalog.GetString ("Replace in Files");
				ShowReplaceUI ();
			} else {
				Title = GettextCatalog.GetString ("Find in Files");
				HideReplaceUI ();
			}

			if (IdeApp.Workbench.ActiveDocument != null) {
				var view = IdeApp.Workbench.ActiveDocument.GetContent<ITextView>(true);
				if (view != null) {
					string selectedText = FormatPatternToSelectionOption (view.Selection.SelectedSpans.FirstOrDefault ().GetText(), properties.Get ("RegexSearch", false));
					if (!string.IsNullOrEmpty (selectedText)) {
						if (selectedText.Any (c => c == '\n' || c == '\r')) {
//							comboboxScope.Active = ScopeSelection;
						} else {
							if (comboboxScope.Active == (int) SearchScope.Selection)
								comboboxScope.Active = (int) SearchScope.CurrentDocument;
							GetEntry (comboboxentryFind).Text = selectedText;
						}
					} else if (comboboxScope.Active == (int) SearchScope.Selection) {
						comboboxScope.Active = (int) SearchScope.CurrentDocument;
					}

				}
			}
			GetEntry (comboboxentryFind).SelectRegion (0, (comboboxentryFind.ActiveText ?? "").Length);
			comboboxentryFind.GrabFocus ();
			UpdateStopButton ();
			UpdateSensitivity ();
			if (!buttonSearch.Sensitive) {
				comboboxScope.Active = (int)SearchScope.Directories;
			}

			updateTimer = GLib.Timeout.Add (750, delegate {
				UpdateSensitivity ();
				return true;
			});
			SetupAccessibility ();
		}

		void SetupAccessibility ()
		{
			comboboxentryFind.SetCommonAccessibilityAttributes ("FindInFilesDialog.comboboxentryFind",
												labelFind,
												GettextCatalog.GetString ("Enter string to find"));

			comboboxScope.SetCommonAccessibilityAttributes ("FindInFilesDialog.comboboxScope",
				labelScope,
				GettextCatalog.GetString ("Select where to search"));
		}

		void SetupAccessibilityForReplace ()
		{
			comboboxentryReplace.SetCommonAccessibilityAttributes ("FindInFilesDialog.comboboxentryReplace",
											labelReplace,
											GettextCatalog.GetString ("Enter string to replace"));
		}

		void SetupAccessibilityForPath ()
		{
			comboboxentryPath.SetCommonAccessibilityAttributes ("FindInFilesDialog.comboboxentryPath",
												labelPath,
												GettextCatalog.GetString ("Enter the Path"));

			buttonBrowsePaths.SetCommonAccessibilityAttributes ("FindInFilesDialog.buttonBrowsePaths",
				GettextCatalog.GetString ("Browse Path"),
				GettextCatalog.GetString ("Select a folder"));
		}

		void SetupAccessibilityForSearch ()
		{
			searchentryFileMask.SetEntryAccessibilityAttributes ("FindInFilesDialog.searchentryFileMask",
				labelFileMask.Text,
				GettextCatalog.GetString ("Enter the file mask"));
		}

		/// <summary>Inserts a new row at <paramref name="row"/>, shifting the following rows down.</summary>
		static void TableAddRow (Gtk.Grid table, int row, Widget column1, Widget column2)
		{
			table.InsertRow (row);

			if (column1 != null) {
				column1.Valign = Align.Center;
				column1.Halign = Align.Start;
				table.Attach (column1, 0, row, 1, 1);
			}

			if (column2 != null) {
				column2.Valign = Align.Center;
				column2.Hexpand = true;
				table.Attach (column2, 1, row, 1, 1);
			}
		}

		/// <summary>Removes the row holding the given widgets (they are unparented, which destroys them).</summary>
		static void TableRemoveRow (Gtk.Grid table, int row, Widget column1, Widget column2)
		{
			table.RemoveRow (row);
		}

		static int TableGetRowForItem (Gtk.Grid table, Widget item)
		{
			table.QueryChild (item, out _, out int row, out _, out _);
			return row;
		}

		void ShowReplaceUI ()
		{
			if (replaceMode)
				return;

			labelReplace = new Label { Xalign = 0f, UseUnderline = true };
			labelReplace.LabelProp = GettextCatalog.GetString ("_Replace:");
			comboboxentryReplace = ComboBoxText.NewWithEntry ();
			labelReplace.MnemonicWidget = comboboxentryReplace;
			LoadHistory ("MonoDevelop.FindReplaceDialogs.ReplaceHistory", comboboxentryReplace);
			SetupAccessibilityForReplace ();

			TableAddRow (tableFindAndReplace, 1, labelReplace, comboboxentryReplace);

			buttonReplace = DesignerLayout.Button (GettextCatalog.GetString ("R_eplace"));
			buttonReplace.Clicked += HandleReplaceClicked;

			AddActionWidget (buttonReplace, 0);
			DefaultWidget = buttonReplace;

			replaceMode = true;
		}

		void HideReplaceUI ()
		{
			if (!replaceMode)
				return;

			buttonReplace.Destroy ();
			buttonReplace = null;

			DefaultWidget = buttonSearch;

			StoreHistory ("MonoDevelop.FindReplaceDialogs.ReplaceHistory", comboboxentryReplace);
			TableRemoveRow (tableFindAndReplace, TableGetRowForItem (tableFindAndReplace, labelReplace), labelReplace, comboboxentryReplace);
			comboboxentryReplace = null;
			labelReplace = null;

			replaceMode = false;
		}

		void ShowDirectoryPathUI ()
		{
			if (labelPath != null)
				return;

			// We want to add the Path combo box right below the Scope
			int row = TableGetRowForItem (tableFindAndReplace, labelScope) + 1;

			// DirectoryScope
			labelPath = new Label {
				LabelProp = GettextCatalog.GetString ("_Path:"),
				UseUnderline = true,
				Xalign = 0f
			};

			hboxPath = new Gtk.Box (Gtk.Orientation.Horizontal, 0);
			comboboxentryPath = ComboBoxText.NewWithEntry ();
			comboboxentryPath.Hexpand = true;
			LoadHistory ("MonoDevelop.FindReplaceDialogs.PathHistory", comboboxentryPath);
			hboxPath.Append (comboboxentryPath);

			labelPath.MnemonicWidget = comboboxentryPath;

			buttonBrowsePaths = new Button { Label = "…" };
			buttonBrowsePaths.Clicked += ButtonBrowsePathsClicked;
			hboxPath.Append (buttonBrowsePaths);

			// Add the Directory Path row to the table
			TableAddRow (tableFindAndReplace, row++, labelPath, hboxPath);

			// Add a checkbox for searching the directory recursively...
			checkbuttonRecursively = new CheckButton {
				Label = GettextCatalog.GetString ("Re_cursively"),
				Active = properties.Get ("SearchPathRecursively", true),
				UseUnderline = true
			};

			TableAddRow (tableFindAndReplace, row, null, checkbuttonRecursively);

			SetupAccessibilityForPath ();
		}

		void HideDirectoryPathUI ()
		{
			if (labelPath == null)
				return;

			StorePathSettings ();

			int row = TableGetRowForItem (tableFindAndReplace, checkbuttonRecursively);
			TableRemoveRow (tableFindAndReplace, row, null, checkbuttonRecursively);
			checkbuttonRecursively = null;

			row = TableGetRowForItem (tableFindAndReplace, labelPath);
			TableRemoveRow (tableFindAndReplace, row, labelPath, hboxPath);
			// comboboxentryPath and buttonBrowsePaths are destroyed with hboxPath
			buttonBrowsePaths = null;
			comboboxentryPath = null;
			labelPath = null;
			hboxPath = null;
		}

		/// <summary>Persists the directory history and the recursive option (GTK 4 has no widget "destroyed" signal to hook).</summary>
		void StorePathSettings ()
		{
			if (comboboxentryPath != null)
				StoreHistory ("MonoDevelop.FindReplaceDialogs.PathHistory", comboboxentryPath);
			if (checkbuttonRecursively != null)
				properties.Set ("SearchPathRecursively", checkbuttonRecursively.Active);
		}

		void ShowFileMaskUI ()
		{
			if (labelFileMask != null)
				return;

			int row;

			if (checkbuttonRecursively != null)
				row = TableGetRowForItem (tableFindAndReplace, checkbuttonRecursively) + 1;
			else
				row = TableGetRowForItem (tableFindAndReplace, labelScope) + 1;

			labelFileMask = new Label {
				LabelProp = GettextCatalog.GetString ("_File Mask:"),
				UseUnderline = true,
				Xalign = 0f
			};

			searchentryFileMask = new MonoDevelop.Components.SearchEntry () {
				ForceFilterButtonVisible = false,
				IsCheckMenu = true,
				ActiveFilterID = 0,
				Visible = true,
				Ready = true,
			};


			searchentryFileMask.Query = properties.Get ("MonoDevelop.FindReplaceDialogs.FileMask", "");

			searchentryFileMask.Entry.ActivatesDefault = true;

			SetupAccessibilityForSearch ();

			TableAddRow (tableFindAndReplace, row, labelFileMask, searchentryFileMask);
		}

		void HideFileMaskUI ()
		{
			if (labelFileMask == null)
				return;

			properties.Set ("MonoDevelop.FindReplaceDialogs.FileMask", searchentryFileMask.Query);

			int row = TableGetRowForItem (tableFindAndReplace, labelFileMask);
			TableRemoveRow (tableFindAndReplace, row, labelFileMask, searchentryFileMask);
			searchentryFileMask = null;
			labelFileMask = null;
		}

		void HandleScopeChanged (object sender, EventArgs e)
		{
			switch ((SearchScope)comboboxScope.Active) {
			case SearchScope.WholeWorkspace:
				HideDirectoryPathUI ();
				ShowFileMaskUI ();
				break;
			case SearchScope.CurrentProject:
				HideDirectoryPathUI ();
				ShowFileMaskUI ();
				break;
			case SearchScope.AllOpenFiles:
				HideDirectoryPathUI ();
				ShowFileMaskUI ();
				break;
			case SearchScope.Directories:
				ShowDirectoryPathUI ();
				ShowFileMaskUI ();
				break;
			case SearchScope.CurrentDocument:
				HideDirectoryPathUI ();
				HideFileMaskUI ();
				break;
			case SearchScope.Selection:
				HideDirectoryPathUI ();
				HideFileMaskUI ();
				break;
			}
			UpdateSensitivity ();
		}


		void UpdateSensitivity ()
		{
			bool isSensitive = true;
			switch ((SearchScope)comboboxScope.Active) {
			case SearchScope.WholeWorkspace:
				isSensitive = IdeApp.Workspace.IsOpen;
				break;
			case SearchScope.CurrentProject:
				isSensitive = IdeApp.ProjectOperations.CurrentSelectedProject != null;
				break;
			case SearchScope.AllOpenFiles:
				isSensitive = IdeApp.Workbench.Documents.Count > 0;
				break;
			case SearchScope.Directories:
				isSensitive = true;
				break;
			case SearchScope.CurrentDocument:
				isSensitive = IdeApp.Workbench.ActiveDocument != null;
				break;
			case SearchScope.Selection:
				isSensitive = IdeApp.Workbench.ActiveDocument != null;
				break;
			}
			buttonSearch.Sensitive = isSensitive;
			if (buttonReplace != null)
				buttonReplace.Sensitive = isSensitive;
		}

		void ButtonBrowsePathsClicked (object sender, EventArgs e)
		{
			var dlg = new SelectFolderDialog (GettextCatalog.GetString ("Select directory")) {
				TransientFor = this,
			};

			string defaultFolder = GetEntry (comboboxentryPath).Text;
			if (string.IsNullOrEmpty (defaultFolder))
				defaultFolder = IdeApp.Preferences.ProjectsDefaultPath;
			if (!string.IsNullOrEmpty (defaultFolder))
				dlg.CurrentFolder = defaultFolder;

			if (dlg.Run ())
				GetEntry (comboboxentryPath).Text = dlg.SelectedFile;
		}

		const char historySeparator = '\n';
		void InitFromProperties ()
		{
			comboboxScope.Active = properties.Get ("Scope", (int) SearchScope.WholeWorkspace);

			//checkbuttonRecursively.Active    = properties.Get ("SearchPathRecursively", true);
			//checkbuttonFileMask.Active       = properties.Get ("UseFileMask", false);
			checkbuttonCaseSensitive.Active = properties.Get ("CaseSensitive", false);
			checkbuttonWholeWordsOnly.Active = properties.Get ("WholeWordsOnly", false);
			checkbuttonRegexSearch.Active = properties.Get ("RegexSearch", false);

			LoadHistory ("MonoDevelop.FindReplaceDialogs.FindHistory", comboboxentryFind);

//			LoadHistory ("MonoDevelop.FindReplaceDialogs.PathHistory", comboboxentryPath);
//			LoadHistory ("MonoDevelop.FindReplaceDialogs.FileMaskHistory", comboboxentryFileMask);
		}

		static void LoadHistory (string propertyName, ComboBoxText comboBox)
		{
			var entry = GetEntry (comboBox);
			entry.ActivatesDefault = true;

			// The combo box owns its list model: the history items are appended to it and the entry completion reads them from there.
			var history = PropertyService.Get<string> (propertyName);
			if (!string.IsNullOrEmpty (history)) {
				string[] items = history.Split (historySeparator);
				foreach (string item in items) {
					if (string.IsNullOrEmpty (item))
						continue;
					comboBox.AppendText (item);
				}
				entry.Text = items[0];
			}

			var ec = new EntryCompletion ();
			ec.Model = comboBox.Model;
			ec.TextColumn = 0;
			entry.Completion = ec;
		}

		void StorePoperties ()
		{
			if (writeScope)
				properties.Set ("Scope", comboboxScope.Active);
//			properties.Set ("SearchPathRecursively", checkbuttonRecursively.Active);
//			properties.Set ("UseFileMask", checkbuttonFileMask.Active);
			properties.Set ("CaseSensitive", checkbuttonCaseSensitive.Active);
			properties.Set ("WholeWordsOnly", checkbuttonWholeWordsOnly.Active);
			properties.Set ("RegexSearch", checkbuttonRegexSearch.Active);

			StoreHistory ("MonoDevelop.FindReplaceDialogs.FindHistory", comboboxentryFind);
			if (replaceMode)
				StoreHistory ("MonoDevelop.FindReplaceDialogs.ReplaceHistory", comboboxentryReplace);
			StorePathSettings ();
			if (searchentryFileMask != null)
				properties.Set ("MonoDevelop.FindReplaceDialogs.FileMask", searchentryFileMask.Query);
//			StoreHistory ("MonoDevelop.FindReplaceDialogs.PathHistory", comboboxentryPath);
			//StoreHistory ("MonoDevelop.FindReplaceDialogs.FileMaskHistory", comboboxentryFileMask);
		}

		static void StoreHistory (string propertyName, ComboBoxText comboBox)
		{
			var store = comboBox.Model;
			var history = new List<string> ();
			TreeIter iter;
			if (store.GetIterFirst (out iter)) {
				do {
					var value = new GLib.Value ();
					store.GetValue (iter, 0, ref value);
					history.Add ((string)value.Val);
					value.Dispose ();
				} while (store.IterNext (ref iter));
			}
			const int limit = 20;
			if (history.Count > limit) {
				history.RemoveRange (history.Count - (history.Count - limit), history.Count - limit);
			}
			var text = GetEntry (comboBox).Text;
			if (history.Contains (text))
				history.Remove (text);
			history.Insert (0, text);
			PropertyService.Set (propertyName, string.Join (historySeparator.ToString (), history.ToArray ()));
		}

		public override void Destroy ()
		{
			if (isDestroyed) {
				base.Destroy ();
				return;
			}
			isDestroyed = true;

			if (resultPad != null) {
				var resultWidget = resultPad.Control.GetNativeWidget<SearchResultWidget> ();
				if (resultWidget.ResultCount > 0) {
					resultPad.Window.Activate (true);
				}
			}

			if (updateTimer != 0) {
				GLib.Source.Remove (updateTimer);
				updateTimer = 0;
			}
			StorePoperties ();
			if (currentFindDialog == this)
				currentFindDialog = null;
			base.Destroy ();
		}

		public static void ShowFind ()
		{
			ShowSingleInstance (new FindInFilesDialog (false));
		}

		public static void ShowReplace ()
		{
			ShowSingleInstance (new FindInFilesDialog (true));
		}

		public static void FindInPath (string path)
		{
			ShowSingleInstance (new FindInFilesDialog (false, path));
		}

		static FindInFilesDialog currentFindDialog;

		static void ShowSingleInstance (FindInFilesDialog newDialog)
		{
			if (currentFindDialog != null) {
				currentFindDialog.Destroy ();
			}
			currentFindDialog = newDialog;
			MessageService.PlaceDialog (currentFindDialog, null);
			currentFindDialog.Present ();
		}

		Scope GetScope ()
		{
			Scope scope = null;

			switch ((SearchScope) comboboxScope.Active) {
			case SearchScope.CurrentDocument:
				scope = new DocumentScope ();
				break;
			case SearchScope.Selection:
				scope = new SelectionScope ();
				break;
			case SearchScope.WholeWorkspace:
				scope = new WholeSolutionScope ();
				break;
			case SearchScope.CurrentProject:
				var currentSelectedProject = IdeApp.ProjectOperations.CurrentSelectedProject;
				if (currentSelectedProject != null) {
					scope = new WholeProjectScope (currentSelectedProject);
					break;
				}
				return null;
			case SearchScope.AllOpenFiles:
				scope = new AllOpenFilesScope ();
				break;
			case SearchScope.Directories: 
				if (!System.IO.Directory.Exists (GetEntry (comboboxentryPath).Text)) {
					MessageService.ShowError (string.Format (GettextCatalog.GetString ("Directory not found: {0}"),
						GetEntry (comboboxentryPath).Text));
					return null;
				}
				
				scope = new DirectoryScope (GetEntry (comboboxentryPath).Text, checkbuttonRecursively.Active);
				break;
			default:
				throw new ApplicationException ("Unknown scope:" + comboboxScope.Active);
			}
			
			return scope;
		}

		FilterOptions GetFilterOptions ()
		{
			return new FilterOptions {
				FileMask = searchentryFileMask != null && !string.IsNullOrEmpty (searchentryFileMask.Query) ? searchentryFileMask.Query : "*",
				CaseSensitive = checkbuttonCaseSensitive.Active,
				RegexSearch = checkbuttonRegexSearch.Active,
				WholeWordsOnly = checkbuttonWholeWordsOnly.Active
			};
		}

		static FindReplace find;
		void HandleReplaceClicked (object sender, EventArgs e)
		{
			SearchReplace (GetEntry (comboboxentryFind).Text, GetEntry (comboboxentryReplace).Text ?? "", GetScope (), GetFilterOptions (), () => UpdateStopButton (), UpdateResultPad);
		}

		void HandleSearchClicked (object sender, EventArgs e)
		{
			SearchReplace (GetEntry (comboboxentryFind).Text, null, GetScope (), GetFilterOptions (), () => UpdateStopButton (), UpdateResultPad);
		}

		static CancellationTokenSource searchTokenSource = new CancellationTokenSource ();
		static Task currentTask;
		uint updateTimer;
		SearchResultPad resultPad;

		void UpdateStopButton ()
		{
			buttonStop.Sensitive = currentTask != null && !currentTask.IsCompleted;
		}

		void UpdateResultPad (SearchResultPad pad)
		{
			resultPad = pad;
		}

		void ButtonStopClicked (object sender, EventArgs e)
		{
			searchTokenSource.Cancel ();
		}

		internal static void SearchReplace (string findPattern, string replacePattern, Scope scope, FilterOptions options, System.Action UpdateStopButton, System.Action<SearchResultPad> UpdateResultPad)
		{
			if (find != null && find.IsRunning) {
				if (!MessageService.Confirm (GettextCatalog.GetString ("There is a search already in progress. Do you want to stop it?"), AlertButton.Stop))
					return;
			}
			searchTokenSource.Cancel ();

			if (scope == null)
				return;
			
			find = new FindReplace ();

			string pattern = findPattern;
			if (String.IsNullOrEmpty (pattern))
				return;
			if (!find.ValidatePattern (options, pattern)) {
				MessageService.ShowError (GettextCatalog.GetString ("Search pattern is invalid"));
				return;
			}

			if (replacePattern != null && !find.ValidatePattern (options, replacePattern)) {
				MessageService.ShowError (GettextCatalog.GetString ("Replace pattern is invalid"));
				return;
			}
			var cancelSource = new CancellationTokenSource ();
			searchTokenSource = cancelSource;
			var token = cancelSource.Token;
			currentTask = Task.Run (delegate {
				using (SearchProgressMonitor searchMonitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true)) {

					searchMonitor.PathMode = scope.PathMode;

					if (UpdateResultPad != null) {
						Application.Invoke ((o, args) => {
							UpdateResultPad (searchMonitor.ResultPad);
						});
					}

					searchMonitor.ReportStatus (scope.GetDescription (options, pattern, null));
					if (UpdateStopButton != null) {
						Application.Invoke ((o, args) => {
							UpdateStopButton ();
						});
					}

					DateTime timer = DateTime.Now;
					string errorMessage = null;
						
					try {
						var results = new List<SearchResult> ();
						foreach (SearchResult result in find.FindAll (scope, searchMonitor, pattern, replacePattern, options, token)) {
							if (token.IsCancellationRequested)
								return;
							results.Add (result);
						}
						searchMonitor.ReportResults (results);
					} catch (Exception ex) {
						errorMessage = ex.Message;
						LoggingService.LogError ("Error while search", ex);
					}
						
					string message = null;
					if (errorMessage != null) {
						message = GettextCatalog.GetString ("The search could not be finished: {0}", errorMessage);
						searchMonitor.ReportError (message, null);
					} else if (!searchMonitor.CancellationToken.IsCancellationRequested) {
						string matches = string.Format (GettextCatalog.GetPluralString ("{0} match found", "{0} matches found", find.FoundMatchesCount), find.FoundMatchesCount);
						string files = string.Format (GettextCatalog.GetPluralString ("in {0} file.", "in {0} files.", find.SearchedFilesCount), find.SearchedFilesCount);
						message = GettextCatalog.GetString ("Search completed.") + Environment.NewLine + matches + " " + files;
						searchMonitor.ReportSuccess (message);
					}
					if (message != null)
						searchMonitor.ReportStatus (message);
					searchMonitor.Log.WriteLine (GettextCatalog.GetString ("Search time: {0} seconds."), (DateTime.Now - timer).TotalSeconds);
				}
				if (UpdateStopButton != null) {
					Application.Invoke ((o, args) => {
						UpdateStopButton ();
					});
				}
			});
		}
	}
}
