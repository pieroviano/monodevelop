//
// GtkReferenceAssembliesOptionsPanelWidget.UI.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2016 Xamarin Inc. (http://xamarin.com)
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

using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace MonoDevelop.Packaging.Gui
{
	partial class GtkReferenceAssembliesOptionsPanelWidget : MonoDevelop.Components.BinWidget
	{
		const int IsEnabledCheckBoxColumn = 0;

		ListStore pclProfilesStore;
		TreeView pclProfilesTreeView;

		void Build ()
		{

			var vbox = new Box (Orientation.Vertical, 6);

			var referenceAssembliesLabelHBox = new Box (Orientation.Horizontal, 6);

			var referenceAssembliesLabel = new Label ();
			referenceAssembliesLabel.Markup = GetBoldMarkup (GettextCatalog.GetString ("Choose the reference assemblies for your NuGet package."));
			referenceAssembliesLabel.UseMarkup = true;
			referenceAssembliesLabel.Xalign = 0;
			referenceAssembliesLabelHBox.Append (referenceAssembliesLabel);

			var learnMoreLabel = new Label ();
			learnMoreLabel.Xalign = 0F;
			learnMoreLabel.LabelProp = GettextCatalog.GetString ("<a href=\"https://docs.nuget.org\">Learn more</a>");
			learnMoreLabel.UseMarkup = true;
			learnMoreLabel.SetLinkHandler (IdeServices.DesktopService.ShowUrl);
			referenceAssembliesLabelHBox.Append (learnMoreLabel);

			referenceAssembliesLabelHBox.MarginTop = referenceAssembliesLabelHBox.MarginBottom = 5;
			vbox.Append (referenceAssembliesLabelHBox);


			var scrolledWindow = new ScrolledWindow ();
			scrolledWindow.HasFrame = true;
			scrolledWindow.Hexpand = true;
			scrolledWindow.Vexpand = true;
			pclProfilesTreeView = new TreeView ();
			pclProfilesTreeView.CanFocus = true;
			pclProfilesTreeView.Name = "pclProfilesTreeView";
			pclProfilesTreeView.HeadersVisible = true;
			scrolledWindow.Child = pclProfilesTreeView;
			pclProfilesTreeView.SearchColumn = -1; // disable the interactive search
			pclProfilesTreeView.AppendColumn (CreateCheckBoxTreeViewColumn ());
			pclProfilesTreeView.AppendColumn (CreateProfileTreeViewColumn ());
			pclProfilesTreeView.AppendColumn (CreateProfileDescriptionTreeViewColumn ());

			pclProfilesStore = new ListStore (typeof (bool), typeof (string), typeof (string), typeof (object));
			pclProfilesTreeView.Model = pclProfilesStore;

			vbox.Append (scrolledWindow);

			Child = vbox;

		}

		static string GetBoldMarkup (string text)
		{
			return "<b>" + text + "</b>";
		}

		TreeViewColumn CreateCheckBoxTreeViewColumn ()
		{
			var column = new TreeViewColumn ();

			var checkBoxRenderer = new CellRendererToggle ();
			checkBoxRenderer.Toggled += PortableProfileCheckBoxToggled;
			column.PackStart (checkBoxRenderer, false);
			column.AddAttribute (checkBoxRenderer, "active", IsEnabledCheckBoxColumn);

			return column;
		}

		TreeViewColumn CreateProfileTreeViewColumn ()
		{
			var column = new TreeViewColumn ();
			column.Title = GettextCatalog.GetString ("Profile");

			var textRenderer = new CellRendererText ();
			column.PackStart (textRenderer, true);
			column.AddAttribute (textRenderer, "text", column: 1);

			return column;
		}

		TreeViewColumn CreateProfileDescriptionTreeViewColumn ()
		{
			var column = new TreeViewColumn ();
			column.Title = GettextCatalog.GetString ("Description");

			var textRenderer = new CellRendererText ();
			column.PackStart (textRenderer, true);
			column.AddAttribute (textRenderer, "text", column: 2);

			return column;
		}
	}
}
