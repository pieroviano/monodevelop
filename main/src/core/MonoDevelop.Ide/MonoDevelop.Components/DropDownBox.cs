// 
// DropDownBox.cs
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
// THE SOFTWARE.

using System;
using System.ComponentModel;

using MonoDevelop.Ide;

namespace MonoDevelop.Components
{
	[Category ("Widgets")]
	[ToolboxItem (true)]
	public class DropDownBox : Gtk.Button
	{
		Pango.Layout layout;
		const int pixbufSpacing = 2;
		const int leftSpacing   = 2;
		const int ySpacing   = 1;
		
		public string Text {
			get {
				return layout.Text;
			}
			set {
				layout.SetMarkup (value);
//				QueueResize ();
			}
		}
		
		public DropDownBoxListWindow.IListDataProvider DataProvider {
			get;
			set;
		}
		
		public bool DrawButtonShape {
			get;
			set;
		}
		
		public bool DrawRightBorder {
			get;
			set;
		}
		
		public bool DrawLeftBorder {
			get;
			set;
		}
		
		public Xwt.Drawing.Image Pixbuf {
			get;
			set;
		}
		
		public object CurrentItem {
			get;
			set;
		}
		
		int defaultIconHeight, defaultIconWidth;
		
		/// <summary>
		/// This is so that the height doesn't jump around depending whether there's an icon assigned or not.
		/// </summary>
		public int DefaultIconHeight {
			get { return defaultIconHeight; }
			set {
				defaultIconHeight = value;
			}
		}
		
		public int DefaultIconWidth  {
			get { return defaultIconWidth; }
			set {
				defaultIconWidth = value;
			}
		}
		
		int fixedWidth;
		public int FixedWidth {
			get {
				return fixedWidth;
			}
			set {
				fixedWidth = value;
				WidthRequest = fixedWidth;
			}
		}

		int fixedHeight;
		public int FixedHeight {
			get {
				return fixedHeight;
			}
			set {
				fixedHeight = value;
				HeightRequest = fixedHeight;
			}
		}
		
		public DropDownBox ()
		{
			layout = CreatePangoLayout (null);
			this.CanFocus = true;
			this.Focusable = true;

			this.AddKeyPressHandler (OnKeyPressed);
			this.AddMotionHandler ((x, y) => QueueDraw (), null, QueueDraw);
			// Capture phase: open the list on press (GTK 2 behaviour), before the button's own click handling.
			this.AddClickHandler (OnButtonPressed, null, 0, Gtk.PropagationPhase.Capture);
		}

		/// <summary>Draws the button background through CSS; without <see cref="DrawButtonShape"/> the button is flat.</summary>
		protected override void OnRealized ()
		{
			base.OnRealized ();
			if (DrawButtonShape)
				RemoveCssClass ("flat");
			else
				AddCssClass ("flat");
		}
		
		void PositionListWindow ()
		{
			if (window == null)
				return;
			window.SetSizeRequest (Width, -1);
			window.Reparent (this);
			window.ShowAt (new Gdk.Rectangle (0, 0, Width, Height), Gtk.PositionType.Bottom);
			window.list.GrabFocus ();
		}
		
		public void SetItem (string text, Xwt.Drawing.Image icon, object currentItem)
		{
			if (currentItem != CurrentItem) {// don't update when the same item is set.
				this.Text = text;
				this.CurrentItem = currentItem;
				this.Pixbuf = icon;
				this.QueueDraw ();
			}
			
			if (ItemSet != null)
				ItemSet (this, EventArgs.Empty);
		}
		
		public void SetItem (int i)
		{
			SetItem (DataProvider.GetMarkup (i), DataProvider.GetIcon (i), DataProvider.GetTag (i));
		}
		
		/// <summary>Releases the list window and the text layout (GTK 2: ::destroy).</summary>
		public override void Destroy ()
		{
			DestroyWindow ();
			if (layout != null) {
				layout.Dispose ();
				layout = null;
			}
			base.Destroy ();
		}

		protected override void OnUnrealized ()
		{
			DestroyWindow ();
			base.OnUnrealized ();
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			int width = 0, height = 0;
			layout?.GetPixelSize (out width, out height);
			
			if (Pixbuf != null) {
				width += (int)Pixbuf.Width + pixbufSpacing * 2;
				height = System.Math.Max (height, (int)Pixbuf.Height);
			} else {
				height = System.Math.Max (height, defaultIconHeight);
			}
			
			if (DrawRightBorder)
				width += 2;
			int arrowHeight = height / 2; 
			int arrowWidth = arrowHeight + 1;
			
			if (orientation == Gtk.Orientation.Horizontal)
				minimum = natural = FixedWidth > 0 ? FixedWidth : width + arrowWidth + leftSpacing;
			else
				minimum = natural = FixedHeight > 0 ? FixedHeight : height + ySpacing * 2;
			minimum_baseline = natural_baseline = -1;
		}
		
		DropDownBoxListWindow window = null;
		internal void DestroyWindow ()
		{
			if (window != null) {
				var w = window;
				window = null;
				w.Destroy ();
				QueueDraw ();
			}
		}
		
		bool OnKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			var key = (Gdk.Key)keyval;
			if (key == Gdk.Key.Escape) {
				DestroyWindow (); 
				return true;
			}
			if (window != null && window.ProcessKey (key, state))
				return true;
			return false;
		}
		
		void OnButtonPressed (ClickInfo e)
		{
			if (e.TriggersContextMenu ())
				return;
			if (e.NPress != 1)
				return;
			if (window != null) {
				DestroyWindow ();
			} else {
				this.GrabFocus ();
				if (DataProvider != null) {
					DataProvider.Reset ();
					if (DataProvider.IconCount > 0) {
						var w = window = new DropDownBoxListWindow (DataProvider);
						w.list.SelectItem += delegate {
							SetItem (w.list.Selection);
						};
						w.Destroyed += delegate {
							if (window == w) {
								window = null;
								QueueDraw ();
							}
						};
						PositionListWindow ();
						w.SelectItem (CurrentItem);
					}
				}
			}
		}
	
		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot); // CSS button background
			snapshot.DrawCairo (this, Draw);
		}

		void Draw (Cairo.Context ctx)
		{
			if (layout == null)
				return;
			int width, height;
			layout.GetPixelSize (out width, out height);
			int allocWidth = Width, allocHeight = Height;
			
			int arrowHeight = height / 2; 
			int arrowWidth = arrowHeight + 1;
			int arrowXPos = allocWidth - arrowWidth;
			if (DrawButtonShape) {
				arrowXPos -= 4;
			} else if (DrawRightBorder) {
				arrowXPos -= 2;
			}

			//HACK: don't ever draw insensitive text, because insensitive generally looks really ugly
			var textColor = StyleContext.Color.ToCairoColor ();
			textColor.A = 1;

			int xPos = 0;
			if (Pixbuf != null) {
				ctx.DrawImage (this, Pixbuf, xPos + pixbufSpacing, (allocHeight - Pixbuf.Height) / 2);
				xPos += (int)Pixbuf.Width + pixbufSpacing * 2;
			}
			if (DrawButtonShape)
				xPos += 4;
			
			//constrain the text area so it doesn't get rendered under the arrows
			var textArea = new Gdk.Rectangle (xPos, ySpacing, arrowXPos - xPos - 2, allocHeight - ySpacing);

			if (FixedWidth > 0) {
				layout.Ellipsize = Pango.EllipsizeMode.End;
				layout.Width = (allocWidth - textArea.X) * (int)Pango.Scale.PangoScale;
			}

			ctx.Save ();
			ctx.Rectangle (textArea.X, textArea.Y, Math.Max (0, textArea.Width), Math.Max (0, textArea.Height));
			ctx.Clip ();
			ctx.MoveTo (textArea.X, textArea.Y + Math.Max (0, (textArea.Height - height) / 2));
			ctx.SetSourceColor (textColor);
			Pango.CairoHelper.ShowLayout (ctx, layout);
			ctx.Restore ();
			
			var arrowColor = textColor;
			if (!Sensitive)
				arrowColor.A = 0.5;
			ctx.SetSourceColor (arrowColor);
			// up arrow
			double midY = allocHeight / 2;
			ctx.MoveTo (arrowXPos + arrowWidth / 2.0, midY - arrowHeight);
			ctx.LineTo (arrowXPos + arrowWidth, midY - 1);
			ctx.LineTo (arrowXPos, midY - 1);
			ctx.ClosePath ();
			ctx.Fill ();
			// down arrow
			ctx.MoveTo (arrowXPos, midY + 1);
			ctx.LineTo (arrowXPos + arrowWidth, midY + 1);
			ctx.LineTo (arrowXPos + arrowWidth / 2.0, midY + arrowHeight);
			ctx.ClosePath ();
			ctx.Fill ();

			if (!DrawButtonShape && (DrawRightBorder || DrawLeftBorder)) {
				var borderColor = textColor;
				borderColor.A = 0.3;
				ctx.SetSourceColor (borderColor);
				ctx.LineWidth = 1;
				if (DrawRightBorder) {
					ctx.MoveTo (allocWidth - 0.5, 0);
					ctx.LineTo (allocWidth - 0.5, allocHeight);
				}
				if (DrawLeftBorder) {
					ctx.MoveTo (0.5, 0);
					ctx.LineTo (0.5, allocHeight);
				}
				ctx.Stroke ();
			}
		}
		
		public EventHandler ItemSet;
	}
}
