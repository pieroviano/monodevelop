//
// TreeViewCellContainer.cs
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
	/// <summary>
	/// Cell editor hosting an arbitrary widget inside a TreeView cell (GTK 4: a widget implementing GtkCellEditable,
	/// parented by the TreeView over the cell area). If the child is itself a cell editable (e.g. a ComboBox), its
	/// editing-done / remove-widget signals are forwarded.
	/// </summary>
	public class TreeViewCellContainer: BinWidget, ICellEditableImplementor
	{
		readonly Gtk.Widget child;
		bool editingCanceled;
		bool finished;

		public event EventHandler EditingDone;

		public TreeViewCellContainer (Gtk.Widget child)
		{
			this.child = child;
			Child = child;
			Focusable = true;

			// Avoid forwarding the button press to the tree, since it would hide the cell editor.
			this.AddClickHandler (c => c.Claim ());

			this.AddKeyPressHandler ((keyval, keycode, state) => {
				if ((Gdk.Key)keyval != Gdk.Key.Escape)
					return false;
				editingCanceled = true;
				Finish ();
				return true;
			});

			if (child is ICellEditable ce) {
				ce.EditingDone += OnChildEditingDone;
				ce.WidgetRemoved += OnChildWidgetRemoved;
			}
		}

		[GLib.Property ("editing-canceled")]
		public bool EditingCanceled {
			get { return editingCanceled; }
			set { editingCanceled = value; }
		}

		void ICellEditableImplementor.StartEditing (Gdk.Event evnt)
		{
			finished = false;
			editingCanceled = false;
			if (child is ICellEditable ce)
				ce.StartEditing (evnt);
			child.GrabFocus ();
		}

		void OnChildEditingDone (object sender, EventArgs e)
		{
			Finish ();
		}

		void OnChildWidgetRemoved (object sender, EventArgs e)
		{
			Finish ();
		}

		/// <summary>Emits editing-done + remove-widget so the TreeView ends editing and removes this widget.</summary>
		void Finish ()
		{
			if (finished)
				return;
			finished = true;
			var editable = CellEditableAdapter.GetObject (this);
			editable.FinishEditing ();
			editable.RemoveWidget ();
		}

		protected override void OnUnroot ()
		{
			base.OnUnroot ();
			EditingDone?.Invoke (this, EventArgs.Empty);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && child is ICellEditable ce && child.Handle != IntPtr.Zero) {
				ce.EditingDone -= OnChildEditingDone;
				ce.WidgetRemoved -= OnChildWidgetRemoved;
			}
			base.Dispose (disposing);
		}
	}
}
