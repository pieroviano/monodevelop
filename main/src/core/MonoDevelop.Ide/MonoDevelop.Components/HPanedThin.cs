//
// HPanedThin.cs
//
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc
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

using MonoDevelop.Ide.Gui;
using System.Collections.Generic;
using System;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Horizontal <see cref="Gtk.Paned"/> with a 1px splitter line (GTK 4: non-wide handle + CSS). GTK 4 already
	/// extends the grab area of a thin handle beyond the drawn line.
	/// </summary>
	public class HPanedThin: Gtk.Paned
	{
		readonly ThinPanedHandle handle;

		public HPanedThin () : base (Gtk.Orientation.Horizontal)
		{
			handle = new ThinPanedHandle (this);
		}

		/// <summary>
		/// Size of the area grabbing the pointer. GTK 4 sizes the grab area of a thin handle itself, so this is
		/// only used as the separator size while <see cref="HandleWidget"/> is set and reports no natural size.
		/// </summary>
		public int GrabAreaSize {
			get { return handle.GrabAreaSize; }
			set {
				handle.GrabAreaSize = value;
				QueueResize ();
			}
		}

		/// <summary>Widget drawn over (and sizing) the splitter; dragging it moves the splitter.</summary>
		public Gtk.Widget HandleWidget {
			get { return handle.HandleWidget; }
			set { handle.HandleWidget = value; }
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);
			handle.Allocate (width, height);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot);
			handle.Snapshot (snapshot);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				handle.Dispose ();
			base.Dispose (disposing);
		}
	}

	/// <summary>
	/// Shared implementation of <see cref="HPanedThin"/>/<see cref="VPanedThin"/>: styles the separator through a
	/// per-size CSS class and lays an optional handle widget over it.
	/// </summary>
	sealed class ThinPanedHandle : IDisposable
	{
		internal const int HandleGrabWidth = 4;
		const string CssClassPrefix = "md-thin-paned-";

		static readonly HashSet<int> stylesParsed = new HashSet<int> ();
		static Gtk.CssProvider provider;

		readonly Gtk.Paned paned;
		Gtk.Widget handleWidget;
		string cssClass;
		int grabAreaSize = HandleGrabWidth;

		public ThinPanedHandle (Gtk.Paned paned)
		{
			this.paned = paned;
			paned.WideHandle = false;
			paned.AddCssClass ("md-thin-paned");
			SetSeparatorSize (1);
		}

		bool Horizontal => paned.Orientation == Gtk.Orientation.Horizontal;

		public int GrabAreaSize {
			get { return grabAreaSize; }
			set {
				grabAreaSize = value;
				if (handleWidget != null)
					UpdateSeparatorSize ();
			}
		}

		public Gtk.Widget HandleWidget {
			get { return handleWidget; }
			set {
				if (handleWidget == value)
					return;
				if (handleWidget != null)
					handleWidget.Unparent ();
				handleWidget = value;
				if (handleWidget != null)
					handleWidget.Parent = paned;
				UpdateSeparatorSize ();
				paned.QueueResize ();
			}
		}

		void UpdateSeparatorSize ()
		{
			if (handleWidget == null) {
				SetSeparatorSize (1);
				return;
			}
			handleWidget.Measure (Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical, -1, out _, out int natural, out _, out _);
			SetSeparatorSize (natural > 0 ? natural : grabAreaSize);
		}

		void SetSeparatorSize (int size)
		{
			string newClass = CssClassPrefix + size;
			if (cssClass == newClass)
				return;
			EnsureStyle (size);
			if (cssClass != null)
				paned.RemoveCssClass (cssClass);
			cssClass = newClass;
			paned.AddCssClass (cssClass);
		}

		// One display-wide provider holding a rule per separator size (replaces GTK 2 Rc "GtkPaned::handle-size").
		static void EnsureStyle (int size)
		{
			if (!stylesParsed.Add (size) && provider != null)
				return;

			var display = Gdk.Display.Default;
			if (display == null)
				return;

			var sb = new System.Text.StringBuilder ();
			string color = Styles.ThinSplitterColor.ToGdkColor ().ToString ();
			foreach (var s in stylesParsed) {
				// 1px line in the middle of a separator 's' pixels wide; a handle widget, if any, draws over it.
				string line = s <= 1 ? "" : "background-size: 1px 100%; background-position: center; background-repeat: no-repeat;";
				string lineV = s <= 1 ? "" : "background-size: 100% 1px; background-position: center; background-repeat: no-repeat;";
				sb.Append ($"paned.horizontal.{CssClassPrefix}{s} > separator {{ min-width: {s}px; background: none; background-color: transparent; background-image: image({color}); {line} border: none; box-shadow: none; }}\n");
				sb.Append ($"paned.vertical.{CssClassPrefix}{s} > separator {{ min-height: {s}px; background: none; background-color: transparent; background-image: image({color}); {lineV} border: none; box-shadow: none; }}\n");
			}

			if (provider == null) {
				provider = new Gtk.CssProvider ();
				Gtk.StyleContext.AddProviderForDisplay (display, provider, Gtk.StyleProviderPriority.Application);
			}
			provider.LoadFromString (sb.ToString ());
		}

		// Called after Gtk.Paned laid out its children: the handle widget covers the separator.
		public void Allocate (int width, int height)
		{
			if (handleWidget == null)
				return;

			var start = paned.StartChild;
			var end = paned.EndChild;
			bool show = start != null && start.Visible && end != null && end.Visible;
			handleWidget.Visible = show;
			if (!show)
				return;

			handleWidget.Measure (Horizontal ? Gtk.Orientation.Horizontal : Gtk.Orientation.Vertical, -1, out _, out int natural, out _, out _);
			if (natural <= 0)
				natural = grabAreaSize;
			// keep the separator as big as the handle (a CSS class change queues a new layout)
			SetSeparatorSize (natural);

			int pos = Horizontal ? start.Allocation.X + start.Allocation.Width : start.Allocation.Y + start.Allocation.Height;
			var rect = Horizontal
				? new Gdk.Rectangle (pos, 0, natural, height)
				: new Gdk.Rectangle (0, pos, width, natural);
			handleWidget.Measure (Gtk.Orientation.Horizontal, -1, out _, out _, out _, out _);
			handleWidget.Measure (Gtk.Orientation.Vertical, rect.Width, out _, out _, out _, out _);
			handleWidget.SizeAllocate (rect, -1);
		}

		public void Snapshot (Gtk.Snapshot snapshot)
		{
			if (handleWidget != null && handleWidget.Visible)
				paned.SnapshotChild (handleWidget, snapshot);
		}

		public void Dispose ()
		{
			if (handleWidget != null) {
				handleWidget.Unparent ();
				handleWidget = null;
			}
		}
	}
}
