//
// DockBarItem.cs
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
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using Xwt.Motion;
using Animations = Xwt.Motion.AnimationExtensions;
using MonoDevelop.Core;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Ide;
using Gdk;

namespace MonoDevelop.Components.Docking
{
	class CrossfadeIcon: CairoWidget, IAnimatable
	{
		Xwt.Drawing.Image primary, secondary;

		double secondaryOpacity;

		public CrossfadeIcon (Xwt.Drawing.Image primary, Xwt.Drawing.Image secondary)
		{
			if (primary == null)
				throw new ArgumentNullException ("primary");
			if (secondary == null)
				throw new ArgumentNullException ("secondary");

			this.primary = primary;
			this.secondary = secondary;
		}

		void IAnimatable.BatchBegin () { }
		void IAnimatable.BatchCommit () { QueueDraw (); }

		public void ShowPrimary ()
		{
			AnimateCrossfade (false);
		}

		public void ShowSecondary ()
		{
			AnimateCrossfade (true);
		}

		void AnimateCrossfade (bool toSecondary)
		{
			this.Animate ("CrossfadeIconSwap",
			              x => secondaryOpacity = x,
			              secondaryOpacity,
			              toSecondary ? 1.0f : 0.0f);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				this.AbortAnimation ("CrossfadeIconSwap");
			base.Dispose (disposing);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = (int)(orientation == Gtk.Orientation.Horizontal ? primary.Width : primary.Height);
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnDraw (Cairo.Context context, int width, int height)
		{
			if (secondaryOpacity < 1.0f)
				RenderIcon (context, width, height, primary, 1.0f - (float)Math.Pow (secondaryOpacity, 3.0f));

			if (secondaryOpacity > 0.0f)
				RenderIcon (context, width, height, secondary, secondaryOpacity);
		}

		void RenderIcon (Cairo.Context context, int width, int height, Xwt.Drawing.Image surface, double opacity)
		{
			context.DrawImage (this, surface.WithAlpha (opacity),
			                          (width - surface.Width) / 2,
			                          (height - surface.Height) / 2);
		}
	}

	/// <summary>
	/// Single line markup label for dock bars, optionally rotated 90 degrees counter-clockwise
	/// (GTK 4 labels can't be rotated).
	/// </summary>
	class DockBarLabel: CairoWidget
	{
		readonly string markup;
		readonly bool vertical;
		readonly Pango.FontDescription font;

		public DockBarLabel (string markup, bool vertical, Pango.FontDescription font)
		{
			this.markup = markup;
			this.vertical = vertical;
			this.font = font;
		}

		Pango.Layout CreateLayout ()
		{
			var la = CreatePangoLayout (null);
			la.SetMarkup (markup);
			la.FontDescription = font;
			return la;
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			int w, h;
			using (var la = CreateLayout ())
				la.GetPixelSize (out w, out h);
			bool useWidth = (orientation == Gtk.Orientation.Horizontal) != vertical;
			minimum = natural = useWidth ? w : h;
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnDraw (Cairo.Context cr, int width, int height)
		{
			using (var la = CreateLayout ()) {
				la.GetPixelSize (out int w, out int h);
				cr.SetSourceColor (Styles.DockBarLabelColor.ToCairoColor ());
				if (vertical) {
					// Text reads bottom to top, aligned to the start of the bar
					cr.MoveTo ((width - h) / 2d, height);
					cr.Rotate (-Math.PI / 2);
				} else {
					cr.MoveTo (0, (height - h) / 2d);
				}
				Pango.CairoHelper.ShowLayout (cr, la);
			}
		}
	}

	class DockBarItem: BinWidget, IAnimatable
	{
		DockBar bar;
		DockItem it;
		Box box;
		DockBarLabel label;
		AutoHideBox autoShowFrame;
		AutoHideBox hiddenFrame;
		uint autoShowTimeout = uint.MaxValue;
		uint autoHideTimeout = uint.MaxValue;
		int size;
		Gdk.Size lastFrameSize;
		MouseTracker tracker;
		CrossfadeIcon crossfade;
		double hoverProgress;

		public DockBarItem (DockBar bar, DockItem it, int size)
		{
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += OnPerformPress;

			this.size = size;
			this.bar = bar;
			this.it = it;
			CanFocus = true;
			Focusable = true;
			UpdateTab ();
			lastFrameSize = new Gdk.Size (bar.Frame.Width, bar.Frame.Height);
			bar.Frame.FrameSizeAllocated += HandleBarFrameSizeAllocated;

			tracker = new MouseTracker (this);
			tracker.TrackMotion = false;
			tracker.HoveredChanged += (sender, e) => {

				if (crossfade == null)
					return;

				AnimateHover (tracker.Hovered);
				if (tracker.Hovered)
					crossfade.ShowSecondary ();
				else
					crossfade.ShowPrimary ();
			};

			Styles.Changed += UpdateStyle;

			this.AddMotionHandler (
				motion: (x, y) => {
					ScheduleAutoHide (true);
					ScheduleAutoShowOnHover ();
				},
				enter: (x, y) => ScheduleAutoShowOnHover (),
				leave: () => {
					ScheduleAutoHide (true);
					if (autoShowFrame == null) {
						QueueDraw ();
					}
				});
			this.AddClickHandler (OnButtonPress, OnButtonRelease);
			this.AddFocusHandler (QueueDraw, QueueDraw);
			this.AddKeyPressHandler ((keyval, keycode, state) => {
				var key = (Gdk.Key)keyval;
				if (key == Gdk.Key.Return || key == Gdk.Key.KP_Enter || key == Gdk.Key.space) {
					AutoShow ();
					return true;
				}
				return false;
			});

			this.SetCommonAttributes ("DockbarItem", null, null);
			AccessibleRole = Gtk.AccessibleRole.Button;
		}

		void IAnimatable.BatchBegin () { }
		void IAnimatable.BatchCommit () { QueueDraw (); }

		void AnimateHover (bool hovered)
		{
			this.Animate ("Hover",
			              x => hoverProgress = x,
			              hoverProgress,
			              hovered ? 1.0f : 0.0f,
			              length: 100);
		}

		void HandleBarFrameSizeAllocated (object o, EventArgs args)
		{
			var frameSize = new Gdk.Size (bar.Frame.Width, bar.Frame.Height);
			if (!lastFrameSize.Equals (frameSize)) {
				lastFrameSize = frameSize;
				// The widget tree can't be modified while the frame is being allocated
				GLib.Idle.Add (() => {
					if (Handle == IntPtr.Zero)
						return false;
					if (autoShowFrame != null)
						bar.Frame.UpdateSize (bar, autoShowFrame);

					UnscheduleAutoHide ();
					AutoHide (false);
					return false;
				});
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				this.AbortAnimation ("Hover");
				bar.Frame.FrameSizeAllocated -= HandleBarFrameSizeAllocated;
				Ide.Gui.Styles.Changed -= UpdateStyle;
			}
			base.Dispose (disposing);
		}


		public void Close ()
		{
			UnscheduleAutoShow ();
			UnscheduleAutoHide ();
			AutoHide (false);
			bar.RemoveItem (this);
			Destroy ();
		}

		public int Size {
			get { return size; }
			set { size = value; }
		}

		public void UpdateTab ()
		{
			Child = null;

			if (bar.Orientation == Gtk.Orientation.Horizontal) {
				box = new Box (Gtk.Orientation.Horizontal, 2);
				if (bar.AlignToEnd)
					Padding = new Gtk.Border { Top = 5, Bottom = 5, Left = 11, Right = 9 };
				else
					Padding = new Gtk.Border { Top = 5, Bottom = 5, Left = 9, Right = 11 };
			}
			else {
				box = new Box (Gtk.Orientation.Vertical, 2);
				if (bar.AlignToEnd)
					Padding = new Gtk.Border { Top = 11, Bottom = 9, Left = 5, Right = 5 };
				else
					Padding = new Gtk.Border { Top = 9, Bottom = 11, Left = 5, Right = 5 };
			}
			box.SetShouldIgnore (true);

			if (it.Icon != null) {
				var desat = it.Icon.WithAlpha (0.5);
				crossfade = new CrossfadeIcon (desat, it.Icon);
				crossfade.SetShouldIgnore (true);
				box.Append (crossfade);
				desat.Dispose ();
			}

			if (!string.IsNullOrEmpty (it.Label)) {
				label = new DockBarLabel (it.Label, bar.Orientation == Gtk.Orientation.Vertical,
					IdeServices.FontService.SansFont.CopyModified (Styles.FontScale11));
				label.SetShouldIgnore (true);
				label.Hexpand = label.Vexpand = true;
				box.Append (label);

				this.SetLabel (it.Label);
				this.SetTitle (it.Label);
				this.SetCommonAttributes (null, null, GettextCatalog.GetString ("Show the {0} pad", it.Label));
			} else
				label = null;

			Child = box;
			UpdateStyle (this, null);
			QueueDraw ();
		}

		void UpdateStyle (object sender, EventArgs e)
		{
			label?.QueueDraw ();
		}

		public MonoDevelop.Components.Docking.DockItem DockItem {
			get {
				return it;
			}
		}

		protected override void OnHidden ()
		{
			base.OnHidden ();
			UnscheduleAutoShow ();
			UnscheduleAutoHide ();
			AutoHide (false);
		}

		public void Present (bool giveFocus)
		{
			AutoShow ();
			if (giveFocus) {
				GLib.Timeout.Add (200, delegate {
					// Using a small delay because AutoShow uses an animation and setting focus may
					// not work until the item is visible
					it.SetFocus ();
					ScheduleAutoHide (false);
					return false;
				});
			}
		}

		public void Minimize ()
		{
			AutoHide (false);
		}

		void AutoShow ()
		{
			UnscheduleAutoHide ();
			if (autoShowFrame == null && !bar.Frame.OverlayWidgetVisible) {
				if (hiddenFrame != null)
					bar.Frame.AutoHide (it, hiddenFrame, false);
				autoShowFrame = bar.Frame.AutoShow (it, bar, size);
				autoShowFrame.PointerEntered += OnFrameEnter;
				autoShowFrame.PointerLeft += OnFrameLeave;
				autoShowFrame.EscapePressed += OnFrameEscape;
				QueueDraw ();
			}
		}

		void AutoHide (bool animate)
		{
			UnscheduleAutoShow ();
			if (autoShowFrame != null) {
				size = autoShowFrame.PadSize;
				hiddenFrame = autoShowFrame;
				autoShowFrame.Hidden += delegate {
					hiddenFrame = null;
				};
				bar.Frame.AutoHide (it, autoShowFrame, animate);
				autoShowFrame.PointerEntered -= OnFrameEnter;
				autoShowFrame.PointerLeft -= OnFrameLeave;
				autoShowFrame.EscapePressed -= OnFrameEscape;
				autoShowFrame = null;
				QueueDraw ();
			}
		}

		void ScheduleAutoShow ()
		{
			UnscheduleAutoHide ();
			if (autoShowTimeout == uint.MaxValue) {
				autoShowTimeout = GLib.Timeout.Add (bar.Frame.AutoShowDelay, delegate {
					autoShowTimeout = uint.MaxValue;

					// Check is mouse over - it may be that the LeaveEvent wasn't fired becaues of a gtk bug. So we double check as a work around.
					bool isMouseOver = IsPointerOver (this);
					if (!isMouseOver)
						return false;
					AutoShow ();
					return false;
				});
			}
		}

		static bool IsPointerOver (Gtk.Widget widget)
		{
			if (!widget.TryGetPointer (out double x, out double y))
				return false;
			return x >= 0 && y >= 0 && x <= widget.Width && y <= widget.Height;
		}

		void ScheduleAutoHide (bool cancelAutoShow)
		{
			ScheduleAutoHide (cancelAutoShow, false);
		}

		void ScheduleAutoHide (bool cancelAutoShow, bool force)
		{
			if (cancelAutoShow)
				UnscheduleAutoShow ();
			if (force) {
				// Remove the focus from the pad
				var window = it.Widget.GetToplevelWindow ();
				if (window != null && window.FocusWidget != null && window.FocusWidget.IsChildOf (it.Widget))
					window.FocusWidget = null;
			}
			if (autoHideTimeout == uint.MaxValue) {
				autoHideTimeout = GLib.Timeout.Add (force ? 0 : bar.Frame.AutoHideDelay, delegate {
					// Don't hide if the context menu for the item is being shown.
					if (it.ShowingContextMenu)
						return true;
					if (!force) {
						// Don't hide the item if it has the focus. Try again later.
#if MAC
						bool hasTopLevel = it.Widget.FocusChild != null && autoShowFrame != null &&
						autoShowFrame.GetToplevelWindow () is IdeWindow ideWindow && (
						ideWindow.IsActive || Mac.GtkMacInterop.GetGtkWindow (AppKit.NSApplication.SharedApplication.KeyWindow) == ideWindow);

						if (hasTopLevel)
							return true;
#else
						if (it.Widget.FocusChild != null && autoShowFrame != null && autoShowFrame.GetToplevelWindow ()?.IsActive == true)
							return true;
#endif
						// Don't hide the item if the mouse pointer is still inside the window. Try again later.
						if (it.Widget.Visible && it.Widget.IsRealized && IsPointerOver (it.Widget))
							return true;
						// Don't hide if the mouse pointer is still inside the DockBar item
						if (IsPointerOver (this))
							return true;
					}
					autoHideTimeout = uint.MaxValue;
					AutoHide (true);
					return false;
				});
			}
		}

		void UnscheduleAutoShow ()
		{
			if (autoShowTimeout != uint.MaxValue) {
				GLib.Source.Remove (autoShowTimeout);
				autoShowTimeout = uint.MaxValue;
			}
		}

		void UnscheduleAutoHide ()
		{
			if (autoHideTimeout != uint.MaxValue) {
				GLib.Source.Remove (autoHideTimeout);
				autoHideTimeout = uint.MaxValue;
			}
		}

		void ScheduleAutoShowOnHover ()
		{
			if (bar.HoverActivationEnabled && autoShowFrame == null) {
				ScheduleAutoShow ();
				QueueDraw ();
			}
		}

		void OnPerformPress (object sender, EventArgs args)
		{
			if (autoShowFrame == null) {
				AutoShow ();
			} else {
				AutoHide (false);
			}
		}

		void OnFrameEnter (object s, EventArgs args)
		{
			AutoShow ();
		}

		void OnFrameEscape (object s, EventArgs args)
		{
			ScheduleAutoHide (true, true);
		}

		void OnFrameLeave (object s, EventArgs args)
		{
			ScheduleAutoHide (true);
		}

		bool itemActivated;

		void OnButtonPress (ClickInfo evnt)
		{
			if (bar.Frame.OverlayWidgetVisible)
				return;
			if (evnt.TriggersContextMenu ()) {
				it.ShowDockPopupMenu (this, evnt);
			} else if (evnt.Button == 1) {
				if (evnt.NPress >= 2) {
					// Instead of changing the state of the pad here, do it when the button is released.
					// Changing the state will make this bar item to vanish before the ReleaseEvent is received, and in this
					// case the ReleaseEvent may be fired on another widget that is taking the space of this bar item.
					// This was happening for example with the feedback button.
					itemActivated = true;
				} else {
					AutoShow ();
					it.Present (true);
				}
			}
			evnt.Claim ();
		}

		void OnButtonRelease (ClickInfo evnt)
		{
			if (itemActivated) {
				itemActivated = false;
				it.Status = DockItemStatus.Dockable;
			}
			evnt.Claim ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0) {
				snapshot.DrawCairo (this, context => {
					// TODO: VV: Remove preflight gradient features and replace with a flat color

					Cairo.LinearGradient lg;

					if (bar.Orientation == Gtk.Orientation.Horizontal) {
						lg = new Cairo.LinearGradient (0, 0, width, 0);
					} else {
						lg = new Cairo.LinearGradient (0, 0, 0, height);
					}

					using (lg) {
						Cairo.Color primaryColor = Styles.DockBarPrelightColor.ToCairoColor ();
						primaryColor.A = hoverProgress;

						Cairo.Color transparent = primaryColor;
						transparent.A = 0;

						lg.AddColorStop (0.0, transparent);
						lg.AddColorStop (0.35, primaryColor);
						lg.AddColorStop (0.65, primaryColor);
						lg.AddColorStop (1.0, transparent);

						context.Rectangle (0, 0, width, height);
						context.SetSource (lg);
					}
					context.Fill ();

					if (HasFocus)
						DockUtil.DrawFocusRectangle (context, new Gdk.Rectangle (2, 2, width - 4, height - 4), this.GetForegroundColor ());
				});
			}
			base.OnSnapshot (snapshot);
		}
	}
}
