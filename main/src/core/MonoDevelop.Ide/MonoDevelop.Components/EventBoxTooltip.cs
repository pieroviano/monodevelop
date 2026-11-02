//
// EventBoxTooltip.cs
//
// Author:
//       Therzok <teromario@yahoo.com>
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
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
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Ide;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Shows a <see cref="TooltipPopoverWindow"/> while the pointer is over (or the focus is in) a widget.
	/// GTK 4: works with any widget (GTK 2 required an EventBox); pointer/focus tracking uses event controllers.
	/// </summary>
	public class EventBoxTooltip : IDisposable
	{
		Gtk.Widget eventBox;
		TooltipPopoverWindow tooltipWindow;
		ImageView image;
		Xwt.Drawing.Image normalImage;
		Xwt.Drawing.Image activeImage;
		string tip;
		bool mouseOver;
		Gtk.EventControllerMotion motionController;
		Gtk.EventControllerFocus focusController;

		public Gtk.IAccessible Accessible {
			get {
				return eventBox;
			}
		}

		/// <summary>
		/// The widget should have Visible set to false otherwise the tooltip pop window
		/// will have the wrong location.
		/// </summary>
		public EventBoxTooltip (Gtk.Widget eventBox)
		{
			this.eventBox = eventBox ?? throw new ArgumentNullException (nameof (eventBox));
			eventBox.Focusable = true;
			eventBox.CanFocus = true;

			motionController = eventBox.AddMotionHandler (null, HandleEnterNotifyEvent, HandleLeaveNotifyEvent);
			focusController = eventBox.AddFocusHandler (HandleFocusInEvent, HandleFocusOutEvent);

			image = eventBox as ImageView ?? eventBox.FirstChild as ImageView;

			if (image?.Image != null) {
				normalImage = image.Image;
				var pixbuf = normalImage.ToPixbuf ();
				activeImage = pixbuf?.ColorShiftPixbuf ().ToXwtImage () ?? normalImage;
			}

			Position = PopupPosition.TopLeft;

			// Accessibility: Disguise this widget as a label
			eventBox.SetRole (AtkCocoa.Roles.AXStaticText);
		}

		void HandleLeaveNotifyEvent ()
		{
			mouseOver = false;
			HideTooltip ();
		}

		void HandleEnterNotifyEvent (double x, double y)
		{
			mouseOver = true;
			ShowTooltip ();
		}

		void HandleFocusOutEvent ()
		{
			if (image != null && normalImage != null)
				image.Image = normalImage;
			mouseOver = false;
			HideTooltip ();
		}

		void HandleFocusInEvent ()
		{
			if (image != null && activeImage != null)
				image.Image = activeImage;
			mouseOver = true;
			ShowTooltip ();
		}

		void UpdateAccessibility ()
		{
			eventBox.SetLabel (tip);
		}

		bool ShowTooltip ()
		{
			if (!string.IsNullOrEmpty (tip)) {
				HideTooltip ();
				tooltipWindow = TooltipPopoverWindow.Create ();
				tooltipWindow.ShowArrow = true;
				if (UseMarkup)
					tooltipWindow.Markup = tip;
				else
					tooltipWindow.Text = tip;
				tooltipWindow.Severity = Severity;
				var rect = new Gdk.Rectangle (0, 0, eventBox.Width, eventBox.Height + 5);
				tooltipWindow.ShowPopup (eventBox, rect, Position);
			}
			return false;
		}

		void HideTooltip ()
		{
			if (tooltipWindow != null) {
				tooltipWindow.Destroy ();
				tooltipWindow = null;
			}
		}

		public void Dispose ()
		{
			HideTooltip ();
			if (motionController != null) {
				eventBox.RemoveController (motionController);
				motionController = null;
			}
			if (focusController != null) {
				eventBox.RemoveController (focusController);
				focusController = null;
			}
		}

		public string ToolTip {
			get { return tip; }
			set {
				tip = value;
				if (tooltipWindow != null) {
					if (!string.IsNullOrEmpty (tip)) {
						if (UseMarkup)
							tooltipWindow.Markup = value;
						else
							tooltipWindow.Text = value;
					} else
						HideTooltip ();
				} else if (!string.IsNullOrEmpty (tip) && mouseOver)
					ShowTooltip ();
				UpdateAccessibility ();
			}
		}

		public TaskSeverity? Severity { get; set; }
		public PopupPosition Position { get; set; }
		public bool UseMarkup { get; set; }
	}
}
