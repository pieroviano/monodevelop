//
// Styles.cs
//
// Author:
//       Vsevolod Kukol <sevo@xamarin.com>
//
// Copyright (c) 2015 Xamarin Inc. (http://www.xamarin.com)
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
using MonoDevelop.Ide;
using Xwt.Drawing;

namespace MonoDevelop.Gettext
{
	public static class Styles
	{
		public static POEditorStyle POEditor { get; internal set; }

		public class POEditorStyle
		{
			public Gdk.RGBA EntryUntranslatedBackgroundColor { get; internal set; }
			public Gdk.RGBA EntryMissingBackgroundColor { get; internal set; }
			public Gdk.RGBA EntryFuzzyBackgroundColor { get; internal set; }
			public Gdk.RGBA TabBarBackgroundColor { get; internal set; }
		}

		static Gdk.RGBA Rgb (int r, int g, int b)
		{
			return new Gdk.RGBA { Red = r / 255f, Green = g / 255f, Blue = b / 255f, Alpha = 1 };
		}

		static Styles ()
		{
			LoadStyles ();
			MonoDevelop.Ide.Gui.Styles.Changed +=  (o, e) => LoadStyles ();
		}

		public static void LoadStyles ()
		{
			POEditor = new POEditorStyle ();
			if (IdeApp.Preferences.UserInterfaceTheme == Theme.Light) {
				POEditor.EntryUntranslatedBackgroundColor = Rgb (234, 232, 227);
				POEditor.EntryMissingBackgroundColor = Rgb (237, 226, 187);
				POEditor.EntryFuzzyBackgroundColor = Rgb (255, 199, 186);
				POEditor.TabBarBackgroundColor = Rgb (241, 241, 241);
			} else {
				POEditor.EntryUntranslatedBackgroundColor = Rgb (255, 238, 194);
				POEditor.EntryMissingBackgroundColor = Rgb (255, 0, 255); // TODO: VV
				POEditor.EntryFuzzyBackgroundColor = Rgb (255, 195, 183);
				POEditor.TabBarBackgroundColor = Rgb (51, 51, 51);
			}
		}
	}
}

