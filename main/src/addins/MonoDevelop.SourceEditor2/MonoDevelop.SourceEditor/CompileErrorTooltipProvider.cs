// CompileErrorTooltipProvider.cs
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
using MonoDevelop.Ide.Editor;
using MonoDevelop.Components;
using System.Threading.Tasks;
using System.Threading;

namespace MonoDevelop.SourceEditor
{
	class CompileErrorTooltipProvider: TooltipProvider
	{
		internal static ExtensibleTextEditor GetExtensibleTextEditor (TextEditor editor)
		{
			var view = editor.GetContent<SourceEditorView> ();
			if (view == null)
				return null;
			return view.TextEditor;
		}

		#region ITooltipProvider implementation 
		public override Task<TooltipItem> GetItem (TextEditor editor, DocumentContext ctx, int offset, CancellationToken token = default(CancellationToken))
		{
			var ed = GetExtensibleTextEditor (editor);
			if (ed == null)
				return Task.FromResult<TooltipItem> (null);

			string errorInformation = ed.GetErrorInformationAt (offset);
			if (string.IsNullOrEmpty (errorInformation))
				return Task.FromResult<TooltipItem> (null);

			return Task.FromResult (new TooltipItem (errorInformation, editor.GetLineByOffset (offset)));
		}

		public override Window CreateTooltipWindow (TextEditor editor, DocumentContext ctx, TooltipItem item, int offset, Xwt.ModifierKeys modifierState)
		{
			var result = new LanguageItemWindow (GetExtensibleTextEditor (editor), modifierState, null, (string)item.Item, null);
			if (result.IsEmpty)
				return null;
			return result;
		}
		
		public override void GetRequiredPosition (TextEditor editor, Window tipWindow, out int requiredWidth, out double xalign)
		{
			var win = (LanguageItemWindow) tipWindow;
			var ed = GetExtensibleTextEditor (editor);
			requiredWidth = win.SetMaxWidth ((ed != null ? ed.Width : 800) / 4);
			xalign = 0.5;
		}

		public override void ShowTooltipWindow (TextEditor editor, Window tipWindow, TooltipItem item, Xwt.ModifierKeys modifierState, int mouseX, int mouseY)
		{
			var win = tipWindow as LanguageItemWindow;
			var ed = GetExtensibleTextEditor (editor);
			if (win == null || ed == null) {
				base.ShowTooltipWindow (editor, tipWindow, item, modifierState, mouseX, mouseY);
				return;
			}
			// the mouse position is relative to the text view margin
			win.ShowPopup (ed.TextArea, new Gdk.Rectangle (mouseX + (int)ed.TextViewMargin.XOffset, mouseY, 1, 1));
		}

		public override bool TryCloseTooltipWindow (Window tipWindow, TooltipCloseReason reason)
		{
			if (tipWindow is LanguageItemWindow win) {
				win.Destroy ();
				return true;
			}
			return base.TryCloseTooltipWindow (tipWindow, reason);
		}
		#endregion 
	}
}
