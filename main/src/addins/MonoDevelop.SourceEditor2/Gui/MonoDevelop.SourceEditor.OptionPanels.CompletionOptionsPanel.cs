// GTK 4 port of the former GUI designer file; field names are kept for the partial class.
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.SourceEditor.OptionPanels
{
	internal partial class CompletionOptionsPanel
	{
		private global::Gtk.Box vbox1;
		private global::Gtk.Box vbox5;
		private global::Gtk.CheckButton autoCodeCompletionCheckbutton;
		private global::Gtk.Box hbox6;
		private global::Gtk.Label fixed5;
		private global::Gtk.CheckButton automaticCompletionModeCheckbutton;
		private global::Gtk.Box hbox4;
		private global::Gtk.Label fixed3;
		private global::Gtk.CheckButton includeKeywordsCheckbutton;
		private global::Gtk.Box hbox5;
		private global::Gtk.Label fixed4;
		private global::Gtk.CheckButton includeCodeSnippetsCheckbutton;
		private global::Gtk.CheckButton showImportsCheckbutton;
		private global::Gtk.CheckButton insertParenthesesCheckbutton;
		private global::Gtk.Box hbox2;
		private global::Gtk.Label fixed1;
		private global::Gtk.CheckButton openingRadiobutton;
		private global::Gtk.Box hbox3;
		private global::Gtk.Label fixed2;
		private global::Gtk.CheckButton bothRadiobutton;

		static global::Gtk.Box Indented (global::Gtk.Box parent, out global::Gtk.Label spacer, global::Gtk.Widget child)
		{
			var row = DesignerLayout.HBox (6);
			spacer = new global::Gtk.Label ();
			spacer.MarginStart = 6;
			spacer.MarginEnd = 6;
			DesignerLayout.Pack (row, spacer);
			DesignerLayout.Pack (row, child);
			DesignerLayout.Pack (parent, row);
			return row;
		}

		protected virtual void Build ()
		{
			this.vbox1 = DesignerLayout.VBox (6);
			this.vbox5 = DesignerLayout.VBox (6);
			this.vbox5.MarginStart = 12;

			this.autoCodeCompletionCheckbutton = new global::Gtk.CheckButton ();
			this.autoCodeCompletionCheckbutton.TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Automatic Completion with Enter or Tab keys");
			this.autoCodeCompletionCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Show completion list after a character is typed");
			this.autoCodeCompletionCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.autoCodeCompletionCheckbutton);

			this.automaticCompletionModeCheckbutton = new global::Gtk.CheckButton ();
			this.automaticCompletionModeCheckbutton.TooltipMarkup = global::Mono.Unix.Catalog.GetString ("Enables automatic completion with the Space key or Punctuation");
			this.automaticCompletionModeCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Complete with Space or Punctuation");
			this.automaticCompletionModeCheckbutton.UseUnderline = true;
			this.hbox6 = Indented (this.vbox5, out this.fixed5, this.automaticCompletionModeCheckbutton);

			this.includeKeywordsCheckbutton = new global::Gtk.CheckButton ();
			this.includeKeywordsCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Include _keywords in completion list");
			this.includeKeywordsCheckbutton.UseUnderline = true;
			this.hbox4 = Indented (this.vbox5, out this.fixed3, this.includeKeywordsCheckbutton);

			this.includeCodeSnippetsCheckbutton = new global::Gtk.CheckButton ();
			this.includeCodeSnippetsCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("Include _code snippets in completion list");
			this.includeCodeSnippetsCheckbutton.UseUnderline = true;
			this.hbox5 = Indented (this.vbox5, out this.fixed4, this.includeCodeSnippetsCheckbutton);

			this.showImportsCheckbutton = new global::Gtk.CheckButton ();
			this.showImportsCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("_Show import items");
			this.showImportsCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.showImportsCheckbutton);

			this.insertParenthesesCheckbutton = new global::Gtk.CheckButton ();
			this.insertParenthesesCheckbutton.Label = global::Mono.Unix.Catalog.GetString ("A_utomatically insert parentheses after completion:");
			this.insertParenthesesCheckbutton.UseUnderline = true;
			DesignerLayout.Pack (this.vbox5, this.insertParenthesesCheckbutton);

			this.openingRadiobutton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("_Opening only"));
			this.hbox2 = Indented (this.vbox5, out this.fixed1, this.openingRadiobutton);

			this.bothRadiobutton = DesignerLayout.Radio (global::Mono.Unix.Catalog.GetString ("_Both opening and closing"), this.openingRadiobutton);
			this.hbox3 = Indented (this.vbox5, out this.fixed2, this.bothRadiobutton);

			DesignerLayout.Pack (this.vbox1, this.vbox5);
			DesignerLayout.SetContent (this, this.vbox1);
		}
	}
}
