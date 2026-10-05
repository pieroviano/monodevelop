//
// CellRendererComboBox.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using Gtk;

namespace MonoDevelop.Components
{
	public class CellRendererComboBox: CellRendererText
	{
		string[] values;
		string path;
		int rowHeight;
		
		public CellRendererComboBox ()
		{
			Mode |= Gtk.CellRendererMode.Editable;
			var dummyEntry = new Gtk.ComboBoxText ();
			dummyEntry.Measure (Gtk.Orientation.Vertical, -1, out _, out rowHeight, out _, out _);
			Ypad = 0;
		}

		public string[] Values {
			get { return values; }
			set { values = value; }
		}
		
		protected override void OnGetPreferredHeight (Widget widget, out int minimum_size, out int natural_size)
		{
			base.OnGetPreferredHeight (widget, out minimum_size, out natural_size);
			minimum_size = Math.Max (minimum_size, rowHeight);
			natural_size = Math.Max (natural_size, rowHeight);
		}

		protected override void OnGetPreferredHeightForWidth (Widget widget, int width, out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeightForWidth (widget, width, out minimum_height, out natural_height);
			minimum_height = Math.Max (minimum_height, rowHeight);
			natural_height = Math.Max (natural_height, rowHeight);
		}
		
		protected override ICellEditable OnStartEditing (Gdk.Event evnt, Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			this.path = path;

			var combo = new Gtk.ComboBoxText ();
			if (values != null) {
				foreach (string s in values)
					combo.AppendText (s);
				combo.Active = Array.IndexOf (values, Text);
			}
			combo.Changed += SelectionChanged;
			return CellEditableAdapter.GetObject (new TreeViewCellContainer (combo));
		}
		
		void SelectionChanged (object s, EventArgs a)
		{
			var combo = (Gtk.ComboBoxText) s;
			if (Changed != null)
				Changed (this, new ComboSelectionChangedArgs (path, combo.Active, combo.ActiveText));
		}
		
		// Fired when the selection changes
		public event ComboSelectionChangedHandler Changed;
	}
	
	public delegate void ComboSelectionChangedHandler (object sender, ComboSelectionChangedArgs args);
	
	public class ComboSelectionChangedArgs: EventArgs 
	{
		string path;
		int active;
		string activeText;
		
		public ComboSelectionChangedArgs (string path, int active, string activeText)
		{
			this.path = path;
			this.active = active;
			this.activeText = activeText;
		}
		
		public string Path {
			get { return path; }
		}
		
		public int Active {
			get { return active; }
		}
		
		public string ActiveText {
			get { return activeText; }
		}
	}
}
