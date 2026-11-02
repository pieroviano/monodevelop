//
// ThemeExtensions.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@xamarin.com>
//
// Copyright (c) 2015 Xamarin, Inc (http://www.xamarin.com)
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
using System.Collections.Generic;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using System.Linq;

#if MAC
using AppKit;
using Foundation;
using MonoDevelop.Components.Mac;
#endif

namespace MonoDevelop.Components
{
	public static class IdeTheme
	{
		internal static string DefaultTheme;

		// GTK 4: gtkrc (Gtk.Rc) theming is gone. MonoDevelop's light/dark theme is applied through
		// Gtk.Settings (gtk-application-prefer-dark-theme / gtk-theme-name) plus an application CSS provider.
		static Gtk.CssProvider themeCssProvider;
		static readonly HashSet<Gdk.Display> themedDisplays = new HashSet<Gdk.Display> ();

		public static Theme UserInterfaceTheme { get; private set; }

		static bool? highContrastThemeEnabled;
		internal static bool HighContrastThemeEnabled {
			get {
				return highContrastThemeEnabled ?? false;
			}
			set {
				if (highContrastThemeEnabled != value) {
					highContrastThemeEnabled = value;
					UpdateStyles ();
				}
			}
		}

		static IdeTheme ()
		{
			// FIXME: Immediate theme switching disabled, until:
			//        MAC: NSAppearance issues are fixed
			//IdeApp.Preferences.UserInterfaceTheme.Changed += (sender, e) => UpdateGtkTheme ();
		}

		internal static bool AccessibilityEnabled { get; private set; }

		internal static void InitializeGtk (string progname, ref string[] args)
		{
			if (Gtk.Settings.Default != null)
				throw new InvalidOperationException ("Gtk already initialized!");

			IdeStartupTracker.StartupTracker.MarkSection ("PreGtkInitialization");
#if MAC
			// Early init Cocoa through xwt
			var loaded = NativeToolkitHelper.LoadCocoa ();
			IdeStartupTracker.StartupTracker.MarkSection ("XamarinMacInitialization");
#endif
			// GTK 4 exposes accessibility natively (Gtk.Accessible); the GTK 2 AtkCocoa module is gone.
			AccessibilityEnabled = true;

			Gtk.Application.Init (progname, ref args);
		}

		internal static void SetupXwtTheme ()
		{
			Xwt.Drawing.Context.RegisterStyles ("dark", "disabled", "error", "contrast");

			if (Core.Platform.IsMac) {
				Xwt.Drawing.Context.RegisterStyles ("mac", "sel");
				Xwt.Drawing.Context.SetGlobalStyle ("mac");
			} else if (Core.Platform.IsWindows) {
				Xwt.Drawing.Context.RegisterStyles ("win");
				Xwt.Drawing.Context.SetGlobalStyle ("win");
			} else if (Core.Platform.IsLinux) {
				Xwt.Drawing.Context.RegisterStyles ("linux");
				Xwt.Drawing.Context.SetGlobalStyle ("linux");
			}

			Xwt.Toolkit.CurrentEngine.RegisterBackend <Xwt.Backends.IWindowBackend, ThemedGtkWindowBackend>();
			Xwt.Toolkit.CurrentEngine.RegisterBackend <Xwt.Backends.IDialogBackend, ThemedGtkDialogBackend>();
		}

		internal static void SetupGtkTheme ()
		{
			if (Gtk.Settings.Default == null)
				return;

			if (Platform.IsLinux) {
				DefaultTheme = Gtk.Settings.Default.ThemeName;
				string theme = IdeApp.Preferences.UserInterfaceThemeName;
				if (string.IsNullOrEmpty (theme))
					theme = DefaultTheme;
				ValidateGtkTheme (ref theme);
				if (theme != DefaultTheme)
					Gtk.Settings.Default.ThemeName = theme;
				LoggingService.LogInfo ("GTK: Using Gtk theme {0}", Gtk.Settings.Default.ThemeName);
			} else
				DefaultTheme = "Light";

			UpdateGtkTheme ();
		}

		internal static void UpdateGtkTheme ()
		{
			if (DefaultTheme == null) {
				SetupGtkTheme ();
				return;
			}

			var settings = Gtk.Settings.Default;
			string current_theme = IdeApp.Preferences.UserInterfaceThemeName;

			if (!Platform.IsLinux) {
				UserInterfaceTheme = IdeApp.Preferences.UserInterfaceThemeName == "Dark" ? Theme.Dark : Theme.Light;
				if (settings != null) {
					// Only Light/Dark are allowed on Win/Mac: GTK's default theme in its light or dark variant.
					settings.ApplicationPreferDarkTheme = UserInterfaceTheme == Theme.Dark;
					LoggingService.LogInfo ("GTK: Using the {0} variant of the Gtk theme {1}", UserInterfaceTheme, settings.ThemeName);
				}
			} else if (settings != null && !string.IsNullOrEmpty (current_theme) && current_theme != settings.ThemeName) {
				LoggingService.LogInfo ("GTK: Using Gtk theme {0}", current_theme);
				settings.ThemeName = current_theme;
			}

			LoadThemeCss ();

			// Styles are updated by DefaultWorkbench (UpdateStyles) once the new theme has been applied.
		}

		/// <summary>
		/// Loads MonoDevelop's CSS for the current light/dark theme into the application provider and registers it
		/// for the default display. A <c>gtk4.css</c> / <c>gtk4-dark.css</c> file next to the entry assembly, if present,
		/// replaces the built-in palette (the role of the GTK 2 <c>gtkrc.*</c> files).
		/// </summary>
		static void LoadThemeCss ()
		{
			bool dark = UserInterfaceTheme == Theme.Dark;
			string css = null;
			try {
				var file = PropertyService.EntryAssemblyPath.Combine (dark ? "gtk4-dark.css" : "gtk4.css");
				if (File.Exists (file))
					css = File.ReadAllText (file);
			} catch (Exception ex) {
				LoggingService.LogError ("Could not read the theme CSS", ex);
			}
			// On Linux the system Gtk theme is used as is (GTK 2 also used no bundled gtkrc there).
			if (css == null)
				css = Platform.IsLinux ? string.Empty : (dark ? DarkPaletteCss : LightPaletteCss);

			if (themeCssProvider == null)
				themeCssProvider = new Gtk.CssProvider ();
			themeCssProvider.LoadFromString (css);

			EnsureThemeCss (Gdk.Display.Default);
		}

		static void EnsureThemeCss (Gdk.Display display)
		{
			if (themeCssProvider == null || display == null || themedDisplays.Contains (display))
				return;
			Gtk.StyleContext.AddProviderForDisplay (display, themeCssProvider, (uint)Gtk.StyleProviderPriority.Application);
			themedDisplays.Add (display);
		}

		// Palettes of the former gtkrc.win32 / gtkrc.win32-dark color schemes.
		const string LightPaletteCss = @"
@define-color theme_bg_color #f5f5f5;
@define-color theme_fg_color #000000;
@define-color theme_base_color #ffffff;
@define-color theme_text_color #000000;
@define-color theme_selected_bg_color #cce8ff;
@define-color theme_selected_fg_color #000000;
@define-color theme_unfocused_bg_color #f5f5f5;
@define-color theme_unfocused_fg_color #000000;
@define-color theme_unfocused_base_color #ffffff;
@define-color theme_unfocused_selected_bg_color #cce8ff;
@define-color theme_unfocused_selected_fg_color #000000;
@define-color insensitive_fg_color #767676;
@define-color borders #b2b2b2;
window, .background { background-color: @theme_bg_color; color: @theme_fg_color; }
.view, textview text, iconview, treeview.view { background-color: @theme_base_color; color: @theme_text_color; }
.view:selected, treeview.view:selected, selection { background-color: @theme_selected_bg_color; color: @theme_selected_fg_color; }
link, .link { color: #175fde; }
tooltip.background { background-color: #f2f2f2; color: #222222; border: 1px solid #b2b2b2; }
";

		const string DarkPaletteCss = @"
@define-color theme_bg_color #333333;
@define-color theme_fg_color #d7d7d7;
@define-color theme_base_color #404040;
@define-color theme_text_color #d7d7d7;
@define-color theme_selected_bg_color #4c5e6e;
@define-color theme_selected_fg_color #bfbfbf;
@define-color theme_unfocused_bg_color #333333;
@define-color theme_unfocused_fg_color #d7d7d7;
@define-color theme_unfocused_base_color #404040;
@define-color theme_unfocused_selected_bg_color #4c5e6e;
@define-color theme_unfocused_selected_fg_color #bfbfbf;
@define-color insensitive_fg_color #ababab;
@define-color borders #5a5a5a;
window, .background { background-color: @theme_bg_color; color: @theme_fg_color; }
.view, textview text, iconview, treeview.view { background-color: @theme_base_color; color: @theme_text_color; }
.view:selected, treeview.view:selected, selection { background-color: @theme_selected_bg_color; color: @theme_selected_fg_color; }
link, .link { color: #ace2ff; }
tooltip.background { background-color: #5a5a5a; color: #d2d5cd; border: 1px solid #b2b2b2; }
";

		internal static void UpdateStyles ()
		{
			if (Platform.IsLinux) {
				// GTK 4 has no Rc style to query the background color: a light foreground color means a dark theme.
				Gtk.Widget window = IdeApp.Workbench?.RootWindow;
				var settings = Gtk.Settings.Default;
				if (window != null)
					UserInterfaceTheme = HslColor.Brightness (window.StyleColor) > 0.5 ? Theme.Dark : Theme.Light;
				else if (settings != null)
					UserInterfaceTheme = settings.ApplicationPreferDarkTheme || (settings.ThemeName ?? "").EndsWith ("-dark", StringComparison.OrdinalIgnoreCase) ? Theme.Dark : Theme.Light;
			}

			if (UserInterfaceTheme == Theme.Dark)
				Xwt.Drawing.Context.SetGlobalStyle ("dark");
			else
				Xwt.Drawing.Context.ClearGlobalStyle ("dark");

			if (HighContrastThemeEnabled)
				Xwt.Drawing.Context.SetGlobalStyle ("contrast");
			else
				Xwt.Drawing.Context.ClearGlobalStyle ("contrast");

			Styles.LoadStyle ();
			UpdateXwtDefaults ();
			#if MAC
			UpdateMacWindows ();
			#endif
		}

		static void UpdateXwtDefaults ()
		{
			// Xwt default dialog icons
			Xwt.Toolkit.CurrentEngine.Defaults.MessageDialog.InformationIcon = ImageService.GetIcon ("gtk-dialog-info", StockIconSize.Dialog);
			Xwt.Toolkit.CurrentEngine.Defaults.MessageDialog.WarningIcon = ImageService.GetIcon ("gtk-dialog-warning", StockIconSize.Dialog);
			Xwt.Toolkit.CurrentEngine.Defaults.MessageDialog.ErrorIcon = ImageService.GetIcon ("gtk-dialog-error", StockIconSize.Dialog);
			Xwt.Toolkit.CurrentEngine.Defaults.MessageDialog.QuestionIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog);
			Xwt.Toolkit.CurrentEngine.Defaults.MessageDialog.ConfirmationIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog);

			if (Platform.IsMac && UserInterfaceTheme == Theme.Dark) {
				// dark NSAppearance can not handle custom drawn images in dialogs
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.InformationIcon = ImageService.GetIcon ("gtk-dialog-info", StockIconSize.Dialog).ToBitmap (GtkWorkarounds.GetScaleFactor ());
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.WarningIcon = ImageService.GetIcon ("gtk-dialog-warning", StockIconSize.Dialog).ToBitmap (GtkWorkarounds.GetScaleFactor ());
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.ErrorIcon = ImageService.GetIcon ("gtk-dialog-error", StockIconSize.Dialog).ToBitmap (GtkWorkarounds.GetScaleFactor ());
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.QuestionIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog).ToBitmap (GtkWorkarounds.GetScaleFactor ());
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.ConfirmationIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog).ToBitmap (GtkWorkarounds.GetScaleFactor ());
			} else {
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.InformationIcon = ImageService.GetIcon ("gtk-dialog-info", StockIconSize.Dialog);
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.WarningIcon = ImageService.GetIcon ("gtk-dialog-warning", StockIconSize.Dialog);
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.ErrorIcon = ImageService.GetIcon ("gtk-dialog-error", StockIconSize.Dialog);
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.QuestionIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog);
				Xwt.Toolkit.NativeEngine.Defaults.MessageDialog.ConfirmationIcon = ImageService.GetIcon ("gtk-dialog-question", StockIconSize.Dialog);
			}

			Xwt.Toolkit.CurrentEngine.Defaults.FallbackLinkColor = Styles.LinkForegroundColor;
			Xwt.Toolkit.NativeEngine.Defaults.FallbackLinkColor = Styles.LinkForegroundColor;
		}

		internal static string[] gtkThemeFallbacks = new string[] {
			"Xamarin",// the best!
			"Gilouche", // SUSE
			"Mint-X", // MINT
			"Radiance", // Ubuntu 'light' theme (MD looks better with the light theme in 4.0 - if that changes switch this one)
			"Clearlooks" // GTK theme
		};

		static void ValidateGtkTheme (ref string theme)
		{
			if (!MonoDevelop.Ide.Gui.OptionPanels.IDEStyleOptionsPanelWidget.IsBadGtkTheme (theme))
				return;

			var themes = MonoDevelop.Ide.Gui.OptionPanels.IDEStyleOptionsPanelWidget.InstalledThemes;

			string fallback = gtkThemeFallbacks
				.Select (fb => themes.FirstOrDefault (t => string.Compare (fb, t, StringComparison.OrdinalIgnoreCase) == 0))
				.FirstOrDefault (t => t != null);

			string message = "Theme Not Supported";

			string detail;
			if (themes.Count > 0) {
				detail =
					"Your system is using the '{0}' GTK+ theme, which is known to be very unstable. MonoDevelop will " +
					"now switch to an alternate GTK+ theme.\n\n" +
					"This message will continue to be shown at startup until you set a alternate GTK+ theme as your " +
					"default in the GTK+ Theme Selector or MonoDevelop Preferences.";
			} else {
				detail =
					"Your system is using the '{0}' GTK+ theme, which is known to be very unstable, and no other GTK+ " +
					"themes appear to be installed. Please install another GTK+ theme.\n\n" +
					"This message will continue to be shown at startup until you install a different GTK+ theme and " +
					"set it as your default in the GTK+ Theme Selector or MonoDevelop Preferences.";
			}

			MessageService.GenericAlert ("gtk-dialog-warning", message, BrandingService.BrandApplicationName (detail), AlertButton.Ok);

			theme = fallback ?? themes.FirstOrDefault () ?? theme;
		}

#if MAC
		static Dictionary<NSWindow, NSObject> nsWindows = new Dictionary<NSWindow, NSObject> ();

		internal static NSAppearance GetAppearance ()
		{
			return IdeApp.Preferences.UserInterfaceTheme == Theme.Light
				? NSAppearance.GetAppearance (NSAppearance.NameAqua)
				: MacSystemInformation.OsVersion < MacSystemInformation.Mojave
					? NSAppearance.GetAppearance (NSAppearance.NameVibrantDark)
					: NSAppearance.GetAppearance (new NSString ("NSAppearanceNameDarkAqua"));
		}

		public static void ApplyTheme (NSWindow window)
		{
			if (!nsWindows.ContainsKey(window)) {
				nsWindows [window] = NSNotificationCenter.DefaultCenter.AddObserver (NSWindow.WillCloseNotification, OnClose, window);
				SetTheme (window);
			}
		}

		static void SetTheme (NSWindow window)
		{
			window.Appearance = GetAppearance ();

			if (IdeApp.Preferences.UserInterfaceTheme == Theme.Light) {
				window.StyleMask &= ~NSWindowStyle.TexturedBackground;
				window.BackgroundColor = MonoDevelop.Ide.Gui.Styles.BackgroundColor.ToNSColor ();
				return;
			}

			if (window is NSPanel || window.ContentView.Class.Name != "GdkQuartzView") {
				window.BackgroundColor = MonoDevelop.Ide.Gui.Styles.BackgroundColor.ToNSColor ();
				if (MacSystemInformation.OsVersion <= MacSystemInformation.Sierra)
					window.StyleMask |= NSWindowStyle.TexturedBackground;
			} else {
				object[] platforms = Mono.Addins.AddinManager.GetExtensionObjects ("/MonoDevelop/Core/PlatformService");
				if (platforms.Length > 0) {
					var platformService = (MonoDevelop.Ide.Desktop.PlatformService)platforms [0];
					var image = Xwt.Drawing.Image.FromResource (platformService.GetType().Assembly, "maintoolbarbg.png");

					window.IsOpaque = false;
					window.BackgroundColor = NSColor.FromPatternImage (image.ToBitmap().ToNSImage());
				}
				window.StyleMask |= NSWindowStyle.TexturedBackground;
			}
			if (MacSystemInformation.OsVersion >= MacSystemInformation.HighSierra && !window.IsSheet)
				window.TitlebarAppearsTransparent = true;
		}

		static void OnClose (NSNotification note)
		{
			var w = (NSWindow)note.Object;
			if (MacSystemInformation.OsVersion < MacSystemInformation.HighSierra)
				// Since HighSierra observers don't need to be removed manually, doing so
				// after a window has been released might even lead to a native crash
				// see: https://developer.apple.com/library/archive/releasenotes/Foundation/RN-Foundation/index.html#10_11NotificationCenter
				NSNotificationCenter.DefaultCenter.RemoveObserver(nsWindows[w]);
			nsWindows.Remove (w);

		}

		static void UpdateMacWindows ()
		{
			foreach (var w in nsWindows.Keys)
				SetTheme (w);
		}

		static void OnGtkWindowRealized (object s, EventArgs a)
		{
			var nsw = MonoDevelop.Components.Mac.GtkMacInterop.GetNSWindow ((Gtk.Window) s);
			if (nsw != null)
				ApplyTheme (nsw);
		}
#endif

		public static void ApplyTheme (this Gtk.Window window)
		{
			// Make sure the theme CSS also reaches windows on other displays.
			if (window?.Display != null)
				EnsureThemeCss (window.Display);
			#if MAC
			window.Realized += OnGtkWindowRealized;
			if (window.IsRealized) {
				var nsw = MonoDevelop.Components.Mac.GtkMacInterop.GetNSWindow (window);
				if (nsw != null)
					ApplyTheme (nsw);
			}
			#endif
		}
	}

	public class ThemedGtkWindowBackend : Xwt.GtkBackend.WindowBackend
	{
		public override void Initialize ()
		{
			base.Initialize ();
			IdeTheme.ApplyTheme (Window);
		}
	}

	public class ThemedGtkDialogBackend : Xwt.GtkBackend.DialogBackend
	{
		public override void Initialize ()
		{
			base.Initialize ();
			IdeTheme.ApplyTheme (Window);
		}
	}
}

