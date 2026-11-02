//
// CommandEntry.cs
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
//

using System;

namespace MonoDevelop.Components.Commands
{
	public enum CommandEntryDisplayType
	{
		Default,
		TextOnly,
		IconOnly,
		IconHasPriority,
		IconAndText
	}

	public class CommandEntry
	{
		object cmdId;
		string overrideLabel;
		bool disabledVisible = true;
		Command localCmd;

		public CommandEntry (object cmdId, string overrideLabel, bool disabledVisible)
		{
			this.cmdId         = CommandManager.ToCommandId (cmdId);
			this.overrideLabel = overrideLabel;
			this.disabledVisible = disabledVisible;
		}
		
		public CommandEntry (object cmdId, string overrideLabel): this (cmdId, overrideLabel, true)
		{
		}
		
		public CommandEntry (object cmdId) : this (cmdId, null, true)
		{
		}
		
		public CommandEntry (Command localCmd) : this (localCmd.Id, null, true)
		{
			this.localCmd = localCmd;
		}
		
		public object CommandId {
			get { return cmdId; }
			set { cmdId = CommandManager.ToCommandId (value); }
		}
		
		public bool DisabledVisible {
			get { return disabledVisible; }
			set { disabledVisible = value; }
		}

		public string OverrideLabel {
			get { return overrideLabel; }
			set { overrideLabel = value; }
		}

		public CommandEntryDisplayType DisplayType {
			get;
			set;
		}
		
		public virtual Command GetCommand (CommandManager manager)
		{
			if (localCmd != null) {
				if (manager.GetCommand (localCmd.Id) == null)
					manager.RegisterCommand (localCmd);
				localCmd = null;
			}
				
			return manager.GetCommand (cmdId);
		}
		
		/// <summary>Evaluates the entry and adds its menu item(s) to the menu being built by <paramref name="context"/>.</summary>
		internal virtual void AddToMenu (CommandMenuContext context)
		{
			if (cmdId == CommandManager.ToCommandId (Command.Separator)) {
				context.AddSeparator ();
				return;
			}

			Command cmd = GetCommand (context.Manager);
			if (cmd is CustomCommand customCmd) {
				context.AddCustomWidget ((Gtk.Widget) Activator.CreateInstance (customCmd.WidgetType));
				return;
			}

			context.AddCommand (cmdId, cmd, overrideLabel, disabledVisible);
		}
		
		/// <summary>Creates the toolbar widget bound to this entry.</summary>
		internal virtual Gtk.Widget CreateToolItem (CommandManager manager)
		{
			if (cmdId == CommandManager.ToCommandId (Command.Separator))
				return new Gtk.Separator (Gtk.Orientation.Vertical);

			Command cmd = GetCommand (manager);
			if (cmd == null)
				return new Gtk.Box (Gtk.Orientation.Horizontal, 0);
			
			if (cmd is CustomCommand) {
				Gtk.Widget child = (Gtk.Widget) Activator.CreateInstance (((CustomCommand)cmd).WidgetType);
				if (cmd.Text != null && cmd.Text.Length > 0) {
					//strip "_" accelerators from tooltips
					child.TooltipText = cmd.Text.Replace ("_", "");
				}
				return child;
			}
			
			ActionCommand acmd = cmd as ActionCommand;
			if (acmd == null)
				throw new InvalidOperationException ("Unknown cmd type.");

			if (acmd.CommandArray) {
				var set = new CommandEntrySet ();
				set.Add (this);
				return new MenuToolButton (new CommandMenu (manager, set), acmd.Icon);
			}
			else if (acmd.ActionType == ActionType.Normal)
				return new CommandToolButton (cmdId, manager);
			else
				return new CommandToggleToolButton (cmdId, manager);
		}
	}
}
