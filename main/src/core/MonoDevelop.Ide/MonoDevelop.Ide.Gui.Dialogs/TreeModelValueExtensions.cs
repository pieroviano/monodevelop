using System;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	/// <summary>
	/// GTK 4's <see cref="Gtk.ITreeModel"/> only exposes <c>GetValue (iter, column, ref GLib.Value)</c>; this restores
	/// the GTK 2 <c>model.GetValue (iter, column)</c> convenience for cell data and row separator callbacks.
	/// </summary>
	static class TreeModelValueExtensions
	{
		public static object GetModelValue (this Gtk.ITreeModel model, Gtk.TreeIter iter, int column)
		{
			var value = new GLib.Value ();
			try {
				model.GetValue (iter, column, ref value);
				return value.Val;
			} finally {
				value.Dispose ();
			}
		}
	}
}
