using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Animation;
using Android.Util;

using DroidParticleFx.Initializer;
using DroidParticleFx.Modifier;

namespace DroidParticleFx {
	public class ParticleSystemView : View {
		int maxParticles;
		List<Particle> particlePool = new List<Particle>();
		List<Particle> activeParticles = new List<Particle>();
		List<IParticleInitializer> initializers = new List<IParticleInitializer>();
		List<IParticleModifier> modifiers = new List<IParticleModifier>();

		Random rnd = new Random();
		int timeToLive;
		int emittingDuration;
		float particlesPerMillisecond;
		int numOfActivatedParticles;
		float dpToPxScale;
		ValueAnimator animator;
		int emitterX, emitterY;
		int emitterXMax, emitterYMax;

		public ParticleSystemView(Context context, int maxParticles, Drawable particleDrawable, int timeToLive) : base(context) {
			this.maxParticles = maxParticles;
			this.timeToLive = timeToLive;
			this.dpToPxScale = context.Resources.DisplayMetrics.Density;

			BuildParticlePool(particleDrawable);
		}
		public ParticleSystemView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(attrs); }
		public ParticleSystemView(Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle) { InitializeFromAttributes(attrs); }

		public void SetEmitterPosition(int left, int top, int right = 0, int bottom = 0) {
			emitterX = left;
			emitterY = top;
			emitterXMax = right;
			emitterYMax = bottom;
		}

		public void Emit(int particlesPerSecond, int duration) {
			StartEmitting(particlesPerSecond, duration);
		}

		public void StopEmitting() {
			if (animator != null) {
				animator.Cancel();
				animator.Dispose();
				animator = null;
			}
		}

		public void SetParticleDrawable(Drawable particleDrawable) { 
			BuildParticlePool(particleDrawable);
		}

		public void SetRandomColors(List<Color> colors) {
			AddInitializer(new RandomColorInitializer(colors));
		}

		public void SetSpeedAndAngleRange(float speedMin, float speedMax, int minAngle, int maxAngle) {
			// fix angles
			while (maxAngle < minAngle) {
				maxAngle += 360;
			}
			AddInitializer(new SpeedAndRangeInitializer(DPToPixels(speedMin), DPToPixels(speedMax), minAngle, maxAngle));
		}

		public void SetRotationSpeed(float rotationSpeed) {
			AddInitializer(new RotationSpeedInitializer(rotationSpeed, rotationSpeed));
		}

		public void SetScaleRange(float minScale, float maxScale) {
			AddInitializer(new RandomScaleInitializer(minScale, maxScale));
		}

		public void SetSpeedMultiplierRange(float minSpeed, float maxSpeed) {
			AddInitializer(new RandomSpeedMultiplierInitializer(minSpeed, maxSpeed));
		}

		public void SetFadeOut(int millisecondsBeforeEnd) {
			modifiers.Add(new AlphaModifier(255, 0, timeToLive - millisecondsBeforeEnd, timeToLive));
		}

		public void AddInitializer(IParticleInitializer initializer) {
			initializers.Add(initializer);
		}

		public void AddModifier(IParticleModifier modifier) {
			modifiers.Add(modifier);
		}

		protected override void OnDraw(Canvas canvas) {
			lock (activeParticles) {
				foreach (var p in activeParticles) {
					p.Draw(canvas);
				}
			}
		}

		protected override void Dispose(bool disposing) {
			StopEmitting();

			if (particlePool != null) {
				for (int i = 0; i < particlePool.Count; i++) {
					particlePool[i].Dispose();
					particlePool[i] = null;
				}
				particlePool = null;
			}

			if (activeParticles != null) {
				for (int i = 0; i < activeParticles.Count; i++) {
					activeParticles[i].Dispose();
					activeParticles[i] = null;
				}
				activeParticles = null;
			}

			base.Dispose(disposing);
		}

		void InitializeFromAttributes(IAttributeSet attrs) {
			if (attrs != null) { 
				var a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.ParticleSystemView);

				try {
					// get max particles attribute
					int attrMaxParticles = a.GetInt(Resource.Styleable.ParticleSystemView_maxParticles, 500);
					maxParticles = attrMaxParticles;

					// get max particles attribute
					int attrTimeToLive = a.GetInt(Resource.Styleable.ParticleSystemView_timeToLive, 5000);
					timeToLive = attrTimeToLive;

					// get icon attribute
					Drawable attrParticleDrawable = a.GetDrawable(Resource.Styleable.ParticleSystemView_particleDrawable);
					if (attrParticleDrawable != null) {
						BuildParticlePool(attrParticleDrawable);
					}
				} finally {
					a.Recycle();
				}
			}
		}

		void BuildParticlePool(Drawable particleDrawable) {
			// create the particles and add them to the pool
			if (particleDrawable is BitmapDrawable) {
				Bitmap bitmap = ((BitmapDrawable)particleDrawable).Bitmap;
				for (int i = 0; i < maxParticles; i++) {
					particlePool.Add(new Particle(bitmap));
				}
			} else {
				throw new ArgumentException("The given drawable needs to be a BitmapDrawable!");
			}
		}

		void StartEmitting(int particlesPerSecond, int duration) {
			numOfActivatedParticles = 0;
			particlesPerMillisecond = particlesPerSecond / 1000f;
			emittingDuration = duration;
			StartAnimator(new LinearInterpolator(), duration + timeToLive);
		}

		void StartAnimator(IInterpolator interpolator, int animationDuration) {
			animator = ValueAnimator.OfInt(0, animationDuration);
			animator.SetDuration(animationDuration);
			animator.Update += (sender, e) => {
				int milliseconds = (int)e.Animation.AnimatedValue;
				OnUpdate(milliseconds);
			};
			animator.AnimationEnd += (sender, e) => { CleanupAnimation(); };
			animator.AnimationCancel += (sender, e) => { CleanupAnimation(); };
			animator.SetInterpolator(interpolator);
			animator.Start();
		}

		void OnUpdate(int milliseconds) {
			while (((emittingDuration > 0 && milliseconds < emittingDuration) || emittingDuration == -1) &&
					particlePool.Count > 0 &&
					numOfActivatedParticles < particlesPerMillisecond * milliseconds) {
				
				ActivateParticle(milliseconds);
			}
			lock (activeParticles) {
				for (int i = activeParticles.Count-1; i >= 0; i--) {
					Particle p = activeParticles[i];

					var active = p.Update(milliseconds);
					if (!active) {
						activeParticles.RemoveAt(i);
						particlePool.Add(p);
					}
				}
			}
			PostInvalidate();
		}

		void ActivateParticle(int milliseconds) {
			// get the first particle and remove it from the pool
			Particle p = particlePool[0];
			particlePool.RemoveAt(0);
			p.Init();

			// initialization goes before configuration, scale is required before can be configured properly
			foreach (var init in initializers) {
				init.InitParticle(p, rnd);
			}

			int particleX = rnd.Next(emitterX, emitterXMax);
			int particleY = rnd.Next(emitterY, emitterYMax);
			p.Configure(timeToLive, particleX, particleY);

			p.Activate(milliseconds, modifiers);
			activeParticles.Add(p);
			numOfActivatedParticles++;
		}

		void CleanupAnimation() {
			particlePool.AddRange(activeParticles);
			activeParticles.Clear();
		}

		float DPToPixels(float dps) {
			return (dps * dpToPxScale + 0.5f);
		}
	}
}
