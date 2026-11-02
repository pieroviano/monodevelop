// Helpers used by the hand-ported GTK 4 Build () methods of the former GUI designer files of the
// Deployment, Autotools, Gettext, ChangeLog and DocFood add-ins. The file is shared (linked) between those
// projects; the class is internal in each of them.
using System;
using MonoDevelop.Components;

namespace MonoDevelop.Components.Designer
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
		/// Sets the single content of a designer-built root: <see cref="BinWidget"/>, <see cref="Gtk.Window"/>,
		/// <see cref="ContainerWidget"/>, <see cref="Gtk.Box"/> or <see cref="Gtk.Frame"/>.
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

		/// <summary>Adds a dialog action button that emits <paramref name="response"/> when activated.</summary>
		public static Gtk.Button AddActionButton (Gtk.Dialog dialog, Gtk.Button button, Gtk.ResponseType response)
		{
			dialog.AddActionWidget (button, response);
			return button;
		}

		/// <summary>
		/// Horizontal box at the bottom of the dialog, for dialogs that mix buttons that emit a response with buttons
		/// that are handled by the dialog itself (GTK 4 offers no public API to add the latter to the action area).
		/// </summary>
		public static Gtk.Box ActionBox (Gtk.Dialog dialog)
		{
			var box = HBox (6);
			box.Halign = Gtk.Align.End;
			box.MarginTop = 6;
			box.MarginBottom = 6;
			box.MarginStart = 6;
			box.MarginEnd = 6;
			dialog.ContentArea.Append (box);
			return box;
		}

		/// <summary>Adds a button to <paramref name="actions"/> which emits <paramref name="response"/> from <paramref name="dialog"/> when clicked.</summary>
		public static Gtk.Button AddResponseButton (Gtk.Dialog dialog, Gtk.Box actions, Gtk.Button button, Gtk.ResponseType response)
		{
			button.Clicked += (s, e) => dialog.Respond (response);
			actions.Append (button);
			return button;
		}

		/// <summary>Content area of <paramref name="dialog"/> with the designer's default border.</summary>
		public static Gtk.Box ContentArea (Gtk.Dialog dialog, int border = 6)
		{
			var area = dialog.ContentArea;
			Margin (area, border);
			return area;
		}
	}
}
