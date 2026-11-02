// 
// InfoBar.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
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

namespace MonoDevelop.Components
{

	/// <summary>
	/// Widget for displaying an informational non-modal question or message at the top of a content area.
	/// Very loosely modelled on the GTK+ 2.18 InfoBar widget.
	/// </summary>
	public class InfoBar : Gtk.Box
	{
		ImageView img;
		Gtk.MessageType messageType;
		Gtk.Box messageHBox = new Gtk.Box (Gtk.Orientation.Horizontal, 0);

		public InfoBar () : this (Gtk.MessageType.Other)
		{
		}

		public InfoBar (Gtk.MessageType messageType) : base (Gtk.Orientation.Horizontal, 6)
		{
			// GTK 2 BorderWidth = 6 (inside the drawn background)
			var messageVBox = new Gtk.Box (Gtk.Orientation.Vertical, 0) {
				Hexpand = true,
				MarginStart = 6, MarginTop = 6, MarginBottom = 6,
			};
			MessageArea = new Gtk.Box (Gtk.Orientation.Vertical, 0) { Hexpand = true };
			messageHBox.Append (MessageArea);
			messageVBox.Append (messageHBox);
			Append (messageVBox);

			ActionArea = new Gtk.Box (Gtk.Orientation.Vertical, 6) {
				Valign = Gtk.Align.Start,
				MarginEnd = 6, MarginTop = 6, MarginBottom = 6,
			};
			Append (ActionArea);

			this.MessageType = messageType;
		}

		public Gtk.MessageType MessageType {
			get { return messageType; }
			set {
				messageType = value;
				UpdateImage ();
			}
		}

		void SetImage (string iconName)
		{
			if (iconName == null) {
				if (img != null) {
					messageHBox.Remove (img);
					img = null;
				}
				return;
			}

			if (img == null) {
				img = new ImageView (iconName, StockIconSize.Dialog) {
					Halign = Gtk.Align.Center,
					Valign = Gtk.Align.Center,
					MarginStart = 12,
					MarginEnd = 12,
				};
				messageHBox.Prepend (img);
			} else {
				img.IconId = iconName;
			}
		}

		void UpdateImage ()
		{
			switch (messageType) {
			case Gtk.MessageType.Error:
				SetImage ("gtk-dialog-error");
				break;
			case Gtk.MessageType.Info:
				SetImage ("gtk-dialog-info");
				break;
			case Gtk.MessageType.Warning:
				SetImage ("gtk-dialog-warning");
				break;
			case Gtk.MessageType.Question:
				SetImage ("gtk-dialog-question");
				break;
			case Gtk.MessageType.Other:
			default:
				SetImage (null);
				break;
			}
		}

		/// <summary>Vertical box for the action buttons (use <c>Append</c>).</summary>
		public Gtk.Box ActionArea { get; private set; }
		/// <summary>Vertical box for the message widgets (use <c>Append</c>).</summary>
		public Gtk.Box MessageArea { get; private set; }

		public void SetMessageLabel (string markup)
		{
			Gtk.Widget child;
			while ((child = MessageArea.FirstChild) != null)
				MessageArea.Remove (child);

			var l = new Gtk.Label () {
				Wrap = true,
				Selectable = true,
				Yalign = 0.5f,
				Xalign = 0f,
				WidthRequest = 350,
				Hexpand = true,
			};
			l.Markup = markup;
			MessageArea.Append (l);
		}

		// Styled like a tooltip
		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			snapshot.DrawCairo (this, cr => {
				cr.Rectangle (0.5, 0.5, Width - 1, Height - 1);
				cr.SetSourceColor (Styles.PopoverWindow.DefaultBackgroundColor.ToCairoColor ());
				cr.FillPreserve ();
				cr.LineWidth = 1;
				cr.SetSourceColor (Styles.PopoverWindow.BorderColor.ToCairoColor ());
				cr.Stroke ();
			});
			base.OnSnapshot (snapshot);
		}
	}
}
