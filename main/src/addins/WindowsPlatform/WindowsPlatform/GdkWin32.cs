//
// GdkWin32.cs
//
// Author:
//   Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (C) 2011 Novell, Inc (http://www.novell.com)
// Copyright (C) 2012-2013 Xamarin Inc. (https://www.xamarin.com)
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using MonoDevelop.Core;
using CustomControls.OS;
using System.Windows;
using System.Windows.Interop;

namespace MonoDevelop.Platform
{
	public static class GdkWin32
	{
		// GTK 4 has no gdk_win32_set_modal_dialog_libgtk_only: while a native modal dialog runs its own message loop,
		// the GLib main context is pumped from a timer owned by the dialog so that GTK windows keep painting.
		const int PumpTimerId = 0x4D44;
		const int PumpIntervalMs = 20;
		const uint WM_TIMER = 0x0113;

		public static IntPtr HgdiobjGet (MonoDevelop.Components.Window window)
		{
			Gtk.Window gtkWindow = window;
			return HgdiobjGet (gtkWindow);
		}

		/// <summary>The Win32 HWND of the GTK 4 toplevel <paramref name="window"/> (IntPtr.Zero if it is not realized).</summary>
		public static IntPtr HgdiobjGet (Gtk.Window window)
		{
			return Xwt.GtkBackend.GtkWorkarounds.GetGtkWindowNativeHandle (window);
		}

		/// <summary>Runs all pending GLib main loop events (replaces Gtk.Application.EventsPending/RunIteration).</summary>
		internal static void PumpGtkEvents ()
		{
			while (GLib.MainContext.Pending ())
				GLib.MainContext.Iteration (false);
		}

		public static bool RunModalWin32Dialog (CommonFileDialog dialog, Gtk.Window parent)
		{
			PumpGtkEvents ();

			IntPtr ph = HgdiobjGet (parent);

			IntPtr hdlg = IntPtr.Zero;
			dialog.DialogOpening += delegate {
				try {
					hdlg = GetDialogHandle (dialog);
					SetGtkDialogHook (hdlg);
				} catch (Exception ex) {
					LoggingService.LogError ("Failed to hook win32 dialog messages", ex);
				}
			};

			bool result;
			try {
				result = dialog.ShowDialog (ph) == CommonFileDialogResult.Ok;
			} finally {
				if (hdlg != IntPtr.Zero)
					ClearGtkDialogHook (hdlg);
			}
			return result;
		}

		public static bool RunModalWPFDialog (Window dialog, Gtk.Window parent)
		{
			PumpGtkEvents ();

			IntPtr ph = HgdiobjGet (parent);

			IntPtr hdlg = IntPtr.Zero;
			System.Windows.Threading.DispatcherTimer pumpTimer = null;
			dialog.Loaded += delegate {
				try {
					hdlg = new WindowInteropHelper (dialog).Handle;
					SetGtkDialogHook (hdlg, false);
					pumpTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds (PumpIntervalMs) };
					pumpTimer.Tick += delegate { PumpGtkEvents (); };
					pumpTimer.Start ();
				} catch (Exception ex) {
					LoggingService.LogError ("Failed to hook win32 dialog messages", ex);
				}
			};

			bool result;
			try {
				result = dialog.ShowDialog ().Value;
			} finally {
				pumpTimer?.Stop ();
				if (hdlg != IntPtr.Zero)
					ClearGtkDialogHook (hdlg);
			}
			return result;
		}

		internal class SpecialForm : Form
		{
			public virtual DialogResult ShowMagicDialog ()
			{
				return ShowDialog ();
			}

			System.Windows.Forms.Timer pumpTimer;

			protected override void OnHandleCreated (EventArgs e)
			{
				base.OnHandleCreated (e);
				pumpTimer = new System.Windows.Forms.Timer { Interval = PumpIntervalMs };
				pumpTimer.Tick += delegate { PumpGtkEvents (); };
				pumpTimer.Start ();
			}

			protected override void OnHandleDestroyed (EventArgs e)
			{
				if (pumpTimer != null) {
					pumpTimer.Dispose ();
					pumpTimer = null;
				}
				base.OnHandleDestroyed (e);
			}
		}

		internal static bool RunModalWin32Form (SpecialForm form, Gtk.Window parent)
		{
			PumpGtkEvents ();

			IntPtr ph = HgdiobjGet (parent);

			IntPtr hdlg = IntPtr.Zero;
			form.Shown += delegate {
				try {
					hdlg = form.Handle;
					SetGtkDialogHook (hdlg, false);
				} catch (Exception ex) {
					LoggingService.LogError ("Failed to hook win32 dialog messages", ex);
				}
			};

			bool result;
			try {
				result = form.ShowMagicDialog () == DialogResult.OK;
			} finally {
				if (hdlg != IntPtr.Zero)
					ClearGtkDialogHook (hdlg);
			}
			return result;
		}

		// logic based on run_mainloop_hook in gtkprintoperation-win32.c
		static IntPtr GtkWindowProc (IntPtr hdlg, uint uiMsg, IntPtr wParam, IntPtr lParam)
		{
			if (uiMsg == WM_TIMER && wParam == (IntPtr)PumpTimerId) {
				PumpGtkEvents ();
				return IntPtr.Zero;
			}
			return CallWindowProc (dialogWndProc, hdlg, uiMsg, wParam, lParam);
		}

		static IntPtr GetDialogHandle (CommonFileDialog dialog)
		{
			var ow = (IOleWindow)dialog.nativeDialog;
			IntPtr handle;
			var hr = ow.GetWindow (out handle);
			if (hr != 0)
				throw Marshal.GetExceptionForHR (hr);
			return handle;
		}

		static void SetGtkDialogHook (IntPtr hdlg, bool overrideWndProc = true)
		{
			if (dialogHookSet)
				throw new InvalidOperationException ("There is already an active hook");
			dialogHookSet = true;
			if (overrideWndProc) {
				SetTimer (hdlg, (IntPtr)PumpTimerId, PumpIntervalMs, IntPtr.Zero);
				dialogWndProc = GetWindowLongPtr (hdlg, DWLP_DLGPROC);
				SetWindowLongPtr (hdlg, DWLP_DLGPROC, Marshal.GetFunctionPointerForDelegate (GtkWindowProcDelegate));
			}
		}

		static void ClearGtkDialogHook (IntPtr hdlg)
		{
			if (dialogWndProc != IntPtr.Zero) {
				KillTimer (hdlg, (IntPtr)PumpTimerId);
				SetWindowLongPtr (hdlg, DWLP_DLGPROC, dialogWndProc);
				dialogWndProc = IntPtr.Zero;
			}
			dialogHookSet = false;
		}

		static IntPtr dialogWndProc;
		static bool dialogHookSet;

		static readonly WindowProc GtkWindowProcDelegate = GtkWindowProc;
		static readonly int DWLP_DLGPROC = IntPtr.Size; // DWLP_MSGRESULT + sizeof(LRESULT);

		[DllImport (Win32.USER32)]
		static extern IntPtr SetTimer (IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

		[DllImport (Win32.USER32)]
		static extern bool KillTimer (IntPtr hWnd, IntPtr uIDEvent);

		[DllImport(Win32.USER32)]
		static extern IntPtr CallWindowProc (IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		static IntPtr SetWindowLongPtr (IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 4)
				return SetWindowLongPtr32 (hWnd, nIndex, dwNewLong);
			return SetWindowLongPtr64 (hWnd, nIndex, dwNewLong);
		}

		[DllImport (Win32.USER32, EntryPoint="SetWindowLongPtr")]
		static extern IntPtr SetWindowLongPtr64 (IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport(Win32.USER32, EntryPoint="SetWindowLong")]
		static extern IntPtr SetWindowLongPtr32 (IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		static IntPtr GetWindowLongPtr (IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 4)
				return GetWindowLongPtr32 (hWnd, nIndex);
			return GetWindowLongPtr64 (hWnd, nIndex);
		}

		[DllImport (Win32.USER32, EntryPoint="GetWindowLongPtr")]
		static extern IntPtr GetWindowLongPtr64 (IntPtr hWnd, int nIndex);

		[DllImport(Win32.USER32, EntryPoint="GetWindowLong")]
		static extern IntPtr GetWindowLongPtr32 (IntPtr hWnd, int nIndex);

		delegate IntPtr WindowProc (IntPtr hdlg, uint uiMsg, IntPtr wParam, IntPtr lParam);
	}

	
	[ComImport]
	[Guid("00000114-0000-0000-C000-000000000046")]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	interface IOleWindow
	{
		/// <summary>
		/// Returns the window handle to one of the windows participating in in-place activation
		/// (frame, document, parent, or in-place object window).
		/// </summary>
		/// <param name="phwnd">Pointer to where to return the window handle.</param>
		int GetWindow (out IntPtr phwnd) ;

		/// <summary>
		/// Determines whether context-sensitive help mode should be entered during an
		/// in-place activation session.
		/// </summary>
		/// <param name="fEnterMode"><c>true</c> if help mode should be entered;
		/// <c>false</c> if it should be exited.</param>
		void ContextSensitiveHelp ([In, MarshalAs(UnmanagedType.Bool)] bool fEnterMode) ;
	}

	[ComImport]
	[Guid ( "b4db1657-70d7-485e-8e3e-6fcb5a5c1802" )]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	interface IModalWindow
	{
		int Show ([In] IntPtr parent);
	}
	
	public class GtkWin32Proxy : System.Windows.Forms.IWin32Window
	{
		public GtkWin32Proxy (Gtk.Window gtkWindow)
		{
			Handle = GdkWin32.HgdiobjGet (gtkWindow);
		}
		
		public IntPtr Handle { get; private set; }
	}
}