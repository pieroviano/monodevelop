using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Input
{
	static class MouseSimulatingExtensions
	{
		public static bool IsMouseOver (this Gtk.Widget widget)
		{
			if (!MonoDevelop.Components.Docking.DockUtil.TryGetPointer (widget, out double x, out double y))
				return false;
			return x >= 0 && y >= 0 && x <= widget.Width && y <= widget.Height;
		}

		public static bool IsMouseOver (this Xwt.Widget widget)
		{
			var mousePosition = Xwt.Desktop.MouseLocation;
			return widget.ScreenBounds.Contains (mousePosition);
		}

		public static bool IsMouseOver (this Xwt.Window widget)
		{
			var mousePosition = Xwt.Desktop.MouseLocation;
			return widget.ScreenBounds.Contains (mousePosition);
		}
	}
}
