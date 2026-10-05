//
// GtkWidgetResult.cs
//
// Author:
//       iain holmes <iain@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Gtk;

namespace MonoDevelop.Components.AutoTest.Results
{
	// GTK 4 cannot synthesize input events (gdk_event_put is gone), so clicks and key presses are
	// emulated by activating widgets, editing text directly and triggering matching shortcuts.
	public class GtkWidgetResult : AppResult
	{
		Widget resultWidget;

		internal GtkWidgetResult (Widget widget)
		{
			resultWidget = widget;
		}

		public override string ToString ()
		{
			return String.Format ("{0} - {1} - {2} - {3}, - {4}", resultWidget, resultWidget.Allocation, resultWidget.Name, resultWidget.GetType ().FullName, (resultWidget.Root as Widget)?.Name);
		}

		public override void ToXml (XmlElement element)
		{
			AddAttribute (element, "type", resultWidget.GetType ().ToString ());
			AddAttribute (element, "fulltype", resultWidget.GetType ().FullName);

			if (resultWidget.Name != null) {
				AddAttribute (element, "name", resultWidget.Name);
			}

			AddAttribute (element, "visible", resultWidget.Visible.ToString ());
			AddAttribute (element, "sensitive", resultWidget.Sensitive.ToString ());
			AddAttribute (element, "allocation", resultWidget.Allocation.ToString ());
		}

		public override string GetResultType  ()
		{
			return resultWidget.GetType ().FullName;
		}

		public override AppResult Marked (string mark)
		{
			if (resultWidget.Name != null && resultWidget.Name.IndexOf (mark) > -1) {
				return this;
			}

			if (resultWidget.GetType ().FullName == mark) {
				return this;
			}

			var window = resultWidget as Gtk.Window;
			if (window != null) {
				if (window.Title != null && window.Title.IndexOf (mark) > -1) {
					return this;
				}
			}
			return null;
		}

		public override AppResult Selected ()
		{
			return HasFocusWithin (resultWidget) ? this : null;
		}

		// In GTK 4 an Entry/SpinButton delegates focus to an internal Gtk.Text.
		static bool HasFocusWithin (Widget widget)
		{
			if (widget.HasFocus)
				return true;
			return widget is IEditable editable && editable.Delegate is Widget inner && inner != widget && inner.HasFocus;
		}

		public override AppResult CheckType (Type desiredType)
		{
			if (desiredType.IsInstanceOfType (resultWidget)) {
				return desiredType == typeof(Notebook) ? DisposeWithResult (new GtkNotebookResult (resultWidget)) : this;
			}

			return null;
		}

		static string GetComboBoxActiveText (ComboBox cb)
		{
			if (cb is ComboBoxText cbt)
				return cbt.ActiveText;

			if (cb.HasEntry && cb.Child is IEditable entry)
				return entry.Text;

			var model = cb.Model;
			if (model == null || !cb.GetActiveIter (out TreeIter iter))
				return null;

			int column = cb.EntryTextColumn >= 0 ? cb.EntryTextColumn : 0;
			if (column >= model.NColumns)
				return null;
			return GtkTreeModelResult.GetModelValue (model, iter, column) as string;
		}

		public override AppResult Text (string text, bool exact)
		{
			// Entries and Labels have Text, Buttons have Label.
			// FIXME: Are there other property names?

			// Check for the combobox first and try to use the active text.
			// If the active text fails then
			ComboBox cb = resultWidget as ComboBox;
			if (cb != null) {
				string activeText = GetComboBoxActiveText (cb);
				if (activeText == null) {
					return null;
				}

				return CheckForText (activeText, text, exact) ? this : null;
			}

			// Look for a Text property on the widget.
			PropertyInfo pinfo = resultWidget.GetType ().GetProperty ("Text");
			if (pinfo != null) {
				string propText = (string)pinfo.GetValue (resultWidget);
				if (CheckForText (propText, text, exact)) {
					return this;
				}
			}

			pinfo = resultWidget.GetType ().GetProperty ("Label");
			if (pinfo != null) {
				string propText = pinfo.GetValue (resultWidget) as string;

				// Stock items are gone in GTK 4; only mnemonic underlines need stripping.
				bool useUnderline = (resultWidget as Button)?.UseUnderline ?? (resultWidget as CheckButton)?.UseUnderline ?? false;
				if (useUnderline && propText != null) {
					int indexOfUnderline = propText.IndexOf ("_");
					if (indexOfUnderline > -1) {
						propText = propText.Remove (indexOfUnderline, 1);
					}
				}

				if (CheckForText (propText, text, exact)) {
					return this;
				}
			}

			return null;
		}

		protected ITreeModel ModelFromWidget (Widget widget)
		{
			TreeView tv = widget as TreeView;
			if (tv != null) {
				return tv.Model;
			}

			ComboBox cb = widget as ComboBox;
			if (cb == null) {
				return null;
			}

			return cb.Model;
		}

		public override AppResult Model (string column)
		{
			ITreeModel model = ModelFromWidget (resultWidget);
			if (model == null) {
				return null;
			}

			if (column == null) {
				return DisposeWithResult (new GtkTreeModelResult (resultWidget, model, 0) { SourceQuery = this.SourceQuery });
			}

			// Check if the class has the SemanticModelAttribute
			var columnNumber = GetColumnNumber (column, model);
			return columnNumber == -1 ? null : DisposeWithResult (new GtkTreeModelResult (resultWidget, model, columnNumber) { SourceQuery = this.SourceQuery });
		}

		protected int GetColumnNumber (string column, ITreeModel model)
		{
			Type modelType = model.GetType ();
			SemanticModelAttribute attr = modelType.GetCustomAttribute<SemanticModelAttribute> ();
			if (attr == null) {
				// Check if the instance has the attributes
				AttributeCollection attrs = TypeDescriptor.GetAttributes (model);
				attr = (SemanticModelAttribute)attrs [typeof(SemanticModelAttribute)];
				if (attr == null) {
					if (column.StartsWith ("column__")) {
						int columnNum;
						if (int.TryParse (column.Replace ("column__", string.Empty), out columnNum))
							return columnNum;
					}
					else
						return -1;
				}
			}
			return Array.IndexOf (attr.ColumnNames, column);
		}

		public override AppResult Property (string propertyName, object value)
		{
			return MatchProperty (propertyName, resultWidget, value);
		}

		public override List<AppResult> NextSiblings ()
		{
			// This really shouldn't happen
			if (resultWidget.Parent == null) {
				return null;
			}

			List<AppResult> siblingResults = new List<AppResult> ();
			for (Widget child = resultWidget.NextSibling; child != null; child = child.NextSibling) {
				siblingResults.Add (DisposeWithResult (new GtkWidgetResult (child) { SourceQuery = this.SourceQuery }));
			}

			return siblingResults;
		}

		public override ObjectProperties Properties ()
		{
			return GetProperties (resultWidget);
		}

		public override bool Select ()
		{
			if (resultWidget.CanFocus == false) {
				return false;
			}

			resultWidget.GrabFocus ();
			return true;
		}

		static readonly Regex hrefRegex = new Regex ("href\\s*=\\s*[\"']([^\"']*)[\"']", RegexOptions.IgnoreCase);

		public override bool Click ()
		{
			Label lbl = resultWidget as Label;
			if (lbl != null) {
				// Follow the focused link, or the first one in the markup.
				string uri = lbl.CurrentUri;
				if (uri == null && lbl.UseMarkup && lbl.LabelProp != null) {
					var match = hrefRegex.Match (lbl.LabelProp);
					if (match.Success)
						uri = match.Groups [1].Value;
				}
				if (uri == null)
					return false;
				GLib.Signal.Emit (lbl, "activate-link", uri);
				return true;
			}

			// Buttons, check buttons, expanders, menu buttons, entries...: "activate" == click.
			return resultWidget.Activate ();
		}

		public override bool Click (double x, double y)
		{
			if (resultWidget is TreeView treeView) {
				treeView.ConvertWidgetToBinWindowCoords ((int)x, (int)y, out int bx, out int by);
				if (!treeView.GetPathAtPos (bx, by, out TreePath path, out TreeViewColumn column))
					return false;
				treeView.SetCursor (path, column, false);
				return true;
			}

			// Activate the deepest widget under the point, or its closest activatable ancestor.
			Widget hit = resultWidget.Pick (x, y, PickFlags.Default) ?? resultWidget;
			for (Widget w = hit; w != null; w = w.Parent) {
				if (w.Activate ())
					return true;
				if (w == resultWidget)
					break;
			}

			return false;
		}

		// Key emulation targets the focus widget when the result is a window.
		Widget GetKeyTarget ()
		{
			if (resultWidget is Gtk.Window window)
				return window.Focus ?? window;
			return resultWidget;
		}

		static IEditable GetEditable (Widget widget)
		{
			return widget is IEditable editable && editable.IsEditable ? editable : null;
		}

		internal void RealTypeKey (Gdk.Key key, Gdk.ModifierType state)
		{
			Widget target = GetKeyTarget ();
			if (target == null)
				return;

			if (TryActivateShortcut (target, (uint)key, state))
				return;

			if (HandleSpecialKey (target, key, state))
				return;

			const Gdk.ModifierType commandMask = Gdk.ModifierType.ControlMask | Gdk.ModifierType.AltMask | Gdk.ModifierType.SuperMask | Gdk.ModifierType.MetaMask | Gdk.ModifierType.HyperMask;
			if ((state & commandMask) != 0)
				return;

			uint unicode = Gdk.Keyval.ToUnicode ((uint)key);
			if (unicode != 0)
				InsertText (target, char.ConvertFromUtf32 ((int)unicode));
		}

		static void InsertText (Widget target, string text)
		{
			if (target is TextView textView) {
				if (textView.Editable)
					GLib.Signal.Emit (textView, "insert-at-cursor", text);
				return;
			}

			var editable = GetEditable (target);
			if (editable == null)
				return;

			editable.DeleteSelection ();
			int position = editable.Position;
			editable.InsertText (text, ref position);
			editable.Position = position;
		}

		static bool HandleSpecialKey (Widget target, Gdk.Key key, Gdk.ModifierType state)
		{
			bool shift = (state & Gdk.ModifierType.ShiftMask) != 0;
			var editable = GetEditable (target);
			var textView = target as TextView;
			var treeView = target as TreeView;

			switch (key) {
			case Gdk.Key.Return:
			case Gdk.Key.KP_Enter:
				if (textView != null) {
					InsertText (target, "\n");
					return true;
				}
				if (!target.Activate ())
					target.ActivateActionVariant ("default.activate", null);
				return true;

			case Gdk.Key.Escape:
				for (Widget w = target; w != null; w = w.Parent) {
					if (w is Popover popover) {
						popover.Popdown ();
						return true;
					}
				}
				(target.Root as Gtk.Window)?.Close ();
				return true;

			case Gdk.Key.Tab:
			case Gdk.Key.ISO_Left_Tab:
				if (textView != null && textView.Editable && !shift && key == Gdk.Key.Tab) {
					InsertText (target, "\t");
					return true;
				}
				MoveFocus (target, shift || key == Gdk.Key.ISO_Left_Tab ? DirectionType.TabBackward : DirectionType.TabForward);
				return true;

			case Gdk.Key.BackSpace:
			case Gdk.Key.Delete:
				if (textView != null) {
					if (key == Gdk.Key.BackSpace)
						GLib.Signal.Emit (textView, "backspace");
					else
						GLib.Signal.Emit (textView, "delete-from-cursor", DeleteType.Chars, 1);
					return true;
				}
				if (editable != null) {
					if (editable.GetSelectionBounds (out int start, out int end) && start != end) {
						editable.DeleteSelection ();
					} else {
						int pos = editable.Position;
						if (key == Gdk.Key.BackSpace && pos > 0)
							editable.DeleteText (pos - 1, pos);
						else if (key == Gdk.Key.Delete)
							editable.DeleteText (pos, pos + 1);
					}
					return true;
				}
				return false;

			case Gdk.Key.Up:
			case Gdk.Key.Down:
			case Gdk.Key.Left:
			case Gdk.Key.Right:
				bool vertical = key == Gdk.Key.Up || key == Gdk.Key.Down;
				int count = key == Gdk.Key.Up || key == Gdk.Key.Left ? -1 : 1;
				if (treeView != null) {
					if (vertical)
						GLib.Signal.Emit (treeView, "move-cursor", MovementStep.DisplayLines, count, shift, false);
					else
						GLib.Signal.Emit (treeView, "expand-collapse-cursor-row", false, count > 0, false);
					return true;
				}
				if (textView != null) {
					GLib.Signal.Emit (textView, "move-cursor", vertical ? MovementStep.DisplayLines : MovementStep.VisualPositions, count, shift);
					return true;
				}
				if (editable != null && !vertical) {
					editable.Position = Math.Max (0, editable.Position + count);
					return true;
				}
				MoveFocus (target, key == Gdk.Key.Up ? DirectionType.Up : key == Gdk.Key.Down ? DirectionType.Down : key == Gdk.Key.Left ? DirectionType.Left : DirectionType.Right);
				return true;
			}

			return false;
		}

		static void MoveFocus (Widget target, DirectionType direction)
		{
			if (target.Root is Gtk.Window window)
				GLib.Signal.Emit (window, "move-focus", direction);
		}

		// Triggers the first Gtk.Shortcut bound to keyval+modifiers in a ShortcutController on the
		// target or one of its ancestors (GTK 4 has no API to dispatch a synthetic key event).
		static bool TryActivateShortcut (Widget target, uint keyval, Gdk.ModifierType state)
		{
			uint lowerKeyval = Gdk.Keyval.ToLower (keyval);
			for (Widget w = target; w != null; w = w.Parent) {
				GLib.IListModel controllers = w.ObserveControllers ();
				for (uint i = 0; i < controllers.NItems; i++) {
					if (!(controllers.GetObject (i) is ShortcutController shortcuts))
						continue;
					for (uint j = 0; j < shortcuts.NItems; j++) {
						if (!(shortcuts.GetObject (j) is Shortcut shortcut) || shortcut.Action == null)
							continue;
						if (!TriggerMatches (shortcut.Trigger, lowerKeyval, state))
							continue;
						if (shortcut.Action.Activate (ShortcutActionFlags.Exclusive, w, shortcut.Arguments))
							return true;
					}
				}
			}
			return false;
		}

		static bool TriggerMatches (ShortcutTrigger trigger, uint lowerKeyval, Gdk.ModifierType state)
		{
			const Gdk.ModifierType mask = Gdk.ModifierType.ShiftMask | Gdk.ModifierType.ControlMask | Gdk.ModifierType.AltMask | Gdk.ModifierType.SuperMask | Gdk.ModifierType.HyperMask | Gdk.ModifierType.MetaMask;
			switch (trigger) {
			case KeyvalTrigger keyvalTrigger:
				return Gdk.Keyval.ToLower (keyvalTrigger.Keyval) == lowerKeyval && (keyvalTrigger.Modifiers & mask) == (state & mask);
			case AlternativeTrigger alternative:
				return TriggerMatches (alternative.First, lowerKeyval, state) || TriggerMatches (alternative.Second, lowerKeyval, state);
			default:
				return false;
			}
		}

		Gdk.ModifierType ParseModifier (string modifierString)
		{
			string[] modifiers = modifierString.Split ('|');
			Gdk.ModifierType modifier = Gdk.ModifierType.None;

			foreach (var m in modifiers) {
				switch (m) {
				case "Shift":
					modifier |= Gdk.ModifierType.ShiftMask;
					break;

				case "Lock":
					modifier |= Gdk.ModifierType.LockMask;
					break;

				case "Control":
					modifier |= Gdk.ModifierType.ControlMask;
					break;

				case "Mod1":
				case "Alt":
					modifier |= Gdk.ModifierType.AltMask;
					break;

				case "Super":
					modifier |= Gdk.ModifierType.SuperMask;
					break;

				case "Hyper":
					modifier |= Gdk.ModifierType.HyperMask;
					break;

				case "Meta":
					modifier |= Gdk.ModifierType.MetaMask;
					break;

				default:
					// Mod2..Mod5 do not exist in GTK 4.
					modifier |= Gdk.ModifierType.None;
					break;
				}
			}

			return modifier;
		}

		public override bool TypeKey (char key, string state = "")
		{
			Gdk.Key realKey;

			if (key == '\n')
				realKey = Gdk.Key.Return;
			else
				realKey = (Gdk.Key) Gdk.Global.UnicodeToKeyval ((uint)key);

			RealTypeKey (realKey, ParseModifier (state));

			return true;
		}

		Gdk.Key ParseKeyString (string keyString)
		{
			switch (keyString) {
			case "ESC":
				return Gdk.Key.Escape;

			case "UP":
				return Gdk.Key.Up;

			case "DOWN":
				return Gdk.Key.Down;

			case "LEFT":
				return Gdk.Key.Left;

			case "RIGHT":
				return Gdk.Key.Right;

			case "RETURN":
				return Gdk.Key.Return;

			case "TAB":
				return Gdk.Key.Tab;

			case "BKSP":
				return Gdk.Key.BackSpace;

			case "DELETE":
				return Gdk.Key.Delete;

			default:
				throw new Exception ("Unknown keystring: " + keyString);
			}
		}

		public override bool TypeKey (string keyString, string state = "")
		{
			Gdk.Key realKey = ParseKeyString (keyString);
			RealTypeKey (realKey, ParseModifier (state));
			return true;
		}

		public override bool EnterText (string text)
		{
			foreach (var c in text) {
				TypeKey (c);
			}

			return true;
		}

		public override bool Toggle (bool active)
		{
			switch (resultWidget) {
			case ToggleButton toggleButton:
				toggleButton.Active = active;
				return true;
			case CheckButton checkButton:
				checkButton.Active = active;
				return true;
			case Switch switchWidget:
				switchWidget.Active = active;
				return true;
			default:
				return false;
			}
		}

		const string FlashCssClass = "autotest-flash";
		static CssProvider flashProvider;

		static void EnsureFlashProvider (Widget widget)
		{
			if (flashProvider != null || widget.Display == null)
				return;
			flashProvider = new CssProvider ();
			flashProvider.LoadFromString ("." + FlashCssClass + " { outline: 2px solid red; outline-offset: -2px; }");
			StyleContext.AddProviderForDisplay (widget.Display, flashProvider, 800 /* GTK_STYLE_PROVIDER_PRIORITY_USER */);
		}

		public override void Flash ()
		{
			var widget = resultWidget;
			if (widget == null)
				return;

			EnsureFlashProvider (widget);

			int flashCount = 10;
			bool flashState = false;

			GLib.Timeout.Add (1000, () => {
				flashState = !flashState;
				flashCount--;

				if (flashState && flashCount > 0)
					widget.AddCssClass (FlashCssClass);
				else
					widget.RemoveCssClass (FlashCssClass);

				return flashCount > 0;
			});
		}

		public override void SetProperty (string propertyName, object value)
		{
			base.SetProperty (resultWidget, propertyName, value);
		}

		protected override void Dispose (bool disposing)
		{
			resultWidget = null;
			base.Dispose (disposing);
		}
	}
}

