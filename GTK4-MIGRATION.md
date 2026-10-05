# GTK 2 → GTK 4 migration (branch `gtk4`)

## Blocking checks (before porting)

| Check | Status |
|---|---|
| GtkSharp4 packages build (`dotnet cake build.cake` in `D:\CommonLibrary\GtkSharp`) | version `4.22.4.<yyddd>` |
| Gtk 4 runtime at `%LOCALAPPDATA%\Gtk\4.22.4` (installed by `GtkSharp.targets`) | present |
| Package feed: `GtkSharp\BuildOutput\NugetPackages` added to `NuGet.config` (the stated `C:\starb\Packages` does not exist) | see `NuGet.config` |

## Decisions

| Decision | Consequence |
|---|---|
| **Native port** — MonoDevelop rewritten against idiomatic GTK 4 | GtkSharp `Gtk/Compat` types (`Gtk.Container`, `EventBox`, `ButtonPressEvent`, `ShowAll`, `OnDrawn`…) are **not** used. Missing *bindings* may be added to GtkSharp; no new compat shims. |
| **New `Xwt.Gtk4` backend** in `external/xwt` | `Xwt.Gtk` (GTK 2) stays untouched; MonoDevelop references `Xwt.Gtk4`. Submodule is `mono/xwt`, detached HEAD: commits are local only. |
| **Everything at once** | All Main.sln projects ported before first run. |
| **Keep Linux build**, keep `MonoDevelop.GtkCore` (Stetic) and `MacPlatform` | Stetic is ported to GTK 4 too; MacPlatform keeps its Mac-only conditions. |

## Build wiring

- `main/msbuild/ReferencesGtk.props`: `Reference gtk/gdk/glib/pango/atk/glade-sharp 2.12` + `Mono.Cairo` → `PackageReference` `GtkSharp4`, `GdkSharp4`, `GLibSharp4`, `GioSharp4`, `PangoSharp4`, `CairoSharp4`, `GskSharp4`, `GrapheneSharp4` (version `4.22.4.*`). `atk-sharp` and `glade-sharp` have no GTK 4 equivalent and are removed.
- Projects with inline `gtk-sharp 2.12` references switch to `$(ReferencesGtk)`.
- `Xwt.Gtk.csproj` refs → `Xwt.Gtk4.csproj`; toolkit loaded as `ToolkitType.Gtk4`.

## Porting rules (GTK 2 → GTK 4)

| GTK 2 | GTK 4 |
|---|---|
| `Gtk.Container`/`Bin` subclass with `Add`/`ForAll`/`OnSizeRequested`/`OnSizeAllocated` | `Gtk.Widget` subclass; children via `child.SetParent(this)` / `Unparent()`; `OnMeasure` + `OnSizeAllocate(w,h,baseline)` calling `child.Allocate(...)`; prefer stock `Box`/`Grid`/`Overlay`/`Fixed`/`Paned` |
| `EventBox` | the inner widget + event controllers |
| `OnExposeEvent` / `ExposeEvent` + `CairoHelper.Create(GdkWindow)` | `OnSnapshot(Gtk.Snapshot)` → `snapshot.AppendCairo(rect)`; or `DrawingArea.SetDrawFunc` |
| `Gdk.GC`, `GdkWindow.Draw*`, `Style.Paint*` | Cairo / `Gtk.Snapshot` render nodes; `StyleContext`/CSS classes |
| `ButtonPress/ReleaseEvent` | `GestureClick` (`Pressed`/`Released`) |
| `MotionNotify`/`Enter`/`LeaveNotifyEvent` | `EventControllerMotion` |
| `KeyPress/ReleaseEvent` | `EventControllerKey` (`KeyPressed` returns handled) |
| `ScrollEvent` | `EventControllerScroll` |
| `FocusIn/OutEvent` | `EventControllerFocus` |
| `Gtk.Menu`/`MenuItem`/`MenuBar` | `Gio.Menu` + `Gio.SimpleAction` + `PopoverMenu`/`PopoverMenuBar` |
| `Toolbar`/`ToolButton` | `Box` with `.toolbar` CSS class + `Button` |
| Popup `Window` + `Move(x,y)` (tooltips, completion, popovers) | `Gtk.Popover` with `SetPointingTo`, `HasArrow=false`, `Autohide` as needed |
| `Window.Move/GetPosition` | removed — no positioning in GTK 4; keep only `SetDefaultSize` |
| `Show/ShowAll`/`Hide` + `NoShowAll` | widgets visible by default; `Visible = bool` |
| `Destroy()` | `Window.Destroy()` for toplevels; otherwise `Unparent()`/remove from parent |
| `Dialog.Run()` | async: `Response` signal + nested `GLib.MainLoop` where MonoDevelop's API is synchronous (`MessageService.RunCustomDialog`) |
| `Gtk.Clipboard` | `Gdk.Clipboard` (async read, `SetText`) via `widget.Clipboard` / `Display.Clipboard` |
| `Drag.*` | `DragSource` / `DropTarget` controllers |
| `Gdk.Pixbuf` drawing | `Gdk.Texture.NewForPixbuf`, `Gtk.Picture`/`Gtk.Image.SetFromPaintable`; Cairo: `Gdk.CairoHelper.SetSourcePixbuf` |
| `Gtk.Style`, `ModifyBg/Fg/Font` | `CssProvider` + CSS classes |
| `Gdk.Window` (GdkWindow) | `Gdk.Surface` (toplevel only) / `widget.Native` |
| `Atk` | `Gtk.Accessible` (`UpdateProperty`, `AccessibleRole`) |
| `Gtk.Application.Init/Run/Quit` | `Gtk.Application` + `GLib.MainLoop` (MonoDevelop drives its own loop) |
| `TreeView`/`ComboBox`/`CellRenderer` | kept (deprecated, still in GTK 4.22) — port API differences only |

Shared GTK 4 helpers live in `MonoDevelop.Components` (`main/src/core/MonoDevelop.Ide/MonoDevelop.Components/`), not in GtkSharp.

## Batches (≤ 5 agents concurrently)

| # | Scope | Depends on |
|---|---|---|
| 0 | Build wiring, this doc | — |
| 1 | `Xwt.Gtk4` (new backend); `Mono.Addins.Gui`; `MonoDevelop.Core` | 0 |
| 2 | `MonoDevelop.Ide` foundation: `MonoDevelop.Components` core, Commands, Docking, DockNotebook, Theming | 1 |
| 3 | `MonoDevelop.Ide` remainder (split by folder) | 2 |
| 4 | `SourceEditor2` / `Mono.TextEditor.Shared`, Debugger, DesignerSupport, VersionControl*, CSharpBinding, Refactoring | 3 |
| 5 | `GtkCore` + `libstetic` + `libsteticui`; remaining addins, `mdmonitor`, F# binding, WindowsPlatform/GnomePlatform, tests | 3 |

Agents in one batch build with a private `IntermediateOutputPath`/`OutputPath` and `BuildProjectReferences=false` to avoid obj/ clashes.

## Verification

- `winbuild.bat` (DebugWin32) builds Main.sln with 0 errors; Linux `msbuild Main.sln /p:Configuration=Debug`.
- IDE launches, opens a solution, edits/builds/debugs a console app.
- `tests/IdeUnitTests`, `tests/Ide.Tests` pass.

## Risks

| Risk | Mitigation |
|---|---|
| No window positioning in GTK 4 (dock floating windows, popups) | Popovers anchored to widgets; floating pads become plain windows |
| Modal `Run()` removed | nested main loop in MonoDevelop's dialog service only |
| Missing GtkSharp bindings surface as `NullReferenceException` at call time | add binding + GtkSharp test (`BindingTests`) |
| xwt submodule commits are on detached HEAD of `mono/xwt` | push to a fork before `git submodule update` discards them |
