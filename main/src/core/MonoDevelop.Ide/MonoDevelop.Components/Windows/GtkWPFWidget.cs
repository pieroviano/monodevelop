//
// GtkWPFWidget.cs
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
using System.Windows;
using System.Windows.Interop;
using System.Threading;
using System.Runtime.InteropServices;

namespace MonoDevelop.Components.Windows
{
	/// <summary>
	/// Hosts a WPF control in an owned Win32 window kept over this widget's area.
	/// </summary>
	/// <remarks>
	/// GTK 4 limitations: the toplevel HWND is not reachable until GtkSharp binds the Gdk Win32 backend
	/// (see <see cref="GtkWin32Interop.HWndGet(Gtk.Window)"/>), so the WPF window is not owned by the IDE window
	/// yet; and GTK 4 does not report toplevel moves, so the WPF window follows size/layout changes only.
	/// </remarks>
	public class GtkWPFWidget : Gtk.Widget
	{
		Gdk.Surface rootSurface;

		internal System.Windows.Forms.Integration.ElementHost wpfWidgetHost {
			get;
			private set;
		}

		public GtkWPFWidget (System.Windows.Controls.Control wpfControl)
		{
			wpfWidgetHost = new System.Windows.Forms.Integration.ElementHost
			{
				BackColor = System.Drawing.Color.SeaGreen,
				Child = wpfControl,
			};

			wpfControl.PreviewKeyDown += (sender, e) => {
				// TODO: Some commands check for toplevels, and this window is not a toplevel.
				if (e.Key == System.Windows.Input.Key.Escape)
				{
					System.Windows.Input.Keyboard.ClearFocus();
					MonoDevelop.Ide.IdeApp.Workbench.Present();
					return;
				}

				var key = e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key;
				var k = GtkWin32Interop.ConvertKeyEvent (e.KeyboardDevice.Modifiers, key);
				e.Handled = Ide.IdeApp.CommandService.ProcessKeyEvent (k.Keyval, k.Keycode, k.State);
			};
		}

		protected virtual void RepositionWpfWindow ()
		{
			int scale = (int)MonoDevelop.Components.GtkWorkarounds.GetScaleFactor(this);
			RepositionWpfWindow (scale, scale);
		}

		protected void RepositionWpfWindow (double hscale, double vscale)
		{
			var root = Root as Gtk.Widget;
			double x, y;
			if (root != null && TranslateCoordinates (root, 0, 0, out x, out y)) {
				wpfWidgetHost.Left = (int)(x * hscale);
				wpfWidgetHost.Top = (int)(y * vscale);
			} else {
				wpfWidgetHost.Left = 0;
				wpfWidgetHost.Top = 0;
			}
			wpfWidgetHost.Width = (int)((Width + 1) * hscale);
			wpfWidgetHost.Height = (int)((Height + 1) * vscale);
		}

		protected override void OnRealized ()
		{
			base.OnRealized ();

			// Initial size setting.
			RepositionWpfWindow ();
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);

			// Needed for window full screening.
			RepositionWpfWindow ();
		}

		void OnRootLayout (object sender, Gdk.LayoutArgs args)
		{
			RepositionWpfWindow();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				wpfWidgetHost.Dispose ();
			base.Dispose (disposing);
		}

		protected override void OnShown ()
		{
			base.OnShown ();

			var rootWindow = Ide.IdeApp.Workbench.RootWindow;
			IntPtr gtkWindowPtr = GtkWin32Interop.HWndGet (rootWindow);
			IntPtr wpfWindowPtr = wpfWidgetHost.Handle;
			if (gtkWindowPtr != IntPtr.Zero)
				GtkWin32Interop.SetWindowLongPtr(wpfWindowPtr, (int)GtkWin32Interop.GWLParameter.GWL_HWNDPARENT, gtkWindowPtr);

			// GTK 4: the toplevel surface's ::layout replaces the GTK 2 ConfigureEvent.
			rootSurface = rootWindow?.Surface;
			if (rootSurface != null)
				rootSurface.Layout += OnRootLayout;
		}

		protected override void OnHidden ()
		{
			base.OnHidden ();

			if (rootSurface != null) {
				rootSurface.Layout -= OnRootLayout;
				rootSurface = null;
			}
		}
	}
}
#endif
