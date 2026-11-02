//
// DockFrameTopLevel.cs
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



using System;

namespace MonoDevelop.Components.Docking
{
	/// <summary>
	/// A pad laid out on top of the dock frame at a fixed position (auto-hide pads). GTK 4 can't position windows,
	/// so top levels are always children of the <see cref="DockFrame"/>.
	/// </summary>
	class DockFrameTopLevel: BinWidget
	{
		int x, y;
		int width, height;

		public DockFrameTopLevel (DockFrame frame)
		{
		}
		
		/// <summary>X position inside the frame.</summary>
		public int X {
			get { return x; }
			set {
				x = value;
				Parent?.QueueAllocate ();
			}
		}
		
		/// <summary>Y position inside the frame.</summary>
		public int Y {
			get { return y; }
			set {
				y = value;
				Parent?.QueueAllocate ();
			}
		}

		/// <summary>Requested size.</summary>
		public Gdk.Size Size {
			get {
				return new Gdk.Size (width, height);
			}
			set {
				width = value.Width;
				height = value.Height;
				SetSizeRequest (width, height);
				Parent?.QueueResize ();
			}
		}

		/// <summary>Requested width.</summary>
		public int RequestedWidth {
			get { return width; }
			set { Size = new Gdk.Size (value, height); }
		}

		/// <summary>Requested height.</summary>
		public int RequestedHeight {
			get { return height; }
			set { Size = new Gdk.Size (width, value); }
		}
	}
}
