//
// GtkShellDocumentViewContainerSplit.cs
//
// Author:
//       Lluis Sanchez <llsan@microsoft.com>
//
// Copyright (c) 2019 Microsoft
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
using System.Collections.Generic;
using Gtk;
using MonoDevelop.Ide.Gui.Documents;
using Xwt.Drawing;
using System.Linq;
using MonoDevelop.Core;
using System.ComponentModel;

namespace MonoDevelop.Ide.Gui.Shell
{
	class GtkShellDocumentViewContainerSplit : IGtkShellDocumentViewContainer
	{
		/// <summary>Paned reporting its size changes (replaces the GTK 2 SizeAllocated event).</summary>
		class SplitPaned : Paned
		{
			public SplitPaned (Orientation orientation) : base (orientation)
			{
			}

			public event Action<int, int> SizeAllocatedHandler;

			protected override void OnSizeAllocate (int width, int height, int baseline)
			{
				base.OnSizeAllocate (width, height, baseline);
				SizeAllocatedHandler?.Invoke (width, height);
			}
		}

		DocumentViewContainerMode mode;
		SplitPaned paned;
		List<double> relativeSplitSizes = new List<double> ();
		bool settingSize, sizesLoaded;
		Gdk.Rectangle lastRecalcSize;

		public GtkShellDocumentViewContainerSplit (DocumentViewContainerMode mode)
		{
			this.mode = mode;
			paned = CreatePaned ();
			paned.SizeAllocatedHandler += Paned_SizeAllocated;
		}

		public Gtk.Widget Widget => paned;


		SplitPaned CreatePaned ()
		{
			// vertical split = side by side views (horizontal paned), horizontal split = stacked views
			var orientation = mode == DocumentViewContainerMode.VerticalSplit ? Orientation.Horizontal : Orientation.Vertical;
			var p = new SplitPaned (orientation);
			p.AddNotification ("position",HandleNotifyHandler);
			return p;
		}

		void HandleNotifyHandler (object o, GLib.NotifyArgs args)
		{
			if (!settingSize && sizesLoaded)
				relativeSplitSizes = GetRelativeSplitSizes ().ToList ();
		}

		public List<Widget> Children { get; } = new List<Widget> ();

		public Paned Paned {
			get {
				return paned;
			}
		}

		public GtkShellDocumentViewItem ActiveView { get; set; }

		public event EventHandler ActiveViewChanged;

		void Rebuild ()
		{
			settingSize = true;
			ClearPaneds ();
			// Views are chained: each paned holds a view in its start child and the next paned (or the last view) in its end child
			var currentPaned = paned;
			for (int n = 0; n < Children.Count; n++) {
				var c = Children [n];
				if (n == Children.Count - 1) {
					if (n == 0)
						currentPaned.StartChild = c;
					else
						currentPaned.EndChild = c;
				} else {
					currentPaned.StartChild = c;
					if (n < Children.Count - 2) {
						var childPaned = CreatePaned ();
						currentPaned.EndChild = childPaned;
						currentPaned = childPaned;
					}
				}
			}
			RestoreSizes ();
			settingSize = false;
		}

		void ClearPaneds ()
		{
			Paned p = paned;
			while (p != null) {
				var next = p.EndChild as Paned;
				p.StartChild = null;
				p.EndChild = null;
				p = next;
			}
		}

		public void Dispose ()
		{
			paned.SizeAllocatedHandler -= Paned_SizeAllocated;
		}

		public void AddViews (IEnumerable<GtkShellDocumentViewItem> views)
		{
			Children.Clear ();
			Children.AddRange (views);
			relativeSplitSizes.Clear ();
			for (int n = 0; n < Children.Count; n++)
				relativeSplitSizes.Add (1 / (double)Children.Count);
			Rebuild ();
		}

		public void InsertView (int position, GtkShellDocumentViewItem view)
		{
			if (Children.Count > 0) {
				var part = 1 / ((double)Children.Count + 1);
				for (int i = 0; i < relativeSplitSizes.Count; i++)
					relativeSplitSizes [i] *= 1 - part;
				relativeSplitSizes.Insert (position, part);
			} else
				relativeSplitSizes.Add (1);
			Children.Insert (position, view);
			Rebuild ();
		}

		public void RemoveAllViews ()
		{
			relativeSplitSizes.Clear ();
			Children.Clear ();
			Rebuild ();
		}

		public void RemoveView (int tabPos)
		{
			Children.RemoveAt (tabPos);
			var part = relativeSplitSizes [tabPos];
			relativeSplitSizes.RemoveAt (tabPos);
			for (int i = 0; i < relativeSplitSizes.Count; i++)
				relativeSplitSizes [i] /= 1 - part;
			Rebuild ();
		}

		public void ReorderView (int currentIndex, int newIndex)
		{
			var c = Children [currentIndex];
			Children.RemoveAt (currentIndex);
			Children.Insert (newIndex, c);

			var s = relativeSplitSizes [currentIndex];
			relativeSplitSizes.RemoveAt (currentIndex);
			relativeSplitSizes.Insert (newIndex, s);

			Rebuild ();
		}

		public void ReplaceView (int position, GtkShellDocumentViewItem view)
		{
			Children [position] = view;
			Rebuild ();
		}

		public void SelectView (GtkShellDocumentViewItem view)
		{
		}

		public void SetCurrentMode (DocumentViewContainerMode currentMode)
		{
		}

		public IEnumerable<GtkShellDocumentViewItem> GetAllViews ()
		{
			return Children.Cast<GtkShellDocumentViewItem> ();
		}

		public GtkShellDocumentViewItem GetChild (int index)
		{
			return (GtkShellDocumentViewItem) Children [index];
		}

		public void SetViewTitle (GtkShellDocumentViewItem view, string label, Xwt.Drawing.Image icon, string accessibilityDescription)
		{
		}

		void Paned_SizeAllocated (int width, int height)
		{
			var allocation = new Gdk.Rectangle (0, 0, width, height);
			if (!lastRecalcSize.Equals (allocation)) {
				lastRecalcSize = allocation;
				RestoreSizes ();
				sizesLoaded = true;
			}
			settingSize = false;
		}

		void RestoreSizes ()
		{
			if (Children.Count > 1) {
				settingSize = true;
				var size = mode == DocumentViewContainerMode.VerticalSplit ? paned.Width : paned.Height;
				var totalWidth = size - (Children.Count - 1) * HandleSize;
				Paned p = paned;
				int index = 0;
				while (p != null && index < relativeSplitSizes.Count) {
					p.Position = (int)(totalWidth * relativeSplitSizes [index++]);
					p = p.EndChild as Paned;
				}
				settingSize = false;
			}
		}

		int HandleSize => 4;

		public void SetRelativeSplitSizes (double [] sizes)
		{
			relativeSplitSizes = sizes.ToList ();
		}

		public double [] GetRelativeSplitSizes ()
		{
			var size = mode == DocumentViewContainerMode.VerticalSplit ? paned.Width : paned.Height;
			var totalWidth = size - (Children.Count - 1) * HandleSize;

			var sizes = new double[Children.Count];
			for (int n = 0; n < Children.Count; n++) {
				var child = Children [n];
				var childSize = mode == DocumentViewContainerMode.VerticalSplit ? (double)child.Width : (double)child.Height;
				sizes [n] = childSize / (double)totalWidth;
			}
			return sizes;
		}
	}
}
