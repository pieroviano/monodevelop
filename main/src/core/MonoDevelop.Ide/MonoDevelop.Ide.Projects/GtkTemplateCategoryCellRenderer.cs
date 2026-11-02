//
// GtkTemplateCategoryCellRenderer.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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

using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Ide.Templates;

namespace MonoDevelop.Ide.Projects
{
	class GtkTemplateCategoryCellRenderer : CellRendererText
	{
		public TemplateCategory Category { get; set; }
		public string CategoryName { get; set; }
		public Xwt.Drawing.Image CategoryIcon { get; set; }
		public int CategoryIconWidth { get; set; }

		const int topLevelTemplateHeadingTotalYPadding = 23;
		const int topLevelTemplateHeadingYOffset = 4;
		const int topLevelIconTextXPadding = 6;
		const int iconTextXPadding = 1;
		const int iconYOffset = -1;

		protected override void OnGetPreferredHeight (Widget widget, out int minimum_size, out int natural_size)
		{
			base.OnGetPreferredHeight (widget, out minimum_size, out natural_size);
			if (CategoryIcon != null)
				minimum_size = natural_size = GetIconCellHeight ();
		}

		protected override void OnGetPreferredHeightForWidth (Widget widget, int width, out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeightForWidth (widget, width, out minimum_height, out natural_height);
			if (CategoryIcon != null)
				minimum_height = natural_height = GetIconCellHeight ();
		}

		int GetIconCellHeight ()
		{
			return (int)CategoryIcon.Height + ((int)Ypad * 2) + topLevelTemplateHeadingTotalYPadding;
		}

		protected override void OnSnapshot (Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			var isSelected = CellRendererDrawing.IsSelected (flags);
			int iconTextPadding = iconTextXPadding;
			int textYOffset = 0;
			Gdk.Rectangle iconRect = GetIconRect (cell_area);

			// the separator line is drawn one pixel below the cell
			snapshot.DrawCairo (new Gdk.Rectangle (cell_area.X, cell_area.Y, cell_area.Width, cell_area.Height + 2), ctx => {
				if (CategoryIcon != null) {
					iconRect = DrawIcon (ctx, widget, cell_area, flags);
					iconTextPadding = topLevelIconTextXPadding;
					textYOffset = (Category == null ? 0 : topLevelTemplateHeadingYOffset);
				}

				DrawTemplateCategoryText (ctx, widget, cell_area, iconRect, iconTextPadding, textYOffset, flags);
				if (Category == null && !isSelected) {
					ctx.MoveTo (cell_area.X + (int)Xpad, cell_area.Y + cell_area.Height + 1);
					ctx.SetSourceColor (Gui.Styles.ThinSplitterColor.ToCairoColor ());
					ctx.LineWidth = 1;
					ctx.LineTo (cell_area.X + cell_area.Width - (int)Xpad, cell_area.Y + cell_area.Height + 1);
					ctx.Stroke ();
				}
			});
		}

		Gdk.Rectangle GetIconRect (Gdk.Rectangle cell_area)
		{
			return new Gdk.Rectangle (cell_area.X + (int)Xpad, cell_area.Y + (int)Ypad + iconYOffset, CategoryIconWidth, CategoryIconWidth);
		}

		Gdk.Rectangle DrawIcon (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			int iconY = cell_area.Y + ((cell_area.Height - (int)CategoryIcon.Height) / 2) + (Category == null ? 0 : topLevelTemplateHeadingYOffset);
			var iconRect = new Gdk.Rectangle (cell_area.X + (int)Xpad, iconY, (int)CategoryIcon.Width, (int)CategoryIcon.Height);

			var img = CategoryIcon;
			if ((flags & Gtk.CellRendererState.Selected) != 0)
				img = img.WithStyles ("sel");
			ctx.DrawImage (widget, img, iconRect.X, iconRect.Y);
			return iconRect;
		}

		void DrawTemplateCategoryText (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, Gdk.Rectangle iconRect, int iconTextPadding, int textYOffset, CellRendererState flags)
		{
			using (var layout = new Pango.Layout (widget.PangoContext)) {

				layout.Ellipsize = Pango.EllipsizeMode.End;
				int textPixelWidth = widget.Width - ((int)Xpad * 2) - iconRect.Width - iconTextPadding;
				layout.Width = (int)(textPixelWidth * Pango.Scale.PangoScale);

				layout.SetMarkup (CategoryName);

				int w, h;
				layout.GetPixelSize (out w, out h);
				int textY = cell_area.Y + (cell_area.Height - h) / 2 + textYOffset;

				CellRendererDrawing.DrawLayout (ctx, CellRendererDrawing.GetTextColor (widget, flags), iconRect.X + iconRect.Width + iconTextPadding, textY, layout);
			}
		}
	}
}
