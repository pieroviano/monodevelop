//
// TabStrip.cs
//
// Author:
//   Lluis Sanchez Gual
//

//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using Gtk;

using System;
using System.Collections.Generic;
using MonoDevelop.Ide.Gui;
using System.Linq;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Core;
using MonoDevelop.Ide;

namespace MonoDevelop.Components.Docking
{
	/// <summary>
	/// Row of pad title tabs. The tabs are the children of the strip and are laid out from the strip itself.
	/// </summary>
	class TabStrip: ContainerWidget
	{
		int currentTab = -1;
		int bottomPadding;
		DockVisualStyle visualStyle;
		static Xwt.Drawing.Image tabbarBackImage = Xwt.Drawing.Image.FromResource ("tabbar-back.9.png");

		public TabStrip (DockFrame frame)
		{
			this.SetRole (AtkCocoa.Roles.AXTabGroup);
			this.SetCommonAttributes ("Docking.TabStrip",
			                          GettextCatalog.GetString ("Pad Tab Bar"),
			                          GettextCatalog.GetString ("The different pads in this dock position"));

			BottomPadding = 3;
		}

		public int BottomPadding {
			get { return bottomPadding; }
			set { bottomPadding = value; }
		}

		public DockVisualStyle VisualStyle {
			get { return visualStyle; }
			set {
				visualStyle = value;
				QueueDraw ();
			}
		}

		IEnumerable<DockItemTitleTab> Tabs => Children.Cast<DockItemTitleTab> ();

		internal bool IsFirstTab (DockItemTitleTab tab) => Children.Count > 0 && Children [0] == tab;

		internal bool IsLastTab (DockItemTitleTab tab) => Children.Count > 0 && Children [Children.Count - 1] == tab;

		int IndexOf (DockItemTitleTab tab)
		{
			for (int n = 0; n < Children.Count; n++) {
				if (Children [n] == tab)
					return n;
			}
			return -1;
		}

		public void AddTab (DockItemTitleTab tab)
		{
			DockUtil.RemoveFromParent (tab);

			AddChild (tab);

			if (currentTab == -1)
				CurrentTab = Children.Count - 1;
			else {
				tab.Active = false;
				tab.Page.Hide ();
			}

			tab.TabPressed += OnTabPress;
			tab.UpdateRole (true, this);

			UpdateAccessibilityTabs ();
		}

		protected override void OnChildRemoved (Gtk.Widget child)
		{
			var w = (DockItemTitleTab)child;
			w.UpdateRole (false, this);

			w.TabPressed -= OnTabPress;
			if (currentTab >= Children.Count)
				currentTab = Children.Count - 1;

			UpdateAccessibilityTabs ();
		}

		public void SetTabLabel (Gtk.Widget page, Xwt.Drawing.Image icon, string label)
		{
			foreach (DockItemTitleTab tab in Tabs) {
				if (tab.Page == page) {
					tab.SetLabel (page, icon, label);
					QueueResize ();
					break;
				}
			}
		}

		void UpdateAccessibilityTabs ()
		{
			this.SetTabs (Tabs.Cast<Gtk.IAccessible> ().ToArray ());
		}

		public void UpdateStyle (DockItem item)
		{
			QueueResize ();
		}

		public int TabCount {
			get { return Children.Count; }
		}

		public int CurrentTab {
			get { return currentTab; }
			set {
				if (currentTab == value)
					return;
				if (currentTab != -1) {
					DockItemTitleTab t = (DockItemTitleTab) Children [currentTab];
					t.Page.Hide ();
					t.Active = false;
				}
				currentTab = value;
				if (currentTab != -1) {
					DockItemTitleTab t = (DockItemTitleTab) Children [currentTab];
					t.Active = true;
					t.Page.Show ();
				}
			}
		}

		internal DockItemTitleTab CurrentTitleTab {
			get {
				if (currentTab != -1)
					return (DockItemTitleTab)Children [currentTab];
				return null;
			}
		}

		public Widget CurrentPage {
			get {
				if (currentTab != -1) {
					var t = (DockItemTitleTab) Children [currentTab];
					return t.Page;
				} else
					return null;
			}
			set {
				if (value != null) {
					for (int n = 0; n < Children.Count; n++) {
						var tab = (DockItemTitleTab) Children [n];
						if (tab.Page == value) {
							CurrentTab = n;
							return;
						}
					}
				}
				CurrentTab = -1;
			}
		}

		public void Clear ()
		{
			currentTab = -1;
			foreach (DockItemTitleTab w in Tabs.ToArray ())
				RemoveChild (w);
		}

		void OnTabPress (object s, EventArgs args)
		{
			CurrentTab = IndexOf ((DockItemTitleTab)s);
			DockItemTitleTab t = (DockItemTitleTab) s;
			DockItem.SetFocus (t.Page);
			QueueDraw ();
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = 0;
			if (orientation == Gtk.Orientation.Horizontal) {
				foreach (var tab in Tabs)
					minimum += tab.MinWidth;
				natural = minimum;
			} else {
				foreach (var tab in Tabs) {
					MeasureChild (tab, orientation, -1, out int cmin, out int cnat);
					minimum = Math.Max (minimum, cmin);
					natural = Math.Max (natural, cnat);
				}
			}
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			var tabs = Tabs.ToArray ();
			int[] sizes = CalcTabSizes (tabs, width);
			int x = 0;
			for (int n = 0; n < tabs.Length; n++) {
				AllocateChild (tabs [n], new Gdk.Rectangle (x, 0, sizes [n], height));
				x += sizes [n];
			}
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0 && visualStyle != null && visualStyle.TabStyle == DockTabStyle.Normal) {
				snapshot.DrawCairo (this, ctx => {
					ctx.DrawImage (this, tabbarBackImage.WithSize (width, height), 0, 0);
				});
			}
			base.OnSnapshot (snapshot);
		}

		int[] CalcTabSizes (DockItemTitleTab[] children, int totalWidth)
		{
			int tabsSize = 0;
			int allocWidth = totalWidth;

			if (children.Length == 0)
				return new int [0];

			foreach (DockItemTitleTab tab in children)
				tabsSize += tab.LabelWidth;

			int[] sizes = new int[children.Length];
			double ratio = (double) allocWidth / (double) tabsSize;

			if (ratio > 1 && visualStyle != null && visualStyle.ExpandedTabs.Value) {
				// The tabs have to fill all the available space. To get started, assume that all tabs with have the same size
				var tsize = totalWidth / children.Length;
				// Maybe the assigned size is too small for some tabs. If it happens the extra space it requires has to be taken
				// from tabs which have surplus of space. To calculate it, first get the difference beteen the assigned space
				// and the required space.
				for (int n=0; n<children.Length; n++)
					sizes[n] = tsize - children[n].LabelWidth;

				// If all is positive, nothing is left to do (all tabs have enough space). If there is any negative, it means
				// that space has to be reassigned. The negative space has to be turned into positive by reducing space from other tabs
				for (int n=0; n<sizes.Length; n++) {
					if (sizes[n] < 0) {
						ReduceSizes (sizes, -sizes[n]);
						sizes[n] = 0;
					}
				}
				// Now calculate the final space assignment of each tab
				for (int n=0; n<children.Length; n++) {
					sizes[n] += children[n].LabelWidth;
					totalWidth -= sizes[n];
				}
			} else {
				if (ratio > 1)
					ratio = 1;
				for (int n=0; n<children.Length; n++) {
					var s = (int)((double)children[n].LabelWidth * ratio);
					sizes[n] = s;
					totalWidth -= s;
				}
			}

			// There may be some remaining space due to rounding. Spread it
			for (int n=0; n<children.Length && totalWidth > 0; n++) {
				sizes[n]++;
				totalWidth--;
			}
			return sizes;
		}

		void ReduceSizes (int[] sizes, int amout)
		{
			// Homogeneously removes 'amount' pixels from the array of sizes, making sure
			// no size goes below 0.
			while (amout > 0) {
				int part;
				int candidates = sizes.Count (s => s > 0);
				if (candidates == 0)
					return;
				part = Math.Max (amout / candidates, 1);

				for (int n=0; n<sizes.Length && amout > 0; n++) {
					var s = sizes [n];
					if (s <= 0) continue;
					if (s > part) {
						s -= part;
						amout -= part;
					} else {
						amout -= s;
						s = 0;
					}
					sizes[n] = s;
				}
			}
		}
	}

}
