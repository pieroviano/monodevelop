//
// GtkColors.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007-2008 Novell, Inc.
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

namespace MonoDevelop.Components.Theming
{
	public enum GtkColorClass
	{
		Light,
		Mid,
		Dark,
		Base,
		Text,
		Background,
		Foreground
	}

	/// <summary>
	/// GTK 2 style color table (light/mid/dark/base/text/bg/fg per state). GTK 4 cannot query theme colors
	/// other than the foreground (<see cref="Widget.StyleColor"/>), so the table is derived from the widget
	/// foreground and MonoDevelop's <see cref="Styles"/> palette, and refreshed when either changes.
	/// </summary>
	class GtkColors
	{
		// States of the GTK 2 table: Normal, Active, Prelight, Selected, Insensitive.
		static readonly StateFlags[] states = { StateFlags.Normal, StateFlags.Active, StateFlags.Prelight, StateFlags.Selected, StateFlags.Insensitive };

		private Cairo.Color[] gtk_colors;
		private Widget widget;
		private bool refreshing = false;
		private bool listeningStyles;

		public event EventHandler Refreshed;

		public Widget Widget {
			get { return widget; }
			set {
				if (widget == value) {
					return;
				} else if (widget != null) {
					widget.Realized -= OnWidgetRealized;
					widget.Unrealized -= OnWidgetUnrealized;
					ListenStyles (false);
				}

				widget = value;
				if (widget == null)
					return;

				if (widget.IsRealized) {
					ListenStyles (true);
					RefreshColors ();
				}

				widget.Realized += OnWidgetRealized;
				widget.Unrealized += OnWidgetUnrealized;
			}
		}

		public GtkColors ()
		{
		}

		void ListenStyles (bool listen)
		{
			if (listen == listeningStyles)
				return;
			listeningStyles = listen;
			if (listen)
				Styles.Changed += OnStylesChanged;
			else
				Styles.Changed -= OnStylesChanged;
		}

		private void OnWidgetRealized (object o, EventArgs args)
		{
			ListenStyles (true);
			RefreshColors ();
		}

		private void OnWidgetUnrealized (object o, EventArgs args)
		{
			ListenStyles (false);
		}

		private void OnStylesChanged (object o, EventArgs args)
		{
			RefreshColors ();
		}

		static int StateIndex (StateFlags state)
		{
			// The most specific flag wins, as in GTK 2 where a widget was in exactly one state.
			if ((state & StateFlags.Insensitive) != 0)
				return 4;
			if ((state & StateFlags.Selected) != 0)
				return 3;
			if ((state & StateFlags.Active) != 0)
				return 1;
			if ((state & StateFlags.Prelight) != 0)
				return 2;
			return 0;
		}

		public Cairo.Color GetWidgetColor (GtkColorClass @class, StateFlags state)
		{
			if (gtk_colors == null) {
				RefreshColors ();
			}

			return gtk_colors[(int)@class * states.Length + StateIndex (state)];
		}

		public void RefreshColors ()
		{
			if (refreshing) {
				return;
			}

			refreshing = true;

			int sn = states.Length;
			int cn = (int)GtkColorClass.Foreground + 1;

			if (gtk_colors == null) {
				gtk_colors = new Cairo.Color[sn * cn];
			}

			for (int c = 0, i = 0; c < cn; c++) {
				for (int s = 0; s < sn; s++,i++) {
					gtk_colors[c * sn + s] = widget != null && widget.IsRealized
						? ComputeColor ((GtkColorClass)c, states [s])
						: new Cairo.Color (0, 0, 0);
				}
			}

			OnRefreshed ();

			refreshing = false;
		}

		Cairo.Color ComputeColor (GtkColorClass @class, StateFlags state)
		{
			bool selected = state == StateFlags.Selected;
			bool insensitive = state == StateFlags.Insensitive;

			Cairo.Color bg = Styles.BackgroundColor.ToCairoColor ();
			if (selected)
				bg = Styles.BaseSelectionBackgroundColor.ToCairoColor ();
			else if (state == StateFlags.Active)
				bg = CairoExtensions.ColorShade (bg, 0.9);
			else if (state == StateFlags.Prelight)
				bg = CairoExtensions.ColorShade (bg, 1.05);

			switch (@class) {
			case GtkColorClass.Light:
				return CairoExtensions.ColorShade (bg, 1.3);
			case GtkColorClass.Dark:
				return CairoExtensions.ColorShade (bg, 0.7);
			case GtkColorClass.Mid:
				return CairoExtensions.AlphaBlend (CairoExtensions.ColorShade (bg, 1.3), CairoExtensions.ColorShade (bg, 0.7), 0.5);
			case GtkColorClass.Background:
				return bg;
			case GtkColorClass.Base:
				if (selected)
					return Styles.BaseSelectionBackgroundColor.ToCairoColor ();
				if (insensitive)
					return Styles.BackgroundColor.ToCairoColor ();
				return Styles.BaseBackgroundColor.ToCairoColor ();
			case GtkColorClass.Text:
				if (selected)
					return Styles.BaseSelectionTextColor.ToCairoColor ();
				if (insensitive)
					return Styles.SecondaryTextColor.ToCairoColor ();
				return Styles.BaseForegroundColor.ToCairoColor ();
			default: // Foreground
				if (selected)
					return Styles.BaseSelectionTextColor.ToCairoColor ();
				if (insensitive)
					return Styles.SecondaryTextColor.ToCairoColor ();
				return widget.StyleColor.ToCairoColor ();
			}
		}

		protected virtual void OnRefreshed ()
		{
			EventHandler handler = Refreshed;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}
	}
}
