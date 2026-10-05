// 
// ConsoleView.cs
//  
// Author:
//       Peter Johanson <latexer@gentoo.org>
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2005, Peter Johanson (latexer@gentoo.org)
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
using System.Collections.Generic;

using Gtk;

using MonoDevelop.Core;

namespace MonoDevelop.Components
{
	public class ConsoleView: ScrolledWindow
	{
		string scriptLines = "";
		
		Stack<string> commandHistoryPast = new Stack<string> ();
		Stack<string> commandHistoryFuture = new Stack<string> ();

		bool inBlock = false;
		string blockText = "";

		TextMark inputBeginMark;
		TextView textView;
	
		public ConsoleView ()
		{
			PromptString = "> ";
			PromptMultiLineString = ">> ";
			
			textView = new TextView ();
			Child = textView;
			
			textView.WrapMode = Gtk.WrapMode.Word;
			// Capture phase: intercept keys before the TextView handles them (was a ConnectBefore KeyPressEvent).
			textView.AddKeyPressHandler (TextViewKeyPressed, null, PropagationPhase.Capture);
			SetupContextMenu ();

			inputBeginMark = Buffer.CreateMark (null, Buffer.EndIter, true);

			// The 'Freezer' tag is used to keep everything except
			// the input line from being editable
			TextTag tag = new TextTag ("Freezer");
			tag.Editable = false;
			Buffer.TagTable.Add (tag);
			Prompt (false);
		}

		// GTK 4: PopulatePopup is gone; extra context-menu items come from TextView.ExtraMenu (a menu model + actions).
		void SetupContextMenu ()
		{
			var clearAction = new GLib.SimpleAction ("clear", null);
			clearAction.Activated += (o, a) => Clear ();
			var group = new GLib.SimpleActionGroup ();
			group.AddAction (clearAction);
			textView.InsertActionGroup ("console", group);

			var section = new GLib.Menu ();
			section.Append (GettextCatalog.GetString ("Clear"), "console.clear");
			var menu = new GLib.Menu ();
			menu.AppendSection (null, section);
			textView.ExtraMenu = menu;
		}

		static int fontClassCounter;
		string fontClass;
		CssProvider fontProvider;

		// GTK 4: ModifyFont is gone; the font is applied through a CSS class unique to this view.
		public void SetFont (Pango.FontDescription font)
		{
			if (fontClass == null) {
				fontClass = "md-console-view-" + (++fontClassCounter);
				textView.AddCssClass (fontClass);
				fontProvider = new CssProvider ();
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

		protected override void Dispose (bool disposing)
		{
			if (disposing && fontProvider != null) {
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.RemoveProviderForDisplay (display, fontProvider);
				fontProvider = null;
			}
			base.Dispose (disposing);
		}

		protected TextView TextView {
			get { return textView; }
		}
		
		public string PromptString { get; set; }
		
		public bool AutoIndent { get; set; }

		public string PromptMultiLineString { get; set; }
		
		bool TextViewKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			return ProcessKeyPressEvent ((Gdk.Key)keyval, state);
		}

		bool ProcessReturn ()
		{
			if (inBlock) {
				if (InputLine == "") {
					ProcessInput (blockText);
					blockText = "";
					inBlock = false;
				} else {
					blockText += "\n" + InputLine;
					string whiteSpace = null;
					if (AutoIndent) {
						System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex (@"^(\s+).*");
						whiteSpace = r.Replace (InputLine, "$1");
						if (InputLine.EndsWith (BlockStart, StringComparison.InvariantCulture))
							whiteSpace += "\t";
					}
					Prompt (true, true);
					if (AutoIndent)
						InputLine += whiteSpace;
				}
			} else {
				// Special case for start of new code block
				if (!string.IsNullOrEmpty (BlockStart) && InputLine.Trim().EndsWith (BlockStart, StringComparison.InvariantCulture)) {
					inBlock = true;
					blockText = InputLine;
					Prompt (true, true);
					if (AutoIndent)
						InputLine += "\t";
					return true;
				}

				// Bookkeeping
				if (InputLine != "") {
					// Everything but the last item (which was input),
					//in the future stack needs to get put back into the
					// past stack
					while (commandHistoryFuture.Count > 1)
						commandHistoryPast.Push (commandHistoryFuture.Pop());
					// Clear the pesky junk input line
					commandHistoryFuture.Clear();

					// Record our input line
					commandHistoryPast.Push(InputLine);
					if (scriptLines == "")
						scriptLines += InputLine;
					else
						scriptLines += "\n" + InputLine;

					ProcessInput (InputLine);
				}
			}

			return true;
		}

		bool ProcessCommandHistoryUp ()
		{
			if (!inBlock && commandHistoryPast.Count > 0) {
				if (commandHistoryFuture.Count == 0) {
					commandHistoryFuture.Push (InputLine);
				} else {
					if (commandHistoryPast.Count == 1)
						return true;

					commandHistoryFuture.Push (commandHistoryPast.Pop ());
				}

				InputLine = commandHistoryPast.Peek ();
			}

			return true;
		}

		bool ProcessCommandHistoryDown ()
		{
			if (!inBlock && commandHistoryFuture.Count > 0) {
				if (commandHistoryFuture.Count == 1) {
					InputLine = commandHistoryFuture.Pop ();
				} else {
					commandHistoryPast.Push (commandHistoryFuture.Pop ());
					InputLine = commandHistoryPast.Peek ();
				}
			}

			return true;
		}
		
		protected virtual bool ProcessKeyPressEvent (Gdk.Key key, Gdk.ModifierType state)
		{
			// Short circuit to avoid getting moved back to the input line
			// when paging up and down in the shell output
			if (key == Gdk.Key.Page_Up || key == Gdk.Key.Page_Down)
				return false;
			
			// Needed so people can copy and paste, but always end up
			// typing in the prompt.
			if (Cursor.Compare (InputLineBegin) < 0) {
				Buffer.MoveMark (Buffer.SelectionBound, InputLineEnd);
				Buffer.MoveMark (Buffer.InsertMark, InputLineEnd);
			}

			if (!TextView.Editable) {
				return false;
			}

//			if (ev.State == Gdk.ModifierType.ControlMask && ev.Key == Gdk.Key.space)
//				TriggerCodeCompletion ();

			switch (key) {
			case Gdk.Key.KP_Enter:
			case Gdk.Key.Return:
				return ProcessReturn ();
			case Gdk.Key.KP_Up:
			case Gdk.Key.Up:
				return ProcessCommandHistoryUp ();
			case Gdk.Key.KP_Down:
			case Gdk.Key.Down:
				return ProcessCommandHistoryDown ();
			case Gdk.Key.KP_Left:
			case Gdk.Key.Left:
				// On Mac, when using a small keyboard, Home is Command+Left
				if (Platform.IsMac && state.HasFlag (Gdk.ModifierType.MetaMask)) {
					Buffer.MoveMark (Buffer.InsertMark, InputLineBegin);

					// Move the selection mark too, if shift isn't held
					if (!state.HasFlag (Gdk.ModifierType.ShiftMask))
						Buffer.MoveMark (Buffer.SelectionBound, InputLineBegin);

					return true;
				}

				// Keep our cursor inside the prompt area
				if (Cursor.Compare (InputLineBegin) <= 0)
					return true;

				break;
			case Gdk.Key.KP_Home:
			case Gdk.Key.Home:
				Buffer.MoveMark (Buffer.InsertMark, InputLineBegin);

				// Move the selection mark too, if shift isn't held
				if (!state.HasFlag (Gdk.ModifierType.ShiftMask))
					Buffer.MoveMark (Buffer.SelectionBound, InputLineBegin);

				return true;
			case Gdk.Key.a:
				if (state.HasFlag (Gdk.ModifierType.ControlMask)) {
					Buffer.MoveMark (Buffer.InsertMark, InputLineBegin);

					// Move the selection mark too, if shift isn't held
					if (!state.HasFlag (Gdk.ModifierType.ShiftMask))
						Buffer.MoveMark (Buffer.SelectionBound, InputLineBegin);

					return true;
				}
				break;
			case Gdk.Key.period:
				return false;
			default:
				return false;
			}
			
			return false;
		}

		public TextIter InputLineBegin {
			get { return Buffer.GetIterAtMark (inputBeginMark); }
		}
	
		public TextIter InputLineEnd {
			get { return Buffer.EndIter; }
		}
		
		public new TextIter Cursor {
			get { return Buffer.GetIterAtMark (Buffer.InsertMark); }
		}
		
		public Gtk.TextBuffer Buffer {
			get { return textView.Buffer; }
		}

		protected Gdk.Rectangle GetIterLocation (TextIter iter)
		{
			return textView.GetIterLocation (iter);
		}
		
		// The current input line
		public string InputLine {
			get {
				return Buffer.GetText (InputLineBegin, InputLineEnd, false);
			}
			set {
				TextIter start = InputLineBegin;
				TextIter end = InputLineEnd;
				Buffer.Delete (ref start, ref end);
				start = InputLineBegin;
				Buffer.Insert (ref start, value);
			}
		}
		
		protected virtual void ProcessInput (string line)
		{
			WriteOutput ("\n");
			if (ConsoleInput != null)
				ConsoleInput (this, new ConsoleInputEventArgs (line));
		}
		
		public void WriteOutput (string line)
		{
			TextIter start = Buffer.EndIter;
			Buffer.Insert (ref start , line);
			Buffer.PlaceCursor (Buffer.EndIter);
			textView.ScrollMarkOnscreen (Buffer.InsertMark);
		}
	
		public void Prompt (bool newLine)
		{
			Prompt (newLine, false);
		}
	
		public void Prompt (bool newLine, bool multiline)
		{
			TextIter end = Buffer.EndIter;
			if (newLine)
				Buffer.Insert (ref end, "\n");
			if (multiline)
				Buffer.Insert (ref end, PromptMultiLineString);
			else
				Buffer.Insert (ref end, PromptString);
	
			Buffer.PlaceCursor (Buffer.EndIter);
			textView.ScrollMarkOnscreen (Buffer.InsertMark);
	
			UpdateInputLineBegin ();
	
			// Freeze all the text except our input line
			Buffer.ApplyTag(Buffer.TagTable.Lookup("Freezer"), Buffer.StartIter, InputLineBegin);
		}

		protected virtual void UpdateInputLineBegin ()
		{
			// Record the end of where we processed, used to calculate start
			// of next input line
			Buffer.MoveMark (inputBeginMark, Buffer.EndIter);
		}
		
		public void Clear ()
		{
			Buffer.Text = "";
			scriptLines = "";
			Prompt (false);
		}
		
		public void ClearHistory ()
		{
			commandHistoryFuture.Clear ();
			commandHistoryPast.Clear ();
		}
		
		public string BlockStart { get; set; }
		
		public string BlockEnd { get; set; }
		
		public event EventHandler<ConsoleInputEventArgs> ConsoleInput;
	}
	
	public class ConsoleInputEventArgs: EventArgs
	{
		public ConsoleInputEventArgs (string text)
		{
			Text = text;
		}
		
		public string Text { get; internal set; }
	}
}
