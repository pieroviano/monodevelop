// LanguageItemWindow.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Gtk;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Core;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.Ide;

namespace MonoDevelop.SourceEditor
{
	/// <summary>
	/// Tooltip with the text of a language item (error, quick fix...). The tooltip is a popover (GTK 4 cannot position
	/// toplevel windows); this wrapper makes it usable as a <see cref="MonoDevelop.Components.Window"/> for the
	/// <see cref="MonoDevelop.Ide.Editor.TooltipProvider"/> API. Show it with <see cref="ShowPopup"/>.
	/// </summary>
	class LanguageItemWindow : MonoDevelop.Components.Window
	{
		readonly LanguageItemPopover popover;

		public bool IsEmpty { get; set; }
		public object Tag { get; set; }

		/// <summary>Raised when the tooltip is closed.</summary>
		public event EventHandler Destroyed;

		public LanguageItemWindow (ExtensibleTextEditor ed, Xwt.ModifierKeys modifierState, object result, string errorInformations, object unit)
			: this (new LanguageItemPopover (), errorInformations)
		{
		}

		LanguageItemWindow (LanguageItemPopover popover, string errorInformations) : base (popover)
		{
			this.popover = popover;
			popover.Owner = this;
			string tooltip = errorInformations;
			if (string.IsNullOrEmpty (tooltip)|| tooltip == "?") {
				IsEmpty = true;
				return;
			}

			var label = new MonoDevelop.Components.FixedWidthWrapLabel () {
				Wrap = Pango.WrapMode.WordChar,
				Indent = -20,
				BreakOnCamelCasing = true,
				BreakOnPunctuation = true,
				Markup = tooltip,
			};
			label.MarginTop = label.MarginBottom = label.MarginStart = label.MarginEnd = 3;
			popover.Child = label;
			UpdateFont (label);
			
			popover.EnableTransparencyControl = true;
		}

		/// <summary>The popover this window wraps.</summary>
		public MonoDevelop.Components.TooltipWindow Popover => popover;

		/// <summary>Anchors the tooltip to <paramref name="anchor"/>, pointing at <paramref name="area"/> (anchor coordinates), and shows it.</summary>
		public void ShowPopup (Gtk.Widget anchor, Gdk.Rectangle area)
		{
			popover.ShowPopup (anchor, area);
		}

		/// <summary>Closes the tooltip.</summary>
		public void Destroy ()
		{
			popover.Destroy ();
		}
		
		//return the real width
		public int SetMaxWidth (int maxWidth)
		{
			var label = popover.Child as MonoDevelop.Components.FixedWidthWrapLabel;
			if (label == null)
				return popover.Width;
			label.MaxWidth = maxWidth;
			return label.RealWidth;
		}
		

		void UpdateFont (MonoDevelop.Components.FixedWidthWrapLabel label)
		{
			if (label == null)
				return;
			label.FontDescription = IdeServices.FontService.GetFontDescription ("Pad");
			
		}

		sealed class LanguageItemPopover : MonoDevelop.Components.TooltipWindow
		{
			public LanguageItemWindow Owner { get; set; }

			bool destroyed;

			public override void Destroy ()
			{
				if (!destroyed) {
					destroyed = true;
					Owner?.Destroyed?.Invoke (Owner, EventArgs.Empty);
				}
				base.Destroy ();
			}
		}
	}
}
