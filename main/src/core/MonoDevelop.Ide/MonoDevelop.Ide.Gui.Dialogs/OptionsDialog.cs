// OptionsDialog.cs
//
// Author:
//   Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2008 Novell, Inc (http://www.novell.com)
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
//
//

using System;
using System.Collections.Generic;
using Gtk;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide.Extensions;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Components;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Ide.Gui.Dialogs
{
	
	public partial class OptionsDialog : IdeDialog
	{
		Gtk.Box mainHBox;
		Gtk.TreeView tree;
		Gtk.Label labelTitle;
		Gtk.Box pageFrame;
		Gtk.Button buttonCancel;
		Gtk.Button buttonOk;
		OptionsDialogHeader imageHeader;
		Gtk.Widget textHeader;

		protected TreeStore store;
		Dictionary<OptionsDialogSection, SectionPage> pages = new Dictionary<OptionsDialogSection, SectionPage> ();
		Dictionary<OptionsPanelNode, PanelInstance> panels = new Dictionary<OptionsPanelNode,PanelInstance> ();
		object mainDataObject;
		string extensionPath;
		ExtensionContext extensionContext;
		HashSet<object> modifiedObjects = new HashSet<object> ();
		bool removeEmptySections;
		
		const string emptyCategoryIcon = "md-prefs-generic";
		const StockIconSize treeIconSize = StockIconSize.Menu;
		const StockIconSize headerIconSize = StockIconSize.Button;
		
		public object DataObject {
			get {
				return mainDataObject;
			}
		}

		public ExtensionContext ExtensionContext {
			get {
				return extensionContext;
			}
		}
		
		public HashSet<object> ModifiedObjects {
			get { return modifiedObjects; }
		}

		public OptionsDialog (string extensionPath): this (null, null, extensionPath)
		{
		}
		
		public OptionsDialog (MonoDevelop.Components.Window parentWindow, object dataObject, string extensionPath) : this (parentWindow, dataObject, extensionPath, true)
		{}
		
		public OptionsDialog (MonoDevelop.Components.Window parentWindow, object dataObject, string extensionPath, bool removeEmptySections)
		{
			buttonCancel = DesignerLayout.Button (GettextCatalog.GetString ("_Cancel"));
			buttonCancel.SetCommonAccessibilityAttributes ("Dialogs.Options.Cancel", (string)null, GettextCatalog.GetString ("Close the options dialog and discard any changes"));
			buttonCancel.Clicked += (sender, e) => Respond (ResponseType.Cancel);

			buttonOk = DesignerLayout.Button (GettextCatalog.GetString ("_OK"));
			buttonOk.SetCommonAccessibilityAttributes ("Dialogs.Options.Ok", (string)null, GettextCatalog.GetString ("Close the options dialog and keep the changes"));
			// The OK button validates and applies the changes before responding, so it is not a response action widget
			buttonOk.Clicked += OnButtonOkClicked;

			if (parentWindow != null)
				TransientFor = parentWindow;

			mainHBox = DesignerLayout.HBox (0);
			tree = new TreeView ();
			tree.SetCommonAccessibilityAttributes ("Dialogs.Options.Categories", (string)null, GettextCatalog.GetString ("The categories of options that are available in this dialog"));

			var sw = new ScrolledWindow ();
			sw.Child = tree;
			sw.SetPolicy (PolicyType.Never, PolicyType.Automatic);
			sw.HasFrame = false;
			sw.Vexpand = true;

			var fboxTree = new HeaderBox ();
			fboxTree.SetMargins (0, 1, 0, 1);
			fboxTree.SetPadding (0, 0, 0, 0);
			fboxTree.BackgroundColor = new Gdk.RGBA { Red = 1, Green = 1, Blue = 1, Alpha = 1 };
			fboxTree.Child = sw;
			DesignerLayout.Pack (mainHBox, fboxTree);

			Realized += delegate {
				if (tree.StyleContext.LookupColor ("theme_base_color", out Gdk.RGBA baseColor))
					fboxTree.BackgroundColor = baseColor;
			};

			var vbox = DesignerLayout.VBox (0);
			DesignerLayout.Pack (mainHBox, vbox, true);
			var headerBox = DesignerLayout.HBox (6);

			labelTitle = new Label ();
			labelTitle.SetCommonAccessibilityAttributes ("Dialogs.Options.PageTitle", (string)null, null);
			labelTitle.Xalign = 0;
			DesignerLayout.Margin (labelTitle, 12);
			textHeader = labelTitle;
			DesignerLayout.Pack (headerBox, textHeader, true);

			imageHeader = new OptionsDialogHeader ();
			imageHeader.Visible = false;
			var imageHeaderWidget = imageHeader.ToGtkWidget ();
			DesignerLayout.Pack (headerBox, imageHeaderWidget);

			var fboxHeader = new HeaderBox ();
			fboxHeader.SetMargins (0, 1, 0, 0);
			fboxHeader.Child = headerBox;
			Realized += delegate {
				if (StyleContext.LookupColor ("theme_bg_color", out Gdk.RGBA bg)) {
					var c = bg.ToXwtColor ();
					c.Light += 0.09;
					fboxHeader.BackgroundColor = c.ToGdkColor ();
				}
			};
			DesignerLayout.Pack (vbox, fboxHeader);

			pageFrame = DesignerLayout.HBox (0);
			var fbox = new HeaderBox ();
			fbox.SetMargins (0, 1, 0, 0);
			fbox.ShowTopShadow = true;
			fbox.Child = pageFrame;
			DesignerLayout.Pack (vbox, fbox, true);

			DesignerLayout.Pack (this.ContentArea, mainHBox, true);
			var buttonBar = DialogButtonBar.Add (this);
			DesignerLayout.Pack (buttonBar, buttonCancel);
			DesignerLayout.Pack (buttonBar, buttonOk);
			DefaultWidget = buttonOk;

			this.removeEmptySections = removeEmptySections;
			extensionContext = AddinManager.CreateExtensionContext ();
			
			this.mainDataObject = dataObject;
			this.extensionPath = extensionPath;
			
			ImageService.EnsureStockIconIsLoaded (emptyCategoryIcon);

			store = new TreeStore (typeof(OptionsDialogSection));
			tree.Model = store;
			tree.HeadersVisible = false;

			// Column 0 is used to add some padding at the left of the expander
			TreeViewColumn col0 = new TreeViewColumn ();
			col0.MinWidth = 6;
			tree.AppendColumn (col0);

			TreeViewColumn col = new TreeViewColumn ();
			var crp = new CellRendererImage ();
			col.PackStart (crp, false);
			col.SetCellDataFunc (crp, PixbufCellDataFunc);
			var crt = new CellRendererText ();
			col.PackStart (crt, true);
			col.SetCellDataFunc (crt, TextCellDataFunc);
			tree.AppendColumn (col);

			tree.ExpanderColumn = col;
			
			tree.Selection.Changed += OnSelectionChanged;
			
			InitializeContext (extensionContext);
			
			FillTree ();
			ExpandCategories ();
			RestoreLastPanel ();
			SetDefaultSize (960, 680);
			Response += OnDialogResponse;
		}

		static OptionsDialogSection GetSectionValue (ITreeModel model, TreeIter iter)
		{
			var value = new GLib.Value ();
			model.GetValue (iter, 0, ref value);
			return (OptionsDialogSection) value.Val;
		}

		static void PixbufCellDataFunc (TreeViewColumn col, CellRenderer cell, ITreeModel model, TreeIter iter)
		{
			TreeIter parent;
			bool toplevel = !model.IterParent (out parent, iter);
			
			var crp = (CellRendererImage) cell;
			crp.Visible = !toplevel;
			
			if (toplevel) {
				return;
			}
			
			var section = GetSectionValue (model, iter);
			
			//HACK: The mimetype panels can't register a single fake stock ID for all the possible image size.
			// Instead, give this some awareness of the mime system.
			var mimeSection = section as MonoDevelop.Ide.Projects.OptionPanels.MimetypeOptionsDialogSection;
			if (mimeSection != null && !string.IsNullOrEmpty (mimeSection.MimeType)) {
				var pix = IdeServices.DesktopService.GetIconForType (mimeSection.MimeType, treeIconSize);
				if (pix != null) {
					crp.Image = pix;
				} else {
					crp.Image = ImageService.GetIcon (emptyCategoryIcon, treeIconSize);
				}
			} else {
				string icon = section.Icon.IsNull? emptyCategoryIcon : section.Icon.ToString ();
				crp.Image = ImageService.GetIcon (icon, treeIconSize);
			}
		}
		
		static void TextCellDataFunc (TreeViewColumn col, CellRenderer cell, ITreeModel model, TreeIter iter)
		{
			TreeIter parent;
			bool toplevel = !model.IterParent (out parent, iter);
			
			var crt = (CellRendererText) cell;
			var section = GetSectionValue (model, iter);
			
			if (toplevel) {
				crt.Markup = "<b>" + GLib.Markup.EscapeText (section.Label) + "</b>";
			} else {
				crt.Text = section.Label;
			}
		}
		
		protected Control MainBox {
			get { return pageFrame; }
		}
		
		protected void ExpandCategories ()
		{
			TreeIter it;
			if (store.GetIterFirst (out it)) {
				tree.Selection.SelectIter (it);
				do {
					tree.ExpandRow (store.GetPath (it), false);
				} while (store.IterNext (ref it));
			}
		}
		
		public void FillTree ()
		{
			DetachWidgets ();
			pages.Clear ();
			store.Clear ();
			foreach (OptionsDialogSection section in extensionContext.GetExtensionNodes (extensionPath)) {
				AddSection (section, mainDataObject);
			}
		}
		
		protected virtual void InitializeContext (ExtensionContext extensionContext)
		{
		}

		public override void Destroy ()
		{
			foreach (PanelInstance pi in panels.Values) {
				if (pi.Widget != null)
					DestroyWidget (pi.Widget);
				else {
					var widget = pi.Panel as Gtk.Widget;
					if (widget != null) {
						//TODO: Panels shouldn't inherit/implement view directly
						//Mostly because it will constrcut some UI(in constrcutor calling this.Build())
						//on Preferences opening that should be defereded until CreatePanelWidget call
						DestroyWidget (widget);
					}
				}
				IDisposable disp = pi.Panel as IDisposable;
				if (disp != null)
					disp.Dispose ();
			}
			base.Destroy ();
		}

		/// <summary>Detaches <paramref name="widget"/> from its container and destroys it.</summary>
		static void DestroyWidget (Gtk.Widget widget)
		{
			RemoveFromParent (widget);
			widget.Destroy ();
		}

		/// <summary>Detaches <paramref name="widget"/> from whatever container currently holds it (GTK 4 has no Container.Remove).</summary>
		static void RemoveFromParent (Gtk.Widget widget)
		{
			switch (widget.Parent) {
			case null:
				return;
			case Gtk.Box box:
				box.Remove (widget);
				break;
			case ContainerWidget container:
				container.RemoveChild (widget);
				break;
			default:
				widget.Unparent ();
				break;
			}
		}

		/// <summary>
		/// Gets a specific dialog panel from a given section.
		/// </summary>
		/// <returns>The panel, or null if the panel wasn't found.</returns>
		/// <param name="id">The section id.</param>
		/// <typeparam name="T">The type of the dialog panel.</typeparam>
		public T GetPanel<T> (string id) where T : class, IOptionsPanel
		{
			foreach (OptionsDialogSection section in pages.Keys) {
				if (section.Id == id) {
					SectionPage page;
					if (!pages.TryGetValue (section, out page))
						return null;
					foreach (var panel in page.Panels) {
						var result = panel.Panel as T;
						if (result != null)
							return result;
					}
				}
			}
			return null;
		}

		public void SelectPanel (string id)
		{
			foreach (OptionsDialogSection section in pages.Keys) {
				if (section.Id == id) {
					ShowPage (section);
					return;
				}
			}
		}

		internal void ExpandChildren (IOptionsPanel parent)
		{
			foreach (SectionPage page in pages.Values) {
				foreach (PanelInstance pi in page.Panels) {
					if (pi.Panel == parent) {
						tree.ExpandToPath (store.GetPath (page.Iter));
						tree.ExpandRow (store.GetPath (page.Iter), false);
						return;
					}
				}
			}
		}

		internal void AddChildSection (IOptionsPanel parent, OptionsDialogSection section, object dataObject)
		{
			foreach (SectionPage page in pages.Values) {
				foreach (PanelInstance pi in page.Panels) {
					if (pi.Panel == parent) {
						AddSection (page.Iter, section, dataObject);
						return;
					}
				}
			}
			throw new InvalidOperationException ("Parent options panel not found in the dialog.");
		}
		
		internal void RemoveSection (OptionsDialogSection section)
		{
			SectionPage page;
			if (pages.TryGetValue (section, out page))
				RemoveSection (page.Iter);
		}
		
		void RemoveSection (Gtk.TreeIter it)
		{
			Gtk.TreeIter ci;
			if (store.IterChildren (out ci, it)) {
				do {
					RemoveSection (ci);
				} while (store.IterNext (ref ci));
			}
			
			// If the this panel is the selection, select the parent before removing
			Gtk.TreeIter itsel;
			if (tree.Selection.GetSelected (out itsel)) {
				if (store.GetPath (itsel).Equals (store.GetPath (it))) {
					Gtk.TreeIter pi;
					if (store.IterParent (out pi, it))
						tree.Selection.SelectIter (pi);
				}
			}
			OptionsDialogSection section = GetSectionValue (store, it);
			if (section != null) {
				SectionPage page;
				if (pages.TryGetValue (section, out page)) {
					foreach (PanelInstance pi in page.Panels) {
						panels.Remove (pi.Node);
						IDisposable d = pi.Panel as IDisposable;
						if (d != null)
							d.Dispose ();
					}
					pages.Remove (section);
					if (page.Widget != null)
						DestroyWidget (page.Widget);
				}
			}
			store.Remove (ref it);
		}
		
		internal TreeIter AddSection (OptionsDialogSection section, object dataObject)
		{
			return AddSection (TreeIter.Zero, section, dataObject);
		}
		
		internal TreeIter AddSection (TreeIter parentIter, OptionsDialogSection section, object dataObject)
		{
			TreeIter it;
			if (parentIter.Equals (TreeIter.Zero)) {
				it = store.AppendValues (section);
			} else {
				it = store.AppendValues (parentIter,section);
			}
			
			if (!section.CustomNode)
				AddChildSections (it, section, dataObject);
			
			// Remove the section if it doesn't have children nor panels
			SectionPage page = CreatePage (it, section, dataObject);
			TreeIter cit;
			if (removeEmptySections && page.Panels.Count == 0 && !store.IterChildren (out cit, it)) {
				store.Remove (ref it);
				pages.Remove (section);
				return TreeIter.Zero;
			}
			return it;
		}
		
		internal virtual void AddChildSections (TreeIter parentIter, OptionsDialogSection section, object dataObject)
		{
			foreach (ExtensionNode nod in section.ChildNodes) {
				if (nod is OptionsDialogSection)
					AddSection (parentIter, (OptionsDialogSection) nod, dataObject);
			}
		}
		
		void DetachWidgets ()
		{
			RemovePageFrameChildren ();

			foreach (SectionPage sp in pages.Values) {
				foreach (PanelInstance pi in sp.Panels) {
					if (pi.Widget != null)
						RemoveFromParent (pi.Widget);
				}
				if (sp.Widget != null) {
					DestroyWidget (sp.Widget);
					sp.Widget = null;
				}
			}
		}
		
		/// <summary>Shows or hides the panel widgets directly hosted by a page widget (the box of panels, or the tabs of a notebook).</summary>
		static void SetPanelsVisible (Gtk.Widget pageWidget, bool visible)
		{
			if (pageWidget is Notebook notebook) {
				for (int n = 0; n < notebook.NPages; n++)
					notebook.GetNthPage (n).Visible = visible;
				return;
			}
			var child = pageWidget.FirstChild;
			while (child != null) {
				child.Visible = visible;
				child = child.NextSibling;
			}
		}

		void RemovePageFrameChildren ()
		{
			var child = pageFrame.FirstChild;
			while (child != null) {
				var next = child.NextSibling;
				pageFrame.Remove (child);
				child = next;
			}
		}

		void OnDialogResponse (object sender, ResponseArgs args)
		{
			DetachWidgets ();
		}
		
		protected virtual bool ValidateChanges ()
		{
			foreach (SectionPage sp in pages.Values) {
				if (sp.Widget == null)
					continue;
				foreach (PanelInstance pi in sp.Panels) {
					if (!pi.Panel.ValidateChanges ())
						return false; // Not valid
				}
			}
			return true;
		}
		
		protected virtual void ApplyChanges ()
		{
			foreach (SectionPage sp in pages.Values) {
				if (sp.Widget == null)
					continue;
				foreach (PanelInstance pi in sp.Panels)
					pi.Panel.ApplyChanges ();
			}
		}
		
		
		void OnSelectionChanged (object s, EventArgs a)
		{
			TreeIter it;
			if (tree.Selection.GetSelected (out it)) {
				OptionsDialogSection section = GetSectionValue (store, it);
				ShowPage (section);

				this.UseNativeContextMenus ();
			}
		}
		
		bool HasVisiblePanel (OptionsDialogSection section)
		{
			SectionPage page;
			if (!pages.TryGetValue (section, out page))
				return false;
			if (page.Panels.Count > 0)
				return true;
			foreach (ExtensionNode node in section.ChildNodes) {
				if (node is OptionsDialogSection) {
					if (HasVisiblePanel ((OptionsDialogSection) node))
						return true;
				}
			}
			return false;
		}
		
		internal void ShowPage (OptionsDialogSection section, bool forceExpand = false)
		{
			if (!IsRealized) {
				// Defer this until the dialog is realized due to the sizing logic in CreatePageWidget.
				EventHandler deferredShowPage = null;
				deferredShowPage = delegate {
					ShowPage (section, true);
					Realized -= deferredShowPage;
				};
				Realized += deferredShowPage;
				return;
			}
			SectionPage page;
			if (!pages.TryGetValue (section, out page))
				return;
			
			if (page.Panels.Count == 0) {
				foreach (ExtensionNode node in section.ChildNodes) {
					if (node is OptionsDialogSection) {
						if (HasVisiblePanel ((OptionsDialogSection) node)) {
							ShowPage ((OptionsDialogSection) node);
							return;
						}
					}
				}
			}
			// Hide the panels of the previous page so that they get their Hidden event (e.g. to save pending changes)
			var previous = pageFrame.FirstChild;
			while (previous != null) {
				SetPanelsVisible (previous, false);
				previous = previous.NextSibling;
			}
			RemovePageFrameChildren ();

			if (page.Widget == null)
				CreatePageWidget (page);

			if (section.HeaderImage == null) {
				labelTitle.Markup = "<span weight=\"bold\" size=\"large\">" + GLib.Markup.EscapeText (section.HeaderLabel) + "</span>";
				textHeader.Visible = true;
				imageHeader.Visible = false;
			} else {
				imageHeader.SetImage (section.HeaderImage, section.HeaderFillerImageResource);
				imageHeader.Visible = true;
				textHeader.Visible = false;
			}
			RemoveFromParent (page.Widget);
			page.Widget.Hexpand = true;
			page.Widget.Vexpand = true;
			pageFrame.Append (page.Widget);

			// Ensures that the Shown event is fired for each panel
			SetPanelsVisible (page.Widget, true);

			//HACK: weird bug - switching page away and back selects last tab. should preserve the selection.
			if (page.Widget is Notebook nb)
				nb.Page = 0;

			if (!IdeServices.DesktopService.AccessibilityInUse && !IdeServices.DesktopService.AccessibilityKeyboardFocusInUse || forceExpand) {
				// Don't automatically expand trees if using accessibility
				// as it can be confusing with screen readers
				tree.ExpandToPath (store.GetPath (page.Iter));
			}
			tree.Selection.SelectIter (page.Iter);
		}
		
		SectionPage CreatePage (TreeIter it, OptionsDialogSection section, object dataObject)
		{
			SectionPage page;
			if (pages.TryGetValue (section, out page))
				return page;
			
			page = new SectionPage ();
			page.Iter = it;
			page.Panels = new List<PanelInstance> ();
			pages [section] = page;
			
			List<OptionsPanelNode> nodes = new List<OptionsPanelNode> ();
			if (!string.IsNullOrEmpty (section.TypeName) || section.CustomNode)
				nodes.Add (section);
			
			if (!section.CustomNode) {
				foreach (ExtensionNode nod in section.ChildNodes) {
					OptionsPanelNode node = nod as OptionsPanelNode;
					if (node != null && !(node is OptionsDialogSection))
						nodes.Add (node);
				}
			}

			foreach (OptionsPanelNode node in nodes.ToArray ()) {
				if (!string.IsNullOrEmpty (node.Replaces)) {
					var replaced = nodes.FindIndex (n => n.Id == node.Replaces);
					if (replaced != -1) {
						nodes.Remove (node);
						nodes [replaced] = node;
					}
				}
			}

			foreach (OptionsPanelNode node in nodes)
			{
				PanelInstance pi = null;
				if (panels.TryGetValue (node, out pi)) {
					if (pi.DataObject == dataObject) {
						// The panel can be reused
						if (!pi.Panel.IsVisible ())
							continue;
					} else {
						// If the data object has changed, this panel instance can't be
						// reused anymore. Destroy it.
						panels.Remove (node);
						if (pi.Widget != null)
							DestroyWidget (pi.Widget);
						IDisposable d = pi.Panel as IDisposable;
						if (d != null)
							d.Dispose ();
						pi = null;
					}
				}
				if (pi == null) {
					IOptionsPanel panel;
					try {
						panel = node.CreatePanel ();
					} catch (Exception e) {
						LoggingService.LogInternalError ("Error while creating options panel: " + node.Id, e);
						continue;
					}
					if (panel == null) {
						LoggingService.LogWarning ("Error panel == null: " + node.Id);
						continue;
					}
					pi = new PanelInstance ();
					pi.Panel = panel;
					pi.Node = node;
					pi.DataObject = dataObject;
					page.Panels.Add (pi);
					panel.Initialize (this, dataObject);
					if (!panel.IsVisible ()) {
						page.Panels.Remove (pi);
						continue;
					}
					panels [node] = pi;
				} else
					page.Panels.Add (pi);
			}
			return page;
		}
		
		void CreatePageWidget (SectionPage page)
		{
			List<PanelInstance> boxPanels = new List<PanelInstance> ();
			List<PanelInstance> tabPanels = new List<PanelInstance> ();
			
			foreach (PanelInstance pi in page.Panels) {
				if (pi.Widget == null) {
					try {
						pi.Widget = pi.Panel.CreatePanelWidget ();
					} catch (Exception e) {
						LoggingService.LogInternalError ("Error while creating panel widget for: " + pi.Node.Id, e);
						continue;
					}
					if (pi.Widget == null)
						continue;

					//HACK: work around bug 469427 - broken themes match on widget names
					if (pi.Widget.Name.IndexOf ("Panel") > 0)
						pi.Widget.Name = pi.Widget.Name.Replace ("Panel", "_");
				}
				else
					RemoveFromParent (pi.Widget);
					
				if (pi.Node.Grouping == PanelGrouping.Tab)
					tabPanels.Add (pi);
				else
					boxPanels.Add (pi);
			}
			
			// Try to fit panels with grouping=box or auto in the main page.
			// If they don't fit. Move auto panels to its own tab page.
			
			int mainPageSize;
			bool fits;
			do {
				PanelInstance lastAuto = null;
				mainPageSize = 0;
				foreach (PanelInstance pi in boxPanels) {
					if (pi.Node.Grouping == PanelGrouping.Auto)
						lastAuto = pi;
					// GTK 4 can measure a widget that is not parented yet
					pi.Widget.Measure (Orientation.Vertical, -1, out int minHeight, out _, out _, out _);
					mainPageSize += minHeight + 6;
				}
				fits = mainPageSize <= pageFrame.Height;
				if (!fits) {
					if (lastAuto != null && boxPanels.Count > 1) {
						boxPanels.Remove (lastAuto);
						tabPanels.Insert (0, lastAuto);
					} else {
						fits = true;
					}
				}
			} while (!fits);
			
			var box = DesignerLayout.VBox (12);
			for (int n=0; n<boxPanels.Count; n++) {
				if (n != 0)
					DesignerLayout.Pack (box, new Separator (Orientation.Horizontal));
				PanelInstance pi = boxPanels [n];
				if (pi.Node.Fill)
					pi.Widget.Vexpand = true;
				box.Append (pi.Widget);
			}
			
			DesignerLayout.Margin (box, 12);

			if (tabPanels.Count > 0) {
				Gtk.Notebook nb = new Notebook ();
				if (boxPanels.Count > 0) {
					Gtk.Label blab = new Gtk.Label (GettextCatalog.GetString ("General"));
					DesignerLayout.Margin (box, 9);
					nb.InsertPage (box, blab, -1);
				}
				foreach (PanelInstance pi in tabPanels) {
					Gtk.Label lab = new Gtk.Label (pi.Node.Label);
					var a = new BinWidget ();
					a.Padding = new Gtk.Border { Left = 9, Right = 9, Top = 9, Bottom = 9 };
					a.Child = pi.Widget;
					nb.InsertPage (a, lab, -1);
				}
				page.Widget = nb;
				DesignerLayout.Margin (nb, 12);
			} else {
				page.Widget = box;
			}
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			// Validate changes before saving
			if (!ValidateChanges ())
				return;

			// Now save
			try {
				ApplyChanges ();
			} catch (Exception ex) {
				LoggingService.LogError ("Error saving options changes", ex);
				MessageService.ShowError (null, GettextCatalog.GetString ("There was an error saving the changes"), null, null, false);
			}

			StoreLastPanel ();

			if (DataObject != null)
				modifiedObjects.Add (DataObject);
			
			this.Respond (ResponseType.Ok);
		}

		#region Restore

		void RestoreLastPanel ()
		{
			string id = PropertyService.Get<string> (extensionPath + "-lastPanel");
			if (string.IsNullOrEmpty (id)) {
				return;
			}

			SelectPanel (id);
		}

		void StoreLastPanel ()
		{
			TreeIter it;
			if (tree.Selection.GetSelected (out it)) {
				OptionsDialogSection section = GetSectionValue (store, it);
				PropertyService.Set (extensionPath + "-lastPanel", section.Id);
			}
		}

		#endregion

		class PanelInstance
		{
			public IOptionsPanel Panel;
			public Gtk.Widget Widget;
			public OptionsPanelNode Node;
			public object DataObject;
		}
		
		class SectionPage
		{
			public List<PanelInstance> Panels;
			public Gtk.Widget Widget;
			public Gtk.TreeIter Iter;
		}
	}

	class OptionsDialogHeader: Xwt.Canvas
	{
		Xwt.Drawing.Image image, filler;

		public void SetImage (Xwt.Drawing.Image image, Xwt.Drawing.Image filler)
		{
			this.image = image;
			this.filler = filler;
			QueueDraw ();
		}

		protected override void OnDraw (Xwt.Drawing.Context ctx, Xwt.Rectangle dirtyRect)
		{
			base.OnDraw (ctx, dirtyRect);
			if (image != null) {
				ctx.DrawImage (image, 0, 0);
				if (filler != null) {
					int fillCount = (int)Math.Ceiling ((Bounds.Width - image.Width) / filler.Width);
					double x = image.Width;
					while ((fillCount--) > 0) {
						ctx.DrawImage (filler, x, 0);
						x += filler.Width;
					}
				}
			}
		}

		protected override Xwt.Size OnGetPreferredSize (Xwt.SizeConstraint widthConstraint, Xwt.SizeConstraint heightConstraint)
		{
			return new Xwt.Size (0, image != null ? image.Height : 0);
		}
	}
}
