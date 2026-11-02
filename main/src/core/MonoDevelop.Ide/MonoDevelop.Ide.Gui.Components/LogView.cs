// 
// LogView.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Fonts;
using Pango;

namespace MonoDevelop.Ide.Gui.Components
{
	public class LogView : Gtk.Box
	{
		TextBuffer buffer;
		TextView textEditorControl;
		TextMark endMark;

		TextTag normalTag;
		TextTag bold;
		TextTag errorTag;
		TextTag consoleLogTag;
		TextTag debugTag;
		List<TextTag> tags = new List<TextTag> ();

		readonly Queue<QueuedUpdate> updates = new Queue<QueuedUpdate> ();
		QueuedTextWrite lastTextWrite;
		GLib.TimeoutHandler outputDispatcher;
		bool outputDispatcherRunning = false;
		readonly Regex consoleTextSanitizerRegex = new Regex ("(\\e[[0-9]*m)|(\b)", RegexOptions.Compiled);

		const int MAX_BUFFER_LENGTH = 4000 * 1024;

		/// <summary>
		/// Incremented any time the pad is cleared, so callers can know when they should stop using it
		/// </summary>
		internal int Cookie = int.MinValue;

		/// <summary>
		/// The log text view allows the user to jump to the source of an error/warning
		/// by double clicking on the line in the text view.
		/// </summary>
		public class LogTextView : TextView
		{
			readonly CommandEntrySet menuSet;

			public LogTextView (TextBuffer buf) : base (buf)
			{
				menuSet = new CommandEntrySet ();
				SetupMenu ();
			}

			public LogTextView () 
			{
				menuSet = new CommandEntrySet ();
				SetupMenu ();
			}

			void SetupMenu ()
			{
				menuSet.AddItem (EditCommands.Copy);
				menuSet.AddItem (EditCommands.Cut);
				menuSet.AddItem (EditCommands.Paste);
				menuSet.AddItem (EditCommands.SelectAll);

				// Capture phase: sees the click before the TextView changes the cursor position (was OnButtonPressEvent).
				this.AddClickHandler (OnButtonPressed, null, 0, PropagationPhase.Capture);
			}

			[CommandHandler (EditCommands.SelectAll)]
			void SelectAllText ()
			{
				TextIter start;
				TextIter end;

				Buffer.GetBounds (out start, out end);
				Buffer.SelectRange (start, end);
			}

			[CommandHandler (EditCommands.Copy)]
			void CopyText ()
			{
				TextIter start;
				TextIter end;

				if (Buffer.HasSelection && Buffer.GetSelectionBounds (out start, out end)) {
					var text = Buffer.GetText (start, end, false);
					ClipboardHelper.SetText (text);

					if (Platform.IsLinux) {
						// gtk has different clipboards for CLIPBOARD and PRIMARY only on Linux.
						ClipboardHelper.SetText (text, primary: true);
					}
				}
			}

			[CommandHandler (EditCommands.Cut)]
			void CutText ()
			{
				if (!Buffer.HasSelection)
					return;
				if (Editable) {
					Buffer.CutClipboard (ClipboardHelper.Clipboard, false);
				} else {
					CopyText ();
				}
			}

			[CommandHandler (EditCommands.Paste)]
			void PasteText ()
			{
				if (Editable) {
					var insert = Buffer.GetIterAtOffset (Buffer.CursorPosition);
					Buffer.PasteClipboard (ClipboardHelper.Clipboard, ref insert, true);
				}
			}

			internal static bool TryExtractFileAndLine (string lineText, out string file, out int line)
			{
				file = null;
				line = 0;

				int i = lineText.Length - 1, mult = 1;
				int fileNameEnd = 0, fileNameStart = 0;
				int state = 0;
				while (i > 0) {
					char ch = lineText [i];
					switch (state) {
					case 0: // search line number start
						if (ch == '.')
							state = 1;
						else if (ch >= '0' && ch <= '9') {
							state = 1;
							goto case 1;
						} else if (!char.IsWhiteSpace (ch))
							return false;
						break;
					case 1: // read line number
						if (ch >= '0' && ch <= '9') {
							line += (ch - '0') * mult;
							mult *= 10;
						} else if (ch == ' ' || char.IsLetter (ch)) {
							state = 2;
						} else if (ch == ':') {
							state = 3;
							fileNameStart = fileNameEnd = i;
						} else
							return false;
						break;
					case 2: // search file name end
						if (ch == ':') {
							state = 3;
							fileNameStart = fileNameEnd = i;
						} else if (ch != ' ' && !char.IsLetter (ch))
							return false;
						break;
					case 3: // search file name start
						if (ch == ' ') {
							if (lineText [i + 1] == '/' ||  // unix style start
							    (char.IsLetter (lineText [i + 1]) && lineText [i + 2] == ':' && lineText [i + 3] == '\\') // windows style start
							   )
								fileNameStart = i + 1;
						}
						break;
					}

					i--;
				}
				if (state == 3 && fileNameEnd - fileNameStart > 0) {
					file = lineText.Substring (fileNameStart, fileNameEnd - fileNameStart);
					return true;
				}
				return false;
			}

			void OnButtonPressed (ClickInfo evnt)
			{
				if (evnt.NPress == 1 && evnt.Button == 3) {
					IdeApp.CommandService.ShowContextMenu (this, evnt, menuSet, this);
				} else if (evnt.NPress == 2) {
					var cursorPos = Buffer.GetIterAtOffset (Buffer.CursorPosition);
					TextIter iterStart;
					TextIter iterEnd;
					string lineText;

					try {
						Buffer.GetIterAtLine (out iterStart, cursorPos.Line);
						iterEnd = Buffer.GetIterAtOffset (iterStart.Offset + iterStart.CharsInLine);

						lineText = Buffer.GetText (iterStart, iterEnd, true);
					} catch (Exception e) {
						LoggingService.LogError ("Error in getting text of the current line.", e);
						return;
					}
					string file;
					int lineNumber;

					if (TryExtractFileAndLine (lineText, out file, out lineNumber)) {
						if (!string.IsNullOrEmpty (file)) {
							bool fileExists;
							try {
								fileExists = File.Exists (file);
							} catch {
								fileExists = false;
							}
							if (fileExists)
								IdeApp.Workbench.OpenDocument (file, null, lineNumber, 1);
						}
					}
				}
			}
		}
		Box searchBox = new Box (Orientation.Horizontal, 0);
		MonoDevelop.Components.SearchEntry searchEntry = new MonoDevelop.Components.SearchEntry ();
		MonoDevelop.Components.CompactScrolledWindow scrollView = new MonoDevelop.Components.CompactScrolledWindow ();

		public LogView () : base (Orientation.Vertical, 0)
		{
			buffer = new TextBuffer (new TextTagTable ());
			textEditorControl = new LogTextView (buffer);
			textEditorControl.Editable = false;
			
			scrollView.Child = textEditorControl;
			scrollView.Vexpand = true;
			Append (scrollView);

			bold = new TextTag ("bold");
			bold.Weight = Weight.Bold;
			buffer.TagTable.Add (bold);
			
			errorTag = new TextTag ("error");
			errorTag.Foreground = Styles.ErrorForegroundColor.ToHexString (false);
			errorTag.Weight = Weight.Bold;
			buffer.TagTable.Add (errorTag);

			debugTag = new TextTag ("debug");
			debugTag.Foreground = Styles.InformationForegroundColor.ToHexString (false);
			buffer.TagTable.Add (debugTag);

			consoleLogTag = new TextTag ("consoleLog");
			consoleLogTag.Foreground = Styles.SecondaryTextColor.ToHexString (false);
			buffer.TagTable.Add (consoleLogTag);

			normalTag = new TextTag ("0");
			normalTag.LeftMargin = 10;
			buffer.TagTable.Add (normalTag);
			tags.Add (normalTag);

			endMark = buffer.CreateMark ("end-mark", buffer.EndIter, false);

			UpdateCustomFont ();
			IdeApp.Preferences.CustomOutputPadFont.Changed += HandleCustomFontChanged;
			
			outputDispatcher = new GLib.TimeoutHandler (outputDispatchHandler);

			InitSearchWidget ();
		}

		#region Searching
		Button buttonSearchForward;
		Button buttonSearchBackward;
		Button closeButton;

		static string currentSearchPattern;

		void InitSearchWidget ()
		{
			searchBox.MarginTop = searchBox.MarginBottom = searchBox.MarginStart = searchBox.MarginEnd = 4;
			var searchLabel = new Label (GettextCatalog.GetString ("Search:"));
			searchLabel.MarginEnd = 4;
			searchBox.Append (searchLabel);
			searchEntry.Hexpand = true;
			searchBox.Append (searchEntry);

			buttonSearchBackward = new Button ();
			buttonSearchBackward.CanFocus = true;
			buttonSearchBackward.HasFrame = false;
			buttonSearchBackward.TooltipText = GettextCatalog.GetString ("Find previous {0}", GetShortcut (SearchCommands.FindPrevious));
			buttonSearchBackward.Child = new ImageView ("gtk-go-up", MonoDevelop.Components.StockIconSize.Menu);
			buttonSearchBackward.Clicked += delegate {
				FindPrev ();
			};
			searchBox.Append (buttonSearchBackward);

			buttonSearchForward = new Button ();
			buttonSearchForward.CanFocus = true;
			buttonSearchForward.HasFrame = false;
			buttonSearchForward.TooltipText = GettextCatalog.GetString ("Find next {0}", GetShortcut (SearchCommands.FindNext));
			buttonSearchForward.Child = new ImageView ("gtk-go-down", MonoDevelop.Components.StockIconSize.Menu);
			buttonSearchForward.Clicked += delegate {
				FindNext ();
			};
			searchBox.Append (buttonSearchForward);

			closeButton = new Button ();
			closeButton.CanFocus = true;
			closeButton.HasFrame = false;
			closeButton.Child = new ImageView ("gtk-close", MonoDevelop.Components.StockIconSize.Menu);
			closeButton.Clicked += delegate {
				HideSearchBox ();
			};
			searchBox.Append (closeButton);

			searchEntry.Ready = true;
			searchEntry.Visible = true;
			searchEntry.Entry.AddKeyPressHandler ((keyval, keycode, state) => {
				if ((Gdk.Key)keyval == Gdk.Key.Escape) {
					HideSearchBox ();
					return true;
				}
				return false;
			});

			textEditorControl.AddKeyPressHandler ((keyval, keycode, state) => {
				if ((Gdk.Key)keyval == Gdk.Key.Escape) {
					HideSearchBox ();
					return true;
				}
				return false;
			});
			searchEntry.Entry.Changed += delegate {
				currentSearchPattern = searchEntry.Entry.Text;
			};
			searchEntry.Entry.Activated += delegate {
				FindNext ();
			};
		}

		void UpdateSearchEntrySearchPattern ()
		{
			searchEntry.Entry.Text = currentSearchPattern ?? "";
		}

		void ShowSearchBox ()
		{
			UpdateSearchEntrySearchPattern ();
			if (searchBox.Parent == null)
				Prepend (searchBox);
			searchEntry.Entry.GrabFocus ();
		}

		void HideSearchBox ()
		{
			if (searchBox.Parent == this)
				Remove (searchBox);
			textEditorControl.GrabFocus ();
		}

		[CommandHandler (SearchCommands.Find)]
		void Find ()
		{
			ShowSearchBox ();
		}

		static StringComparison GetComparer ()
		{
			if (PropertyService.Get ("AutoSetPatternCasing", false)) {
				if (currentSearchPattern != null && currentSearchPattern.Any (Char.IsUpper))
					return StringComparison.Ordinal;
			}

			return StringComparison.OrdinalIgnoreCase;
		}

		[CommandHandler (SearchCommands.FindNext)]
		void FindNext ()
		{
			if (string.IsNullOrEmpty (currentSearchPattern))
				return;
			int searchPosition = buffer.CursorPosition;
			TextIter start;
			TextIter end;

			if (buffer.HasSelection && buffer.GetSelectionBounds (out start, out end))
				searchPosition = end.Offset;

			var comparer = GetComparer ();
			var text = buffer.Text;
			var idx = text.IndexOf (currentSearchPattern, searchPosition, comparer);
			if (idx < 0) {
				idx = text.IndexOf (currentSearchPattern, 0, searchPosition, comparer);
			}

			if (idx >= 0) {
				var iter = buffer.GetIterAtOffset (idx + currentSearchPattern.Length);
				buffer.PlaceCursor (iter);
				buffer.SelectRange (buffer.GetIterAtOffset (idx), iter); 
				textEditorControl.ScrollToIter (iter, 0, false, 0, 0);
			}
		}

		[CommandHandler (SearchCommands.FindNextSelection)]
		void FindNextSelection ()
		{
			SetSearchPatternToSelection ();
			UpdateSearchEntrySearchPattern ();
			FindNext ();
		}

		[CommandHandler (SearchCommands.FindPrevious)]
		void FindPrev ()
		{
			if (string.IsNullOrEmpty (currentSearchPattern))
				return;
			int searchPosition = buffer.CursorPosition;
			TextIter start;
			TextIter end;

			if (buffer.HasSelection && buffer.GetSelectionBounds (out start, out end))
				searchPosition = start.Offset;

			var comparer = GetComparer ();
			var text = buffer.Text;
			var idx = text.LastIndexOf (currentSearchPattern, searchPosition, comparer);
			if (idx < 0) {
				idx = text.LastIndexOf (currentSearchPattern, text.Length, text.Length - searchPosition, comparer);
			}

			if (idx >= 0) {
				var iter = buffer.GetIterAtOffset (idx + currentSearchPattern.Length);
				buffer.PlaceCursor (iter);
				buffer.SelectRange (buffer.GetIterAtOffset (idx), iter); 
				textEditorControl.ScrollToIter (iter, 0, false, 0, 0);
			}

		}

		[CommandHandler (SearchCommands.FindPreviousSelection)]
		void FindPreviousSelection ()
		{
			SetSearchPatternToSelection ();
			UpdateSearchEntrySearchPattern ();
			FindPrev ();
		}

		void SetSearchPatternToSelection ()
		{
			TextIter start;
			TextIter end;

			if (buffer.HasSelection && buffer.GetSelectionBounds (out start, out end))
				currentSearchPattern = buffer.GetText (start, end, false);
		}

		static string GetShortcut (object commandId)
		{
			var key = IdeApp.CommandService.GetCommand (commandId).AccelKey;
			if (string.IsNullOrEmpty (key))
				return "";
			var nextShortcut = KeyBindingManager.BindingToDisplayLabel (key, false);
			return "(" + nextShortcut + ")";
		}
		#endregion

		public OutputProgressMonitor GetProgressMonitor ()
		{
			return GetProgressMonitor (clearConsole: true);
		}

		internal OutputProgressMonitor GetProgressMonitor (bool clearConsole)
		{
			return new LogViewProgressMonitor (this, clearConsole);
		}

		internal TextMark CreateMark ()
		{
			return buffer.CreateMark (null, buffer.EndIter, false);
		}

		public void Clear ()
		{
			lock (updates) {
				updates.Clear ();
				lastTextWrite = null;
				outputDispatcherRunning = false;
			}

			buffer.Clear();
		}
		
		void HandleCustomFontChanged (object sender, EventArgs e)
		{
			UpdateCustomFont ();
		}
		
		static int fontClassCounter;
		string fontClass;
		Gtk.CssProvider fontProvider;

		void UpdateCustomFont ()
		{
			// GTK 4: ModifyFont is gone; the font is applied through a CSS class unique to this view.
			var font = IdeApp.Preferences.CustomOutputPadFont ?? IdeServices.FontService.MonospaceFont;
			if (fontClass == null) {
				fontClass = "md-log-view-" + (++fontClassCounter);
				textEditorControl.AddCssClass (fontClass);
				fontProvider = new Gtk.CssProvider ();
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.AddProviderForDisplay (display, fontProvider, StyleProviderPriority.Application);
			}
			fontProvider.LoadFromString (font != null ? "textview." + fontClass + " { " + FontToCss (font) + " }" : "");
		}

		static string FontToCss (Pango.FontDescription font)
		{
			var css = new System.Text.StringBuilder ();
			if (!string.IsNullOrEmpty (font.Family))
				css.Append ("font-family: \"").Append (font.Family.Replace ("\"", "")).Append ("\"; ");
			if (font.Size > 0) {
				var size = font.Size / Pango.Scale.PangoScale;
				css.Append ("font-size: ").Append (size.ToString (System.Globalization.CultureInfo.InvariantCulture))
					.Append (font.SizeIsAbsolute ? "px; " : "pt; ");
			}
			css.Append ("font-weight: ").Append ((int)font.Weight).Append ("; ");
			if (font.Style == Pango.Style.Italic)
				css.Append ("font-style: italic; ");
			else if (font.Style == Pango.Style.Oblique)
				css.Append ("font-style: oblique; ");
			return css.ToString ();
		}
		
		//mechanism to to batch copy text when large amounts are being dumped
		bool outputDispatchHandler ()
		{
			lock (updates) {
				lastTextWrite = null;
				if (updates.Count == 0) {
					outputDispatcherRunning = false;
					return false;
				}

				if (!outputDispatcherRunning) {
					updates.Clear ();
					return false;
				}

				while (updates.Count > 0) {
					var up = updates.Dequeue ();
					up.Execute (this);
				}
			}
			return true;
		}

		void addQueuedUpdate (QueuedUpdate update)
		{
			lock (updates) {
				if (destroyed)
					return;
				
				updates.Enqueue (update);
				if (!outputDispatcherRunning) {
					GLib.Timeout.Add (50, outputDispatcher);
					outputDispatcherRunning = true;
				}
				lastTextWrite = update as QueuedTextWrite;
			}
		}

		protected void UnsafeBeginTask (LogViewProgressMonitor monitor, string name, int totalWork)
		{
			if (monitor != null) {
				var indent = monitor.Indent;
				var t = indent.Indent (name);

				if (buffer.TagTable.Lookup (t.Name) == null) {
					buffer.TagTable.Add (t);
				}
			}

			if (name != null)
				UnsafeAddText (null, Environment.NewLine + name + Environment.NewLine, normalTag, bold);
			else {
				UnsafeAddText (null, Environment.NewLine, normalTag, null);
			}

			var marker = buffer.CreateMark (name, buffer.EndIter, false);
			if (monitor != null) {
				monitor.Marker = marker;
			}
			UnsafeAddText (null, Environment.NewLine, normalTag, null);

			// Move the mark to the line before EndIter, so other text inserted at EndIter
			// doesn't move this mark.
			buffer.MoveMark (marker, buffer.GetIterAtOffset (buffer.CharCount - 1));
		}

		[Obsolete ("Use BeginTask (LogviewProgressMonitor, string, int) instead")]
		public void BeginTask (string name, int totalWork)
		{
			BeginTask (null, name, totalWork);
		}

		public void BeginTask (LogViewProgressMonitor monitor, string name, int totalWork)
		{
			var bt = new QueuedBeginTask (monitor, name, totalWork);
			addQueuedUpdate (bt);
		}

		[Obsolete ("Use EndTask (LogViewProgressMonitor) instead")]
		public void EndTask ()
		{
			EndTask (null);
		}

		public void EndTask (LogViewProgressMonitor monitor)
		{
			var et = new QueuedEndTask (monitor);
			addQueuedUpdate (et);
		}
		
		protected void UnsafeEndTask (LogViewProgressMonitor monitor)
		{
			if (monitor != null) {
				if (buffer.TagTable.Lookup (monitor.Indent.IndentTag.Name) != null) {
					buffer.TagTable.Remove (monitor.Indent.IndentTag);
				}
				monitor.Indent.Unindent ();
			}
		}

		[Obsolete ("Use WriteText (LogViewProgressMonitor, string) instead")]
		public void WriteText (string text)
		{
			WriteText (null, text);
		}

		public void WriteText (LogViewProgressMonitor monitor, string text)
		{
			text = SanitizeConsoleText (text);

			//raw text has an extra optimisation here, as we can append it to existing updates
			lock (updates) {
				if (lastTextWrite != null) {
					if (lastTextWrite.Tag == null) {
						lastTextWrite.Write (text);
						return;
					}
				}
			}

			var qtw = new QueuedTextWrite (monitor, text, null);
			addQueuedUpdate (qtw);
		}

		[Obsolete ("Use WriteConsoleLogText (LogViewProgressMonitor, string) instead")]
		public void WriteConsoleLogText (string text)
		{
			WriteConsoleLogText (null, text);
		}

		public void WriteConsoleLogText (LogViewProgressMonitor monitor, string text)
		{
			lock (updates) {
				if (lastTextWrite != null && lastTextWrite.Tag == consoleLogTag) {
					lastTextWrite.Write (text);
					return;
				}
			}

			var w = new QueuedTextWrite (monitor, text, consoleLogTag);
			addQueuedUpdate (w);
		}

		[Obsolete ("Use WriteError (LogViewProgressMonitor, string) instead")]
		public void WriteError (string text)
		{
			WriteError (null, text);
		}

		public void WriteError (LogViewProgressMonitor monitor, string text)
		{
			var w = new QueuedTextWrite (monitor, text, errorTag);
			addQueuedUpdate (w);
		}

		[Obsolete ("Use WriteDebug (LogViewProgressMonitor, int, string, string) instead")]
		public void WriteDebug (int level, string category, string message)
		{
			WriteDebug (null, level, category, message);
		}

		public void WriteDebug (LogViewProgressMonitor monitor, int level, string category, string message)
		{
			//TODO: Give user ability to filter levels and categories
			if (string.IsNullOrEmpty (category))
				addQueuedUpdate (new QueuedTextWrite (monitor, message, debugTag));
			else
				addQueuedUpdate (new QueuedTextWrite (monitor, category + ": " + message, debugTag));
		}

		bool ShouldAutoScroll ()
		{
			if (scrollView == null || scrollView.Vadjustment == null)
				return false;

			// we need to account for the page size as well for some reason
			return scrollView.Vadjustment.Value + scrollView.Vadjustment.PageSize >= scrollView.Vadjustment.Upper;
		}

		/// <summary>
		/// This method removes ANSI/VT100 Color escape sequences from the string, along with other
		/// control/non-printable characters. We currently do not handle control characters performantly in the
		/// LogView, as the text rendering component tries to render a custom glyph denoting the control code. We
		/// also do not support colorization of the output, so the color information is redundant.
		///
		/// Specifically we remove color escape sequences (\e[ + color code + m) and the backspace (\b) control
		/// character.
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <returns>original text with control characters and escape sequences removed</returns>
		string SanitizeConsoleText(string text)
		{
			return consoleTextSanitizerRegex.Replace (text, string.Empty);
		}

		protected void UnsafeAddText (TextMark mark, string text, TextTag indentTag, TextTag extraTag)
		{
			//don't allow the pad to hold more than MAX_BUFFER_LENGTH chars
			int overrun = (buffer.CharCount + text.Length) - MAX_BUFFER_LENGTH;

			if (overrun > 0) {
				TextIter start = buffer.StartIter;
				TextIter end = buffer.GetIterAtOffset (overrun);
				buffer.Delete (ref start, ref end);
			}

			bool scrollToEnd = ShouldAutoScroll ();

			TextIter it;
			if (mark != null) {
				it = buffer.GetIterAtMark (mark);
			} else {
				it = buffer.EndIter;
			}

			if (extraTag != null)
				buffer.InsertWithTags (ref it, text, indentTag ?? normalTag, extraTag);
			else
				buffer.InsertWithTags (ref it, text, indentTag ?? normalTag);
			
			if (scrollToEnd) {
				it.LineOffset = 0;
				buffer.MoveMark (endMark, it);
				textEditorControl.ScrollToMark (endMark, 0, false, 0, 0);
			}
		}

		bool destroyed = false;
		public override void Destroy ()
		{
			lock (updates) {
				destroyed = true;
				updates.Clear ();
				lastTextWrite = null;
			}
			IdeApp.Preferences.CustomOutputPadFont.Changed -= HandleCustomFontChanged;

			if (fontProvider != null) {
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.RemoveProviderForDisplay (display, fontProvider);
				fontProvider = null;
			}

			base.Destroy ();
		}
		
		abstract class QueuedUpdate
		{
			public LogViewProgressMonitor Monitor { get; protected set; }
			public abstract void Execute (LogView pad);
		}
		
		class QueuedTextWrite : QueuedUpdate
		{
			readonly System.Text.StringBuilder Text;
			public TextTag Tag;

			public override void Execute (LogView pad)
			{
				pad.UnsafeAddText (Monitor?.Marker, Text.ToString (), Monitor?.Indent.IndentTag, Tag);
			}
			
			public QueuedTextWrite (LogViewProgressMonitor monitor, string text, TextTag tag)
			{
				Monitor = monitor;
				Text = new System.Text.StringBuilder (text);
				Tag = tag;
			}
			
			public void Write (string s)
			{
				Text.Append (s);
				if (Text.Length > MAX_BUFFER_LENGTH)
					Text.Remove (0, Text.Length - MAX_BUFFER_LENGTH);
			}
		}
		
		class QueuedBeginTask : QueuedUpdate
		{
			public string Name;
			public int TotalWork;

			public override void Execute (LogView pad)
			{
				pad.UnsafeBeginTask (Monitor, Name, TotalWork);
			}

			public QueuedBeginTask (LogViewProgressMonitor monitor, string name, int totalWork)
			{
				Monitor = monitor;
				TotalWork = totalWork;
				Name = name;
			}
		}
		
		class QueuedEndTask : QueuedUpdate
		{
			public override void Execute (LogView pad)
			{
				pad.UnsafeEndTask (Monitor);
			}

			public QueuedEndTask (LogViewProgressMonitor monitor)
			{
				Monitor = monitor;
			}
		}
	}

	public class LogViewProgressMonitor : OutputProgressMonitor
	{
		int padCookie;
		LogView outputPad;

		LogTextWriter internalLogger = new LogTextWriter ();
		NotSupportedTextReader inputReader = new NotSupportedTextReader ();
		OperationConsole console;
		
		internal LogView LogView {
			get { return outputPad; }
		}

		internal class IndentTracker {
			static int trackerID = 0;

			Stack<TextTag> tags = new Stack<TextTag> ();
			public TextTag IndentTag;

			int indent = 0;
			public TextTag Indent (string name)
			{
				TextTag tag;

				indent++;
				// create a unique tagname
				// Fixes VSTS 584931
				string tagname = $"{trackerID} - {indent} - {name ?? "(no name)"}";
				tag = new TextTag (tagname);
				trackerID++;

				tag.LeftMargin = 10 + 15 * (indent - 1);
				tags.Push (tag);

				IndentTag = tag;

				return tag;
			}

			public void Unindent ()
			{
				var tag = tags.Pop ();
				if (tag != null) {
					indent--;
					if (tags.Count > 0) {
						IndentTag = tags.Peek ();
					} else {
						IndentTag = null;
					}
				}
			}
		}

		internal IndentTracker Indent { get; set; }
		internal TextMark Marker { get; set; }

		//FIXME: this sync context is somewhat redundant, as the pad does its own GUI synchronization
		//that said, it's also used for the console and writers, so it's not simple to fix
		internal LogViewProgressMonitor (LogView pad, bool clearConsole): base (Runtime.MainSynchronizationContext)
		{
			outputPad = pad;

			Indent = new IndentTracker ();

			if (clearConsole) {
				unchecked {
					outputPad.Cookie++;
				}
				outputPad.Clear ();
			}

			padCookie = pad.Cookie;
			internalLogger.TextWritten += WriteConsoleLogText;
			console = new LogViewProgressConsole (this);
		}

		public override OperationConsole Console {
			get { return console; }
		}

		internal void Cancel ()
		{
			CancellationTokenSource.Cancel ();
		}

		protected override void OnWriteLog (string message)
		{
			if (CheckPadValid ()) {
				outputPad.WriteText (this, message);
			}
		}

		protected override void OnWriteErrorLog (string message)
		{
			if (CheckPadValid ()) {
				outputPad.WriteText (this, message);
			}
		}

		protected override void OnBeginTask (string name, int totalWork, int stepWork)
		{
			if (CheckPadValid ()) {
				outputPad.BeginTask (this, name, totalWork);
			}
		}

		protected override void OnEndTask (string name, int totalWork, int stepWork)
		{
			if (CheckPadValid ()) {
				outputPad.EndTask (this);
			}
		}

		void WriteConsoleLogText (string text)
		{
			if (CheckPadValid ()) {
				outputPad.WriteConsoleLogText (this, text);
			}
		}
		
		protected override void OnCompleted ()
		{
			if (!CheckPadValid ())
				return;

			outputPad.WriteText (this, "\n");

			foreach (string msg in SuccessMessages)
				outputPad.WriteText (this, msg + "\n");

			foreach (string msg in Warnings)
				outputPad.WriteText (this, msg + "\n");

			foreach (ProgressError msg in Errors)
				outputPad.WriteError (this, msg.DisplayMessage + "\n");

			base.OnCompleted ();

			outputPad = null;

			Completed?.Invoke (this, EventArgs.Empty);
		}

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		bool CheckPadValid ()
		{
			if (outputPad == null)
				throw GetDisposedException ();
			return padCookie == outputPad.Cookie;
		}

		static Exception GetDisposedException ()
		{
			return new InvalidOperationException ("Output progress monitor already disposed.");
		}

		protected override void OnDispose (bool disposing)
		{
			base.OnDispose (disposing);
			internalLogger.TextWritten -= WriteConsoleLogText;

			console.Dispose ();
			Disposed?.Invoke (this, EventArgs.Empty);
		}

		internal event EventHandler Disposed;
		internal event EventHandler Completed;

		class LogViewProgressConsole : OperationConsole
		{
			LogViewProgressMonitor monitor;

			public LogViewProgressConsole (LogViewProgressMonitor monitor)
			{
				this.monitor = monitor;
				CancellationSource = monitor.CancellationTokenSource;
			}

			public override TextReader In {
				get {
					return monitor.inputReader;
				}
			}
			public override TextWriter Out {
				get {
					return monitor.Log;
				}
			}
			public override TextWriter Error {
				get {
					return monitor.ErrorLog;
				}
			}
			public override TextWriter Log {
				get {
					return monitor.internalLogger;
				}
			}

			public override void Debug (int level, string category, string message)
			{
				if (monitor.CheckPadValid ()) {
					monitor.outputPad.WriteDebug (monitor, level, category, message);
				}
			}

			public override void Dispose ()
			{
				if (monitor != null) {
					var m = monitor; // Avoid recursive dispose, since the monitor also disposes this console
					monitor = null;
					m.Dispose ();
				}
				base.Dispose ();
			}
		}
	}
	
	class NotSupportedTextReader: TextReader
	{
		bool userWarned;
		
		void WarnUser ()
		{
			if (userWarned)
				return;
			userWarned = true;
			string title = GettextCatalog.GetString ("Console input not supported");
			string desc = GettextCatalog.GetString (
				"Console input is not supported when using the {0} output console. If your application needs to read " +
				"data from the standard input, please set the 'Run in External Console' option in the project options.",
				BrandingService.ApplicationName
			);
			MessageService.ShowWarning (title, desc);
		}
		
		public override int Peek ()
		{
			WarnUser ();
			return -1;
		}
		
		public override int ReadBlock (char[] buffer, int index, int count)
		{
			WarnUser ();
			return base.ReadBlock(buffer, index, count);
		}
		
		public override int Read (char[] buffer, int index, int count)
		{
			WarnUser ();
			return base.Read(buffer, index, count);
		}
		
		public override int Read ()
		{
			WarnUser ();
			return base.Read();
		}
		
		public override string ReadLine ()
		{
			WarnUser ();
			return base.ReadLine();
		}
		
		public override string ReadToEnd ()
		{
			WarnUser ();
			return base.ReadToEnd();
		}
	}
}

