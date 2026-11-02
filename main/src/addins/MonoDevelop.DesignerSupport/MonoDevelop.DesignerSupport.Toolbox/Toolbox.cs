/* 
 * Toolbox.cs - A toolbox widget
 * 
 * Authors: 
 *  Michael Hutchinson <m.j.hutchinson@gmail.com>
 *  
 * Copyright (C) 2005 Michael Hutchinson
 *
 * This sourcecode is licenced under The MIT License:
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Gtk;
using System.Drawing.Design;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.AtkCocoaHelper;
using MonoDevelop.Components.Commands;
using MonoDevelop.Components.Docking;
using MonoDevelop.Ide;
using MonoDevelop.Components;

namespace MonoDevelop.DesignerSupport.Toolbox
{
	public class Toolbox : Gtk.Box, IPropertyPadProvider, IToolboxConfiguration
	{
		ToolboxService toolboxService;
		
		ItemToolboxNode selectedNode;
	//	Hashtable expandedCategories = new Hashtable ();
		
		ToolboxWidget toolboxWidget;
		ScrolledWindow scrolledWindow;
		
		ToggleButton catToggleButton;
		ToggleButton compactModeToggleButton;
		MonoDevelop.Components.SearchEntry filterEntry;
		MonoDevelop.Ide.Gui.PadFontChanger fontChanger;
		IPadWindow container;
		Dictionary<string,int> categoryPriorities = new Dictionary<string, int> ();
		Button toolboxAddButton;
		Gtk.DragSource dragSource;
		
		public Toolbox (ToolboxService toolboxService, IPadWindow container) : base (Gtk.Orientation.Vertical, 0)
		{
			this.toolboxService = toolboxService;
			this.container = container;
			
			#region Toolbar
			DockItemToolbar toolbar = container.GetToolbar (DockPositionType.Top);
		
			filterEntry = new MonoDevelop.Components.SearchEntry ();
			filterEntry.Ready = true;
			filterEntry.HasFrame = true;
			filterEntry.WidthRequest = 150;
			filterEntry.Changed += new EventHandler (filterTextChanged);
			filterEntry.SetCommonAccessibilityAttributes ("Toolbox.SearchEntry", GettextCatalog.GetString ("Search Toolbox"),
				GettextCatalog.GetString ("Enter a term to search for it in the toolbox"));

			toolbar.Add (filterEntry, true);
			
			catToggleButton = new ToggleButton ();
			catToggleButton.Child = new ImageView (Ide.Gui.Stock.GroupByCategory, StockIconSize.Menu);
			catToggleButton.Toggled += new EventHandler (toggleCategorisation);
			catToggleButton.TooltipText = GettextCatalog.GetString ("Show categories");
			catToggleButton.SetCommonAccessibilityAttributes ("Toolbox.ShowCategories", GettextCatalog.GetString ("Show Categories"),
				GettextCatalog.GetString ("Toggle to show categories"));
			toolbar.Add (catToggleButton);
			
			compactModeToggleButton = new ToggleButton ();
			compactModeToggleButton.Child = new ImageView (ImageService.GetIcon ("md-compact-display", StockIconSize.Menu));
			compactModeToggleButton.Toggled += new EventHandler (ToggleCompactMode);
			compactModeToggleButton.TooltipText = GettextCatalog.GetString ("Use compact display");
			compactModeToggleButton.SetCommonAccessibilityAttributes ("Toolbox.CompactButton", GettextCatalog.GetString ("Compact Layout"),
				GettextCatalog.GetString ("Toggle for toolbox to use compact layout"));
			toolbar.Add (compactModeToggleButton);
	
			toolboxAddButton = new Button { Child = new ImageView (Ide.Gui.Stock.Add, StockIconSize.Menu) };
			toolbar.Add (toolboxAddButton);
			toolboxAddButton.TooltipText = GettextCatalog.GetString ("Add toolbox items");
			toolboxAddButton.Clicked += new EventHandler (toolboxAddButton_Clicked);
			toolboxAddButton.SetCommonAccessibilityAttributes ("Toolbox.Add", GettextCatalog.GetString ("Add"),
				GettextCatalog.GetString ("Add toolbox items"));


			#endregion
			
			toolboxWidget = new ToolboxWidget ();
			toolboxWidget.SetCommonAccessibilityAttributes ("Toolbox.Toolbox", GettextCatalog.GetString ("Toolbox Items"),
				GettextCatalog.GetString ("The toolbox items"));

			toolboxWidget.SelectedItemChanged += delegate {
				selectedNode = this.toolboxWidget.SelectedItem != null ? this.toolboxWidget.SelectedItem.Tag as ItemToolboxNode : null;
				toolboxService.SelectItem (selectedNode);
			};
			dragSource = new Gtk.DragSource { Actions = Gdk.DragAction.Copy | Gdk.DragAction.Move };
			dragSource.Prepare += OnDragPrepare;
			this.toolboxWidget.AddController (dragSource);
			this.toolboxWidget.ActivateSelectedItem += delegate {
				toolboxService.UseSelectedItem ();
			};
			
			fontChanger = new MonoDevelop.Ide.Gui.PadFontChanger (toolboxWidget, toolboxWidget.SetCustomFont, toolboxWidget.QueueResize);
			
			this.toolboxWidget.DoPopupMenu = ShowPopup;
			scrolledWindow = new MonoDevelop.Components.CompactScrolledWindow ();
			scrolledWindow.Vexpand = true;
			scrolledWindow.Hexpand = true;
			base.Append (scrolledWindow);
			
			//Initialise self
			scrolledWindow.HasFrame = false;
			scrolledWindow.SetPolicy (PolicyType.Never, PolicyType.Automatic);
			scrolledWindow.WidthRequest = 150;
			scrolledWindow.Child = this.toolboxWidget;
			this.toolboxWidget.ScrollAdjustment = scrolledWindow.Vadjustment;
			
			//update view when toolbox service updated
			toolboxService.ToolboxContentsChanged += delegate { Refresh (); };
			toolboxService.ToolboxConsumerChanged += delegate { Refresh (); };
			Refresh ();
			
			//set initial state
			this.toolboxWidget.ShowCategories = catToggleButton.Active = true;
			compactModeToggleButton.Active = MonoDevelop.Core.PropertyService.Get ("ToolboxIsInCompactMode", false);
			this.toolboxWidget.IsListMode  = !compactModeToggleButton.Active;
		}
		
		#region Toolbar event handlers
		
		void ToggleCompactMode (object sender, EventArgs e)
		{
			this.toolboxWidget.IsListMode = !compactModeToggleButton.Active;
			MonoDevelop.Core.PropertyService.Set ("ToolboxIsInCompactMode", compactModeToggleButton.Active);

			if (compactModeToggleButton.Active) {
				compactModeToggleButton.SetCommonAccessibilityAttributes (null, GettextCatalog.GetString ("Full Layout"), GettextCatalog.GetString ("Toggle for toolbox to use full layout"));
			} else {
				compactModeToggleButton.SetCommonAccessibilityAttributes (null, GettextCatalog.GetString ("Compact Layout"), GettextCatalog.GetString ("Toggle for toolbox to use compact layout"));
			}
		}
		
		void toggleCategorisation (object sender, EventArgs e)
		{
			this.toolboxWidget.ShowCategories = catToggleButton.Active;
			if (catToggleButton.Active) {
				catToggleButton.SetCommonAccessibilityAttributes (null, GettextCatalog.GetString ("Hide Categories"), GettextCatalog.GetString ("Toggle to hide toolbox categories"));
			} else {
				catToggleButton.SetCommonAccessibilityAttributes (null, GettextCatalog.GetString ("Show Categories"), GettextCatalog.GetString ("Toggle to show toolbox categories"));
			}
		}
		
		void filterTextChanged (object sender, EventArgs e)
		{
			refilter ();
		}

		void refilter ()
		{
			foreach (ToolboxWidgetCategory cat in toolboxWidget.Categories) {
				bool hasVisibleChild = false;
				foreach (ToolboxWidgetItem child in cat.Items) {
					child.IsVisible = ((ItemToolboxNode)child.Tag).Filter (filterEntry.Entry.Text);
					hasVisibleChild |= child.IsVisible;
				}
				cat.IsVisible = hasVisibleChild;
			}
			toolboxWidget.QueueDraw ();
			toolboxWidget.QueueResize ();
		}
		
		async void toolboxAddButton_Clicked (object sender, EventArgs e)
		{
			await toolboxService.AddUserItems ();
		}
		
		void ShowPopup (ClickInfo evt)
		{
			if (!AllowEditingComponents)
				return;
			CommandEntrySet eset = IdeApp.CommandService.CreateCommandEntrySet ("/MonoDevelop/DesignerSupport/ToolboxItemContextMenu");
			if (evt != null) {
				IdeApp.CommandService.ShowContextMenu (toolboxWidget, evt, eset, this);
			} else {
				IdeApp.CommandService.ShowContextMenu (toolboxWidget, 0, 0, eset, this);
			}
		}

		[CommandHandler (MonoDevelop.Ide.Commands.EditCommands.Delete)]
		internal void OnDeleteItem ()
		{
			if (MessageService.Confirm (GettextCatalog.GetString ("Are you sure you want to remove the selected Item?"), AlertButton.Delete))
				toolboxService.RemoveUserItem (selectedNode);
		}

		[CommandUpdateHandler (MonoDevelop.Ide.Commands.EditCommands.Delete)]
		internal void OnUpdateDeleteItem (CommandInfo info)
		{
			// Hack manually filter out gtk# widgets & container since they cannot be re added
			// because they're missing the toolbox attributes.
			info.Enabled = selectedNode != null
				&& (selectedNode.ItemDomain != GtkWidgetDomain
				    || (selectedNode.Category != "Widgets" && selectedNode.Category != "Container"));
		}
		
		static readonly string GtkWidgetDomain = GettextCatalog.GetString ("GTK# Widgets");
		
		#endregion
		
		#region GUI population
		
		Dictionary<string, ToolboxWidgetCategory> categories = new Dictionary<string, ToolboxWidgetCategory> ();
		void AddItems (IEnumerable<ItemToolboxNode> nodes)
		{
			foreach (ItemToolboxNode itbn in nodes) {
				var newItem = new ToolboxWidgetItem (itbn);
				if (!categories.ContainsKey (itbn.Category)) {
					var cat = new ToolboxWidgetCategory (itbn.Category);
					int prio;
					if (!categoryPriorities.TryGetValue (itbn.Category, out prio))
						prio = -1;
					cat.Priority = prio;
					categories[itbn.Category] = cat;
				}
				if (newItem.Text != null)
					categories[itbn.Category].Add (newItem);
			}
		}
		
		public void Refresh ()
		{
			// GUI assert here is to catch Bug 434065 - Exception while going to the editor
			Runtime.AssertMainThread ();
			
			if (toolboxService.Initializing) {
				toolboxWidget.CustomMessage = GettextCatalog.GetString ("Initializing...");
				return;
			}
			
			ConfigureToolbar ();
			
			toolboxWidget.CustomMessage = null;
			
			categories.Clear ();
			AddItems (toolboxService.GetCurrentToolboxItems ());
			
			toolboxWidget.ClearCategories ();
			
			var cats = categories.Values.ToList ();
			cats.Sort ((a,b) => a.Priority != b.Priority ? a.Priority.CompareTo (b.Priority) : a.Text.CompareTo (b.Text));
			cats.Reverse ();
			foreach (ToolboxWidgetCategory category in cats) {
				category.IsExpanded = true;
				toolboxWidget.AddCategory (category);
			}
			toolboxWidget.QueueResize ();
			var targetTable = toolboxService.GetCurrentDragTargetTable ();
			dragSource.Actions = targetTable != null && targetTable.Length > 0 ? Gdk.DragAction.Copy | Gdk.DragAction.Move : 0;
			compactModeToggleButton.Visible = toolboxWidget.CanIconizeToolboxCategories;
			refilter ();
		}
		
		void ConfigureToolbar ()
		{
			// Default configuration
			categoryPriorities.Clear ();
			toolboxAddButton.Visible = true;
			
			toolboxService.Customize (container, this);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing && fontChanger != null) {
				fontChanger.Dispose ();
				fontChanger = null;
			}
			base.Dispose (disposing);
		}

		void OnDragPrepare (object sender, PrepareArgs args)
		{
			args.RetVal = null;
			var item = toolboxWidget.SelectedItem;
			if (item == null || dragSource.Actions == 0)
				return;

			toolboxWidget.HideTooltipWindow ();
			var providers = new List<Gdk.ContentProvider> ();

			// marker format: identifies drags started from the toolbox
			providers.Add (new Gdk.ContentProvider (ToolboxPad.ToolBoxDragDropFormat, new GLib.Bytes (new byte [0])));

			// node specific formats and data
			if (item.Node is IDragDataToolboxNode dragNode) {
				foreach (var format in dragNode.Formats)
					providers.Add (new Gdk.ContentProvider (format, new GLib.Bytes (dragNode.GetData (format) ?? new byte [0])));
			}

			// consumer content (formats of IToolboxConsumer.DragTargets)
			var consumerContent = toolboxService.DragSelectedItem (toolboxWidget);
			if (consumerContent != null)
				providers.Add (consumerContent);

			args.RetVal = providers.Count == 1 ? providers [0] : new Gdk.ContentProvider (providers.ToArray ());
		}
		
		#endregion
		
		#region IPropertyPadProvider
		
		object IPropertyPadProvider.GetActiveComponent ()
		{
			return selectedNode;
		}

		object IPropertyPadProvider.GetProvider ()
		{
			return selectedNode;
		}

		void IPropertyPadProvider.OnEndEditing (object obj)
		{
		}

		void IPropertyPadProvider.OnChanged (object obj)
		{
		}
		
		#endregion

		#region IToolboxConfiguration implementation
		public void SetCategoryPriority (string category, int priority)
		{
			categoryPriorities[category] = priority;
		}

		public bool AllowEditingComponents {
			get {
				return toolboxAddButton.Visible;
			}
			set {
				toolboxAddButton.Visible = value;
			}
		}
		#endregion
	}
}
