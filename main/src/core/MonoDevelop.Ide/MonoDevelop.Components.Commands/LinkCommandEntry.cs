//
// LinkCommandEntry.cs
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
using System.Diagnostics;
using Mono.Addins;
using MonoDevelop.Core;

namespace MonoDevelop.Components.Commands
{
	public class LinkCommandEntry: CommandEntry
	{
		static object Id = new object ();
		string text;
		string url;
		IconId icon = "gtk-jump-to";
		
		public LinkCommandEntry (string text, string url): base (Id)
		{
			this.text = text;
			this.url = url;
		}
		
		public LinkCommandEntry (string text, string url, IconId icon): base (Id)
		{
			this.text = text;
			this.url = url;
			this.icon = icon;
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		public string Url {
			get { return url; }
			set { url = value; }
		}
		
		internal void HandleActivation (Gtk.Widget parent)
		{
			try {
				System.Diagnostics.Process.Start (new ProcessStartInfo (url) { UseShellExecute = true });
			} catch (Exception) {
				string msg = AddinManager.CurrentLocalizer.GetString ("Could not open the url {0}", url);
				MonoDevelop.Ide.MessageService.ShowError (parent?.Root as Gtk.Window, msg);
			}
		}
		
		internal override void AddToMenu (CommandMenuContext context)
		{
			// TODO GTK4: GMenu items have no hover event, so CommandManager.NotifySelected (open url description) is not raised.
			context.AddActionItem (text != null ? text : url, icon, () => HandleActivation (null));
		}

		internal override Gtk.Widget CreateToolItem (CommandManager manager)
		{
			var item = new Gtk.Button { Label = text, IconName = null };
			item.Clicked += (s, e) => HandleActivation (item);
			return item;
		}
	}
}
