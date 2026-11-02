// 
// WelcomePageFallbackWidget.cs
// 
// Author:
//   Scott Ellington
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
// Copyright (c) 2005 Scott Ellington
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using Gdk;
using Gtk;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Desktop;
using System.Reflection;
using System.Xml.Linq;

namespace MonoDevelop.Ide.WelcomePage
{
	public class WelcomePageWidget : BinWidget
	{
		public Xwt.Drawing.Image LogoImage { get; set; }
		public int LogoHeight { get; set; }
		public Xwt.Drawing.Image TopBorderImage { get; set; }
		public Xwt.Drawing.Image BackgroundImage { get; set; }
		public string BackgroundColor { get; set; }

		protected double OverdrawOpacity {
			get { return Background.OverdrawOpacity; }
			set { Background.OverdrawOpacity = value; }
		}

		protected int OverdrawOffset {
			get { return Background.OverdrawOffset; }
			set { Background.OverdrawOffset = value; }
		}

		WelcomePageWidgetBackground Background { get; set; }

		public bool ShowScrollbars { get; set; }

		public WelcomePageWidget ()
		{
			ShowScrollbars = true;

			UpdateTeme (null, null);
			LogoHeight = 90;

			var background = new WelcomePageWidgetBackground ();
			background.SetShouldIgnore (true);
			Background = background;
			background.Owner = this;
			var mainBox = new Gtk.Box (Gtk.Orientation.Vertical, 0);
			mainBox.SetShouldIgnore (true);
			background.Child = mainBox;

			BuildContent (mainBox);

			if (ShowScrollbars) {
				var scroller = new ScrolledWindow ();
				scroller.SetShouldIgnore (true);
				scroller.HasFrame = false;
				scroller.Hexpand = true;
				scroller.Vexpand = true;
				scroller.Child = background;
				Child = scroller;
			} else
				Child = background;

			if (LogoImage != null) {
				var logoHeight = LogoHeight;
				mainBox.MarginTop = logoHeight + Styles.WelcomeScreen.Spacing;
				mainBox.MarginStart = Styles.WelcomeScreen.Spacing;
			}
		}

		protected override void OnRoot ()
		{
			base.OnRoot ();
			IdeApp.Workbench.GuiLocked += OnLock;
			IdeApp.Workbench.GuiUnlocked += OnUnlock;
			MonoDevelop.Ide.Gui.Styles.Changed += UpdateTeme;
		}

		protected override void OnUnroot ()
		{
			IdeApp.Workbench.GuiLocked -= OnLock;
			IdeApp.Workbench.GuiUnlocked -= OnUnlock;
			MonoDevelop.Ide.Gui.Styles.Changed -= UpdateTeme;
			base.OnUnroot ();
		}

		void UpdateTeme (object sender, EventArgs e)
		{
			BackgroundColor = Styles.WelcomeScreen.BackgroundColor;
		}

		void OnLock (object s, EventArgs a)
		{
			Sensitive = false;
		}
		
		void OnUnlock (object s, EventArgs a)
		{
			Sensitive = true;
		}

		/// <summary>Adds the page content to <paramref name="parent"/> (a vertical box inside the scrolled background).</summary>
		protected virtual void BuildContent (Gtk.Box parent)
		{
		}

		public class WelcomePageWidgetBackground : BinWidget
		{
			public WelcomePageWidget Owner { get; set; }

			public double OverdrawOpacity { get; set; }
			public int OverdrawOffset { get; set; }

			Cairo.Color backgroundColor = new Cairo.Color (1, 1, 1);

			void UpdateTeme (object sender, EventArgs e)
			{
				if (!TryParseColor (Owner.BackgroundColor, ref backgroundColor) || !TryParseColor (Styles.WelcomeScreen.BackgroundColor, ref backgroundColor))
					backgroundColor = new Cairo.Color (1, 1, 1);
				QueueDraw ();
			}

			static bool TryParseColor (string color, ref Cairo.Color result)
			{
				try {
					result = CairoExtensions.ParseColor (color);
					return true;
				} catch (Exception) {
					return false;
				}
			}

			protected override void OnRoot ()
			{
				base.OnRoot ();
				UpdateTeme (null, null);
				MonoDevelop.Ide.Gui.Styles.Changed -= UpdateTeme;
				MonoDevelop.Ide.Gui.Styles.Changed += UpdateTeme;
			}

			protected override void OnUnroot ()
			{
				MonoDevelop.Ide.Gui.Styles.Changed -= UpdateTeme;
				base.OnUnroot ();
			}

			void DrawOverdraw (Cairo.Context context, double opacity)
			{
				if (Owner.BackgroundImage == null)
					return;

				var allocation = new Gdk.Rectangle (0, 0, Width, Height);
				context.RenderTiled (this, Owner.BackgroundImage, allocation, new Gdk.Rectangle (0, OverdrawOffset, allocation.Width, allocation.Height - OverdrawOffset), opacity);
			}

			void DrawBackground (Cairo.Context context)
			{
				if (Owner.BackgroundImage == null)
					return;

				var allocation = new Gdk.Rectangle (0, 0, Width, Height);
				context.RenderTiled (this, Owner.BackgroundImage, allocation, allocation, 1);
			}

			protected override void OnSnapshot (Gtk.Snapshot snapshot)
			{
				snapshot.DrawCairo (this, context => {
					context.SetSourceRGB (backgroundColor.R, backgroundColor.G, backgroundColor.B);
					context.Operator = Cairo.Operator.Source;
					context.Paint ();
					context.Operator = Cairo.Operator.Over;
					DrawBackground (context);

					if (Owner.LogoImage != null) {
						context.DrawImage (this, Owner.LogoImage, 0, 0);

						var bgRect = new Gdk.Rectangle ((int)Owner.LogoImage.Width, 0, Width - (int)Owner.LogoImage.Width, (int)Owner.TopBorderImage.Height);
						for (int x = bgRect.X; x < bgRect.Right; x += (int)Owner.TopBorderImage.Width)
							context.DrawImage (this, Owner.TopBorderImage.WithSize (Owner.TopBorderImage.Width, bgRect.Height), x, 0);
					}
				});

				base.OnSnapshot (snapshot);

				if (OverdrawOpacity > 0)
					snapshot.DrawCairo (this, context => DrawOverdraw (context, OverdrawOpacity));
			}
		}
	}
}
