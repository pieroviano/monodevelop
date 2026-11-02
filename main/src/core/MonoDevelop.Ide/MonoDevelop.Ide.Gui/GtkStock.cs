//
// GtkStock.cs
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
using MonoDevelop.Core;

namespace MonoDevelop.Ide.Gui
{
	/// <summary>
	/// Identifiers of the stock icons / buttons that GTK 2 provided as <c>Gtk.Stock</c> and that GTK 4 removed.
	/// The ids are MonoDevelop icon ids (see <c>StockIcons.addin.xml</c>, resolved by <see cref="ImageService"/>)
	/// and, for the ones that GTK used for buttons, also have a label (<see cref="GetLabel"/>).
	/// </summary>
	public static class GtkStock
	{
		public const string About = "gtk-about";
		public const string Add = "gtk-add";
		public const string Apply = "gtk-apply";
		public const string Bold = "gtk-bold";
		public const string Cancel = "gtk-cancel";
		public const string Clear = "gtk-clear";
		public const string Close = "gtk-close";
		public const string Copy = "gtk-copy";
		public const string Cut = "gtk-cut";
		public const string Delete = "gtk-delete";
		public const string DialogError = "gtk-dialog-error";
		public const string DialogInfo = "gtk-dialog-info";
		public const string DialogQuestion = "gtk-dialog-question";
		public const string DialogWarning = "gtk-dialog-warning";
		public const string Directory = "gtk-directory";
		public const string Edit = "gtk-edit";
		public const string Execute = "gtk-execute";
		public const string File = "gtk-file";
		public const string Find = "gtk-find";
		public const string FindAndReplace = "gtk-find-and-replace";
		public const string Fullscreen = "gtk-fullscreen";
		public const string GoBack = "gtk-go-back";
		public const string GoDown = "gtk-go-down";
		public const string GoForward = "gtk-go-forward";
		public const string GoUp = "gtk-go-up";
		public const string Help = "gtk-help";
		public const string Home = "gtk-home";
		public const string Info = "gtk-info";
		public const string MissingImage = "gtk-missing-image";
		public const string New = "gtk-new";
		public const string No = "gtk-no";
		public const string Ok = "gtk-ok";
		public const string Open = "gtk-open";
		public const string Paste = "gtk-paste";
		public const string Preferences = "gtk-preferences";
		public const string Print = "gtk-print";
		public const string PrintPreview = "gtk-print-preview";
		public const string Properties = "gtk-properties";
		public const string Quit = "gtk-quit";
		public const string Redo = "gtk-redo";
		public const string Refresh = "gtk-refresh";
		public const string Remove = "gtk-remove";
		public const string RevertToSaved = "gtk-revert-to-saved";
		public const string Save = "gtk-save";
		public const string SaveAs = "gtk-save-as";
		public const string SortAscending = "gtk-sort-ascending";
		public const string SortDescending = "gtk-sort-descending";
		public const string Stop = "gtk-stop";
		public const string Undo = "gtk-undo";
		public const string Yes = "gtk-yes";
		public const string ZoomIn = "gtk-zoom-in";
		public const string ZoomOut = "gtk-zoom-out";

		static readonly Dictionary<string, Func<string>> labels = new Dictionary<string, Func<string>> {
			{ About, () => GettextCatalog.GetString ("_About") },
			{ Add, () => GettextCatalog.GetString ("_Add") },
			{ Apply, () => GettextCatalog.GetString ("_Apply") },
			{ Bold, () => GettextCatalog.GetString ("_Bold") },
			{ Cancel, () => GettextCatalog.GetString ("_Cancel") },
			{ Clear, () => GettextCatalog.GetString ("C_lear") },
			{ Close, () => GettextCatalog.GetString ("_Close") },
			{ Copy, () => GettextCatalog.GetString ("_Copy") },
			{ Cut, () => GettextCatalog.GetString ("Cu_t") },
			{ Delete, () => GettextCatalog.GetString ("_Delete") },
			{ Edit, () => GettextCatalog.GetString ("_Edit") },
			{ Execute, () => GettextCatalog.GetString ("_Execute") },
			{ Find, () => GettextCatalog.GetString ("_Find") },
			{ FindAndReplace, () => GettextCatalog.GetString ("Find and _Replace") },
			{ Fullscreen, () => GettextCatalog.GetString ("_Fullscreen") },
			{ GoBack, () => GettextCatalog.GetString ("_Back") },
			{ GoDown, () => GettextCatalog.GetString ("_Down") },
			{ GoForward, () => GettextCatalog.GetString ("_Forward") },
			{ GoUp, () => GettextCatalog.GetString ("_Up") },
			{ Help, () => GettextCatalog.GetString ("_Help") },
			{ Home, () => GettextCatalog.GetString ("_Home") },
			{ Info, () => GettextCatalog.GetString ("_Information") },
			{ New, () => GettextCatalog.GetString ("_New") },
			{ No, () => GettextCatalog.GetString ("_No") },
			{ Ok, () => GettextCatalog.GetString ("_OK") },
			{ Open, () => GettextCatalog.GetString ("_Open") },
			{ Paste, () => GettextCatalog.GetString ("_Paste") },
			{ Preferences, () => GettextCatalog.GetString ("_Preferences") },
			{ Print, () => GettextCatalog.GetString ("_Print") },
			{ PrintPreview, () => GettextCatalog.GetString ("Print Pre_view") },
			{ Properties, () => GettextCatalog.GetString ("_Properties") },
			{ Quit, () => GettextCatalog.GetString ("_Quit") },
			{ Redo, () => GettextCatalog.GetString ("_Redo") },
			{ Refresh, () => GettextCatalog.GetString ("_Refresh") },
			{ Remove, () => GettextCatalog.GetString ("_Remove") },
			{ RevertToSaved, () => GettextCatalog.GetString ("_Revert") },
			{ Save, () => GettextCatalog.GetString ("_Save") },
			{ SaveAs, () => GettextCatalog.GetString ("Save _As") },
			{ SortAscending, () => GettextCatalog.GetString ("_Ascending") },
			{ SortDescending, () => GettextCatalog.GetString ("_Descending") },
			{ Stop, () => GettextCatalog.GetString ("_Stop") },
			{ Undo, () => GettextCatalog.GetString ("_Undo") },
			{ Yes, () => GettextCatalog.GetString ("_Yes") },
			{ ZoomIn, () => GettextCatalog.GetString ("Zoom _In") },
			{ ZoomOut, () => GettextCatalog.GetString ("Zoom _Out") },
		};

		/// <summary>The ids that have a button label (see <see cref="GetLabel"/>).</summary>
		public static IEnumerable<string> LabeledIds => labels.Keys;

		/// <summary>Gets the translated label (with mnemonic underscore) of a stock id, or null if it has none.</summary>
		public static string GetLabel (string stockId)
		{
			return stockId != null && labels.TryGetValue (stockId, out var label) ? label () : null;
		}
	}
}
