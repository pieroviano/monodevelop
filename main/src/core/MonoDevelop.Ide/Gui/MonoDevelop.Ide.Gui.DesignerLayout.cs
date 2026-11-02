// Helpers used by the hand-ported GTK 4 Build () methods of the former GUI designer files in this folder.
using System;
using MonoDevelop.Components;

namespace MonoDevelop.Ide.Gui
{
	/// <summary>
	/// Small construction helpers replacing the GTK 2 designer idioms (BoxChild packing, Alignment padding,
	/// Table attachments, stock buttons, Dialog action area) with their native GTK 4 equivalents.
	/// </summary>
	static class DesignerLayout
	{
		/// <summary>Vertical <see cref="Gtk.Box"/>.</summary>
		public static Gtk.Box VBox (int spacing = 6)
		{
			return new Gtk.Box (Gtk.Orientation.Vertical, spacing);
		}

		/// <summary>Horizontal <see cref="Gtk.Box"/>.</summary>
		public static Gtk.Box HBox (int spacing = 6)
		{
			return new Gtk.Box (Gtk.Orientation.Horizontal, spacing);
		}

		/// <summary>
		/// Appends <paramref name="child"/> to <paramref name="box"/>. When <paramref name="expand"/> is set the child
		/// grows along the box orientation (the GTK 2 <c>Expand = true</c> box child property).
		/// </summary>
		public static T Pack<T> (Gtk.Box box, T child, bool expand = false) where T : Gtk.Widget
		{
			if (expand) {
				if (box.Orientation == Gtk.Orientation.Horizontal)
					child.Hexpand = true;
				else
					child.Vexpand = true;
			}
			box.Append (child);
			return child;
		}

		/// <summary>Sets the same margin on the four sides of <paramref name="widget"/> (replaces BorderWidth).</summary>
		public static T Margin<T> (T widget, int all) where T : Gtk.Widget
		{
			return Margin (widget, all, all, all, all);
		}

		/// <summary>Sets the margins of <paramref name="widget"/> (replaces Alignment padding).</summary>
		public static T Margin<T> (T widget, int top, int start, int bottom, int end) where T : Gtk.Widget
		{
			widget.MarginTop = top;
			widget.MarginStart = start;
			widget.MarginBottom = bottom;
			widget.MarginEnd = end;
			return widget;
		}

		/// <summary>Button with a mnemonic label (e.g. "_Add").</summary>
		public static Gtk.Button Button (string label)
		{
			return new Gtk.Button { Label = label, UseUnderline = true };
		}

		/// <summary>
		/// Radio button (GTK 4 removed RadioButton: grouped <see cref="Gtk.CheckButton"/>s). Pass the first button of
		/// the group as <paramref name="group"/> for the following ones.
		/// </summary>
		public static Gtk.CheckButton Radio (string label, Gtk.CheckButton group = null)
		{
			var button = new Gtk.CheckButton { Label = label, UseUnderline = true };
			if (group != null)
				button.Group = group;
			return button;
		}

		/// <summary>Label left aligned, optionally with markup and a mnemonic target.</summary>
		public static Gtk.Label Label (string text, bool markup = false, Gtk.Widget mnemonicWidget = null)
		{
			var label = new Gtk.Label ((string)null) {
				Xalign = 0f,
				Halign = Gtk.Align.Start,
			};
			if (markup)
				label.UseMarkup = true;
			if (text != null)
				label.LabelProp = text;
			if (mnemonicWidget != null) {
				label.UseUnderline = true;
				label.MnemonicWidget = mnemonicWidget;
			}
			return label;
		}

		/// <summary>Scrolled window hosting <paramref name="child"/> (TreeView, TextView... scroll natively).</summary>
		public static Gtk.ScrolledWindow Scrolled (Gtk.Widget child, bool hasFrame = true)
		{
			var sw = new Gtk.ScrolledWindow {
				HasFrame = hasFrame,
				Hexpand = true,
				Vexpand = true,
			};
			sw.Child = child;
			return sw;
		}

		/// <summary>Grid with the given spacings (replaces Gtk.Table).</summary>
		public static Gtk.Grid Grid (uint rowSpacing = 6, uint columnSpacing = 6)
		{
			return new Gtk.Grid {
				RowSpacing = rowSpacing,
				ColumnSpacing = columnSpacing,
			};
		}

		/// <summary>Attaches <paramref name="child"/> to <paramref name="grid"/> (replaces Table.Attach with Xoptions/Yoptions).</summary>
		public static T Attach<T> (Gtk.Grid grid, T child, int column, int row, int columnSpan = 1, int rowSpan = 1, bool hexpand = false, bool vexpand = false) where T : Gtk.Widget
		{
			if (hexpand)
				child.Hexpand = true;
			if (vexpand)
				child.Vexpand = true;
			grid.Attach (child, column, row, columnSpan, rowSpan);
			return child;
		}

		/// <summary>
		/// A titled section: bold <paramref name="title"/> above <paramref name="content"/> indented by 12 pixels
		/// (the Stetic "frame with bold label and alignment" idiom).
		/// </summary>
		public static Gtk.Box Section (string title, Gtk.Widget content)
		{
			var box = VBox (6);
			Pack (box, Label ("<b>" + GLib.Markup.EscapeText (title) + "</b>", true));
			content.MarginStart = 12;
			Pack (box, content, true);
			return box;
		}

		/// <summary>
		/// Sets the single content of a designer-built root: <see cref="BinWidget"/>, <see cref="Gtk.Window"/>,
		/// <see cref="ContainerWidget"/> or <see cref="Gtk.Box"/>.
		/// </summary>
		public static void SetContent (Gtk.Widget host, Gtk.Widget content)
		{
			switch (host) {
			case BinWidget bin:
				bin.Child = content;
				break;
			case ContainerWidget container:
				container.AddChild (content);
				break;
			case Gtk.Window window:
				window.Child = content;
				break;
			case Gtk.Box box:
				box.Append (content);
				break;
			case Gtk.Frame frame:
				frame.Child = content;
				break;
			default:
				throw new NotSupportedException ("Unsupported designer root: " + host.GetType ());
			}
		}

		/// <summary>Adds a dialog action button (replaces ActionArea packing); the first button added with <paramref name="isDefault"/> becomes the default.</summary>
		public static Gtk.Button AddActionButton (Gtk.Dialog dialog, Gtk.Button button, Gtk.ResponseType response, bool isDefault = false)
		{
			dialog.AddActionWidget (button, response);
			if (isDefault)
				dialog.DefaultWidget = button;
			return button;
		}

		/// <summary>Content area of <paramref name="dialog"/> with the designer's default 6px border.</summary>
		public static Gtk.Box ContentArea (Gtk.Dialog dialog, int border = 6)
		{
			var area = dialog.ContentArea;
			Margin (area, border);
			return area;
		}
	}
}
