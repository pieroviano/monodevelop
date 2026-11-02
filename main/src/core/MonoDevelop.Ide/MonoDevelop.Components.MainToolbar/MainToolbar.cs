// 
// MainToolbar.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
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
#if !MAC

using System;
using Gtk;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Core;
using System.Linq;
using MonoDevelop.Components;
using Cairo;
using MonoDevelop.Projects;
using System.Collections.Generic;
using Mono.Addins;
using MonoDevelop.Components.Commands.ExtensionNodes;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide.TypeSystem;
using System.Threading;
using MonoDevelop.Ide.Editor;
using System.Text;
using MonoDevelop.Components.AtkCocoaHelper;
using System.Diagnostics;

namespace MonoDevelop.Components.MainToolbar
{
	class MainToolbar: BinWidget, IMainToolbarView
	{
		Box contentBox = new Box (Orientation.Horizontal, 0);

		Box configurationCombosBox;

		ComboBox configurationCombo;
		TreeStore configurationStore = new TreeStore (typeof(string), typeof(IConfigurationModel));

		ComboBox runConfigurationCombo;
		TreeStore runConfigurationStore = new TreeStore (typeof (string), typeof (IRunConfigurationModel));
	
		ComboBox runtimeCombo;
		TreeStore runtimeStore = new TreeStore (typeof(IRuntimeModel));

		StatusArea statusArea;

		SearchEntry matchEntry;
		static WeakReference lastCommandTarget;

		ButtonBar buttonBar = new ButtonBar ();
		RoundButton button = new RoundButton ();
		Box buttonBarBox;

		// attributes for the runtime combo (bold / normal)
		readonly Pango.AttrList boldAttributes = new Pango.AttrList ();
		readonly Pango.AttrList normalAttributes = new Pango.AttrList ();

		public Cairo.ImageSurface Background {
			get;
			set;
		}

		public int TitleBarHeight {
			get;
			set;
		}

		public MonoDevelop.Ide.StatusBar StatusBar {
			get {
				return statusArea;
			}
		}

		static bool RuntimeIsSeparator (ITreeModel model, TreeIter iter)
		{
			var runtime = (IRuntimeModel)model.GetValue (iter, 0);
			return runtime == null || runtime.IsSeparator;
		}

		void RuntimeRenderCell (ICellLayout layout, CellRenderer cell, ITreeModel model, TreeIter iter)
		{
			var runtime = (IRuntimeModel)model.GetValue (iter, 0);
			var renderer = (CellRendererText) cell;

			if (runtime == null || runtime.IsSeparator) {
				renderer.Xpad = (uint)0;
				return;
			}

			using (var mutableModel = runtime.GetMutableModel ()) {
				renderer.Visible = mutableModel.Visible;
				renderer.Sensitive = mutableModel.Enabled;
				renderer.Xpad = (uint)(runtime.IsIndented ? 18 : 3);

				if (!runtimeCombo.PopupShown) {
					// no need to ident text when the combo dropdown is not showing
					if (Platform.IsWindows)
						renderer.Xpad = 3;
					renderer.Text = mutableModel.FullDisplayString;
					renderer.Attributes = normalAttributes;
				} else {
					renderer.Text = mutableModel.DisplayString;
					renderer.Attributes = runtime.Notable ? boldAttributes : normalAttributes;
				}

			}
		}

		TreeIter lastSelection = TreeIter.Zero;
		public MainToolbar ()
		{
			AddWidget (button);
			AddSpace (8);

			configurationCombosBox = new Box (Orientation.Horizontal, 8);

			var ctx = new Gtk.CellRendererText ();

			runConfigurationCombo = new Gtk.ComboBox ();
			runConfigurationCombo.Model = runConfigurationStore;
			runConfigurationCombo.PackStart (ctx, true);
			runConfigurationCombo.AddAttribute (ctx, "text", 0);
			runConfigurationCombo.TooltipText = GettextCatalog.GetString ("A project or named set of projects and execution options that should be launched when running or debugging the solution.");

			runConfigurationCombo.Valign = Align.Center;
			configurationCombosBox.Append (runConfigurationCombo);
		
			configurationCombo = new Gtk.ComboBox ();
			configurationCombo.Model = configurationStore;
			configurationCombo.PackStart (ctx, true);
			configurationCombo.AddAttribute (ctx, "text", 0);
			configurationCombo.TooltipText = GettextCatalog.GetString ("A named set of projects and their configurations to be built when building the solution.");

			configurationCombo.Valign = Align.Center;
			configurationCombosBox.Append (configurationCombo);

			// bold attributes for running runtime targets / (emulators)
			boldAttributes.Insert (new Pango.AttrWeight (Pango.Weight.Bold));

			runtimeCombo = new Gtk.ComboBox ();
			runtimeCombo.Model = runtimeStore;
			ctx = new Gtk.CellRendererText ();
			if (Platform.IsWindows)
				ctx.Ellipsize = Pango.EllipsizeMode.Middle;
			runtimeCombo.PackStart (ctx, true);
			runtimeCombo.SetCellDataFunc (ctx, RuntimeRenderCell);
			runtimeCombo.RowSeparatorFunc = RuntimeIsSeparator;
			runtimeCombo.TooltipText = GettextCatalog.GetString ("The device on which to deploy and launch the projects when running or debugging.");

			runtimeCombo.Valign = Align.Center;
			configurationCombosBox.Append (runtimeCombo);
			AddWidget (configurationCombosBox);

			buttonBarBox = new Box (Orientation.Horizontal, 0);
			buttonBarBox.Halign = Align.Center;
			buttonBarBox.Valign = Align.Center;
			buttonBarBox.MarginStart = 7;
			buttonBarBox.Append (buttonBar);
			buttonBarBox.Visible = false;
			AddWidget (buttonBarBox);
			AddSpace (24);

			statusArea = new StatusArea ();
			statusArea.ShowMessage (BrandingService.ApplicationName);

			var statusAreaAlign = new StatusAreaContainer ();
			statusAreaAlign.Child = statusArea;
			statusAreaAlign.Hexpand = true;
			contentBox.Append (statusAreaAlign);
			AddSpace (24);

			matchEntry = new SearchEntry ();

			matchEntry.ForceFilterButtonVisible = true;
			matchEntry.Entry.AddFocusHandler (null, () => {
				if (SearchEntryLostFocus != null)
					SearchEntryLostFocus (matchEntry.Entry, EventArgs.Empty);
			});

			matchEntry.Ready = true;
			matchEntry.Visible = true;
			matchEntry.IsCheckMenu = true;
			matchEntry.WidthRequest = 240;
			if (!Platform.IsMac && !Platform.IsWindows)
				ApplySearchEntryFont (matchEntry.Entry); // TODO: VV: "Segoe UI 9"
			matchEntry.RoundedShape = true;
			matchEntry.Entry.Changed += HandleSearchEntryChanged;
			matchEntry.Activated += HandleSearchEntryActivated;
			matchEntry.Entry.AddKeyPressHandler (HandleSearchEntryKeyPressed, null, PropagationPhase.Capture);

			contentBox.Append (matchEntry);

			Padding = new Border { Top = 5, Left = 9, Right = 18, Bottom = 10 };
			Child = contentBox;
			SetDefaultSizes (-1, 21);

			// Dragging the empty area of the toolbar moves the window (children that handle the click claim it)
			this.AddClickHandler (HandleToolbarPressed, null, 1, PropagationPhase.Bubble);

			configurationCombo.Changed += (o, e) => {
				if (ConfigurationChanged != null)
					ConfigurationChanged (o, e);
			};
			runConfigurationCombo.Changed += (o, e) => {
				if (RunConfigurationChanged != null)
					RunConfigurationChanged (o, e);
			};
			runtimeCombo.Changed += (o, e) => {
				var ea = new HandledEventArgs ();
				if (RuntimeChanged != null)
					RuntimeChanged (o, ea);

				TreeIter it;
				if (runtimeCombo.GetActiveIter (out it)) {
					if (ea.Handled) {
						runtimeCombo.SetActiveIter (lastSelection);
						return;
					}
					lastSelection = it;
				}
			};

			button.Clicked += HandleStartButtonClicked;

			this.statusArea.statusIconBox.Visible = false;
		}

		static Gtk.CssProvider searchEntryFontProvider;

		static void ApplySearchEntryFont (Gtk.Widget entry)
		{
			if (searchEntryFontProvider == null) {
				searchEntryFontProvider = new Gtk.CssProvider ();
				searchEntryFontProvider.LoadFromString ("entry.md-toolbar-search { font-family: Sans; font-size: 9pt; }");
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.AddProviderForDisplay (display, searchEntryFontProvider, StyleProviderPriority.Application);
			}
			entry.AddCssClass ("md-toolbar-search");
		}

		void HandleToolbarPressed (ClickInfo click)
		{
			if (click.Button != 1 || !(Root is Gtk.Window window) || window.Native?.Surface == null)
				return;
			// Only the toolbar background starts a window drag, not the widgets on top of it
			var picked = Pick (click.X, click.Y, PickFlags.Default);
			if (picked != this && picked != contentBox)
				return;
			if (!IdeServices.DesktopService.GetIsFullscreen (window)) {
				TranslateCoordinates (window, click.X, click.Y, out double rootX, out double rootY);
				var toplevel = Gdk.ToplevelAdapter.GetObject (window.Native.Surface);
				toplevel.BeginMove (click.Gesture.CurrentEventDevice, (int)click.Button, rootX, rootY, click.Time);
				click.Claim ();
			}
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);
			SearchEntryResized?.Invoke (this, EventArgs.Empty);
		}

		/// <summary>Hosts the status area, keeping it centered in the window with a maximum width of 600.</summary>
		class StatusAreaContainer : BinWidget
		{
			protected override void OnSizeAllocate (int width, int height, int baseline)
			{
				var child = Child;
				if (child == null)
					return;
				int left = 0, right = width;
				if (Root is Gtk.Widget toplevel && TranslateCoordinates (toplevel, 0, 0, out double x, out double y)) {
					int center = toplevel.Width / 2;
					left = Math.Max ((int)(center - 300 - x), 0);
					right = Math.Min (left + 600, width);
				}
				AllocateChild (child, new Gdk.Rectangle (left, 0, Math.Max (0, right - left), height));
			}
		}

		void SetDefaultSizes (int comboHeight, int height)
		{
			configurationCombo.SetSizeRequest (150, comboHeight);
			runConfigurationCombo.SetSizeRequest (150, comboHeight);
			// make the windows runtime slightly wider to accomodate select devices text
			runtimeCombo.SetSizeRequest (Platform.IsWindows ? 175 : 150, comboHeight);
			statusArea.SetSizeRequest (32, 32);
			matchEntry.HeightRequest = height + 4;
			buttonBar.HeightRequest = height + 2;
		}

		void AddSpace (int w)
		{
			Label la = new Label ("");
			la.WidthRequest = w;
			contentBox.Append (la);
		}

		void HandleSearchEntryChanged (object sender, EventArgs e)
		{
			SearchEntryChanged?.Invoke (sender, e);
		}

		void HandleSearchEntryActivated (object sender, EventArgs e)
		{
			if (SearchEntryActivated != null)
				SearchEntryActivated (sender, e);
		}

		bool HandleSearchEntryKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			if (PerformCommand != null) {
				bool cmdPressed = Platform.IsMac && (state & Gdk.ModifierType.MetaMask) != 0;
				SearchPopupCommand command;

				switch ((Gdk.Key)keyval) {
				case Gdk.Key.Down:
				case Gdk.Key.downarrow:
					if (cmdPressed) {
						goto case Gdk.Key.Page_Down;
					}

					command = SearchPopupCommand.NextItem;
					break;

				case Gdk.Key.Up:
				case Gdk.Key.uparrow:
					if (cmdPressed) {
						goto case Gdk.Key.Page_Up;
					}

					command = SearchPopupCommand.PreviousItem;
					break;

				case Gdk.Key.KP_Page_Down:
				case Gdk.Key.Page_Down:
					command = SearchPopupCommand.NextCategory;
					break;

				case Gdk.Key.KP_Page_Up:
				case Gdk.Key.Page_Up:
					command = SearchPopupCommand.PreviousCategory;
					break;

				case Gdk.Key.Escape:
					command = SearchPopupCommand.Cancel;
					break;

				case Gdk.Key.Return:
				case Gdk.Key.KP_Enter:
					command = SearchPopupCommand.Activate;
					break;

				default:
					return false;
				}

				var kargs = new SearchEntryCommandArgs (command);
				PerformCommand?.Invoke (matchEntry.Entry, kargs);
				return kargs.Handled;
			}
			return false;
		}

		public void AddWidget (Gtk.Widget widget)
		{
			contentBox.Append (widget);
		}

		protected override void OnSnapshot (Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0) {
				using (var context = snapshot.CreateCairoContext (new Gdk.Rectangle (0, 0, width, height))) {
					context.LineWidth = 1;
					if (Background != null && Background.Width > 0) {
						for (int x = 0; x < width; x += Background.Width) {
							Background.Show (context, x, -TitleBarHeight);
						}
					} else {
						context.Rectangle (0, 0, width, height);
						using (var lg = new LinearGradient (0, 0, 0, height)) {
							lg.AddColorStop (0, Styles.BackgroundColor.WithIncreasedLight (0.05).ToCairoColor ());
							lg.AddColorStop (1, Styles.BackgroundColor.WithIncreasedLight (-0.03).ToCairoColor ());
							context.SetSource (lg);
						}
						context.Fill ();

					}
					context.MoveTo (0, height - 0.5);
					context.RelLineTo (width, 0);
					context.SetSourceColor (Styles.ToolbarBottomBorderColor.ToCairoColor ());
					context.Stroke ();
				}
			}
			base.OnSnapshot (snapshot);
		}

		void HandleStartButtonClicked (object sender, EventArgs e)
		{
			if (RunButtonClicked != null)
				RunButtonClicked (sender, e);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (button != null)
					button.Clicked -= HandleStartButtonClicked;

				if (Background != null) {
					((IDisposable)Background).Dispose ();
					Background = null;
				}
			}
			base.Dispose (disposing);
		}

		#region IMainToolbarView implementation
		public event EventHandler RunButtonClicked;

		public bool RunButtonSensitivity {
			get { return button.Sensitive; }
			set { button.Sensitive = value; }
		}

		public OperationIcon RunButtonIcon {
			set {
				button.Icon = value;
				switch (value) {
				case OperationIcon.Stop:
					button.TooltipText = GettextCatalog.GetString ("Stop the executing solution");
					break;
				case OperationIcon.Run:
					button.TooltipText = GettextCatalog.GetString ("Run the project or projects in the active run configuration. Builds the projects in the active solution build configuration if necessary.");
					break;
				case OperationIcon.Build:
					button.TooltipText = GettextCatalog.GetString ("Build the projects in the active solution build configuration.");
					break;
				default:
					button.TooltipText = string.Empty;
					break;
				}
			}
		}

		public bool ConfigurationPlatformSensitivity {
			get { return configurationCombosBox.Sensitive; }
			set { configurationCombosBox.Sensitive = value; }
		}

		static bool FindIter<T> (TreeStore store, Func<T, bool> match, out TreeIter iter)
		{
			if (store.GetIterFirst (out iter)) {
				do {
					if (match((T)store.GetValue (iter, 1)))
						return true;
				} while (store.IterNext (ref iter));
			}
			return false;
		}

		public IConfigurationModel ActiveConfiguration {
			get {
				TreeIter iter;
				if (!configurationCombo.GetActiveIter (out iter))
					return null;

				return (IConfigurationModel) configurationStore.GetValue (iter, 1);
			}
			set {
				TreeIter iter;
				if (FindIter<IConfigurationModel> (configurationStore, it => value.OriginalId == it.OriginalId, out iter))
					configurationCombo.SetActiveIter (iter);
				else
					configurationCombo.Active = 0;
			}
		}

		public IRunConfigurationModel ActiveRunConfiguration {
			get {
				TreeIter iter;
				if (!runConfigurationCombo.GetActiveIter (out iter))
					return null;

				return (IRunConfigurationModel)runConfigurationStore.GetValue (iter, 1);
			}
			set {
				TreeIter iter;
				if (FindIter<IRunConfigurationModel> (runConfigurationStore, it => value.OriginalId == it.OriginalId, out iter))
					runConfigurationCombo.SetActiveIter (iter);
				else
					runConfigurationCombo.Active = 0;
			}
		}

		public IRuntimeModel ActiveRuntime {
			get {
				TreeIter iter;
				if (!runtimeCombo.GetActiveIter (out iter))
					return null;

				return (IRuntimeModel)runtimeStore.GetValue (iter, 0);
			}
			set {
				TreeIter iter;
				bool found = false;
				if (runtimeStore.GetIterFirst (out iter)) {
					do {
						if (value == runtimeStore.GetValue (iter, 0)) {
							found = true;
							break;
						}
					} while (runtimeStore.IterNext (ref iter));
				}
				if (found)
					runtimeCombo.SetActiveIter (iter);
				else
					runtimeCombo.Active = 0;
			}
		}

		public bool PlatformSensitivity {
			set { runtimeCombo.Sensitive = value; }
		}

		IEnumerable<IConfigurationModel> configurationModel;
		public IEnumerable<IConfigurationModel> ConfigurationModel {
			get { return configurationModel; }
			set {
				configurationModel = value;
				configurationStore.Clear ();
				foreach (var item in value) {
					configurationStore.AppendValues (item.DisplayString, item);
				}
			}
		}

		IEnumerable<IRunConfigurationModel> runConfigurationModel;
		public IEnumerable<IRunConfigurationModel> RunConfigurationModel {
			get { return runConfigurationModel; }
			set {
				runConfigurationModel = value;
				runConfigurationStore.Clear ();
				foreach (var item in value) {
					runConfigurationStore.AppendValues (item.DisplayString, item);
				}
			}
		}

		IEnumerable<IRuntimeModel> runtimeModel;
		public IEnumerable<IRuntimeModel> RuntimeModel {
			get { return runtimeModel; }
			set {
				runtimeModel = value;
				runtimeStore.Clear ();
				foreach (var item in value) {
					var iter = runtimeStore.AppendValues (item);
					foreach (var subitem in item.Children)
						runtimeStore.AppendValues (iter, subitem);
				}
			}
		}

		public bool RunConfigurationVisible {
			get { return runConfigurationCombo.Visible; }
			set { runConfigurationCombo.Visible = value; }
		}

		public bool SearchSensivitity {
			set { matchEntry.Sensitive = value; }
		}

		public IEnumerable<ISearchMenuModel> SearchMenuItems {
			set {
				foreach (var item in value) {
					var menuItem = matchEntry.AddMenuItem (item.DisplayString);
					menuItem.Activated += delegate {
						item.NotifyActivated ();
					};
				}
			}
		}

		public string SearchCategory {
			set {
				matchEntry.Entry.Text = value;
				matchEntry.Entry.GrabFocus ();
				var pos = matchEntry.Entry.Text.Length;
				matchEntry.Entry.SelectRegion (pos, pos);
			}
		}

		public void FocusSearchBar ()
		{
			matchEntry.Entry.GrabFocus ();
		}

		public event EventHandler SearchEntryChanged;
		public event EventHandler SearchEntryActivated;
		public event EventHandler<Xwt.KeyEventArgs> SearchEntryKeyPressed;
		public event EventHandler<SearchEntryCommandArgs> PerformCommand;
		public event EventHandler SearchEntryResized;
		public event EventHandler SearchEntryLostFocus;

		public Widget PopupAnchor {
			get { return matchEntry; }
		}

		public string SearchText {
			get { return matchEntry.Entry.Text; }
			set { matchEntry.Entry.Text = value; }
		}

		public string SearchPlaceholderMessage {
			set { matchEntry.EmptyMessage = value; }
		}

		public void RebuildToolbar (IEnumerable<ButtonBarGroup> groups)
		{
			if (!groups.Any ()) {
				buttonBarBox.Visible = false;
				return;
			}

			buttonBarBox.Visible = true;
			buttonBar.Groups = groups;
		}

		public void Focus (DirectionType direction, Action<DirectionType> exitAction) {}

		public bool ButtonBarSensitivity {
			set { buttonBar.Sensitive = value; }
		}

		public event EventHandler ConfigurationChanged;
		public event EventHandler RunConfigurationChanged;
		public event EventHandler<HandledEventArgs> RuntimeChanged;

		public void ShowAccessibilityAnnouncement (string message)
		{
			this.MakeAccessibilityAnnouncement (message);
		}
		#endregion
	}
}

#endif