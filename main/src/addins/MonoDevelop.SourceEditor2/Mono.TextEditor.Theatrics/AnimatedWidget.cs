//
// AnimatedVboxActor.cs
//
// Authors:
//   Scott Peterson <lunchtimemama@gmail.com>
//
// Copyright (C) 2008 Scott Peterson
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
using Gdk;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;

namespace Mono.TextEditor.Theatrics
{
	internal enum AnimationState
	{
		Coming,
		Idle,
		IntendingToGo,
		Going
	}

	/// <summary>
	/// Hosts a widget and grows/shrinks the space it occupies according to the animation <see cref="Percent"/>.
	/// </summary>
	internal class AnimatedWidget : ContainerWidget
	{
		public event EventHandler WidgetDestroyed;

		public Gtk.Widget Widget;
		public Easing Easing;
		public Blocking Blocking;
		public AnimationState AnimationState;
		public uint Duration;
		public double Bias = 1.0;
		public int StartPadding;
		public int EndPadding;

		public LinkedListNode<AnimatedWidget> Node;

		private readonly bool horizontal;
		private double percent;
		private Gdk.Rectangle widget_alloc;
		private int animatedWidth;
		private int animatedHeight;

		public AnimatedWidget (Gtk.Widget widget, uint duration, Easing easing, Blocking blocking, bool horizontal)
		{
			this.horizontal = horizontal;
			Widget = widget;
			Duration = duration;
			Easing = easing;
			Blocking = blocking;
			AnimationState = AnimationState.Coming;
			Overflow = Overflow.Hidden;

			AddChild (widget);
			// GTK 4 widgets have no Destroyed event: a widget leaving this container (Destroy () unparents it) counts as destroyed.
			widget.AddNotification ("parent", OnWidgetParentChanged);
		}

		/// <summary>The width the animated widget currently requests.</summary>
		public int AnimatedWidth => animatedWidth;

		/// <summary>The height the animated widget currently requests.</summary>
		public int AnimatedHeight => animatedHeight;

		public double Percent {
			get { return percent; }
			set {
				percent = value * Bias;
				QueueResize ();
			}
		}

		private void OnWidgetParentChanged (object sender, GLib.NotifyArgs args)
		{
			var widget = Widget;
			if (widget == null || widget.Parent == this)
				return;
			widget.RemoveNotification ("parent", OnWidgetParentChanged);
			Widget = null;
			if (AnimationState != AnimationState.Going)
				WidgetDestroyed?.Invoke (this, EventArgs.Empty);
		}

		#region Overrides

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum_baseline = natural_baseline = -1;
			if (Widget != null) {
				var size = GetChildNaturalSize (Widget);
				widget_alloc.Width = size.Width;
				widget_alloc.Height = size.Height;
			}

			if (horizontal) {
				animatedWidth = Choreographer.PixelCompose (percent, widget_alloc.Width + StartPadding + EndPadding, Easing);
				animatedHeight = widget_alloc.Height;
			} else {
				animatedWidth = widget_alloc.Width;
				animatedHeight = Choreographer.PixelCompose (percent, widget_alloc.Height + StartPadding + EndPadding, Easing);
			}

			minimum = natural = orientation == Gtk.Orientation.Horizontal ? animatedWidth : animatedHeight;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			if (Widget != null) {
				if (horizontal) {
					widget_alloc.Height = height;
					widget_alloc.X = StartPadding;
					if (Blocking == Blocking.Downstage) {
						widget_alloc.X += width - widget_alloc.Width;
					}
				} else {
					widget_alloc.Width = width;
					widget_alloc.Y = StartPadding;
					if (Blocking == Blocking.Downstage) {
						widget_alloc.Y = height - widget_alloc.Height;
					}
				}
				
				if (widget_alloc.Height > 0 && widget_alloc.Width > 0) {
					AllocateChild (Widget, widget_alloc);
				}
			}
		}

		#endregion
		
	}
}
