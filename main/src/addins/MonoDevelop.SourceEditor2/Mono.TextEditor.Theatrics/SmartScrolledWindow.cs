using System;
using System.Linq;
using Gtk;
using Gdk;
using System.Collections.Generic;
using MonoDevelop.Components;

namespace Mono.TextEditor.Theatrics
{
	/// <summary>
	/// Implemented by scrollable children of the <see cref="SmartScrolledWindow"/>: the child scrolls its content
	/// virtually and keeps the bounds of the adjustments up to date (replaces GTK 2 <c>SetScrollAdjustments</c>).
	/// </summary>
	interface IScrollAdjustable
	{
		void SetScrollAdjustments (Gtk.Adjustment hAdjustment, Gtk.Adjustment vAdjustment);
	}

	/// <summary>
	/// A scrolled window with the ability to put widgets beside the scrollbars.
	/// </summary>
	class SmartScrolledWindow : ContainerWidget
	{
		Gtk.Adjustment vAdjustment;
		Gtk.Widget vScrollBar;
		
		Gtk.Adjustment hAdjustment;
		Gtk.Scrollbar hScrollBar;

		Gtk.Widget child;

		List<SmartScrolledWindowContainerChild> children = new List<SmartScrolledWindowContainerChild> ();
		public enum ChildPosition {
			Top,
			Bottom,
			Left,
			Right
		}
		
		public Gtk.Adjustment Vadjustment {
			get { return this.vAdjustment; }
		}

		public Gtk.Adjustment Hadjustment {
			get { return this.hAdjustment; }
		}

		public bool BorderVisible { get; set; }

		/// <summary>The vertical scroll bar (or the widget that replaced it).</summary>
		public Gtk.Widget VScrollbar {
			get { return vScrollBar; }
		}

		bool suppressVScrollbar;

		/// <summary>
		/// Keeps the vertical scroll bar hidden (and scrolling by wheel and adjustment working). Used when another
		/// widget (the quick task strip) replaces the scroll bar visually.
		/// </summary>
		public bool SuppressVScrollbar {
			get { return suppressVScrollbar; }
			set {
				if (suppressVScrollbar == value)
					return;
				suppressVScrollbar = value;
				HandleAdjustmentChanged (vAdjustment, EventArgs.Empty);
			}
		}

		/// <summary>The horizontal scroll bar.</summary>
		public Gtk.Scrollbar HScrollbar {
			get { return hScrollBar; }
		}

		/// <summary>The scrolled widget. If it implements <see cref="IScrollAdjustable"/> it receives the adjustments.</summary>
		public Gtk.Widget Child {
			get { return child; }
			set {
				if (child == value)
					return;
				if (child != null) {
					RemoveChild (child);
					child = null;
				}
				if (value != null) {
					child = value;
					InsertChild (value, 0);
					(value as IScrollAdjustable)?.SetScrollAdjustments (hAdjustment, vAdjustment);
				}
			}
		}

		/// <summary>Removes the scrolled child without destroying it.</summary>
		public void RemoveScrolledChild ()
		{
			Child = null;
		}
		
		internal class SmartScrolledWindowContainerChild
		{
			public Gtk.Widget Child { get; }

			public ChildPosition ChildPosition {
				get;
				set;
			}
			
			public SmartScrolledWindowContainerChild (Gtk.Widget child)
			{
				Child = child;
			}
		}
		
		public SmartScrolledWindow (Gtk.Widget vScrollBar = null)
		{
			Overflow = Overflow.Hidden;
			Hexpand = true;
			Vexpand = true;

			vAdjustment = new Gtk.Adjustment (0, 0, 0, 0, 0, 0);
			vAdjustment.Changed += HandleAdjustmentChanged;
			
			this.vScrollBar = vScrollBar ?? new Gtk.Scrollbar (Orientation.Vertical, vAdjustment);
			AddChild (this.vScrollBar);
			
			hAdjustment = new Gtk.Adjustment (0, 0, 0, 0, 0, 0);
			hAdjustment.Changed += HandleAdjustmentChanged;
			
			hScrollBar = new Gtk.Scrollbar (Orientation.Horizontal, hAdjustment);
			AddChild (hScrollBar);
		}
		
		public void ReplaceVScrollBar (Gtk.Widget widget)
		{
			if (vScrollBar != null) {
				RemoveChild (vScrollBar);
			}
			this.vScrollBar = widget;
			AddChild (widget);
		}

		bool destroyed;

		public override void Destroy ()
		{
			if (!destroyed) {
				destroyed = true;
				if (vAdjustment != null)
					vAdjustment.Changed -= HandleAdjustmentChanged;
				if (hAdjustment != null)
					hAdjustment.Changed -= HandleAdjustmentChanged;
			}
			base.Destroy ();
		}
		
		void HandleAdjustmentChanged (object sender, EventArgs e)
		{
			var adjustment = (Gtk.Adjustment)sender;
			var scrollbar = adjustment == vAdjustment ? vScrollBar : hScrollBar;
			if (!(scrollbar is Gtk.Scrollbar))
				return;
			bool newVisible = adjustment.Upper - adjustment.Lower > adjustment.PageSize;
			if (adjustment == vAdjustment && suppressVScrollbar)
				newVisible = false;
			if (scrollbar.Visible != newVisible) {
				scrollbar.Visible = newVisible;
				QueueResize ();
			}
		}
		
		public void AddChild (Gtk.Widget child, ChildPosition position)
		{
			base.AddChild (child);
			children.Add (new SmartScrolledWindowContainerChild (child) { ChildPosition = position });
		}

		public override void RemoveChild (Gtk.Widget widget)
		{
			foreach (var info in children.ToArray ()) {
				if (info.Child == widget) {
					children.Remove (info);
					break;
				}
			}
			if (widget == vScrollBar)
				vScrollBar = null;
			else if (widget == hScrollBar)
				hScrollBar = null;
			base.RemoveChild (widget);
		}

		static int NaturalWidth (Gtk.Widget widget)
		{
			return widget != null && widget.Visible ? GetChildNaturalSize (widget).Width : 0;
		}

		static int NaturalHeight (Gtk.Widget widget)
		{
			return widget != null && widget.Visible ? GetChildNaturalSize (widget).Height : 0;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			int margin = BorderVisible ? 1 : 0;
			int vwidth = vScrollBar.Visible ? NaturalWidth (vScrollBar) : 0;
			int hheight = hScrollBar.Visible ? NaturalHeight (hScrollBar) : 0; 
			var childRectangle = new Gdk.Rectangle (margin, margin, width - vwidth - margin*2, height - hheight - margin*2);

			if (Child != null) 
				AllocateChild (Child, childRectangle);
			if (vScrollBar.Visible) {
				int vChildTopHeight = -1;
				foreach (var child in children.Where (child => child.ChildPosition == ChildPosition.Top)) {
					var childHeight = NaturalHeight (child.Child);
					AllocateChild (child.Child, new Gdk.Rectangle (childRectangle.RightInside (), childRectangle.Y + vChildTopHeight, width - vwidth, childHeight));
					vChildTopHeight += childHeight;
				}
				int v = vScrollBar is Gtk.Scrollbar && hScrollBar.Visible ? hheight : 0;
				AllocateChild (vScrollBar, new Gdk.Rectangle (childRectangle.X + childRectangle.Width + margin, childRectangle.Y + vChildTopHeight, vwidth, height - v - vChildTopHeight - margin));
				vAdjustment.Value = System.Math.Max (System.Math.Min (vAdjustment.Upper - vAdjustment.PageSize, vAdjustment.Value), vAdjustment.Lower);
			}
			
			if (hScrollBar.Visible) {
				int v = vScrollBar.Visible ? vwidth : 0;
				AllocateChild (hScrollBar, new Gdk.Rectangle (0, childRectangle.Y + childRectangle.Height + margin, width - v, hheight));
				hAdjustment.Value = System.Math.Max (System.Math.Min (hAdjustment.Upper - hAdjustment.PageSize, hAdjustment.Value), hAdjustment.Lower);
			}
		}
		
		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			// the scrolled child can be any size: only the scroll bars have a minimum
			minimum = natural = 0;
			minimum_baseline = natural_baseline = -1;
			if (orientation == Gtk.Orientation.Horizontal) {
				minimum = natural = NaturalWidth (vScrollBar);
			} else {
				minimum = natural = NaturalHeight (hScrollBar);
			}
		}
		
		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			base.OnSnapshot (snapshot);
			if (BorderVisible) {
				snapshot.DrawCairo (this, cr => {
					cr.LineWidth = 1;
					
					int right = Width - 1;
					int bottom = Height - 1;
					
					cr.SharpLineX (0, 0, 0, bottom);
					cr.SharpLineX (right, 0, right, bottom);
					
					cr.SharpLineY (0, 0, right, 0);
					cr.SharpLineY (0, bottom, right, bottom);
					
					cr.SetSourceColor (MonoDevelop.Ide.Gui.Styles.WidgetBorderColor.ToCairoColor ());
					cr.Stroke ();
				});
			}
		}
		
	}
}
