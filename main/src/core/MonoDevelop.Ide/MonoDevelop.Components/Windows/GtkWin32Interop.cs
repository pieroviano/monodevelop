//
// GtkWin32Interop.cs
//
// Author:
//       Marius Ungureanu <marius.ungureanu@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc (http://www.xamarin.com)
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
#if WIN32
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MonoDevelop.Components.Windows
{
	public static class GtkWin32Interop
	{
		internal const string USER32 = "user32.dll";

		public enum ExtendedWindowStyles
		{
			// Hides it from alt-tab.
			WS_EX_TOOLWINDOW = 0x80,
		}

		public enum GWLParameter
		{
			// Sets a new extended style.
			GWL_EXSTYLE = -20,

			// Sets a new application handle instance.
			GWL_HINSTANCE = -6,

			// Sets a new window handle as the parent.
			GWL_HWNDPARENT = -8,

			// Sets a new identifier of the window.
			GWL_ID = -12,

			// Sets a new window style.
			GWL_STYLE = -16,

			// Sets a new user data associated with the window. Initially zero.
			GWL_USERDATA = -21,

			// Sets a new address of the window procedure.
			GWL_WNDPROC = -4,
		}

		/// <summary>
		/// Win32 HWND of a GTK 4 toplevel surface, or <see cref="IntPtr.Zero"/> when it cannot be obtained.
		/// </summary>
		/// <remarks>
		/// TODO: GTK 4 provides gdk_win32_surface_get_handle (GdkWin32Surface), but GtkSharp4 does not bind the
		/// Gdk Win32 backend API yet, and P/Invokes into the GTK native libraries are not allowed here. Until the
		/// binding exists this returns IntPtr.Zero and callers must skip native parenting.
		/// </remarks>
		public static IntPtr HWndGet (Gdk.Surface surface)
		{
			return IntPtr.Zero;
		}

		/// <summary>Win32 HWND of <paramref name="window"/>'s surface (see <see cref="HWndGet(Gdk.Surface)"/>).</summary>
		public static IntPtr HWndGet (Gtk.Window window)
		{
			var surface = window?.Surface;
			return surface != null ? HWndGet (surface) : IntPtr.Zero;
		}

		[DllImport (USER32, EntryPoint="SetWindowLongPtr")]
		static extern IntPtr SetWindowLongPtr64 (IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport(USER32, EntryPoint="SetWindowLong")]
		static extern IntPtr SetWindowLongPtr32 (IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		public static IntPtr SetWindowLongPtr (IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 4)
				return SetWindowLongPtr32 (hWnd, nIndex, dwNewLong);
			return SetWindowLongPtr64 (hWnd, nIndex, dwNewLong);
		}

		[DllImport (USER32, EntryPoint="GetWindowLongPtr")]
		static extern IntPtr GetWindowLongPtr64 (IntPtr hWnd, int nIndex);

		[DllImport(USER32, EntryPoint="GetWindowLong")]
		static extern IntPtr GetWindowLongPtr32 (IntPtr hWnd, int nIndex);

		public static IntPtr GetWindowLongPtr (IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 4)
				return GetWindowLongPtr32 (hWnd, nIndex);
			return GetWindowLongPtr64 (hWnd, nIndex);
		}

		/// <summary>
		/// Converts a WPF key press to the GTK 4 key data (keyval, hardware keycode, modifier state) that
		/// GDK's Win32 backend would report for it (replaces the GTK 2 synthesized <c>Gdk.EventKey</c>).
		/// </summary>
		internal static (uint Keyval, uint Keycode, Gdk.ModifierType State) ConvertKeyEvent (ModifierKeys mod, Key key)
		{
			var state = (Gdk.ModifierType)0;
			if ((mod & ModifierKeys.Control) != 0)
				state |= Gdk.ModifierType.ControlMask;
			if ((mod & ModifierKeys.Shift) != 0)
				state |= Gdk.ModifierType.ShiftMask;
			if ((mod & ModifierKeys.Windows) != 0)
				state |= Gdk.ModifierType.SuperMask;
			if ((mod & ModifierKeys.Alt) != 0)
				state |= Gdk.ModifierType.AltMask;

			// On Windows GDK uses the virtual-key code as the hardware keycode.
			uint keycode = (uint)KeyInterop.VirtualKeyFromKey (key);
			uint keyval = 0;
			var display = Gdk.Display.Default;
			if (display != null)
				display.TranslateKey (keycode, state, 0, out keyval, out _, out _, out _);
			return (keyval, keycode, state);
		}
	}
}
#endif
