//
// BooleanEditorCell.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
using System.ComponentModel;

namespace MonoDevelop.Components.PropertyGrid.PropertyEditors
{
	[PropertyEditorType (typeof (bool))]
	public class BooleanEditorCell : PropertyEditorCell 
	{
		const int indicatorSize = 16;
		const int indicatorSpacing = 2;

		public override void GetSize (int availableWidth, out int width, out int height)
		{
			width = indicatorSize;
			height = indicatorSize;
		}
		
		internal static void DrawCheck (Cairo.Context ctx, Gdk.RGBA color, double x, double y, int s, bool active)
		{
			ctx.Save ();
			ctx.SetSourceColor (color.ToCairoColor ());
			ctx.LineWidth = 1;
			ctx.Rectangle (x, y, s - 1, s - 1);
			ctx.Stroke ();
			if (active) {
				ctx.LineWidth = 2;
				ctx.MoveTo (x + s * 0.2, y + s * 0.5);
				ctx.LineTo (x + s * 0.42, y + s * 0.72);
				ctx.LineTo (x + s * 0.8, y + s * 0.25);
				ctx.Stroke ();
			}
			ctx.Restore ();
		}

		public override void Render (Cairo.Context ctx, Gdk.Rectangle bounds, Gtk.StateFlags state)
		{
			int s = indicatorSize - 1;
			if (s > bounds.Height)
				s = bounds.Height;
			if (s > bounds.Width)
				s = bounds.Width;
			DrawCheck (ctx, GetTextColor (state), bounds.X + indicatorSpacing - 1 + 0.5, bounds.Y + (bounds.Height - s) / 2 + 0.5, s, (bool) Value);
		}
		
		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cell_area, Gtk.StateFlags state)
		{
			return new BooleanEditor ();
		}
	}
	
	public class BooleanEditor : Gtk.CheckButton, IPropertyEditor 
	{
		public BooleanEditor ()
		{
			Toggled += (sender, e) => ValueChanged?.Invoke (this, EventArgs.Empty);
		}

		public void Initialize (EditSession session)
		{
			if (session.Property.PropertyType != typeof(bool))
				throw new ApplicationException ("Boolean editor does not support editing values of type " + session.Property.PropertyType);
			Sensitive = !session.Property.IsReadOnly;
		}
		
		public object Value { 
			get { return Active; } 
			set { Active = (bool) value; }
		}

		public event EventHandler ValueChanged;
	}
}
