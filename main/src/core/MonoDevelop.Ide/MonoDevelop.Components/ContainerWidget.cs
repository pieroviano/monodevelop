//
// ContainerWidget.cs
//
// Copyright (c) Microsoft Corp.
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

namespace MonoDevelop.Components
{
	/// <summary>
	/// GTK 4 base for custom containers (replaces GTK 2 Container/Bin subclasses): keeps a child list,
	/// parents/unparents children, and by default stacks every child over the full allocation.
	/// Override <see cref="OnMeasure"/>/<see cref="OnSizeAllocate"/> for a custom layout and use
	/// <see cref="AllocateChild"/>/<see cref="MeasureChild"/> there.
	/// </summary>
	public abstract class ContainerWidget : Gtk.Widget
	{
		readonly List<Gtk.Widget> children = new List<Gtk.Widget> ();

		protected ContainerWidget ()
		{
		}

		protected ContainerWidget (IntPtr raw) : base (raw)
		{
		}

		/// <summary>Children in insertion (and drawing) order.</summary>
		public IReadOnlyList<Gtk.Widget> Children => children;

		/// <summary>Appends <paramref name="child"/>; no-op if it is already a child.</summary>
		public virtual void AddChild (Gtk.Widget child)
		{
			InsertChild (child, children.Count);
		}

		/// <summary>Inserts <paramref name="child"/> at <paramref name="index"/> (drawing order follows the list).</summary>
		public virtual void InsertChild (Gtk.Widget child, int index)
		{
			if (child == null)
				throw new ArgumentNullException (nameof (child));
			if (children.Contains (child))
				return;
			index = Math.Max (0, Math.Min (index, children.Count));
			children.Insert (index, child);
			var next = index + 1 < children.Count ? children [index + 1] : null;
			if (next != null)
				child.InsertBefore (this, next);
			else
				child.Parent = this;
			OnChildAdded (child);
			QueueResize ();
		}

		/// <summary>Removes and unparents <paramref name="child"/>; no-op if it is not a child.</summary>
		public virtual void RemoveChild (Gtk.Widget child)
		{
			if (child == null || !children.Remove (child))
				return;
			child.Unparent ();
			OnChildRemoved (child);
			QueueResize ();
		}

		/// <summary>Removes every child.</summary>
		public void ClearChildren ()
		{
			foreach (var c in children.ToArray ())
				RemoveChild (c);
		}

		protected virtual void OnChildAdded (Gtk.Widget child)
		{
		}

		protected virtual void OnChildRemoved (Gtk.Widget child)
		{
		}

		/// <summary>Measures <paramref name="child"/> in one orientation (returns 0 for hidden children).</summary>
		protected static void MeasureChild (Gtk.Widget child, Gtk.Orientation orientation, int forSize, out int minimum, out int natural)
		{
			if (child == null || !child.Visible) {
				minimum = natural = 0;
				return;
			}
			child.Measure (orientation, forSize, out minimum, out natural, out _, out _);
		}

		/// <summary>Natural (width, height) of <paramref name="child"/>.</summary>
		protected static Gdk.Size GetChildNaturalSize (Gtk.Widget child)
		{
			MeasureChild (child, Gtk.Orientation.Horizontal, -1, out _, out int w);
			MeasureChild (child, Gtk.Orientation.Vertical, w, out _, out int h);
			return new Gdk.Size (w, h);
		}

		/// <summary>Allocates <paramref name="child"/> at <paramref name="rect"/> in this widget's coordinates.</summary>
		protected static void AllocateChild (Gtk.Widget child, Gdk.Rectangle rect)
		{
			if (child == null || !child.Visible)
				return;
			child.SizeAllocate (rect, -1);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = 0;
			foreach (var c in children) {
				MeasureChild (c, orientation, for_size, out int cmin, out int cnat);
				minimum = Math.Max (minimum, cmin);
				natural = Math.Max (natural, cnat);
			}
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			var r = new Gdk.Rectangle (0, 0, width, height);
			foreach (var c in children)
				AllocateChild (c, r);
		}

		protected override void Dispose (bool disposing)
		{
			// GTK 4 warns when a widget is finalized with children still parented.
			if (disposing && Handle != IntPtr.Zero) {
				foreach (var c in children)
					c.Unparent ();
				children.Clear ();
			}
			base.Dispose (disposing);
		}
	}

	/// <summary>
	/// <see cref="ContainerWidget"/> holding at most one child, allocated over the full area minus <see cref="Padding"/>
	/// (replaces GTK 2 <c>Gtk.Bin</c>/<c>Gtk.Alignment</c> subclasses).
	/// </summary>
	public class BinWidget : ContainerWidget
	{
		Gtk.Border padding;

		public BinWidget ()
		{
		}

		protected BinWidget (IntPtr raw) : base (raw)
		{
		}

		/// <summary>The single child; setting replaces (and unparents) the previous one.</summary>
		public Gtk.Widget Child {
			get => Children.Count > 0 ? Children [0] : null;
			set {
				var old = Child;
				if (old == value)
					return;
				if (old != null)
					RemoveChild (old);
				if (value != null)
					AddChild (value);
			}
		}

		/// <summary>Space reserved around the child (replaces BorderWidth / Alignment padding).</summary>
		public Gtk.Border Padding {
			get => padding;
			set { padding = value; QueueResize (); }
		}

		public override void AddChild (Gtk.Widget child)
		{
			if (Child != null && Child != child)
				RemoveChild (Child);
			base.AddChild (child);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			int extra = orientation == Gtk.Orientation.Horizontal ? padding.Left + padding.Right : padding.Top + padding.Bottom;
			int otherExtra = orientation == Gtk.Orientation.Horizontal ? padding.Top + padding.Bottom : padding.Left + padding.Right;
			MeasureChild (Child, orientation, for_size < 0 ? -1 : Math.Max (0, for_size - otherExtra), out minimum, out natural);
			minimum += extra;
			natural += extra;
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			AllocateChild (Child, new Gdk.Rectangle (padding.Left, padding.Top,
				Math.Max (0, width - padding.Left - padding.Right),
				Math.Max (0, height - padding.Top - padding.Bottom)));
		}
	}
}
