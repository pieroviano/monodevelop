using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;
using Microsoft.VisualStudio.Platform;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Editor.Highlighting;
using MonoDevelop.Ide.Fonts;
using MonoDevelop.Ide.Gui;

namespace Microsoft.VisualStudio.Language.Intellisense.Implementation
{
    class DefaultSignatureHelpPresenterSurfaceElement
    {
        SignatureHelpSessionView dataContext;
        public SignatureHelpSessionView DataContext
        {
            get
            {
                return dataContext;
            }
            set
            {
                if (value == dataContext)
                    return;
                if (dataContext != null)
                    dataContext.PropertyChanged -= DataContext_PropertyChanged;
                dataContext = value;
                if (dataContext != null)
                    dataContext.PropertyChanged += DataContext_PropertyChanged;
                DataContext_PropertyChanged (this, new System.ComponentModel.PropertyChangedEventArgs (""));
            }
        }

        private void DataContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RemoveAllChildren(vb);
            if (DataContext != null)
            {
                DataContext.SignatureWpfViewVisualElement.Hexpand = DataContext.SignatureWpfViewVisualElement.Vexpand = true;
                descriptionBox.Hexpand = descriptionBox.Vexpand = true;
                vb.Append(DataContext.SignatureWpfViewVisualElement);
                vb.Append(descriptionBox);
            }
            ShowTooltipInfo();
        }

        static void RemoveAllChildren(Gtk.Box box)
        {
            while (box.FirstChild != null)
                box.Remove(box.FirstChild);
        }

        Gtk.Box descriptionBox = new Gtk.Box(Orientation.Vertical, 0);
        Gtk.Box vb = new Gtk.Box(Orientation.Vertical, 0);
        Gtk.Box vb2 = new Gtk.Box(Orientation.Vertical, 0);
        Cairo.Color foreColor;
        public Xwt.Widget Content;
        private XwtPopupWindowTheme Theme { get => ((XwtThemedPopup)Content.ParentWindow).Theme; }
        public DefaultSignatureHelpPresenterSurfaceElement()
        {
            descriptionBox.Spacing = 4;


            Gtk.Box hb = new Gtk.Box(Orientation.Horizontal, 0);
            vb.Hexpand = vb.Vexpand = true;
            hb.Append(vb);

            vb2.Spacing = 4;
            hb.Hexpand = hb.Vexpand = true;
            vb2.Append(hb);

            // GTK 4 widgets have no ParentSet signal: the parent window is checked when the widget is mapped.
            vb2.Mapped += Vb2_Mapped;
            Content = Xwt.Toolkit.CurrentEngine.WrapWidget(vb2, Xwt.NativeWidgetSizing.DefaultPreferredSize);

            Styles.Changed += HandleThemeChanged;
            IdeApp.Preferences.ColorScheme.Changed += HandleThemeChanged;
            Content.Disposed += Content_Disposed;
        }

        private void Vb2_Mapped(object o, EventArgs args)
        {
            ShowTooltipInfo();
        }

        void UpdateStyle()
        {
            var scheme = SyntaxHighlightingService.GetEditorTheme(IdeApp.Preferences.ColorScheme);
            if (!scheme.FitsIdeTheme(IdeApp.Preferences.UserInterfaceTheme))
                scheme = SyntaxHighlightingService.GetDefaultColorStyle(IdeApp.Preferences.UserInterfaceTheme);
            Theme.SetSchemeColors(scheme);
            Theme.Font = IdeServices.FontService.SansFont.CopyModified(Styles.FontScale11).ToXwtFont();
            Theme.ShadowColor = Styles.PopoverWindow.ShadowColor;
            foreColor = Styles.PopoverWindow.DefaultTextColor.ToCairoColor();

            if (DataContext != null)
            {
                MonoDevelop.SourceEditor.EditorWidgetStyle.SetForeground(DataContext.SignatureWpfViewVisualElement, foreColor);
                DataContext.SignatureWpfViewVisualElement.FontDescription = IdeServices.FontService.GetFontDescription("Editor").CopyModified(Styles.FontScale11);
            }
            //if (this.Visible)
            //	QueueDraw ();
        }

        void HandleThemeChanged(object sender, EventArgs e)
        {
            UpdateStyle();
        }


        private void Content_Disposed(object sender, EventArgs e)
        {
            if (Content == null)
                return;
            Styles.Changed -= HandleThemeChanged;
            IdeApp.Preferences.ColorScheme.Changed -= HandleThemeChanged;
            Content.Disposed -= Content_Disposed;
            ((XwtThemedPopup)Content.ParentWindow).PagerLeftClicked -= DownButtonClick;
            ((XwtThemedPopup)Content.ParentWindow).PagerRightClicked -= UpButtonClick;
            Content = null;
        }
        bool arrowEventsRegistered;
        void ShowTooltipInfo()
        {
            if (Content.ParentWindow == null)
                return;
            if (DataContext == null)
                return;
            if (!arrowEventsRegistered)
            {
                UpdateStyle();
                arrowEventsRegistered = true;
                ((XwtThemedPopup)Content.ParentWindow).PagerLeftClicked += DownButtonClick;
                ((XwtThemedPopup)Content.ParentWindow).PagerRightClicked += UpButtonClick;
            }
            Theme.NumPages = DataContext.Session.Signatures.Count;
            Theme.CurrentPage = DataContext.Session.Signatures.IndexOf(dataContext.Session.SelectedSignature);

            if (Theme.NumPages > 1)
            {
                Theme.DrawPager = true;
                Theme.PagerVertical = true;
            }
            ClearDescriptions();
            if (Theme.DrawPager)
                DataContext.SignatureWpfViewVisualElement.WidthRequest = DataContext.SignatureWpfViewVisualElement.RealWidth + 70;

            if (!string.IsNullOrEmpty(dataContext.Session.SelectedSignature.Documentation))
            {
                var category = CreateCategory(TooltipInformationWindow.GetHeaderMarkup(GettextCatalog.GetString("Summary")), dataContext.Session.SelectedSignature.Documentation);
                category.Hexpand = category.Vexpand = true;
                category.MarginTop = category.MarginBottom = 4;
                descriptionBox.Append(category);
            }

            Content.QueueForReallocate();
        }

        public event EventHandler UpButtonClick;
        public event EventHandler DownButtonClick;

        void CurrentTooltipInformation_Changed(object sender, EventArgs e)
        {
            ShowTooltipInfo();
        }

        void ClearDescriptions()
        {
            RemoveAllChildren(descriptionBox);
        }

        Gtk.Box CreateCategory(string categoryName, string categoryContentMarkup)
        {
            return TooltipInformationWindow.CreateCategory(categoryName, categoryContentMarkup, foreColor, Theme.Font.ToPangoFont());
        }

        internal void Hide()
        {
            //Content.Hide ();
        }
    }
}