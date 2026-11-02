//
// OverlayMessageWindow.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using Mono.TextEditor;
using Gtk;
using MonoDevelop.Components;
using Gdk;
using MonoDevelop.Ide.Editor.Highlighting;

namespace MonoDevelop.SourceEditor
{
	class OverlayMessageWindow : MonoDevelop.Components.BinWidget
	{
		const int border = 8;

		private Func<int> sizeFunc;

		ExtensibleTextEditor textEditor;

		public Func<int> SizeFunc {
			get {
				return sizeFunc;
			}

			set {
				sizeFunc = value;
				QueueResize ();
			}
		}

		public OverlayMessageWindow ()
		{
		}

		public void ShowOverlay (ExtensibleTextEditor textEditor)
		{
			this.textEditor = textEditor;
			textEditor.AddTopLevelWidget (this, 0, 0); 
			textEditor.SizeAllocated += HandleSizeAllocated;
			var child = textEditor [this];
			child.FixedPosition = true;
		}

		bool destroyed;

		public override void Destroy ()
		{
			if (!destroyed) {
				destroyed = true;
				if (textEditor != null) {
					textEditor.SizeAllocated -= HandleSizeAllocated;
					textEditor.Remove (this);
					textEditor = null;
				}
			}
			base.Destroy ();
		}

		protected override void OnMeasure (Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			base.OnMeasure (orientation, for_size, out minimum, out natural, out minimum_baseline, out natural_baseline);

			if (orientation == Orientation.Horizontal && SizeFunc != null && textEditor != null) {
				minimum = natural = Math.Min (SizeFunc (), textEditor.Allocation.Width - border * 2);
			}
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);
			AdjustPositionInEditor (width, height);
		}

		void HandleSizeAllocated (object o, EventArgs args)
		{
			AdjustPositionInEditor (Width, Height);
		}

		void AdjustPositionInEditor (int width, int height)
		{
			if (textEditor == null)
				return;
			textEditor.MoveTopLevelWidget (this, (textEditor.Allocation.Width - width) / 2, textEditor.Allocation.Height - height - 8);
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			if (textEditor != null) {
				snapshot.DrawCairo (this, cr => {
					cr.LineWidth = 1;
					cr.Rectangle (0, 0, Width, Height);

					cr.SetSourceColor (SyntaxHighlightingService.GetColor (textEditor.EditorTheme, EditorThemeColors.NotificationTextBackground));
					cr.Fill ();
					cr.RoundedRectangle (0, 0, Width, Height, 3);
					cr.SetSourceColor (SyntaxHighlightingService.GetColor (textEditor.EditorTheme, EditorThemeColors.NotificationTextBackground));
					cr.FillPreserve ();

					cr.SetSourceColor (SyntaxHighlightingService.GetColor (textEditor.EditorTheme, EditorThemeColors.NotificationBorder));
					cr.Stroke();
				});
			}

			base.OnSnapshot (snapshot);
		}

	}
}
