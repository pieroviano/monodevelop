//
// AtkCocoaHelper.cs
//
// Author:
//       Iain Holmes <iain@xamarin.com>
//
// Copyright (c) 2016 Xamarin, Inc
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
#nullable enable

using System;
using System.Collections.Generic;

#if MAC
using AppKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#endif

namespace MonoDevelop.Components.AtkCocoaHelper
{
	public static class AtkCocoaExtensions
	{
		// Atk is gone in GTK 4: every Gtk.Widget is a Gtk.IAccessible. The name (an automation identifier)
		// has no settable GTK 4 counterpart (accessible-id is read-only) and is ignored.
		public static void SetCommonAttributes (this Gtk.IAccessible o, string? name, string? label, string? help)
		{
			if (!string.IsNullOrEmpty (label)) {
				GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.Label, label);
			}
			if (!string.IsNullOrEmpty (help)) {
				GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.Description, help);
			}
		}

		public static void SetCommonAttributes (this Xwt.Accessibility.Accessible o, string? name, string? label, string? help)
		{
			if (!string.IsNullOrEmpty (name)) {
				o.Identifier = name;
			}

			if (!string.IsNullOrEmpty (label)) {
				o.Label = label;
			}

			if (!string.IsNullOrEmpty (help)) {
				o.Description = help;
			}
		}

		public static void SetCommonAccessibilityAttributes (this Xwt.Widget w, string? name, Xwt.Widget? label, string? help)
		{
			w.Accessible.SetCommonAttributes (name, null, help);
			if (label != null) {
				// FIXME Add relationship to Xwt
			}
		}

		public static void SetCommonAccessibilityAttributes (this Xwt.Widget w, string? name, string? label, string? help)
		{
			w.Accessible.SetCommonAttributes (name, label, help);
		}

		public static void SetCommonAccessibilityAttributes (this Gtk.Widget w, string? name, string? label, string? help)
		{
			w.SetCommonAttributes (name, label, help);
		}

		public static void SetCommonAccessibilityAttributes (this Gtk.Widget w, string? name, Gtk.Widget? label, string? help)
		{
			w.SetCommonAttributes (name, null, help);

			if (label != null) {
				w.SetAccessibilityLabelRelationship (label);
			}
		}

		public static void SetAccessibilityLabelRelationship (this Gtk.Widget w, Gtk.Widget label)
		{
			GtkAccessibleHelper.SetRelation (w, Gtk.AccessibleRelation.LabelledBy, label);
		}

		public static void SetCurrentFocus (AccessibilityElementProxy focusElement)
		{
#if MAC
			AtkCocoaMacExtensions.SetCurrentFocus (focusElement);
#endif
		}
	}

	// GTK 4 Gtk.Accessible plumbing shared by the non-Mac extension methods.
	// Gtk.AccessibleExtensions is called statically: a `using Gtk;` would make EventHandler ambiguous.
	static class GtkAccessibleHelper
	{
		public static void SetProperty (Gtk.IAccessible? o, Gtk.AccessibleProperty property, object? value)
		{
			if (o == null)
				return;
			if (value == null) {
				o.ResetProperty (property);
				return;
			}
			// InitValue gives the exact GType GTK expects for the property.
			GLib.Value boxed = Gtk.AccessibleExtensions.InitValue (property);
			try {
				boxed.Val = value;
				Gtk.AccessibleExtensions.UpdateProperty (o, property, boxed);
			} finally {
				boxed.Dispose ();
			}
		}

		public static void SetState (Gtk.IAccessible? o, Gtk.AccessibleState state, bool value)
		{
			if (o == null)
				return;
			GLib.Value boxed = new GLib.Value (value);
			try {
				Gtk.AccessibleExtensions.UpdateState (o, state, boxed);
			} finally {
				boxed.Dispose ();
			}
		}

		public static void SetRelation (Gtk.IAccessible? o, Gtk.AccessibleRelation relation, params Gtk.IAccessible? [] targets)
		{
			if (o == null || targets == null)
				return;
			var list = new List<Gtk.IAccessible> ();
			foreach (var t in targets) {
				if (t != null)
					list.Add (t);
			}
			if (list.Count == 0) {
				o.ResetRelation (relation);
				return;
			}
			Gtk.AccessibleExtensions.UpdateRelation (o, relation, list.ToArray ());
		}

		// GTK 4 only lets the role change before the accessible context is realized.
		public static void SetRole (Gtk.IAccessible? o, Gtk.AccessibleRole role)
		{
			if (o is Gtk.Widget w && w.IsRealized)
				return;
			if (o != null && o.AccessibleRole != role)
				o.AccessibleRole = role;
		}

		public static Gtk.AccessibleRole? ToGtkRole (AtkCocoa.Roles role)
		{
			switch (role) {
			case AtkCocoa.Roles.AXButton: return Gtk.AccessibleRole.Button;
			case AtkCocoa.Roles.AXCell: return Gtk.AccessibleRole.Cell;
			case AtkCocoa.Roles.AXColumn: return Gtk.AccessibleRole.ColumnHeader;
			case AtkCocoa.Roles.AXGroup: return Gtk.AccessibleRole.Group;
			case AtkCocoa.Roles.AXImage: return Gtk.AccessibleRole.Img;
			case AtkCocoa.Roles.AXLink: return Gtk.AccessibleRole.Link;
			case AtkCocoa.Roles.AXList: return Gtk.AccessibleRole.List;
			case AtkCocoa.Roles.AXMenuButton: return Gtk.AccessibleRole.Button;
			case AtkCocoa.Roles.AXPopUpButton: return Gtk.AccessibleRole.ComboBox;
			case AtkCocoa.Roles.AXRadioButton: return Gtk.AccessibleRole.Radio;
			case AtkCocoa.Roles.AXRow: return Gtk.AccessibleRole.Row;
			case AtkCocoa.Roles.AXSplitter: return Gtk.AccessibleRole.Separator;
			case AtkCocoa.Roles.AXStaticText: return Gtk.AccessibleRole.Label;
			case AtkCocoa.Roles.AXTabGroup: return Gtk.AccessibleRole.TabList;
			case AtkCocoa.Roles.AXTextArea: return Gtk.AccessibleRole.TextBox;
			default: return null;
			}
		}
	}

	// AtkCocoaHelper wraps NSAccessibilityElement to set NSAccessibility properties that aren't supported by Atk
	public static class AtkCocoa
	{
		public enum Actions
		{
			AXCancel,
			AXConfirm,
			AXDecrement,
			AXDelete,
			AXIncrement,
			AXPick,
			AXPress,
			AXRaise,
			AXShowAlternateUI,
			AXShowDefaultUI,
			AXShowMenu
		};

		public enum Roles
		{
			AXButton,
			AXCell,
			AXColumn,
			AXGroup,
			AXGrowArea,
			AXImage,
			AXLink,
			AXList,
			AXMenuButton,
			AXPopUpButton,
			AXRadioButton,
			AXRow,
			AXRuler,
			AXSplitGroup,
			AXSplitter,
			AXStaticText,
			AXTabGroup,
			AXTextArea
		};

		public enum SubRoles
		{
			AXCloseButton,
			AXTabButton,
		};

		public struct Range
		{
			public int Location { get; set; }
			public int Length { get; set; }
		}
	}

	// GTK 4 has no AT-SPI/NSAccessibility action hooks (the Atk "perform-*" signals were a MonoDevelop
	// GTK 2 patch). The class keeps its shape so callers compile; the events are never raised off Mac.
	public class ActionDelegate
	{
		public ActionDelegate (Gtk.Widget widget)
		{
		}

#pragma warning disable 67 // never used
		public event EventHandler? PerformCancel;
		public event EventHandler? PerformConfirm;
		public event EventHandler? PerformDecrement;
		public event EventHandler? PerformDelete;
		public event EventHandler? PerformIncrement;
		public event EventHandler? PerformPick;
		public event EventHandler? PerformPress;
		public event EventHandler? PerformRaise;
		public event EventHandler? PerformShowAlternateUI;
		public event EventHandler? PerformShowDefaultUI;
		public event EventHandler? PerformShowMenu;
#pragma warning restore 67
	}

	// On anything other than Mac this is just a dummy class to prevent needing to have #ifdefs all over the main code
	public interface IAccessibilityElementProxy
	{
		event EventHandler PerformCancel;
		event EventHandler PerformConfirm;
		event EventHandler PerformDecrement;
		event EventHandler PerformDelete;
		event EventHandler PerformIncrement;
		event EventHandler PerformPick;
		event EventHandler PerformPress;
		event EventHandler PerformRaise;
		event EventHandler PerformShowAlternateUI;
		event EventHandler PerformShowDefaultUI;
		event EventHandler PerformShowMenu;

		Gtk.Widget GtkParent { get; set; }
		Gdk.Rectangle FrameInGtkParent { get; set; }
		Gdk.Rectangle FrameInParent { get; set; }

		void AddAccessibleChild (IAccessibilityElementProxy child);
		void RemoveAccessibleChild (IAccessibilityElementProxy child);

		void SetRole (string role, string? description = null);
		void SetRole (AtkCocoa.Roles role, string? description = null);

		string Value { get; set; }
		string Title { get; set; }
		string Label { get; set; }
		string Identifier { get; set; }
		string Help { get; set; }
		bool Hidden { get; set; }
		int Index { get; set; }
		bool Focused { get; set; }

		// For Navigable Static Text
		Func<string> Contents { set; }
		Func<int> NumberOfCharacters { set; }
		Func<int> InsertionPointLineNumber { set; }
		Func<AtkCocoa.Range, Gdk.Rectangle> FrameForRange { set; }
		Func<int, int> LineForIndex { set; }
		Func<int, AtkCocoa.Range> RangeForLine { set; }
		Func<AtkCocoa.Range, string> StringForRange { set; }
		Func<int, AtkCocoa.Range> RangeForIndex { set; }
		Func<int, AtkCocoa.Range> StyleRangeForIndex { set; }
		Func<Gdk.Point, AtkCocoa.Range> RangeForPosition { set; }
	}
}
