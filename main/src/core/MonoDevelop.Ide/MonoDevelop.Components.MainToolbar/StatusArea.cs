//
// StatusArea.cs
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
using System;
using System.Diagnostics;
using Gtk;
using MonoDevelop.Components;
using Cairo;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Tasks;
using System.Collections.Generic;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Components;
using StockIcons = MonoDevelop.Ide.Gui.Stock;
using Xwt.Motion;
using MonoDevelop.Ide.Fonts;
using System.Threading;
using System.Linq;

namespace MonoDevelop.Components.MainToolbar
{
	class StatusArea : BinWidget, ITestableStatusBar, Xwt.Motion.IAnimatable
	{
		struct Message
		{
			public string Text;
			public IconId Icon;
			public bool IsMarkup;

			public Message (IconId icon, string text, bool markup)
			{
				Text = text;
				Icon = icon;
				IsMarkup = markup;
			}
		}

		public struct RenderArg
		{
			public Gdk.Rectangle Allocation { get; set; }
			public double        BuildAnimationProgress { get; set; }
			public double        BuildAnimationOpacity { get; set; }
			public Gdk.Rectangle ChildAllocation { get; set; }
			public Xwt.Drawing.Image CurrentPixbuf { get; set; }
			public string        CurrentText { get; set; }
			public bool          CurrentTextIsMarkup { get; set; }
			public double        ErrorAnimationProgress { get; set; }
			public double        HoverProgress { get; set; }
			public string        LastText { get; set; }
			public bool          LastTextIsMarkup { get; set; }
			public Xwt.Drawing.Image LastPixbuf { get; set; }
			public Gdk.Point     MousePosition { get; set; }
			public Pango.Context Pango { get; set; }
			public double        ProgressBarAlpha { get; set; }
			public float         ProgressBarFraction { get; set; }
			public bool          ShowProgressBar { get; set; }
			public double        TextAnimationProgress { get; set; }
		}

		StatusAreaTheme theme;
		RenderArg renderArg;

		Box contentBox = new Box (Orientation.Horizontal, 8);

		StatusAreaSeparator statusIconSeparator;
		Gtk.Widget buildResultWidget;

		readonly Box messageBox = new Box (Orientation.Horizontal, 0);
		internal readonly Box statusIconBox = new Box (Orientation.Horizontal, 3);

		// Area of the widget where the status bar is drawn (limited by MaxWidth)
		Gdk.Rectangle drawArea;
		Action buildResultsCleanup;

		uint animPauseHandle;

		MouseTracker tracker;

		AnimatedIcon iconAnimation;
		IconId currentIcon;
		static Pad sourcePad;
		IDisposable currentIconAnimation;

		bool errorAnimPending;

		StatusBarContextHandler ctxHandler;
		bool progressBarVisible;

		string currentApplicationName = String.Empty;

		Queue<Message> messageQueue;

		public StatusBar MainContext {
			get { return ctxHandler.MainContext; }
		}

		public int MaxWidth { get; set; }

		void messageBoxToolTip (object o, QueryTooltipArgs e)
		{
			if (theme.IsEllipsized && (e.X < messageBox.Width)) {
				var label = new Label ();
				if (renderArg.CurrentTextIsMarkup) {
					label.Markup = renderArg.CurrentText;
				} else {
					label.Text = renderArg.CurrentText;
				}

				label.Wrap = true;
				label.WidthRequest = messageBox.Width;

				e.Tooltip.Custom = label;
				e.RetVal = true;
			} else {
				e.RetVal = false;
			}
		}

		public StatusArea ()
		{
			theme = new StatusAreaTheme ();
			renderArg = new RenderArg ();

			ctxHandler = new StatusBarContextHandler (this);
			Padding = new Border { Left = 12, Right = 8 };

			Action<bool> animateProgressBar =
				showing => this.Animate ("ProgressBarFade",
				                         val => renderArg.ProgressBarAlpha = val,
				                         renderArg.ProgressBarAlpha,
				                         showing ? 1.0f : 0.0f,
				                         easing: Easing.CubicInOut);

			ProgressBegin += delegate {
				renderArg.ShowProgressBar = true;
//				StartBuildAnimation ();
				renderArg.ProgressBarFraction = 0;
				QueueDraw ();
				animateProgressBar (true);
			};

			ProgressEnd += delegate {
				renderArg.ShowProgressBar = false;
//				StopBuildAnimation ();
				QueueDraw ();
				animateProgressBar (false);
			};

			ProgressFraction += delegate(object sender, FractionEventArgs e) {
				renderArg.ProgressBarFraction = (float)e.Work;
				QueueDraw ();
			};

			messageBox.Hexpand = true;
			contentBox.Append (messageBox);
			contentBox.Append (buildResultWidget = CreateBuildResultsWidget (Orientation.Horizontal));
			contentBox.Append (statusIconSeparator = new StatusAreaSeparator ());
			contentBox.Append (statusIconBox);

			HasTooltip = true;
			QueryTooltip += messageBoxToolTip;

			contentBox.Valign = Align.Center;
			Child = contentBox;

			statusIconBox.Visible = false;
			statusIconSeparator.Visible = false;
			buildResultWidget.Visible = false;

			this.AddClickHandler (click => {
				if (sourcePad != null)
					sourcePad.BringToFront (true);
			});

			statusIconBox.Shown += delegate {
				UpdateSeparators ();
			};

			statusIconBox.Hidden += delegate {
				UpdateSeparators ();
			};

			messageQueue = new Queue<Message> ();

			tracker = new MouseTracker(this);
			tracker.MouseMoved += (sender, e) => QueueDraw ();
			tracker.HoveredChanged += (sender, e) => {
				this.Animate ("Hovered",
				              x => renderArg.HoverProgress = x,
				              renderArg.HoverProgress,
				              tracker.Hovered ? 1.0f : 0.0f,
				              easing: Easing.SinInOut);
			};

			IdeApp.FocusIn += delegate {
				// If there was an error while the application didn't have the focus,
				// trigger the error animation again when it gains the focus
				if (errorAnimPending) {
					errorAnimPending = false;
					TriggerErrorAnimation ();
				}
			};
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				buildResultsCleanup?.Invoke ();
				buildResultsCleanup = null;
				if (theme != null) {
					theme.Dispose ();
					theme = null;
				}
			}
			base.Dispose (disposing);
		}

		void IAnimatable.BatchBegin () { }
		void IAnimatable.BatchCommit () { QueueDraw (); }

		void StartBuildAnimation ()
		{
			this.Animate ("Build",
			              val => renderArg.BuildAnimationProgress = val,
			              length: 5000,
			              repeat: () => true);

			this.Animate ("BuildOpacity",
			              start: renderArg.BuildAnimationOpacity,
			              end: 1.0f,
			              callback: x => renderArg.BuildAnimationOpacity = x);
		}

		void StopBuildAnimation ()
		{
			this.Animate ("BuildOpacity",
			              x => renderArg.BuildAnimationOpacity = x,
			              renderArg.BuildAnimationOpacity,
			              0.0f,
			              finished: (val, aborted) => { if (!aborted) this.AbortAnimation ("Build"); });
		}

		protected override void OnMeasure (Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			base.OnMeasure (orientation, for_size, out minimum, out natural, out minimum_baseline, out natural_baseline);
			if (orientation == Orientation.Vertical) {
				minimum = Math.Max (minimum, 32);
				natural = Math.Max (natural, 32);
			}
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			int x = 0;
			if (MaxWidth > 0 && width > MaxWidth) {
				x = (width - MaxWidth) / 2;
				width = MaxWidth;
			}
			drawArea = new Gdk.Rectangle (x, 0, width, height);

			var child = Child;
			if (child == null)
				return;
			int childWidth = Math.Max (0, width - Padding.Left - Padding.Right);
			MeasureChild (child, Orientation.Vertical, childWidth, out _, out int childHeight);
			childHeight = Math.Min (height, childHeight);
			AllocateChild (child, new Gdk.Rectangle (x + Padding.Left, (height - childHeight) / 2, childWidth, childHeight));
		}

		void TriggerErrorAnimation ()
		{
/* Hack for a compiler error - csc crashes on this:
 			this.Animate (name: "statusAreaError",
			              length: 700,
			              callback: val => renderArg.ErrorAnimationProgress = val);
*/
			this.Animate ("statusAreaError",
			              val => renderArg.ErrorAnimationProgress = val,
			              length: 900);
		}

		void UpdateSeparators ()
		{
			statusIconSeparator.Visible = statusIconBox.Visible && buildResultWidget.Visible;
		}

		public Widget CreateBuildResultsWidget (Orientation orientation)
		{
			Gtk.Box box = new Box (orientation, 3);

			var errorIcon = ImageService.GetIcon (StockIcons.Error).WithSize (Xwt.IconSize.Small);
			var warningIcon = ImageService.GetIcon (StockIcons.Warning).WithSize (Xwt.IconSize.Small);

			var errorImage = new Xwt.ImageView (errorIcon);
			var warningImage = new Xwt.ImageView (warningIcon);

			box.Append (errorImage.ToGtkWidget ());
			Label errors = new Gtk.Label ();
			box.Append (errors);

			box.Append (warningImage.ToGtkWidget ());
			Label warnings = new Gtk.Label ();
			box.Append (warnings);
			ApplyCountLabelStyle (errors);
			ApplyCountLabelStyle (warnings);

			TaskEventHandler updateHandler = delegate {
				int ec=0, wc=0;

				foreach (TaskListEntry t in IdeServices.TaskService.Errors) {
					if (t.Severity == TaskSeverity.Error)
						ec++;
					else if (t.Severity == TaskSeverity.Warning)
						wc++;
				}


				errors.Visible = ec > 0;
				errors.Text = ec.ToString ();
				errorImage.Visible = ec > 0;

				warnings.Visible = wc > 0;
				warnings.Text = wc.ToString ();
				warningImage.Visible = wc > 0;

				box.Visible = ec > 0 || wc > 0;

				UpdateSeparators ();
			};

			updateHandler (null, null);

			IdeServices.TaskService.Errors.TasksAdded += updateHandler;
			IdeServices.TaskService.Errors.TasksRemoved += updateHandler;

			currentApplicationName = BrandingService.ApplicationLongName;
			BrandingService.ApplicationNameChanged += ApplicationNameChanged;
			
			buildResultsCleanup = delegate {
				IdeServices.TaskService.Errors.TasksAdded -= updateHandler;
				IdeServices.TaskService.Errors.TasksRemoved -= updateHandler;
				BrandingService.ApplicationNameChanged -= ApplicationNameChanged;
			};

			box.AddClickHandler (null, click => {
				var pad = IdeApp.Workbench.GetPad<MonoDevelop.Ide.Gui.Pads.ErrorListPad> ();
				pad.BringToFront ();
			});

			errors.Visible = false;
			errorImage.Visible = false;
			warnings.Visible = false;
			warningImage.Visible = false;

			return box;
		}

		static Gtk.CssProvider countLabelProvider;
		static string countLabelCss;

		// GTK 4: ModifyFont/ModifyFg are gone; the font and color of the counters are applied through a CSS class.
		static void ApplyCountLabelStyle (Label label)
		{
			string css;
			using (var font = IdeServices.FontService.SansFont.CopyModified (MonoDevelop.Ide.Gui.Styles.FontScale11)) {
				var c = Styles.StatusBarTextColor;
				css = string.Format (System.Globalization.CultureInfo.InvariantCulture, "label.md-status-count {{ font-family: \"{0}\"; font-size: {1}pt; color: rgb({2},{3},{4}); }}",
					font.Family.Replace ("\"", ""), font.Size / Pango.Scale.PangoScale, (int)(c.Red * 255), (int)(c.Green * 255), (int)(c.Blue * 255));
			}
			if (countLabelProvider == null) {
				countLabelProvider = new Gtk.CssProvider ();
				var display = Gdk.Display.Default;
				if (display != null)
					StyleContext.AddProviderForDisplay (display, countLabelProvider, StyleProviderPriority.Application);
			}
			if (css != countLabelCss) {
				countLabelCss = css;
				countLabelProvider.LoadFromString (css);
			}
			label.AddCssClass ("md-status-count");
		}

		void ApplicationNameChanged (object sender, EventArgs e)
		{
			if (renderArg.CurrentText == currentApplicationName) {
				LoadText (BrandingService.ApplicationLongName, false);
				LoadPixbuf (null);
				QueueDraw ();
			}
			currentApplicationName = BrandingService.ApplicationLongName;
		}

		protected override void OnSnapshot (Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width > 0 && height > 0 && drawArea.Width > 0) {
				using (var context = snapshot.CreateCairoContext (new Gdk.Rectangle (0, 0, width, height))) {
					renderArg.Allocation            = drawArea;
					messageBox.TranslateCoordinates (this, 0, 0, out double messageX, out double messageY);
					renderArg.ChildAllocation       = new Gdk.Rectangle ((int)messageX, (int)messageY, messageBox.Width, messageBox.Height);
					renderArg.MousePosition         = tracker.MousePosition;
					renderArg.Pango                 = PangoContext;

					theme.Render (context, renderArg, this);
				}
			}
			base.OnSnapshot (snapshot);
		}


		#region StatusBar implementation

		List<StatusIcon> icons = new List<StatusIcon> ();

		public StatusBarIcon ShowStatusIcon (Xwt.Drawing.Image pixbuf)
		{
			Runtime.AssertMainThread ();
			StatusIcon icon = new StatusIcon (this, pixbuf);
			statusIconBox.Prepend (icon.box);
			statusIconBox.Visible = true;
			icons.Add (icon);
			return icon;
		}

		void HideStatusIcon (StatusIcon icon)
		{
			icons.Remove (icon);
			statusIconBox.Remove (icon.EventBox);
			if (statusIconBox.FirstChild == null)
				statusIconBox.Visible = false;
		}

		public StatusBarContext CreateContext ()
		{
			return ctxHandler.CreateContext ();
		}

		public void ShowReady ()
		{
			ShowMessage ("");
			SetMessageSourcePad (null);
		}

		public void SetMessageSourcePad (Pad pad)
		{
			sourcePad = pad;
		}

		public bool HasResizeGrip {
			get;
			set;
		}

		public class StatusIcon : StatusBarIcon
		{
			StatusArea statusBar;
			internal Box box;
			string tip;
			DateTime alertEnd;
			Xwt.Drawing.Image icon;
			uint animation;
			Xwt.ImageView image;

			int astep;
			Xwt.Drawing.Image[] images;
			TooltipPopoverWindow tooltipWindow;
			bool mouseOver;
			uint tipShowTimeoutId;
			DateTime scheduledTipTime;
			const int TooltipTimeout = 350;

			public StatusIcon (StatusArea statusBar, Xwt.Drawing.Image icon)
			{
				if (!icon.HasFixedSize)
					icon = icon.WithSize (Xwt.IconSize.Small);

				this.statusBar = statusBar;
				this.icon = icon;
				box = new Box (Orientation.Horizontal, 0);
				image = new Xwt.ImageView (icon);
				box.Append (image.ToGtkWidget ());
				box.AddMotionHandler (null, (x, y) => {
					mouseOver = true;
					ShowTooltip ();
				}, () => {
					mouseOver = false;
					HideTooltip ();
				});
				box.AddClickHandler (click => {
					// TODO: Refactor this in Xwt as an extension method.
					var m = Xwt.ModifierKeys.None;
					if ((click.State & Gdk.ModifierType.ShiftMask) != 0)
						m |= Xwt.ModifierKeys.Shift;
					if ((click.State & Gdk.ModifierType.ControlMask) != 0)
						m |= Xwt.ModifierKeys.Control;
					if ((click.State & Gdk.ModifierType.AltMask) != 0)
						m |= Xwt.ModifierKeys.Alt;
					// TODO: Backport this one.
					if ((click.State & Gdk.ModifierType.MetaMask) != 0)
						m |= Xwt.ModifierKeys.Command;

					Clicked?.Invoke (this, new StatusBarIconClickedEventArgs {
						Button = (Xwt.PointerButton)click.Button,
						Modifiers = m,
					});
				});
			}

			void ShowTooltip ()
			{
				scheduledTipTime = DateTime.Now + TimeSpan.FromMilliseconds (TooltipTimeout);
				if (tipShowTimeoutId == 0)
					tipShowTimeoutId = GLib.Timeout.Add (TooltipTimeout, ShowTooltipEvent);
			}

			bool ShowTooltipEvent ()
			{
				tipShowTimeoutId = 0;

				if (!mouseOver)
					return false;

				int remainingMs = (int) (scheduledTipTime - DateTime.Now).TotalMilliseconds;
				if (remainingMs > 50) {
					// Still some significant time left. Re-schedule the timer
					tipShowTimeoutId = GLib.Timeout.Add ((uint) remainingMs, ShowTooltipEvent);
					return false;
				}

				if (!string.IsNullOrEmpty (tip)) {
					HideTooltip ();
					tooltipWindow = TooltipPopoverWindow.Create ();
					tooltipWindow.ShowArrow = true;
					tooltipWindow.Text = tip;
					tooltipWindow.ShowPopup (box, PopupPosition.Top);
				}
				return false;
			}

			void HideTooltip ()
			{
				if (tooltipWindow != null) {
					tooltipWindow.Destroy ();
					tooltipWindow = null;
				}
			}

			public void Dispose ()
			{
				HideTooltip ();
				statusBar.HideStatusIcon (this);
				if (images != null) {
					foreach (Xwt.Drawing.Image img in images) {
						img.Dispose ();
					}
				}
				if (animation != 0) {
					GLib.Source.Remove (animation);
					animation = 0;
				}
			}

			public string ToolTip {
				get { return tip; }
				set {
					tip = value;
					if (tooltipWindow != null) {
						if (!string.IsNullOrEmpty (tip))
							tooltipWindow.Text = value;
						else
							HideTooltip ();
					} else if (!string.IsNullOrEmpty (tip) && mouseOver)
						ShowTooltip ();
				}
			}

			public string Title {
				get;
				set;
			}

			public string Help {
				get;
				set;
			}

			public Gtk.Widget EventBox {
				get { return box; }
			}

			public Xwt.Drawing.Image Image {
				get { return icon; }
				set {
					icon = value;
					if (!icon.HasFixedSize)
						icon = icon.WithSize (Xwt.IconSize.Small);
					image.Image = icon;
				}
			}

			public void SetAlertMode (int seconds)
			{
				astep = 0;
				alertEnd = DateTime.Now.AddSeconds (seconds);

				if (animation != 0)
					GLib.Source.Remove (animation);

				animation = GLib.Timeout.Add (60, new GLib.TimeoutHandler (AnimateIcon));

				if (images == null) {
					images = new Xwt.Drawing.Image [10];
					for (int n=0; n<10; n++)
						images [n] = icon.WithAlpha (((double)(9-n))/10.0);
				}
			}

			bool AnimateIcon ()
			{
				if (DateTime.Now >= alertEnd && astep == 0) {
					image.Image = icon;
					animation = 0;
					return false;
				}
				if (astep < 10)
					image.Image = images [astep];
				else
					image.Image = images [20 - astep - 1];

				astep = (astep + 1) % 20;
				return true;
			}

			public event EventHandler<StatusBarIconClickedEventArgs> Clicked;
		}

		#endregion

		#region StatusBarContextBase implementation

		public void ShowError (string error)
		{
			ShowMessage (StockIcons.StatusError, error);
		}

		public void ShowWarning (string warning)
		{
			ShowMessage (StockIcons.StatusWarning, warning);
		}

		public void ShowMessage (string message)
		{
			ShowMessage (null, message, false);
		}

		public void ShowMessage (string message, bool isMarkup)
		{
			ShowMessage (null, message, isMarkup);
		}

		public void ShowMessage (IconId image, string message)
		{
			ShowMessage (image, message, false);
		}

		public void ShowMessage (IconId image, string message, bool isMarkup)
		{
			if (this.AnimationIsRunning("Text") || animPauseHandle > 0) {
				messageQueue.Clear ();
				messageQueue.Enqueue (new Message (image, message, isMarkup));
			} else {
				ShowMessageInner (image, message, isMarkup);
			}
		}

		void ShowMessageInner (IconId image, string message, bool isMarkup)
		{
			Runtime.AssertMainThread ();

			if (image == StockIcons.StatusError) {
				// If the application doesn't have the focus, trigger the animation
				// again when it gains the focus
				if (!IdeApp.CommandService.ApplicationHasFocus)
					errorAnimPending = true;
				TriggerErrorAnimation ();
			}

			LoadText (message, isMarkup);
			LoadPixbuf (image);
			/* Hack for a compiler error - csc crashes on this:
			this.Animate ("Text", easing: Easing.SinInOut,
			              callback: x => renderArg.TextAnimationProgress = x,
			              finished: x => { animPauseHandle = GLib.Timeout.Add (1000, () => {
					if (messageQueue.Count > 0) {
						Message m = messageQueue.Dequeue();
						ShowMessageInner (m.Icon, m.Text, m.IsMarkup);
					}
					animPauseHandle = 0;
					return false;
				});
			});
			*/
			this.Animate ("Text",
			              x => renderArg.TextAnimationProgress = x,
			              easing: Easing.SinInOut,
			              finished: (x, b) => { animPauseHandle = GLib.Timeout.Add (1000, () => {
					if (messageQueue.Count > 0) {
						Message m = messageQueue.Dequeue();
						ShowMessageInner (m.Icon, m.Text, m.IsMarkup);
					}
					animPauseHandle = 0;
					return false;
				});
			});


			if (renderArg.CurrentText == renderArg.LastText)
				this.AbortAnimation ("Text");

			QueueDraw ();
		}

		void LoadText (string message, bool isMarkup)
		{
			if (string.IsNullOrEmpty(message))
				message = BrandingService.ApplicationLongName;
			message = message ?? "";

			renderArg.LastText = renderArg.CurrentText;
			renderArg.CurrentText = message.Replace (System.Environment.NewLine, " ").Replace ("\n", " ").Trim ();

			renderArg.LastTextIsMarkup = renderArg.CurrentTextIsMarkup;
			renderArg.CurrentTextIsMarkup = isMarkup;
		}

		static bool iconLoaded = false;
		void LoadPixbuf (IconId image)
		{
			// We dont need to load the same image twice
			if (currentIcon == image && iconLoaded)
				return;

			currentIcon = image;
			iconAnimation = null;

			// clean up previous running animation
			if (currentIconAnimation != null) {
				currentIconAnimation.Dispose ();
				currentIconAnimation = null;
			}

			// if we have nothing, use the default icon
			if (image == IconId.Null)
				image = "md-status-steady";

			// load image now
			if (ImageService.IsAnimation (image, StockIconSize.Menu)) {
				iconAnimation = ImageService.GetAnimatedIcon (image, StockIconSize.Menu);
				renderArg.CurrentPixbuf = iconAnimation.FirstFrame.WithSize (16,16);
				currentIconAnimation = iconAnimation.StartAnimation (delegate (Xwt.Drawing.Image p) {
					renderArg.CurrentPixbuf = p.WithSize (16,16);
					QueueDraw ();
				});
			} else {
				renderArg.CurrentPixbuf = ImageService.GetIcon (image).WithSize (16,16);
			}

			iconLoaded = true;
		}
		#endregion


		#region Progress Monitor implementation
		public static event EventHandler ProgressBegin, ProgressEnd, ProgressPulse;
		public static event EventHandler<FractionEventArgs> ProgressFraction;

		public sealed class FractionEventArgs : EventArgs
		{
			public double Work { get; private set; }

			public FractionEventArgs (double work)
			{
				this.Work = work;
			}
		}

		static void OnProgressBegin (EventArgs e)
		{
			var handler = ProgressBegin;
			if (handler != null)
				handler (null, e);
		}

		static void OnProgressEnd (EventArgs e)
		{
			var handler = ProgressEnd;
			if (handler != null)
				handler (null, e);
		}

		static void OnProgressPulse (EventArgs e)
		{
			var handler = ProgressPulse;
			if (handler != null)
				handler (null, e);
		}

		static void OnProgressFraction (FractionEventArgs e)
		{
			var handler = ProgressFraction;
			if (handler != null)
				handler (null, e);
		}

		public void BeginProgress (string name)
		{
			ShowMessage (name);
			if (!progressBarVisible) {
				progressBarVisible = true;
				OnProgressBegin (EventArgs.Empty);
			}
		}

		public void BeginProgress (IconId image, string name)
		{
			ShowMessage (image, name);
			if (!progressBarVisible) {
				progressBarVisible = true;
				OnProgressBegin (EventArgs.Empty);
			}
		}

		public void SetProgressFraction (double work)
		{
			Runtime.AssertMainThread ();
			OnProgressFraction (new FractionEventArgs (work));
		}

		public void EndProgress ()
		{
			if (!progressBarVisible)
				return;

			progressBarVisible = false;
			OnProgressEnd (EventArgs.Empty);
			AutoPulse = false;
		}

		public void Pulse ()
		{
			Runtime.AssertMainThread ();
			OnProgressPulse (EventArgs.Empty);
		}

		uint autoPulseTimeoutId;
		public bool AutoPulse {
			get { return autoPulseTimeoutId != 0; }
			set {
				Runtime.AssertMainThread ();
				if (value) {
					if (autoPulseTimeoutId == 0) {
						autoPulseTimeoutId = GLib.Timeout.Add (100, delegate {
							Pulse ();
							return true;
						});
					}
				} else {
					if (autoPulseTimeoutId != 0) {
						GLib.Source.Remove (autoPulseTimeoutId);
						autoPulseTimeoutId = 0;
					}
				}
			}
		}

		public void SetCancellationTokenSource (CancellationTokenSource source)
		{
		}
		#endregion

		string ITestableStatusBar.CurrentText => messageQueue.Count > 0 ? "" : renderArg.CurrentText;
		string [] ITestableStatusBar.CurrentIcons => icons.Select (x => x.ToolTip).ToArray ();
	}

	class StatusAreaSeparator: CairoWidget
	{
		protected override void OnMeasure (Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			minimum = natural = orientation == Orientation.Horizontal ? 1 : 0;
			minimum_baseline = natural_baseline = -1;
		}

		protected override void OnDraw (Cairo.Context ctx, int width, int height)
		{
			ctx.Rectangle (0, 0, 1, height);

			// FIXME: VV: Remove gradient features
			using (Cairo.LinearGradient gr = new LinearGradient (0, 0, 0, height)) {
				gr.AddColorStop (0, new Cairo.Color (0, 0, 0, 0));
				gr.AddColorStop (0.5, new Cairo.Color (0, 0, 0, 0.2));
				gr.AddColorStop (1, new Cairo.Color (0, 0, 0, 0));
				ctx.SetSource (gr);
				ctx.Fill ();
			}
		}
	}
}

