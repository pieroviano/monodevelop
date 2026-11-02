// 
// DocumentationPanel.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
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

using System;
using Gtk;
using MonoDevelop.Core;

namespace MonoDevelop.AssemblyBrowser
{
	[System.ComponentModel.ToolboxItem(true)]
	partial class DocumentationPanel : Gtk.DrawingArea
	{
		Pango.Layout layout;
		int requestedHeight = -1;

		string markup;
		public string Markup {
			get { return markup; }
			set { markup = value; CalculateSize ();}
		}

		public DocumentationPanel ()
		{
			// the "view" style class paints the (insensitive aware) base background
			AddCssClass ("view");
			layout = new Pango.Layout (PangoContext);
			layout.Wrap = Pango.WrapMode.Word;
			DrawFunc = OnDraw;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && layout != null) {
				layout.Dispose ();
				layout = null;
			}
			base.Dispose (disposing);
		}

		void OnDraw (Gtk.DrawingArea area, Cairo.Context cr, int allocatedWidth, int allocatedHeight)
		{
			if (layout == null)
				return;
			int x = 0;
			int y = 0;
			if (string.IsNullOrEmpty (Markup)) {
				layout.SetMarkup (GettextCatalog.GetString ("No documentation available."));
				layout.Width = allocatedWidth * (int)Pango.Scale.PangoScale;
				layout.GetPixelSize (out int width, out int height);
				x = (allocatedWidth - width) / 2;
				y = (allocatedHeight - height) / 2;
			} else {
				layout.SetMarkup (Markup);
				layout.Width = allocatedWidth * (int)Pango.Scale.PangoScale;
			}
			var color = StyleColor;
			cr.SetSourceRGBA (color.Red, color.Green, color.Blue, color.Alpha);
			cr.MoveTo (x, y);
			Pango.CairoHelper.ShowLayout (cr, layout);
		}

		public void CalculateSize ()
		{
			if (layout == null)
				return;
			layout.SetMarkup (Markup ?? "");
			layout.Width = ContentWidth * (int)Pango.Scale.PangoScale;
			layout.GetPixelSize (out int width, out int height);
			if (height != requestedHeight) {
				requestedHeight = height;
				SetSizeRequest (width, height);
			}
			QueueDraw ();
		}
	}
}
