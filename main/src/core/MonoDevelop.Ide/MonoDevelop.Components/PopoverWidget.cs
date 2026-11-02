//
// PopoverWidget.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//       Lluis Sanchez <lluis@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
using Xwt.Motion;

namespace MonoDevelop.Components
{
	/// <summary>Widget drawing the <see cref="PopoverWindowTheme"/> chrome (border, arrow, pager, shadow) around its content.</summary>
	public class PopoverWidget: ContainerWidget, IAnimatable
	{
		PopoverWindowTheme theme;

		PopupPosition position;
		readonly BinWidget alignment;

		const int MinArrowSpacing = 5;

		public PopoverWidget ()
		{
			alignment = new BinWidget ();
			AddChild (alignment);

			UpdatePadding ();
		}
		
		void IAnimatable.BatchBegin () { }
		void IAnimatable.BatchCommit () { QueueDraw (); }

		public bool EnableAnimation { get; set; }

		public PopoverWindowTheme Theme { 
			get { 
				if (theme == null) {
					theme = new PopoverWindowTheme ();
					theme.RedrawNeeded += OnRedrawNeeded;
				}
				return theme; 
			}
			set {
				if (theme == value)
					return;

				if (theme != null)
					theme.RedrawNeeded -= OnRedrawNeeded;
				theme = value;
				if (theme != null)
					theme.RedrawNeeded += OnRedrawNeeded;

				UpdatePadding ();
				QueueDraw ();
			}
		}

		public bool ShowArrow {
			get { return Theme.ShowArrow; }
			set { Theme.ShowArrow = value; }
		}

		/// <summary>Content container: set its <c>Child</c> (GTK 2: <c>Gtk.Alignment.Add</c>).</summary>
		public BinWidget ContentBox {
			get { return alignment; }
		}

		public PopupPosition PopupPosition {
			get { return position; }
			set {
				Theme.TargetPosition = position = value;
				UpdatePadding ();
			}
		}
		
		/// <summary>GTK 4: size changes are not animated any more; queues a resize.</summary>
		public void AnimatedResize ()
		{
			QueueResize ();
		}

		protected override void OnUnrealized ()
		{
			this.AbortAnimation ("Resize");
			base.OnUnrealized ();
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if ((position & PopupPosition.Top) != 0 || (position & PopupPosition.Bottom) != 0)
				Theme.ArrowOffset = Width / 2;
			else
				Theme.ArrowOffset = Height / 2;

			var border = BorderAllocation;
			snapshot.DrawCairo (this, context => {
				context.Save ();
				Theme.SetBorderPath (context, border, position);
				context.Clip ();
				OnDrawContent (context); // Draw content first so we can easily clip it
				context.Restore ();


				// protect against overriden methods which leave in a bad state
				context.Save ();
				if (Theme.DrawPager) {
					Theme.RenderPager (context, 
					                   PangoContext,
					                   border);
				}

				Theme.RenderShadow (context, border, position);
				context.Restore ();
			});
			base.OnSnapshot (snapshot);
		}

		/// <summary>Draws the background (already clipped to the border path); (0,0) is the widget origin.</summary>
		protected virtual void OnDrawContent (Cairo.Context context)
		{
			Theme.RenderBackground (context, new Gdk.Rectangle (0, 0, Width, Height));
		}

		void UpdatePadding ()
		{
			if (alignment == null)
				return;
			int top,left,bottom,right;
			top = left = bottom = right = Theme.Padding + 1;

			if (ShowArrow) {
				if ((position & PopupPosition.Top) != 0)
					top += Theme.ArrowLength;
				else if ((position & PopupPosition.Bottom) != 0)
					bottom += Theme.ArrowLength;
				else if ((position & PopupPosition.Left) != 0)
					left += Theme.ArrowLength;
				else if ((position & PopupPosition.Right) != 0)
					right += Theme.ArrowLength;
			}
			var p = new Gtk.Border { Top = (short)top, Bottom = (short)bottom, Left = (short)left, Right = (short)right };
			if (!p.Equals (alignment.Padding))
				alignment.Padding = p;
		}

		void OnRedrawNeeded (object sender, EventArgs args)
		{
			UpdatePadding ();
			QueueDraw ();
		}

		protected Gdk.Rectangle ChildAllocation {
			get {
				var rect = BorderAllocation;
				rect.Inflate (-Theme.Padding - 1, -Theme.Padding - 1);
				return rect;
			}
		}

		Gdk.Rectangle BorderAllocation {
			get {
				var rect = new Gdk.Rectangle (0, 0, Width, Height);
				if (ShowArrow) {
					if ((position & PopupPosition.Top) != 0) {
						rect.Y += Theme.ArrowLength;
						rect.Height -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Bottom) != 0) {
						rect.Height -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Left) != 0) {
						rect.X += Theme.ArrowLength;
						rect.Width -= Theme.ArrowLength;
					}
					else if ((position & PopupPosition.Right) != 0) {
						rect.Width -= Theme.ArrowLength;
					}
				}
				return rect;
			}
		}
	}
}
