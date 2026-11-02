//
// CommandMenuContext.cs
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
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace MonoDevelop.Components.Commands
{
	/// <summary>
	/// Evaluates command entries against the current command state and renders them as a GTK 4 menu model
	/// (<see cref="GLib.Menu"/> + <see cref="GLib.SimpleActionGroup"/>). Separators split the menu in sections;
	/// check and radio commands become stateful actions. The model is a snapshot: it has to be rebuilt to
	/// reflect later changes of the command state.
	/// </summary>
	class CommandMenuContext
	{
		/// <summary>Action group prefix the <see cref="Actions"/> group has to be inserted with.</summary>
		public const string ActionPrefix = "cmd";

		class Scope
		{
			public readonly GLib.Menu Menu = new GLib.Menu ();
			/// <summary>Items are added directly to the menu, without sections (needed by menu bars).</summary>
			public bool Flat;
			public GLib.Menu Section = new GLib.Menu ();
			public int SectionItems;
			public int TotalItems;

			public void EndSection ()
			{
				if (SectionItems > 0)
					Menu.AppendSection (null, Section);
				Section = new GLib.Menu ();
				SectionItems = 0;
			}
		}

		readonly Stack<Scope> scopes = new Stack<Scope> ();
		int counter;

		public CommandMenuContext (CommandManager manager, object initialTarget, CommandSource source)
		{
			Manager = manager;
			InitialTarget = initialTarget;
			Source = source;
			Route = new CommandTargetRoute (initialTarget);
			Actions = new GLib.SimpleActionGroup ();
			CustomWidgets = new List<KeyValuePair<string, Gtk.Widget>> ();
		}

		public CommandManager Manager { get; }
		public object InitialTarget { get; }
		public CommandSource Source { get; }
		public CommandTargetRoute Route { get; }

		/// <summary>Actions of the menu items. Insert them in the menu's widget with <see cref="ActionPrefix"/>.</summary>
		public GLib.SimpleActionGroup Actions { get; }

		/// <summary>Widgets of custom commands. They have to be added to the popover with <c>AddChild (widget, id)</c>.</summary>
		public List<KeyValuePair<string, Gtk.Widget>> CustomWidgets { get; }

		/// <summary>Builds the menu model for the given entries.</summary>
		public GLib.Menu Build (IEnumerable<CommandEntry> entries, bool flat = false)
		{
			var scope = BeginScope (flat);
			foreach (var entry in entries)
				entry.AddToMenu (this);
			return EndScope (scope);
		}

		/// <summary>True if the last built root menu has no items.</summary>
		public static bool IsEmpty (GLib.Menu menu)
		{
			return menu == null || menu.NItems == 0;
		}

		internal object BeginScope (bool flat = false)
		{
			var scope = new Scope { Flat = flat };
			scopes.Push (scope);
			return scope;
		}

		/// <summary>Closes the scope started with <see cref="BeginScope"/>, returning its menu.</summary>
		internal GLib.Menu EndScope (object scopeToken)
		{
			var scope = scopes.Pop ();
			scope.EndSection ();
			return scope.Menu;
		}

		/// <summary>Number of items added to the scope since it was started.</summary>
		internal int GetItemCount (object scopeToken)
		{
			return ((Scope)scopeToken).TotalItems;
		}

		/// <summary>Starts a new section (a separator is rendered between sections).</summary>
		public void AddSeparator ()
		{
			if (!scopes.Peek ().Flat)
				scopes.Peek ().EndSection ();
		}

		/// <summary>Adds an item to the current scope.</summary>
		public void AddItem (GLib.MenuItem item)
		{
			var scope = scopes.Peek ();
			if (scope.Flat)
				scope.Menu.AppendItem (item);
			else {
				scope.Section.AppendItem (item);
				scope.SectionItems++;
			}
			scope.TotalItems++;
		}

		/// <summary>Adds a submenu item, unless the submenu is empty and <paramref name="hideIfEmpty"/> is set.</summary>
		public void AddSubmenu (string label, GLib.Menu submenu, IconId icon, bool hideIfEmpty)
		{
			if (hideIfEmpty && submenu.NItems == 0)
				return;
			var item = new GLib.MenuItem (label, (string)null);
			item.SetLink ("submenu", submenu);
			SetIcon (item, icon);
			AddItem (item);
		}

		/// <summary>Registers an action and returns the detailed name to be used by menu items.</summary>
		public string AddAction (GLib.SimpleAction action)
		{
			Actions.Insert (action);
			return ActionPrefix + "." + action.Name;
		}

		public string NextActionName ()
		{
			return "item" + (counter++);
		}

		/// <summary>Adds an item that opens a url or runs a plain handler.</summary>
		public void AddActionItem (string label, IconId icon, Action activated, string accel = null)
		{
			string name = NextActionName ();
			var action = new GLib.SimpleAction (name, null);
			action.Activated += (s, e) => activated ();
			var item = new GLib.MenuItem (label, AddAction (action));
			SetIcon (item, icon);
			SetAccel (item, accel);
			AddItem (item);
		}

		/// <summary>Adds a widget (custom command) as a menu item. Requires a popover to host it.</summary>
		public void AddCustomWidget (Gtk.Widget widget)
		{
			string id = "custom" + (counter++);
			var item = new GLib.MenuItem (null, (string)null);
			item.SetAttributeValue ("custom", new GLib.Variant (id));
			CustomWidgets.Add (new KeyValuePair<string, Gtk.Widget> (id, widget));
			AddItem (item);
		}

		/// <summary>Creates the item(s) of a command, evaluating the current command state.</summary>
		public void AddCommand (object commandId, Command cmd, string overrideLabel, bool disabledVisible)
		{
			if (cmd == null)
				cmd = Manager.GetCommand (commandId);

			if (cmd == null) {
				LoggingService.LogWarning ("Unknown command '{0}'", commandId);
				AddItem (new GLib.MenuItem ("<Unknown Command>", (string)null));
				return;
			}

			var acmd = cmd as ActionCommand;
			if (acmd == null)
				throw new InvalidOperationException ("Unknown cmd type.");

			CommandInfo info = Manager.GetCommandInfo (commandId, Route);
			WaitForAsyncUpdate (commandId, info);

			if (acmd.CommandArray) {
				if (info.ArrayInfo == null)
					return;
				foreach (CommandInfo sub in info.ArrayInfo) {
					if (sub.IsArraySeparator)
						AddSeparator ();
					else
						AddInfo (commandId, acmd, sub, null, disabledVisible);
				}
				return;
			}

			AddInfo (commandId, acmd, info, overrideLabel, disabledVisible);
		}

		static void WaitForAsyncUpdate (object commandId, CommandInfo info)
		{
			// A menu model is a snapshot: asynchronous command updates have to finish before it is built.
			if (info.IsUpdatingAsynchronously && !info.UpdateTask.Wait (CommandManager.SlowUpdateCommandTime)) {
				info.CancelAsyncUpdate ();
				LoggingService.LogError ("Slow command update task timed out: Command:{0}", commandId);
			}
		}

		void AddInfo (object commandId, ActionCommand acmd, CommandInfo info, string overrideLabel, bool disabledVisible)
		{
			if (!info.Visible || (!disabledVisible && !info.Enabled))
				return;

			string label = GetLabel (info, overrideLabel);
			string accel = info.AccelKey != null ? KeyBindingManager.BindingToGtkAccelerator (info.AccelKey) : null;

			var set = info as CommandInfoSet;
			if (set != null) {
				var scope = BeginScope ();
				foreach (CommandInfo sub in set.CommandInfos) {
					if (sub.IsArraySeparator)
						AddSeparator ();
					else
						AddInfo (commandId, acmd, sub, null, true);
				}
				AddSubmenu (label, EndScope (scope), info.Icon, false);
				return;
			}

			string name = NextActionName ();
			GLib.SimpleAction action;
			GLib.MenuItem item;
			object dataItem = info.DataItem;

			if (acmd.ActionType == ActionType.Radio) {
				// Radio look: string state, item target; checked when state == target.
				const string on = "on";
				action = new GLib.SimpleAction (name, GLib.VariantType.String, new GLib.Variant (info.Checked ? on : string.Empty));
				item = new GLib.MenuItem (label, (string)null);
				item.SetActionAndTargetValue (ActionPrefix + "." + name, new GLib.Variant (on));
			} else if (acmd.ActionType == ActionType.Check) {
				// Check look: boolean state, no target.
				action = new GLib.SimpleAction (name, null, new GLib.Variant (info.Checked));
				item = new GLib.MenuItem (label, ActionPrefix + "." + name);
			} else {
				action = new GLib.SimpleAction (name, null);
				item = new GLib.MenuItem (label, ActionPrefix + "." + name);
			}

			action.Enabled = info.Enabled;
			action.Activated += (s, e) => Manager.DispatchCommand (commandId, dataItem, InitialTarget, Source, info);
			Actions.Insert (action);

			// TODO GTK4: GMenu items have no hover/tooltip support, so CommandInfo.Description is not shown
			// and CommandManager.CommandSelected/CommandDeselected are not raised for menu items.
			SetIcon (item, info.Icon);
			SetAccel (item, accel);
			AddItem (item);
		}

		static string GetLabel (CommandInfo info, string overrideLabel)
		{
			string text = overrideLabel ?? info.Text ?? string.Empty;
			if (info.UseMarkup && overrideLabel == null)
				text = StripMarkup (text);
			return text;
		}

		/// <summary>GMenu labels are plain text: removes the tags of a Pango markup string.</summary>
		static string StripMarkup (string markup)
		{
			if (markup.IndexOfAny (new [] { '<', '&' }) < 0)
				return markup;
			var sb = new StringBuilder ();
			bool inTag = false;
			for (int i = 0; i < markup.Length; i++) {
				char c = markup [i];
				if (c == '<')
					inTag = true;
				else if (c == '>' && inTag)
					inTag = false;
				else if (!inTag)
					sb.Append (c);
			}
			return sb.ToString ().Replace ("&lt;", "<").Replace ("&gt;", ">").Replace ("&quot;", "\"").Replace ("&apos;", "'").Replace ("&amp;", "&");
		}

		static void SetAccel (GLib.MenuItem item, string accel)
		{
			if (!string.IsNullOrEmpty (accel))
				item.SetAttributeValue ("accel", new GLib.Variant (accel));
		}

		static readonly ConditionalWeakTable<Xwt.Drawing.Image, GLib.IIcon> iconCache = new ConditionalWeakTable<Xwt.Drawing.Image, GLib.IIcon> ();

		static void SetIcon (GLib.MenuItem item, IconId iconId)
		{
			if (iconId.IsNull)
				return;
			var icon = GetIcon (ImageService.GetIcon (iconId, StockIconSize.Menu));
			if (icon != null)
				item.Icon = icon;
		}

		static GLib.IIcon GetIcon (Xwt.Drawing.Image image)
		{
			if (image == null)
				return null;
			if (iconCache.TryGetValue (image, out var cached))
				return cached;
			GLib.IIcon icon = null;
			try {
				using (var ms = new MemoryStream ()) {
					image.WithSize (Xwt.IconSize.Small).Save (ms, Xwt.Drawing.ImageFileType.Png);
					icon = new GLib.BytesIcon (new GLib.Bytes (ms.ToArray ()));
				}
			} catch (Exception ex) {
				LoggingService.LogError ("Could not convert menu icon", ex);
			}
			iconCache.Add (image, icon);
			return icon;
		}
	}
}
