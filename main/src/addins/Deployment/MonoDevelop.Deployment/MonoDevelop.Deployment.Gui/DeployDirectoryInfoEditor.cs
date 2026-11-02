
using System;
using System.ComponentModel;
using MonoDevelop.Components.PropertyGrid;
using Gtk;
using Gdk;

namespace MonoDevelop.Deployment
{
	internal class DeployDirectoryInfoEditor: PropertyEditorCell
	{
		protected override string GetValueText ()
		{
			if (Value != null)
				return ((DeployDirectoryInfo)Value).Description;
			else
				return string.Empty;
		}
		
		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cell_area, Gtk.StateFlags state)
		{
			return new DeployDirectoryInfoEditorWidget ();
		}
	}
	
	class DeployDirectoryInfoEditorWidget : Gtk.Box, IPropertyEditor {

		Gtk.ComboBoxText combo;
		DeployDirectoryInfo[] values;

		public DeployDirectoryInfoEditorWidget () : base (Gtk.Orientation.Horizontal, 0)
		{
		}
		
		public void Initialize (EditSession session)
		{
			values = DeployService.GetDeployDirectoryInfo ();

			combo = new Gtk.ComboBoxText ();
			combo.Hexpand = true;
			combo.Changed += combo_Changed;
			Append (combo);

			foreach (DeployDirectoryInfo value in values) {
				combo.AppendText (value.Description);
			}
		}

		public object Value {
			get {
				if (combo.Active != -1)
					return values [combo.Active];
				else
					return null;
			}
			set {
				if (value == null)
					combo.Active = -1;
				else {
					int i = Array.IndexOf (values, value);
					if (i != -1)
						combo.Active = i;
					else
						combo.Active = -1;
				}
			}
		}

		public event EventHandler ValueChanged;

		void combo_Changed (object o, EventArgs args)
		{
			if (ValueChanged != null)
				ValueChanged (this, EventArgs.Empty);
		}
	}
}
