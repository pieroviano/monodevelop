//
// CompletionWidgetStyle.cs
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

namespace MonoDevelop.Ide.CodeCompletion
{
	/// <summary>
	/// GTK 4 replacement for <c>Widget.ModifyFg</c>: the foreground colour is applied through a CSS provider
	/// attached to the widget itself.
	/// </summary>
	static class CompletionWidgetStyle
	{
		static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, Gtk.CssProvider> providers = new System.Runtime.CompilerServices.ConditionalWeakTable<Gtk.Widget, Gtk.CssProvider> ();

		/// <summary>Sets the CSS <c>color</c> of <paramref name="widget"/> (and the children inheriting it).</summary>
		public static void SetForeground (Gtk.Widget widget, Cairo.Color color)
		{
			if (!providers.TryGetValue (widget, out var provider)) {
				provider = new Gtk.CssProvider ();
				providers.Add (widget, provider);
				widget.StyleContext.AddProvider (provider, (uint)Gtk.StyleProviderPriority.Application);
			}
			provider.LoadFromString ("* { color: " + ToCss (color) + "; }");
		}

		static string ToCss (Cairo.Color c)
		{
			return string.Format (CultureInfo.InvariantCulture, "rgba({0},{1},{2},{3})",
				(int)Math.Round (c.R * 255), (int)Math.Round (c.G * 255), (int)Math.Round (c.B * 255), c.A);
		}
	}
}
