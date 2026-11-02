// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace Mono.Instrumentation.Monitor
{
	internal partial class TimeLineViewWindow
	{
		private global::Gtk.Box vbox2;
		private global::Gtk.Box hbox1;
		private global::Gtk.Button buttonExpand;
		private global::Gtk.Button buttonCollapse;
		private global::Gtk.CheckButton checkSingleThread;
		private global::Gtk.Box hbox2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Mono.Instrumentation.Monitor.TimeLineView timeView;
		private global::Gtk.Box vbox1;
		private global::Gtk.Button button3;
		private global::Gtk.Scale vscaleZoom;
		private global::Gtk.Label label2;
		private global::Gtk.Button buttonResetScale;
		private global::Gtk.Scale vscaleScale;

		static global::Gtk.Scale CreateVScale ()
		{
			var adjustment = new global::Gtk.Adjustment (100, 10, 300, 1, 10, 0);
			var scale = new global::Gtk.Scale (global::Gtk.Orientation.Vertical, adjustment);
			scale.HeightRequest = 150;
			scale.CanFocus = true;
			scale.DrawValue = true;
			scale.Digits = 0;
			return scale;
		}

		protected virtual void Build ()
		{
			this.Title = global::Mono.Unix.Catalog.GetString ("Time Line");
			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 6);

			this.hbox1 = DesignerLayout.HBox (6);
			this.buttonExpand = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Expand All"));
			this.buttonExpand.CanFocus = true;
			DesignerLayout.Pack (this.hbox1, this.buttonExpand);
			this.buttonCollapse = DesignerLayout.Button (global::Mono.Unix.Catalog.GetString ("Collapse All"));
			this.buttonCollapse.CanFocus = true;
			DesignerLayout.Pack (this.hbox1, this.buttonCollapse);
			this.checkSingleThread = new global::Gtk.CheckButton ();
			this.checkSingleThread.CanFocus = true;
			this.checkSingleThread.Label = global::Mono.Unix.Catalog.GetString ("Single Thread");
			this.checkSingleThread.UseUnderline = true;
			DesignerLayout.Pack (this.hbox1, this.checkSingleThread);
			DesignerLayout.Pack (this.vbox2, this.hbox1);

			this.hbox2 = DesignerLayout.HBox (6);
			this.timeView = new global::Mono.Instrumentation.Monitor.TimeLineView ();
			this.timeView.SingleThread = false;
			this.timeView.TimeScale = 0;
			this.timeView.Zoom = 0;
			this.timeView.Scale = 0;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.timeView);
			DesignerLayout.Pack (this.hbox2, this.GtkScrolledWindow, true);

			this.vbox1 = DesignerLayout.VBox (6);
			this.button3 = DesignerLayout.IconButton ("zoom-original-symbolic", null, false);
			this.button3.CanFocus = true;
			DesignerLayout.Pack (this.vbox1, this.button3);
			this.vscaleZoom = CreateVScale ();
			DesignerLayout.Pack (this.vbox1, this.vscaleZoom);
			this.label2 = new global::Gtk.Label ((string)null);
			DesignerLayout.Pack (this.vbox1, this.label2);
			this.buttonResetScale = DesignerLayout.IconButton ("object-flip-vertical-symbolic", null, false);
			this.buttonResetScale.CanFocus = true;
			DesignerLayout.Pack (this.vbox1, this.buttonResetScale);
			this.vscaleScale = CreateVScale ();
			DesignerLayout.Pack (this.vbox1, this.vscaleScale);
			DesignerLayout.Pack (this.hbox2, this.vbox1);
			DesignerLayout.Pack (this.vbox2, this.hbox2, true);

			this.Child = this.vbox2;
			this.DefaultWidth = 806;
			this.DefaultHeight = 638;
			this.buttonExpand.Clicked += new global::System.EventHandler (this.OnButtonExpandClicked);
			this.buttonCollapse.Clicked += new global::System.EventHandler (this.OnButtonCollapseClicked);
			this.checkSingleThread.Toggled += new global::System.EventHandler (this.OnCheckSingleThreadToggled);
			this.buttonResetScale.Clicked += new global::System.EventHandler (this.OnButtonResetScaleClicked);
			this.vscaleScale.ChangeValue += new global::Gtk.ChangeValueHandler (this.OnVscaleScaleChangeValue);
		}
	}
}
