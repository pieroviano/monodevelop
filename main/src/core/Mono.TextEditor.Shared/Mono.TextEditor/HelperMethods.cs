// HelperMethods.cs
// 
// Cut & paste from PangoCairoHelper.
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Core.Text;

namespace Mono.TextEditor
{
	static class HelperMethods
	{
		public static T Kill<T>(this T gc) where T : IDisposable
		{
			if (gc != null) 
				gc.Dispose ();
			return default(T);
		}
		
		public static T Kill<T>(this T gc, Action<T> action) where T : IDisposable
		{
			if (gc != null) {
				action (gc);
				gc.Dispose ();
			}
			
			return default(T);
		}
		
		// ShowLayout, CreateLayout, LayoutPath and ContextSetResolution are provided by MonoDevelop.Components.HelperMethods.

		public static string GetColorString(Gdk.RGBA color)
		{
			return string.Format ("#{0:X02}{1:X02}{2:X02}", (int)(color.Red * 255), (int)(color.Green * 255), (int)(color.Blue * 255));
		}

		public static Pango.Context LayoutGetContext (this Pango.Layout layout)
		{
			return layout.Context;
		}
		
		public static void DrawLine (this Cairo.Context cr, Cairo.Color color, double x1, double y1, double x2, double y2)
		{
			cr.SetSourceColor (color);
			cr.MoveTo (x1, y1);
			cr.LineTo (x2, y2);
			cr.Stroke ();
		}
		
		public static void Line (this Cairo.Context cr, double x1, double y1, double x2, double y2)
		{
			cr.MoveTo (x1, y1);
			cr.LineTo (x2, y2);
		}
		
		public static void SharpLineX (this Cairo.Context cr, double x1, double y1, double x2, double y2)
		{
			cr.MoveTo (x1 + 0.5, y1);
			cr.LineTo (x2 + 0.5, y2);
		}
		
		public static void SharpLineY (this Cairo.Context cr, double x1, double y1, double x2, double y2)
		{
			cr.MoveTo (x1, y1 + 0.5);
			cr.LineTo (x2, y2 + 0.5);
		}
	}
}