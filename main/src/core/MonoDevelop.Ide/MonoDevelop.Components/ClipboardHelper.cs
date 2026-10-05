//
// ClipboardHelper.cs
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
using System.Threading.Tasks;

namespace MonoDevelop.Components
{
	/// <summary>
	/// <see cref="Gdk.Clipboard"/> access (replaces GTK 2 <c>Gtk.Clipboard.Get (atom)</c>; reads are async in GTK 4).
	/// <code>
	/// ClipboardHelper.SetText ("abc");
	/// string s = await ClipboardHelper.GetTextAsync ();
	/// string t = ClipboardHelper.GetText ();   // blocks by iterating the main loop; avoid in new code
	/// </code>
	/// </summary>
	public static class ClipboardHelper
	{
		/// <summary>The default display's CLIPBOARD selection.</summary>
		public static Gdk.Clipboard Clipboard => Gdk.Display.Default?.Clipboard;

		/// <summary>The default display's PRIMARY selection (X11/Wayland; same as <see cref="Clipboard"/> elsewhere).</summary>
		public static Gdk.Clipboard PrimaryClipboard => Gdk.Display.Default?.PrimaryClipboard ?? Clipboard;

		/// <summary>Puts <paramref name="text"/> on the clipboard (primary selection when <paramref name="primary"/>).</summary>
		public static void SetText (string text, bool primary = false)
		{
			var cb = primary ? PrimaryClipboard : Clipboard;
			if (cb != null)
				cb.Text = text ?? string.Empty;
		}

		/// <summary>Reads the clipboard text; null when it holds no text.</summary>
		public static Task<string> GetTextAsync (bool primary = false)
		{
			var cb = primary ? PrimaryClipboard : Clipboard;
			var tcs = new TaskCompletionSource<string> ();
			if (cb == null) {
				tcs.SetResult (null);
				return tcs.Task;
			}
			cb.ReadTextAsync (null, (source, res, data) => {
				try {
					tcs.TrySetResult (cb.ReadTextFinish (res));
				} catch (Exception) {
					tcs.TrySetResult (null);
				}
			});
			return tcs.Task;
		}

		/// <summary>Synchronous text read: spins the main loop until the async read completes.</summary>
		public static string GetText (bool primary = false)
		{
			var t = GetTextAsync (primary);
			ModalRunner.RunUntil (() => t.IsCompleted);
			return t.Result;
		}

		/// <summary>True when the clipboard currently advertises text.</summary>
		public static bool ContainsText (bool primary = false)
		{
			var cb = primary ? PrimaryClipboard : Clipboard;
			return cb?.Formats?.ContainGtype (GLib.GType.String) ?? false;
		}
	}
}
