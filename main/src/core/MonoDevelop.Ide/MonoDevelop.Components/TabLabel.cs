//
// Authors: John Luke  <jluke@cfl.rr.com>
//          Jacob Ilsø Christensen <jacobilsoe@gmail.com>
// License: LGPL
//

using System;

namespace MonoDevelop.Components
{
	public class TabLabel : Gtk.Box
	{
		private Gtk.Label title;
		private ImageView icon;
		private Gtk.Widget titleBox;
		private static Xwt.Drawing.Image closeImage;

		static TabLabel ()
		{
			try {
				closeImage = Xwt.Drawing.Image.FromResource ("popup-close-16.png");
			} catch (Exception e) {
				MonoDevelop.Core.LoggingService.LogError ("Can't create pixbuf from resource: popup-close-16.png", e);
			}
		}

		protected TabLabel (IntPtr p): base (p)
		{
		}

		public TabLabel (Gtk.Label label, ImageView icon) : base (Gtk.Orientation.Horizontal, 0)
		{
			this.title = label;
			this.icon = icon;
			icon.MarginStart = icon.MarginEnd = 2;
			Append (icon);

			titleBox = title;
			title.Hexpand = true;
			Append (title);

			var button = new Gtk.Button ();
			button.HasFrame = false;
			button.AddCssClass ("flat");
			if (closeImage != null)
				button.Child = new Xwt.ImageView (closeImage).ToGtkWidget ();
			button.Clicked += ButtonClicked;
			button.Name = "MonoDevelop.TabLabel.CloseButton";
			button.FocusOnClick = false;
			Append (button);

			this.AddClickHandler (null, OnReleased, 2);
		}

		public Gtk.Label Label
		{
			get { return title; }
			set { title = value; }
		}

		public ImageView Icon
		{
			get { return icon; }
			set { icon = value; }
		}

		public event EventHandler CloseClicked;

		public void SetTooltip (string tip, string desc)
		{
			titleBox.TooltipText = tip;
		}

		void OnReleased (ClickInfo click)
		{
			if (CloseClicked != null) {
				click.Claim ();
				CloseClicked (this, null);
			}
		}

		private void ButtonClicked (object o, EventArgs eventArgs)
		{
			if (CloseClicked != null)
			{
				CloseClicked(this, null);
			}
		}
	}
}
