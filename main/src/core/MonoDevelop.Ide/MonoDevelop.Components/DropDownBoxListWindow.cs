// 
// DropDownBoxListWindow.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Ide;
using System.Text;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Core;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Drop-down list of items (path bar, <see cref="DropDownBox"/>). GTK 4: an autohide <see cref="Gtk.Popover"/>;
	/// attach it with <c>popover.Reparent (anchor)</c> and show it with <c>popover.ShowAt (area)</c>
	/// (or let <see cref="DropDownBox"/> do it). <see cref="Destroy"/> pops it down and detaches it.
	/// </summary>
	public class DropDownBoxListWindow : Gtk.Popover
	{
		readonly Gtk.ScrolledWindow vScrollbar;
		internal ListWidget list;
		bool destroyed;

		public IListDataProvider DataProvider {
			get;
			private set;
		}

		public int FixedRowHeight {
			get {
				return list.FixedRowHeight;
			}
			set {
				list.FixedRowHeight = value;
				list.CalcRowHeight ();
			}
		}

		public int MaxVisibleRows {
			get {
				return list.MaxVisibleRows;
			}
			set {
				list.MaxVisibleRows = value;
				list.CalcVisibleRows ();
			}
		}

		public DropDownBoxListWindow (IListDataProvider provider)
		{
			Name = "DropDownBoxListWindow";

			this.DataProvider = provider;
			Autohide = true;
			HasArrow = false;
			Position = Gtk.PositionType.Bottom;
			Halign = Gtk.Align.Start;

			vScrollbar = new Gtk.ScrolledWindow ();
			vScrollbar.SetPolicy (Gtk.PolicyType.Never, Gtk.PolicyType.Automatic);
			vScrollbar.PropagateNaturalWidth = true;

			list = new ListWidget (this);
			list.Name = "DropDownBoxListWindow.List";

			list.SelectItem += delegate {
				var sel = list.Selection;
				if (sel >= 0 && sel < DataProvider.IconCount) {
					try {
						DataProvider.ActivateItem (sel);
					} catch (Exception ex) {
						LoggingService.LogInternalError ("Offset seems to be out of sync with the snapshot.", ex);
					}

					Destroy ();
				}
			};
			// Non-scrollable child: the scrolled window wraps it in a viewport.
			vScrollbar.Child = list;
			Child = vScrollbar;
			list.CalcVisibleRows ();

			// Keys typed while the list is open (the popover takes the keyboard focus).
			this.AddKeyPressHandler ((keyval, keycode, state) => ProcessKey ((Gdk.Key)keyval, state));
			Closed += (s, e) => GLib.Idle.Add (() => {
				Destroy ();
				return false;
			});
		}

		internal Gtk.Adjustment VAdjustment => vScrollbar.Vadjustment;

		internal void SetVisibleHeight (int height)
		{
			vScrollbar.MinContentHeight = -1;
			vScrollbar.MaxContentHeight = height;
			vScrollbar.MinContentHeight = height;
		}

		public void SelectItem (object item)
		{
			int count = DataProvider.IconCount;
			for (int i = 0; i < count; i++) {
				if (DataProvider.GetTag (i) == item) {
					list.Selection = i;
					var adj = vScrollbar.Vadjustment;
					adj.Value = Math.Max (0, i * list.RowHeight - adj.PageSize / 2);
					break;
				}
			}
		}

		void SwitchToSeletedWord ()
		{
			string selection = list.WordSelection.ToString ();
			int count = DataProvider.IconCount;
			for (int i = 0; i < count; i++) {
				if (DataProvider.GetMarkup (i).StartsWith (selection, StringComparison.OrdinalIgnoreCase)) {
					list.Selection = i;
					list.WordSelection.Append (selection);
				}
			}
		}

		public bool ProcessKey (Gdk.Key key, Gdk.ModifierType modifier)
		{
			switch (key) {
			case Gdk.Key.Up:
				if (list.SelectionDisabled)
					list.SelectionDisabled = false;
				else
					list.Selection--;
				return true;

			case Gdk.Key.Down:
				if (list.SelectionDisabled)
					list.SelectionDisabled = false;
				else
					list.Selection++;
				return true;

			case Gdk.Key.Page_Up:
				list.Selection -= list.VisibleRows - 1;
				return true;

			case Gdk.Key.Page_Down:
				list.Selection += list.VisibleRows - 1;
				return true;

			case Gdk.Key.Home:
				list.Selection = 0;
				return true;

			case Gdk.Key.End:
				list.Selection = DataProvider.IconCount;
				return true;

			case Gdk.Key.Escape:
				Destroy ();
				return true;

			case Gdk.Key.Return:
			case Gdk.Key.ISO_Enter:
			case Gdk.Key.Key_3270_Enter:
			case Gdk.Key.KP_Enter:
				list.OnSelectItem (EventArgs.Empty);
				return true;
			default:
				char ch = (char)key;
				if (char.IsLetterOrDigit (ch)) {
					list.WordSelection.Append (ch);
					SwitchToSeletedWord ();
				}
				break;
			}

			return false;
		}

		/// <summary>Raised once when the list is popped down and detached (GTK 2: ::destroy).</summary>
		public event EventHandler Destroyed;

		/// <summary>Pops the list down and detaches it from its anchor.</summary>
		public override void Destroy ()
		{
			if (destroyed || Handle == IntPtr.Zero)
				return;
			destroyed = true;
			if (Visible)
				Popdown ();
			list.Cleanup ();
			base.Destroy ();
			Destroyed?.Invoke (this, EventArgs.Empty);
		}

		internal class ListWidget: CairoWidget
		{
			const int leftXAlignment = 1;
			const int padding = 4;
			const int iconTextDistance = 4;
			int listWidth = 300;
			Pango.Layout layout;
			readonly DropDownBoxListWindow win;
			int selection;
			int rowHeight;

			public int RowHeight {
				get {
					return rowHeight;
				}
			}

			public int FixedRowHeight {
				get;
				set;
			}
			bool disableSelection;

			public event EventHandler SelectionChanged;

			public ListWidget (DropDownBoxListWindow win)
			{
				this.SetRole (AtkCocoa.Roles.AXList);
				this.win = win;
				this.CanFocus = true;
				this.Focusable = true;
				UpdateStyle ();

				this.AddClickHandler (OnButtonPressed, null, 1);
				this.AddMotionHandler (OnMotion, null, OnLeave);

				CalcAccessibility ();
			}

			class TextElement : AtkCocoaHelper.AccessibilityElementProxy
			{
				public int RowIndex { get; set; }
				internal TextElement ()
				{
				}
			}

			TextElement[] textElements;
			void CalcAccessibility ()
			{
				if (!AtkCocoaHelper.AccessibilityElementProxy.Enabled) {
					return;
				}

				var columnElement = new AtkCocoaHelper.AccessibilityElementProxy ();
				columnElement.GtkParent = this;
				columnElement.SetRole (AtkCocoa.Roles.AXColumn);
				this.AddAccessibleElement (columnElement);

				int count = win.DataProvider.IconCount;
				textElements = new TextElement[count];
				for (int i = 0; i < count; i++) {
					var rowElement = new AtkCocoaHelper.AccessibilityElementProxy ();
					rowElement.GtkParent = this;
					rowElement.SetRole (AtkCocoa.Roles.AXRow);
					this.AddAccessibleElement (rowElement);

					var cellElement = new AtkCocoaHelper.AccessibilityElementProxy ();
					cellElement.GtkParent = this;
					cellElement.SetRole (AtkCocoa.Roles.AXCell);
					columnElement.AddAccessibleChild (cellElement);
					rowElement.AddAccessibleChild (cellElement);

					var textElement = textElements[i] = new TextElement ();
					textElement.RowIndex = i;
					textElement.PerformPress += PerformPress;
					textElement.GtkParent = this;
					textElement.Value = win.DataProvider.GetMarkup (i);
					cellElement.AddAccessibleChild (textElement);
				}
			}

			void PerformPress (object sender, EventArgs args)
			{
				try {
					var element = (TextElement)sender;
					win.DataProvider.ActivateItem (element.RowIndex);
				} catch (Exception ex) {
					LoggingService.LogInternalError ("Offset seems to be out of sync with the snapshot.", ex);
				}
			}

			internal void CalcRowHeight ()
			{
				if (FixedRowHeight > 0) {
					rowHeight = FixedRowHeight;
				} else {
					layout.SetText ("|");
					int rowWidth;
					layout.GetPixelSize (out rowWidth, out rowHeight);
					rowHeight += padding;
				}
				CalcVisibleRows ();
			}

			void OnLeave ()
			{
				selection = -1;
				QueueDraw ();
			}

			internal void Cleanup ()
			{
				if (textElements != null) {
					foreach (var textElement in textElements) {
						textElement.PerformPress -= PerformPress;
					}
					textElements = null;
				}

				if (layout != null) {
					layout.Dispose ();
					layout = null;
				}
			}

			public void Reset ()
			{
				if (win.DataProvider == null) {
					selection = -1;
					return;
				}

				selection = win.DataProvider.IconCount == 0 ? -1 : 0;
				CalcVisibleRows ();
				disableSelection = false;
				if (IsRealized) {
					UpdateStyle ();
					QueueDraw ();
				}
				if (SelectionChanged != null)
					SelectionChanged (this, EventArgs.Empty);
			}

			readonly StringBuilder wordSelection = new StringBuilder ();

			public StringBuilder WordSelection {
				get {
					return wordSelection;
				}
			}

			public int Selection {
				get {
					return selection;
				}

				set {
					wordSelection.Length = 0;
					var newValue = Math.Max (0, Math.Min (value, win.DataProvider.IconCount - 1));

					if (newValue != selection) {
						selection = newValue;
						UpdatePage ();

						if (SelectionChanged != null)
							SelectionChanged (this, EventArgs.Empty);
					}

					disableSelection &= !disableSelection;

					QueueDraw ();
				}
			}

			void UpdatePage ()
			{
				var vAdjustment = win.VAdjustment;
				if (vAdjustment == null || selection < 0)
					return;
				var area = GetRowArea (selection);
				var value = vAdjustment.Value;
				if (area.Y < value) {
					value = area.Y;
				} else if (value + vAdjustment.PageSize < area.Bottom) {
					value = Math.Max (0, area.Bottom - vAdjustment.PageSize + 1);
				}
				vAdjustment.Value = Math.Max (vAdjustment.Lower, Math.Min (value, vAdjustment.Upper - vAdjustment.PageSize));
			}

			public bool SelectionDisabled {
				get { return disableSelection; }
				set {
					disableSelection = value;
					QueueDraw ();
				}
			}

			void OnButtonPressed (ClickInfo evnt)
			{
				Selection = GetRowByPosition ((int)evnt.Y);
				if (evnt.NPress == 1) {
					evnt.Claim ();
					OnSelectItem (EventArgs.Empty);
				}
			}

			void OnMotion (double x, double y)
			{
				Selection = GetRowByPosition ((int)y);
			}

			protected override void OnDraw (Cairo.Context cr, int winWidth, int winHeight)
			{
				if (layout == null || rowHeight <= 0)
					return;

				var bg = Ide.Gui.Styles.BaseBackgroundColor.ToCairoColor ();
				var fg = Ide.Gui.Styles.BaseForegroundColor.ToCairoColor ();
				var selBg = Ide.Gui.Styles.BaseSelectionBackgroundColor.ToCairoColor ();
				var selFg = Ide.Gui.Styles.BaseSelectionTextColor.ToCairoColor ();

				cr.SetSourceColor (bg);
				cr.Paint ();

				int lineWidth = winWidth - leftXAlignment * 2;
				const int xpos = leftXAlignment + padding;

				// The list is inside a viewport: only draw the rows in the visible page.
				var vAdjustment = win.VAdjustment;
				double top = vAdjustment?.Value ?? 0;
				double bottom = vAdjustment != null && vAdjustment.PageSize > 0 ? top + vAdjustment.PageSize : winHeight;

				int n = Math.Max (0, (int)(top / rowHeight));
				int ypos = leftXAlignment + n * rowHeight;
				while (ypos < Math.Min (bottom, winHeight - leftXAlignment) && n < win.DataProvider.IconCount) {
					string text = win.DataProvider.GetMarkup (n) ?? "&lt;null&gt;";

					var icon = win.DataProvider.GetIcon (n);
					int iconHeight = icon != null ? (int)icon.Height : 24;
					int iconWidth = icon != null ? (int)icon.Width : 0;

					layout.Ellipsize = Pango.EllipsizeMode.End;
					layout.Width = (winWidth - xpos - padding - iconWidth - iconTextDistance) * (int)Pango.Scale.PangoScale;
					layout.SetMarkup (PathBar.GetFirstLineFromMarkup (text));

					int wi, he, typos, iypos;
					layout.GetPixelSize (out wi, out he);
					typos = he < rowHeight ? ypos + (rowHeight - he) / 2 : ypos;
					iypos = iconHeight < rowHeight ? ypos + (rowHeight - iconHeight) / 2 : ypos;

					var textColor = fg;
					if (n == selection) {
						cr.Rectangle (leftXAlignment, ypos, lineWidth, rowHeight);
						if (!disableSelection) {
							cr.SetSourceColor (selBg);
							cr.Fill ();
							textColor = selFg;
							if (icon != null)
								icon = icon.WithStyles ("sel");
						} else {
							cr.SetSourceColor (selBg);
							cr.LineWidth = 1;
							cr.Stroke ();
						}
					}
					cr.MoveTo (xpos + iconWidth + iconTextDistance, typos);
					cr.SetSourceColor (textColor);
					Pango.CairoHelper.ShowLayout (cr, layout);

					if (icon != null)
						cr.DrawImage (this, icon, xpos, iypos);

					ypos += rowHeight;
					n++;

//reset the markup or it carries over to the next SetText
					layout.SetMarkup (string.Empty);
				}
			}

			int GetRowByPosition (int ypos)
			{
				// ypos is in list coordinates (the viewport scrolls the whole list widget)
				return rowHeight > 0 ? ypos / rowHeight : 0;
			}

			public Gdk.Rectangle GetRowArea (int row)
			{
				return new Gdk.Rectangle (0, row * rowHeight, Width, rowHeight - 1);
			}

			public int VisibleRows {
				get {
					var vAdjustment = win.VAdjustment;
					var pageHeight = vAdjustment != null && vAdjustment.PageSize > 0 ? (int)vAdjustment.PageSize : Height;
					return rowHeight > 0 ? Math.Max (1, pageHeight / rowHeight) : 1;
				}
			}

			internal int MaxVisibleRows = 8;

			internal void CalcVisibleRows ()
			{
				if (layout == null)
					return;
				int count = win.DataProvider.IconCount;
				int newHeight;
				newHeight = count > MaxVisibleRows ?
								(rowHeight * MaxVisibleRows) + leftXAlignment * 2 :
								(rowHeight * count) + leftXAlignment * 2;
				newHeight += 2;
				listWidth = Math.Min (450, CalcWidth ());
				// The list has its full height; the scrolled window shows MaxVisibleRows of it.
				SetSizeRequest (listWidth, rowHeight * count + leftXAlignment * 2 + 2);
				win.SetVisibleHeight (newHeight);
			}

			internal int CalcWidth ()
			{
				if (win.DataProvider.IconCount == 0)
					return 0;
				int longest = 0;
				string longestText = win.DataProvider.GetMarkup (0) ?? "";

				int count = win.DataProvider.IconCount;
				for (int i = 1; i < count; i++) {
					string curText = win.DataProvider.GetMarkup (i) ?? "";
					if (curText.Length > longestText.Length) {
						longestText = curText;
						longest = i;
					}
				}
				layout.Width = -1;
				layout.SetMarkup (longestText ?? "&lt;null&gt;");
				int w, h;
				layout.GetPixelSize(out w, out h);
				var icon = win.DataProvider.GetIcon (longest);
				int iconWidth = icon != null ? (int) icon.Width : 24;
				return iconWidth + iconTextDistance + (padding * 2) + leftXAlignment + w;
			}

			protected override void OnRealized ()
			{
				base.OnRealized ();
				UpdateStyle ();
			}

			void UpdateStyle ()
			{
				if (layout != null)
					layout.Dispose ();
				layout = CreatePangoLayout (null);
				layout.Wrap = Pango.WrapMode.Char;
				layout.FontDescription = IdeServices.FontService.SansFont.CopyModified (Ide.Gui.Styles.FontScale11);
				CalcRowHeight ();
			}

			internal virtual void OnSelectItem (EventArgs e)
			{
				if (SelectItem != null)
					SelectItem (this, e);
			}

			public event EventHandler SelectItem;
		}

		public interface IListDataProvider
		{
			int IconCount {
				get;
			}

			void Reset ();

			string GetMarkup (int n);

			Xwt.Drawing.Image GetIcon (int n);

			object GetTag (int n);

			void ActivateItem (int n);
		}
	}
}
