// GTK 4 port of the former GUI designer support file.
// The designer-built widgets are now laid out natively (see MonoDevelop.Components.BinWidget), so the
// size-request / size-allocate forwarding and UIManager accelerator plumbing of the GTK 2 version are gone.
namespace MonoDevelop.Components
{
	public class Gui
	{
		public static bool initialized;

		public static void Initialize (Gtk.Widget iconRenderer)
		{
			if ((initialized == false)) {
				initialized = true;
			}
		}
	}

	/// <summary>
	/// Remains only so that designer-style <c>BinContainer.Attach (this)</c> calls keep compiling.
	/// A single-child container (<see cref="BinWidget"/>) measures and allocates its child itself, so there is nothing to attach.
	/// </summary>
	public class BinContainer
	{
		public static BinContainer Attach (Gtk.Widget bin)
		{
			return new BinContainer ();
		}
	}
}
