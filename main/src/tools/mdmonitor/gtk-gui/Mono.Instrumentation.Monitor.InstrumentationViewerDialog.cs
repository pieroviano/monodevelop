// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
// The Gtk.UIManager / Gtk.Action menu became a Gio menu model shown by a Gtk.PopoverMenuBar.
namespace Mono.Instrumentation.Monitor
{
	public partial class InstrumentationViewerDialog
	{
		private global::GLib.SimpleAction openAction;
		private global::GLib.SimpleAction connectAction;
		private global::GLib.SimpleAction ExitAction;
		private global::GLib.SimpleAction FlushMemoryAction;
		private global::Gtk.Box dialog1_VBox;
		private global::Gtk.PopoverMenuBar menubar1;
		private global::Gtk.Box vbox2;
		private global::Gtk.Paned hpaned;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView treeCounters;
		private global::Gtk.Box vbox3;
		private global::Gtk.Box headerBox;
		private global::Gtk.Box hbox1;
		private global::Gtk.Label labelHeader;
		private global::Gtk.Box buttonsBox;
		private global::Gtk.Button buttonSave;
		private global::Gtk.Button buttonSaveAs;
		private global::Gtk.Button buttonDelete;
		private global::MonoDevelop.Components.BinWidget viewBox;

		protected virtual void Build ()
		{
			this.Title = "Instrumentation Monitor";

			// Menu: actions live in the "win" group of this window
			var actions = new global::GLib.SimpleActionGroup ();
			this.openAction = new global::GLib.SimpleAction ("open", null);
			this.connectAction = new global::GLib.SimpleAction ("connect", null) { Enabled = false };
			this.ExitAction = new global::GLib.SimpleAction ("exit", null);
			this.FlushMemoryAction = new global::GLib.SimpleAction ("flushmemory", null);
			actions.AddAction (this.openAction);
			actions.AddAction (this.connectAction);
			actions.AddAction (this.ExitAction);
			actions.AddAction (this.FlushMemoryAction);
			this.InsertActionGroup ("win", actions);

			var fileMenu = new global::GLib.Menu ();
			var fileSection = new global::GLib.Menu ();
			fileSection.Append (global::Mono.Unix.Catalog.GetString ("_Open"), "win.open");
			fileSection.Append (global::Mono.Unix.Catalog.GetString ("C_onnect"), "win.connect");
			fileMenu.AppendSection (null, fileSection);
			var exitSection = new global::GLib.Menu ();
			exitSection.Append (global::Mono.Unix.Catalog.GetString ("Exit"), "win.exit");
			fileMenu.AppendSection (null, exitSection);
			var toolsMenu = new global::GLib.Menu ();
			toolsMenu.Append (global::Mono.Unix.Catalog.GetString ("Flush Memory"), "win.flushmemory");
			var menuModel = new global::GLib.Menu ();
			menuModel.AppendSubmenu (global::Mono.Unix.Catalog.GetString ("File"), fileMenu);
			menuModel.AppendSubmenu (global::Mono.Unix.Catalog.GetString ("Tools"), toolsMenu);

			this.dialog1_VBox = DesignerLayout.VBox (0);
			this.menubar1 = new global::Gtk.PopoverMenuBar (menuModel);
			DesignerLayout.Pack (this.dialog1_VBox, this.menubar1);

			this.vbox2 = DesignerLayout.Margin (DesignerLayout.VBox (6), 9);
			this.hpaned = new global::Gtk.Paned (global::Gtk.Orientation.Horizontal) { Vexpand = true };
			this.hpaned.CanFocus = true;

			this.treeCounters = new global::Gtk.TreeView ();
			this.treeCounters.CanFocus = true;
			this.treeCounters.HeadersVisible = false;
			this.GtkScrolledWindow = DesignerLayout.Scrolled (this.treeCounters);
			this.hpaned.StartChild = this.GtkScrolledWindow;
			this.hpaned.ResizeStartChild = false;
			this.hpaned.ShrinkStartChild = false;

			this.vbox3 = DesignerLayout.VBox (6);
			this.headerBox = DesignerLayout.HBox (6);
			this.headerBox.AddCssClass ("toolbar");
			this.hbox1 = DesignerLayout.Margin (DesignerLayout.HBox (6), 3);
			this.hbox1.Hexpand = true;
			this.labelHeader = DesignerLayout.Label (null);
			this.labelHeader.Hexpand = true;
			DesignerLayout.Pack (this.hbox1, this.labelHeader, true);
			this.buttonsBox = DesignerLayout.HBox (6);
			this.buttonSave = DesignerLayout.IconButton ("document-save-symbolic", global::Mono.Unix.Catalog.GetString ("Save"));
			DesignerLayout.Pack (this.buttonsBox, this.buttonSave);
			this.buttonSaveAs = DesignerLayout.IconButton ("edit-copy-symbolic", global::Mono.Unix.Catalog.GetString ("Copy"));
			DesignerLayout.Pack (this.buttonsBox, this.buttonSaveAs);
			this.buttonDelete = DesignerLayout.IconButton ("edit-delete-symbolic", global::Mono.Unix.Catalog.GetString ("Delete"));
			DesignerLayout.Pack (this.buttonsBox, this.buttonDelete);
			DesignerLayout.Pack (this.hbox1, this.buttonsBox);
			DesignerLayout.Pack (this.headerBox, this.hbox1, true);
			DesignerLayout.Pack (this.vbox3, this.headerBox);
			this.viewBox = new global::MonoDevelop.Components.BinWidget { Hexpand = true, Vexpand = true };
			DesignerLayout.Pack (this.vbox3, this.viewBox, true);
			this.hpaned.EndChild = this.vbox3;
			this.hpaned.ResizeEndChild = true;
			this.hpaned.ShrinkEndChild = true;

			DesignerLayout.Pack (this.vbox2, this.hpaned, true);
			DesignerLayout.Pack (this.dialog1_VBox, this.vbox2, true);
			this.Child = this.dialog1_VBox;

			this.DefaultWidth = 942;
			this.DefaultHeight = 587;
			this.CloseRequest += new global::Gtk.CloseRequestHandler (this.OnCloseRequest);
			this.openAction.Activated += (o, a) => this.OnOpenActionActivated (o, global::System.EventArgs.Empty);
			this.ExitAction.Activated += (o, a) => this.OnExitActionActivated (o, global::System.EventArgs.Empty);
			this.FlushMemoryAction.Activated += (o, a) => this.OnFlushMemoryActionActivated (o, global::System.EventArgs.Empty);
			this.buttonSave.Clicked += new global::System.EventHandler (this.OnButtonSaveClicked);
			this.buttonSaveAs.Clicked += new global::System.EventHandler (this.OnButtonSaveAsClicked);
			this.buttonDelete.Clicked += new global::System.EventHandler (this.OnButtonDeleteClicked);
		}
	}
}
