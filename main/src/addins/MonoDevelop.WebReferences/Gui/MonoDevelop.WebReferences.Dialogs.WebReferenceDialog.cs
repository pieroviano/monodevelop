// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
namespace MonoDevelop.WebReferences.Dialogs
{
	internal partial class WebReferenceDialog
	{
		private global::Gtk.Button btnNavBack;
		private global::Gtk.Button btnNavNext;
		private global::Gtk.Button btnRefresh;
		private global::Gtk.Button btnStop;
		private global::Gtk.Button btnHome;
		private global::Gtk.Box vbxContainer;
		private global::Gtk.Box tlbNavigate;
		private global::Gtk.Grid tblWebReferenceUrl;
		private global::Gtk.Button btnGO;
		private global::Gtk.Label lblWebServiceUrl;
		private global::Gtk.Entry tbxReferenceURL;
		private global::Gtk.Frame frmBrowser;
		private global::Gtk.Grid tblReferenceName;
		private global::Gtk.Box hbox1;
		private global::Gtk.ComboBoxText comboModel;
		private global::Gtk.Label label1;
		private global::Gtk.Label lblNamespace;
		private global::Gtk.Label lblReference;
		private global::Gtk.Entry tbxNamespace;
		private global::Gtk.Entry tbxReferenceName;
		private global::Gtk.Button btnCancel;
		private global::Gtk.Button btnBack;
		private global::Gtk.Button btnOK;
		private global::Gtk.Button btnConfig;

		protected virtual void Build ()
		{
			this.Name = "MonoDevelop.WebReferences.Dialogs.WebReferenceDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Add Web Reference");

			var content = this.ContentArea;
			content.Name = "dlgWindow";
			content.MarginTop = content.MarginBottom = content.MarginStart = content.MarginEnd = 2;

			this.vbxContainer = new global::Gtk.Box (global::Gtk.Orientation.Vertical, 0);
			this.vbxContainer.Name = "vbxContainer";
			this.vbxContainer.Vexpand = true;

			// Navigation toolbar (GTK 4 has no Toolbar: a box with the "toolbar" style class and icon buttons)
			this.tlbNavigate = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 0);
			this.tlbNavigate.Name = "tlbNavigate";
			this.tlbNavigate.AddCssClass ("toolbar");
			this.btnNavBack = CreateToolButton ("btnNavBack", "go-previous-symbolic", global::Mono.Unix.Catalog.GetString ("Go back one page"));
			this.btnNavNext = CreateToolButton ("btnNavNext", "go-next-symbolic", global::Mono.Unix.Catalog.GetString ("Go forward one page"));
			this.btnRefresh = CreateToolButton ("btnRefresh", "view-refresh-symbolic", global::Mono.Unix.Catalog.GetString ("Reload current page"));
			this.btnStop = CreateToolButton ("btnStop", "process-stop-symbolic", global::Mono.Unix.Catalog.GetString ("Stop loading this page"));
			this.btnHome = CreateToolButton ("btnHome", "go-home-symbolic", global::Mono.Unix.Catalog.GetString ("Go back to the home page"));
			this.tlbNavigate.Append (this.btnNavBack);
			this.tlbNavigate.Append (this.btnNavNext);
			this.tlbNavigate.Append (this.btnRefresh);
			this.tlbNavigate.Append (this.btnStop);
			this.tlbNavigate.Append (this.btnHome);
			this.vbxContainer.Append (this.tlbNavigate);

			this.tblWebReferenceUrl = new global::Gtk.Grid ();
			this.tblWebReferenceUrl.Name = "tblWebReferenceUrl";
			this.tblWebReferenceUrl.RowSpacing = 6;
			this.tblWebReferenceUrl.ColumnSpacing = 6;

			this.lblWebServiceUrl = new global::Gtk.Label ((string)null);
			this.lblWebServiceUrl.Name = "lblWebServiceUrl";
			this.lblWebServiceUrl.Xalign = 0F;
			this.lblWebServiceUrl.Halign = global::Gtk.Align.Start;
			this.lblWebServiceUrl.MarginStart = this.lblWebServiceUrl.MarginEnd = this.lblWebServiceUrl.MarginTop = this.lblWebServiceUrl.MarginBottom = 4;
			this.lblWebServiceUrl.LabelProp = global::Mono.Unix.Catalog.GetString ("Web Service Url: ");
			this.tblWebReferenceUrl.Attach (this.lblWebServiceUrl, 0, 0, 1, 1);

			this.tbxReferenceURL = new global::Gtk.Entry ();
			this.tbxReferenceURL.CanFocus = true;
			this.tbxReferenceURL.Name = "tbxReferenceURL";
			this.tbxReferenceURL.Editable = true;
			this.tbxReferenceURL.Hexpand = true;
			this.tblWebReferenceUrl.Attach (this.tbxReferenceURL, 1, 0, 1, 1);

			this.btnGO = new global::Gtk.Button ();
			this.btnGO.CanFocus = true;
			this.btnGO.Name = "btnGO";
			this.btnGO.UseUnderline = true;
			this.btnGO.Label = global::Mono.Unix.Catalog.GetString ("_Go");
			this.btnGO.MarginStart = this.btnGO.MarginEnd = this.btnGO.MarginTop = this.btnGO.MarginBottom = 4;
			this.tblWebReferenceUrl.Attach (this.btnGO, 2, 0, 1, 1);
			this.vbxContainer.Append (this.tblWebReferenceUrl);

			this.frmBrowser = new global::Gtk.Frame ((string)null);
			this.frmBrowser.Name = "frmBrowser";
			this.frmBrowser.Vexpand = true;
			this.frmBrowser.Hexpand = true;
			this.vbxContainer.Append (this.frmBrowser);

			this.tblReferenceName = new global::Gtk.Grid ();
			this.tblReferenceName.Name = "tblReferenceName";
			this.tblReferenceName.RowSpacing = 4;
			this.tblReferenceName.ColumnSpacing = 4;
			this.tblReferenceName.MarginStart = this.tblReferenceName.MarginEnd = this.tblReferenceName.MarginTop = this.tblReferenceName.MarginBottom = 9;

			this.label1 = CreateLabel ("label1", global::Mono.Unix.Catalog.GetString ("Framework:"));
			this.tblReferenceName.Attach (this.label1, 0, 0, 1, 1);
			this.hbox1 = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 6);
			this.hbox1.Name = "hbox1";
			this.comboModel = new global::Gtk.ComboBoxText ();
			this.comboModel.AppendText (global::Mono.Unix.Catalog.GetString ("Windows Communication Foundation (WCF)"));
			this.comboModel.AppendText (global::Mono.Unix.Catalog.GetString (".NET 2.0 Web Services"));
			this.comboModel.Name = "comboModel";
			this.comboModel.Active = 0;
			this.comboModel.Hexpand = true;
			this.hbox1.Append (this.comboModel);
			this.hbox1.Hexpand = true;
			this.tblReferenceName.Attach (this.hbox1, 1, 0, 1, 1);

			this.lblReference = CreateLabel ("lblReference", global::Mono.Unix.Catalog.GetString ("Reference: "));
			this.tblReferenceName.Attach (this.lblReference, 0, 1, 1, 1);
			this.tbxReferenceName = new global::Gtk.Entry ();
			this.tbxReferenceName.CanFocus = true;
			this.tbxReferenceName.Name = "tbxReferenceName";
			this.tbxReferenceName.Editable = true;
			this.tbxReferenceName.Hexpand = true;
			this.tblReferenceName.Attach (this.tbxReferenceName, 1, 1, 1, 1);

			this.lblNamespace = CreateLabel ("lblNamespace", global::Mono.Unix.Catalog.GetString ("Namespace: "));
			this.tblReferenceName.Attach (this.lblNamespace, 0, 2, 1, 1);
			this.tbxNamespace = new global::Gtk.Entry ();
			this.tbxNamespace.Sensitive = false;
			this.tbxNamespace.CanFocus = true;
			this.tbxNamespace.Name = "tbxNamespace";
			this.tbxNamespace.Editable = true;
			this.tbxNamespace.Hexpand = true;
			this.tblReferenceName.Attach (this.tbxNamespace, 1, 2, 1, 1);
			this.vbxContainer.Append (this.tblReferenceName);

			content.Append (this.vbxContainer);

			// The Back, OK and Config buttons respond through their own handlers, so they are not dialog action widgets.
			var buttonBox = new global::Gtk.Box (global::Gtk.Orientation.Horizontal, 6);
			buttonBox.Name = "pnlActionArea";
			buttonBox.Halign = global::Gtk.Align.End;
			buttonBox.MarginTop = buttonBox.MarginBottom = buttonBox.MarginStart = buttonBox.MarginEnd = 5;
			content.Append (buttonBox);

			this.btnCancel = new global::Gtk.Button ();
			this.btnCancel.CanFocus = true;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseUnderline = true;
			this.btnCancel.Label = global::Mono.Unix.Catalog.GetString ("_Cancel");
			buttonBox.Append (this.btnCancel);

			this.btnBack = new global::Gtk.Button ();
			this.btnBack.Sensitive = false;
			this.btnBack.CanFocus = true;
			this.btnBack.Name = "btnBack";
			this.btnBack.UseUnderline = true;
			this.btnBack.Label = global::Mono.Unix.Catalog.GetString ("_Back");
			buttonBox.Append (this.btnBack);

			this.btnOK = new global::Gtk.Button ();
			this.btnOK.Sensitive = false;
			this.btnOK.CanFocus = true;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseUnderline = true;
			this.btnOK.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			buttonBox.Append (this.btnOK);

			this.btnConfig = new global::Gtk.Button ();
			this.btnConfig.Sensitive = false;
			this.btnConfig.CanFocus = true;
			this.btnConfig.Name = "btnConfig";
			this.btnConfig.UseUnderline = true;
			this.btnConfig.Label = global::Mono.Unix.Catalog.GetString ("_Config");
			buttonBox.Append (this.btnConfig);

			this.DefaultWidth = 652;
			this.DefaultHeight = 509;
			this.btnBack.Visible = false;
			this.btnOK.Visible = false;
			this.btnConfig.Visible = false;

			this.btnNavBack.Clicked += this.Browser_BackButtonClicked;
			this.btnNavNext.Clicked += this.Browser_NextButtonClicked;
			this.btnRefresh.Clicked += this.Browser_RefreshButtonClicked;
			this.btnStop.Clicked += this.Browser_StopButtonClicked;
			this.btnHome.Clicked += this.Browser_HomeButtonClicked;
			global::MonoDevelop.Components.EventControllerExtensions.AddKeyPressHandler (this.tbxReferenceURL, null, this.Browser_URLKeyReleased);
			this.btnGO.Clicked += this.Browser_GoButtonClicked;
			this.comboModel.Changed += this.OnComboModelChanged;
			this.btnBack.Clicked += this.OnBtnBackClicked;
			this.btnCancel.Clicked += (sender, e) => this.Respond (global::Gtk.ResponseType.Cancel);
			this.btnOK.Clicked += this.OnBtnOKClicked;
			this.btnConfig.Clicked += this.OnBtnConfigClicked;
		}

		static global::Gtk.Button CreateToolButton (string name, string iconName, string tooltip)
		{
			var button = new global::Gtk.Button ();
			button.Name = name;
			button.IconName = iconName;
			button.TooltipText = tooltip;
			button.HasFrame = false;
			return button;
		}

		static global::Gtk.Label CreateLabel (string name, string text)
		{
			var label = new global::Gtk.Label ((string)null);
			label.Name = name;
			label.Xalign = 0F;
			label.Halign = global::Gtk.Align.Start;
			label.LabelProp = text;
			return label;
		}
	}
}
