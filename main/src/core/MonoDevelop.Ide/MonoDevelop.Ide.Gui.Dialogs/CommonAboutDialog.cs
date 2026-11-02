//  CommonAboutDialog.cs
//
// Author:
//   Todd Berman  <tberman@sevenl.net>
//   John Luke  <jluke@cfl.rr.com>
//   Lluis Sanchez Gual  <lluis@novell.com>
//   Viktoria Dudka  <viktoriad@remobjects.com>
//   Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2004 Todd Berman
// Copyright (c) 2004 John Luke
// Copyright (C) 2008 Novell, Inc.
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
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
//
//
using System;
using System.Text;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;

using Gdk;
using Gtk;
using GLib;
using Pango;
using System.IO;
using Mono.Addins;
using System.Collections.Generic;
using MonoDevelop.Components;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	public static class AboutDialogImage
	{
		public static string Name =  "AboutImage.png";
	}

	internal class CommonAboutDialog : IdeDialog
	{
		public CommonAboutDialog ()
		{
			Title = string.Format (GettextCatalog.GetString ("About {0}"), BrandingService.ApplicationName);
			Resizable = false;

			var notebook = new Notebook ();
			notebook.ShowTabs = false;
			notebook.ShowBorder = false;
			notebook.AppendPage (new AboutMonoDevelopTabPage (), new Label (Title));
			notebook.AppendPage (new VersionInformationTabPage (), new Label (GettextCatalog.GetString ("Version Information")));
			notebook.Vexpand = true;
			DesignerLayout.Pack (ContentArea, notebook, true);
			
			var copyButton = DesignerLayout.Button (GettextCatalog.GetString ("Copy Information"));
			copyButton.Clicked += (sender, e) => CopyBufferToClipboard ();
			copyButton.Visible = false;
			var buttonBar = DialogButtonBar.Add (this);
			DesignerLayout.Pack (buttonBar, copyButton);

			var backButton = DesignerLayout.Button (GettextCatalog.GetString ("Show Details"));
			DesignerLayout.Pack (buttonBar, backButton);
			backButton.Clicked += (sender, e) => {
				if (notebook.Page == 0) {
					backButton.Label = GettextCatalog.GetString ("Hide Details");
					copyButton.Visible = true;
					notebook.Page = 1;
				}
				else {
					backButton.Label = GettextCatalog.GetString ("Show Details");
					copyButton.Visible = false;
					notebook.Page = 0;
				}
			};
			DefaultWidget = backButton;
		}

		static void CopyBufferToClipboard ()
		{
			var text = SystemInformation.GetTextDescription ().ToString ();
			ClipboardHelper.SetText (text);
			ClipboardHelper.SetText (text, true);
		}

		static CommonAboutDialog instance;

		public static void ShowAboutDialog ()
		{
			if (Platform.IsMac) {
				if (instance == null) {
					var parent = IdeServices.DesktopService.GetFocusedTopLevelWindow ();
					instance = new CommonAboutDialog ();
					MessageService.PlaceDialog (instance, parent);
					instance.Response += delegate {
						instance.Destroy ();
						instance = null;
					};
				}

				instance.Present ();
				return;
			}

			using (var dlg = new CommonAboutDialog ())
				MessageService.ShowCustomDialog (dlg);
		}
	}
}