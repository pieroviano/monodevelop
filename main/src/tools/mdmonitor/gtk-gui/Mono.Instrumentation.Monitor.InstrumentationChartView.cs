// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
// frameCharts holds chartsBox (the chart widgets, added by the code-behind) above chartScroller.
namespace Mono.Instrumentation.Monitor
{
	internal partial class InstrumentationChartView
	{
		private global::Gtk.Box vbox3;
		private global::Gtk.Paned hpaned2;
		private global::Gtk.Box hbox4;
		private global::Gtk.Box boxCharts;
		private global::Gtk.Box hboxChartBar;
		private global::Gtk.ToggleButton toggleTimeView;
		private global::Gtk.ToggleButton toggleListView;
		private global::Gtk.Button buttonZoomOut;
		private global::Gtk.Button buttonZoomIn;
		private global::Gtk.Box frameCharts;
		private global::Gtk.Box chartsBox;
		private global::Gtk.Scrollbar chartScroller;
		private global::Gtk.Box vbox5;
		private global::Gtk.Box hboxSeriesBar;
		private global::Gtk.Button buttonRemoveCounter;
		private global::Gtk.Button buttonAddCounter;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView listSeries;

		protected virtual void Build ()
		{
			this.vbox3 = DesignerLayout.VBox (6);
			this.hpaned2 = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal) { Vexpand = true };
			this.hpaned2.CanFocus = true;
			this.hbox4 = DesignerLayout.HBox (6);
			this.boxCharts = DesignerLayout.VBox (6);

			this.hboxChartBar = DesignerLayout.HBox (6);
			this.toggleTimeView = new global::Gtk.ToggleButton ();
			this.toggleTimeView.CanFocus = true;
			this.toggleTimeView.UseUnderline = true;
			this.toggleTimeView.HasFrame = false;
			this.toggleTimeView.Label = global::Mono.Unix.Catalog.GetString ("Time View");
			DesignerLayout.Pack (this.hboxChartBar, this.toggleTimeView);
			this.toggleListView = new global::Gtk.ToggleButton ();
			this.toggleListView.CanFocus = true;
			this.toggleListView.UseUnderline = true;
			this.toggleListView.HasFrame = false;
			this.toggleListView.Label = global::Mono.Unix.Catalog.GetString ("List View");
			DesignerLayout.Pack (this.hboxChartBar, this.toggleListView);
			DesignerLayout.Pack (this.hboxChartBar, new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 0), true);
			this.buttonZoomIn = DesignerLayout.IconButton ("zoom-in-symbolic");
			this.buttonZoomIn.CanFocus = true;
			DesignerLayout.Pack (this.hboxChartBar, this.buttonZoomIn);
			this.buttonZoomOut = DesignerLayout.IconButton ("zoom-out-symbolic");
			this.buttonZoomOut.CanFocus = true;
			DesignerLayout.Pack (this.hboxChartBar, this.buttonZoomOut);
			DesignerLayout.Pack (this.boxCharts, this.hboxChartBar);

			this.frameCharts = DesignerLayout.VBox (6);
			this.chartsBox = DesignerLayout.VBox (6);
			DesignerLayout.Pack (this.frameCharts, this.chartsBox, true);
			this.chartScroller = new global::Gtk.Scrollbar (global::Gtk.Orientation.Horizontal, new global::Gtk.Adjustment (0, 0, 100, 1, 10, 10));
			DesignerLayout.Pack (this.frameCharts, this.chartScroller);
			DesignerLayout.Pack (this.boxCharts, this.frameCharts, true);
			this.boxCharts.Hexpand = true;
			DesignerLayout.Pack (this.hbox4, this.boxCharts, true);
			this.hpaned2.StartChild = this.hbox4;
			this.hpaned2.ResizeStartChild = true;

			this.vbox5 = DesignerLayout.VBox (6);
			this.hboxSeriesBar = DesignerLayout.HBox (6);
			DesignerLayout.Pack (this.hboxSeriesBar, new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 0), true);
			this.buttonAddCounter = DesignerLayout.IconButton ("list-add-symbolic", global::Mono.Unix.Catalog.GetString ("_Add"));
			this.buttonAddCounter.CanFocus = true;
			DesignerLayout.Pack (this.hboxSeriesBar, this.buttonAddCounter);
			this.buttonRemoveCounter = DesignerLayout.IconButton ("list-remove-symbolic", global::Mono.Unix.Catalog.GetString ("_Remove"));
			this.buttonRemoveCounter.CanFocus = true;
			DesignerLayout.Pack (this.hboxSeriesBar, this.buttonRemoveCounter);
			DesignerLayout.Pack (this.vbox5, this.hboxSeriesBar);
			this.listSeries = new global::Gtk.TreeView ();
			this.listSeries.WidthRequest = 100;
			this.listSeries.CanFocus = true;
			this.listSeries.HeadersVisible = false;
			this.GtkScrolledWindow1 = DesignerLayout.Scrolled (this.listSeries);
			DesignerLayout.Pack (this.vbox5, this.GtkScrolledWindow1, true);
			this.hpaned2.EndChild = this.vbox5;
			this.hpaned2.ResizeEndChild = false;
			this.hpaned2.ShrinkEndChild = false;

			DesignerLayout.Pack (this.vbox3, this.hpaned2, true);
			this.Child = this.vbox3;

			this.toggleTimeView.Toggled += new global::System.EventHandler (this.OnToggleTimeViewToggled);
			this.toggleListView.Toggled += new global::System.EventHandler (this.OnToggleListViewToggled);
			this.buttonZoomIn.Clicked += new global::System.EventHandler (this.OnButtonZoomInClicked);
			this.buttonZoomOut.Clicked += new global::System.EventHandler (this.OnButtonZoomOutClicked);
			this.chartScroller.Adjustment.ValueChanged += new global::System.EventHandler (this.OnChartScrollerValueChanged);
			this.buttonAddCounter.Clicked += new global::System.EventHandler (this.OnButtonAddCounterClicked);
			this.buttonRemoveCounter.Clicked += new global::System.EventHandler (this.OnButtonRemoveCounterClicked);
		}
	}
}
