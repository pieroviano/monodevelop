//
// GtkWorkarounds.cs
//
// Authors: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (C) 2011 Xamarin Inc.
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
//

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Gtk;
using MonoDevelop.Core;
using System.Text.RegularExpressions;

#if MAC
using AppKit;
using MonoDevelop.Components.Mac;
using CoreGraphics;
#endif
#if WIN32
using System.Windows.Input;
#endif

namespace MonoDevelop.Components
{
	public static partial class GtkWorkarounds
	{
		const string USER32DLL = "User32.dll";

		/// <summary>GTK 2 feature-level checks; GTK 4 provides every feature these guarded, so they report the last GTK 2 level.</summary>
		public static int GtkMinorVersion = 24, GtkMicroVersion = 0;

#if MAC
		static Gdk.Rectangle MacGetUsableMonitorGeometry (Gdk.Monitor monitor)
		{
			var geometry = monitor.Geometry;
			int index = IndexOfMonitor (monitor);
			var screens = NSScreen.Screens;
			if (index < 0 || (uint)index >= screens.Length)
				return geometry;

			var macScreen = screens [index];
			CGRect visible = macScreen.VisibleFrame;
			CGRect frame = macScreen.Frame;

			// VisibleFrame.Y is the height of the Dock if it is at the bottom of the screen; the menu height is the
			// difference between the visible and full frame heights minus the Dock height.
			double x, y, width, height;
			if (visible.Height < frame.Height) {
				double dockHeight = visible.Y - frame.Y;
				double menubarHeight = (frame.Height - visible.Height) - dockHeight;
				height = frame.Height - menubarHeight - dockHeight;
				y = geometry.Y + menubarHeight;
			} else {
				height = frame.Height;
				y = geometry.Y;
			}
			width = System.Math.Min (visible.Width, frame.Width);
			x = geometry.X + System.Math.Max (0, visible.X - frame.X);
			return new Gdk.Rectangle ((int) x, (int) y, (int) width, (int) height);
		}

		static void MacRequestAttention (bool critical)
		{
			NSRequestUserAttentionType kind = critical ? NSRequestUserAttentionType.CriticalRequest : NSRequestUserAttentionType.InformationalRequest;
			NSApplication.SharedApplication.RequestUserAttention (kind);
		}
#endif

		// Note: we can't reuse RectangleF because the layout is different...
		[StructLayout (LayoutKind.Sequential)]
		struct Rect {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public int Width { get { return Right - Left; } }
			public int Height { get { return Bottom - Top; } }
		}

		[StructLayout (LayoutKind.Sequential)]
		unsafe struct MonitorInfo {
			public int Size;
			public Rect Frame;         // Monitor
			public Rect VisibleFrame;  // Work
			public int Flags;
			public fixed byte Device[32];
		}

		[UnmanagedFunctionPointer (CallingConvention.Winapi)]
		delegate int EnumMonitorsCallback (IntPtr hmonitor, IntPtr hdc, IntPtr prect, IntPtr user_data);

		[DllImport (USER32DLL)]
		extern static int EnumDisplayMonitors (IntPtr hdc, IntPtr clip, EnumMonitorsCallback callback, IntPtr user_data);

		[DllImport (USER32DLL)]
		extern static int GetMonitorInfoA (IntPtr hmonitor, ref MonitorInfo info);

		static Gdk.Rectangle WindowsGetUsableMonitorGeometry (Gdk.Monitor monitor)
		{
			// GDK reports logical pixels, Win32 physical ones: match the monitor by its scaled frame.
			Gdk.Rectangle geometry = monitor.Geometry;
			double scale = monitor.Scale > 0 ? monitor.Scale : 1;
			Gdk.Rectangle result = geometry;

			EnumDisplayMonitors (IntPtr.Zero, IntPtr.Zero, delegate (IntPtr hmonitor, IntPtr hdc, IntPtr prect, IntPtr user_data) {
				var info = new MonitorInfo ();
				unsafe {
					info.Size = sizeof (MonitorInfo);
				}
				GetMonitorInfoA (hmonitor, ref info);
				if (Math.Abs (info.Frame.Width / scale - geometry.Width) <= 1 && Math.Abs (info.Frame.Height / scale - geometry.Height) <= 1
				    && Math.Abs (info.Frame.Left / scale - geometry.X) <= 1 && Math.Abs (info.Frame.Top / scale - geometry.Y) <= 1) {
					var visible = info.VisibleFrame;
					var frame = info.Frame;
					result = new Gdk.Rectangle (
						geometry.X + (int)((visible.Left - frame.Left) / scale),
						geometry.Y + (int)((visible.Top - frame.Top) / scale),
						(int)(visible.Width / scale),
						(int)(visible.Height / scale));
					return 0;
				}
				return 1;
			}, IntPtr.Zero);

			return result;
		}

		/// <summary>Monitor work area (excluding task bar / dock / menu bar), in GDK logical coordinates.</summary>
		public static Gdk.Rectangle GetUsableMonitorGeometry (this Gdk.Monitor monitor)
		{
#if WIN32
			return WindowsGetUsableMonitorGeometry (monitor);
#elif MAC
			return MacGetUsableMonitorGeometry (monitor);
#else
			return monitor.Geometry;
#endif
		}

		/// <summary>The monitor showing <paramref name="widget"/> (first monitor if the widget is not realized).</summary>
		public static Gdk.Monitor GetMonitor (this Gtk.Widget widget)
		{
			var display = widget?.Display ?? Gdk.Display.Default;
			var surface = widget?.Native?.Surface;
			if (surface != null) {
				var m = display.GetMonitorAtSurface (surface);
				if (m != null)
					return m;
			}
			return GetMonitors (display).Count > 0 ? GetMonitors (display) [0] : null;
		}

		/// <summary>Monitors of <paramref name="display"/> (default display when null).</summary>
		public static IList<Gdk.Monitor> GetMonitors (Gdk.Display display = null)
		{
			var list = new List<Gdk.Monitor> ();
			var model = (display ?? Gdk.Display.Default)?.Monitors;
			if (model == null)
				return list;
			for (uint i = 0; i < model.NItems; i++) {
				if (GLib.Object.GetObject (model.GetItem (i), true) is Gdk.Monitor m)
					list.Add (m);
			}
			return list;
		}

		static int IndexOfMonitor (Gdk.Monitor monitor)
		{
			return GetMonitors (monitor.Display).IndexOf (monitor);
		}

		/// <summary>Runs <paramref name="dialog"/> modally (see <see cref="ModalRunner"/>), requesting user attention on Mac.</summary>
		public static int RunDialogWithNotification (Gtk.Dialog dialog)
		{
#if MAC
			MacRequestAttention (dialog.Modal);
#endif
			return dialog.RunModal ();
		}

		public static void PresentWindowWithNotification (this Gtk.Window window)
		{
			window.Present ();

#if MAC
			MacRequestAttention (window is Gtk.Dialog && window.Modal);
#endif
		}

		public static GLib.Value GetProperty (GLib.Object obj, string name)
		{
			return obj.GetProperty (name);
		}

		public static void SetProperty (GLib.Object obj, string name, GLib.Value value)
		{
			obj.SetProperty (name, value);
		}

		/// <summary>True for a single press that should open a context menu.</summary>
		public static bool TriggersContextMenu (this ClickInfo evt)
		{
			return evt != null && evt.NPress == 1 && IsContextMenuButton (evt);
		}

		/// <summary>Right button (no other button held), or Ctrl+left on Mac.</summary>
		public static bool IsContextMenuButton (this ClickInfo evt)
		{
			if (evt == null)
				return false;
			if (evt.Button == 3 &&
				(evt.State & (Gdk.ModifierType.Button1Mask | Gdk.ModifierType.Button2Mask)) == 0)
				return true;

			if (Platform.IsMac) {
				if (evt.Button == 1 &&
					(evt.State & Gdk.ModifierType.ControlMask) != 0 &&
					(evt.State & (Gdk.ModifierType.Button2Mask | Gdk.ModifierType.Button3Mask)) == 0)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>Current keyboard modifier state (GTK 4 names: Alt = AltMask, Command/Windows = MetaMask).</summary>
		public static Gdk.ModifierType GetCurrentKeyModifiers ()
		{
#if WIN32
			Gdk.ModifierType mtype = Gdk.ModifierType.None;
			ModifierKeys mod = Keyboard.Modifiers;
			if ((mod & ModifierKeys.Shift) > 0)
				mtype |= Gdk.ModifierType.ShiftMask;
			if ((mod & ModifierKeys.Control) > 0)
				mtype |= Gdk.ModifierType.ControlMask;
			if ((mod & ModifierKeys.Alt) > 0)
				mtype |= Gdk.ModifierType.AltMask;
			if ((mod & ModifierKeys.Windows) > 0)
				mtype |= Gdk.ModifierType.MetaMask;
			return mtype;
#elif MAC
			return GtkMacInterop.ConvertModifierMask (NSEvent.CurrentModifierFlags);
#else
			var kb = Gdk.Display.Default?.DefaultSeat?.Keyboard;
			return kb != null ? kb.ModifierState : Gdk.ModifierType.None;
#endif
		}

		/// <summary>
		/// Converts the deltas of a <see cref="Gtk.EventControllerScroll"/> Scroll signal to pixels: smooth (surface unit)
		/// deltas are already pixels, wheel steps are scaled to pageSize^(2/3) like GTK 2 did.
		/// </summary>
		public static void GetPageScrollPixelDeltas (this Gtk.EventControllerScroll controller, double dx, double dy, double pageSizeX, double pageSizeY,
			out double deltaX, out double deltaY)
		{
			if (controller != null && controller.Unit == Gdk.ScrollUnit.Surface) {
				deltaX = dx;
				deltaY = dy;
				return;
			}
			deltaX = pageSizeX != 0 ? dx * System.Math.Pow (pageSizeX, 2.0 / 3.0) : 0;
			deltaY = pageSizeY != 0 ? dy * System.Math.Pow (pageSizeY, 2.0 / 3.0) : 0;
		}

		public static void AddValueClamped (this Gtk.Adjustment adj, double value)
		{
			adj.Value = System.Math.Max (adj.Lower, System.Math.Min (adj.Value + value, adj.Upper - adj.PageSize));
		}

		/// <summary>Pops up <paramref name="menu"/> anchored to <paramref name="parent"/> at (x, y) in parent coordinates.</summary>
		public static void ShowContextMenu (Gtk.PopoverMenu menu, Gtk.Widget parent, int x, int y)
		{
			if (menu.Parent != parent) {
				if (menu.Parent != null)
					menu.Unparent ();
				menu.Parent = parent;
				menu.Closed += (sender, e) => GLib.Idle.Add (() => {
					if (menu.Handle != IntPtr.Zero && menu.Parent != null && !menu.Visible)
						menu.Unparent ();
					return false;
				});
			}
			menu.HasArrow = false;
			menu.Halign = Gtk.Align.Start;
			menu.PointingTo = new Gdk.Rectangle (x, y, 1, 1);
			menu.Popup ();
		}

		/// <summary>Pops up <paramref name="menu"/> at the click position, or at the parent's origin when <paramref name="evt"/> is null.</summary>
		public static void ShowContextMenu (Gtk.PopoverMenu menu, Gtk.Widget parent, ClickInfo evt)
		{
			ShowContextMenu (menu, parent, evt != null ? (int)evt.X : 0, evt != null ? (int)evt.Y : 0);
		}

		/// <summary>Pops up <paramref name="menu"/> at the caret rectangle (parent coordinates).</summary>
		public static void ShowContextMenu (Gtk.PopoverMenu menu, Gtk.Widget parent, Gdk.Rectangle caret)
		{
			ShowContextMenu (menu, parent, caret.X, caret.Y + caret.Height);
		}

		const Gdk.ModifierType accelMods = Gdk.ModifierType.ShiftMask | Gdk.ModifierType.AltMask
			| Gdk.ModifierType.ControlMask | Gdk.ModifierType.SuperMask | Gdk.ModifierType.MetaMask;

		/// <summary>Map raw GTK key input to work around platform bugs and decompose accelerator keys.</summary>
		/// <param name="controller">The key controller that received the key (gives display and layout group).</param>
		/// <param name="keyval">Keyval from <c>KeyPressed</c>.</param>
		/// <param name="keycode">Hardware keycode from <c>KeyPressed</c>.</param>
		/// <param name="modifiers">Modifier state from <c>KeyPressed</c>.</param>
		/// <param name='key'>The composed key</param>
		/// <param name='state'>The composed modifiers</param>
		/// <param name='shortcuts'>All the key/modifier decompositions that can be used as accelerators</param>
		public static void MapKeys (Gtk.EventControllerKey controller, uint keyval, uint keycode, Gdk.ModifierType modifiers,
			out Gdk.Key key, out Gdk.ModifierType state, out KeyboardShortcut[] shortcuts)
		{
			var display = controller?.Widget?.Display ?? Gdk.Display.Default;
			int group = controller != null ? (int)controller.Group : 0;
			MapKeys (display, keyval, keycode, modifiers, group, out key, out state, out shortcuts);
		}

		/// <summary>Map raw GTK key input to work around platform bugs and decompose accelerator keys.</summary>
		public static void MapKeys (Gdk.Display display, uint keyval, uint keycode, Gdk.ModifierType modifiers, int group,
			out Gdk.Key key, out Gdk.ModifierType state, out KeyboardShortcut[] shortcuts)
		{
			bool remapKey = Platform.IsWindows && keycode == 0;
			Gdk.ModifierType modifier = modifiers;

			//full key mapping
			uint kv;
			int effectiveGroup, level;
			Gdk.ModifierType consumedModifiers;
			TranslateKeyboardState (display, keycode, keyval, modifier, group, out kv, out effectiveGroup,
				out level, out consumedModifiers);
			key = (Gdk.Key)kv;
			state = modifiers & ~consumedModifiers;

			//decompose the key into accel combinations
			var accelList = new List<KeyboardShortcut> ();

			//all accels ignore the lock key
			modifier &= ~Gdk.ModifierType.LockMask;

			//fully decomposed
			TranslateKeyboardState (display, keycode, keyval, Gdk.ModifierType.None, 0,
				out kv, out effectiveGroup, out level, out consumedModifiers);
			accelList.Add (new KeyboardShortcut ((Gdk.Key)kv, modifier & accelMods));

			//with shift composed
			if ((modifier & Gdk.ModifierType.ShiftMask) != 0) {
				if (display == null || !display.TranslateKey (keycode, Gdk.ModifierType.ShiftMask, 0,
					out kv, out effectiveGroup, out level, out consumedModifiers)) {
					kv = keyval;
					consumedModifiers = Gdk.ModifierType.None;
				}

				if (Platform.IsWindows && keycode == 0) {
					kv = keyval;
				}

				// Prevent consumption of non-Shift modifiers (that we didn't even provide!)
				consumedModifiers &= Gdk.ModifierType.ShiftMask;

				var m = (modifier & ~consumedModifiers) & accelMods;
				AddIfNotDuplicate (accelList, new KeyboardShortcut ((Gdk.Key)kv, m));
			}

			//with group 1 composed
			if (group == 1) {
				TranslateKeyboardState (display, keycode, keyval, modifier & ~Gdk.ModifierType.ShiftMask, 1,
					out kv, out effectiveGroup, out level, out consumedModifiers);

				// Prevent consumption of Shift modifier (that we didn't even provide!)
				consumedModifiers &= ~Gdk.ModifierType.ShiftMask;

				var m = (modifier & ~consumedModifiers) & accelMods;
				AddIfNotDuplicate (accelList, new KeyboardShortcut ((Gdk.Key)kv, m));
			}

			//with group 1 and shift composed
			if (group == 1 && (modifier & Gdk.ModifierType.ShiftMask) != 0) {
				TranslateKeyboardState (display, keycode, keyval, modifier, 1,
					out kv, out effectiveGroup, out level, out consumedModifiers);
				var m = (modifier & ~consumedModifiers) & accelMods;
				AddIfNotDuplicate (accelList, new KeyboardShortcut ((Gdk.Key)kv, m));
			}

			//and also allow the fully mapped key as an accel
			AddIfNotDuplicate (accelList, new KeyboardShortcut (key, state & accelMods));

			shortcuts = accelList.ToArray ();

			if (remapKey) {
				key = (Gdk.Key)keyval;
			}
		}

		// Workaround for bug "Bug 688247 - Ctrl+Alt key not work on windows7 with bootcamp on a Mac Book Pro"
		// Ctrl+Alt should behave like right alt key - unfortunately TranslateKeyboardState doesn't handle it.
		static void TranslateKeyboardState (Gdk.Display display, uint hardware_keycode, uint keyval, Gdk.ModifierType state, int group, out uint resultKeyval,
			out int effective_group, out int level, out Gdk.ModifierType consumed_modifiers)
		{
			if (Platform.IsWindows) {
				const Gdk.ModifierType ctrlAlt = Gdk.ModifierType.ControlMask | Gdk.ModifierType.AltMask;
				if ((state & ctrlAlt) == ctrlAlt) {
					state = (state & ~ctrlAlt);
					group = 1;
				}
				// Case: Caps lock on + shift + key
				// See: Bug 8069 - [UI Refresh] If caps lock is on, holding the shift key prevents typed characters from appearing
				if (state.HasFlag (Gdk.ModifierType.ShiftMask)) {
					state &= ~Gdk.ModifierType.ShiftMask;
				}
			}

			if (display == null || !display.TranslateKey (hardware_keycode, state, group, out resultKeyval, out effective_group,
				out level, out consumed_modifiers)) {
				resultKeyval = keyval;
				effective_group = group;
				level = 0;
				consumed_modifiers = Gdk.ModifierType.None;
			}

			if (Platform.IsWindows && hardware_keycode == 0) {
				resultKeyval = keyval;
			}
		}

		public static Gdk.Key[] KeysForMod (Gdk.ModifierType mod)
		{
			switch (mod) {
			case Gdk.ModifierType.ControlMask:
				return new Gdk.Key [] { Gdk.Key.Control_R, Gdk.Key.Control_L };
			case Gdk.ModifierType.AltMask:
				return new Gdk.Key [] { Gdk.Key.Alt_R, Gdk.Key.Alt_L };
			case Gdk.ModifierType.ShiftMask:
				return new Gdk.Key [] { Gdk.Key.Shift_R, Gdk.Key.Shift_L };
			case Gdk.ModifierType.MetaMask:
			case Gdk.ModifierType.SuperMask:
				// Just in case we get both of the modifiers
			case Gdk.ModifierType.SuperMask | Gdk.ModifierType.MetaMask:
				return new Gdk.Key [] { Gdk.Key.Meta_R, Gdk.Key.Meta_L, Gdk.Key.Super_R, Gdk.Key.Super_L };
			}
			return new Gdk.Key [0];
		}

		static void AddIfNotDuplicate<T> (List<T> list, T item) where T : IEquatable<T>
		{
			if (!list.Contains (item))
				list.Add (item);
		}

		/// <summary>Sets the IM cursor rectangle (in <paramref name="clientWidget"/> coordinates; the IM context's client widget).</summary>
		public static void SetImCursorLocation (Gtk.IMContext ctx, Gtk.Widget clientWidget, Gdk.Rectangle cursor)
		{
			ctx.CursorLocation = cursor;
		}

		/// <summary>X coordinate of the pixels inside the right edge of the rectangle</summary>
		/// <remarks>Workaround for inconsistency of Right property between GTK# versions</remarks>
		public static int RightInside (this Gdk.Rectangle rect)
		{
			return rect.X + rect.Width - 1;
		}

		/// <summary>Y coordinate of the pixels inside the bottom edge of the rectangle</summary>
		/// <remarks>Workaround for inconsistency of Bottom property between GTK# versions#</remarks>
		public static int BottomInside (this Gdk.Rectangle rect)
		{
			return rect.Y + rect.Height - 1;
		}

		/// <summary>
		/// Shows or hides the shadow of the window rendered by the native toolkit
		/// </summary>
		public static void ShowNativeShadow (Gtk.Window window, bool show)
		{
#if MAC
			var nsWindow = GtkMacInterop.GetNSWindow (window);
			if (nsWindow != null)
				nsWindow.HasShadow = show;
#endif
		}

		public static void UpdateNativeShadow (Gtk.Window window)
		{
#if MAC
			var nsWindow = GtkMacInterop.GetNSWindow (window);
			if (nsWindow != null)
				nsWindow.InvalidateShadow ();
#endif
		}

		/// <summary>No-op: the GTK# 2 container resurrection leak (BXC #3801) does not exist with GTK 4 widgets.</summary>
		public static void FixContainerLeak (Gtk.Widget c)
		{
		}

		const string urlRegexStr = @"(http|ftp)s?\:\/\/[\w\d\.,;_/\-~%@()+:?&^=#!]*[\w\d/]";
		static readonly Regex UrlRegex  = new Regex (urlRegexStr, RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		public static string MarkupLinks (string text)
		{
			return UrlRegex.Replace (text, MatchToUrl);
		}

		static string MatchToUrl (System.Text.RegularExpressions.Match m)
		{
			var s = m.ToString ();
			return String.Format ("<a href='{0}'>{1}</a>", s, s.Replace ("_", "__"));
		}

		public static void SetLinkHandler (this Gtk.Label label, Action<string> urlHandler)
		{
			label.ActivateLink += (sender, args) => {
				// GtkSharp's ActivateLinkArgs has no Uri accessor: the uri is the first signal argument.
				urlHandler ((string)args.Args [0]);
				args.RetVal = true;
			};
		}

		/// <summary>Sets the scrollbar policies (GTK 4 scrollbars are overlay scrollbars by default).</summary>
		public static void SetOverlayScrollbarPolicy (Gtk.ScrolledWindow sw, Gtk.PolicyType hpolicy, Gtk.PolicyType vpolicy)
		{
			sw.OverlayScrolling = true;
			sw.SetPolicy (hpolicy, vpolicy);
		}

		public static void GetOverlayScrollbarPolicy (Gtk.ScrolledWindow sw, out Gtk.PolicyType hpolicy, out Gtk.PolicyType vpolicy)
		{
			sw.GetPolicy (out hpolicy, out vpolicy);
		}

		/// <summary>Managed per-object data (the GTK 2 version wrapped g_object_get_data for patched GTK features).</summary>
		public static IntPtr GetData (GLib.Object o, string name)
		{
			return o.Data [name] is IntPtr p ? p : IntPtr.Zero;
		}

		public static void SetData (GLib.Object o, string name, IntPtr dataHandle)
		{
			o.Data [name] = dataHandle;
		}

		/// <summary>Always null: GTK 4 has no "gdk-pixbuf-2x-variant" (hi-dpi is handled by textures/scale factor).</summary>
		public static Gdk.Pixbuf Get2xVariant (Gdk.Pixbuf px)
		{
			return null;
		}

		public static void Set2xVariant (Gdk.Pixbuf px, Gdk.Pixbuf variant2x)
		{
		}

		public static double GetScaleFactor (Gtk.Widget w)
		{
			return w != null ? w.ScaleFactor : GetScaleFactor ();
		}

		public static double GetScaleFactor (this Gdk.Monitor monitor)
		{
			return monitor != null && monitor.Scale > 0 ? monitor.Scale : 1;
		}

		/// <summary>Scale factor of the primary (first) monitor.</summary>
		public static double GetScaleFactor ()
		{
			var monitors = GetMonitors ();
			return monitors.Count > 0 ? GetScaleFactor (monitors [0]) : 1;
		}

		/// <summary>No-op (GTK 2 Mac patch hint).</summary>
		public static void SetTransparentBgHint (this Widget widget, bool enable)
		{
		}

		public static void SetMarkup (this Gtk.TextView view, string pangoMarkup)
		{
			view.Buffer.Clear ();
			var iter = view.Buffer.StartIter;
			view.Buffer.InsertMarkup (ref iter, pangoMarkup);
		}

		/// <summary>Replaces the buffer content with <paramref name="pangoMarkup"/> (same behaviour as the GTK 2 version).</summary>
		public static void InsertMarkup (this Gtk.TextBuffer buffer, TextIter iter, string pangoMarkup)
		{
			buffer.Clear ();
			iter = buffer.StartIter;
			buffer.InsertMarkup (ref iter, pangoMarkup);
		}

		public static bool IsChildOf (this Gtk.Widget child, Gtk.Widget widget)
		{
			var parent = child.Parent;
			while (parent != null) {
				if (parent == widget)
					return true;
				parent = parent.Parent;
			};
			return false;
		}

#if MAC
		static void OnMappedDisableButtons (object sender, EventArgs args)
		{
			var window = (Gtk.Window)sender;

			DisableButtonsInternal (window);

			window.Mapped -= OnMappedDisableButtons;
		}

		static void DisableButtonsInternal (Gtk.Window window)
		{
			NSWindow nsWindow = GtkMacInterop.GetNSWindow (window);
			if (nsWindow == null)
				return;

			nsWindow.StandardWindowButton (NSWindowButton.MiniaturizeButton).Enabled = false;
			nsWindow.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = false;
		}
#endif

		public static void DisableMinimizeMaximizeButtons (Gtk.Window window)
		{
#if MAC
			if (window.IsMapped) {
				DisableButtonsInternal (window);
				return;
			}

			window.Mapped += OnMappedDisableButtons;
#endif
		}

		/// <summary>No-op: GTK 4 accessibility tracks children itself (was a Mac GTK 2 a11y workaround).</summary>
		public static void EmitAddSignal (Gtk.Widget container, Gtk.Widget child)
		{
		}

		/// <summary>No-op: GTK 4 accessibility tracks children itself (was a Mac GTK 2 a11y workaround).</summary>
		public static void EmitRemoveSignal (Gtk.Widget container, Gtk.Widget child)
		{
		}

		internal static bool IsRunFromBundle ()
		{
			bool result = false;
#if MAC
			IntPtr mainBundle = GetMainBundle ();
			var sel_bundleIdentifier = ObjCRuntime.Selector.GetHandle ("bundleIdentifier");

			result = Messaging.IntPtr_objc_msgSend (mainBundle, sel_bundleIdentifier) != IntPtr.Zero;

			static IntPtr GetMainBundle ()
			{
				var class_runningApplication = ObjCRuntime.Class.GetHandle ("NSBundle");
				var sel_mainBundle = ObjCRuntime.Selector.GetHandle ("mainBundle");

				return Messaging.IntPtr_objc_msgSend (class_runningApplication, sel_mainBundle);
			}
#endif
			return result;
		}
	}

	public readonly struct KeyboardShortcut : IEquatable<KeyboardShortcut>
	{
		public static readonly KeyboardShortcut Empty = new KeyboardShortcut ((Gdk.Key) 0, (Gdk.ModifierType) 0);

		public KeyboardShortcut (Gdk.Key key, Gdk.ModifierType modifier)
		{
			this.Modifier = modifier;
			this.Key = key;
		}

		public Gdk.Key Key { get; }

		public Gdk.ModifierType Modifier { get; }

		public bool IsEmpty {
			get { return Key == (Gdk.Key) 0; }
		}

		public override bool Equals (object obj)
		{
			return obj is KeyboardShortcut && this.Equals ((KeyboardShortcut) obj);
		}

		public override int GetHashCode ()
		{
			//FIXME: we're only using a few bits of mod and mostly the lower bits of key - distribute it better
			return (int) Key ^ (int) Modifier;
		}

		public bool Equals (KeyboardShortcut other)
		{
			return other.Key == Key && other.Modifier == Modifier;
		}
	}

}
