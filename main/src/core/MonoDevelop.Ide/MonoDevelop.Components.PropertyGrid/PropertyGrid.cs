/* 
 * PropertyGrid.cs - A Gtk# widget that displays and allows 
 * editing of all of an object's public properties 
 *
 * Authors:
 *  Michael Hutchinson <m.j.hutchinson@gmail.comk>
 * 	Eric Butler <eric@extremeboredom.net>
 *  Lluis Sanchez Gual <lluis@novell.com>
 *
 * Copyright (C) 2005 Michael Hutchinson
 * Copyright (C) 2005 Eric Butler
 * Copyright (C) 2007 Novell, Inc (http://www.novell.com)
 *
 * This sourcecode is licenced under The MIT License:
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;

using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Components.PropertyGrid.PropertyEditors;
using System.Collections.Generic;
using System.Linq;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Ide;

namespace MonoDevelop.Components.PropertyGrid
{
	[System.ComponentModel.Category("MonoDevelop.Components")]
	[System.ComponentModel.ToolboxItem(true)]
	public class PropertyGrid: Gtk.Box, IPropertyGrid
	{
		object currentObject;
		object[] propertyProviders;

		PropertyGridTable tree;
		Separator helpSeparator;
		Separator toolbarSeparator;
		Paned vpaned;
		
		IToolbarProvider toolbar;
		ToggleButton catButton;
		ToggleButton alphButton;

		string descTitle, descText;
		Label descTitleLabel;
		TextView descTextView;
		Gtk.Widget descFrame;
		TextTag descTextFontTag;
		
		EditorManager editorManager;
		
		PropertySort propertySort = PropertySort.Categorized;
		
		const string PROP_HELP_KEY = "MonoDevelop.PropertyPad.ShowHelp";

		public Widget Widget => this;

		public PropertyGrid (): this (new EditorManager ())
		{
		}
		
		internal PropertyGrid (EditorManager editorManager) : base (Orientation.Vertical, 0)
		{
			this.editorManager = editorManager;
			
			#region Toolbar
			
			PropertyGridToolbar tb = new PropertyGridToolbar ();
			base.Append (tb);
			toolbar = tb;
			
			catButton = new ToggleButton ();
			catButton.Name = "MonoDevelop.PropertyGridToolbar.GtkRadioButton";
			catButton.HasFrame = false;
			catButton.Child = new ImageView (MonoDevelop.Ide.Gui.Stock.GroupByCategory, StockIconSize.Menu);
			catButton.TooltipText = GettextCatalog.GetString ("Sort in categories");
			catButton.Toggled += new EventHandler (toolbarClick);
			toolbar.Insert (catButton, 0);
			
			alphButton = new ToggleButton ();
			alphButton.Group = catButton;
			alphButton.Name = "MonoDevelop.PropertyGridToolbar.GtkRadioButton";
			alphButton.HasFrame = false;
			alphButton.Child = new ImageView (MonoDevelop.Ide.Gui.Stock.SortAlphabetically, StockIconSize.Menu);
			alphButton.TooltipText = GettextCatalog.GetString ("Sort alphabetically");
			alphButton.Toggled += new EventHandler (toolbarClick);
			toolbar.Insert (alphButton, 1);
			
			catButton.Active = true;
			
			#endregion

			vpaned = new Paned (Orientation.Vertical);
			vpaned.Vexpand = true;

			tree = new PropertyGridTable (editorManager, this);
			tree.Changed += delegate {
				Update ();
			};

			CompactScrolledWindow sw = new CompactScrolledWindow ();
			sw.Child = tree;
			sw.HasFrame = false;
			sw.Vexpand = true;
			sw.HscrollbarPolicy = PolicyType.Never;
			sw.VscrollbarPolicy = PolicyType.Automatic;

			var tbox = new Box (Orientation.Vertical, 0);
			toolbarSeparator = new Separator (Orientation.Horizontal);
			toolbarSeparator.Visible = true;
			tbox.Append (toolbarSeparator);
			tbox.Append (sw);
			helpSeparator = new Separator (Orientation.Horizontal);
			helpSeparator.Visible = false;
			tbox.Append (helpSeparator);
			vpaned.StartChild = tbox;
			vpaned.ResizeStartChild = true;
			vpaned.ShrinkStartChild = true;
			
			AddPropertyTab (new DefaultPropertyTab ());
			AddPropertyTab (new EventPropertyTab ());

			base.Append (vpaned);
			
			Populate (saveEditSession: false);
			UpdateTabs ();
		}

		public ISite Site { get; set; }
		
		public void SetToolbarProvider (IToolbarProvider toolbarProvider)
		{
			PropertyGridToolbar t = toolbar as PropertyGridToolbar;
			if (t == null)
				throw new InvalidOperationException ("Custom toolbar provider already set");
			base.Remove (t);
			foreach (Widget w in t.Children) {
				t.Remove (w);
				toolbarProvider.Insert (w, -1);
			}
			toolbarSeparator.Hide ();
			toolbar = toolbarProvider;
			UpdateTabs ();
		}
		
		public event EventHandler Changed {
			add { tree.Changed += value; }
			remove { tree.Changed -= value; }
		}
			
		internal EditorManager EditorManager {
			get { return editorManager; }
		}
		
		#region Toolbar state and handlers
		
		private const int FirstTabIndex = 3;
		
		void toolbarClick (object sender, EventArgs e)
		{
			if (sender == alphButton || sender == catButton) {
				// radio group: only react to the button that became active
				if (!((ToggleButton) sender).Active)
					return;
				PropertySort = sender == alphButton ? PropertySort.Alphabetical : PropertySort.Categorized;
			}
			else {
				TabRadioToolButton button = (TabRadioToolButton) sender;
				if (!button.Active || selectedTab == button.Tab) return;
				selectedTab = button.Tab;
				Populate (saveEditSession: true);
			}
			// If the tree is re-populated while a value is being edited, the focus that the value editor had
			// is not returned back to the tree. We need to explicitly get it.
			tree?.GrabFocus ();
		}
		
		public PropertySort PropertySort {
			get { return propertySort; }
			set {
				if (value != propertySort) {
					propertySort = value;
					Populate (saveEditSession: true);
				}
			}
		}
		
		ArrayList propertyTabs = new ArrayList ();
		PropertyTab selectedTab;
		
		public PropertyTab SelectedTab {
			get { return selectedTab; }
		}
		
		TabRadioToolButton firstTab;
		Separator tabSectionSeparator;
		
		private void AddPropertyTab (PropertyTab tab)
		{
			TabRadioToolButton rtb;
			if (propertyTabs.Count == 0) {
				selectedTab = tab;
				rtb = new TabRadioToolButton ();
				rtb.Active = true;
				firstTab = rtb;
				toolbar.Insert (tabSectionSeparator = new Separator (Orientation.Vertical), FirstTabIndex - 1);
			}
			else
				rtb = new TabRadioToolButton ();
				rtb.Group = firstTab;
			
			//load image from PropertyTab's bitmap
			var icon = tab.GetIcon ();
			if (icon != null)
				rtb.Child = new ImageView (icon);
			else
				rtb.Child = new ImageView (MonoDevelop.Ide.ImageService.GetIcon ("gtk-missing-image", StockIconSize.Menu));
			
			rtb.Tab = tab;
			rtb.TooltipText = tab.TabName;
			rtb.Toggled += new EventHandler (toolbarClick);	
			
			toolbar.Insert (rtb, propertyTabs.Count + FirstTabIndex);
			
			propertyTabs.Add(tab);
		}
			
		#endregion
		
		public object CurrentObject {
			get { return currentObject; }
			set { SetCurrentObject (value, new object[] {value}); }
		}
		
		public void SetCurrentObject (object obj, object[] propertyProviders)
		{
			if (this.currentObject == obj)
				return;
			if (this.propertyProviders != null) {
				foreach (var old in this.propertyProviders.OfType<IDisposable> ())
					old.Dispose ();
			}
			this.currentObject = obj;
			this.propertyProviders = propertyProviders;
			UpdateTabs ();
			Populate(saveEditSession: false);
		}
		
		public void CommitPendingChanges ()
		{
			tree.CommitChanges ();
		}
		
		void UpdateTabs ()
		{
			bool visible = currentObject != null && toolbar.Children.OfType<TabRadioToolButton> ().Count (but => but.Tab.CanExtend (currentObject)) > 1;
			foreach (var w in toolbar.Children.OfType<TabRadioToolButton> ())
				w.Visible = visible;
			if (tabSectionSeparator != null)
				tabSectionSeparator.Visible = visible;
		}
	
		//TODO: add more intelligence for editing state etc. Maybe need to know which property has changed, then 
		//just update that
		public void Refresh ()
		{
			Update (); 
			QueueDraw ();
		}

		public bool IsEditing {
			get { return tree.IsEditing; } 
		}
		
		public void Populate (bool saveEditSession)
		{
			PropertyDescriptorCollection properties;
			
			tree.SaveStatus ();
			if (saveEditSession)
				tree.SaveEditSession ();
			tree.Clear ();
			tree.PropertySort = propertySort;
			
			if (currentObject == null) {
				properties = new PropertyDescriptorCollection (new PropertyDescriptor[0] {});
				tree.Populate (properties, currentObject);
			}
			else {
				foreach (object prov in propertyProviders) {
					properties = selectedTab.GetProperties (prov);
					tree.Populate (properties, prov);
				}
			}
			tree.RestoreStatus ();
			tree.RestoreEditSession ();
		}
		
		void Update ()
		{
			PropertyDescriptorCollection properties;
			
			if (currentObject == null) {
				properties = new PropertyDescriptorCollection (new PropertyDescriptor[0] {});
				tree.Update (properties, currentObject);
			}
			else {
				foreach (object prov in propertyProviders) {
					properties = selectedTab.GetProperties (prov);
					tree.Update (properties, prov);
				}
			}
		}
		
		public bool ShowToolbar {
			get { return toolbar.Visible; }
			set { toolbar.Visible = toolbarSeparator.Visible = value; }
		}
		
		#region Hel Pane
		
		public bool ShowHelp
		{
			get { return descFrame != null; }
			set {
				// Disable for now. Documentation is shown in tooltips
/*				if (value == ShowHelp)
					return;
				if (value) {
					AddHelpPane ();
					helpSeparator.Show ();
				} else {
					vpaned.Remove (descFrame);
					descFrame.Destroy ();
					descFrame = null;
					descTextView = null;
					descTitleLabel = null;
					helpSeparator.Hide ();
				}*/
			}
		}
		
		void AddHelpPane ()
		{
			var desc = new Box (Orientation.Vertical, 0);

			descTitleLabel = new Label ();
			descTitleLabel.Xalign = 0;
			descTitleLabel.Yalign = 0;
			descTitleLabel.MarginStart = descTitleLabel.MarginEnd = 5;
			descTitleLabel.MarginTop = descTitleLabel.MarginBottom = 2;
			descTitleLabel.UseMarkup = true;
			desc.Append (descTitleLabel);

			ScrolledWindow textScroll = new ScrolledWindow ();
			textScroll.SetPolicy (PolicyType.Never, PolicyType.Automatic);
			textScroll.Vexpand = true;
			desc.Append (textScroll);
			
			//TODO: Use label, but wrapping seems dodgy.
			descTextView = new TextView ();
			descTextView.WrapMode = Gtk.WrapMode.Word;
			descTextView.WidthRequest = 1;
			descTextView.HeightRequest = 70;
			descTextView.Editable = false;
			descTextView.LeftMargin = 5;
			descTextView.RightMargin = 5;
			descTextFontTag = new TextTag ("help-font") {
				FontDesc = IdeServices.FontService.SansFont.CopyModified (Ide.Gui.Styles.FontScale11)
			};
			descTextView.Buffer.TagTable.Add (descTextFontTag);
			
			textScroll.Child = descTextView;
			
			descFrame = desc;
			vpaned.EndChild = descFrame;
			vpaned.ResizeEndChild = false;
			vpaned.ShrinkEndChild = true;
			UpdateHelp ();
		}
		
		public void SetHelp (string title, string description)
		{
			descTitle = title;
			descText = description;
			UpdateHelp ();
		}
		
		void UpdateHelp ()
		{
			if (!ShowHelp)
				return;
			descTextView.Buffer.Text = descText ?? string.Empty;
			TextIter start, end;
			descTextView.Buffer.GetBounds (out start, out end);
			descTextView.Buffer.ApplyTag (descTextFontTag, start, end);
			descTitleLabel.Markup = descTitle != null?
				"<b>" + descTitle + "</b>" : string.Empty;
		}

		public void ClearHelp()
		{
			descTitle = descText = null;
			UpdateHelp ();
		}

		public void BlankPad () => CurrentObject = null;

		public void OnPadContentShown ()
		{
			//not implemented
		}

		public interface IToolbarProvider
		{
			/// <summary>Inserts <paramref name="w"/> at position <paramref name="pos"/> (-1 appends).</summary>
			void Insert (Widget w, int pos);
			Widget[] Children { get; }
			bool Visible { get; set; }
		}
		
		class PropertyGridToolbar: Box, IToolbarProvider
		{
			public PropertyGridToolbar () : base (Orientation.Horizontal, 3)
			{
			}
			
			public void Insert (Widget w, int pos)
			{
				if (pos <= 0 && pos != -1) {
					Prepend (w);
					return;
				}
				var previous = FirstChild;
				for (int i = 1; pos != -1 && previous != null && i < pos; i++)
					previous = previous.NextSibling;
				if (pos == -1 || previous == null)
					Append (w);
				else
					InsertChildAfter (w, previous);
			}

			public Widget[] Children {
				get {
					var list = new List<Widget> ();
					for (var c = FirstChild; c != null; c = c.NextSibling)
						list.Add (c);
					return list.ToArray ();
				}
			}
		}
		
		#endregion
	}
	
	class TabRadioToolButton: ToggleButton
	{
		public TabRadioToolButton ()
		{
			HasFrame = false;
			Visible = false;
			Name = "MonoDevelop.PropertyGridToolbar.GtkRadioButton";
		}
		
		public PropertyTab Tab;
	}
	
	public enum PropertySort
	{
		NoSort = 0,
		Alphabetical,
		Categorized,
		CategorizedAlphabetical
	}
}
