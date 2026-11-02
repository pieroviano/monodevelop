using System;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	/// <summary>
	/// Right aligned row of buttons at the bottom of a dialog content area, for dialogs whose buttons run their own logic
	/// (validation, async work, ...) before responding. GTK 4 action widgets always emit a response when clicked, which would
	/// end a modal run before that logic completes; buttons in this bar only respond when the dialog calls <c>Respond</c>.
	/// </summary>
	static class DialogButtonBar
	{
		/// <summary>Appends an empty button bar to the content area of <paramref name="dialog"/>.</summary>
		public static Gtk.Box Add (Gtk.Dialog dialog)
		{
			var bar = DesignerLayout.HBox (6);
			bar.Halign = Gtk.Align.End;
			bar.MarginTop = 6;
			DesignerLayout.Pack (dialog.ContentArea, bar);
			return bar;
		}
	}
}
