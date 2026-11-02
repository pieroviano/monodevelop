// 
// XmlFormattingPolicyPanelWidget.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Xml.Formatting
{
	class XmlFormattingPolicyPanelWidget : Box
	{
		XmlFormattingPolicy policy;
		ListStore store;
		
		public XmlFormattingPolicyPanelWidget () : base (Orientation.Vertical, 6)
		{
			Build ();
			
			store = new ListStore (typeof(string), typeof (XmlFormattingSettings));
			listView.Model = store;
			listView.AppendColumn ("", new CellRendererText (), "text", 0);
			
			listView.Selection.Changed += HandleListViewSelectionChanged;
			propertyGrid.ShowToolbar = false;
//			propertyGrid.PropertySort = MonoDevelop.Components.PropertyGrid.PropertySort.NoSort;
		}

		Box boxScopes;
		ScrolledWindow scrolledWindow;
		TreeView listView;
		Button buttonAdd;
		Button buttonRemove;
		Label labelScopes;
		Grid tableScopes;
		DesignerSupport.PropertyGridWrapper propertyGrid;
		Button buttonAdvanced;

		void Build ()
		{
			listView = new TreeView {
				CanFocus = true,
				HeadersVisible = false
			};

			scrolledWindow = new ScrolledWindow { HasFrame = true, Child = listView, Vexpand = true };
			buttonAdd = new Button { Label = GettextCatalog.GetString ("_Add"), UseUnderline = true };
			buttonRemove = new Button { Label = GettextCatalog.GetString ("_Remove"), UseUnderline = true };

			labelScopes = new Label {
				Xalign = 0F,
				Halign = Align.Start,
				Text = GettextCatalog.GetString ("Enter one or more XPath expressions to which this format applies")
			};

			tableScopes = new Grid {
				RowSpacing = 6,
				ColumnSpacing = 6
			};

			propertyGrid = new DesignerSupport.PropertyGridWrapper {
				ShowToolbar = false,
				ShowHelp = false
			};

			buttonAdvanced = new Button {
				Label = GettextCatalog.GetString ("Advanced Settings"),
				CanFocus = true,
				UseUnderline = true
			};

			var buttonBox = new Box (Orientation.Horizontal, 6);
			buttonBox.Append (buttonAdd);
			buttonBox.Append (buttonRemove);

			boxScopes = new Box (Orientation.Vertical, 6);
			boxScopes.Append (scrolledWindow);
			boxScopes.Append (buttonBox);

			var rightVBox = new Box (Orientation.Vertical, 6) { Hexpand = true };
			rightVBox.Append (labelScopes);
			rightVBox.Append (tableScopes);
			propertyGrid.Widget.Vexpand = true;
			rightVBox.Append (propertyGrid.Widget);

			var mainBox = new Box (Orientation.Horizontal, 6) { Vexpand = true };
			mainBox.Append (boxScopes);
			mainBox.Append (rightVBox);

			var abbBox = new Box (Orientation.Horizontal, 6);
			abbBox.Append (buttonAdvanced);

			Append (mainBox);
			Append (abbBox);

			buttonAdd.Clicked += OnButtonAddClicked;
			buttonRemove.Clicked += OnButtonRemoveClicked;
			buttonAdvanced.Clicked += OnButtonAdvancedClicked;
		}

		public void CommitPendingChanges ()
		{
			propertyGrid.CommitPendingChanges ();
		}
		
		public void SetFormat (XmlFormattingPolicy policy)
		{
			this.policy = policy;
			Update ();
			TreeIter it;
			if (store.GetIterFirst (out it))
				listView.Selection.SelectIter (it);
			if (policy.Formats.Count == 0) {
				boxScopes.Visible = false;
				buttonAdvanced.Visible = true;
			} else {
				boxScopes.Visible = true;
				buttonAdvanced.Visible = false;
			}
		}
		
		void Update ()
		{
			store.Clear ();
			AppendSettings (policy.DefaultFormat);
			foreach (XmlFormattingSettings s in policy.Formats)
				AppendSettings (s);
		}

		TreeIter AppendSettings (XmlFormattingSettings format)
		{
			return store.AppendValues (GetName (format), format);
		}
		
		string GetName (XmlFormattingSettings format)
		{
			if (format == policy.DefaultFormat)
				return GettextCatalog.GetString ("Default");
			
			string name = "";
			foreach (string s in format.ScopeXPath) {
				if (name.Length != 0)
					name += ", ";
				name += s;
			}
			if (name.Length != 0)
				return name;
			else {
				int i = policy.Formats.IndexOf (format) + 1;
				return string.Format (GettextCatalog.GetString ("Format #{0}"), i);
			}
		}
		
		void HandleListViewSelectionChanged (object sender, EventArgs e)
		{
			TreeIter it;
			if (listView.Selection.GetSelected (out it)) {
				var s = (XmlFormattingSettings) store.GetValue (it, 1);
				FillFormat (s);
			} else
				FillFormat (null);
			UpdateButtons ();
		}
		
		XmlFormattingSettings currentFormat;
		
		void FillFormat (XmlFormattingSettings format)
		{
			currentFormat = format;
			if (currentFormat != null && currentFormat.ScopeXPath.Count == 0)
				currentFormat.ScopeXPath.Add ("");
			propertyGrid.CurrentObject = format;
			UpdateScopes ();
			propertyGrid.Sensitive = currentFormat != null;
		}
		
		void UpdateScopes ()
		{
			if (currentFormat == policy.DefaultFormat || currentFormat == null) {
				tableScopes.Visible = false;
				labelScopes.Visible = false;
				return;
			}
			labelScopes.Visible = true;
			tableScopes.Visible = true;

			Widget child;
			while ((child = tableScopes.FirstChild) != null)
				tableScopes.Remove (child);

			for (int n = 0; n < currentFormat.ScopeXPath.Count; n++) {
				int capn = n;
				var la = new Label (GettextCatalog.GetString ("XPath scope:")) { Xalign = 0, Halign = Align.Start };
				tableScopes.Attach (la, 0, n, 1, 1);
				var en = new Entry { Hexpand = true };
				en.SetCommonAccessibilityAttributes ("XmlFormattingPolicyPanel.Entry", la,
				                                     GettextCatalog.GetString ("Enter a new XPath expression to which this format applies"));

				en.Text = currentFormat.ScopeXPath[capn];
				tableScopes.Attach (en, 1, n, 1, 1);
				en.Changed += delegate {
					currentFormat.ScopeXPath [capn] = en.Text;
					UpdateCurrentName ();
				};
				int c = 2;
				if (currentFormat.ScopeXPath.Count != 1) {
					var butRem = new Button { Child = new ImageView (GtkStock.Remove, StockIconSize.Menu) };
					butRem.SetCommonAccessibilityAttributes ("XmlFormattingPolicyPanel.Remove",
					                                         GettextCatalog.GetString ("Remove Scope"),
					                                         GettextCatalog.GetString ("Remove this scope expression"));
					tableScopes.Attach (butRem, 2, n, 1, 1);
					butRem.Clicked += delegate {
						currentFormat.ScopeXPath.RemoveAt (capn);
						UpdateScopes ();
						UpdateCurrentName ();
					};
					c++;
				}
				if (n == currentFormat.ScopeXPath.Count - 1) {
					var butAdd = new Button { Child = new ImageView (GtkStock.Add, StockIconSize.Menu) };
					butAdd.SetCommonAccessibilityAttributes ("XmlFormattingPolicyPanel.Add",
					                                         GettextCatalog.GetString ("Add Scope"),
					                                         GettextCatalog.GetString ("Add a new scope expression"));
					tableScopes.Attach (butAdd, c, n, 1, 1);
					butAdd.Clicked += delegate {
						currentFormat.ScopeXPath.Add ("");
						UpdateScopes ();
						UpdateCurrentName ();
					};
				}
			}
		}
		
		
		protected virtual void OnButtonAddClicked (object sender, EventArgs e)
		{
			var format = new XmlFormattingSettings ();
			policy.Formats.Add (format);
			TreeIter it = AppendSettings (format);
			listView.Selection.SelectIter (it);
		}
		
		protected virtual void OnButtonRemoveClicked (object sender, EventArgs e)
		{
			TreeIter it;
			if (listView.Selection.GetSelected (out it)) {
				var s = (XmlFormattingSettings) store.GetValue (it, 1);
				policy.Formats.Remove (s);
				TreePath p = store.GetPath (it);
				store.Remove (ref it);
				if (store.IterIsValid (it))
					listView.Selection.SelectIter (it);
				else {
					if (p.Prev ()) {
						store.GetIter (out it, p);
						listView.Selection.SelectIter (it);
					}
				}
			}
		}
		
		void UpdateCurrentName ()
		{
			TreeIter it;
			if (listView.Selection.GetSelected (out it)) {
				var s = (XmlFormattingSettings) store.GetValue (it, 1);
				store.SetValue (it, 0, GetName (s));
			}
		}
		
		void UpdateButtons ()
		{
			TreeIter it;
			if (listView.Selection.GetSelected (out it)) {
				var s = (XmlFormattingSettings) store.GetValue (it, 1);
				buttonAdd.Sensitive = true;
				buttonRemove.Sensitive = s != policy.DefaultFormat;
			} else {
				buttonAdd.Sensitive = buttonRemove.Sensitive = false;
			}
		}
		
		protected virtual void OnButtonAdvancedClicked (object sender, EventArgs e)
		{
			boxScopes.Visible = true;
			buttonAdvanced.Visible = false;
		}
	}
}

