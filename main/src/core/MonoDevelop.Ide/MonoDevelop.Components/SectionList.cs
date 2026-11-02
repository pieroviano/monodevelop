// 
// SectionList.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2011 Novell, Inc.
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
using Cairo;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components
{
	class SectionList : ContainerWidget
	{
		const int borderLineWidth = 1;
		const int headerPadding = 2;

		List<Section> sections = new List<Section> ();

		Pango.Layout layout;
		int headerHeight = 20;
		int activeIndex = 0;
		int hoverIndex = -1;
		int activeChildHeight;

		public int ActiveIndex {
			get { return activeIndex; }
			set {
				if (value >= sections.Count || value < 0)
					throw new ArgumentOutOfRangeException ();

				if (activeIndex == value)
					return;

				activeIndex = value;
				UpdateVisibility ();
				QueueDraw ();
				OnActiveSectionChanged ();
				sections[activeIndex].OnActivated ();
			}
		}

		public Section ActiveSection {
			get {
				return ActiveIndex >= 0 && sections.Count > 0? sections[activeIndex] : null;
			}
			set {
				int idx = sections.IndexOf (value);
				if (idx < 0)
					throw new ArgumentException ("Section is not in this container");
				ActiveIndex = idx;
			}
		}

		void OnActiveSectionChanged ()
		{
			var evt = ActiveSectionChanged;
			if (evt != null)
				evt (this, EventArgs.Empty);
		}

		public event EventHandler ActiveSectionChanged;

		public SectionList ()
		{
			WidthRequest = 100;
			EnsureLayout ();

			this.AddMotionHandler (
				(x, y) => SetHoverIndex (GetSectionHeaderAtPosition ((int)x, (int)y)),
				(x, y) => SetHoverIndex (GetSectionHeaderAtPosition ((int)x, (int)y)),
				() => SetHoverIndex (-1));
			this.AddClickHandler (click => {
				var idx = GetSectionHeaderAtPosition ((int)click.X, (int)click.Y);
				if (idx >= 0) {
					click.Claim ();
					ActiveIndex = idx;
				}
			});
		}

		public Section AddSection (string title, Gtk.Widget child)
		{
			var s = new Section (this, title, child);
			sections.Add (s);
			addingSection = true;
			try {
				base.InsertChild (child, Children.Count);
			} finally {
				addingSection = false;
			}
			UpdateVisibility ();
			return s;
		}

		bool addingSection;

		/// <summary>Children can only be added through <see cref="AddSection"/>.</summary>
		public override void InsertChild (Gtk.Widget child, int index)
		{
			if (!addingSection)
				throw new InvalidOperationException ("Cannot add widget directly, must add a section");
			base.InsertChild (child, index);
		}

		protected override void OnChildRemoved (Gtk.Widget widget)
		{
			for (int i = 0; i < sections.Count; i++) {
				var section = sections[i];
				if (section.Child == widget) {
					section.Parent = null;
					section.Child = null;
					sections.RemoveAt (i);
					break;
				}
			}
			if (activeIndex >= sections.Count)
				activeIndex = Math.Max (0, sections.Count - 1);
			UpdateVisibility ();
			base.OnChildRemoved (widget);
		}

		protected override void OnCssChanged (Gtk.CssStyleChange change)
		{
			base.OnCssChanged (change);
			KillLayout ();
			EnsureLayout ();
		}

		void EnsureLayout ()
		{
			if (layout != null)
				layout.Dispose ();
			layout = CreatePangoLayout (null);

			layout.SetText ("lp");
			int w, h;
			layout.GetPixelSize (out w, out h);
			headerHeight = Math.Max (20, h + headerPadding + headerPadding);
		}

		void KillLayout ()
		{
			if (layout == null)
				return;
			layout.Dispose ();
			layout = null;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				KillLayout ();
			base.Dispose (disposing);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			int min = 0, nat = 0;
			foreach (var section in sections) {
				MeasureChild (section.Child, orientation, -1, out int cmin, out int cnat);
				min = Math.Max (min, cmin);
				nat = Math.Max (nat, cnat);
			}

			int extra = borderLineWidth * 2;
			if (orientation == Gtk.Orientation.Vertical)
				extra += sections.Count * headerHeight + borderLineWidth;

			minimum = min + extra;
			natural = nat + extra;
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			if (sections.Count == 0)
				return;

			int bw = borderLineWidth;
			var allocation = new Gdk.Rectangle (bw, bw, width - bw * 2, height - bw * 2);
			allocation.Height -= (borderLineWidth + headerHeight) * sections.Count;
			allocation.Y += (headerHeight + borderLineWidth) * (activeIndex + 1);
			allocation.Width = Math.Max (0, allocation.Width);
			allocation.Height = Math.Max (0, allocation.Height);
			activeChildHeight = allocation.Height;

			AllocateChild (sections[activeIndex].Child, allocation);
		}

		void UpdateVisibility ()
		{
			for (int i = 0; i < sections.Count; i++)
				sections[i].Child.Visible = activeIndex == i;
			QueueResize ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (sections.Count == 0)
				return;
			snapshot.DrawCairo (this, DrawHeaders);
			base.OnSnapshot (snapshot);
		}

		void DrawHeaders (Cairo.Context cr)
		{
			double halfLineWidth = borderLineWidth / 2.0;
			int w = Width;
			int h = Height;

			var borderCol = Styles.BorderColor.ToCairoColor ();
			cr.SetSourceColor (borderCol);
			cr.Rectangle (halfLineWidth, halfLineWidth, w - borderLineWidth, h - borderLineWidth);
			cr.LineWidth = borderLineWidth;
			cr.Stroke ();

			cr.Translate (borderLineWidth, borderLineWidth);
			w = w - (2 * borderLineWidth);

			using (LinearGradient unselectedGrad = new LinearGradient (0, 0, 0, headerHeight),
			       hoverGrad = new LinearGradient (0, 0, 0, headerHeight),
			       selectedGrad = new LinearGradient (0, 0, 0, headerHeight)
			       )
			{
				var unselectedCol = Styles.BackgroundColor.ToCairoColor ();
				var unselectedTextCol = Styles.BaseForegroundColor.ToCairoColor ();
				unselectedCol.A = 0.6;
				unselectedGrad.AddColorStop (0, unselectedCol);
				unselectedCol.A = 1;
				unselectedGrad.AddColorStop (1, unselectedCol);

				var hoverCol = Styles.BackgroundColor.AddLight (0.05).ToCairoColor ();
				var hoverTextCol = Styles.BaseForegroundColor.ToCairoColor ();
				hoverCol.A = 0.6;
				hoverGrad.AddColorStop (0, hoverCol);
				hoverCol.A = 1;
				hoverGrad.AddColorStop (1, hoverCol);

				var selectedCol = Styles.BackgroundColor.ToCairoColor ();
				var selectedTextCol = Styles.BaseForegroundColor.ToCairoColor ();
				selectedCol.A = 0.6;
				selectedGrad.AddColorStop (0, selectedCol);
				selectedCol.A = 1;
				selectedGrad.AddColorStop (1, selectedCol);

				for (int i = 0; i < sections.Count; i++) {
					var section = sections[i];
					bool isActive = activeIndex == i;
					bool isHover = hoverIndex == i;

					cr.Rectangle (0, 0, w, headerHeight);
					cr.SetSource (isActive? selectedGrad : (isHover? hoverGrad : unselectedGrad));
					cr.Fill ();

					cr.SetSourceColor (isActive? selectedTextCol : (isHover? hoverTextCol : unselectedTextCol));
					layout.SetText (section.Title);
					layout.Ellipsize = Pango.EllipsizeMode.End;
					layout.Width = (int) ((w - headerPadding - headerPadding) * Pango.Scale.PangoScale);
					cr.MoveTo (headerPadding, headerPadding);
					Pango.CairoHelper.ShowLayout (cr, layout);

					cr.MoveTo (-halfLineWidth, i > activeIndex? -halfLineWidth : headerHeight + halfLineWidth);
					cr.RelLineTo (w + borderLineWidth, 0.0);
					cr.SetSourceColor (borderCol);
					cr.Stroke ();

					cr.Translate (0, headerHeight + borderLineWidth);
					if (isActive)
						cr.Translate (0, activeChildHeight + borderLineWidth);
				}
			}
		}

		int GetSectionHeaderAtPosition (int x, int y)
		{
			if (sections.Count == 0)
				return -1;

			x -= borderLineWidth;
			y -= borderLineWidth;
			if (y < 0 || x < 0 || x > Width - borderLineWidth * 2)
				return -1;

			int childWidgetStart = (headerHeight + borderLineWidth) * (activeIndex + 1);
			if (y < childWidgetStart)
				return y / (headerHeight + borderLineWidth);

			y -= activeChildHeight;
			if (y < childWidgetStart)
				return -1;

			int idx = y / (headerHeight + borderLineWidth);
			return idx < sections.Count? idx : -1;
		}

		void SetHoverIndex (int index)
		{
			if (hoverIndex == index)
				return;
			hoverIndex = index;
			QueueDraw ();
		}

		public class Section
		{
			string title;
			public SectionList Parent { get; internal set; }
			public Gtk.Widget Child { get; internal set; }

			internal Section (SectionList parent, string title, Gtk.Widget child)
			{
				this.Parent = parent;
				this.title = title;
				this.Child = child;
			}

			public bool IsActive {
				get { return Parent.ActiveSection == this; }
				set { Parent.ActiveSection = this; }
			}

			public string Title {
				get { return this.title; }
				set {
					this.title = value;
					Parent?.QueueDraw ();
				}
			}

			internal void OnActivated ()
			{
				var evt = Activated;
				if (evt != null)
					evt (this, EventArgs.Empty);
			}

			public event EventHandler Activated;
		}
	}
}
