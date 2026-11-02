// XspOptionsPanelWidget.cs
// 
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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

using System;
using Gtk;

using MonoDevelop.Core;
using MonoDevelop.AspNet.Projects;
using MonoDevelop.Components;

namespace MonoDevelop.AspNet.Execution
{
	class XspOptionsPanelWidget : Box
	{
		Entry ipAddress = new Entry { WidthChars = 39 }; // sized for IPv6
		SpinButton portNumber = new SpinButton (0, Int16.MaxValue, 1) { WidthChars = 5} ;
		CheckButton verboseCheck = new CheckButton ();
		readonly ComboBoxText sslMode = new ComboBoxText ();
		readonly ComboBoxText sslProtocol = new ComboBoxText ();
		readonly ComboBoxText keyType = new ComboBoxText ();
		readonly ComboBoxText passwordOptions = new ComboBoxText ();
		readonly FileEntry keyLocation = new FileEntry ();
		readonly FileEntry certLocation = new FileEntry ();
		readonly Entry passwordEntry = new Entry { InvisibleChar = '●' };
		
		public XspOptionsPanelWidget (AspNetAppProjectFlavor project) : base (Orientation.Vertical, 6)
		{
			Build ();

			XspParameters xPar = project.XspParameters;
			
			//index should be equivalent to XspSslMode enum
			sslMode.RemoveAll ();
			sslMode.AppendText (GettextCatalog.GetString ("None"));
			sslMode.AppendText (GettextCatalog.GetString ("Enabled"));
			sslMode.AppendText (GettextCatalog.GetString ("Accept Client Certificates"));
			sslMode.AppendText (GettextCatalog.GetString ("Require Client Certificates"));
			
			//index should be equivalent to XspSslProtocol enum
			sslProtocol.RemoveAll ();
			sslProtocol.AppendText (GettextCatalog.GetString ("Default"));
#pragma warning disable MD0005
			sslProtocol.AppendText ("TLS");
			sslProtocol.AppendText ("SSL 2");
			sslProtocol.AppendText ("SSL 3");
#pragma warning restore MD0005

			keyType.RemoveAll ();
			keyType.AppendText (GettextCatalog.GetString ("None"));
#pragma warning disable MD0005
			keyType.AppendText ("Pkcs12");
			keyType.AppendText ("PVK");
#pragma warning restore MD0005

			passwordOptions.RemoveAll ();
			passwordOptions.AppendText (GettextCatalog.GetString ("None"));
			passwordOptions.AppendText (GettextCatalog.GetString ("Ask"));
			passwordOptions.AppendText (GettextCatalog.GetString ("Store (insecure)"));
			
			//load all options
			ipAddress.Text = xPar.Address;
			portNumber.Value = xPar.Port;
			verboseCheck.Active = xPar.Verbose;
			sslMode.Active = (int) xPar.SslMode;
			sslProtocol.Active = (int) xPar.SslProtocol;
			keyType.Active = (int) xPar.KeyType;
			keyLocation.Path = xPar.PrivateKeyFile;
			certLocation.Path = xPar.CertificateFile;
			passwordOptions.Active = (int) xPar.PasswordOptions;
			passwordEntry.Text = xPar.PrivateKeyPassword;
		}

		static Label CreateLabel (string text)
		{
			return new Label (text) { Xalign = 0, Halign = Align.Start };
		}

		static Label CreateHeader (string text)
		{
			return new Label { Markup = "<b>" + text + "</b>", Xalign = 0, Halign = Align.Start };
		}

		static Box CreateHBox ()
		{
			return new Box (Orientation.Horizontal, 6);
		}

		void Build ()
		{
			Append (CreateHeader (GettextCatalog.GetString ("General Options")));

			var gpadLabel = new Label { WidthRequest = 12 };
			var ipLabel = CreateLabel (GettextCatalog.GetString ("IP address:"));
			var ipHint = CreateLabel (GettextCatalog.GetString ("(blank = localhost)"));
			var ipHbox = CreateHBox ();
			ipHbox.Append (ipAddress);
			ipHbox.Append (ipHint);
			var portLabel = CreateLabel (GettextCatalog.GetString ("Port:"));
			var portHint = CreateLabel (GettextCatalog.GetString ("(0 = random)"));
			var portHbox = CreateHBox ();
			portHbox.Append (portNumber);
			portHbox.Append (portHint);
			verboseCheck.Label = GettextCatalog.GetString ("Verbose console output");
			verboseCheck.Halign = Align.Start;

			var ipTable = new Grid { ColumnSpacing = 6, RowSpacing = 6 };
			ipTable.Attach (gpadLabel, 0, 0, 1, 1);
			ipTable.Attach (ipLabel, 1, 0, 1, 1);
			ipTable.Attach (ipHbox, 2, 0, 1, 1);
			ipTable.Attach (portLabel, 1, 1, 1, 1);
			ipTable.Attach (portHbox, 2, 1, 1, 1);
			ipTable.Attach (verboseCheck, 1, 2, 2, 1);
			Append (ipTable);

			Append (CreateHeader (GettextCatalog.GetString ("Security")));

			var sslPadLabel = new Label { WidthRequest = 12 };
			var sslModeLabel = CreateLabel (GettextCatalog.GetString ("SSL mode:"));
			sslMode.Halign = Align.Start;
			var sslProtocolLabel = CreateLabel (GettextCatalog.GetString ("SSL protocol:"));
			sslProtocol.Halign = Align.Start;

			var sslTable = new Grid { ColumnSpacing = 6, RowSpacing = 6 };
			sslTable.Attach (sslPadLabel, 0, 0, 1, 1);
			sslTable.Attach (sslModeLabel, 1, 0, 1, 1);
			sslTable.Attach (sslMode, 2, 0, 1, 1);
			sslTable.Attach (sslProtocolLabel, 1, 1, 1, 1);
			sslTable.Attach (sslProtocol, 2, 1, 1, 1);
			Append (sslTable);

			Append (CreateHeader (GettextCatalog.GetString ("SSL Key")));

			var keyPadLabel = new Label { WidthRequest = 12 };
			var keyTypeLabel = CreateLabel (GettextCatalog.GetString ("Key type:"));
			keyType.Halign = Align.Start;
			var keyFileLabel = CreateLabel (GettextCatalog.GetString ("Key file:"));
			var certFileLabel = CreateLabel (GettextCatalog.GetString ("Certificate file:"));
			var passwordLabel = CreateLabel (GettextCatalog.GetString ("Password:"));
			var passwordHbox = CreateHBox ();
			passwordHbox.Append (passwordOptions);
			passwordEntry.Hexpand = true;
			passwordHbox.Append (passwordEntry);
			keyLocation.Hexpand = true;
			certLocation.Hexpand = true;
			passwordHbox.Hexpand = true;

			var keyTable = new Grid { ColumnSpacing = 6, RowSpacing = 6 };
			keyTable.Attach (keyPadLabel,     0, 0, 1, 1);
			keyTable.Attach (keyTypeLabel,    1, 0, 1, 1);
			keyTable.Attach (keyType,         2, 0, 1, 1);
			keyTable.Attach (keyFileLabel,    1, 1, 1, 1);
			keyTable.Attach (keyLocation,     2, 1, 2, 1);
			keyTable.Attach (certFileLabel,   1, 2, 1, 1);
			keyTable.Attach (certLocation,    2, 2, 2, 1);
			keyTable.Attach (passwordLabel,   1, 3, 1, 1);
			keyTable.Attach (passwordHbox,    2, 3, 2, 1);
			Append (keyTable);

			sslMode.Changed += UpdateSensitivity;
			keyType.Changed += UpdateSensitivity;
			passwordOptions.Changed += UpdateSensitivity;
		}
		
		public void Store (AspNetAppProjectFlavor project)
		{
			XspParameters xPar = project.XspParameters;
			
			xPar.Address = ipAddress.Text;
			xPar.Port = Convert.ToUInt16 (portNumber.Value);
			xPar.Verbose = verboseCheck.Active;
			xPar.SslMode = (XspSslMode) sslMode.Active;
			xPar.SslProtocol = (XspSslProtocol) sslProtocol.Active;
			xPar.KeyType = (XspKeyType) keyType.Active;
			xPar.PrivateKeyFile = keyLocation.Path;
			xPar.CertificateFile = certLocation.Path;
			xPar.PasswordOptions = (XspPasswordOptions) passwordOptions.Active;
			xPar.PrivateKeyPassword = passwordEntry.Text;
		}
		
		void UpdateSensitivity (object sender, EventArgs e)
		{
			bool sslEnabled = ((XspSslMode) sslMode.Active) != XspSslMode.None;
			sslProtocol.Sensitive = sslEnabled;
			keyType.Sensitive = sslEnabled;
			
			bool keyEnabled = (sslEnabled) && (keyType.Active != 0);
			keyLocation.Sensitive = keyEnabled;
			passwordOptions.Sensitive = keyEnabled;
			
			bool certEnabled = (keyEnabled) && (keyType.Active == 2);
			certLocation.Sensitive = certEnabled;
			
			passwordEntry.Sensitive = (keyEnabled) && (passwordOptions.Active == 2);
			if (!passwordEntry.Sensitive)
				passwordEntry.Text = "";
		}
	}
}
