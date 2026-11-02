//
// WindowTransparencyDecorator.cs
//
// Author:
//   Michael Hutchinson <mhutch@xamarin.com>
//
// Based on code derived from Banshee.Widgets.EllipsizeLabel
// by Aaron Bockover (aaron@aaronbock.net)
//
// Copyright (C) 2005-2008 Novell, Inc (http://www.novell.com)
// Copyright (C) 2012 Xamarin Inc (http://www.xamarin.com)
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
	/// Makes a window (or popover) semi-transparent while Control is held down.
	/// GTK 4 has no key snoopers: a capture-phase key controller is installed on the widget's root while it is shown.
	/// </summary>
	public class WindowTransparencyDecorator
	{
		Gtk.Widget window;
		bool semiTransparent;
		Gtk.EventControllerKey keyController;
		Gtk.Widget keyControllerOwner;
		const double opacity = 0.2;

		WindowTransparencyDecorator (Gtk.Widget window)
		{
			this.window = window;

			window.Shown += ShownHandler;
			window.Hidden += HiddenHandler;
			if (window.Visible)
				ShownHandler (window, EventArgs.Empty);
		}

		/// <summary>Attaches to a toplevel window or to a popover (any widget with its own surface).</summary>
		public static WindowTransparencyDecorator Attach (Gtk.Widget window)
		{
			return new WindowTransparencyDecorator (window);
		}

		public static WindowTransparencyDecorator Attach (Xwt.WindowFrame window)
		{
			var gtkWindow = Xwt.Toolkit.Load (Xwt.ToolkitType.Gtk4).GetNativeWindow (window) as Gtk.Window;
			if (gtkWindow != null)
				return new WindowTransparencyDecorator (gtkWindow);
			throw new NotSupportedException ();
		}

		public void Detach ()
		{
			if (window == null)
				return;

			//remove the key controller
			HiddenHandler (null,  null);

			//annul allreferences between this and the window
			window.Shown -= ShownHandler;
			window.Hidden -= HiddenHandler;
			window = null;
		}

		void ShownHandler (object sender, EventArgs args)
		{
			if (keyController == null) {
				// Popovers don't get the keyboard focus: watch the keys of the toplevel they belong to.
				keyControllerOwner = window.Root as Gtk.Widget ?? window;
				keyController = keyControllerOwner.AddKeyPressHandler (
					(keyval, keycode, state) => { OnControlKey (keyval, true); return false; },
					(keyval, keycode, state) => OnControlKey (keyval, false),
					Gtk.PropagationPhase.Capture);
			}

			//NOTE: we unset transparency when showing, instead of when hiding
			//because the latter case triggers a metacity+compositing bug that shows the window again
			SemiTransparent = false;
		}

		void HiddenHandler (object sender, EventArgs args)
		{
			if (keyController != null) {
				keyControllerOwner?.RemoveController (keyController);
				keyController = null;
				keyControllerOwner = null;
			}
		}

		void OnControlKey (uint keyval, bool pressed)
		{
			var key = (Gdk.Key)keyval;
			if (key == Gdk.Key.Control_L || key == Gdk.Key.Control_R)
				SemiTransparent = pressed;
		}

		bool SemiTransparent {
			set {
				if (semiTransparent != value && window != null) {
					semiTransparent = value;
					window.Opacity = semiTransparent? opacity : 1.0;
				}
			}
		}
	}
}
