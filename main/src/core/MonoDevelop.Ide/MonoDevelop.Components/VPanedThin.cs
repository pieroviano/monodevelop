//
// VPanedThin.cs
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

using System;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Components
{
	/// <summary>Vertical <see cref="Gtk.Paned"/> with a 1px splitter line (see <see cref="HPanedThin"/>).</summary>
	public class VPanedThin: Gtk.Paned
	{
		readonly ThinPanedHandle handle;

		public VPanedThin () : base (Gtk.Orientation.Vertical)
		{
			handle = new ThinPanedHandle (this);
		}

		/// <inheritdoc cref="HPanedThin.GrabAreaSize"/>
		public int GrabAreaSize {
			get { return handle.GrabAreaSize; }
			set {
				handle.GrabAreaSize = value;
				QueueResize ();
			}
		}

		/// <inheritdoc cref="HPanedThin.HandleWidget"/>
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
}
