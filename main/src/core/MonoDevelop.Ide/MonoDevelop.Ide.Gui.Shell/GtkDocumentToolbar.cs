// 
// DocumentToolbar.cs
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
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Ide.Gui.Shell
{
	class GtkDocumentToolbar : IShellDocumentToolbar
	{
		const string ToolbarCssClass = "md-document-toolbar";
		static Gtk.CssProvider textColorProvider;

		Gtk.Widget frame;
		Box box;
		bool empty = true;

		internal GtkDocumentToolbar ()
		{
			box = new Box (Orientation.Horizontal, 3);
			box.Hexpand = true;
			var al = new ToolbarBox ();
			al.Padding = new Border { Left = 5, Top = 4, Bottom = 4 };
			al.Child = box;
			al.Hexpand = true;
			al.AddCssClass (ToolbarCssClass);
			frame = al;
			EnsureTextColorProvider ();
		}

		// Children of the toolbar use the breadcrumb text color (replaces the GTK 2 ModifyFg/ModifyText calls)
		static void EnsureTextColorProvider ()
		{
			if (textColorProvider != null)
				return;
			textColorProvider = new Gtk.CssProvider ();
			UpdateTextColor ();
			Gtk.StyleContext.AddProviderForDisplay (Gdk.Display.Default, textColorProvider, (uint)Gtk.StyleProviderPriority.Application);
			Styles.Changed += (o, e) => UpdateTextColor ();
		}

		static void UpdateTextColor ()
		{
			var color = Styles.BreadcrumbTextColor.ToGdkColor ().GetHex ();
			textColorProvider.LoadFromString ($".{ToolbarCssClass}, .{ToolbarCssClass} * {{ color: {color}; }}");
		}

		internal Control Container {
			get { return frame; }
		}

		public void Insert (Control w, int index)
		{
			Add (w, false, 0, index);
		}

		public void Add (Control control, bool fill, int padding)
		{
			Add (control, fill, padding, -1);
		}

		void Add (Control control, bool fill, int padding, int index)
		{
			int defaultPadding = 3;

			Gtk.Widget widget = control;
			if (widget is Button button) {
				button.HasFrame = false;
				button.FocusOnClick = false;
				defaultPadding = 0;
			} else if (widget is Entry entry) {
				entry.HasFrame = false;
			} else if (widget is ComboBox combo) {
				combo.HasFrame = false;
			} else if (widget is Separator separator && separator.Orientation == Orientation.Vertical) {
				separator.HeightRequest = 10;
			}

			if (padding == -1)
				padding = defaultPadding;

			widget.Hexpand = fill;
			widget.MarginStart = padding;
			widget.MarginEnd = padding;
			box.Append (widget);
			if (empty) {
				empty = false;
				frame.Visible = true;
			}
			if (index != -1) {
				var siblings = new List<Widget> ();
				for (var c = box.FirstChild; c != null; c = c.NextSibling) {
					if (c != widget)
						siblings.Add (c);
				}
				box.ReorderChildAfter (widget, index <= 0 || siblings.Count == 0 ? null : siblings [Math.Min (index, siblings.Count) - 1]);
			}
		}

		public void AddSpace ()
		{
			var spacer = new Box (Orientation.Horizontal, 0);
			spacer.SetShouldIgnore (true);
			Add (spacer, true, 0, -1);
		}

		public void Remove (Control widget)
		{
			box.Remove (widget);
		}

		public bool Visible {
			get {
				return empty || frame.Visible;
			}
			set {
				frame.Visible = value;
			}
		}

		public bool Sensitive {
			get { return frame.Sensitive; }
			set { frame.Sensitive = value; }
		}

		public void ShowAll ()
		{
			frame.Visible = true;
		}

		public Control [] Children {
			get {
				var children = new List<Control> ();
				for (var child = box.FirstChild; child != null; child = child.NextSibling)
					children.Add ((Control)child);
				return children.ToArray ();
			}
		}

		class ToolbarBox : BinWidget
		{
			protected override void OnSnapshot (Gtk.Snapshot snapshot)
			{
				int width = Width, height = Height;
				if (width > 0 && height > 0) {
					snapshot.DrawCairo (this, ctx => {
						ctx.Rectangle (0, 0, width, height);
						ctx.SetSourceColor (Styles.BreadcrumbBackgroundColor.ToCairoColor ());
						ctx.Fill ();

						ctx.MoveTo (0.5, height - 0.5);
						ctx.RelLineTo (width, 0);
						ctx.SetSourceColor (Styles.BreadcrumbBottomBorderColor.ToCairoColor ());
						ctx.LineWidth = 1;
						ctx.Stroke ();
					});
				}
				base.OnSnapshot (snapshot);
			}
		}
	}

	public class DocumentToolButton : Control
	{
		public ImageView Image {
			get { return image; }
			set {
				if (image != null)
					content.Remove (image);
				image = value;
				if (image != null)
					content.Prepend (image);
			}
		}

		public string TooltipText {
			get { return button.TooltipText; }
			set { button.TooltipText = value; }
		}

		public string Label {
			get { return label.LabelProp; }
			set {
				label.LabelProp = value ?? "";
				label.Visible = !string.IsNullOrEmpty (value);
			}
		}

		Gtk.Button button;
		Gtk.Box content;
		Gtk.Label label;
		ImageView image;

		public DocumentToolButton (string stockId) : this (stockId, null)
		{
		}

		public DocumentToolButton (string stockId, string label)
		{
			button = new Button ();
			content = new Box (Orientation.Horizontal, 3);
			this.label = new Gtk.Label ();
			content.Append (this.label);
			button.Child = content;
			Label = label;
			Image = new ImageView (stockId, StockIconSize.Menu);
		}

		protected override object CreateNativeWidget<T> ()
		{
			return button;
		}

		public event EventHandler Clicked {
			add {
				button.Clicked += value;
			}
			remove {
				button.Clicked -= value;
			}
		}

		public class DocumentToolButtonImage : Control
		{
			ImageView image;
			internal DocumentToolButtonImage (ImageView image)
			{
				this.image = image;
			}

			protected override object CreateNativeWidget<T> ()
			{
				return image;
			}

			public static implicit operator Gtk.Widget (DocumentToolButtonImage d)
			{
				return d.GetNativeWidget<Gtk.Widget> ();
			}

			public static implicit operator DocumentToolButtonImage (ImageView d)
			{
				return new DocumentToolButtonImage (d);
			}
		}
	}
}
