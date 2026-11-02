//
// MonoDevelopStatusBar.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
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
using Gtk;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.Docking;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide.Gui.Shell;

namespace MonoDevelop.Ide
{
	class MonoDevelopStatusBar : Gtk.Box
	{
		Gtk.DrawingArea resizeGrip = new Gtk.DrawingArea ();

		const int ResizeGripWidth = 14;

		internal MonoDevelopStatusBar () : base (Gtk.Orientation.Horizontal, 0)
		{
			HasResizeGrip = true;

			this.SetRole (AtkCocoa.Roles.AXGroup);

			HeaderBox hb = new HeaderBox (1, 0, 0, 0);
			hb.SetRole (AtkCocoa.Roles.AXGroup);
			void UpdateColors ()
			{
				hb.BorderColor = Styles.DockSeparatorColor.ToGdkColor ();
				hb.BackgroundColor = Styles.DockBarBackground.ToGdkColor ();
			}
			UpdateColors ();
			Styles.Changed += (o, args) => UpdateColors ();
			var mainBox = new Gtk.Box (Gtk.Orientation.Horizontal, 0);
			mainBox.SetRole (AtkCocoa.Roles.AXGroup);
			// filler pushing the dock bar and the resize grip to the end
			var filler = new Gtk.Box (Gtk.Orientation.Horizontal, 0);
			filler.Hexpand = true;
			filler.SetRole (AtkCocoa.Roles.AXGroup);
			mainBox.Append (filler);
			mainBox.Hexpand = true;
			hb.Child = mainBox;
			hb.Hexpand = true;
			Append (hb);

			// Dock area

			CustomFrame dfr = new CustomFrame (0, 0, 1, 0);
			dfr.SetRole (AtkCocoa.Roles.AXGroup);
			void UpdateFrameColors ()
			{
				dfr.BorderColor = Styles.DockSeparatorColor.ToGdkColor ();
			}
			UpdateFrameColors ();
			Styles.Changed += (o, args) => UpdateFrameColors ();
			DefaultWorkbench wb = (DefaultWorkbench)IdeApp.Workbench.RootWindow;
			var dockBar = wb.DockFrame.ExtractDockBar (PositionType.Bottom);
			dockBar.AlignToEnd = true;
			dockBar.ShowBorder = false;
			dfr.Child = dockBar;
			mainBox.Append (dfr);

			// Resize grip

			resizeGrip.SetRole (AtkCocoa.Roles.AXGrowArea);
			resizeGrip.SetSizeRequest (ResizeGripWidth, -1);
			resizeGrip.DrawFunc = DrawResizeGrip;
			mainBox.Append (resizeGrip);

			resizeGrip.AddClickHandler (click => {
				if (click.Button == 1 && resizeGrip.Native?.Surface != null) {
					var toplevel = Gdk.ToplevelAdapter.GetObject (resizeGrip.Native.Surface);
					resizeGrip.TranslateCoordinates ((Gtk.Widget)resizeGrip.Root, click.X, click.Y, out double rootX, out double rootY);
					toplevel.BeginResize (Direction == TextDirection.Ltr ? Gdk.SurfaceEdge.SouthEast : Gdk.SurfaceEdge.SouthWest,
						click.Gesture.CurrentEventDevice, (int)click.Button, rootX, rootY, click.Time);
				}
			}, button: 1);

//			// todo: Move this to the CompletionWindowManager when it's possible.
//			StatusBarContext completionStatus = null;
//			CompletionWindowManager.WindowShown += delegate {
//				CompletionListWindow wnd = CompletionWindowManager.Wnd;
//				if (wnd != null && wnd.List != null && wnd.List.CategoryCount > 1) {
//					if (completionStatus == null)
//						completionStatus = CreateContext ();
//					completionStatus.ShowMessage (string.Format (GettextCatalog.GetString ("To toggle categorized completion mode press {0}."), IdeApp.CommandService.GetCommandInfo (Commands.TextEditorCommands.ShowCompletionWindow).AccelKey));
//				}
//			};
		}

		[Obsolete]
		public void ShowCaretState (int line, int column, int selectedChars, bool isInInsertMode)
		{
		}

		[Obsolete]
		public void ClearCaretState ()
		{
		}

		bool hasResizeGrip;
		public bool HasResizeGrip {
			get { return hasResizeGrip; }
			set { hasResizeGrip = value; resizeGrip.Visible = hasResizeGrip; }
		}

		// Draws the classic diagonal grip lines in the bottom corner (Gtk.Style.PaintResizeGrip no longer exists)
		void DrawResizeGrip (Gtk.DrawingArea area, Cairo.Context cr, int width, int height)
		{
			var color = area.StyleColor.ToCairoColor ();
			cr.SetSourceRGBA (color.R, color.G, color.B, 0.5);
			cr.LineWidth = 1;
			bool ltr = Direction == TextDirection.Ltr;
			for (int i = 1; i <= 3; i++) {
				int d = i * 4;
				if (ltr) {
					cr.MoveTo (width - d, height - 0.5);
					cr.LineTo (width - 0.5, height - d);
				} else {
					cr.MoveTo (d, height - 0.5);
					cr.LineTo (0.5, height - d);
				}
			}
			cr.Stroke ();
		}
	}
}
