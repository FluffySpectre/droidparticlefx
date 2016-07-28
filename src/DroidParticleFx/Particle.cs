using System;
using System.Collections.Generic;
using Android.Graphics;

using DroidParticleFx.Modifier;

namespace DroidParticleFx {
	public class Particle : IDisposable {
		public float CurrentX { get; set; }
		public float CurrentY { get; set; }

		public float Scale { get; set; }
		public int Alpha { get; set; }

		public float InitialRotation { get; set; }
		public float RotationSpeed { get; set; }

		public float SpeedX { get; set; }
		public float SpeedY { get; set; }

		public float AccelerationX { get; set; }
		public float AccelerationY { get; set; }

		public Paint Paint { get; set; }

		protected Bitmap Image;
		protected int startingMillisecond;

		Matrix Matrix = new Matrix();

		float initialX;
		float initialY;

		float rotation;

		int timeToLive;

		int bitmapHalfWidth;
		int bitmapHalfHeight;

		List<IParticleModifier> modifiers;

		public Particle(Bitmap bitmap) {
			Paint = new Paint();
			Image = bitmap;
		}

		public void Init() {
			Scale = 1;
			Alpha = 255;
		}

		public void Configure(int timeToLive, float emitterX, float emitterY) {
			bitmapHalfWidth = Image.Width / 2;
			bitmapHalfHeight = Image.Height / 2;

			initialX = emitterX - bitmapHalfWidth;
			initialY = emitterY - bitmapHalfHeight;
			CurrentX = initialX;
			CurrentY = initialY;

			this.timeToLive = timeToLive;
		}

		public bool Update(int milliseconds) {
			int realMilliseconds = milliseconds - startingMillisecond;
			if (realMilliseconds > timeToLive) {
				return false;
			}

			CurrentX = initialX + SpeedX * realMilliseconds + AccelerationX * realMilliseconds * realMilliseconds;
			CurrentY = initialY + SpeedY * realMilliseconds + AccelerationY * realMilliseconds * realMilliseconds;
			rotation = InitialRotation + RotationSpeed * realMilliseconds / 1000;

			for (int i = 0; i < modifiers.Count; i++) {
				modifiers[i].Apply(this, realMilliseconds);
			}

			return true;
		}

		public void Draw(Canvas c) {
			Matrix.Reset();
			Matrix.PostRotate(rotation, bitmapHalfWidth, bitmapHalfHeight);
			Matrix.PostScale(Scale, Scale, bitmapHalfWidth, bitmapHalfHeight);
			Matrix.PostTranslate(CurrentX, CurrentY);
			Paint.Alpha = Alpha;
			c.DrawBitmap(Image, Matrix, Paint);
		}

		public void Activate(int startingMillisecond, List<IParticleModifier> modifiers) {
			this.startingMillisecond = startingMillisecond;
			this.modifiers = modifiers;
		}

		public void Dispose() {
			if (Image != null) {
				Image.Recycle();
				Image = null;
			}
		}
	}
}

