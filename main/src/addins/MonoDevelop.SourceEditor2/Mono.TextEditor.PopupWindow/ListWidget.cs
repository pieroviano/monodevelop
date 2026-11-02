// 
// ListWidget.cs
//  
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2005 Novell, Inc (http://www.novell.com)
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

using Gtk;
using Gdk;
using Pango;
using System;
using System.Text;
using Mono.TextEditor.Theatrics;
using MonoDevelop.Components;

namespace Mono.TextEditor.PopupWindow
{
	class ListWidget<T> : Gtk.Widget, IScrollAdjustable
	{
		int margin = 0;
		int padding = 4;
		int listWidth = 300;

		Xwt.Drawing.TextLayout layout;
		ListWindow<T> win;
		int selection = 0;
		int visibleRows = -1;
		int rowHeight;
		bool buttonPressed;
		bool disableSelection;

		protected virtual void OnSelectionChanged (EventArgs e)
		{
			EventHandler handler = this.SelectionChanged;
			if (handler != null)
				handler (this, e);
		}
		public event EventHandler SelectionChanged;

		/// <summary>Raised for every press, with the number of consecutive presses (2 for a double click).</summary>
		public event EventHandler<ClickInfo> Pressed;

		public ListWidget (ListWindow<T> win)
		{
			this.win = win;
			hadj = new Gtk.Adjustment (0, 0, 0, 0, 0, 0);
			vadj = new Gtk.Adjustment (0, 0, 0, 0, 0, 0);
			vadj.ValueChanged += (sender, e) => QueueDraw ();

			this.AddClickHandler (e => {
				Selection = GetRowByPosition ((int) (vadj.Value + e.Y));
				buttonPressed = true;
				Pressed?.Invoke (this, e);
			}, e => buttonPressed = false);
			this.AddMotionHandler ((x, y) => {
				if (buttonPressed)
					Selection = GetRowByPosition ((int) (vadj.Value + y));
			});
			this.AddScrollHandler ((dx, dy, state) => {
				vadj.AddValueClamped (dy * rowHeight);
				return true;
			}, Gtk.EventControllerScrollFlags.Vertical);
			Realized += (o, a) => {
				UpdateStyle ();
				UpdatePage ();
			};
		}

		public void Reset ()
		{
			if (win.DataProvider == null) {
				selection = -1;
				return;
			}

			if (win.DataProvider.Count == 0)
				selection = -1;
			else
				selection = 0;

			disableSelection = false;
			if (IsRealized) {
				UpdateStyle ();
				QueueDraw ();
			}
			OnSelectionChanged (EventArgs.Empty);
			SetAdjustments (Width, Height);
		}

		public int Selection
		{
			get {
				return selection;
			}

			set {
				if (value < 0)
					value = 0;
				if (value >= win.DataProvider.Count)
					value = win.DataProvider.Count - 1;

				if (value != selection) {
					selection = value;
					UpdatePage ();

					OnSelectionChanged (EventArgs.Empty);
				}

				if (disableSelection)
					disableSelection = false;

				this.QueueDraw ();
			}
		}

		void UpdatePage ()
		{
			var area = GetRowArea (selection);
			if (area.Y < vadj.Value) {
				vadj.Value = area.Y;
				return;
			}
			if (vadj.Value + Height < area.Bottom) {
				vadj.Value = System.Math.Max (0, area.Bottom - vadj.PageSize + 1);
			}
		}

		public bool SelectionDisabled
		{
			get { return disableSelection; }

			set {
				disableSelection = value;
				this.QueueDraw ();
			}
		}

		Gtk.Adjustment hadj;
		Gtk.Adjustment vadj;

		void IScrollAdjustable.SetScrollAdjustments (Gtk.Adjustment hadj, Gtk.Adjustment vadj)
		{
			this.hadj = hadj;
			this.vadj = vadj;
			if (this.vadj != null)
				this.vadj.ValueChanged += (sender, e) => QueueDraw ();
		}

		void SetAdjustments (int width, int height)
		{
			if (hadj == null || vadj == null || win.DataProvider == null)
				return;
			hadj.SetBounds (0, width, 0, 0, width);
			var totalHeight = System.Math.Max (height, rowHeight * this.win.DataProvider.Count);
			vadj.SetBounds (0, totalHeight, rowHeight, height, height);
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			SetAdjustments (width, height);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			if (visibleRows == -1 && layout != null)
				CalcVisibleRows ();
			minimum = natural = orientation == Gtk.Orientation.Horizontal ? listWidth : System.Math.Max (0, requestedHeight);
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			snapshot.DrawCairo (this, cr => DrawList (cr, Width, Height));
		}

		public int TextOffset {
			get {
				int iconWidth, iconHeight;
				StockIconSize.Menu.GetSize (out iconWidth, out iconHeight);
				return iconWidth + margin + padding + 2;
			}
		}

		void DrawList (Cairo.Context context, int winWidth, int winHeight)
		{
			if (layout == null)
				UpdateStyle ();

			int ypos = margin;
			int lineWidth = winWidth - margin*2;
			int xpos = margin + padding;

			using (var cr = this.CreateXwtContext (context)) {

				var textColor = MonoDevelop.Ide.Gui.Styles.BaseForegroundColor;
				var selectionColor = MonoDevelop.Ide.Gui.Styles.BaseSelectionBackgroundColor;
				var selectionTextColor = MonoDevelop.Ide.Gui.Styles.BaseSelectionTextColor;

				int n = 0;
				n = (int)(vadj.Value / rowHeight);

				while (ypos < winHeight - margin && n < win.DataProvider.Count) {
					bool hasMarkup = false;
					IMarkupListDataProvider<T> markupListDataProvider = win.DataProvider as IMarkupListDataProvider<T>;
					if (markupListDataProvider != null) {
						if (markupListDataProvider.HasMarkup (n)) {
							layout.Markup = (markupListDataProvider.GetMarkup (n) ?? "&lt;null&gt;");
							hasMarkup = true;
						}
					}

					if (!hasMarkup)
						layout.Text = (win.DataProvider.GetText (n) ?? "<null>");

					Xwt.Drawing.Image icon = win.DataProvider.GetIcon (n);
					int iconHeight, iconWidth;

					if (icon != null) {
						iconWidth = (int)icon.Width;
						iconHeight = (int)icon.Height;
					} else {
						StockIconSize.Menu.GetSize (out iconWidth, out iconHeight);
					}

					var s = layout.GetSize ();
					int typos, iypos;
					int he = (int)s.Height;

					typos = he < rowHeight ? ypos + (rowHeight - he) / 2 : ypos;
					iypos = iconHeight < rowHeight ? ypos + (rowHeight - iconHeight) / 2 : ypos;

					if (n == selection) {
						if (!disableSelection) {
							cr.Rectangle (margin, ypos, lineWidth, he + padding);
							cr.SetColor (selectionColor);
							cr.Fill ();

							cr.SetColor (selectionTextColor);
							cr.DrawTextLayout (layout, xpos + iconWidth + 2, typos);
						} else {
							cr.Rectangle (margin, ypos, lineWidth, he + padding);
							cr.SetColor (selectionColor);
							cr.Stroke ();

							cr.SetColor (textColor);
							cr.DrawTextLayout (layout, xpos + iconWidth + 2, typos);
						}
					} else {
						cr.SetColor (textColor);
						cr.DrawTextLayout (layout, xpos + iconWidth + 2, typos);
					}

					if (icon != null)
						cr.DrawImage (icon, xpos, iypos);

					ypos += rowHeight;
					n++;

					//reset the markup or it carries over to the next SetText
					if (hasMarkup)
						layout.Markup = string.Empty;
				}
			}
		}

		int GetRowByPosition (int ypos)
		{
			return ypos / rowHeight;
		}

		public Gdk.Rectangle GetRowArea (int row)
		{
			return new Gdk.Rectangle (0, row * rowHeight, Width, rowHeight);
		}

		public int VisibleRows
		{
			get {
				if (visibleRows == -1) CalcVisibleRows ();
				return visibleRows;
			}
		}

		int requestedHeight;

		void CalcVisibleRows ()
		{
			if (layout == null)
				UpdateStyle ();
			var s = layout.GetSize ();
			rowHeight = (int)s.Height + padding;

			visibleRows = 7;//(winHeight + padding - margin * 2) / rowHeight;

			int newHeight;

			if (this.win.DataProvider.Count > this.visibleRows)
				newHeight = (rowHeight * visibleRows) + margin * 2;
			else
				newHeight = (rowHeight * this.win.DataProvider.Count) + margin * 2;

			if (requestedHeight != newHeight) {
				requestedHeight = newHeight;
				QueueResize ();
			}
		}

		void UpdateStyle ()
		{
			if (layout != null)
				layout.Dispose ();
			layout = new Xwt.Drawing.TextLayout ();
			layout.Trimming = Xwt.Drawing.TextTrimming.Word;

			CalcVisibleRows ();
		}
	}
}
