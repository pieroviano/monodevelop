// 
// MiniButton.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components
{
	class MiniButton: BinWidget
	{
		public event EventHandler Clicked;
		Gdk.RGBA normalColor;
		bool pressed;
		bool highligted;
		bool backgroundColorSet;
		bool hilightBackground;

		public MiniButton ()
		{
			Clickable = true;

			this.SetRole (AtkCocoa.Roles.AXButton);
			var actionHandler = new ActionDelegate (this);
			actionHandler.PerformPress += PerformPress;

			this.AddClickHandler (OnPressed, OnReleased, 1);
			this.AddMotionHandler (null, (x, y) => OnEnter (), OnLeave);
		}

		public MiniButton (Gtk.Widget label): this ()
		{
			Child = label;
		}

		public MiniButton (string text): this ()
		{
			Child = new Gtk.Label (text);
		}

		public MiniButton (string text, string icon): this ()
		{
			var box = new Gtk.Box (Gtk.Orientation.Horizontal, 3);
			var img = new ImageView (icon, StockIconSize.Menu);
			box.Append (img);
			var label = new Gtk.Label (text) {
				Xalign = 0,
				Hexpand = true,
			};
			box.Append (label);
			Child = box;
		}

		public bool Clickable { get; set; }

		public bool ToggleMode { get; set; }

		public bool ClickOnRelease { get; set; }

		public Gdk.RGBA BackroundColor {
			get { return normalColor; }
			set {
				normalColor = value;
				backgroundColorSet = true;
				QueueDraw ();
			}
		}

		public bool Pressed {
			get {
				return pressed;
			}
			set {
				if (ToggleMode && Clickable && pressed != value) {
					pressed = value;
					SetBg (value || highligted);
				}
			}
		}

		protected virtual void OnClicked ()
		{
			if (ToggleMode)
				Pressed = !pressed;
			if (Clicked != null)
				Clicked (this, EventArgs.Empty);
		}

		void PerformPress (object sender, EventArgs args)
		{
			OnClicked ();
		}

		void OnPressed (ClickInfo click)
		{
			if (!ClickOnRelease && Clickable && !click.IsContextMenuTrigger) {
				click.Claim ();
				OnClicked ();
			}
		}

		void OnReleased (ClickInfo click)
		{
			if (ClickOnRelease && !click.IsContextMenuTrigger)
				OnClicked ();
		}

		void OnEnter ()
		{
			if (Clickable) {
				highligted = true;
				if (!ToggleMode || !pressed)
					SetBg (true);
			}
		}

		void OnLeave ()
		{
			if (Clickable) {
				highligted = false;
				if (!ToggleMode || !pressed)
					SetBg (false);
			}
		}

		void SetBg (bool hilight)
		{
			hilightBackground = hilight;
			QueueDraw ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (hilightBackground || backgroundColorSet) {
				var color = hilightBackground ? Styles.BaseBackgroundColor.ToGdkColor () : normalColor;
				snapshot.AppendColor (color, new Gdk.Rectangle (0, 0, Width, Height).ToGrapheneRect ());
			}
			base.OnSnapshot (snapshot);
		}
	}
}
