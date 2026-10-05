//
// ModalRunner.cs
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
	/// <summary>
	/// Synchronous modal run for GTK 4 (which removed <c>gtk_dialog_run</c>): presents the window and spins a nested
	/// <see cref="GLib.MainLoop"/> until it responds/closes. Used by MessageService/IdeDialog.
	/// <code>
	/// int r = dialog.RunModal ();           // Gtk.Dialog: returns the response id
	/// window.RunModal ();                   // Gtk.Window: returns when hidden/closed; call ModalRunner.EndModal (window) to stop
	/// </code>
	/// </summary>
	public static class ModalRunner
	{
		/// <summary>Runs <paramref name="dialog"/> modally; returns the (int) <see cref="Gtk.ResponseType"/> emitted. Does not destroy it.</summary>
		public static int RunModal (this Gtk.Dialog dialog)
		{
			int response = (int)Gtk.ResponseType.None;
			var loop = new GLib.MainLoop ();
			void Quit ()
			{
				if (loop.IsRunning)
					loop.Quit ();
			}
			Gtk.ResponseHandler onResponse = (o, a) => { response = (int)a.ResponseId; Quit (); };
			Gtk.CloseRequestHandler onClose = (o, a) => { response = (int)Gtk.ResponseType.DeleteEvent; Quit (); a.RetVal = true; };
			EventHandler onHidden = (o, a) => Quit ();

			dialog.Response += onResponse;
			dialog.CloseRequest += onClose;
			dialog.Hidden += onHidden;
			bool wasModal = dialog.Modal;
			dialog.Modal = true;
			try {
				dialog.Present ();
				loop.Run ();
			} finally {
				if (dialog.Handle != IntPtr.Zero) {
					dialog.Response -= onResponse;
					dialog.CloseRequest -= onClose;
					dialog.Hidden -= onHidden;
					dialog.Modal = wasModal;
				}
			}
			return response;
		}

		/// <summary>Runs <paramref name="window"/> modally until it is hidden, closed or <see cref="EndModal"/> is called. Does not destroy it.</summary>
		public static void RunModal (this Gtk.Window window)
		{
			if (window is Gtk.Dialog d) {
				d.RunModal ();
				return;
			}
			var loop = new GLib.MainLoop ();
			void Quit ()
			{
				if (loop.IsRunning)
					loop.Quit ();
			}
			Gtk.CloseRequestHandler onClose = (o, a) => Quit ();
			EventHandler onHidden = (o, a) => Quit ();
			window.CloseRequest += onClose;
			window.Hidden += onHidden;
			bool wasModal = window.Modal;
			window.Modal = true;
			window.Data [LoopKey] = loop;
			try {
				window.Present ();
				loop.Run ();
			} finally {
				if (window.Handle != IntPtr.Zero) {
					window.Data.Remove (LoopKey);
					window.CloseRequest -= onClose;
					window.Hidden -= onHidden;
					window.Modal = wasModal;
				}
			}
		}

		/// <summary>Ends a <see cref="RunModal(Gtk.Window)"/> without hiding the window.</summary>
		public static void EndModal (Gtk.Window window)
		{
			if (window?.Data [LoopKey] is GLib.MainLoop loop && loop.IsRunning)
				loop.Quit ();
		}

		/// <summary>Spins the default main context until <paramref name="done"/> returns true (synchronous wait for an async GTK 4 API).</summary>
		public static void RunUntil (Func<bool> done)
		{
			var ctx = GLib.MainContext.Default;
			while (!done ())
				ctx.RunIteration (true);
		}

		const string LoopKey = "md-modal-loop";
	}
}
