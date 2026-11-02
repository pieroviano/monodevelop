// 
// ContextMenuTreeView.cs
//  
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc. (http://xamarin.com)
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
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components
{
	/// <summary>
	/// TreeView with context menu support.
	/// </summary>
	public class ContextMenuTreeView : Gtk.TreeView
	{
		internal ActionDelegate ActionHandler { get; private set; }
		public ContextMenuTreeView ()
		{
			ActionHandler = new ActionDelegate (this);
			ActionHandler.PerformShowMenu += PerformShowMenu;
			InstallControllers ();
		}

		public ContextMenuTreeView (Gtk.ITreeModel model) : base (model)
		{
			InstallControllers ();
		}

		/// <summary>Invoked with the click that requested the menu, or null when requested from the keyboard.</summary>
		public Action<ClickInfo> DoPopupMenu { get; set; }

		Gtk.TreePath buttonPressPath;
		bool selectOnRelease;

		void InstallControllers ()
		{
			// Capture phase: runs before the tree view's own click gesture, like the GTK 2 OnButtonPressEvent override.
			var click = this.AddClickHandler (OnButtonPressed, OnButtonReleased, 0, Gtk.PropagationPhase.Capture);
			// The click gesture is cancelled when a drag starts: don't do any selection then
			// (useful when the user presses Esc to abort dragging and didn't release the mouse button yet).
			click.Stopped += (s, e) => selectOnRelease = false;

			// GTK 4 has no ::popup-menu signal: Menu key / Shift+F10 open the context menu.
			this.AddKeyPressHandler ((keyval, keycode, state) => {
				var key = (Gdk.Key)keyval;
				if (key == Gdk.Key.Menu || (key == Gdk.Key.F10 && (state & Gdk.ModifierType.ShiftMask) != 0))
					return OnPopupMenu ();
				return false;
			});
		}

		// Workaround for Bug 31712 - Solution pad doesn't refresh properly after resizing application window
		// If the treeview size is modified while the pad is unrealized (autohidden), the treeview
		// doesn't update its internal vertical offset. This can lead to items becoming offset outside the
		// visible area and therefore becoming unreachable. The only way to force the treeview to recalculate
		// this offset is by setting the Vadjustment.Value, but it ignores values the same as the current value.
		// Therefore we simply set it to something slightly different then back again.

		// See also MonoDevelop.Ide.Gui.Components.PadTreeView for same fix.
		bool forceInternalOffsetUpdate;

		protected override void OnUnrealized ()
		{
			base.OnUnrealized ();
			forceInternalOffsetUpdate = true;
		}

		protected override void OnSizeAllocate (int width, int height, int baseline)
		{
			base.OnSizeAllocate (width, height, baseline);
			if (forceInternalOffsetUpdate && IsRealized && Vadjustment != null) {
				forceInternalOffsetUpdate = false;
				var v = Vadjustment.Value;
				int delta = v > 2? 0 : 1;
				Vadjustment.Value = v + delta;
				Vadjustment.Value = v;
			}
		}

		protected override void OnRowActivated (Gtk.TreePath path, Gtk.TreeViewColumn column)
		{
			// This is to work around an issue in ContextMenuTreeView, when we set the
			// SelectFunction to block selection then it doesn't seem to always get
			// properly unset.
			//   https://bugzilla.xamarin.com/show_bug.cgi?id=40469
			Selection.SelectFunction = DefaultTreeSelectFunction;
			base.OnRowActivated (path, column);
		}

		bool GetPathAtWidgetPos (double x, double y, out Gtk.TreePath path)
		{
			ConvertWidgetToBinWindowCoords ((int)x, (int)y, out int bx, out int by);
			return GetPathAtPos (bx, by, out path);
		}

		void OnButtonPressed (ClickInfo evnt)
		{
			selectOnRelease = false;
			if (!evnt.TriggersContextMenu ()) {
				//The tree view's own gesture runs after this one, so expanders keep working. When the
				//expander is clicked SelectFunction is not called so selectOnRelease remains false,
				//which means no selection operation is performed in OnButtonReleased.
				//When Shift is pressed we don't do our magic becasue:
				//a) it works as expected((item is still selected when dragging starts
				//(it's by nature of Shift selecting))
				//b) we would have to simulate Shift selecting in OnButtonReleased
				//which would mean we have to implement all selecting logic...
				//Also notice that our magic is requiered only when item is selected.
				if (GetPathAtWidgetPos (evnt.X, evnt.Y, out buttonPressPath) &&
				    ((evnt.State & Gdk.ModifierType.ShiftMask) == 0) &&
				    Selection.PathIsSelected (buttonPressPath)) {
					this.Selection.SelectFunction = (s, m, p, b) => {
						selectOnRelease = true;
						//Always returning false means we are blocking the tree view's default handling
						//from doing any changes to selectiong we will do changes in OnButtonReleased
						return false;
					};
				} else {
					Selection.SelectFunction = DefaultTreeSelectFunction;
				}
				return;
			}

			//update the selection unless the node is already selected, in which case
			//we don't want to change the selection (deselect multi selection)
			if (!this.IsClickedNodeSelected ((int)evnt.X, (int)evnt.Y)) {
				Selection.SelectFunction = DefaultTreeSelectFunction;
				if (GetPathAtWidgetPos (evnt.X, evnt.Y, out var path)) {
					Selection.UnselectAll ();
					SetCursor (path, null, false);
					Selection.SelectPath (path);
				}
			}

			if (DoPopupMenu != null) {
				evnt.Claim ();
				DoPopupMenu (evnt);
			}
		}

		void OnButtonReleased (ClickInfo evnt)
		{
			Selection.SelectFunction = DefaultTreeSelectFunction;
			Gtk.TreePath buttonReleasePath;
			//If OnButtonPressed attempted on making deselection and dragging was not started
			//check if we are on same item as when we clicked(could be different if dragging is disabled)
			if (selectOnRelease &&
			    buttonPressPath != null &&
			    GetPathAtWidgetPos (evnt.X, evnt.Y, out buttonReleasePath) &&
			    buttonPressPath.Compare (buttonReleasePath) == 0) {

				//Simulate what would happen in OnButtonPressed if we were not blocking selection
				//notice that item is currently 100% selected since this check was performed in OnButtonPressed
				if (Selection.Mode == Gtk.SelectionMode.Multiple &&
				    (evnt.State & Gdk.ModifierType.ControlMask) > 0) {
					Selection.UnselectPath (buttonReleasePath);
				} else {
					//UnselectAll in case multiple were selected we want only our item to be selected now
					//if it was clicked but not dragged
					Selection.UnselectAll ();
					Selection.SelectPath (buttonReleasePath);
				}
				buttonPressPath = null;
			}
			selectOnRelease = false;
		}

		/// <summary>
		/// Force the tree view's SelectFunction to be reset so nodes an be selected. Sometimes a button press
		/// occurs without any corresponding release which prevent a tree node from being selected
		/// by the TreeNodeNavigator.
		/// </summary>
		internal void ClearSelectOnRelease ()
		{
			selectOnRelease = false;
			Selection.SelectFunction = DefaultTreeSelectFunction;
		}

		static bool DefaultTreeSelectFunction (Gtk.TreeSelection selection, Gtk.ITreeModel model, Gtk.TreePath path, bool selected)
		{
			return true;
		}

		void PerformShowMenu (object sender, EventArgs args)
		{
			OnPopupMenu ();
		}

		/// <summary>Keyboard/accessibility request for the context menu (GTK 2 ::popup-menu). Returns true when handled.</summary>
		protected virtual bool OnPopupMenu ()
		{
			if (DoPopupMenu != null) {
				DoPopupMenu (null);
				return true;
			}
			return false;
		}

		bool MultipleNodesSelected ()
		{
			return Selection.GetSelectedRows ().Length > 1;
		}
	}
}
