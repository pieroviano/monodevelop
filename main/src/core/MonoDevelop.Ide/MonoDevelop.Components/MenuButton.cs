// 
// MenuButton.cs
// 
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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

using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components
{
	
	
	[System.ComponentModel.Category("MonoDevelop.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class MenuButton : Gtk.Button
	{
		ContextMenuCreator contextMenuCreator;
		Gtk.Label label;
		ImageView image;
		Gtk.Image arrow;
		Gtk.ArrowType arrowType;
		Gtk.Box box;
		bool isOpen;
		
		public MenuButton ()
			: base ()
		{
			this.SetRole (AtkCocoa.Roles.AXMenuButton);

			box = new Gtk.Box (Gtk.Orientation.Horizontal, 6);
			Child = box;
			
			image = new ImageView ();
			image.SetShouldIgnore (true);
			image.Visible = false;
			box.Append (image);
			label = new Gtk.Label ();
			label.Visible = false;
			label.SetShouldIgnore (true);

			box.Append (label);
			ArrowType = Gtk.ArrowType.Down;
		}
		
		protected MenuButton (IntPtr raw)
			: base (raw)
		{
			
		}

		public ContextMenuCreator ContextMenuRequested {
			get { return contextMenuCreator; }
			set { contextMenuCreator = value; }
		}

		bool MenuOpened ()
		{
			isOpen = true;
			//make sure the button looks depressed while the menu is open
			bool wasFlat = HasCssClass ("flat");
			RemoveCssClass ("flat");
			SetStateFlags (Gtk.StateFlags.Active, false);
			return wasFlat;
		}

		void MenuClosed (bool wasFlat)
		{
			if (wasFlat)
				AddCssClass ("flat");
			isOpen = false;
			UnsetStateFlags (Gtk.StateFlags.Active);
		}

		protected override void OnClicked ()
		{
			base.OnClicked ();
			if (contextMenuCreator != null) {
				ContextMenu menu = contextMenuCreator (this);
				var wasFlat = MenuOpened ();

				this.GrabFocus ();
				// Offset the menu by the height of the button
				menu.Show (this, 0, Height, () => MenuClosed (wasFlat)); 
			}
		}
		
		static string GetArrowIconName (Gtk.ArrowType type)
		{
			switch (type) {
			case Gtk.ArrowType.Up: return "pan-up-symbolic";
			case Gtk.ArrowType.Left: return "pan-start-symbolic";
			case Gtk.ArrowType.Right: return "pan-end-symbolic";
			default: return "pan-down-symbolic";
			}
		}

		public Gtk.ArrowType? ArrowType {
			get { return arrow == null? (Gtk.ArrowType?) null : arrowType; }
			set {
				if (value == null) {
					if (arrow != null) {
						box.Remove (arrow);
						arrow = null;
					}
				} else {
					if (arrow == null ) {
						arrow = new Gtk.Image ();
						arrow.SetShouldIgnore (true);
						box.Append (arrow);
					}
					arrowType = value.Value;
					arrow.IconName = GetArrowIconName (arrowType);
				}
			}
		}
		
		public override void Destroy ()
		{
			contextMenuCreator = null;

			base.Destroy ();
		}

		public new string Label {
			get { return label.Text; }
			set {
				label.Text = value;
				label.Visible = !string.IsNullOrEmpty (value);

				this.SetLabel (value);
			}
		}
		
		public new bool UseUnderline {
			get { return label.UseUnderline; }
			set { label.UseUnderline = value; }
		}
		
		public string StockImage {
			set {
				image.SetIcon (value, StockIconSize.Button);
				image.Visible = true;
			}
		}
		
		public bool UseMarkup
		{
			get { return label.UseMarkup; }
			set { label.UseMarkup = value; }
		}
		
		public string Markup {
			set { label.Markup = value; }
		}
	}

	public delegate ContextMenu ContextMenuCreator (MenuButton button);
}
