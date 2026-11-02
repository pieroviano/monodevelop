using System;

/// <summary>
/// GTK 4 replacements of <c>Widget.GetPointer</c> for the Visual Studio editor glue code.
/// </summary>
static class GtkPointerExtensions
{
	/// <summary>The pointer position relative to <paramref name="widget"/>; (0, 0) when it cannot be determined.</summary>
	public static void GetPointer (this Gtk.Widget widget, out int x, out int y)
	{
		if (MonoDevelop.Components.Docking.DockUtil.TryGetPointer (widget, out double px, out double py)) {
			x = (int)px;
			y = (int)py;
		} else {
			x = y = 0;
		}
	}
}
