// 
// QuickTaskFullMode.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
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
using Gtk;
using Mono.TextEditor;
using Gdk;
using MonoDevelop.Core;
using MonoDevelop.Components;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Editor.Highlighting;

namespace MonoDevelop.SourceEditor.QuickTasks
{
	class QuickTaskMiniMapMode : Gtk.Box, IMapMode
	{
		Minimpap minimap;
		QuickTaskOverviewMode rightMap;
		Gtk.Adjustment vadjustment;

		public QuickTaskMiniMapMode (QuickTaskStrip parent) : base (Gtk.Orientation.Horizontal, 0)
		{
			minimap = new Minimpap (parent);
			minimap.Hexpand = true;
			Append (minimap);

			rightMap = new QuickTaskOverviewMode (parent);
			rightMap.Hexpand = true;
			Append (rightMap);
			vadjustment = parent.VAdjustment;

			vadjustment.ValueChanged += RedrawOnVAdjustmentChange;

		}

		bool destroyed;

		public override void Destroy ()
		{
			if (!destroyed) {
				destroyed = true;
				vadjustment.ValueChanged -= RedrawOnVAdjustmentChange;
			}

			base.Destroy ();
		}

		void RedrawOnVAdjustmentChange (object sender, EventArgs e)
		{
			QueueDraw ();
		}

		public void ForceDraw ()
		{
			minimap.RequestRedraw ();
		}

		class Minimpap : QuickTaskOverviewMode
		{
			const double lineHeight = 2;

			public Minimpap (QuickTaskStrip parent) : base (parent)
			{
				doc = parent.TextEditor.Document;
				doc.TextChanged += TextReplaced;
				doc.Folded += HandleFolded;
			}

			Cairo.ImageSurface backgroundPixbuf, backgroundBuffer;
			uint redrawTimeout;
			TextDocument doc;

			void HandleFolded (object sender, FoldSegmentEventArgs e)
			{
				RequestRedraw ();
			}

			void TextReplaced (object sender, TextChangeEventArgs args)
			{
				RequestRedraw ();
			}

			public void RemoveRedrawTimer ()
			{
				if (redrawTimeout != 0) {
					GLib.Source.Remove (redrawTimeout);
					redrawTimeout = 0;
				}
			}

			public void RequestRedraw ()
			{
				RemoveRedrawTimer ();
				redrawTimeout = GLib.Timeout.Add (450, delegate {
					if (curUpdate != null) {
						curUpdate.RemoveHandler ();
						curUpdate = null;
					}
					if (backgroundPixbuf != null)
						curUpdate = new BgBufferUpdate (this);
					redrawTimeout = 0;
					return false;
				});
			}

			protected override void DrawBar (Cairo.Context cr)
			{
				/*
				if (vadjustment == null || vadjustment.Upper <= vadjustment.PageSize) 
					return;
				var h = Allocation.Height;

				cr.Rectangle (
					1.5,
					y,
					Allocation.Width - 2,
					h * (vadjustment.PageSize / vadjustment.Upper));
				Cairo.Color color = (TextEditor.ColorStyle != null) ? TextEditor.ColorStyle.Default.CairoColor : new Cairo.Color (0, 0, 0);
				color.A = 0.5;
				cr.Color = color;
				cr.StrokePreserve ();
				
				color.A = 0.05;
				cr.Color = color;
				cr.Fill ();*/
			}


			protected override void MovePosition (double y)
			{
				var ph = (int)(lineHeight * (TextEditor.GetTextEditorData ().VisibleLineCount));
				double position = vadjustment.Upper * (Math.Min (GetBufferYOffset () + y, ph) / (double)ph) - vadjustment.PageSize / 2;
				position = Math.Max (vadjustment.Lower, Math.Min (position, vadjustment.Upper - vadjustment.PageSize));
				vadjustment.Value = position;
			}

			protected override void OnMeasure (Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
			{
				base.OnMeasure (orientation, for_size, out minimum, out natural, out minimum_baseline, out natural_baseline);
				if (orientation == Orientation.Horizontal)
					minimum = natural = 150;
			}

			void DestroyBgBuffer ()
			{
				if (curUpdate != null)
					curUpdate.RemoveHandler ();
				if (backgroundPixbuf != null) {
					backgroundPixbuf.Dispose ();
					backgroundBuffer.Dispose ();
					backgroundPixbuf = backgroundBuffer = null;
					curWidth = curHeight = -1;
				}
			}

			protected override void OnDestroyed ()
			{
				base.OnDestroyed ();
				doc.Folded -= HandleFolded;
				doc.TextChanged -= TextReplaced;
				RemoveRedrawTimer ();
				DestroyBgBuffer ();
			}

			protected override void OnSizeAllocate (int width, int height, int baseline)
			{
				base.OnSizeAllocate (width, height, baseline);
				if (width > 1 && (width != curWidth || height != curHeight))
					CreateBgBuffer ();
			}

			protected override void OnMapped ()
			{
				if (backgroundPixbuf == null && Width > 1)
					CreateBgBuffer ();
				base.OnMapped ();
			}

			protected override void OnUnmapped ()
			{
				DestroyBgBuffer ();
				base.OnUnmapped ();
			}

			BgBufferUpdate curUpdate = null;
			void SwapBuffer ()
			{
				var tmp = backgroundPixbuf;
				backgroundPixbuf = backgroundBuffer;
				backgroundBuffer = tmp;
			}

			int curWidth = -1, curHeight = -1;
			void CreateBgBuffer ()
			{
				DestroyBgBuffer ();
				curWidth = Width;
				curHeight = Math.Max (Height, (int)(lineHeight * (TextEditor.GetTextEditorData ().VisibleLineCount)));
				if (!IsRealized || curWidth < 1 || curHeight < 1)
					return;
				var displayScale = Platform.IsWindows ? GtkWorkarounds.GetScaleFactor (this) : 1.0;
				backgroundPixbuf = new Cairo.ImageSurface (Cairo.Format.ARGB32, (int)(curWidth * displayScale), (int)(curHeight * displayScale));
				backgroundBuffer = new Cairo.ImageSurface (Cairo.Format.ARGB32, (int)(curWidth * displayScale), (int)(curHeight * displayScale));

				if (TextEditor.EditorTheme != null) {
					using (var cr = new Cairo.Context (backgroundPixbuf)) {
						cr.Rectangle (0, 0, curWidth * displayScale, curHeight * displayScale);
						cr.SetSourceColor (SyntaxHighlightingService.GetColor (TextEditor.EditorTheme, EditorThemeColors.Background));
						cr.Fill ();
					}
				}
				curUpdate = new BgBufferUpdate (this);
			}

			class BgBufferUpdate
			{
				int maxLine;
				double sx;
				double sy;
				uint handler;

				Cairo.Context cr;

				Minimpap mode;

				int curLine = 1;

				public BgBufferUpdate (Minimpap mode)
				{
					this.mode = mode;

					cr = new Cairo.Context (mode.backgroundBuffer);

					cr.LineWidth = 1;
					int w = mode.backgroundBuffer.Width;
					int h = mode.backgroundBuffer.Height;
					cr.Rectangle (0, 0, w, h);
					if (mode.TextEditor.EditorTheme != null)
						cr.SetSourceColor (SyntaxHighlightingService.GetColor (mode.TextEditor.EditorTheme, EditorThemeColors.Background));
					cr.Fill ();

					maxLine = mode.TextEditor.GetTextEditorData ().VisibleLineCount;
					sx = w / (double)mode.TextEditor.Width;
					sy = Math.Min (1, lineHeight * maxLine / (double)mode.TextEditor.GetTextEditorData ().TotalHeight);
					cr.Scale (sx, sy);

					handler = GLib.Idle.Add (BgBufferUpdater);
				}

				public void RemoveHandler ()
				{
					if (cr == null)
						return;
					GLib.Source.Remove (handler);
					handler = 0;
					((IDisposable)cr).Dispose ();
					cr = null;
					mode.curUpdate = null;
				}

				bool BgBufferUpdater ()
				{
					if (mode.TextEditor.Document == null || handler == 0)
						return false;
					try {
						for (int i = 0; i < 25 && curLine < maxLine; i++) {
							var nr = mode.TextEditor.GetTextEditorData ().VisualToLogicalLine (curLine);
							var line = mode.TextEditor.GetLine (nr);
							if (line != null) {
								var layout = mode.TextEditor.TextViewMargin.GetLayout (line);
								cr.MoveTo (0, (curLine - 1) * mode.TextEditor.LineHeight);
								cr.ShowLayout (layout.Layout);

								if (layout.IsUncached)
									layout.Dispose ();
							}

							curLine++;
						}

						if (curLine >= maxLine) {
							mode.SwapBuffer ();
							((IDisposable)cr).Dispose ();
							cr = null;
							mode.curUpdate = null;
							mode.QueueDraw ();
							handler = 0;
							return false;
						}
					} catch (Exception e) {
						LoggingService.LogError ("Error in background buffer drawer.", e);
						handler = 0;
						return false;
					}
					return true;
				}
			}

			int GetBufferYOffset ()
			{
				var displayScale = Platform.IsWindows ? GtkWorkarounds.GetScaleFactor (this) : 1.0;
				int h = (int)(backgroundPixbuf.Height / displayScale) - Height;
				if (h < 0)
					return 0;
				return Math.Max (0, (int)(h * (vadjustment.Value) / (vadjustment.Upper - vadjustment.Lower - vadjustment.PageSize)));
			}

			protected override void OnSnapshot (Gtk.Snapshot snapshot)
			{
				if (TextEditor == null || Width <= 0 || Height <= 0)
					return;
				using (Cairo.Context cr = snapshot.CreateCairoContext (this)) {
					cr.LineWidth = 1;
					if (backgroundPixbuf != null) {
						cr.Save ();
						cr.Rectangle (0, 0, Width, Height);
						cr.Clip ();
						cr.SetSourceSurface (backgroundPixbuf, 0, -GetBufferYOffset ());
						cr.Paint ();
						cr.Restore ();
					} else {
						cr.Rectangle (0, 0, Width, Height);
						if (TextEditor.EditorTheme != null)
							cr.SetSourceColor (SyntaxHighlightingService.GetColor (TextEditor.EditorTheme, EditorThemeColors.Background));
						cr.Fill ();
					}
					/*
					cr.Color = (HslColor)Style.Dark (State);
					cr.MoveTo (-0.5, 0.5);
					cr.LineTo (Allocation.Width, 0.5);
					cr.MoveTo (-0.5, Allocation.Height - 0.5);
					cr.LineTo (Allocation.Width, Allocation.Height - 0.5);
					cr.Stroke ();*/

					if (backgroundPixbuf != null) {
						int y = GetBufferYOffset ();

						int startLine = TextEditor.YToLine (vadjustment.Value);
						double dy = TextEditor.LogicalToVisualLocation (startLine, 1).Line * lineHeight;

						cr.Rectangle (0,
									  dy - y,
									  Width,
									  lineHeight * vadjustment.PageSize / TextEditor.LineHeight);
						var c = MonoDevelop.Ide.Gui.Styles.WidgetBorderColor.ToCairoColor ();
						c.A = 0.2;
						cr.SetSourceColor (c);
						cr.Fill ();
					}
					DrawLeftBorder (cr);
				}
			}
		}
	}

}
