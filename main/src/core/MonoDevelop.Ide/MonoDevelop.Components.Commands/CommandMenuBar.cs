//
// CommandMenuBar.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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

using System;
using System.Collections.Generic;
using MonoDevelop.Core;

namespace MonoDevelop.Components.Commands
{
	/// <summary>
	/// Main menu bar (GTK 4: a <see cref="Gtk.PopoverMenuBar"/>). Menu models are snapshots of the command state,
	/// so the model is rebuilt whenever the user is about to interact with the bar (pointer enters it, Alt/F10 pressed).
	/// </summary>
	class CommandMenuBar: Gtk.Box
	{
		readonly CommandManager manager;
		readonly CommandEntrySet entrySet;
		readonly Gtk.PopoverMenuBar bar;
		readonly List<KeyValuePair<int, KeyValuePair<string, CommandMenu>>> extraMenus = new List<KeyValuePair<int, KeyValuePair<string, CommandMenu>>> ();
		DateTime lastRebuild = DateTime.MinValue;

		public CommandMenuBar (CommandManager manager, CommandEntrySet entrySet): base (Gtk.Orientation.Horizontal, 0)
		{
			this.manager = manager;
			this.entrySet = entrySet;
			bar = new Gtk.PopoverMenuBar (new GLib.Menu ());
			bar.Hexpand = true;
			Append (bar);

			this.AddMotionHandler (null, (x, y) => RebuildIfIdle ());
			this.AddKeyPressHandler ((key, code, state) => {
				if (key == (uint)Gdk.Key.F10 || key == (uint)Gdk.Key.Alt_L || key == (uint)Gdk.Key.Alt_R || (state & Gdk.ModifierType.AltMask) != 0)
					RebuildIfIdle ();
				return false;
			}, null, Gtk.PropagationPhase.Capture);
			Mapped += (s, e) => RebuildIfIdle ();
			Rebuild ();
		}
		
		internal CommandManager CommandManager {
			get { return manager; }
		}

		/// <summary>Inserts a menu (e.g. the application menu) as an additional top level menu at the given position.</summary>
		public void InsertMenu (int position, string label, CommandMenu menu)
		{
			extraMenus.Add (new KeyValuePair<int, KeyValuePair<string, CommandMenu>> (position, new KeyValuePair<string, CommandMenu> (label, menu)));
			Rebuild ();
		}

		void RebuildIfIdle ()
		{
			if ((DateTime.Now - lastRebuild).TotalMilliseconds < 500 || HasOpenPopover (bar))
				return;
			Rebuild ();
		}

		static bool HasOpenPopover (Gtk.Widget widget)
		{
			for (var child = widget.FirstChild; child != null; child = child.NextSibling) {
				if (child is Gtk.Popover popover && popover.Visible)
					return true;
				if (HasOpenPopover (child))
					return true;
			}
			return false;
		}

		void Rebuild ()
		{
			lastRebuild = DateTime.Now;
			manager.RegisterUserInteraction ();
			var context = new CommandMenuContext (manager, null, CommandSource.MainMenu);
			var scope = context.BeginScope (true);
			int index = 0;
			foreach (CommandEntry entry in entrySet) {
				AddExtraMenus (context, index, false);
				entry.AddToMenu (context);
				index++;
			}
			AddExtraMenus (context, index, true);
			var model = context.EndScope (scope);

			bar.MenuModel = model;
			InsertActionGroup (CommandMenuContext.ActionPrefix, context.Actions);
		}

		void AddExtraMenus (CommandMenuContext context, int position, bool atEnd)
		{
			foreach (var extra in extraMenus) {
				if (extra.Key == position || (atEnd && extra.Key > position))
					context.AddSubmenu (extra.Value.Key, extra.Value.Value.Build (context), IconId.Null, true);
			}
		}
	}
}
