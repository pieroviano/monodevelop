//
// BooleanEditorCell.cs
//
// Author:
//   Lluis Sanchez Gual
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
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
	[PropertyEditorType (typeof (System.Drawing.Color))]
	public class ColorEditorCell: PropertyEditorCell 
	{
		const int ColorBoxSize = 16;
		const int ColorBoxSpacing = 3;
		
		public override void GetSize (int availableWidth, out int width, out int height)
		{
			base.GetSize (availableWidth - ColorBoxSize - ColorBoxSpacing, out width, out height);
			width += ColorBoxSize + ColorBoxSpacing;
			if (height < ColorBoxSize) height = ColorBoxSize;
		}
		
		protected override string GetValueText ()
		{
			System.Drawing.Color color = (System.Drawing.Color) Value;
			//TODO: dropdown known color selector so this does something
			if (color.IsKnownColor)
				return color.Name;
			else if (color.IsEmpty)
				return "";
			else
				return String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
		}
		
		public override void Render (Cairo.Context ctx, Gdk.Rectangle bounds, Gtk.StateFlags state)
		{
			int yd = (bounds.Height - ColorBoxSize) / 2;
			ctx.Save ();
			ctx.SetSourceColor (GetColor ().ToCairoColor ());
			ctx.Rectangle (bounds.X, bounds.Y + yd, ColorBoxSize - 1, ColorBoxSize - 1);
			ctx.Fill ();
			ctx.SetSourceColor (new Cairo.Color (0, 0, 0));
			ctx.LineWidth = 1;
			ctx.Rectangle (bounds.X + 0.5, bounds.Y + yd + 0.5, ColorBoxSize - 1, ColorBoxSize - 1);
			ctx.Stroke ();
			ctx.Restore ();
			bounds.X += ColorBoxSize + ColorBoxSpacing;
			bounds.Width -= ColorBoxSize + ColorBoxSpacing;
			base.Render (ctx, bounds, state);
		}
		
		private Gdk.RGBA GetColor ()
		{
			System.Drawing.Color color = (System.Drawing.Color) Value;
			//TODO: Property.Converter.ConvertTo() fails: why?
			return new Gdk.RGBA { Red = color.R / 255f, Green = color.G / 255f, Blue = color.B / 255f, Alpha = 1 };
		}

		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cell_area, Gtk.StateFlags state)
		{
			return new ColorEditor ();
		}
	}
	
	public class ColorEditor : Gtk.ColorButton, IPropertyEditor
	{
		public void Initialize (EditSession session)
		{
			if (session.Property.PropertyType != typeof(System.Drawing.Color))
				throw new ApplicationException ("Color editor does not support editing values of type " + session.Property.PropertyType);
		}
		
		public object Value { 
			get {
				var c = Rgba;
				return System.Drawing.Color.FromArgb ((int) Math.Round (255 * c.Red), (int) Math.Round (255 * c.Green), (int) Math.Round (255 * c.Blue));
			}
			set {
				System.Drawing.Color color = (System.Drawing.Color) value;
				SetRgba (new Gdk.RGBA { Red = color.R / 255f, Green = color.G / 255f, Blue = color.B / 255f, Alpha = 1 });
			}
		}
		
		public ColorEditor ()
		{
			ColorSet += (sender, e) => ValueChanged?.Invoke (this, EventArgs.Empty);
		}

		public event EventHandler ValueChanged;
	}
}
