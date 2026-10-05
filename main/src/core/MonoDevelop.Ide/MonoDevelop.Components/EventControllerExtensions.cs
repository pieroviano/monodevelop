//
// EventControllerExtensions.cs
//
// Copyright (c) Microsoft Corp.
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
	/// <summary>Data of a pointer click (replaces GTK 2 <c>Gdk.EventButton</c> in MonoDevelop APIs).</summary>
	public sealed class ClickInfo
	{
		public ClickInfo (Gtk.GestureClick gesture, double x, double y, int nPress)
		{
			Gesture = gesture;
			X = x;
			Y = y;
			NPress = nPress;
			Button = gesture?.CurrentButton ?? 0;
			State = gesture?.CurrentEventState ?? 0;
			Time = gesture?.CurrentEventTime ?? 0;
		}

		/// <summary>The gesture that recognized the click (null when synthesized).</summary>
		public Gtk.GestureClick Gesture { get; }
		/// <summary>X in the controller widget's coordinates.</summary>
		public double X { get; }
		/// <summary>Y in the controller widget's coordinates.</summary>
		public double Y { get; }
		/// <summary>1 = single, 2 = double, 3 = triple click.</summary>
		public int NPress { get; }
		/// <summary>1 left, 2 middle, 3 right.</summary>
		public uint Button { get; }
		public Gdk.ModifierType State { get; }
		public uint Time { get; }
		/// <summary>Right button, or Ctrl+left on macOS.</summary>
		public bool IsContextMenuTrigger => Button == 3 || (Button == 1 && Core.Platform.IsMac && (State & Gdk.ModifierType.ControlMask) != 0);

		/// <summary>Claims the event sequence so other gestures/widgets stop handling it.</summary>
		public void Claim ()
		{
			Gesture?.SetState (Gtk.EventSequenceState.Claimed);
		}
	}

	/// <summary>
	/// Event-controller shortcuts (replace GTK 2 Button/Key/Motion/Scroll/Focus events). Each returns the controller
	/// so callers can remove it with <c>widget.RemoveController (c)</c>.
	/// <code>
	/// widget.AddClickHandler (c => { if (c.IsContextMenuTrigger) ShowMenu (c); });
	/// widget.AddKeyPressHandler ((key, code, mods) => (Gdk.Key)key == Gdk.Key.Escape &amp;&amp; Close ());
	/// </code>
	/// </summary>
	public static class EventControllerExtensions
	{
		/// <summary>Click gesture for <paramref name="button"/> (0 = any) raising <paramref name="pressed"/>/<paramref name="released"/>.</summary>
		public static Gtk.GestureClick AddClickHandler (this Gtk.Widget widget, Action<ClickInfo> pressed, Action<ClickInfo> released = null, uint button = 0, Gtk.PropagationPhase phase = Gtk.PropagationPhase.Bubble)
		{
			var g = new Gtk.GestureClick { Button = button, PropagationPhase = phase };
			if (pressed != null)
				g.Pressed += (o, a) => pressed (new ClickInfo (g, a.X, a.Y, a.NPress));
			if (released != null)
				g.Released += (o, a) => released (new ClickInfo (g, a.X, a.Y, a.NPress));
			widget.AddController (g);
			return g;
		}

		/// <summary>Key controller; <paramref name="pressed"/>(keyval, keycode, state) returns true when handled.</summary>
		public static Gtk.EventControllerKey AddKeyPressHandler (this Gtk.Widget widget, Func<uint, uint, Gdk.ModifierType, bool> pressed, Action<uint, uint, Gdk.ModifierType> released = null, Gtk.PropagationPhase phase = Gtk.PropagationPhase.Bubble)
		{
			var k = new Gtk.EventControllerKey { PropagationPhase = phase };
			if (pressed != null)
				k.KeyPressed += (o, a) => a.RetVal = pressed (a.Keyval, a.Keycode, a.State);
			if (released != null)
				k.KeyReleased += (o, a) => released (a.Keyval, a.Keycode, a.State);
			widget.AddController (k);
			return k;
		}

		/// <summary>Motion controller (pointer enter/move/leave, widget coordinates).</summary>
		public static Gtk.EventControllerMotion AddMotionHandler (this Gtk.Widget widget, Action<double, double> motion, Action<double, double> enter = null, Action leave = null)
		{
			var m = new Gtk.EventControllerMotion ();
			if (motion != null)
				m.Motion += (o, a) => motion (a.X, a.Y);
			if (enter != null)
				m.Enter += (o, a) => enter (a.X, a.Y);
			if (leave != null)
				m.Leave += (o, a) => leave ();
			widget.AddController (m);
			return m;
		}

		/// <summary>Scroll controller; <paramref name="scroll"/>(dx, dy, state) returns true when handled.</summary>
		public static Gtk.EventControllerScroll AddScrollHandler (this Gtk.Widget widget, Func<double, double, Gdk.ModifierType, bool> scroll, Gtk.EventControllerScrollFlags flags = Gtk.EventControllerScrollFlags.BothAxes)
		{
			var s = new Gtk.EventControllerScroll (flags);
			s.Scroll += (o, a) => a.RetVal = scroll (a.Dx, a.Dy, s.CurrentEventState);
			widget.AddController (s);
			return s;
		}

		/// <summary>Focus controller (keyboard focus entered/left the widget or a descendant).</summary>
		public static Gtk.EventControllerFocus AddFocusHandler (this Gtk.Widget widget, Action enter, Action leave)
		{
			var f = new Gtk.EventControllerFocus ();
			if (enter != null)
				f.Enter += (o, a) => enter ();
			if (leave != null)
				f.Leave += (o, a) => leave ();
			widget.AddController (f);
			return f;
		}

		/// <summary>Drag gesture: start(x, y), update(offsetX, offsetY), end(offsetX, offsetY).</summary>
		public static Gtk.GestureDrag AddDragHandler (this Gtk.Widget widget, Action<double, double> start, Action<double, double> update, Action<double, double> end = null, uint button = 1)
		{
			var d = new Gtk.GestureDrag { Button = button };
			if (start != null)
				d.DragStarted += (o, a) => start (a.StartX, a.StartY);
			if (update != null)
				d.DragUpdate += (o, a) => update (a.OffsetX, a.OffsetY);
			if (end != null)
				d.DragEnded += (o, a) => end (a.OffsetX, a.OffsetY);
			widget.AddController (d);
			return d;
		}

		/// <summary>Current keyboard modifiers of the default seat (replaces <c>Gtk.Global.GetCurrentEventState</c>).</summary>
		public static Gdk.ModifierType GetCurrentModifiers (this Gtk.Widget widget)
		{
			var kb = widget?.Display?.DefaultSeat?.Keyboard;
			return kb != null ? kb.ModifierState : 0;
		}
	}
}
