//
// AutoHideBox.cs
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
using Gtk;
using Gdk;
using MonoDevelop.Core;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components.Docking
{
	class AutoHideBox: DockFrameTopLevel
	{
		bool resizing;
		int resizePos;
		int origSize;
		int origPos;
		bool horiz;
		bool startPos;
		DockFrame frame;
		Gtk.PositionType position;
		bool disposed;
		bool insideGrip;
		GripWidget grip;

		int gripSize = 8;

		/// <summary>Raised when the pointer enters the pad.</summary>
		public event EventHandler PointerEntered;

		/// <summary>Raised when the pointer leaves the pad.</summary>
		public event EventHandler PointerLeft;

		/// <summary>Raised when Escape is pressed while the focus is inside the pad.</summary>
		public event EventHandler EscapePressed;

		public AutoHideBox (DockFrame frame, DockItem item, Gtk.PositionType pos, int size): base (frame)
		{
			this.position = pos;
			this.frame = frame;
			horiz = pos == PositionType.Left || pos == PositionType.Right;
			startPos = pos == PositionType.Top || pos == PositionType.Left;

			Box fr;
			CustomFrame cframe = new CustomFrame ();
			cframe.SetShouldIgnore (true);

			switch (pos) {
			case PositionType.Left: cframe.SetMargins (0, 0, 1, 1); break;
			case PositionType.Right: cframe.SetMargins (0, 0, 1, 1); break;
			case PositionType.Top: cframe.SetMargins (1, 1, 0, 0); break;
			case PositionType.Bottom: cframe.SetMargins (1, 1, 0, 0); break;
			}

			grip = new GripWidget (this);

			// FIXME How to actually resize this?
			grip.SetRole (AtkCocoa.Roles.AXSplitter, GettextCatalog.GetString ("Pad resize handle"));
			grip.SetLabel (GettextCatalog.GetString ("Pad resize handle"));

			cframe.Child = grip;

			if (horiz) {
				fr = new Box (Orientation.Horizontal, 0);
				grip.CursorFromName = "col-resize";
				grip.WidthRequest = gripSize;
				cframe.Vexpand = true;
			} else {
				fr = new Box (Orientation.Vertical, 0);
				grip.CursorFromName = "row-resize";
				grip.HeightRequest = gripSize;
				cframe.Hexpand = true;
			}
			fr.SetShouldIgnore (true);

			Child = fr;

			var itemBox = new Box (Orientation.Vertical, 0);
			itemBox.SetShouldIgnore (true);
			itemBox.Hexpand = itemBox.Vexpand = true;

			DockUtil.RemoveFromParent (item.TitleTab);
			DockUtil.RemoveFromParent (item.Widget);
			item.TitleTab.Active = true;
			itemBox.Append (item.TitleTab);

			item.Widget.SetShouldIgnore (true);
			item.Widget.Vexpand = true;
			itemBox.Append (item.Widget);

			item.Widget.Show ();

			if (pos == PositionType.Left || pos == PositionType.Top) {
				fr.Append (itemBox);
				fr.Append (cframe);
			} else {
				fr.Append (cframe);
				fr.Append (itemBox);
			}

			// A click inside the pad must not reach the frame, whose handler hides all visible auto-hide pads
			this.AddClickHandler (c => c.Claim ());
			this.AddMotionHandler (null,
				enter: (x, y) => PointerEntered?.Invoke (this, EventArgs.Empty),
				leave: () => PointerLeft?.Invoke (this, EventArgs.Empty));
			this.AddKeyPressHandler ((keyval, keycode, state) => {
				if ((Gdk.Key)keyval == Gdk.Key.Escape)
					EscapePressed?.Invoke (this, EventArgs.Empty);
				return false;
			}, phase: PropagationPhase.Capture);

			Visible = false;

			grip.AddDragHandler (OnSizeDragBegin, OnSizeDragUpdate, OnSizeDragEnd);
			grip.AddMotionHandler (null,
				enter: (x, y) => { insideGrip = true; grip.QueueDraw (); },
				leave: () => { insideGrip = false; grip.QueueDraw (); });
		}

		public bool Disposed {
			get { return disposed; }
			set { disposed = value; }
		}

		public void AnimateShow ()
		{
			Show ();
		}

		public void AnimateHide ()
		{
			Hide ();
		}

		public int PadSize {
			get {
				return horiz ? RequestedWidth : RequestedHeight;
			}
		}

		// Position of the pointer along the resize axis, relative to the frame
		double GetFramePointer (double gripX, double gripY)
		{
			if (!grip.TranslateCoordinates (frame, gripX, gripY, out double fx, out double fy))
				return 0;
			return horiz ? fx : fy;
		}

		double dragStartX, dragStartY;

		void OnSizeDragBegin (double x, double y)
		{
			dragStartX = x;
			dragStartY = y;
			resizePos = (int)GetFramePointer (x, y);
			origSize = horiz ? RequestedWidth : RequestedHeight;
			if (!startPos)
				origPos = (horiz ? X : Y) + origSize;
			resizing = true;
		}

		void OnSizeDragEnd (double offsetX, double offsetY)
		{
			resizing = false;
		}

		void OnSizeDragUpdate (double offsetX, double offsetY)
		{
			if (!resizing)
				return;

			int newPos = (int)GetFramePointer (dragStartX + offsetX, dragStartY + offsetY);
			int diff = startPos ? (newPos - resizePos) : (resizePos - newPos);
			int newSize = origSize + diff;
			var req = Child.GetRequisition ();
			if (horiz) {
				if (newSize < req.Width)
					newSize = req.Width;
				if (!startPos)
					X = origPos - newSize;
				RequestedWidth = newSize;
			} else {
				if (newSize < req.Height)
					newSize = req.Height;
				if (!startPos)
					Y = origPos - newSize;
				RequestedHeight = newSize;
			}
			frame.QueueResize ();
		}

		bool IsGripHovered => insideGrip;

		/// <summary>Resize handle of the pad.</summary>
		sealed class GripWidget: CairoWidget
		{
			readonly AutoHideBox owner;

			public GripWidget (AutoHideBox owner)
			{
				this.owner = owner;
			}

			protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
			{
				minimum = natural = 0;
				minimum_baseline = natural_baseline = -1;
			}

			protected override void OnDraw (Cairo.Context ctx, int width, int height)
			{
				var color = owner.IsGripHovered ? Ide.Gui.Styles.DockBarPrelightColor : Ide.Gui.Styles.DockBarBackground;
				ctx.SetSourceColor (color.ToCairoColor ());
				ctx.Paint ();
			}
		}
	}
}
