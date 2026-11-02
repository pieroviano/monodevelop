// 
// InstrumentationViewerDialog.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Collections.Generic;
using MonoDevelop.Core.Instrumentation;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Components.Chart;
using Gtk;
using System.Globalization;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;

namespace Mono.Instrumentation.Monitor
{
	public partial class InstrumentationViewerDialog : Gtk.Window
	{
		TreeStore store;
		List<ChartView> views = new List<ChartView> ();
		
		TreeIter iterViews;
//		TreeIter iterStart;
		TreeIter iterTimers;
		TreeIter iterTimerStats;
		Dictionary<ChartView,InstrumentationChartView> chartWidgets = new Dictionary<ChartView, InstrumentationChartView> ();
		InstrumentationChartView timersWidget;
		
		public InstrumentationViewerDialog ()
		{
			Build ();
			
			store = new TreeStore (typeof(string), typeof(ChartView));
			treeCounters.Model = store;
			treeCounters.AppendColumn ("", new CellRendererText (), "text", 0);
			
			//iterStart = store.AppendValues ("Start");
			iterTimers = store.AppendValues ("Timers Events");
			iterTimerStats = store.AppendValues ("Timer Statistics");
			iterViews = store.AppendValues ("Views");
			LoadViews ();
			
			treeCounters.ExpandAll ();
			treeCounters.Selection.Changed += HandleTreeCountersSelectionChanged;
			Present ();
		}

		void LoadViews ()
		{
			try {
				XmlDataSerializer ser = new XmlDataSerializer (new DataContext ());
				FilePath file = ConfigFile;
				if (System.IO.File.Exists (file)) {
					views = (List<ChartView>) ser.Deserialize (file, typeof (List<ChartView>));
					UpdateViews ();
					return;
				}
			} catch (Exception ex) {
				LoggingService.LogError ("Error while loading monitor-views.xml", ex);
			}
			views = new List<ChartView> ();
			ChartView v = new ChartView ();
			v.Name = "Default";
			views.Add (v);
			UpdateViews ();
		}

		static string ConfigFile {
			get { return UserProfile.Current.ConfigDir.Combine ("monitor-views.xml"); }
		}
		
		public void SaveViews ()
		{
			try {
				XmlDataSerializer ser = new XmlDataSerializer (new DataContext ());
				ser.Serialize (ConfigFile, views);
			} catch (Exception ex) {
				LoggingService.LogError ("Error while saving monitor-views.xml", ex);
			}
		}
		
		void UpdateViews ()
		{
			TreeIter it;
			while (store.IterChildren (out it, iterViews))
				store.Remove (ref it);
			
			foreach (ChartView view in views) {
				store.AppendValues (iterViews, view.Name, view);
			}
			hpaned.Sensitive = (App.Service != null);
		}
		
		internal void NewView (ChartView source)
		{
			string name;
			do {
				NewProfile dlg = new NewProfile ();
				dlg.TransientFor = this;
				if (dlg.RunModal () == (int) Gtk.ResponseType.Cancel) {
					dlg.Destroy ();
					return;
				}
				name = dlg.ViewName;
				dlg.Destroy ();
			} while (name == string.Empty);
			
			if (name == null)
				return;
			
			ChartView v = new ChartView ();
			v.CopyFrom (source);
			v.Name = name;
			views.Add (v);
			SaveViews ();
			
			TreeIter it = store.AppendValues (iterViews, name, v);
			treeCounters.Selection.SelectIter (it);
		}
		
		internal void DeleteView (ChartView view)
		{
			string msg = string.Format ("Are you sure you want to delete the profile '{0}'?", view.Name);
			MessageDialog dlg = new MessageDialog (this, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel, msg);
			if (dlg.RunModal () == (int) ResponseType.Ok) {
				views.Remove (view);
				SaveViews ();
				UpdateViews ();
			}
			dlg.Destroy ();
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			Destroy ();
		}

		protected virtual void OnCloseRequest (object o, Gtk.CloseRequestArgs args)
		{
			Gtk.Application.Quit ();
			args.RetVal = false;
		}

		public override void Destroy ()
		{
			// Views are cached and may not be parented right now: stop their timers explicitly.
			if (timersWidget != null)
				timersWidget.Destroy ();
			foreach (var w in chartWidgets.Values)
				w.Destroy ();
			base.Destroy ();
		}

		TimeStatisticsView timerStatsView;
		void HandleTreeCountersSelectionChanged (object sender, EventArgs e)
		{
			TreeIter it;
			if (!treeCounters.Selection.GetSelected (out it)) {
				SetView (null, null, false);
				return;
			}
			
			if (store.GetPath (it).Equals (store.GetPath (iterTimers))) {
				if (timersWidget == null) {
					timersWidget = new InstrumentationChartView (this);
					timersWidget.ShowAllTimers ();
				}
				SetView (timersWidget, "Timers", false);
			}
			else if (store.GetPath (it).Equals (store.GetPath (iterTimerStats))) {
				if (timerStatsView == null)
					timerStatsView = new TimeStatisticsView (this);
				timerStatsView.Fill ();
				SetView (timerStatsView, "Timer Statistics", false);
			}
			else {
				ChartView v = (ChartView) store.GetValue (it, 1);
				if (v != null) {
					InstrumentationChartView cv;
					if (!chartWidgets.TryGetValue (v, out cv)) {
						cv = new InstrumentationChartView (this);
						chartWidgets [v] = cv;
						cv.SetView (v);
					}
					SetView (cv, v.Name, true);
				} else {
					SetView (null, null, false);
				}
			}
		}
		
		void SetView (Widget w, string title, bool showSaveButtons)
		{
			viewBox.Child = w;
			
			if (title != null) {
				labelHeader.Markup = "<big><b>" + GLib.Markup.EscapeText (title) + "</b></big>";
				headerBox.Visible = true;
				buttonsBox.Visible = showSaveButtons;
			} else {
				headerBox.Visible = false;
				buttonsBox.Visible = false;
			}
		}
		
		public void EnableSave (bool enable)
		{
			buttonSave.Sensitive = enable;
		}
		
		protected virtual void OnButtonFlushClicked (object sender, System.EventArgs e)
		{
			GC.Collect ();
			for (int n=0; n<500; n++) {
				byte[] mem = new byte [1024 * 1000];
				mem [0] = 0;
			}
		}
		
		protected virtual void OnButtonSaveClicked (object sender, System.EventArgs e)
		{
			((InstrumentationChartView)viewBox.Child).Save ();
		}
		
		protected virtual void OnButtonSaveAsClicked (object sender, System.EventArgs e)
		{
			((InstrumentationChartView)viewBox.Child).SaveAs ();
		}
		
		protected virtual void OnButtonDeleteClicked (object sender, System.EventArgs e)
		{
			((InstrumentationChartView)viewBox.Child).Delete ();
		}
		
		protected virtual void OnFlushMemoryActionActivated (object sender, System.EventArgs e)
		{
			GC.Collect ();
			for (int n=0; n<500; n++) {
				byte[] mem = new byte [1024 * 1000];
				mem [0] = 0;
			}
		}
		
		protected virtual void OnExitActionActivated (object sender, System.EventArgs e)
		{
			Gtk.Application.Quit ();
		}
		
		protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
		{
			FileChooserDialog fdiag  = new FileChooserDialog ("Open Data File", this, Gtk.FileChooserAction.Open);
			fdiag.AddButton ("_Cancel", (int) ResponseType.Cancel);
			fdiag.AddButton ("_Open", (int) ResponseType.Ok);
			fdiag.SelectMultiple = false;
			
			try {
				if (fdiag.RunModal () == (int) Gtk.ResponseType.Ok && fdiag.File?.Path != null) {
					treeCounters.Selection.UnselectAll ();
					App.LoadServiceData (fdiag.File.Path);
					UpdateViews ();
				}
			} finally {
				fdiag.Destroy ();
			}
		}
		
	}
	
	internal static class CounterColor
	{
		static Dictionary<Counter,Gdk.RGBA> colors = new Dictionary<Counter, Gdk.RGBA> ();
		static Dictionary<Counter,Gdk.Pixbuf> icons = new Dictionary<Counter, Gdk.Pixbuf> ();

		static Gdk.RGBA Rgb (int r, int g, int b)
		{
			return new Gdk.RGBA { Red = r / 255f, Green = g / 255f, Blue = b / 255f, Alpha = 1f };
		}

		public static Gdk.RGBA GetColor (this Counter c)
		{
			Gdk.RGBA cachedColor;
			if (colors.TryGetValue (c, out cachedColor))
				return cachedColor;

			Random r = new Random (c.Name.GetHashCode ());
			int nc = c.Name.GetHashCode ();
			if (nc < 0) nc = -nc;
			Xwt.Drawing.Color col = Xwt.Drawing.Color.FromHsl (r.NextDouble (), r.NextDouble (), 0.3 + (r.NextDouble () * 0.3));
			Gdk.RGBA gc = col.ToGdkColor ();
			colors [c] = gc;
			return gc;
		}

		/// <summary>CSS color of the counter, for cell renderer "foreground" attributes.</summary>
		public static string ToCss (this Gdk.RGBA color)
		{
			return color.GetHex ();
		}

		public static Gdk.Pixbuf GetIcon (this Counter c)
		{
			Gdk.Pixbuf cachedIcon;
			if (icons.TryGetValue (c, out cachedIcon))
				return cachedIcon;

			Gdk.RGBA gcolor = c.GetColor ();
			uint color = (((uint)(gcolor.Red * 255)) << 24) | (((uint)(gcolor.Green * 255)) << 16) | (((uint)(gcolor.Blue * 255)) << 8) | 0xff;
			cachedIcon = new Gdk.Pixbuf (Gdk.Colorspace.Rgb, true, 8, 16, 16);
			cachedIcon.Fill (color);
			icons [c] = cachedIcon;
			return cachedIcon;
		}

		public static Gdk.RGBA GetTimeColor (this CounterValue val, TimerCounter c)
		{
			long m = c.AverageTime.Ticks;
			long v = val.Duration.Ticks;
			if (v >= m*3)
				return Rgb (255, 0, 0);
			if (v >= m*2)
				return Rgb (150, 0, 0);
			if (v >= (long)((double)m) * 1.5d)
				return Rgb (100, 0, 0);
			if (v >= (long)((double)m) * 1.1d)
				return Rgb (50, 0, 0);
			if (v <= m/3)
				return Rgb (0, 255, 0);
			if (v <= m/2)
				return Rgb (0, 150, 0);
			if (v <= (long)((double)m) * 0.75d)
				return Rgb (0, 100, 0);
			if (v <= (long)((double)m) * 0.90d)
				return Rgb (0, 50, 0);
			return Rgb (0, 0, 0);
		}
	}
}
