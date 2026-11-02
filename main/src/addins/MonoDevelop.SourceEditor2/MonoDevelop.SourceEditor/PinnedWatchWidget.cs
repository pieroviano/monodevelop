// 
// PinnedWatchWidget.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using Mono.TextEditor;
using MonoDevelop.Debugger;
using Mono.Debugging.Client;
using Gtk;
using MonoDevelop.Components;

namespace MonoDevelop.SourceEditor
{
	class PinnedWatchWidget : MonoDevelop.Components.BinWidget
	{
		static bool UseNewTreeView = true;

		readonly ObjectValueTreeViewController controller;
		readonly TreeView treeView;

		readonly ObjectValueTreeView valueTree;

		ObjectValue objectValue;
		ScrolledWindow sw;

		MonoTextEditor Editor {
			get; set;
		}
		
		public PinnedWatch Watch {
			get; private set;
		}
		
		public ObjectValue ObjectValue {
			get {
				return objectValue;
			}
			set {
				if (objectValue == value)
					return;

				if (UseNewTreeView) {
					controller.ClearAll ();

					if (value != null)
						controller.AddValue (value);
				} else {
					if (objectValue != null && value != null) {
						valueTree.ReplaceValue (objectValue, value);
					} else {
						valueTree.ClearValues ();
						if (value != null)
							valueTree.AddValue (value);
					}
				}

				objectValue = value;
			}
		}
		
		public PinnedWatchWidget (MonoTextEditor editor, PinnedWatch watch)
		{
			objectValue = watch.Value;
			Editor = editor;
			Watch = watch;

			if (UseNewTreeView) {
				controller = new ObjectValueTreeViewController ();
				controller.AllowEditing = true;

				treeView = (TreeView) controller.GetGtkControl (ObjectValueTreeViewFlags.PinnedWatchFlags);

				controller.PinnedWatch = watch;
				valueTree = null;

				if (objectValue != null)
					controller.AddValue (objectValue);
			} else {
				valueTree = new ObjectValueTreeView ();
				valueTree.AllowAdding = false;
				valueTree.AllowEditing = true;
				valueTree.AllowPinning = true;
				valueTree.HeadersVisible = false;
				valueTree.CompactView = true;
				valueTree.PinnedWatch = watch;
				if (objectValue != null)
					valueTree.AddValue (objectValue);

				treeView = valueTree;
				controller = null;
			}
			
			// the capture phase gets the events before the tree view selects/edits rows
			treeView.AddClickHandler (HandleValueTreeButtonPressEvent, HandleValueTreeButtonReleaseEvent, phase: PropagationPhase.Capture);
			dragGesture = new GestureDrag { PropagationPhase = PropagationPhase.Capture };
			dragGesture.DragUpdate += HandleValueTreeDragUpdate;
			treeView.AddController (dragGesture);

			// grows with the tree up to maxHeight, then scrolls
			const int maxHeight = 240;
			sw = new ScrolledWindow ();
			sw.HscrollbarPolicy = PolicyType.Never;
			sw.VscrollbarPolicy = PolicyType.Automatic;
			sw.PropagateNaturalHeight = true;
			sw.MaxContentHeight = maxHeight;
			sw.Child = treeView;

			var fr = new Frame ();
			fr.Child = sw;
			Child = fr;

			DebuggingService.PausedEvent += HandleDebuggingServicePausedEvent;
			DebuggingService.ResumedEvent += HandleDebuggingServiceResumedEvent;
		}

		readonly GestureDrag dragGesture;

		void HandleDebuggingServiceResumedEvent (object sender, EventArgs e)
		{
			if (UseNewTreeView) {
				controller.ChangeCheckpoint ();
				controller.AllowExpanding = false;
				controller.AllowEditing = false;
			} else {
				valueTree.ChangeCheckpoint ();
				valueTree.AllowEditing = false;
				valueTree.AllowExpanding = false;
			}
		}

		void HandleDebuggingServicePausedEvent (object sender, EventArgs e)
		{
			if (UseNewTreeView) {
				controller.AllowExpanding = true;
				controller.AllowEditing = true;
			} else {
				valueTree.AllowExpanding = true;
				valueTree.AllowEditing = true;
			}
		}
		
		bool destroyedFlag;

		public override void Destroy ()
		{
			if (!destroyedFlag) {
				destroyedFlag = true;
				OnDestroyed ();
			}
			base.Destroy ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && !destroyedFlag) {
				destroyedFlag = true;
				OnDestroyed ();
			}
			base.Dispose (disposing);
		}

		protected virtual void OnDestroyed ()
		{
			DebuggingService.PausedEvent -= HandleDebuggingServicePausedEvent;
			DebuggingService.ResumedEvent -= HandleDebuggingServiceResumedEvent;

			treeView.RemoveController (dragGesture);
		}


		#region Moving PinWatchWidget inside TextArea
		bool mousePressed = false;
		double originX, originY;

		void HandleValueTreeButtonPressEvent (ClickInfo args)
		{
			originX = originY = 0;

			TreePath path;
			TreeViewColumn col;
			int cx, cy;
			treeView.GetPathAtPos ((int)args.X, (int)args.Y, out path, out col, out cx, out cy);
			if (!mousePressed && col != null && treeView.Columns[0] == col) {
				mousePressed = true;
				Editor.TextArea.MoveToTop (this);
			}
		}

		void HandleValueTreeButtonReleaseEvent (ClickInfo args)
		{
			if (mousePressed) {
				mousePressed = false;
			}
		}

		void HandleValueTreeDragUpdate (object o, DragUpdateArgs args)
		{
			if (mousePressed) {
				// the drag gesture reports the offset from the drag start: apply the delta since the last update
				Watch.OffsetX += (int)(args.OffsetX - originX);
				Watch.OffsetY += (int)(args.OffsetY - originY);

				originX = args.OffsetX;
				originY = args.OffsetY;
			}
		}
		#endregion
	}
}