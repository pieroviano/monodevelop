//
// DockBar.cs
//
// Author:
//   Lluis Sanchez Gual
//

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
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components.Docking
{
	class DockBar: BinWidget
	{
		Gtk.PositionType position;
		Box box;
		DockFrame frame;
		Label filler;
		bool alwaysVisible;
		bool showBorder = true;
		readonly List<DockBarItem> items = new List<DockBarItem> ();

		internal DockBar (DockFrame frame, Gtk.PositionType position)
		{
			this.frame = frame;
			this.position = position;
			box = new Box (Orientation, 0);
			// The items keep their natural size at the start of the bar
			box.Halign = Align.Start;
			box.Valign = Align.Start;
			Child = box;
			UpdateBorderPadding ();

			filler = new Label ();
			filler.WidthRequest = 4;
			filler.HeightRequest = 4;
			box.Append (filler);

			UpdateVisibility ();
		}

		// Reserves the space for the separator line
		void UpdateBorderPadding ()
		{
			var p = new Gtk.Border ();
			if (showBorder) {
				switch (position) {
				case PositionType.Left: p.Right = 1; break;
				case PositionType.Right: p.Left = 1; break;
				case PositionType.Top: p.Bottom = 1; break;
				case PositionType.Bottom: p.Top = 1; break;
				}
			}
			Padding = p;
		}
		
		public bool IsExtracted {
			get { return OriginalBar != null; }
		}
		
		internal DockBar OriginalBar { get; set; }
		
		public bool AlwaysVisible {
			get { return this.alwaysVisible; }
			set { this.alwaysVisible = value; UpdateVisibility (); }
		}

		public bool AlignToEnd { get; set; }

		public bool ShowBorder {
			get { return showBorder; }
			set { showBorder = value; UpdateBorderPadding (); QueueResize (); }
		}
		
		internal Gtk.Orientation Orientation {
			get {
				return (position == PositionType.Left || position == PositionType.Right) ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal;
			}
		}
		
		internal Gtk.PositionType Position {
			get {
				return position;
			}
		}

		internal DockFrame Frame {
			get {
				return frame;
			}
		}

		DateTime hoverActivationDelay;

		void DisableHoverActivation ()
		{
			// Temporarily disables hover activation of items in this bar
			// Useful to avoid accidental triggers when the bar items
			// are reallocated
			hoverActivationDelay = DateTime.Now + TimeSpan.FromSeconds (1.5);
		}

		internal bool HoverActivationEnabled {
			get { return DateTime.Now >= hoverActivationDelay; }
		}
		
		internal DockBarItem AddItem (DockItem item, int size)
		{
			DisableHoverActivation ();
			DockBarItem it = new DockBarItem (this, item, size);
			// Items go before the filler, in insertion order
			box.InsertChildAfter (it, items.Count > 0 ? items [items.Count - 1] : null);
			items.Add (it);
			UpdateVisibility ();
			it.Shown += OnItemVisibilityShown;
			it.Hidden += OnItemVisibilityHidden;
			return it;
		}
		
		void OnItemVisibilityShown (object o, EventArgs args)
		{
			DisableHoverActivation ();
			UpdateVisibility ();
		}

		void OnItemVisibilityHidden (object o, EventArgs args)
		{
			UpdateVisibility ();
		}

		internal void OnCompactLevelChanged ()
		{
			UpdateVisibility ();
			if (OriginalBar != null)
				OriginalBar.UpdateVisibility ();
		}
		
		internal void UpdateVisibility ()
		{
			if (Frame.OverlayWidgetVisible) {
				Visible = false;
			} else {
				filler.Visible = (Frame.CompactGuiLevel < 3);
				int visibleCount = 0;
				foreach (var w in items) {
					if (w.Visible)
						visibleCount++;
				}
				Visible = alwaysVisible || filler.Visible || visibleCount > 0;
			}
		}
		
		internal void RemoveItem (DockBarItem it)
		{
			DisableHoverActivation ();
			items.Remove (it);
			box.Remove (it);
			it.Shown -= OnItemVisibilityShown;
			it.Hidden -= OnItemVisibilityHidden;
			UpdateVisibility ();
		}

		internal void UpdateTitle (DockItem item)
		{
			foreach (var it in items.ToArray ()) {
				if (it.DockItem == item) {
					it.UpdateTab ();
					break;
				}
			}
		}
		
		internal void UpdateStyle (DockItem item)
		{
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width <= 0 || height <= 0)
				return;

			snapshot.DrawCairo (this, ctx => {
				ctx.Rectangle (0, 0, width, height);
				ctx.SetSourceColor (Styles.DockBarBackground.ToCairoColor ());
				ctx.Fill ();
			});

			base.OnSnapshot (snapshot);

			if (ShowBorder) {
				snapshot.DrawCairo (this, ctx => {
					ctx.LineWidth = 1;

					// Dark separator
					switch (Position) {
					case PositionType.Left: ctx.MoveTo (width - 0.5, 0); ctx.RelLineTo (0, height); break;
					case PositionType.Right: ctx.MoveTo (0.5, 0); ctx.RelLineTo (0, height); break;
					case PositionType.Top: ctx.MoveTo (0, height - 0.5); ctx.RelLineTo (width, 0); break;
					case PositionType.Bottom: ctx.MoveTo (0, 0.5); ctx.RelLineTo (width, 0); break;
					}
					ctx.SetSourceColor (Styles.DockSeparatorColor.ToCairoColor ());
					ctx.Stroke ();
				});
			}
		}
	}
}

