//
// SearchPopupWindow.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using MonoDevelop.Ide.Gui;
using Xwt.Motion;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Themed popup (GTK 2: a Popup <c>Gtk.Window</c> moved next to its target). GTK 4 cannot position toplevels, so this
	/// is a <see cref="Gtk.Popover"/> anchored to the target widget; the popover's own chrome is removed (CSS class
	/// <c>md-themed-popover</c>) and the <see cref="PopoverWindowTheme"/> border, arrow, pager and shadow are drawn by the
	/// content area. GTK places (and flips) the popover; the arrow side/offset are read back from the popup surface.
	/// </summary>
	public class PopoverWindow : Gtk.Popover, IAnimatable
	{
		PopoverWindowTheme theme;

		PopupPosition position;
		readonly ContentArea alignment;

		Gdk.Rectangle currentCaret;
		Gtk.Widget anchor;
		bool eventProvided;

		const int MinArrowSpacing = 5;

		public PopoverWindow ()
		{
			HasArrow = false;
			Autohide = false;
			ThemedPopoverStyle.Apply (this);

			alignment = new ContentArea (this);
			Child = alignment;

			UpdatePadding ();
		}

		public PopoverWindowTheme Theme {
			get {
				if (theme == null) {
					theme = new PopoverWindowTheme ();
					theme.RedrawNeeded += OnRedrawNeeded;
				}
				return theme;
			}
			set {
				if (theme == value)
					return;

				if (theme != null)
					theme.RedrawNeeded -= OnRedrawNeeded;
				theme = value;
				if (theme != null)
					theme.RedrawNeeded += OnRedrawNeeded;

				UpdatePadding ();
				QueueDraw ();
			}
		}

		public int CaretSpacing { get; set; }

		public bool ShowArrow {
			get { return Theme.ShowArrow; }
			set { Theme.ShowArrow = value; }
		}

		/// <summary>Content container: set its <c>Child</c> (GTK 2: <c>Gtk.Alignment.Add</c>).</summary>
		public BinWidget ContentBox {
			get { return alignment; }
		}

		public void ShowPopup (Gtk.Widget widget, PopupPosition position)
		{
			ShowPopup (widget, null, Gdk.Rectangle.Zero, position);
		}

		/// <summary>Shows the popup pointing at the click position (<paramref name="evt"/> coordinates are relative to the gesture widget).</summary>
		public void ShowPopup (Gtk.Widget widget, ClickInfo evt, PopupPosition position)
		{
			ShowPopup (widget, evt, Gdk.Rectangle.Zero, position);
		}

		public void ShowPopup (Gtk.Widget widget, Gdk.Rectangle caret, PopupPosition position)
		{
			ShowPopup (widget, null, caret, position);
		}

		/// <summary>
		/// GTK 4 has no global screen coordinates: <paramref name="onScreenArea"/> is interpreted in the coordinates
		/// of the IDE main window.
		/// </summary>
		public void ShowPopup (Xwt.Rectangle onScreenArea, PopupPosition position)
		{
			Gtk.Window root = IdeApp.Workbench.RootWindow;
			ShowPopup (root, null, onScreenArea.ToGdkRectangle (), position);
		}

		internal void ShowPopup (Xwt.Widget widget, Xwt.Rectangle caret, PopupPosition position)
		{
			if (widget?.Surface?.NativeWidget is Gtk.Widget native) {
				ShowPopup (native, null, caret.ToGdkRectangle (), position);
				return;
			}
			// Not a GTK widget: anchor to the main window at the widget's bounds.
			var bounds = GtkUtil.GetSceenBounds (widget);
			ShowPopup (new Xwt.Rectangle (bounds.X + caret.X, bounds.Y + caret.Y, caret.Width, caret.Height), position);
		}

		void ShowPopup (Gtk.Widget parent, ClickInfo evt, Gdk.Rectangle caret, PopupPosition position)
		{
			if (parent == null)
				throw new ArgumentNullException (nameof (parent));
			this.anchor = parent;
			this.currentCaret = caret;
			Theme.TargetPosition = position;
			eventProvided = evt != null;
			if (evt != null) {
				var owner = evt.Gesture?.Widget;
				var r = new Gdk.Rectangle ((int)evt.X, (int)evt.Y, 1, 1);
				currentCaret = owner != null ? owner.TranslateRectangle (parent, r) : r;
			}
			RepositionWindow ();
		}

		bool HasParent {
			get { return anchor != null; }
		}

		void IAnimatable.BatchBegin () { }
		void IAnimatable.BatchCommit () { QueueDraw (); }

		/// <summary>GTK 4: the popover surface is resized by GTK; the size is no longer animated.</summary>
		public void AnimatedResize ()
		{
			QueueResize ();
		}

		protected bool IsDestroyed { get; private set; }

		/// <summary>Called once from <see cref="Destroy"/> (replaces the GTK 2 ::destroy handler).</summary>
		protected virtual void OnDestroyed ()
		{
		}

		/// <summary>Pops the popover down and detaches it from its anchor.</summary>
		public override void Destroy ()
		{
			if (IsDestroyed || Handle == IntPtr.Zero)
				return;
			IsDestroyed = true;
			this.AbortAnimation ("Resize");
			OnDestroyed ();
			if (Visible)
				Popdown ();
			base.Destroy ();
			anchor = null;
		}

		/// <summary>
		/// Gets or sets the maximum Y top bound. The popover window will be placed below this bound.
		/// 0 means it's not set. Default value: 0. GTK 4 places popovers itself: kept for API compatibility only.
		/// </summary>
		public int MaximumYTopBound {
			get;
			set;
		}

		protected PopupPosition CurrentPosition { get { return position; }}

		public virtual void RepositionWindow (Gdk.Rectangle? newCaret = null)
		{
			if (!HasParent || IsDestroyed)
				return;

			if (newCaret.HasValue) {//Update caret if parameter is given
				currentCaret = newCaret.Value;
			}
			Gdk.Rectangle caret = currentCaret;
			if (!eventProvided && caret.Equals (Gdk.Rectangle.Zero))
				caret = new Gdk.Rectangle (0, 0, anchor.Width, anchor.Height);

			caret.Inflate (CaretSpacing, CaretSpacing);
			if (!Core.Platform.IsWindows && !eventProvided)
				caret.Inflate (-1, -1);

			position = Theme.TargetPosition;
			UpdatePadding ();
			Position = ToGtkPosition (position);

			// GTK centers the popover on the pointing rectangle: offset it for the left/right aligned positions.
			alignment.Measure (Gtk.Orientation.Horizontal, -1, out _, out int width, out _, out _);
			alignment.Measure (Gtk.Orientation.Vertical, width, out _, out int height, out _, out _);
			int offsetX = 0, offsetY = 0;
			int arrowSpace = MinArrowSpacing + Theme.ArrowWidth / 2;
			if ((position & PopupPosition.Top) != 0 || (position & PopupPosition.Bottom) != 0) {
				if (((int)position & 0x10) != 0)
					offsetX = width / 2 - caret.Width / 2 - arrowSpace;
				else if (((int)position & 0x20) != 0)
					offsetX = caret.Width / 2 - width / 2 + arrowSpace;
			} else {
				if (((int)position & 0x10) != 0)
					offsetY = height / 2 - caret.Height / 2 - arrowSpace;
				else if (((int)position & 0x20) != 0)
					offsetY = caret.Height / 2 - height / 2 + arrowSpace;
			}
			SetOffset (offsetX, offsetY);
			bool vertical = (position & PopupPosition.Top) != 0 || (position & PopupPosition.Bottom) != 0;
			Theme.ArrowOffset = (vertical ? width : height) / 2;

			this.Reparent (anchor);
			this.ShowAt (caret);
		}

		/// <summary>Popover side for a <see cref="PopupPosition"/> (which names the side of the arrow, i.e. of the target).</summary>
		internal static Gtk.PositionType ToGtkPosition (PopupPosition position)
		{
			switch ((PopupPosition)((int)position & 0x0f)) {
			case PopupPosition.Bottom:
				return Gtk.PositionType.Top;
			case PopupPosition.Left:
				return Gtk.PositionType.Right;
			case PopupPosition.Right:
				return Gtk.PositionType.Left;
			default:
				return Gtk.PositionType.Bottom;
			}
		}

		/// <summary>True when translucent surfaces are available (GTK 2: RGBA colormap).</summary>
		public bool SupportsAlpha {
			get { return GtkUtil.ScreenSupportsARGB (); }
		}

		/// <summary>GTK 4 draws no native shadow for popovers stripped of their chrome; kept for API compatibility.</summary>
		public bool ShowWindowShadow {
			get;
			set;
		}

		Gdk.Surface layoutSurface;

		protected override void OnRealized ()
		{
			base.OnRealized ();
			layoutSurface = Surface;
			if (layoutSurface != null)
				layoutSurface.Layout += OnSurfaceLayout;
		}

		protected override void OnUnrealized ()
		{
			if (layoutSurface != null) {
				layoutSurface.Layout -= OnSurfaceLayout;
				layoutSurface = null;
			}
			base.OnUnrealized ();
		}

		void OnSurfaceLayout (object sender, Gdk.LayoutArgs args)
		{
			UpdatePlacement ();
		}

		/// <summary>Reads back the side GTK placed the popover on (it may flip it) and the target center for the arrow.</summary>
		void UpdatePlacement ()
		{
			if (!ShowArrow || anchor == null || IsDestroyed)
				return;
			bool vertical;
			if (ThemedPopoverStyle.TryGetPlacement (this, anchor, currentCaret, alignment, out var actualSide, out var arrowOffset)) {
				var side = (PopupPosition)(((int)position & 0xf0) | (int)actualSide);
				if (side != position) {
					position = side;
					UpdatePadding ();
				}
				vertical = (position & PopupPosition.Top) != 0 || (position & PopupPosition.Bottom) != 0;
				int size = vertical ? alignment.Width : alignment.Height;
				int min = MinArrowSpacing + Theme.ArrowWidth / 2;
				Theme.ArrowOffset = Math.Max (min, Math.Min (size - min, arrowOffset));
			} else {
				vertical = (position & PopupPosition.Top) != 0 || (position & PopupPosition.Bottom) != 0;
				Theme.ArrowOffset = (vertical ? alignment.Width : alignment.Height) / 2;
			}
			alignment.QueueDraw ();
		}

		void DrawChrome (Gtk.Snapshot snapshot, Action drawChildren)
		{
			var border = BorderAllocation;
			snapshot.DrawCairo (alignment, context => {
				if (!SupportsAlpha) {
					context.SetSourceRGB (1, 1, 1);
					context.Paint ();
				}
				context.Save ();
				Theme.SetBorderPath (context, border, position);
				context.Clip ();
				OnDrawContent (context); // Draw content first so we can easily clip it
				context.Restore ();
			});

			drawChildren ();

			snapshot.DrawCairo (alignment, context => {
				// protect against overriden methods which leave in a bad state
				context.Save ();
				if (Theme.DrawPager) {
					Theme.RenderPager (context,
					                   PangoContext,
					                   border);
				}

				Theme.RenderShadow (context, border, position);
				context.Restore ();
			});
		}

		/// <summary>Draws the background (already clipped to the border path); (0,0) is the content area origin.</summary>
		protected virtual void OnDrawContent (Cairo.Context context)
		{
			Theme.RenderBackground (context, new Gdk.Rectangle (0, 0, alignment.Width, alignment.Height));
		}

		void UpdatePadding ()
		{
			if (alignment == null)
				return;
			int top,left,bottom,right;
			top = left = bottom = right = Theme.Padding + (Core.Platform.IsWindows ? 1 : 2);

			if (ShowArrow) {
				if ((position & PopupPosition.Top) != 0)
					top += Theme.ArrowLength;
				else if ((position & PopupPosition.Bottom) != 0)
					bottom += Theme.ArrowLength;
				else if ((position & PopupPosition.Left) != 0)
					left += Theme.ArrowLength;
				else if ((position & PopupPosition.Right) != 0)
					right += Theme.ArrowLength;
			}
			var p = new Gtk.Border { Top = (short)top, Bottom = (short)bottom, Left = (short)left, Right = (short)right };
			if (!p.Equals (alignment.Padding))
				alignment.Padding = p;
		}

		void OnRedrawNeeded (object sender, EventArgs args)
		{
			UpdatePadding ();
			alignment?.QueueDraw ();
		}

		protected Gdk.Rectangle ChildAllocation {
			get {
				var rect = BorderAllocation;
				rect.Inflate (-Theme.Padding - 1, -Theme.Padding - 1);
				return rect;
			}
		}

		Gdk.Rectangle BorderAllocation {
			get {
				var rect = new Gdk.Rectangle (0, 0, alignment.Width, alignment.Height);
				if (ShowArrow) {
					if ((position & PopupPosition.Top) != 0) {
						rect.Y += Theme.ArrowLength;
						rect.Height -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Bottom) != 0) {
						rect.Height -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Left) != 0) {
						rect.X += Theme.ArrowLength;
						rect.Width -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Right) != 0) {
						rect.Width -= Theme.ArrowLength;
					}
				}
				if (!Core.Platform.IsWindows) {
					if ((position & PopupPosition.Top) != 0) {
						rect.Y += 1;
						rect.Height -= 1;
					}
					else if ((position & PopupPosition.Bottom) != 0) {
						rect.Height -= 1;
					}
					else if ((position & PopupPosition.Left) != 0) {
						rect.X += 1;
						rect.Width -= 1;
					}
					else if ((position & PopupPosition.Right) != 0) {
						rect.Width -= 1;
					}
				}
				return rect;
			}
		}

		public event EventHandler PagerLeftClicked;
		public event EventHandler PagerRightClicked;

		protected virtual void OnPagerLeftClicked ()
		{
			if (PagerLeftClicked != null)
				PagerLeftClicked (this, null);
		}

		protected virtual void OnPagerRightClicked ()
		{
			if (PagerRightClicked != null)
				PagerRightClicked (this, null);
		}

		void OnContentButtonReleased (ClickInfo evnt)
		{
			if (evnt.Button != 1 || !Theme.DrawPager)
				return;

			var point = new Gdk.Point ((int)evnt.X, (int)evnt.Y);
			if (Theme.HitTestPagerLeftArrow (PangoContext, BorderAllocation, point)) {
				evnt.Claim ();
				OnPagerLeftClicked ();
			} else if (Theme.HitTestPagerRightArrow (PangoContext, BorderAllocation, point)) {
				evnt.Claim ();
				OnPagerRightClicked ();
			}
		}

		/// <summary>Content area: padding for the themed border/arrow, draws the theme around the child.</summary>
		sealed class ContentArea : BinWidget
		{
			readonly PopoverWindow owner;

			public ContentArea (PopoverWindow owner)
			{
				this.owner = owner;
				this.AddClickHandler (null, owner.OnContentButtonReleased, 1);
			}

			protected override void OnSnapshot (Gtk.Snapshot snapshot)
			{
				owner.DrawChrome (snapshot, () => base.OnSnapshot (snapshot));
			}
		}
	}

	/// <summary>
	/// Shared GTK 4 support for popovers that draw their own themed chrome.
	/// </summary>
	static class ThemedPopoverStyle
	{
		public const string CssClass = "md-themed-popover";
		static bool installed;

		/// <summary>Removes the default popover background/border/shadow so the content can draw the themed chrome.</summary>
		public static void Apply (Gtk.Widget popover)
		{
			if (!installed) {
				var display = Gdk.Display.Default;
				if (display != null) {
					var provider = new Gtk.CssProvider ();
					provider.LoadFromString (
						"popover." + CssClass + " { background: none; box-shadow: none; padding: 0; margin: 0; } " +
						"popover." + CssClass + " > contents { background: none; border: none; box-shadow: none; padding: 0; margin: 0; border-radius: 0; min-width: 0; min-height: 0; }");
					Gtk.StyleContext.AddProviderForDisplay (display, provider, Gtk.StyleProviderPriority.Application);
					installed = true;
				}
			}
			popover.AddCssClass (CssClass);
		}

		/// <summary>
		/// Reads back where GTK placed <paramref name="popover"/>: the side of the target relative to the popover
		/// (<see cref="PopupPosition"/> semantics, i.e. the arrow side) and the target center along that side,
		/// in <paramref name="content"/> coordinates. False if the popover isn't mapped yet.
		/// </summary>
		public static bool TryGetPlacement (Gtk.Popover popover, Gtk.Widget anchor, Gdk.Rectangle pointingTo, Gtk.Widget content, out PopupPosition side, out int arrowOffset)
		{
			side = PopupPosition.Top;
			arrowOffset = 0;
			try {
				var surface = popover.Surface;
				if (surface == null || anchor == null || !popover.IsMapped)
					return false;
				var popup = Gdk.PopupAdapter.GetObject (surface);
				if (popup == null)
					return false;

				switch (popup.SurfaceAnchor) {
				case Gdk.Gravity.North:
				case Gdk.Gravity.NorthWest:
				case Gdk.Gravity.NorthEast:
					side = PopupPosition.Top;
					break;
				case Gdk.Gravity.South:
				case Gdk.Gravity.SouthWest:
				case Gdk.Gravity.SouthEast:
					side = PopupPosition.Bottom;
					break;
				case Gdk.Gravity.West:
					side = PopupPosition.Left;
					break;
				case Gdk.Gravity.East:
					side = PopupPosition.Right;
					break;
				default:
					return false;
				}

				// Target center: anchor -> its native (window/popover) -> parent surface coordinates.
				Gtk.Widget native = anchor;
				while (native != null && !(native is Gtk.Window) && !(native is Gtk.Popover))
					native = native.Parent;
				if (native == null)
					return false;
				double cx = pointingTo.X + pointingTo.Width / 2.0;
				double cy = pointingTo.Y + pointingTo.Height / 2.0;
				if (!anchor.TranslateCoordinates (native, cx, cy, out double nx, out double ny))
					return false;
				double tx, ty;
				if (native is Gtk.Window w)
					w.GetSurfaceTransform (out tx, out ty);
				else
					((Gtk.Popover)native).GetSurfaceTransform (out tx, out ty);
				// -> popover surface -> popover widget -> content.
				popover.GetSurfaceTransform (out double ptx, out double pty);
				double px = nx + tx - popup.PositionX - ptx;
				double py = ny + ty - popup.PositionY - pty;
				if (!popover.TranslateCoordinates (content, px, py, out double x, out double y))
					return false;
				arrowOffset = (int)Math.Round (side == PopupPosition.Top || side == PopupPosition.Bottom ? x : y);
				return true;
			} catch (Exception) {
				return false;
			}
		}
	}
}
