// 
// PangoUtils.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
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
using Gtk;

namespace MonoDevelop.Components
{
	public static class PangoUtil
	{
		/// <summary>
		/// Creates a Pango layout for <paramref name="widget"/> (GTK 4: <c>gtk_widget_create_pango_layout</c>).
		/// </summary>
		public static Pango.Layout CreateLayout (Widget widget)
		{
			return widget.CreatePangoLayout (null);
		}

		public static Pango.Layout CreateLayout (Widget widget, string text)
		{
			return widget.CreatePangoLayout (text);
		}

		public static Pango.Layout CreateLayout (PrintContext context)
		{
			return context?.CreatePangoLayout ();
		}
	}

	/// <summary>
	/// Builds a Pango attribute list (GTK 4: thin wrapper over <see cref="Pango.AttrList"/>).
	/// </summary>
	class FastPangoAttrList : IDisposable
	{
		Pango.AttrList list;

		public FastPangoAttrList ()
		{
			list = new Pango.AttrList ();
		}

		public void AddStyleAttribute (Pango.Style style, uint start, uint end)
		{
			Add (new Pango.AttrStyle (style), start, end);
		}

		public void AddWeightAttribute (Pango.Weight weight, uint start, uint end)
		{
			Add (new Pango.AttrWeight (weight), start, end);
		}

		public void AddForegroundAttribute (Gdk.RGBA color, uint start, uint end)
		{
			Add (new Pango.AttrForeground (ToPango (color.Red), ToPango (color.Green), ToPango (color.Blue)), start, end);
		}

		public void AddForegroundAttribute (Cairo.Color color, uint start, uint end)
		{
			Add (new Pango.AttrForeground (ToPango (color.R), ToPango (color.G), ToPango (color.B)), start, end);
		}

		public void AddBackgroundAttribute (Gdk.RGBA color, uint start, uint end)
		{
			Add (new Pango.AttrBackground (ToPango (color.Red), ToPango (color.Green), ToPango (color.Blue)), start, end);
		}

		public void AddBackgroundAttribute (Cairo.Color color, uint start, uint end)
		{
			Add (new Pango.AttrBackground (ToPango (color.R), ToPango (color.G), ToPango (color.B)), start, end);
		}

		public void AddUnderlineAttribute (Pango.Underline underline, uint start, uint end)
		{
			Add (new Pango.AttrUnderline (underline), start, end);
		}

		static ushort ToPango (double c)
		{
			return (ushort)(Math.Max (0, Math.Min (1, c)) * ushort.MaxValue);
		}

		void Add (Pango.Attribute attribute, uint start, uint end)
		{
			using (attribute) {
				attribute.StartIndex = start;
				attribute.EndIndex = end;
				list.Insert (attribute); // inserts a copy
			}
		}

		/// <summary>
		/// Like Splice, except it only offsets/clamps the inserted items, doesn't affect items already in the list.
		/// </summary>
		public void InsertOffsetList (Pango.AttrList atts, uint startOffset, uint endOffset)
		{
			foreach (var attr in atts.Attributes) {
				using (attr) {
					attr.EndIndex = Math.Min (endOffset, startOffset + attr.EndIndex);
					attr.StartIndex = startOffset + attr.StartIndex;
					list.Insert (attr);
				}
			}
		}

		public void Splice (Pango.AttrList attrs, int pos, int len)
		{
			list.Splice (attrs, pos, len);
		}

		public void AssignTo (Pango.Layout layout)
		{
			layout.Attributes = list;
		}

		public void Dispose ()
		{
			list?.Dispose ();
			list = null;
		}
	}

	internal class TextIndexer
	{
		static System.Collections.Generic.List<int> emptyList = new System.Collections.Generic.List<int> ();
		int [] indexToByteIndex;
		System.Collections.Generic.List<int> byteIndexToIndex;

		public TextIndexer (string text)
		{
			SetupTables (text);
		}

		public int IndexToByteIndex (int i)
		{
			if (i >= indexToByteIndex.Length)
				// if the index exceeds the byte index range, return the last byte index + 1
				// telling pango to span the attribute to the end of the string
				// this happens if the string contains multibyte characters
				return indexToByteIndex [i - 1] + 1;
			return indexToByteIndex [i];
		}

		public int ByteIndexToIndex (int i)
		{
			return byteIndexToIndex [i];
		}

		public void SetupTables (string text)
		{
			if (string.IsNullOrEmpty (text)) {
				this.indexToByteIndex = Array.Empty<int> ();
				this.byteIndexToIndex = emptyList;
				return;
			}

			int byteIndex = 0;
			int [] indexToByteIndex = new int [text.Length];
			var byteIndexToIndex = new System.Collections.Generic.List<int> (text.Length);
			unsafe {
				fixed (char *p = text) {
					for (int i = 0; i < text.Length; i++) {
						indexToByteIndex [i] = byteIndex;
						byteIndex += System.Text.Encoding.UTF8.GetByteCount (p + i, 1);
						while (byteIndexToIndex.Count < byteIndex)
							byteIndexToIndex.Add (i);
						}
					}
			}
			this.indexToByteIndex = indexToByteIndex;
			this.byteIndexToIndex = byteIndexToIndex;
		}
	}
}