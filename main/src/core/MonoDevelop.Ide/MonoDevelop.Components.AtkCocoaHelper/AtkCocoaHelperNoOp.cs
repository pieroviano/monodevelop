//
// AtkCocoaHelperNoOp.cs
//
// Author:
//       iain <iaholmes@microsoft.com>
//
// Copyright (c) 2017 
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

#if !MAC

using System;

namespace MonoDevelop.Components.AtkCocoaHelper
{
	// Non-Mac: maps onto GTK 4 Gtk.Accessible where GTK has an equivalent; NSAccessibility-only concepts are no-ops.
	public static class AtkCocoaNoopExtensions
	{
		public static void SetLabel (this Gtk.IAccessible o, string label)
		{
			GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.Label, label);
		}

		public static void SetLabel (this Gtk.CellRenderer r, string label)
		{
		}

		public static void SetDescription (this Gtk.CellRenderer r, string description)
		{
		}

		public static void SetShouldIgnore (this Gtk.IAccessible o, bool ignore)
		{
		}

		public static void SetTitle (this Gtk.IAccessible o, string title)
		{
		}

		public static void SetDocument (this Gtk.IAccessible o, string documentUrl)
		{
		}

		public static void SetFilename (this Gtk.IAccessible o, string filename)
		{
		}

		public static void SetIsMainWindow (this Gtk.IAccessible o, bool isMainWindow)
		{
		}

		public static void SetMainWindow (this Gtk.IAccessible o, Gtk.IAccessible mainWindow)
		{
		}

		public static void SetValue (this Gtk.IAccessible o, string stringValue)
		{
			GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.ValueText, stringValue);
		}

		public static void SetUrl (this Gtk.IAccessible o, string url)
		{
		}

		public static void SetRole (this Gtk.IAccessible o, string role, string description = null)
		{
			// NSAccessibility role strings have no GTK 4 mapping; only the description survives.
			if (!string.IsNullOrEmpty (description))
				GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.RoleDescription, description);
		}

		public static void SetRole (this Gtk.IAccessible o, AtkCocoa.Roles role, string description = null)
		{
			var gtkRole = GtkAccessibleHelper.ToGtkRole (role);
			if (gtkRole.HasValue)
				GtkAccessibleHelper.SetRole (o, gtkRole.Value);
			if (!string.IsNullOrEmpty (description))
				GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.RoleDescription, description);
		}

		public static void SetSubRole (this Gtk.IAccessible o, string subrole)
		{
		}

		public static void SetSubRole (this Gtk.IAccessible o, AtkCocoa.SubRoles subrole)
		{
		}

		public static void SetValue (this Gtk.IAccessible o, object value)
		{
			GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.ValueText, value?.ToString ());
		}

		public static void SetHidden (this Gtk.IAccessible o, bool hidden)
		{
			GtkAccessibleHelper.SetState (o, Gtk.AccessibleState.Hidden, hidden);
		}

		public static void SetCurrentFocus (this Gtk.IAccessible o)
		{
		}

		public static void SetTitleUIElement (this Gtk.IAccessible o, Gtk.IAccessible title)
		{
			GtkAccessibleHelper.SetRelation (o, Gtk.AccessibleRelation.LabelledBy, title);
		}

		public static void SetAlternateUIVisible (this Gtk.IAccessible o, bool visible)
		{
		}

		public static void SetOrientation (this Gtk.IAccessible o, Gtk.Orientation orientation)
		{
			GtkAccessibleHelper.SetProperty (o, Gtk.AccessibleProperty.Orientation, orientation);
		}

		public static void SetTitleFor (this Gtk.IAccessible o, params Gtk.IAccessible [] objects)
		{
			if (objects == null)
				return;
			foreach (var labelled in objects)
				GtkAccessibleHelper.SetRelation (labelled, Gtk.AccessibleRelation.LabelledBy, o);
		}

		public static void SetTabs (this Gtk.IAccessible o, AccessibilityElementProxy [] tabs)
		{
		}

		public static void SetTabs (this Gtk.IAccessible o, Gtk.IAccessible [] tabs)
		{
		}

		public static void AddElementToTitle (this Gtk.IAccessible title, Gtk.IAccessible o)
		{
		}

		public static void RemoveElementFromTitle (this Gtk.IAccessible title, Gtk.IAccessible o)
		{
		}

		public static void ReplaceAccessibilityElements (this Gtk.IAccessible parent, AccessibilityElementProxy [] children)
		{
		}

		public static void SetColumns (this Gtk.IAccessible parent, AccessibilityElementProxy [] columns)
		{
		}

		public static void SetRows (this Gtk.IAccessible parent, AccessibilityElementProxy [] rows)
		{
		}

		public static void AddAccessibleElement (this Gtk.IAccessible o, AccessibilityElementProxy child)
		{
		}

		public static void RemoveAccessibleElement (this Gtk.IAccessible o, AccessibilityElementProxy child)
		{
		}

		public static void SetAccessibleChildren (this Gtk.IAccessible o, AccessibilityElementProxy [] children)
		{
		}

		public static void AddLinkedUIElement (this Gtk.IAccessible o, Gtk.IAccessible linked)
		{
		}

		public static void AddLinkedUIElement (this Gtk.IAccessible o, params Gtk.IAccessible [] linked)
		{
		}
	
		public static void MakeAccessibilityAnnouncement (this Gtk.IAccessible o,  string message)
		{
			if (o != null && !string.IsNullOrEmpty (message))
				o.Announce (message, Gtk.AccessibleAnnouncementPriority.Medium);
		}

		public static void TransferAccessibleChild (this Gtk.IAccessible from, Gtk.IAccessible to, Gtk.IAccessible child)
		{
		}
	}

	public class AccessibilityElementProxy : IAccessibilityElementProxy
	{
		public AccessibilityElementProxy ()
		{
		}

		public static AccessibilityElementProxy ButtonElementProxy ()
		{
			return new AccessibilityElementProxy ();
		}

		public static AccessibilityElementProxy TextElementProxy ()
		{
			return new AccessibilityElementProxy ();
		}

		public static bool Enabled {
			get {
				return false;
			}
		}
		
		public string Identifier {
			get {
				return null;
			}
			set {
			}
		}

		public string Help {
			get {
				return null;
			}
			set {
			}
		}

		public string Label {
			get {
				return null;
			}
			set {
			}
		}

		public string Title {
			get {
				return null;
			}
			set {
			}
		}

		public string Value {
			get {
				return null;
			}
			set {
			}
		}

		public bool Hidden {
			get {
				return false;
			}
			set {
			}
		}

		public int Index {
			get {
				return 0;
			}
			set {
			}
		}

		public bool Focused {
			get {
				return false;
			}
			set {
			}
		}

		public Gtk.Widget GtkParent { get; set; }
		public Gdk.Rectangle FrameInGtkParent { get; set; }
		public Gdk.Rectangle FrameInParent { get; set; }

		public void AddAccessibleChild (IAccessibilityElementProxy child)
		{
		}

		public void RemoveAccessibleChild (IAccessibilityElementProxy child)
		{
		}

		public void SetRole (string role, string description = null)
		{
		}

		public void SetRole (AtkCocoa.Roles role, string description = null)
		{
		}

#pragma warning disable 67 // never used
		public event EventHandler PerformCancel;
		public event EventHandler PerformConfirm;
		public event EventHandler PerformDecrement;
		public event EventHandler PerformDelete;
		public event EventHandler PerformIncrement;
		public event EventHandler PerformPick;
		public event EventHandler PerformPress;
		public event EventHandler PerformRaise;
		public event EventHandler PerformShowAlternateUI;
		public event EventHandler PerformShowDefaultUI;
		public event EventHandler PerformShowMenu;
#pragma warning restore 67

		// For Navigable Text elements
		public Func<string> Contents {
			set {
			}
		}
		public Func<int> NumberOfCharacters {
			set {
			}
		}
		public Func<int> InsertionPointLineNumber {
			set {
			}
		}
		public Func<AtkCocoa.Range, Gdk.Rectangle> FrameForRange {
			set {
			}
		}
		public Func<int, int> LineForIndex {
			set {
			}
		}
		public Func<int, AtkCocoa.Range> RangeForLine {
			set {
			}
		}
		public Func<AtkCocoa.Range, string> StringForRange {
			set {
			}
		}
		public Func<int, AtkCocoa.Range> RangeForIndex {
			set {
			}
		}
		public Func<int, AtkCocoa.Range> StyleRangeForIndex {
			set {
			}
		}
		public Func<Gdk.Point, AtkCocoa.Range> RangeForPosition {
			set {
			}
		}
		
		public Func<AtkCocoa.Range> GetVisibleCharacterRange {
			set {
			}
		}
	}
}

#endif
