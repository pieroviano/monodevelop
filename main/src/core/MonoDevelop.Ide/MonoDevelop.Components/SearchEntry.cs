//
// SearchEntry.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//   Gabriel Burt <gburt@novell.com>
//
// Copyright 2007-2010 Novell, Inc.
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
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Components.AtkCocoaHelper;

namespace MonoDevelop.Components
{
	/// <summary>
	/// Search field: [filter button][entry][optional label][clear button] inside a drawn frame.
	/// The filter menu is a <see cref="GLib.Menu"/> shown in a <see cref="Gtk.PopoverMenu"/>; its built-in
	/// filter options use actions in the <see cref="MenuActionPrefix"/> group of this widget.
	/// </summary>
	[System.ComponentModel.ToolboxItem(true)]
	public class SearchEntry : BinWidget
	{
		/// <summary>Action group prefix of <see cref="MenuActions"/> (menu items use "searchentry.&lt;name&gt;").</summary>
		public const string MenuActionPrefix = "searchentry";

		Gtk.Box box;
		Gtk.Entry entry;
		HoverImageButton filter_button;
		HoverImageButton clear_button;

		GLib.Menu menu;
		GLib.Menu currentSection;
		readonly GLib.SimpleActionGroup menuActions = new GLib.SimpleActionGroup ();
		readonly List<FilterOption> filterOptions = new List<FilterOption> ();
		int actionCounter;
		Gtk.PopoverMenu menuPopover;
		int active_filter_id = -1;

		uint changed_timeout_id = 0;

		string empty_message;
		bool ready = false;

		event EventHandler filter_changed;
		event EventHandler entry_changed;
		EventHandler activated_event;
		bool roundedShape;
		bool hasFrame = true;

		public event EventHandler Changed {
			add { entry_changed += value; }
			remove { entry_changed -= value; }
		}

		public event EventHandler Activated {
			add { activated_event += value; }
			remove { activated_event -= value; }
		}

		public event EventHandler FilterChanged {
			add { filter_changed += value; }
			remove { filter_changed -= value; }
		}

		bool forceFilterButtonVisible = true;
		public bool ForceFilterButtonVisible {
			get {
				return forceFilterButtonVisible;
			}
			set {
				forceFilterButtonVisible = value;
				ShowHideButtons ();
			}
		}

		/// <summary>
		/// Filter menu model. A replacement menu must reference actions that can be resolved from this widget,
		/// e.g. added to <see cref="MenuActions"/> ("searchentry.&lt;name&gt;") or inserted with <c>InsertActionGroup</c>.
		/// </summary>
		public GLib.Menu Menu {
			get {
				if (menu == null)
					SetMenu (new GLib.Menu ());
				return menu;
			}
			set {
				SetMenu (value);
			}
		}

		/// <summary>Actions available to <see cref="Menu"/> items under the "searchentry." prefix.</summary>
		public GLib.SimpleActionGroup MenuActions {
			get { return menuActions; }
		}

		void SetMenu (GLib.Menu value)
		{
			ClosePopover ();
			menu = value;
			currentSection = null;
			if (filterOptions.Count > 0) {
				foreach (var o in filterOptions)
					menuActions.Remove (o.ActionName);
				filterOptions.Clear ();
			}
			ShowHideButtons ();
		}

		public Gtk.Entry Entry {
			get { return this.entry; }
		}

		public bool HasFrame {
			get { return hasFrame; }
			set { hasFrame = value; QueueDraw (); }
		}

		public bool RoundedShape {
			get { return roundedShape; }
			set {
				if (!Platform.IsMac)
					return;

				roundedShape = value;
				ShowHideButtons ();
				QueueDraw ();
			}
		}

		public SearchEntry ()
		{
			BuildWidget ();
			this.SetShouldIgnore (true);
		}

		public Xwt.Drawing.Image FilterButtonPixbuf {
			get {
				return filter_button.Pixbuf;
			}
			set {
				filter_button.Pixbuf = value;
			}
		}

		void BuildWidget ()
		{
			Padding = new Gtk.Border { Left = 3, Right = 3, Top = 1, Bottom = 1 };
			InsertActionGroup (MenuActionPrefix, menuActions);

			box = new Gtk.Box (Gtk.Orientation.Horizontal, 0) {
				Valign = Gtk.Align.Center,
			};
			box.SetShouldIgnore (true);
			entry = new Gtk.Entry {
				HasFrame = false,
				WidthChars = 1,
				Hexpand = true,
			};
			entry.SetSubRole ("AXSearchField");
			entry.SetLabel (GettextCatalog.GetString ("Search"));

			filter_button = new HoverImageButton (StockIconSize.Menu, "md-searchbox-search");
			filter_button.SetRole (AtkCocoa.Roles.AXMenuButton);
			filter_button.SetCommonAttributes (null, GettextCatalog.GetString ("Search filter menu"), GettextCatalog.GetString ("Change the search filters"));

			// This will be set to false if an event handler is attached to RequestMenu
			filter_button.SetShouldIgnore (true);

			clear_button = new HoverImageButton (StockIconSize.Menu, "md-searchbox-clear");
			clear_button.SetCommonAttributes (null, GettextCatalog.GetString ("Clear"), GettextCatalog.GetString ("Clear the search entry"));

			box.Append (filter_button);
			box.Append (entry);
			box.Append (clear_button);
			Child = box;

			entry.AddFocusHandler (QueueDraw, QueueDraw);
			entry.Changed += OnInnerEntryChanged;
			entry.Activated += delegate {
				NotifyActivated ();
			};

			filter_button.CanFocus = false;
			clear_button.CanFocus = false;

			filter_button.AddClickHandler (null, c => OnButtonRelease (filter_button), 1);
			clear_button.AddClickHandler (null, c => OnButtonRelease (clear_button), 1);
			clear_button.Clicked += OnClearButtonClicked;

			this.AddKeyPressHandler (OnKeyPressed);

			ShowHideButtons ();
		}

		Gtk.Widget statusLabelWidget;

		/// <summary>Adds <paramref name="label"/> before the clear button; returns the widget hosting it (the label itself in GTK 4).</summary>
		public Gtk.Widget AddLabelWidget (Gtk.Label label)
		{
			if (statusLabelWidget != null)
				box.Remove (statusLabelWidget);
			statusLabelWidget = label;
			box.InsertChildAfter (label, entry);
			return label;
		}

		void NotifyActivated ()
		{
			if (activated_event != null)
				activated_event (this, EventArgs.Empty);
		}

		public void PopupFilterMenu ()
		{
			ShowMenu ();
		}

		void ShowMenu ()
		{
			OnRequestMenu (EventArgs.Empty);
			if (!MenuHasChildren ())
				return;

			ClosePopover ();
			menuPopover = new Gtk.PopoverMenu (menu) {
				HasArrow = false,
				Position = Gtk.PositionType.Bottom,
			};
			menuPopover.Parent = filter_button;
			var popover = menuPopover;
			popover.Closed += (s, e) => GLib.Idle.Add (() => {
				popover.Detach ();
				if (menuPopover == popover)
					menuPopover = null;
				filter_button.QueueDraw ();
				return false;
			});
			popover.Popup ();
		}

		void ClosePopover ()
		{
			if (menuPopover != null) {
				menuPopover.Detach ();
				menuPopover = null;
			}
		}

		bool MenuHasChildren ()
		{
			return menu != null && menu.NItems > 0;
		}

		void ShowHideButtons ()
		{
			if (entry == null)
				return;
			clear_button.Visible = entry.Text.Length > 0;
			entry.MarginEnd = !clear_button.Visible && roundedShape ? 6 : 0;

			filter_button.Visible = ForceFilterButtonVisible || MenuHasChildren ();
			entry.MarginStart = !filter_button.Visible && roundedShape ? 6 : 0;
		}

		bool toggling = false;
		public bool IsCheckMenu { get; set; }

		void OnFilterOptionToggled (FilterOption item)
		{
			if (IsCheckMenu || toggling) {
				return;
			}

			toggling = true;
			foreach (var other in filterOptions) {
				if (other != item)
					other.Active = false;
			}

			item.Active = true;
			ActiveFilterID = item.ID;
			toggling = false;
		}

		void OnInnerEntryChanged (object o, EventArgs args)
		{
			ShowHideButtons ();

			if (changed_timeout_id > 0) {
				GLib.Source.Remove (changed_timeout_id);
				changed_timeout_id = 0;
			}

			if (Ready)
				changed_timeout_id = GLib.Timeout.Add (25, OnChangedTimeout);
		}

		bool OnChangedTimeout ()
		{
			changed_timeout_id = 0;
			OnChanged ();
			return false;
		}

		void OnButtonRelease (Gtk.Widget button)
		{
			entry.GrabFocus ();

			if (button == filter_button) {
				ShowMenu ();
			}
		}

		protected virtual void OnRequestMenu (EventArgs e)
		{
			requestMenu?.Invoke (this, e);
		}

		event EventHandler requestMenu;
		object requestMenuLock = new object ();
		public event EventHandler RequestMenu {
			add {
				lock (requestMenuLock) {
					requestMenu += value;
					filter_button.SetShouldIgnore (false);
				}
			}

			remove {
				lock (requestMenuLock) {
					requestMenu -= value;
					if (requestMenu == null) {
						filter_button.SetShouldIgnore (true);
					}
				}
			}
		}

		public void GrabFocusEntry ()
		{
			this.entry.GrabFocus ();
		}

		void OnClearButtonClicked (object o, EventArgs args)
		{
			active_filter_id = 0;
			entry.Text = String.Empty;
			NotifyActivated ();
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				ClosePopover ();
				if (changed_timeout_id > 0) {
					GLib.Source.Remove (changed_timeout_id);
					changed_timeout_id = 0;
				}
			}
			base.Dispose (disposing);
		}

		bool OnKeyPressed (uint keyval, uint keycode, Gdk.ModifierType state)
		{
			if ((Gdk.Key)keyval == Gdk.Key.Escape) {
				active_filter_id = 0;
				entry.Text = String.Empty;
				NotifyActivated ();
				return true;
			}
			return false;
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			int width = Width, height = Height;
			if (width <= 0 || height <= 0)
				return;

			// frame around the inner box (vertically centered), like the GTK 2 entry shadow
			var boxBounds = box.Allocation;
			double x = Padding.Left + 0.5, y = boxBounds.Y + 0.5;
			double w = Math.Max (0, width - Padding.Left - Padding.Right - 1), h = Math.Max (0, boxBounds.Height - 1);

			snapshot.DrawCairo (this, ctx => {
				var baseColor = Styles.BaseBackgroundColor.ToCairoColor ();
				if (roundedShape)
					RoundBorder (ctx, x, y, w, h);
				else
					CairoExtensions.RoundedRectangle (ctx, x, y, w, h, hasFrame ? 3 : 4);
				ctx.SetSourceColor (baseColor);
				if (hasFrame) {
					ctx.FillPreserve ();
					ctx.SetSourceColor ((entry.HasFocus ? Styles.FocusColor : Styles.WidgetBorderColor).ToCairoColor ());
					ctx.LineWidth = 1;
					ctx.Stroke ();
				} else {
					ctx.Fill ();
				}
			});

			base.OnSnapshot (snapshot);
		}

		static void RoundBorder (Cairo.Context ctx, double x, double y, double w, double h)
		{
			double r = h / 2;
			ctx.Arc (x + r, y + r, r, Math.PI / 2, Math.PI + Math.PI / 2);
			ctx.LineTo (x + w - r, y);

			ctx.Arc (x + w - r, y + r, r, Math.PI + Math.PI / 2, Math.PI + Math.PI + Math.PI / 2);

			ctx.LineTo (x + r, y + h);

			ctx.ClosePath ();
		}

		protected override void OnMapped ()
		{
			base.OnMapped ();
			ShowHideButtons ();
		}

		protected virtual void OnChanged ()
		{
			if (!Ready) {
				return;
			}

			EventHandler handler = entry_changed;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}

		protected virtual void OnFilterChanged ()
		{
			EventHandler handler = filter_changed;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}

			if (IsQueryAvailable) {
				OnInnerEntryChanged (this, EventArgs.Empty);
			}
		}

		GLib.Menu CurrentSection {
			get {
				if (currentSection == null) {
					currentSection = new GLib.Menu ();
					Menu.AppendSection (null, currentSection);
				}
				return currentSection;
			}
		}

		/// <summary>Adds a check (radio unless <see cref="IsCheckMenu"/>) filter item to the menu.</summary>
		public FilterOption AddFilterOption (int id, string label)
		{
			if (id < 0) {
				throw new ArgumentException ("id", "must be >= 0");
			}

			var item = new FilterOption (this, id, label, "filter" + (actionCounter++));
			filterOptions.Add (item);
			menuActions.Insert (item.Action);
			item.Section = CurrentSection;
			item.Section.Append (label, MenuActionPrefix + "." + item.ActionName);

			if (ActiveFilterID < 0) {
				item.Toggle ();
			}

			filter_button.Visible = true;
			return item;
		}

		/// <summary>Adds a plain item; handle <see cref="GLib.SimpleAction.Activated"/> on the returned action.</summary>
		public GLib.SimpleAction AddMenuItem (string label)
		{
			var action = new GLib.SimpleAction ("item" + (actionCounter++), null);
			menuActions.Insert (action);
			CurrentSection.Append (label, MenuActionPrefix + "." + action.Name);
			ShowHideButtons ();
			return action;
		}

		public void AddFilterSeparator ()
		{
			// GMenu draws separators between sections
			currentSection = new GLib.Menu ();
			Menu.AppendSection (null, currentSection);
		}

		public void RemoveFilterOption (int id)
		{
			FilterOption item = FindFilterOption (id);
			if (item == null)
				return;

			string detailed = MenuActionPrefix + "." + item.ActionName;
			var section = item.Section;
			for (int i = 0; i < section.NItems; i++) {
				using (var v = section.GetItemAttributeValue (i, "action", GLib.VariantType.String)) {
					if (v != null && (string)v == detailed) {
						section.Remove (i);
						break;
					}
				}
			}
			menuActions.Remove (item.ActionName);
			filterOptions.Remove (item);
		}

		public void ActivateFilter (int id)
		{
			FilterOption item = FindFilterOption (id);
			if (item != null) {
				item.Toggle ();
			}
		}

		FilterOption FindFilterOption (int id)
		{
			foreach (var item in filterOptions) {
				if (item.ID == id)
					return item;
			}
			return null;
		}

		public string GetLabelForFilterID (int id)
		{
			FilterOption item = FindFilterOption (id);
			if (item == null) {
				return null;
			}

			return item.Label;
		}

		public void CancelSearch ()
		{
			entry.Text = String.Empty;
			ActivateFilter (0);
		}

		public int ActiveFilterID {
			get { return active_filter_id; }
			set {
				if (value == active_filter_id) {
					return;
				}

				active_filter_id = value;
				OnFilterChanged ();
			}
		}

		/// <summary>Placeholder shown while the entry is empty (Pango markup is stripped).</summary>
		public string EmptyMessage {
			get { return entry.Sensitive ? empty_message : String.Empty; }
			set {
				empty_message = value;
				entry.PlaceholderText = StripMarkup (value);
			}
		}

		static string StripMarkup (string markup)
		{
			if (string.IsNullOrEmpty (markup) || markup.IndexOf ('<') < 0 && markup.IndexOf ('&') < 0)
				return markup;
			var text = System.Text.RegularExpressions.Regex.Replace (markup, "<[^>]*>", string.Empty);
			return System.Net.WebUtility.HtmlDecode (text);
		}

		public string Query {
			get { return entry.Text.Trim (); }
			set { entry.Text = value.Trim (); }
		}

		public bool IsQueryAvailable {
			get { return Query != null && Query != String.Empty; }
		}

		public bool Ready {
			get { return ready; }
			set { ready = value; }
		}

		public new bool HasFocus {
			get { return entry.HasFocus; }
			set { if (value) entry.GrabFocus (); }
		}

		public Gtk.Entry InnerEntry {
			get { return entry; }
		}

		/// <summary>Check item of the filter menu (replaces the GTK 2 <c>CheckMenuItem</c>).</summary>
		public sealed class FilterOption
		{
			readonly SearchEntry owner;
			bool active;

			internal FilterOption (SearchEntry owner, int id, string label, string actionName)
			{
				this.owner = owner;
				ID = id;
				Label = label;
				ActionName = actionName;
				Action = new GLib.SimpleAction (actionName, null, new GLib.Variant (false));
				// a stateful action without parameter: activation toggles (like GTK 2 CheckMenuItem activate)
				Action.Activated += (o, e) => Toggle ();
			}

			public int ID { get; }

			public string Label { get; }

			internal string ActionName { get; }

			internal GLib.SimpleAction Action { get; }

			internal GLib.Menu Section { get; set; }

			/// <summary>Checked state; changing it raises <see cref="Toggled"/>.</summary>
			public bool Active {
				get { return active; }
				set {
					if (active == value)
						return;
					active = value;
					Action.ChangeState (new GLib.Variant (value));
					Toggled?.Invoke (this, EventArgs.Empty);
					owner.OnFilterOptionToggled (this);
				}
			}

			public void Toggle ()
			{
				Active = !Active;
			}

			public event EventHandler Toggled;
		}

		public void SetEntryAccessibilityAttributes (string name, string label, string help)
		{
			entry.SetCommonAccessibilityAttributes (name, label, help);
		}

		public Gtk.IAccessible EntryAccessible => entry;
	}
}
