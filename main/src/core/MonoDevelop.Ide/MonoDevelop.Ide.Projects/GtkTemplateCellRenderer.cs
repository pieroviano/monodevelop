//
// TemplateCellRendererText.cs
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
//

using System;
using System.Linq;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Templates;

namespace MonoDevelop.Ide.Projects
{
	class GtkTemplateCellRenderer : CellRendererText
	{
		const int iconTextPadding = 9;
		int groupTemplateHeadingTotalYPadding = 24;
		int recentTemplateHeadingTotalYPadding = 30;
		private SolutionTemplate template;
		private string templateCategory;
		private string selectedLanguage;
		const int groupTemplateHeadingYOffset = 4;
		const int categoryTextPaddingX = 4;

		public SolutionTemplate Template {
			get { return template; }
			set {
				template = value;
				SetAccessibilityText ();
			}
		}
		public string SelectedLanguage {
			get { return selectedLanguage; }
			set {
				selectedLanguage = value;
				SetAccessibilityText ();
			}
		}
		public Xwt.Drawing.Image TemplateIcon { get; set; }
		public string TemplateCategory {
			get { return templateCategory; }
			set {
				templateCategory = value;
				SetAccessibilityText ();
			}
		}
		public bool RenderRecentTemplate { get; set; }

		void SetAccessibilityText ()
		{
			Text = template?.Name ?? string.Empty;
			if (!string.IsNullOrEmpty (templateCategory)) {
				if (!string.IsNullOrEmpty (Text))
					Text += ", ";
				Text += templateCategory.Replace ("→", "–"); // we don't want narrators to read "right arrow"
			}
		}

		public GtkTemplateCellRenderer ()
		{
			if (IsYosemiteOrHigher ()) {
				groupTemplateHeadingTotalYPadding -= 1;
				recentTemplateHeadingTotalYPadding -= 1;
			}
		}

		static bool IsYosemiteOrHigher ()
		{
			return Platform.IsMac && (Platform.OSVersion >= MacSystemInformation.Yosemite);
		}

		protected override void OnGetPreferredHeight (Widget widget, out int minimum_size, out int natural_size)
		{
			base.OnGetPreferredHeight (widget, out minimum_size, out natural_size);
			minimum_size = natural_size = AdjustHeight (natural_size);
		}

		protected override void OnGetPreferredHeightForWidth (Widget widget, int width, out int minimum_height, out int natural_height)
		{
			base.OnGetPreferredHeightForWidth (widget, width, out minimum_height, out natural_height);
			minimum_height = natural_height = AdjustHeight (natural_height);
		}

		int AdjustHeight (int textHeight)
		{
			if (TemplateIcon != null)
				return (int)TemplateIcon.Height + ((int)Ypad * 2);
			return textHeight + (RenderRecentTemplate ? recentTemplateHeadingTotalYPadding : groupTemplateHeadingTotalYPadding);
		}

		protected override void OnSnapshot (Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			snapshot.DrawCairo (cell_area, ctx => {
				if (Template == null) {
					DrawTemplateCategoryText (ctx, widget, cell_area, flags);
					return;
				}

				Gdk.Rectangle iconRect = DrawIcon (ctx, widget, cell_area, flags);

				DrawTemplateNameText (ctx, widget, cell_area, iconRect, flags);
				if (RenderRecentTemplate)
					DrawCategoryText (ctx, widget, cell_area, iconRect, flags);
			});
		}

		void DrawTemplateCategoryText (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			using (var layout = new Pango.Layout (widget.PangoContext)) {

				layout.Ellipsize = Pango.EllipsizeMode.End;
				int textPixelWidth = widget.Width - ((int)Xpad * 2);
				layout.Width = (int)(textPixelWidth * Pango.Scale.PangoScale);

				layout.SetMarkup (MarkupTopLevelCategoryName (TemplateCategory));

				int w, h;
				layout.GetPixelSize (out w, out h);

				int textX = cell_area.X + (int)Xpad + categoryTextPaddingX;
				int textY = cell_area.Y + (cell_area.Height - h) / 2 + groupTemplateHeadingYOffset;
				CellRendererDrawing.DrawLayout (ctx, CellRendererDrawing.GetTextColor (widget, flags), textX, textY, layout);
			}
		}

		Gdk.Rectangle DrawIcon (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			var iconRect = new Gdk.Rectangle (cell_area.X + (int)Xpad, cell_area.Y + (int)Ypad, (int)TemplateIcon.Width, (int)TemplateIcon.Height);

			var img = TemplateIcon;
			if ((flags & Gtk.CellRendererState.Selected) != 0)
				img = img.WithStyles ("sel");
			ctx.DrawImage (widget, img, iconRect.X, iconRect.Y);

			return iconRect;
		}

		void DrawTemplateNameText (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, Gdk.Rectangle iconRect, CellRendererState flags)
		{
			using (var layout = new Pango.Layout (widget.PangoContext)) {

				layout.Ellipsize = Pango.EllipsizeMode.End;
				int textPixelWidth = cell_area.Width - ((int)Xpad * 2) - iconRect.Width - iconTextPadding;
				layout.Width = (int)(textPixelWidth * Pango.Scale.PangoScale);

				layout.SetMarkup (GLib.Markup.EscapeText (Template.Name));

				int w, h;
				layout.GetPixelSize (out w, out h);
				int textY = cell_area.Y + (RenderRecentTemplate ? (2) : (cell_area.Height - h) / 2);

				CellRendererDrawing.DrawLayout (ctx, CellRendererDrawing.GetTextColor (widget, flags), iconRect.X + iconRect.Width + iconTextPadding, textY, layout);
			}
		}

		void DrawCategoryText (Cairo.Context ctx, Widget widget, Gdk.Rectangle cell_area, Gdk.Rectangle iconRect, CellRendererState flags)
		{
			var isSelected = CellRendererDrawing.IsSelected (flags);

			using (var layout = new Pango.Layout (widget.PangoContext)) {

				layout.Ellipsize = Pango.EllipsizeMode.End;
				int textPixelWidth = cell_area.Width - ((int)Xpad * 2) - iconRect.Width - iconTextPadding;
				layout.Width = (int)(textPixelWidth * Pango.Scale.PangoScale);
				layout.FontDescription = Fonts.FontExtensions.CopyModified (widget.PangoContext.FontDescription, -1);

				layout.SetMarkup (GLib.Markup.EscapeText (TemplateCategory));

				int w, h;
				layout.GetPixelSize (out w, out h);
				int textY = cell_area.Y + ((cell_area.Height - h) - 2);

				ctx.MoveTo (iconRect.X + iconRect.Width + iconTextPadding, textY);
				ctx.SetSourceColor ((isSelected ? Styles.BaseSelectionTextColor : Styles.SecondaryTextColor).ToCairoColor ());
				ctx.ShowLayout (layout);
			}
		}

		static string MarkupTopLevelCategoryName (string name)
		{
			return "<span font_weight='bold'>" + GLib.Markup.EscapeText (name) + "</span>";
		}
	}
}
