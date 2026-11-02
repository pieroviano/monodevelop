// VersionInformationTabPage.cs
//
// Author:
//   Viktoria Dudka (viktoriad@remobjects.com)
//   Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2009 RemObjects Software
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
using Gtk;
using MonoDevelop.Core;
using System.Reflection;
using System.Text;
using System.IO;
using MonoDevelop.Ide.Fonts;
using Mono.Addins;
using System.Collections.Generic;
using System.Linq;


namespace MonoDevelop.Ide.Gui.Dialogs
{
	internal class VersionInformationTabPage: Gtk.Box
	{
		bool destroyed;
		
		public VersionInformationTabPage () : base (Gtk.Orientation.Vertical, 0)
		{
			DesignerLayout.Margin (this, 6);
			SetLabel (GettextCatalog.GetString ("Loading..."));
			
			new System.Threading.Thread (() => {
				try {
					var info = SystemInformation.GetDescription ().ToArray ();
					Runtime.RunInMainThread (() => {
						if (destroyed)
							return;
						SetText (info);
					});
				} catch (Exception ex) {
					LoggingService.LogError ("Failed to load version information", ex);
					Runtime.RunInMainThread (() => {
						if (destroyed)
							return;
						SetLabel (GettextCatalog.GetString ("Failed to load version information."));
					});
				}
			}).Start ();
		}
		
		void Clear ()
		{
			var child = FirstChild;
			while (child != null) {
				var next = child.NextSibling;
				Remove (child);
				child = next;
			}
		}
		
		void SetLabel (string text)
		{
			Clear ();
			var label = new Gtk.Label (text);
			DesignerLayout.Pack (this, label, true);
		}

		void SetText (IEnumerable<ISystemInformationProvider> text)
		{
			Clear ();

			var buf = new Gtk.Label ();
			buf.Selectable = true;
			buf.Xalign = 0;

			StringBuilder sb = new StringBuilder ();

			foreach (var info in text) {
				sb.Append ("<b>").Append (GLib.Markup.EscapeText (info.Title)).Append ("</b>\n");
				sb.Append (GLib.Markup.EscapeText (info.Description.Trim ())).Append ("\n\n");
			}

			buf.Markup = sb.ToString ().Trim () + "\n";

			var contentBox = DesignerLayout.Margin (DesignerLayout.VBox (0), 4);
			DesignerLayout.Pack (contentBox, buf);

			var asmButton = new Gtk.Button { Label = GettextCatalog.GetString ("Show loaded assemblies") };
			asmButton.Halign = Gtk.Align.Start;
			asmButton.Clicked += (sender, e) => {
				asmButton.Visible = false;
				DesignerLayout.Pack (contentBox, CreateAssembliesTable ());
			};
			DesignerLayout.Pack (contentBox, asmButton);

			var sw = new MonoDevelop.Components.CompactScrolledWindow () {
				ShowBorderLine = true,
			};
			sw.Child = contentBox;
			sw.Hexpand = true;
			sw.Vexpand = true;
			DesignerLayout.Pack (this, sw, true);
		}

		Gtk.Widget CreateAssembliesTable ()
		{
			var box = DesignerLayout.VBox (0);
			DesignerLayout.Pack (box, new Gtk.Label () {
				Markup = string.Format ("<b>{0}</b>", GettextCatalog.GetString ("Loaded Assemblies")),
				Xalign = 0
			});
			var table = DesignerLayout.Grid (0, 3);
			int line = 0;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies ().Where (a => !a.IsDynamic).OrderBy (a => a.FullName)) {
				try {
					var assemblyName = assembly.GetName ();
					table.Attach (new Gtk.Label (assemblyName.Name) { Xalign = 0 }, 0, line, 1, 1);
					table.Attach (new Gtk.Label (assemblyName.Version.ToString ()) { Xalign = 0 }, 1, line, 1, 1);
					table.Attach (new Gtk.Label (System.IO.Path.GetFullPath (assembly.Location)) { Xalign = 0 }, 2, line, 1, 1);
				} catch {
				}
				line++;
			}
			DesignerLayout.Pack (box, table);
			return box;
		}

		// GTK 4 has no ::destroy: the page is gone once it is unrealized (removed from its toplevel)
		protected override void OnUnrealized ()
		{
			destroyed = true;
			base.OnUnrealized ();
		}
	}
}
