//
// EditorWidgetStyle.cs
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
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MonoDevelop.SourceEditor
{
	/// <summary>
	/// GTK 4 replacement for <c>Widget.ModifyFg/ModifyBg/ModifyBase/ModifyText</c>: colors are applied through a
	/// CSS provider attached to the widget itself. Passing <c>null</c> for a color resets it to the theme default.
	/// </summary>
	static class EditorWidgetStyle
	{
		class State
		{
			public Gtk.CssProvider Provider = new Gtk.CssProvider ();
			public Cairo.Color? Foreground;
			public Cairo.Color? Background;
		}

		static readonly ConditionalWeakTable<Gtk.Widget, State> states = new ConditionalWeakTable<Gtk.Widget, State> ();

		static State GetState (Gtk.Widget widget)
		{
			if (!states.TryGetValue (widget, out var state)) {
				state = new State ();
				states.Add (widget, state);
				widget.StyleContext.AddProvider (state.Provider, (uint)Gtk.StyleProviderPriority.Application);
			}
			return state;
		}

		static void Apply (State state)
		{
			var css = new System.Text.StringBuilder ("* {");
			if (state.Foreground.HasValue)
				css.Append (" color: ").Append (ToCss (state.Foreground.Value)).Append (";");
			if (state.Background.HasValue)
				css.Append (" background-color: ").Append (ToCss (state.Background.Value)).Append (";");
			css.Append (" }");
			state.Provider.LoadFromString (css.ToString ());
		}

		/// <summary>Sets the text color of <paramref name="widget"/> and the children inheriting it.</summary>
		public static void SetForeground (Gtk.Widget widget, Cairo.Color? color)
		{
			var state = GetState (widget);
			state.Foreground = color;
			Apply (state);
		}

		/// <summary>Sets the background color of <paramref name="widget"/>.</summary>
		public static void SetBackground (Gtk.Widget widget, Cairo.Color? color)
		{
			var state = GetState (widget);
			state.Background = color;
			Apply (state);
		}

		static string ToCss (Cairo.Color c)
		{
			return string.Format (CultureInfo.InvariantCulture, "rgba({0},{1},{2},{3})",
				(int)Math.Round (c.R * 255), (int)Math.Round (c.G * 255), (int)Math.Round (c.B * 255), c.A);
		}
	}

	static class EditorWidgetExtensions
	{
		/// <summary>
		/// Calls <paramref name="changed"/> whenever the allocated size of <paramref name="widget"/> changes (replaces the
		/// GTK 2 <c>SizeAllocated</c> event, which GTK 4 does not provide for arbitrary widgets). Dispose the result to stop observing.
		/// </summary>
		public static IDisposable ObserveSizeChanged (this Gtk.Widget widget, Action changed)
		{
			return new SizeObserver (widget, changed);
		}

		sealed class SizeObserver : IDisposable
		{
			readonly Gtk.Widget widget;
			readonly Action changed;
			uint tickId;
			int width, height;

			public SizeObserver (Gtk.Widget widget, Action changed)
			{
				this.widget = widget;
				this.changed = changed;
				width = widget.Width;
				height = widget.Height;
				tickId = widget.AddTickCallback (OnTick);
			}

			bool OnTick (Gtk.Widget w, Gdk.FrameClock clock)
			{
				if (w.Width != width || w.Height != height) {
					width = w.Width;
					height = w.Height;
					changed ();
				}
				return true;
			}

			public void Dispose ()
			{
				if (tickId != 0) {
					widget.RemoveTickCallback (tickId);
					tickId = 0;
				}
			}
		}

		/// <summary>The size the widget would like to have (replaces GTK 2 <c>Widget.SizeRequest ()</c>).</summary>
		public static Gdk.Size GetNaturalSize (this Gtk.Widget widget)
		{
			if (!widget.Visible)
				return new Gdk.Size (0, 0);
			widget.Measure (Gtk.Orientation.Horizontal, -1, out _, out int width, out _, out _);
			widget.Measure (Gtk.Orientation.Vertical, width, out _, out int height, out _, out _);
			return new Gdk.Size (width, height);
		}
	}
}
