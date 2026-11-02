//
// GtkGestures.cs
//
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
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

namespace MonoDevelop.Components
{
	/// <summary>
	/// Touchpad gestures. GTK 2 used Quartz-only GDK events; GTK 4 provides portable gesture controllers
	/// (<see cref="Gtk.GestureZoom"/>, <see cref="Gtk.GestureRotate"/>, <see cref="Gtk.GestureSwipe"/>).
	/// </summary>
	public static class GtkGestures
	{
		public static bool IsSupported { get { return true; } }

		/// <summary>Raises <paramref name="handler"/> with the incremental magnification (scale ratio - 1) of a pinch.</summary>
		public static void AddGestureMagnifyHandler (this Gtk.Widget widget, EventHandler<GestureMagnifyEventArgs> handler)
		{
			if (widget == null)
				throw new ArgumentNullException (nameof (widget));
			var gesture = new Gtk.GestureZoom ();
			double lastScale = 1;
			gesture.Begin += (o, a) => lastScale = 1;
			gesture.ScaleChanged += (o, a) => {
				if (lastScale <= 0 || a.Scale <= 0)
					return;
				var magnification = a.Scale / lastScale - 1;
				lastScale = a.Scale;
				gesture.GetBoundingBoxCenter (out double x, out double y);
				handler (widget, new GestureMagnifyEventArgs (gesture, x, y, magnification));
			};
			widget.AddController (gesture);
		}

		/// <summary>Raises <paramref name="handler"/> with the incremental rotation (radians) of a two-finger rotation.</summary>
		public static void AddGestureRotateHandler (this Gtk.Widget widget, EventHandler<GestureRotateEventArgs> handler)
		{
			if (widget == null)
				throw new ArgumentNullException (nameof (widget));
			var gesture = new Gtk.GestureRotate ();
			double lastDelta = 0;
			gesture.Begin += (o, a) => lastDelta = 0;
			gesture.AngleChanged += (o, a) => {
				var rotation = a.AngleDelta - lastDelta;
				lastDelta = a.AngleDelta;
				gesture.GetBoundingBoxCenter (out double x, out double y);
				handler (widget, new GestureRotateEventArgs (gesture, x, y, rotation));
			};
			widget.AddController (gesture);
		}

		/// <summary>Raises <paramref name="handler"/> when a swipe ends, with its velocity (pixels/second) as delta.</summary>
		public static void AddGestureSwipeHandler (this Gtk.Widget widget, EventHandler<GestureSwipeEventArgs> handler)
		{
			if (widget == null)
				throw new ArgumentNullException (nameof (widget));
			var gesture = new Gtk.GestureSwipe ();
			gesture.Swipe += (o, a) => {
				gesture.GetBoundingBoxCenter (out double x, out double y);
				handler (widget, new GestureSwipeEventArgs (gesture, x, y, a.VelocityX, a.VelocityY));
			};
			widget.AddController (gesture);
		}
	}

	public abstract class GestureEventArgs : EventArgs
	{
		protected GestureEventArgs (Gtk.Gesture gesture, double x, double y)
		{
			X = x;
			Y = y;
			if (gesture != null) {
				Time = gesture.CurrentEventTime;
				State = gesture.CurrentEventState;
				Device = gesture.Device;
			}
		}

		public Gdk.Device Device { get; }
		public uint Time { get; }
		/// <summary>Gesture center, widget coordinates.</summary>
		public double X { get; }
		public double Y { get; }
		public Gdk.ModifierType State { get; }
	}

	public class GestureMagnifyEventArgs : GestureEventArgs
	{
		public GestureMagnifyEventArgs (Gtk.Gesture gesture, double x, double y, double magnification) : base (gesture, x, y)
		{
			Magnification = magnification;
		}

		public double Magnification { get; }
	}

	public class GestureRotateEventArgs : GestureEventArgs
	{
		public GestureRotateEventArgs (Gtk.Gesture gesture, double x, double y, double rotation) : base (gesture, x, y)
		{
			Rotation = rotation;
		}

		public double Rotation { get; }
	}

	public class GestureSwipeEventArgs : GestureEventArgs
	{
		public GestureSwipeEventArgs (Gtk.Gesture gesture, double x, double y, double deltaX, double deltaY) : base (gesture, x, y)
		{
			DeltaX = deltaX;
			DeltaY = deltaY;
		}

		public double DeltaX { get; }
		public double DeltaY { get; }
	}
}
