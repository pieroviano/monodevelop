//
// TextEditor.cs
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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Mono.TextEditor.Highlighting;
using Mono.TextEditor.PopupWindow;
using Mono.TextEditor.Theatrics;

using Gdk;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Core.Text;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide.Editor.Highlighting;

namespace Mono.TextEditor
{
	[System.ComponentModel.Category("Mono.TextEditor")]
	[System.ComponentModel.ToolboxItem(true)]
	partial class MonoTextEditor : ContainerWidget, IScrollAdjustable
	{
		readonly TextArea textArea;

		internal LayoutCache LayoutCache {
			get;
			private set;
		}

		internal bool IsInKeypress {
			get { return textArea.IsInKeypress; }
		}

		public TextArea TextArea {
			get {
				return textArea;
			}
		}

		public MonoTextEditor () : this(new TextDocument ())
		{
		}

		public MonoTextEditor (TextDocument doc)
			: this (doc, null)
		{
		}
		
		internal MonoTextEditor (TextDocument doc, ITextEditorOptions options)
			: this (doc, options, new SimpleEditMode ())
		{
		}
		Thread uiThread;

		internal MonoTextEditor (TextDocument doc, ITextEditorOptions options, EditMode initialMode) 
		{
			Direction = TextDirection.Ltr;
			Hexpand = true;
			Vexpand = true;
			Overflow = Gtk.Overflow.Hidden;
			uiThread = Thread.CurrentThread;
			LayoutCache = new LayoutCache (this);
			this.textArea = new TextArea (doc, options, initialMode);
			this.textArea.Initialize (this, doc, options, initialMode);
			this.textArea.EditorOptionsChanged += (sender, e) => OptionsChanged (sender, e);
			AddTopLevelWidget (textArea, 0, 0);

			stage.ActorStep += OnActorStep;
			if (Platform.IsMac) {
				VScroll += delegate {
					for (int i = 1; i < containerChildren.Count; i++) {
						containerChildren[i].Child.QueueDraw ();
					}
				};
				HScroll += delegate {
					for (int i = 1; i < containerChildren.Count; i++) {
						containerChildren[i].Child.QueueDraw ();
					}
				};
			}
		}

		internal bool IsUIThread { get { return Thread.CurrentThread != uiThread; } }

		internal void CheckUIThread ()
		{
			if (Thread.CurrentThread != uiThread)
				throw new InvalidOperationException ("Not executed on UI thread.");
		}

		public new void GrabFocus ()
		{
			TextArea.GrabFocus ();
		}

		public new bool HasFocus {
			get {
				return TextArea.HasFocus;
			}
		}

		protected virtual void OnDestroyed ()
		{
			UnregisterAdjustments ();
			LayoutCache.Dispose ();
		}

		bool destroyed;

		/// <summary>Raised when the editor is destroyed (GTK 4 widgets have no Destroyed event).</summary>
		public event EventHandler Destroyed;

		/// <summary>True once <see cref="Destroy"/> was called or the widget was disposed.</summary>
		public bool IsDestroyed {
			get { return destroyed; }
		}

		public override void Destroy ()
		{
			if (!destroyed) {
				destroyed = true;
				OnDestroyed ();
				Destroyed?.Invoke (this, EventArgs.Empty);
			}
			base.Destroy ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && !destroyed) {
				destroyed = true;
				OnDestroyed ();
				Destroyed?.Invoke (this, EventArgs.Empty);
			}
			base.Dispose (disposing);
		}

		void UnregisterAdjustments ()
		{
			if (vAdjustement != null)
				vAdjustement.ValueChanged -= HandleAdjustmentValueChange;
			if (hAdjustement != null)
				hAdjustement.ValueChanged -= HandleAdjustmentValueChange;
			vAdjustement = null;
			hAdjustement = null;
		}

		Gtk.Adjustment hAdjustement;
		Gtk.Adjustment vAdjustement;

		/// <summary>
		/// Connects the editor to the scroll bars' adjustments. The editor scrolls its content virtually and
		/// keeps the adjustments' bounds up to date (replaces GTK 2 <c>OnSetScrollAdjustments</c>).
		/// </summary>
		public void SetScrollAdjustments (Gtk.Adjustment hAdjustement, Gtk.Adjustment vAdjustement)
		{
			UnregisterAdjustments ();
			this.vAdjustement = vAdjustement;
			this.hAdjustement = hAdjustement;
			textArea.SetTextEditorScrollAdjustments (hAdjustement, vAdjustement);
			if (hAdjustement != null) {
				hAdjustement.ValueChanged += HandleAdjustmentValueChange;
			}

			if (vAdjustement != null) {
				vAdjustement.ValueChanged += HandleAdjustmentValueChange;
			}
			OnScrollAdjustmentsSet ();
		}

		void IScrollAdjustable.SetScrollAdjustments (Gtk.Adjustment hAdjustment, Gtk.Adjustment vAdjustment)
		{
			SetScrollAdjustments (hAdjustment, vAdjustment);
		}

		void HandleAdjustmentValueChange (object sender, EventArgs e)
		{
			SetChildrenPositions (Allocation);
		}

		protected virtual void OnScrollAdjustmentsSet ()
		{
		}

		/// <summary>Raised after the editor has been allocated a new size.</summary>
		internal event EventHandler SizeAllocated;

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			var allocation = new Gdk.Rectangle (0, 0, width, height);
			CurrentMode.AllocateTextArea (this, textArea, allocation);
			SetChildrenPositions (allocation);
			SizeAllocated?.Invoke (this, EventArgs.Empty);
		}

		protected override void OnMeasure (Gtk.Orientation orientation, int for_size, out int minimum, out int natural, out int minimum_baseline, out int natural_baseline)
		{
			// The editor scrolls its content virtually: it can be any size.
			minimum = natural = 0;
			minimum_baseline = natural_baseline = -1;
			containerChildren.ForEach (c => MeasureChild (c.Child, orientation, -1, out _, out _));
		}

		internal protected virtual string GetIdeColorStyleName ()
		{
			return TextEditorOptions.DefaultColorStyle;
		}

		#region Container
		/// <summary>An embedded widget with its position in the scrolled editor coordinate space.</summary>
		internal class EditorContainerChild
		{
			public Gtk.Widget Child { get; }
			public int X { get; set; }
			public int Y { get; set; }
			public bool FixedPosition { get; set; }
			public Gdk.Size LastSize { get; set; }
			public EditorContainerChild (Gtk.Widget child)
			{
				Child = child;
			}
		}

		internal List<EditorContainerChild> containerChildren = new List<EditorContainerChild> ();

		/// <summary>Raised when the natural size of an embedded widget changed (GTK 4 widgets have no SizeAllocated event).</summary>
		internal event EventHandler<Gtk.Widget> ChildSizeChanged;

		/// <summary>The position information of an embedded widget (or of the widget wrapped by an animated embedded widget).</summary>
		internal EditorContainerChild this [Gtk.Widget w] {
			get {
				return containerChildren.FirstOrDefault (info => info.Child == w || (info.Child is AnimatedWidget && ((AnimatedWidget)info.Child).Widget == w));
			}
		}

		public void AddTopLevelWidget (Gtk.Widget widget, int x, int y)
		{
			AddChild (widget);
			EditorContainerChild info = new EditorContainerChild (widget);
			info.X = x;
			info.Y = y;
			containerChildren.Add (info);
			SetAdjustments ();
		}
		
		public void MoveTopLevelWidget (Gtk.Widget widget, int x, int y)
		{
			foreach (EditorContainerChild info in containerChildren.ToArray ()) {
				if (info.Child == widget || (info.Child is AnimatedWidget && ((AnimatedWidget)info.Child).Widget == widget)) {
					if (info.X == x && info.Y == y)
						break;
					info.X = x;
					info.Y = y;
					if (widget.Visible)
						ResizeChild (Allocation, info);
					break;
				}
			}
		}

		/// <summary>
		/// Returns the position of an embedded widget
		/// </summary>
		public void GetTopLevelWidgetPosition (Gtk.Widget widget, out int x, out int y)
		{
			foreach (EditorContainerChild info in containerChildren.ToArray ()) {
				if (info.Child == widget || (info.Child is AnimatedWidget && ((AnimatedWidget)info.Child).Widget == widget)) {
					x = info.X;
					y = info.Y;
					return;
				}
			}
			x = y = 0;
		}
		
		public void MoveToTop (Gtk.Widget widget)
		{
			EditorContainerChild editorContainerChild = containerChildren.FirstOrDefault (c => c.Child == widget);
			if (editorContainerChild == null)
				throw new Exception ("child " + widget + " not found.");
			List<EditorContainerChild> newChilds = new List<EditorContainerChild> (containerChildren.Where (child => child != editorContainerChild));
			newChilds.Add (editorContainerChild);
			this.containerChildren = newChilds;
			// the last sibling is painted (and picked) on top
			widget.InsertAfter (this, LastChild);
		}

		/// <summary>Removes an embedded widget (see <see cref="AddTopLevelWidget"/>).</summary>
		public void Remove (Gtk.Widget widget)
		{
			foreach (EditorContainerChild info in containerChildren.ToArray ()) {
				if (info.Child == widget) {
					RemoveChild (widget);
					containerChildren.Remove (info);
					break;
				}
			}
		}

		void ResizeChild (Gdk.Rectangle allocation, EditorContainerChild child)
		{
			if (!child.Child.Visible)
				return;
			var req = GetChildNaturalSize (child.Child);
			var childRectangle = new Gdk.Rectangle (child.X, child.Y, System.Math.Max (1, req.Width), System.Math.Max (1, req.Height));
			if (!child.FixedPosition) {
				double zoom = Options.Zoom;
				childRectangle.X = (int)(child.X * zoom - HAdjustment.Value);
				childRectangle.Y = (int)(child.Y * zoom - VAdjustment.Value);
			}
			AllocateChild (child.Child, childRectangle);
			if (child.LastSize.Width != req.Width || child.LastSize.Height != req.Height) {
				child.LastSize = req;
				ChildSizeChanged?.Invoke (this, child.Child);
			}
		}

		void SetChildrenPositions (Gdk.Rectangle allocation)
		{
			foreach (EditorContainerChild child in containerChildren.ToArray ()) {
				if (child.Child == textArea)
					continue;
				ResizeChild (allocation, child);
			}
		}
		#endregion

		#region Animated Widgets
		Stage<AnimatedWidget> stage = new Stage<AnimatedWidget> ();
		
		bool OnActorStep (Actor<AnimatedWidget> actor)
		{
			switch (actor.Target.AnimationState) {
			case AnimationState.Coming:
				actor.Target.QueueDraw ();
				actor.Target.Percent = actor.Percent;
				if (actor.Expired) {
					actor.Target.AnimationState = AnimationState.Idle;
					return false;
				}
				break;
			case AnimationState.IntendingToGo:
				actor.Target.AnimationState = AnimationState.Going;
				actor.Target.Bias = actor.Percent;
				actor.Reset ((uint)(actor.Target.Duration * actor.Percent));
				break;
			case AnimationState.Going:
				if (actor.Expired) {
					this.Remove (actor.Target);
					return false;
				}
				actor.Target.Percent = 1.0 - actor.Percent;
				break;
			}
			return true;
		}
		
		void OnWidgetDestroyed (object sender, EventArgs args)
		{
			RemoveCore ((AnimatedWidget)sender);
		}
		
		void RemoveCore (AnimatedWidget widget)
		{
			RemoveCore (widget, widget.Duration, 0, 0, false, false);
		}
		
		void RemoveCore (AnimatedWidget widget, uint duration, Easing easing, Blocking blocking, bool use_easing, bool use_blocking)
		{
			if (duration > 0)
				widget.Duration = duration;
			
			if (use_easing)
				widget.Easing = easing;
			
			if (use_blocking)
				widget.Blocking = blocking;
			
			if (widget.AnimationState == AnimationState.Coming) {
				widget.AnimationState = AnimationState.IntendingToGo;
			} else {
				if (widget.Easing == Easing.QuadraticIn) {
					widget.Easing = Easing.QuadraticOut;
				} else if (widget.Easing == Easing.QuadraticOut) {
					widget.Easing = Easing.QuadraticIn;
				} else if (widget.Easing == Easing.ExponentialIn) {
					widget.Easing = Easing.ExponentialOut;
				} else if (widget.Easing == Easing.ExponentialOut) {
					widget.Easing = Easing.ExponentialIn;
				}
				widget.AnimationState = AnimationState.Going;
				stage.Add (widget, widget.Duration);
			}
		}
		
		internal void AddAnimatedWidget (Widget widget, uint duration, Easing easing, Blocking blocking, int x, int y)
		{
			AnimatedWidget animated_widget = new AnimatedWidget (widget, duration, easing, blocking, false);
			AddChild (animated_widget);
			animated_widget.WidgetDestroyed += OnWidgetDestroyed;
			stage.Add (animated_widget, duration);
			animated_widget.StartPadding = 0;
			animated_widget.EndPadding = widget.Height;
			//			animated_widget.Node = animated_widget;

			EditorContainerChild info = new EditorContainerChild (animated_widget);
			info.X = x;
			info.Y = y;
			info.FixedPosition = true;
			containerChildren.Add (info);
		}
		#endregion
		
		#region TextArea delegation
		public TextDocument Document {
			get {
				return textArea.Document;
			}
		}
		
		public bool IsDisposed {
			get {
				return textArea.IsDisposed;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MonoTextEditor"/> converts tabs to spaces.
		/// It is possible to overwrite the default options value for certain languages (like F#).
		/// </summary>
		/// <value>
		/// <c>true</c> if tabs to spaces should be converted; otherwise, <c>false</c>.
		/// </value>
		public bool TabsToSpaces {
			get {
				return textArea.TabsToSpaces;
			}
			set {
				textArea.TabsToSpaces = value;
			}
		}

		public Mono.TextEditor.CaretImpl Caret {
			get {
				return textArea.Caret;
			}
		}

		protected internal IMMulticontext IMContext {
			get { return textArea.IMContext; }
		}

		public string IMModule {
			get {
				return textArea.IMModule;
			}
			set {
				textArea.IMModule = value;
			}
		}

		internal ITextEditorOptions Options {
			get {
				return textArea.Options;
			}
			set {
				textArea.Options = value;
			}
		}

		public string FileName {
			get {
				return textArea.FileName;
			}
		}

		public string MimeType {
			get {
				return textArea.MimeType;
			}
		}

		public double LineHeight {
			get {
				return textArea.LineHeight;
			}
			internal set {
				textArea.LineHeight = value;
			}
		}

		public TextViewMargin TextViewMargin {
			get {
				return textArea.TextViewMargin;
			}
		}

		public ActionMargin ActionMargin {
			get {
				return textArea.ActionMargin;
			}
		}

		public Margin IconMargin {
			get { return textArea.IconMargin; }
		}

		public DocumentLocation LogicalToVisualLocation (DocumentLocation location)
		{
			return textArea.LogicalToVisualLocation (location);
		}
		
		public DocumentLocation LogicalToVisualLocation (int line, int column)
		{
			return textArea.LogicalToVisualLocation (line, column);
		}
		
		public void CenterToCaret ()
		{
			textArea.CenterToCaret ();
		}
		
		public void CenterTo (int offset)
		{
			textArea.CenterTo (offset);
		}
		
		public void CenterTo (int line, int column)
		{
			textArea.CenterTo (line, column);
		}
		
		public void CenterTo (DocumentLocation p)
		{
			textArea.CenterTo (p);
		}

		internal void SmoothScrollTo (double value)
		{
			textArea.SmoothScrollTo (value);
		}

		public void ScrollTo (int offset)
		{
			textArea.ScrollTo (offset);
		}
		
		public void ScrollTo (int line, int column)
		{
			textArea.ScrollTo (line, column);
		}

		public void ScrollTo (DocumentLocation p)
		{
			textArea.ScrollTo (p);
		}

		public void ScrollTo (Gdk.Rectangle rect)
		{
			textArea.ScrollTo (rect);
		}

		public void ScrollToCaret ()
		{
			textArea.ScrollToCaret ();
		}

		public void TryToResetHorizontalScrollPosition ()
		{
			textArea.TryToResetHorizontalScrollPosition ();
		}

		public int GetWidth (string text)
		{
			return textArea.GetWidth (text);
		}

		internal void HideMouseCursor ()
		{
			textArea.HideMouseCursor ();
		}

		public void ClearTooltipProviders ()
		{
			GetTextEditorData ().ClearTooltipProviders ();
		}
		
		public void AddTooltipProvider (TooltipProvider provider)
		{
			GetTextEditorData ().AddTooltipProvider (provider);
		}
		
		public void RemoveTooltipProvider (TooltipProvider provider)
		{
			GetTextEditorData ().RemoveTooltipProvider (provider);
		}

		internal void RedrawMargin (Margin margin)
		{
			textArea.RedrawMargin (margin);
		}
		
		public void RedrawMarginLine (Margin margin, int logicalLine)
		{
			textArea.RedrawMarginLine (margin, logicalLine);
		}
		internal void RedrawPosition (int logicalLine, int logicalColumn)
		{
			textArea.RedrawPosition (logicalLine, logicalColumn);
		}

		internal void RedrawLine (int line, bool removeLineCache = true)
		{
			textArea.RedrawLine (line, removeLineCache);
		}

		internal void RedrawLines (int start, int end, bool removeLineCache = true)
		{
			textArea.RedrawLines (start, end, removeLineCache);
		}

		internal string preeditString {
			get {
				return textArea.preeditString;
			}
		}

		internal int preeditOffset {
			get {
				return textArea.preeditOffset;
			}
		}
		
		internal int preeditCursorCharIndex {
			get {
				return textArea.preeditCursorCharIndex;
			}
		}
		
		internal Pango.AttrList preeditAttrs {
			get {
				return textArea.preeditAttrs;
			}
		}

		internal bool UpdatePreeditLineHeight ()
		{
			return textArea.UpdatePreeditLineHeight ();
		}

		internal void ResetIMContext ()
		{
			textArea.ResetIMContext ();
		}

		internal void CommitPreedit ()
		{
			textArea.CommitPreedit ();
		}

		internal bool ContainsPreedit (int offset, int length)
		{
			return textArea.ContainsPreedit (offset, length);
		}

		internal void FireLinkEvent (string link, uint button, ModifierType modifierState)
		{
			textArea.FireLinkEvent (link, button, modifierState);
		}

		internal void RequestResetCaretBlink ()
		{
			textArea.RequestResetCaretBlink ();
		}
		
		internal void SetAdjustments  ()
		{
			textArea.SetAdjustments ();
		}
		
		public bool IsInDrag {
			get {
				return textArea.IsInDrag;
			}
		}

		public event EventHandler VScroll {
			add { textArea.VScroll += value; }
			remove { textArea.VScroll -= value; }
		}

		public event EventHandler HScroll {
			add { textArea.HScroll += value; }
			remove { textArea.HScroll -= value; }
		}

		#endregion

		#region TextEditorData delegation
		public string EolMarker {
			get {
				return textArea.EolMarker;
			}
		}

		internal EditorTheme EditorTheme {
			get {
				return textArea.EditorTheme;
			}
		}
		
		public EditMode CurrentMode {
			get {
				return textArea.CurrentMode;
			}
			set {
				textArea.CurrentMode = value;
			}
		}
		
		public bool IsSomethingSelected {
			get {
				return textArea.IsSomethingSelected;
			}
		}
		
		public MonoDevelop.Ide.Editor.Selection MainSelection {
			get {
				return textArea.MainSelection;
			}
			set {
				textArea.MainSelection = value;
			}
		}
		
		public MonoDevelop.Ide.Editor.SelectionMode SelectionMode {
			get {
				return textArea.SelectionMode;
			}
			set {
				textArea.SelectionMode = value;
			}
		}
		
		public ISegment SelectionRange {
			get {
				return textArea.SelectionRange;
			}
			set {
				textArea.SelectionRange = value;
			}
		}
		
		public string SelectedText {
			get {
				return textArea.SelectedText;
			}
			set {
				textArea.SelectedText = value;
			}
		}
		
		public int SelectionAnchor {
			get {
				return textArea.SelectionAnchor;
			}
			set {
				textArea.SelectionAnchor = value;
			}
		}

		public int SelectionLead {
			get {
				return textArea.SelectionLead;
			}
			set {
				textArea.SelectionLead = value;
			}
		}

		public IEnumerable<DocumentLine> SelectedLines {
			get {
				return textArea.SelectedLines;
			}
		}
		
		public Gtk.Adjustment HAdjustment {
			get {
				return textArea.HAdjustment;
			}
		}
		
		public Gtk.Adjustment VAdjustment {
			get {
				return textArea.VAdjustment;
			}
		}
		
		public int Insert (int offset, string value)
		{
			return textArea.Insert (offset, value);
		}
		
		public void Remove (DocumentRegion region)
		{
			textArea.Remove (region);
		}
		
		public void Remove (ISegment removeSegment)
		{
			textArea.Remove (removeSegment);
		}
		
		public void Remove (int offset, int count)
		{
			textArea.Remove (offset, count);
		}
		
		public int Replace (int offset, int count, string value)
		{
			return textArea.Replace (offset, count, value);
		}
		
		public void ClearSelection ()
		{
			textArea.ClearSelection ();
		}
		
		public void DeleteSelectedText ()
		{
			textArea.DeleteSelectedText ();
		}
		
		public void DeleteSelectedText (bool clearSelection)
		{
			textArea.DeleteSelectedText (clearSelection);
		}
		
		public void RunEditAction (Action<TextEditorData> action)
		{
			action (GetTextEditorData ());
		}
		
		public void SetSelection (int anchorOffset, int leadOffset)
		{
			textArea.SetSelection (anchorOffset, leadOffset);
		}
		
		public void SetSelection (DocumentLocation anchor, DocumentLocation lead)
		{
			textArea.SetSelection (anchor, lead);
		}
		
		public void SetSelection (int anchorLine, int anchorColumn, int leadLine, int leadColumn)
		{
			textArea.SetSelection (anchorLine, anchorColumn, leadLine, leadColumn);
		}
		
		public void ExtendSelectionTo (DocumentLocation location)
		{
			textArea.ExtendSelectionTo (location);
		}
		public void ExtendSelectionTo (int offset)
		{
			textArea.ExtendSelectionTo (offset);
		}
		public void SetSelectLines (int from, int to)
		{
			textArea.SetSelectLines (from, to);
		}
		
		public void InsertAtCaret (string text)
		{
			textArea.InsertAtCaret (text);
		}
		
		public bool CanEdit (int line)
		{
			return textArea.CanEdit (line);
		}
		
		public string GetLineText (int line)
		{
			return textArea.GetLineText (line);
		}
		
		public string GetLineText (int line, bool includeDelimiter)
		{
			return textArea.GetLineText (line, includeDelimiter);
		}
		
		/// <summary>
		/// Use with care.
		/// </summary>
		/// <returns>
		/// A <see cref="TextEditorData"/>
		/// </returns>
		public TextEditorData GetTextEditorData ()
		{
			return textArea.GetTextEditorData ();
		}

		/// <remarks>
		/// The Key may be null if it has been handled by the IMContext. In such cases, the char is the value.
		/// </remarks>
		/// <summary>
		/// Called when a key is pressed in the text area. <paramref name="handleKeyPress"/> runs the default handling;
		/// overrides can run code around it (replaces the GTK 2 <c>OnKeyPressEvent</c> override).
		/// </summary>
		protected internal virtual bool OnKeyPressEvent (Func<bool> handleKeyPress)
		{
			return handleKeyPress ();
		}

		/// <summary>Called when the text area loses the keyboard focus (replaces the GTK 2 <c>OnFocusOutEvent</c> override).</summary>
		protected internal virtual void OnFocusOutEvent ()
		{
		}

		protected internal virtual bool OnIMProcessedKeyPressEvent (Gdk.Key key, uint ch, Gdk.ModifierType state)
		{
			SimulateKeyPress (key, ch, state);
			return true;
		}

		public void SimulateKeyPress (Gdk.Key key, uint unicodeChar, ModifierType modifier)
		{
			textArea.SimulateKeyPress (key, unicodeChar, modifier);
		}

		
		public void RunAction (Action<TextEditorData> action)
		{
			try {
				action (GetTextEditorData ());
			} catch (Exception e) {
				if (Debugger.IsAttached)
					Debugger.Break ();
				LoggingService.LogError ("Error while executing " + action, e);
			}
		}

		public void HideTooltip (bool checkMouseOver = true)
		{
			textArea.HideTooltip (TooltipCloseReason.Force);
		}
		public Action<ClickInfo> DoPopupMenu {
			get {
				return textArea.DoPopupMenu;
			}
			set {
				textArea.DoPopupMenu = value;
			} 
		}


		internal event EventHandler<TextDroppedEventArgs> TextDropped {
			add { textArea.TextDropped += value; }
			remove { textArea.TextDropped -= value; }
		}

		public event EventHandler SelectionChanged {
			add { textArea.SelectionChanged += value; }
			remove { textArea.SelectionChanged -= value; }
		}

		public void CaretToDragCaretPosition ()
		{
			textArea.CaretToDragCaretPosition ();
		}

		public event EventHandler<PaintEventArgs> Painted {
			add { textArea.Painted += value; }
			remove { textArea.Painted -= value; }
		}

		public event EventHandler<LinkEventArgs> LinkRequest {
			add { textArea.LinkRequest += value; }
			remove { textArea.LinkRequest -= value; }
		}

		internal void ShowListWindow<T> (ListWindow<T> window, DocumentLocation loc)
		{
			textArea.ShowListWindow<T> (window, loc);
		}

		public Margin LockedMargin {
			get {
				return textArea.LockedMargin;
			}
			set {
				textArea.LockedMargin = value;
			}
		}
		#endregion
		
		#region Document delegation

		public event EventHandler EditorOptionsChanged {
			add { textArea.EditorOptionsChanged += value; }
			remove { textArea.EditorOptionsChanged -= value; }
		}

		protected virtual void OptionsChanged (object sender, EventArgs args)
		{
		}

		public int Length {
			get {
				return Document.Length;
			}
		}
		
		public string Text {
			get {
				return Document.Text;
			}
			set {
				Document.Text = value;
			}
		}
		
		public string GetTextBetween (int startOffset, int endOffset)
		{
			return Document.GetTextBetween (startOffset, endOffset);
		}
		
		public string GetTextBetween (DocumentLocation start, DocumentLocation end)
		{
			return Document.GetTextBetween (start, end);
		}
		
		public string GetTextBetween (int startLine, int startColumn, int endLine, int endColumn)
		{
			return Document.GetTextBetween (startLine, startColumn, endLine, endColumn);
		}
		
		public string GetTextAt (int offset, int count)
		{
			return Document.GetTextAt (offset, count);
		}
		
		public string GetTextAt (ISegment segment)
		{
			return Document.GetTextAt (segment);
		}
		
		public string GetTextAt (DocumentRegion region)
		{
			return Document.GetTextAt (region);
		}
		
		public char GetCharAt (int offset)
		{
			return Document.GetCharAt (offset);
		}
		
		public IEnumerable<DocumentLine> Lines {
			get {
				return Document.Lines;
			}
		}
		
		public int LineCount {
			get {
				return Document.LineCount;
			}
		}
		
		public int LocationToOffset (int line, int column)
		{
			return Document.LocationToOffset (line, column);
		}
		
		public int LocationToOffset (DocumentLocation location)
		{
			return Document.LocationToOffset (location);
		}
		
		public DocumentLocation OffsetToLocation (int offset)
		{
			return Document.OffsetToLocation (offset);
		}
		
		public string GetLineIndent (int lineNumber)
		{
			return Document.GetLineIndent (lineNumber);
		}
		
		public string GetLineIndent (DocumentLine segment)
		{
			return Document.GetLineIndent (segment);
		}
		
		public DocumentLine GetLine (int lineNumber)
		{
			return Document.GetLine (lineNumber);
		}
		
		public DocumentLine GetLineByOffset (int offset)
		{
			return Document.GetLineByOffset (offset);
		}
		
		public int OffsetToLineNumber (int offset)
		{
			return Document.OffsetToLineNumber (offset);
		}
		
		public IDisposable OpenUndoGroup()
		{
			return Document.OpenUndoGroup ();
		}
#endregion
		
		#region Search & Replace
		public string SearchPattern {
			get {
				return textArea.SearchPattern;
			}
			set {
				textArea.SearchPattern = value;
			}
		}
		
		public ISearchEngine SearchEngine {
			get {
				return textArea.SearchEngine;
			}
			set {
				textArea.SearchEngine = value;
			}
		}

		public event EventHandler HighlightSearchPatternChanged {
			add { textArea.HighlightSearchPatternChanged += value; }
			remove { textArea.HighlightSearchPatternChanged -= value; }
		}

		public bool HighlightSearchPattern {
			get {
				return textArea.HighlightSearchPattern;
			}
			set {
				textArea.HighlightSearchPattern = value;
			}
		}
		
		public bool IsCaseSensitive {
			get {
				return textArea.IsCaseSensitive;
			}
			set {
				textArea.IsCaseSensitive = value;
			}
		}
		
		public bool IsWholeWordOnly {
			get {
				return textArea.IsWholeWordOnly;
			}
			
			set {
				textArea.IsWholeWordOnly = value;
			}
		}
		
		public ISegment SearchRegion {
			get {
				return textArea.SearchRegion;
			}
			
			set {
				textArea.SearchRegion = value;
			}
		}
		
		public SearchResult SearchForward (int fromOffset)
		{
			return textArea.SearchForward (fromOffset);
		}
		
		public SearchResult SearchBackward (int fromOffset)
		{
			return textArea.SearchBackward (fromOffset);
		}
		
		/// <summary>
		/// Initiate a pulse at the specified document location
		/// </summary>
		/// <param name="pulseStart">
		/// A <see cref="DocumentLocation"/>
		/// </param>
		public void PulseCharacter (DocumentLocation pulseStart)
		{
			textArea.PulseCharacter (pulseStart);
		}
		
		
		public SearchResult FindNext (bool setSelection)
		{
			return textArea.FindNext (setSelection);
		}
		
		public void StartCaretPulseAnimation ()
		{
			textArea.StartCaretPulseAnimation ();
		}

		public void StopSearchResultAnimation ()
		{
			textArea.StopSearchResultAnimation ();
		}
		
		public void AnimateSearchResult (SearchResult result)
		{
			textArea.AnimateSearchResult (result);
		}

		public SearchResult FindPrevious (bool setSelection)
		{
			return textArea.FindPrevious (setSelection);
		}
		
		public bool Replace (string withPattern)
		{
			return textArea.Replace (withPattern);
		}
		
		public int ReplaceAll (string withPattern)
		{
			return textArea.ReplaceAll (withPattern);
		}
		#endregion

		#region Coordinate transformation
		public DocumentLocation PointToLocation (double xp, double yp)
		{
			return TextViewMargin.PointToLocation (xp, yp);
		}
		
		public DocumentLocation PointToLocation (Cairo.Point p)
		{
			return TextViewMargin.PointToLocation (p);
		}
		
		public DocumentLocation PointToLocation (Cairo.PointD p)
		{
			return TextViewMargin.PointToLocation (p);
		}
		
		public Cairo.Point LocationToPoint (DocumentLocation loc)
		{
			return TextViewMargin.LocationToPoint (loc);
		}
		
		public Cairo.Point LocationToPoint (int line, int column)
		{
			return TextViewMargin.LocationToPoint (line, column);
		}
		
		public Cairo.Point LocationToPoint (int line, int column, bool useAbsoluteCoordinates)
		{
			return TextViewMargin.LocationToPoint (line, column, useAbsoluteCoordinates);
		}
		
		public Cairo.Point LocationToPoint (DocumentLocation loc, bool useAbsoluteCoordinates)
		{
			return TextViewMargin.LocationToPoint (loc, useAbsoluteCoordinates);
		}
		
		public double ColumnToX (DocumentLine line, int column)
		{
			if (line == null)
				throw new ArgumentNullException ("line");
			return TextViewMargin.ColumnToX (line, column);
		}
		
		/// <summary>
		/// Calculates the line number at line start (in one visual line could be several logical lines be displayed).
		/// </summary>
		public int YToLine (double yPos)
		{
			return TextViewMargin.YToLine (yPos);
		}
		
		public double LineToY (int logicalLine)
		{
			return TextViewMargin.LineToY (logicalLine);
		}
		
		public double GetLineHeight (DocumentLine line)
		{
			return TextViewMargin.GetLineHeight (line);
		}
		
		public double GetLineHeight (int logicalLineNumber)
		{
			return TextViewMargin.GetLineHeight (logicalLineNumber);
		}
		#endregion


		public void SetCaretTo (int line, int column)
		{
			textArea.SetCaretTo (line, column);
		}
		
		public void SetCaretTo (int line, int column, bool highlight)
		{
			textArea.SetCaretTo (line, column, highlight);
		}
		
		public void SetCaretTo (int line, int column, bool highlight, bool centerCaret)
		{
			textArea.SetCaretTo (line, column, highlight, centerCaret);
		}
		public event EventHandler<Xwt.MouseMovedEventArgs> BeginHover {
			add { textArea.BeginHover += value; }
			remove { textArea.BeginHover -= value; }
		}

	}
}

