//
// AlertDialog.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
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
using System.Collections.Generic;
using System.Text;
using Gtk;
using MonoDevelop.Core;
using System.Linq;
using MonoDevelop.Components;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	/// <summary>
	/// A Gnome HIG compliant alert dialog.
	/// </summary>
	internal class GtkAlertDialog : Gtk.Dialog
	{
		MessageDescription message;
		AlertButton resultButton = null;
		AlertButton[] buttons;
		
		Gtk.Box hbox = DesignerLayout.HBox (12);
		ImageView image;
		Gtk.Label label = new Label ();
		Gtk.Box labelsBox = DesignerLayout.VBox (6);
		List<Gtk.Button> actionButtons = new List<Gtk.Button> ();
		Gtk.Box buttonBar;
		
		public AlertButton ResultButton {
			get {
				return resultButton;
			}
		}
		
		public bool ApplyToAll { get; set; }
		
		void Init ()
		{
			DesignerLayout.Pack (ContentArea, hbox);
			DesignerLayout.Pack (hbox, labelsBox, true);
			DesignerLayout.Pack (labelsBox, label, true);
				
			// Table 3.1
			this.Title        = "";
			//this.Type         = WindowType.Toplevel;
			this.Resizable    = false;
			
			// Table 3.2
			DesignerLayout.Margin (this.ContentArea, 6);
			this.ContentArea.Spacing = 12;
			
			// Table 3.3
			DesignerLayout.Margin (this.hbox, 6);

			// Table 3.5
			this.label.UseMarkup = true;
			this.label.Wrap      = true;
			this.label.Yalign    = 0.00f;
			this.label.Xalign    = 0.00f;
		}
		
		public GtkAlertDialog (MessageDescription message)
		{
			Init ();
			this.buttons = message.Buttons.ToArray ();
			this.message = message;

			Modal = true;

			string primaryText;
			string secondaryText;
			
			if (string.IsNullOrEmpty (message.SecondaryText)) {
				secondaryText = message.Text;
				primaryText = null;
			} else {
				primaryText = message.Text;
				secondaryText = message.SecondaryText;
			}

			if (!message.UseMarkup) {
				primaryText = GLib.Markup.EscapeText (primaryText);
				secondaryText = GLib.Markup.EscapeText (secondaryText);
			}

			if (!string.IsNullOrEmpty (message.Icon)) {
				image = new ImageView ();
				image.Yalign   = 0.00f;
				image.Image = ImageService.GetIcon (message.Icon, StockIconSize.Dialog);
				hbox.Prepend (image);
			}
			
			StringBuilder markup = new StringBuilder (@"<span weight=""bold"" size=""larger"">");
			markup.Append (primaryText);
			markup.Append ("</span>");
			if (!String.IsNullOrEmpty (secondaryText)) {
				if (!String.IsNullOrEmpty (primaryText)) {
					markup.AppendLine ();
					markup.AppendLine ();
				}
				markup.Append (secondaryText);
			}
			label.Markup = markup.ToString ();
			label.Selectable = true;
			label.CanFocus = false;
			
			// The buttons notify the message and close the dialog themselves, so they are not response action widgets
			buttonBar = DialogButtonBar.Add (this);
			foreach (AlertButton button in message.Buttons) {
				Button newButton = DesignerLayout.Button (button.Label);
				if (!String.IsNullOrEmpty (button.Icon)) {
					var content = DesignerLayout.HBox (6);
					DesignerLayout.Pack (content, new ImageView (button.Icon, StockIconSize.Button));
					DesignerLayout.Pack (content, new Label (button.Label) { UseUnderline = true });
					newButton.Label = null;
					newButton.Child = content;
				}
				newButton.Clicked += ButtonClicked;
				actionButtons.Add (newButton);
				DesignerLayout.Pack (buttonBar, newButton);
			}
			
			foreach (var op in message.Options) {
				CheckButton check = new CheckButton { Label = op.Text };
				check.Active = op.Value;
				DesignerLayout.Pack (labelsBox, check);
				check.Toggled += delegate {
					message.SetOptionValue (op.Id, check.Active);
				};
			}
			
			if (message.AllowApplyToAll) {
				CheckButton check = new CheckButton { Label = GettextCatalog.GetString ("Apply to all") };
				DesignerLayout.Pack (labelsBox, check);
				check.Toggled += delegate {
					ApplyToAll = check.Active;
				};
			}
			
		}
		
		public void FocusButton (int buttonNumber)
		{
			if (buttonNumber == -1)
				buttonNumber = actionButtons.Count - 1;
			actionButtons [buttonNumber].GrabFocus ();
		}

		void ButtonClicked (object sender, EventArgs e) 
		{
			Gtk.Button clickButton = (Gtk.Button)sender;
			foreach (AlertButton alertButton in buttons) {
				if (clickButton == actionButtons [Array.IndexOf (buttons, alertButton)]) {
					resultButton = alertButton;
					break;
				}
			}
			bool close = message.NotifyClicked (resultButton);
			if (close)
				this.Destroy ();
		}
	}
}
