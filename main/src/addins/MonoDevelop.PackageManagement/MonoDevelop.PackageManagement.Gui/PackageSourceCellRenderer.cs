//
// PackageSourceCellRenderer.cs
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

using System;
using Gtk;
using MonoDevelop.PackageManagement;
using MonoDevelop.Components;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Projects;


namespace MonoDevelop.PackageManagement
{
	internal class PackageSourceCellRenderer : CellRenderer
	{
		[GLib.Property("package-source")]
		public PackageSourceViewModel PackageSourceViewModel { get; set; }

		[GLib.Property("text")]
		public string Text {
			get {
				if (PackageSourceViewModel == null) {
					return "";
				}

				return PackageSourceViewModel.Name + " - " + PackageSourceViewModel.Source;
			}
		}

		static Xwt.Drawing.Image warningImage = ImageService.GetIcon (MonoDevelop.Ide.Gui.Stock.Warning, StockIconSize.Menu);
		const int imageSpacing = 5;
		const int textSpacing = 7;
		const int textTopSpacing = 3;

		protected override void OnSnapshot (Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			if (PackageSourceViewModel == null)
				return;

			using (var layout = new Pango.Layout (widget.PangoContext)) {
				layout.Alignment = Pango.Alignment.Left;
				layout.SetMarkup (GetPackageSourceNameMarkup ());
				int packageSourceNameWidth = GetLayoutWidth (layout);
				var textColor = CellRendererDrawing.GetTextColor (widget, flags);

				layout.SetMarkup (GetPackageSourceDescriptionMarkup (flags));

				snapshot.DrawCairo (cell_area, ctx => {
					CellRendererDrawing.DrawLayout (ctx, textColor, cell_area.X + textSpacing, cell_area.Y + textTopSpacing, layout);

					if (!PackageSourceViewModel.IsValid) {
						ctx.DrawImage (widget, warningImage, cell_area.X + textSpacing + packageSourceNameWidth + imageSpacing, cell_area.Y + textTopSpacing);

						layout.SetMarkup (GetPackageSourceErrorMarkup (flags));
						int packageSourceErrorTextX = cell_area.X + textSpacing + packageSourceNameWidth + (int)warningImage.Width + (2 * imageSpacing);
						CellRendererDrawing.DrawLayout (ctx, textColor, packageSourceErrorTextX, cell_area.Y + textTopSpacing, layout);
					}
				});
			}
		}

		string GetPackageSourceNameMarkup ()
		{
			return MarkupString.Format (
				"<b>{0}</b>",
				PackageSourceViewModel.Name);
		}

		int GetLayoutWidth (Pango.Layout layout)
		{
			return GetLayoutSize (layout).Width;
		}

		Gdk.Size GetLayoutSize (Pango.Layout layout)
		{
			int width;
			int height;
			layout.GetPixelSize (out width, out height);

			return new Gdk.Size (width, height);
		}

		string GetPackageSourceDescriptionMarkup (CellRendererState flags = CellRendererState.Focused)
		{
			return MarkupString.Format (
				"<b>{0}</b>\n<span foreground='{2}'>{1}</span>",
				PackageSourceViewModel.Name,
				PackageSourceViewModel.Source,
				Ide.Gui.Styles.ColorGetHex (flags.HasFlag (CellRendererState.Selected) ? Styles.PackageSourceUrlSelectedTextColor : Styles.PackageSourceUrlTextColor));
		}

		string GetPackageSourceErrorMarkup (CellRendererState flags = CellRendererState.Focused)
		{
			return MarkupString.Format (
				"<span foreground='{0}'>{1}</span>",
				Ide.Gui.Styles.ColorGetHex (flags.HasFlag (CellRendererState.Selected) ? Styles.PackageSourceErrorSelectedTextColor : Styles.PackageSourceErrorTextColor),
				PackageSourceViewModel.ValidationFailureMessage);
		}

		protected override void OnGetPreferredHeight (Widget widget, out int minimum_size, out int natural_size)
		{
			using (var layout = new Pango.Layout (widget.PangoContext)) {
				layout.SetMarkup (GetPackageSourceDescriptionMarkup ());
				minimum_size = natural_size = GetLayoutSize (layout).Height + 8 + textTopSpacing;
			}
		}

		protected override void OnGetPreferredHeightForWidth (Widget widget, int width, out int minimum_height, out int natural_height)
		{
			OnGetPreferredHeight (widget, out minimum_height, out natural_height);
		}
	}
}
