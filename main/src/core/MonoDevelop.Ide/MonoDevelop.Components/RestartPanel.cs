//
// RestartPanel.cs
//
// Author:
//       iain <>
//
// Copyright (c) 2017 
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
using MonoDevelop.Core;

namespace MonoDevelop.Components
{
	public class RestartPanel : Gtk.Grid
	{
		public event EventHandler<EventArgs> RestartRequested;

		public RestartPanel ()
		{
			RowSpacing = 6;
			ColumnSpacing = 6;

			var btnRestart = new Gtk.Button () {
				Label = GettextCatalog.GetString ("Restart {0}", BrandingService.ApplicationName),
				CanFocus = true, UseUnderline = true,
				Halign = Gtk.Align.Start,
			};
			Attach (btnRestart, 1, 1, 1, 1);

			var imageRestart = new ImageView ("md-information", StockIconSize.Menu) {
				Valign = Gtk.Align.Start,
			};
			Attach (imageRestart, 0, 0, 1, 1);

			var labelRestart = new Gtk.Label (GettextCatalog.GetString ("These preferences will take effect next time you start {0}", BrandingService.ApplicationName)) {
				Xalign = 0,
			};
			Attach (labelRestart, 1, 0, 2, 1);

			imageRestart.SetCommonAccessibilityAttributes ("IDEStyleOptionsPanel.RestartImage", "",
			                                               GettextCatalog.GetString ("A restart is required before these changes take effect"));
			imageRestart.SetAccessibilityLabelRelationship (labelRestart);


			btnRestart.Clicked += (sender, e) => {
				RestartRequested?.Invoke (this, EventArgs.Empty);

			};
		}
	}
}
