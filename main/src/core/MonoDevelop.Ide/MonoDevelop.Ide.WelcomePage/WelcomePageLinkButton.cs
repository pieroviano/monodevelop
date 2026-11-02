//
// WelcomePageLinkButton.cs
//
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
//
// Copyright (c) 2011 Xamarin Inc.
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
using Gtk;
using MonoDevelop.Core;
using System.Xml.Linq;
using MonoDevelop.Components;

namespace MonoDevelop.Ide.WelcomePage
{
	public class WelcomePageLinkButton : Gtk.Button
	{
		static readonly string linkUnderlinedFormat = "<span underline=\"single\" foreground=\"" + Styles.WelcomeScreen.Links.Color + "\">{0}</span>";
		static readonly string linkFormat = "<span foreground=\"" + Styles.WelcomeScreen.Links.Color + "\">{0}</span>";
		static readonly string descFormat = "<span size=\"small\" foreground=\"" + Styles.WelcomeScreen.Pad.TextColor + "\">{0}</span>";
		
		Label label;
		ImageView image;
		string text, desc, icon;
		StockIconSize iconSize = StockIconSize.Menu;
		Gtk.Box box;
		
		public WelcomePageLinkButton (string title, string href, StockIconSize iconSize = StockIconSize.Menu, string icon = null, string desc = null, string tooltip = null)
			: this ()
		{
			this.iconSize = iconSize;
			
			if (string.IsNullOrEmpty (title))
				throw new InvalidOperationException ("Link is missing title");
			this.text = GettextCatalog.GetString (title);
			
			if (string.IsNullOrEmpty (href))
				throw new InvalidOperationException ("Link is missing href");
			this.LinkUrl = href;
			
			if (!string.IsNullOrEmpty (desc))
				this.desc = GettextCatalog.GetString (desc);
			
			if (!string.IsNullOrEmpty (tooltip))
				this.TooltipText = GettextCatalog.GetString (tooltip);
			else
				this.TooltipText = GetLinkTooltip (href);
			
			if (!string.IsNullOrEmpty (icon))
				this.icon = icon;
			
			UpdateLabel (false);
			UpdateImage ();
		}
		
		public WelcomePageLinkButton (string label, string link)
			: this ()
		{
			this.text = label;
			this.LinkUrl = link;
			UpdateLabel (false);
		}
		
		public WelcomePageLinkButton ()
		{
			HasFrame = false;
			
			label = new Label () {
				Xalign = 0,


			};
			
			box = new Gtk.Box (Gtk.Orientation.Horizontal, 6);
			label.Hexpand = true;
			box.Append (label);
			Child = box;

			this.AddMotionHandler (null, (x, y) => UpdateLabel (true), () => UpdateLabel (false));
		}
		
		public string LinkUrl { get; private set; }
		
		public new string Label {
			get {
				return text;
			}
			set {
				text = value;
				UpdateLabel (false);
			}
		}
		
		public string Description {
			get {
				return desc;
			}
			set {
				desc = value;
				UpdateLabel (false);
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				UpdateImage ();
			}
		}
		
		void UpdateLabel (bool underlined)
		{
			string markup = string.Format (underlined? linkUnderlinedFormat : linkFormat, text);
			if (!string.IsNullOrEmpty (desc))
				markup += "\n" + string.Format (descFormat, desc);
			label.Markup = markup;
		}
		
		void UpdateImage ()
		{
			if (string.IsNullOrEmpty (icon)) {
				if (image != null) {
					box.Remove (image);
					image.Destroy ();
					image = null;
				}
				return;
			}
			if (image == null) {
				image = new ImageView ();
				box.Prepend (image);
			}
			image.Image = ImageService.GetIcon (icon, iconSize);
		}
		
		public int MaxWidthChars {
			get { return label.MaxWidthChars; }
			set { label.MaxWidthChars = value; }
		}
		
		public Pango.EllipsizeMode Ellipsize {
			get { return label.Ellipsize; }
			set { label.Ellipsize = value; }
		}
		
		protected override void OnClicked ()
		{
			base.OnClicked ();
			DispatchLink (LinkUrl);
		}
		
		void DispatchLink (string uri)
		{
			try {
				if (uri.StartsWith ("project://")) {
					string file = uri.Substring ("project://".Length);
					Gdk.ModifierType mtype = GtkWorkarounds.GetCurrentKeyModifiers ();
					bool inWorkspace = (mtype & Gdk.ModifierType.ControlMask) != 0;
					IdeApp.Workspace.OpenWorkspaceItem (file, !inWorkspace);
				} else if (uri.StartsWith ("monodevelop://")) {
					var cmdId = uri.Substring ("monodevelop://".Length);
					IdeApp.CommandService.DispatchCommand (cmdId, MonoDevelop.Components.Commands.CommandSource.WelcomePage);
				} else {
					IdeServices.DesktopService.ShowUrl (uri);
				}
			} catch (Exception ex) {
				LoggingService.LogInternalError (GettextCatalog.GetString ("Could not open the url '{0}'", uri), ex);
			}
		}
		
		string GetLinkTooltip (string link)
		{
			if (link == null)
				return "";
			if (link.IndexOf ("monodevelop://") != -1)
				return "";
				
			if (link.IndexOf ("project://") != -1) {
				string message = link;
				message = message.Substring (10);
				string msg = GettextCatalog.GetString ("Open solution {0}", message);
				if (IdeApp.Workspace.IsOpen)
					msg += " - " + GettextCatalog.GetString ("Hold Control key to open in current workspace.");
				return msg;
			} else {
				return GettextCatalog.GetString ("Open {0}", link);
			}
		}
	}
}