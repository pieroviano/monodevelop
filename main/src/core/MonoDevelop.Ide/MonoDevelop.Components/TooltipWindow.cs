// 
// TooltipWindow.cs
// 
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Ide;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Tooltip popup. GTK 4 cannot position toplevels: this is a non-modal <see cref="Gtk.Popover"/>; set its
	/// <c>Child</c> and show it with <see cref="ShowPopup"/> (anchor widget + area in anchor coordinates).
	/// </summary>
	public abstract class TooltipWindow : Gtk.Popover
	{
		WindowTransparencyDecorator decorator;
		
		public string LinkColor {
			get {
				HslColor color = Ide.Gui.Styles.LinkForegroundColor;
				return color.ToPangoString ();
			}
		}
		
		protected TooltipWindow ()
		{
			Autohide = false;
			HasArrow = false;
			CanFocus = false;
		}
		
		/// <summary>GTK 4 keeps popovers on screen itself; kept for API compatibility.</summary>
		public bool NudgeVertical {
			get; set;
		}
		
		/// <summary>GTK 4 keeps popovers on screen itself; kept for API compatibility.</summary>
		public bool NudgeHorizontal {
			get; set;
		}
		
		public bool EnableTransparencyControl {
			get { return decorator != null; }
			set {
				if (value && decorator == null)
					decorator = WindowTransparencyDecorator.Attach (this);
				else if (!value && decorator != null) {
					decorator.Detach ();
					decorator = null;
				}
			}
		}

		/// <summary>Anchors the tooltip to <paramref name="anchor"/>, pointing at <paramref name="area"/> (anchor coordinates), and shows it.</summary>
		public void ShowPopup (Gtk.Widget anchor, Gdk.Rectangle area, Gtk.PositionType position = Gtk.PositionType.Bottom)
		{
			if (anchor == null)
				throw new ArgumentNullException (nameof (anchor));
			this.Reparent (anchor);
			this.ShowAt (area, position);
		}

		/// <summary>Pops the tooltip down and detaches it from its anchor.</summary>
		public override void Destroy ()
		{
			if (decorator != null) {
				decorator.Detach ();
				decorator = null;
			}
			if (Handle != IntPtr.Zero && Visible)
				Popdown ();
			base.Destroy ();
		}
	}
}
