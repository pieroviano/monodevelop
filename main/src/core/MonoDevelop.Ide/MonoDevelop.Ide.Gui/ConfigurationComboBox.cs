//
// ConfigurationComboBox.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Projects;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Core.FeatureConfiguration;

namespace MonoDevelop.Ide.Gui
{
	internal class ConfigurationComboBox: CustomItem
	{
		DropDownBox combo;
		DropDownBox.ComboItemSet configs = new DropDownBox.ComboItemSet ();
		DropDownBox.ComboItemSet runtimes = new DropDownBox.ComboItemSet ();
		object defaultRuntime = new object ();
		bool updating;
		
		public ConfigurationComboBox () 
		{
			combo = new DropDownBox ();
			combo.AddItemSet (configs);
			combo.AddItemSet (runtimes);
			
			combo.Hexpand = true;
			combo.Valign = Gtk.Align.Center;
			combo.MarginStart = 3;
			combo.MarginEnd = 3;
			Append (combo);
			
			Combo.Changed += OnChanged;
			IdeApp.Workspace.ConfigurationsChanged += OnConfigurationsChanged;
			IdeApp.Workspace.RuntimesChanged += OnConfigurationsChanged;
			IdeApp.Workspace.ActiveRuntimeChanged += OnActiveRuntimeChanged;
			IdeApp.Workspace.ActiveConfigurationChanged += OnActiveConfigurationChanged;
			Reset ();
		}
		
		protected DropDownBox Combo {
			get { return combo; }
		}
		
		Pango.FontDescription fd;
		
		public override void SetToolbarStyle (Gtk.Widget toolbar)
		{
			if (fd != null) {
				fd.Dispose ();
				fd = null;
			}
			// the main toolbar uses small icons, so it uses the small font
			fd = IdeServices.FontService.SansFont.CopyModified (Styles.FontScale11);
			combo.ModifyLabelFont (fd);
		}
		
		void Reset ()
		{
			configs.Clear ();
			runtimes.Clear ();
			UpdateLabel ();
		}
		
		void RefreshCombo ()
		{
			configs.Clear ();
			runtimes.Clear ();
			
			foreach (string conf in IdeApp.Workspace.GetConfigurations ()) {
				DropDownBox.ComboItem item = new DropDownBox.ComboItem (conf, conf);
				configs.Add (item);
				if (conf == IdeApp.Workspace.ActiveConfigurationId)
					configs.CurrentItem = item.Item;
			}

			var enabled = FeatureSwitchService.IsFeatureEnabled (FeatureSwitches.RuntimeSelectorFeatureSwitchName);

			if (enabled.GetValueOrDefault ()) {
				foreach (TargetRuntime tr in Runtime.SystemAssemblyService.GetTargetRuntimes ()) {
					DropDownBox.ComboItem item = new DropDownBox.ComboItem (tr.DisplayName, tr);
					runtimes.Add (item);
					if (tr == IdeApp.Workspace.ActiveRuntime)
						runtimes.CurrentItem = tr;
				}

				// If there is only one runtime, there is no need to show it
				if (runtimes.Count == 1)
					runtimes.Clear ();
				else {
					DropDownBox.ComboItem item = new DropDownBox.ComboItem (GettextCatalog.GetString ("Default Runtime"), defaultRuntime);
					runtimes.Insert (0, item);
					if (IdeApp.Workspace.UseDefaultRuntime)
						runtimes.CurrentItem = defaultRuntime;
				}
			}
			
			UpdateLabel ();
			
			if (configs.CurrentItem == null && configs.Count > 0)
				IdeApp.Workspace.ActiveConfigurationId = (string) configs [0].Item;
			
			if (runtimes.CurrentItem == null && runtimes.Count > 0)
				IdeApp.Workspace.ActiveRuntime = Runtime.SystemAssemblyService.CurrentRuntime;
		}
		
		void UpdateLabel ()
		{
			if ((configs.Count == 0 && runtimes.Count == 0) || !IdeApp.Workspace.IsOpen) {
				combo.Sensitive = false;
				combo.ActiveText = "";
				return;
			}
			combo.Sensitive = true;
			string label = "";
			if (configs.Count > 0) {
				label = (string) configs.CurrentItem;
				if (runtimes.Count > 0) {
					if (runtimes.CurrentItem is TargetRuntime)
						label += " (" + ((TargetRuntime)runtimes.CurrentItem).DisplayName + ")";
				}
			} else if (runtimes.Count > 0) {
				if (runtimes.CurrentItem is TargetRuntime)
					label = ((TargetRuntime)runtimes.CurrentItem).DisplayName;
				else
					label = GettextCatalog.GetString ("Default Runtime");
			}
			combo.ActiveText = label;
		}
		
		void OnConfigurationsChanged (object sender, EventArgs e)
		{
			RefreshCombo ();
		}
		
		void OnActiveConfigurationChanged (object sender, EventArgs e)
		{
			if (updating)
				return;

			foreach (DropDownBox.ComboItem item in configs) {
				if ((string)(item.Item) == IdeApp.Workspace.ActiveConfigurationId) {
					configs.CurrentItem = IdeApp.Workspace.ActiveConfigurationId;
					UpdateLabel ();
					return;
				}
			}

			// Configuration not found. Set one that is known.
			if (configs.Count > 0)
				IdeApp.Workspace.ActiveConfigurationId = configs [0].Label;
		}
		
		void OnActiveRuntimeChanged (object sender, EventArgs e)
		{
			if (updating)
				return;
			
			foreach (DropDownBox.ComboItem item in runtimes) {
				if ((IdeApp.Workspace.UseDefaultRuntime && item.Item == defaultRuntime) ||
				    (item.Label == IdeApp.Workspace.ActiveRuntime.DisplayName)) {
					runtimes.CurrentItem = item.Item;
					UpdateLabel ();
					return;
				}
			}
		}
		
		protected void OnChanged (object sender, DropDownBox.ChangedEventArgs args)
		{
			if (updating)
				return;
			if (IdeApp.Workspace.IsOpen) {
				updating = true;
				if (args.ItemSet == configs) {
					IdeApp.Workspace.ActiveConfigurationId = (string) args.Item;
				} else if (args.ItemSet == runtimes) {
					if (args.Item is TargetRuntime)
						IdeApp.Workspace.ActiveRuntime = (TargetRuntime) args.Item;
					else
						IdeApp.Workspace.UseDefaultRuntime = true;
				}
				UpdateLabel ();
				updating = false;
			}
		}
	}
	
	class DropDownBox : Gtk.Button
	{
		Gtk.Label label;
		List<ComboItemSet> items = new List<ComboItemSet> ();
		Gtk.Popover popover;
		
		public event EventHandler<ChangedEventArgs> Changed;
		
		public class ComboItem
		{
			public ComboItem (string label, object item) {
				Label = label;
				Item = item;
			}
			public string Label { get; set; }
			public object Item { get; set; }
		}
		
		public class ComboItemSet: List<ComboItem>
		{
			public object CurrentItem { get; set; }
			
			public new void Clear ()
			{
				base.Clear ();
				CurrentItem = null;
			}
		}
		
		public class ChangedEventArgs: EventArgs
		{
			public ComboItemSet ItemSet { get; set; }
			public object Item { get; set; }
		}
		
		public DropDownBox ()
		{
			var hbox = new Gtk.Box (Gtk.Orientation.Horizontal, 0);
			
			label = new Gtk.Label ();
			label.Xalign = 0;
			label.WidthRequest = 125;
			label.Hexpand = true;
			label.MarginStart = 3;
			label.MarginEnd = 3;
			label.Ellipsize = Pango.EllipsizeMode.End;
			
			hbox.Append (label);
			
			var separator = new Gtk.Separator (Gtk.Orientation.Vertical);
			separator.MarginStart = 1;
			separator.MarginEnd = 1;
			hbox.Append (separator);
			var arrow = Gtk.Image.NewFromIconName ("pan-down-symbolic");
			arrow.MarginStart = 1;
			arrow.MarginEnd = 1;
			hbox.Append (arrow);
			Child = hbox;

			Clicked += (o, args) => ShowMenu ();
		}
		
		public void ModifyLabelFont (Pango.FontDescription font)
		{
			if (font == null) {
				label.Attributes = null;
				return;
			}
			var attributes = new Pango.AttrList ();
			attributes.Insert (new Pango.AttrFontDesc (font));
			label.Attributes = attributes;
		}
		
		public string ActiveText {
			get {
				return label.Text;
			}
			set {
				label.Text = value;
			}
		}
		
		public void Clear ()
		{
			items.Clear ();
		}
		
		public void AddItemSet (ComboItemSet iset)
		{
			items.Add (iset);
		}
		
		void SelectItem (ComboItemSet iset, ComboItem item)
		{
			iset.CurrentItem = item.Item;
			if (Changed != null) {
				ChangedEventArgs args = new ChangedEventArgs ();
				args.ItemSet = iset;
				args.Item = item.Item;
				Changed (this, args);
			}
		}

		void ShowMenu ()
		{
			// The item sets are shown as groups of radio items separated by lines, in a popover below the button
			var menuBox = new Gtk.Box (Gtk.Orientation.Vertical, 0);
			bool first = true;
			foreach (ComboItemSet iset in items) {
				if (iset.Count == 0)
					continue;
				
				if (!first)
					menuBox.Append (new Gtk.Separator (Gtk.Orientation.Horizontal));
				first = false;
				
				Gtk.CheckButton grp = null;
				foreach (ComboItem ci in iset) {
					var mi = new Gtk.CheckButton ();
					mi.Label = ci.Label;
					if (grp == null)
						grp = mi;
					else
						mi.Group = grp;
					if (ci.Item == iset.CurrentItem || ci.Item.Equals (iset.CurrentItem))
						mi.Active = true;
					
					ComboItemSet isetLocal = iset;
					ComboItem ciLocal = ci;
					mi.Toggled += delegate {
						if (!mi.Active)
							return;
						popover?.Popdown ();
						SelectItem (isetLocal, ciLocal);
					};
					menuBox.Append (mi);
				}
			}

			if (popover == null) {
				popover = new Gtk.Popover ();
				popover.HasArrow = false;
				popover.Position = Gtk.PositionType.Bottom;
				popover.Parent = this;
			}
			popover.Child = menuBox;
			// let the list be at least as wide as the button
			menuBox.WidthRequest = Width;
			popover.Popup ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing && popover != null) {
				popover.Unparent ();
				popover = null;
			}
			base.Dispose (disposing);
		}
	}
	
}
