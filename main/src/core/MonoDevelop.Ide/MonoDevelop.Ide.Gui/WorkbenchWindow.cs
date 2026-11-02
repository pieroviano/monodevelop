// 
// WorkbenchWindow.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
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
using System.Collections.Generic;
using MonoDevelop.Components;

namespace MonoDevelop.Ide.Gui
{
	/// <summary>
	/// Main window. Its content is hosted in an overlay so that widgets can be placed at arbitrary
	/// positions over it (see <see cref="AddTopLevelWidget"/>).
	/// </summary>
	class WorkbenchWindow: Gtk.Window
	{
		List<TopLevelChild> topLevels = new List<TopLevelChild> ();
		Gtk.Overlay overlay = new Gtk.Overlay ();
		
		public WorkbenchWindow ()
		{
			base.Child = overlay;
		}

		/// <summary>The main content of the window (hosted in the overlay).</summary>
		protected Gtk.Widget Content {
			get { return overlay.Child; }
			set { overlay.Child = value; }
		}

		class TopLevelChild
		{
			public int X;
			public int Y;
			public Gtk.Widget Child;
		}
		
		public void AddTopLevelWidget (Gtk.Widget w, int x, int y)
		{
			TopLevelChild info = new TopLevelChild ();
			info.X = x;
			info.Y = y;
			info.Child = w;
			topLevels.Add (info);
			w.Halign = Gtk.Align.Start;
			w.Valign = Gtk.Align.Start;
			PlaceTopLevel (info);
			overlay.AddOverlay (w);
		}
		
		public void RemoveTopLevelWidget (Gtk.Widget w)
		{
			foreach (TopLevelChild info in topLevels) {
				if (info.Child == w) {
					overlay.RemoveOverlay (w);
					topLevels.Remove (info);
					break;
				}
			}
		}
		
		public void MoveTopLevelWidget (Gtk.Widget w, int x, int y)
		{
			foreach (TopLevelChild info in topLevels) {
				if (info.Child == w) {
					info.X = x;
					info.Y = y;
					PlaceTopLevel (info);
					break;
				}
			}
		}

		void PlaceTopLevel (TopLevelChild info)
		{
			info.Child.MarginStart = info.X;
			info.Child.MarginTop = info.Y;
		}
		
		public Gdk.Rectangle GetTopLevelPosition (Gtk.Widget w)
		{
			foreach (TopLevelChild info in topLevels) {
				if (info.Child == w) {
					w.Measure (Gtk.Orientation.Horizontal, -1, out _, out int width, out _, out _);
					w.Measure (Gtk.Orientation.Vertical, width, out _, out int height, out _, out _);
					return new Gdk.Rectangle (info.X, info.Y, width, height);
				}
			}
			return new Gdk.Rectangle (0,0,0,0);
		}
		
		public Gdk.Rectangle GetCoordinates (Gtk.Widget w)
		{
			if (!w.TranslateCoordinates (this, 0, 0, out double px, out double py))
				return new Gdk.Rectangle (0,0,0,0);

			return new Gdk.Rectangle ((int)px, (int)py, w.Width, w.Height);
		}
	}
}
