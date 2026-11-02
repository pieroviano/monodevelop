//
// CommandMenu.cs
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

namespace MonoDevelop.Components.Commands
{
	/// <summary>
	/// A menu of commands (GTK 4: a <see cref="Gtk.PopoverMenu"/> built on demand from a menu model).
	/// The command state is evaluated every time the menu is popped up.
	/// </summary>
	public class CommandMenu
	{
		readonly CommandEntrySet commandEntrySet;
		readonly CommandManager manager;
		object initialCommandTarget;

		internal CommandMenu (CommandManager manager, CommandEntrySet commandEntrySet)
		{
			this.manager = manager;
			this.commandEntrySet = commandEntrySet;
		}

		/// <summary>Object where the command handler lookup starts.</summary>
		public object InitialCommandTarget {
			get { return initialCommandTarget; }
			set { initialCommandTarget = value; }
		}

		/// <summary>Raised when the popped up menu is closed.</summary>
		public event EventHandler Closed;

		/// <summary>True if no command is currently visible in the menu.</summary>
		public bool IsEmpty {
			get {
				var context = CreateContext (CommandSource.ContextMenu);
				return CommandMenuContext.IsEmpty (context.Build (commandEntrySet));
			}
		}

		internal CommandManager CommandManager {
			get { return manager; }
		}

		/// <summary>Builds the model of this menu into an existing context (so its actions share the context action group).</summary>
		internal GLib.Menu Build (CommandMenuContext context)
		{
			return context.Build (commandEntrySet);
		}

		internal CommandMenuContext CreateContext (CommandSource source)
		{
			return new CommandMenuContext (manager, initialCommandTarget, source);
		}

		/// <summary>Creates, without showing it, a popover menu for the current command state. It has to be parented to a widget.</summary>
		internal Gtk.PopoverMenu CreatePopover ()
		{
			manager.RegisterUserInteraction ();
			var context = CreateContext (CommandSource.ContextMenu);
			var model = context.Build (commandEntrySet);
			var popover = new Gtk.PopoverMenu (model, Gtk.PopoverMenuFlags.Nested);
			popover.InsertActionGroup (CommandMenuContext.ActionPrefix, context.Actions);
			foreach (var custom in context.CustomWidgets)
				popover.AddChild (custom.Value, custom.Key);
			popover.Closed += (s, e) => Closed?.Invoke (this, EventArgs.Empty);
			return popover;
		}

		/// <summary>Pops up the menu at (<paramref name="x"/>, <paramref name="y"/>) in <paramref name="parent"/> coordinates.</summary>
		public void Popup (Gtk.Widget parent, int x, int y)
		{
			if (parent == null)
				throw new ArgumentNullException (nameof (parent));
			if (initialCommandTarget == null)
				initialCommandTarget = parent;
			GtkWorkarounds.ShowContextMenu (CreatePopover (), parent, x, y);
		}

		/// <summary>Pops up the menu at the position of the click, or at the origin of the parent if <paramref name="evt"/> is null.</summary>
		public void Popup (Gtk.Widget parent, ClickInfo evt)
		{
			if (parent == null)
				throw new ArgumentNullException (nameof (parent));
			int x = 0, y = 0;
			if (evt != null) {
				// ClickInfo coordinates are relative to the widget owning the gesture.
				var owner = evt.Gesture?.Widget;
				var r = new Gdk.Rectangle ((int)evt.X, (int)evt.Y, 1, 1);
				if (owner != null)
					r = owner.TranslateRectangle (parent, r);
				x = r.X;
				y = r.Y;
			}
			Popup (parent, x, y);
		}
	}
}
