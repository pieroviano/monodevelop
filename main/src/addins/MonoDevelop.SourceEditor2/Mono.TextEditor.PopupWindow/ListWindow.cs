// ListWindow.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2005 Novell, Inc (http://www.novell.com)
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

using Gtk;
using Gdk;
using Pango;
using System;
using System.Text;
using Mono.TextEditor;
using Mono.TextEditor.Theatrics;
using MonoDevelop.Components;

namespace Mono.TextEditor.PopupWindow
{
	[Flags]
	enum ListWindowKeyAction { 
		Process = 1, 
		Ignore = 2, 
		CloseWindow = 4, 
		Complete = 8 
	}
	
	/// <summary>
	/// A popup list of items. GTK 4 cannot position toplevel windows: the list is a <see cref="Gtk.Popover"/>
	/// attached to the widget it is shown for (see <see cref="ShowAt"/>).
	/// </summary>
	class ListWindow<T> : Gtk.Popover
	{
		SmartScrolledWindow scrollbar;
		ListWidget<T> list;
		IListDataProvider<T> provider;
		Widget footer;
		Gtk.Box vbox;

		StringBuilder word;
		int curPos;

		public ListWindow ()
		{
			Autohide = false;
			HasArrow = false;
			vbox = new Gtk.Box (Orientation.Vertical, 0);

			var box = new Gtk.Box (Orientation.Horizontal, 0);
			list = new ListWidget<T> (this);
			list.SelectionChanged += new EventHandler (OnSelectionChanged);

			scrollbar = new SmartScrolledWindow ();
			scrollbar.Child = list;
			box.Append (scrollbar);
			list.Pressed += (o, args) => {
				if (args.Button == 1 && args.NPress == 2)
					OnDoubleClicked (EventArgs.Empty);
			};
			vbox.Append (box);
			Child = vbox;
		}

		/// <summary>Pops the list up below (x, y) in the coordinates of <paramref name="parent"/>.</summary>
		public void ShowAt (Gtk.Widget parent, int x, int y)
		{
			if (Parent != parent) {
				if (Parent != null)
					Unparent ();
				Parent = parent;
			}
			PointingTo = new Gdk.Rectangle (x, y, 1, 1);
			Popup ();
			ResetSizes ();
		}

		/// <summary>Closes the list and detaches it from the widget it was shown for.</summary>
		public override void Destroy ()
		{
			if (Handle != IntPtr.Zero && Visible)
				Popdown ();
			base.Destroy ();
		}

		public event EventHandler DoubleClicked;

		protected virtual void OnDoubleClicked (EventArgs e)
		{
			EventHandler handler = this.DoubleClicked;
			if (handler != null)
				handler (this, e);
		}

		public void SetHeader (Widget w)
		{
			vbox.Prepend (w);
		}

		public void ShowFooter (Widget w)
		{
			HideFooter ();
			vbox.Append (w);
			footer = w;
		}
		
		public void HideFooter ()
		{
			if (footer != null) {
				vbox.Remove (footer);
				footer = null;
			}
		}
		
		protected void Reset (bool clearWord)
		{
			if (clearWord) {
				word = new StringBuilder ();
				curPos = 0;
			}
			
			list.Reset ();
			if (provider == null)
				return;
			
			if (IsRealized) {
				ResetSizes ();
			}
		}
		
		void ResetSizes ()
		{
			list.QueueResize ();
		}
		
		public IListDataProvider<T> DataProvider
		{
			get { return provider; }
			set { provider = value; }
		}
		
		public T CurrentItem {
			get { 
				return (list.Selection != -1 && !SelectionDisabled) ? provider[list.Selection] : default(T);
			}
		}
		
		public int Selection {
			get {
				return list.Selection;
			}
		}
		
		public bool SelectionDisabled {
			get {
				return list.SelectionDisabled; 
			}
		}
		
		public string PartialWord
		{
			get { return word.ToString (); }
			set
			{
				string newword = value;
				if (newword.Trim ().Length == 0)
					return;
				
				word = new StringBuilder (newword);
				curPos = newword.Length;
				UpdateWordSelection ();
			}
		}
		
		public bool IsUniqueMatch {
			get {
				int pos = list.Selection + 1;
				if (provider.Count > pos && provider.GetText (pos).StartsWith (PartialWord, StringComparison.OrdinalIgnoreCase)
				    || !(provider.GetText (list.Selection).StartsWith (PartialWord, StringComparison.OrdinalIgnoreCase)))
					return false;
				
				return true;	
			}
		}
		
		internal ListWidget<T> List
		{
			get { return list; }
		}
		
		public ListWindowKeyAction ProcessKey (Gdk.Key key, Gdk.ModifierType modifier)
		{
			switch (key)
			{
				case Gdk.Key.Up:
					if (list.SelectionDisabled)
						list.SelectionDisabled = false;
					else
						list.Selection --;
					return ListWindowKeyAction.Ignore;
					
				case Gdk.Key.Down:
					if (list.SelectionDisabled)
						list.SelectionDisabled = false;
					else
						list.Selection ++;
					return ListWindowKeyAction.Ignore;
					
				case Gdk.Key.Page_Up:
					list.Selection -= list.VisibleRows - 1;
					return ListWindowKeyAction.Ignore;
					
				case Gdk.Key.Page_Down:
					list.Selection += list.VisibleRows - 1;
					return ListWindowKeyAction.Ignore;
					
				case Gdk.Key.Left:
					//if (curPos == 0) return KeyAction.CloseWindow | KeyAction.Process;
					//curPos--;
					return ListWindowKeyAction.Process;
					
				case Gdk.Key.BackSpace:
					if (curPos == 0 || (modifier & Gdk.ModifierType.ControlMask) != 0)
						return ListWindowKeyAction.CloseWindow | ListWindowKeyAction.Process;
					curPos--;
					word.Remove (curPos, 1);
					UpdateWordSelection ();
					return ListWindowKeyAction.Process;
					
				case Gdk.Key.Right:
					//if (curPos == word.Length) return KeyAction.CloseWindow | KeyAction.Process;
					//curPos++;
					return ListWindowKeyAction.Process;
				
				case Gdk.Key.Caps_Lock:
				case Gdk.Key.Num_Lock:
				case Gdk.Key.Scroll_Lock:
					return ListWindowKeyAction.Ignore;
					
				case Gdk.Key.Return:
				case Gdk.Key.ISO_Enter:
				case Gdk.Key.Key_3270_Enter:
				case Gdk.Key.KP_Enter:
					return (list.SelectionDisabled? ListWindowKeyAction.Process : (ListWindowKeyAction.Complete | ListWindowKeyAction.Ignore))
						| ListWindowKeyAction.CloseWindow;
				
				case Gdk.Key.Escape:
					return ListWindowKeyAction.CloseWindow | ListWindowKeyAction.Ignore;
				
				case Gdk.Key.Home:
				case Gdk.Key.End:
					return ListWindowKeyAction.CloseWindow | ListWindowKeyAction.Process;
					
				case Gdk.Key.Control_L:
				case Gdk.Key.Control_R:
				case Gdk.Key.Alt_L:
				case Gdk.Key.Alt_R:
				case Gdk.Key.Shift_L:
				case Gdk.Key.Shift_R:
				case Gdk.Key.ISO_Level3_Shift:	// AltGr
					return ListWindowKeyAction.Process;
			}
			
			
			return ListWindowKeyAction.CloseWindow | ListWindowKeyAction.Process;
		}
		
		void UpdateWordSelection ()
		{
			SelectEntry (word.ToString ());
		}
		
		//note: finds the full match, or the best partial match
		//returns -1 if there is no match at all
		int findMatchedEntry (string s, out bool hasMismatches)
		{
			int max = (provider == null ? 0 : provider.Count);
			string sLower = s.ToLower ();
			
			int bestMatch = -1;
			int bestMatchLength = 0;
			for (int n=0; n<max; n++) 
			{
				string txt = provider.GetText (n);
				if (txt.StartsWith (s)) {
					hasMismatches = false;
					return n;
				} else {
					//try to match as many characters at the beginning of the words as possible
					int matchLength = 0;
					int minLength = System.Math.Min (s.Length, txt.Length);
					while (matchLength < minLength && char.ToLower (txt[matchLength]) == sLower [matchLength]) {
						matchLength++;
					}
					if (matchLength > bestMatchLength) {
						bestMatchLength = matchLength;
						bestMatch = n;
					}
				}
			}
			hasMismatches = (bestMatch > -1) && (bestMatchLength != s.Length);
			return bestMatch;
		}
		
		void SelectEntry (int n)
		{
			if (n < 0) {
				list.SelectionDisabled = true;
			} else {
				list.Selection = n;
			}
		}
		
		public void SelectEntry (string s)
		{
			//when the list is empty, disable the selection or users get annoyed by it accepting
			//the top entry automatically
			if (string.IsNullOrEmpty (s)) {
				list.Selection = 0;
				list.SelectionDisabled = true;
				return;
			}
				
			bool hasMismatches;
			int n = findMatchedEntry (s, out hasMismatches);
			SelectEntry (n);
			if (hasMismatches)
				list.SelectionDisabled = true;
		}
		/*
		void OnScrollChanged (object o, EventArgs args)
		{
			list.Page = (int) scrollbar.Value;
		}

		void OnScrolled (object o, ScrollEventArgs args)
		{
			if (!scrollbar.Visible)
				return;
			
			var adj = scrollbar.Adjustment;
			var alloc = Allocation;
			
			//This widget is a special case because it's always aligned to items as it scrolls.
			//Although this means we can't use the pixel deltas for true smooth scrolling, we 
			//can still make use of the effective scrolling velocity by basing the calculation 
			//on pixels and rounding to the nearest item.
			
			double dx, dy;
			args.Event.GetPageScrollPixelDeltas (0, alloc.Height, out dx, out dy);
			if (dy == 0)
				return;
			
			var itemDelta = dy / (alloc.Height / adj.PageSize);
			double discreteItemDelta = System.Math.Round (itemDelta);
			if (discreteItemDelta == 0.0 && dy != 0.0)
				discreteItemDelta = dy > 0? 1.0 : -1.0;
			
			adj.AddValueClamped (discreteItemDelta);
			args.RetVal = true;
		}
		*/
		void OnSelectionChanged (object o, EventArgs args)
		{
			OnSelectionChanged ();
		}
		
		protected virtual void OnSelectionChanged ()
		{
		}
		
		public int TextOffset {
			get { return list.TextOffset; }
		}
	}

}