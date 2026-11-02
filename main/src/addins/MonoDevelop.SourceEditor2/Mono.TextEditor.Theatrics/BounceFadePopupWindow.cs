// 
// BounceFadePopupWindow.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
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
using Gdk;
using MonoDevelop.Core;
using MonoDevelop.Components;

namespace Mono.TextEditor.Theatrics
{
	/// <summary>
	/// Tooltip that "bounces", then fades away.
	/// </summary>
	abstract class BounceFadePopupWidget : Gtk.Widget
	{
		Stage<BounceFadePopupWidget> stage = new Stage<BounceFadePopupWidget> ();
		Gdk.Pixbuf textImage = null;
		MonoTextEditor editor;
		
		protected double scale = 0.0;
		protected double opacity = 1.0;
		
		public BounceFadePopupWidget (MonoTextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException ("Editor");
			this.editor = editor;
			Duration = 500;
			ExpandWidth = 12;
			ExpandHeight = 2;
			BounceEasing = Easing.Sine;
			CanTarget = false;

			stage.ActorStep += OnAnimationActorStep;
			stage.Iteration += OnAnimationIteration;
			stage.UpdateFrequency = 10;
		}
		
		protected MonoTextEditor Editor { get { return editor; } }
		
		/// <summary>Duration of the animation, in milliseconds.</summary>
		public uint Duration { get; set; }
		
		/// <summary>The number of pixels by which the window's width will expand</summary>
		public uint ExpandWidth { get; set; }
		
		/// <summary>The number of pixels by which the window's height will expand</summary>
		public uint ExpandHeight { get; set; }

		/// <summary>The easing used for the bounce part of the animation.</summary>
		public Easing BounceEasing { get; set; }

		int xExpandedOffset, yExpandedOffset;
		Cairo.Rectangle userspaceArea;
		Cairo.Rectangle bounds;

		public virtual void Popup ()
		{
			if (!editor.IsRealized){
				editor.Realized += HandleRealized;
				return;
			}

			bounds = CalculateInitialBounds ();

			//GTK uses integer position coords, so round fractions down, we'll add them back later as draw offsets
			int x = (int) System.Math.Floor (bounds.X);
			int y = (int) System.Math.Floor (bounds.Y);

			//capture any lost fractions to pass as an offset to Draw
			userspaceArea = new Cairo.Rectangle (bounds.X - x, bounds.Y - y, bounds.Width, bounds.Height);

			//lose half-pixels on the expansion, it's not a big deal
			xExpandedOffset = (int) (System.Math.Floor (ExpandWidth / 2d));
			yExpandedOffset = (int) (System.Math.Floor (ExpandHeight / 2d));

			//round the width/height up to make sure we have room for restored fractions
			int width = System.Math.Max (1, (int) (System.Math.Ceiling (bounds.Width) + ExpandWidth));
			int height = System.Math.Max (1, (int) (System.Math.Ceiling (bounds.Height) + ExpandHeight));
			this.SetSizeRequest (width, height);
			editor.TextArea.AddTopLevelWidget (this, x - xExpandedOffset, y - yExpandedOffset);

			stage.AddOrReset (this, Duration);
			stage.Play ();
			Visible = true;
		}

		void HandleRealized (object sender, EventArgs e)
		{
			editor.Realized -= HandleRealized;
			Popup ();
		}

		void OnAnimationIteration (object sender, EventArgs args)
		{
			QueueDraw ();
		}
		
		protected virtual bool OnAnimationActorStep (Actor<BounceFadePopupWidget> actor)
		{
			if (actor.Expired) {
				OnAnimationCompleted ();
				return false;
			}
			
			// for the first half, use an easing
			if (actor.Percent < 0.5) {
				scale = Choreographer.Compose (actor.Percent * 2, BounceEasing);
				opacity = 1.0;
			}
			//for the second half, vary opacity linearly from 1 to 0.
			else {
				scale = Choreographer.Compose (1.0, BounceEasing);
				opacity = 1 - 2 * (actor.Percent - 0.5);
			}
			return true;
		}

		protected override void OnSnapshot (Gtk.Snapshot snapshot)
		{
			try {
				int width = Width, height = Height;
				if (width <= 0 || height <= 0)
					return;
				snapshot.DrawCairo (this, cr => {
					cr.Translate (xExpandedOffset * (1 - scale), yExpandedOffset * (1 - scale));
					var scaleX = (width / userspaceArea.Width - 1) * scale + 1;
					var scaleY = (height / userspaceArea.Height - 1) * scale + 1;
					cr.Scale (scaleX, scaleY);
					Draw (cr, userspaceArea);
				});
			} catch (Exception e) {
				LoggingService.LogError ("Exception in animation:", e);
			}
		}

		protected abstract void Draw (Cairo.Context context, Cairo.Rectangle area);

		protected virtual void OnAnimationCompleted ()
		{
			StopPlaying ();
		}
		
		bool destroyed;

		/// <summary>Raised when the popup is destroyed (GTK 4 widgets have no Destroyed event).</summary>
		public event EventHandler Destroyed;

		public override void Destroy ()
		{
			if (!destroyed) {
				destroyed = true;
				StopPlaying ();
				OnDestroyed ();
				editor.TextArea.Remove (this);
				Destroyed?.Invoke (this, EventArgs.Empty);
			}
			base.Destroy ();
		}

		protected virtual void OnDestroyed ()
		{
		}
		
		internal virtual void StopPlaying ()
		{
			stage.Playing = false;
			
			if (textImage != null) {
				textImage.Dispose ();
				textImage = null;
			}
		}

		protected abstract Cairo.Rectangle CalculateInitialBounds ();
	}
}
