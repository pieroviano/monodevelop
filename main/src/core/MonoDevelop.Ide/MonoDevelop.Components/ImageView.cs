//
// ImageView.cs
//
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc.
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

namespace MonoDevelop.Components
{
	/// <summary>
	/// Draws an <see cref="Xwt.Drawing.Image"/> (GTK 4: plain Gtk.Widget leaf; GTK 2 Gtk.Misc padding/alignment kept as properties).
	/// </summary>
	[System.ComponentModel.ToolboxItem (true)]
	public class ImageView: Gtk.Widget
	{
		Xwt.Drawing.Image image;
		string iconId;
		StockIconSize? size;
		float xalign = 0.5f, yalign = 0.5f;
		int xpad, ypad;

		public ImageView ()
		{
			AccessibleRole = Gtk.AccessibleRole.Img;
		}

		public ImageView (Xwt.Drawing.Image image): this ()
		{
			this.image = image;
		}

		public ImageView (string iconId, StockIconSize size): this ()
		{
			this.iconId = iconId;
			this.size = size;
			image = MonoDevelop.Ide.ImageService.GetIcon (iconId, size);
		}

		public Xwt.Drawing.Image Image {
			get { return image; }
			set {
				image = value;
				QueueDraw ();
				QueueResize ();
			}
		}

		/// <summary>Horizontal alignment of the image in the allocation (0 left .. 1 right).</summary>
		public float Xalign {
			get { return xalign; }
			set { xalign = value; QueueDraw (); }
		}

		/// <summary>Vertical alignment of the image in the allocation (0 top .. 1 bottom).</summary>
		public float Yalign {
			get { return yalign; }
			set { yalign = value; QueueDraw (); }
		}

		/// <summary>Horizontal padding on each side.</summary>
		public int Xpad {
			get { return xpad; }
			set { xpad = value; QueueResize (); }
		}

		/// <summary>Vertical padding on each side.</summary>
		public int Ypad {
			get { return ypad; }
			set { ypad = value; QueueResize (); }
		}

		public void SetAlignment (float xalign, float yalign)
		{
			this.xalign = xalign;
			this.yalign = yalign;
			QueueDraw ();
		}

		public void SetPadding (int xpad, int ypad)
		{
			this.xpad = xpad;
			this.ypad = ypad;
			QueueResize ();
		}

		public void SetIcon (string iconId, StockIconSize size)
		{
			this.iconId = iconId;
			this.size = size;
			Image = MonoDevelop.Ide.ImageService.GetIcon (iconId, size);
		}

		public StockIconSize IconSize {
			get {
				return size.HasValue ? size.Value : StockIconSize.Invalid;
			}
			set {
				size = value;
				if (iconId != null)
					Image = MonoDevelop.Ide.ImageService.GetIcon (iconId, size.Value);
			}
		}

		public string IconId {
			get {
				return iconId;
			}
			set {
				iconId = value;
				if (size.HasValue)
					Image = MonoDevelop.Ide.ImageService.GetIcon (iconId, size.Value);
			}
		}

		protected override Gtk.SizeRequestMode OnGetRequestMode ()
		{
			return Gtk.SizeRequestMode.ConstantSize;
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			if (orientation == Gtk.Orientation.Horizontal)
				minimum = xpad * 2 + (image != null ? (int)image.Width : 0);
			else
				minimum = ypad * 2 + (image != null ? (int)image.Height : 0);
			natural = minimum;
			minimum_baseline = natural_baseline = -1;
		}

		bool IsParentDisabled ()
		{
			// Insensitivity is inherited in GTK 4: an insensitive ancestor (e.g. a Button) makes this widget insensitive.
			return !IsSensitive;
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (image == null)
				return;
			var alloc = new Gdk.Rectangle (0, 0, Width, Height);
			alloc.Inflate (-xpad, -ypad);
			snapshot.DrawCairo (this, ctx => {
				var x = Math.Round (alloc.X + (alloc.Width - image.Width) * xalign);
				var y = Math.Round (alloc.Y + (alloc.Height - image.Height) * yalign);
				ctx.Save ();
				ctx.DrawImage (this, IsParentDisabled () ? image.WithAlpha (0.4) : image, x, y);
				ctx.Restore ();
			});
		}
	}
}
