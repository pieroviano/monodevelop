//
// ContextMenuExtensionsGtk.cs
//
// Author:
//       Greg Munn <greg.munn@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc
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
using System.IO;
using System.Runtime.CompilerServices;
using MonoDevelop.Core;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Renders a <see cref="ContextMenu"/> as a GTK 4 <see cref="Gtk.PopoverMenu"/> built from a <c>GMenu</c> model and a
	/// <c>GSimpleActionGroup</c> (one action per item; check items are boolean-state actions, radio items string-state actions).
	/// </summary>
	static class ContextMenuExtensionsGtk
	{
		const string ActionPrefix = "mdctx";

		public static void ShowContextMenu (Gtk.Widget parent, ClickInfo evt, ContextMenu menu)
		{
			ShowContextMenu (parent, evt, menu, null);
		}

		public static void ShowContextMenu (Gtk.Widget parent, ClickInfo evt, ContextMenu menu, Action closeHandler)
		{
			if (parent == null)
				throw new ArgumentNullException ("parent");
			if (menu == null)
				throw new ArgumentNullException ("menu");

			Gdk.Rectangle? area = null;
			if (evt != null) {
				// ClickInfo coordinates are relative to the widget owning the gesture.
				var owner = evt.Gesture?.Widget;
				var r = new Gdk.Rectangle ((int)evt.X, (int)evt.Y, 1, 1);
				area = owner != null ? owner.TranslateRectangle (parent, r) : r;
			}
			Popup (parent, area, menu, closeHandler);
		}

		public static void ShowContextMenu (Gtk.Widget parent, int x, int y, ContextMenu menu)
		{
			ShowContextMenu (parent, x, y, menu, null);
		}

		public static void ShowContextMenu (Gtk.Widget parent, int x, int y, ContextMenu menu, Action closeHandler, bool selectFirstItem = false)
		{
			if (parent == null)
				throw new ArgumentNullException ("parent");
			if (menu == null)
				throw new ArgumentNullException ("menu");

			var pop = Popup (parent, new Gdk.Rectangle (x, y, 1, 1), menu, closeHandler);
			if (selectFirstItem && pop != null) {
				pop.ChildFocus (Gtk.DirectionType.TabForward);
				var first = FirstVisibleItem (menu);
				first?.FireSelectedEvent (new Xwt.Rectangle (x, y, pop.Width, pop.Height));
			}
		}

		/// <summary>Creates (not shown) a popover menu for <paramref name="menu"/> attached to <paramref name="parent"/>.</summary>
		public static Gtk.PopoverMenu CreatePopoverMenu (Gtk.Widget parent, ContextMenu menu, Action closeHandler)
		{
			if (parent == null)
				throw new ArgumentNullException ("parent");
			if (menu == null)
				throw new ArgumentNullException ("menu");

			var actions = new GLib.SimpleActionGroup ();
			int counter = 0;
			var model = FromMenu (menu, actions, ref counter);

			var pop = new Gtk.PopoverMenu (model, Gtk.PopoverMenuFlags.Nested) {
				HasArrow = false,
				Position = Gtk.PositionType.Bottom,
				Halign = Gtk.Align.Start,
			};
			// Actions are looked up from the menu items up through the popover's ancestors.
			pop.InsertActionGroup (ActionPrefix, actions);
			pop.Parent = parent;
			pop.Closed += (s, e) => {
				closeHandler?.Invoke ();
				menu.FireClosedEvent ();
				// Item actions are activated after the popover closes: unparent it once they have run.
				GLib.Idle.Add (() => {
					pop.Detach ();
					return false;
				});
			};
			return pop;
		}

		static Gtk.PopoverMenu Popup (Gtk.Widget parent, Gdk.Rectangle? area, ContextMenu menu, Action closeHandler)
		{
			var pop = CreatePopoverMenu (parent, menu, closeHandler);
			pop.ShowAt (area);
			return pop;
		}

		static ContextMenuItem FirstVisibleItem (ContextMenu menu)
		{
			foreach (var item in menu.Items)
				if (item.Visible && !item.IsSeparator)
					return item;
			return null;
		}

		static GLib.Menu FromMenu (ContextMenu menu, GLib.SimpleActionGroup actions, ref int counter)
		{
			// Separators split the menu in sections (GMenu renders a separator between sections).
			var root = new GLib.Menu ();
			var section = new GLib.Menu ();
			bool sectionHasItems = false;

			foreach (var item in menu.Items) {
				if (!item.Visible)
					continue;
				if (item.IsSeparator) {
					if (sectionHasItems) {
						root.AppendSection (null, section);
						section = new GLib.Menu ();
						sectionHasItems = false;
					}
					continue;
				}
				var menuItem = CreateMenuItem (item, actions, ref counter);
				if (menuItem != null) {
					section.AppendItem (menuItem);
					sectionHasItems = true;
				}
			}
			if (sectionHasItems)
				root.AppendSection (null, section);
			return root;
		}

		static GLib.MenuItem CreateMenuItem (ContextMenuItem item, GLib.SimpleActionGroup actions, ref int counter)
		{
			var label = GetLabel (item);

			if (item.SubMenu != null && item.SubMenu.Items.Count > 0) {
				var sub = FromMenu (item.SubMenu, actions, ref counter);
				var subItem = new GLib.MenuItem (label, (string)null);
				subItem.SetLink ("submenu", sub);
				SetIcon (subItem, item.Image);
				return subItem;
			}

			string name = "item" + (counter++);
			GLib.SimpleAction action;
			GLib.MenuItem menuItem;

			if (item is RadioButtonContextMenuItem radioItem) {
				// Radio look: string state, item target; checked when state == target.
				const string on = "on";
				action = new GLib.SimpleAction (name, GLib.VariantType.String, new GLib.Variant (radioItem.Checked ? on : string.Empty));
				menuItem = new GLib.MenuItem (label, (string)null);
				menuItem.SetActionAndTargetValue (ActionPrefix + "." + name, new GLib.Variant (on));
			} else if (item is CheckBoxContextMenuItem checkItem) {
				// Check look: boolean state, no target.
				action = new GLib.SimpleAction (name, null, new GLib.Variant (checkItem.Checked));
				menuItem = new GLib.MenuItem (label, ActionPrefix + "." + name);
			} else {
				action = new GLib.SimpleAction (name, null);
				menuItem = new GLib.MenuItem (label, ActionPrefix + "." + name);
			}

			action.Enabled = item.Sensitive;
			action.Activated += (s, e) => item.Click ();
			actions.Insert (action);

			SetIcon (menuItem, item.Image);
			return menuItem;
		}

		/// <summary>GMenu labels always parse mnemonics: escape underscores when the item doesn't use one.</summary>
		static string GetLabel (ContextMenuItem item)
		{
			var label = item.Label ?? string.Empty;
			return item.UseMnemonic ? label : label.Replace ("_", "__");
		}

		static readonly ConditionalWeakTable<Xwt.Drawing.Image, GLib.IIcon> iconCache = new ConditionalWeakTable<Xwt.Drawing.Image, GLib.IIcon> ();

		static void SetIcon (GLib.MenuItem menuItem, Xwt.Drawing.Image image)
		{
			if (image == null)
				return;
			var icon = GetIcon (image);
			if (icon != null)
				menuItem.Icon = icon;
		}

		static GLib.IIcon GetIcon (Xwt.Drawing.Image image)
		{
			if (iconCache.TryGetValue (image, out var cached))
				return cached;
			GLib.IIcon icon = null;
			try {
				using (var ms = new MemoryStream ()) {
					image.WithSize (Xwt.IconSize.Small).Save (ms, Xwt.Drawing.ImageFileType.Png);
					icon = new GLib.BytesIcon (new GLib.Bytes (ms.ToArray ()));
				}
			} catch (Exception ex) {
				LoggingService.LogError ("Could not convert context menu icon", ex);
			}
			iconCache.Add (image, icon);
			return icon;
		}
	}
}
