using System;
using Android.Animation;
using Android.Views.Animations;

namespace DroidParticleFx.Modifier {
	public class AlphaModifier : IParticleModifier {
		int initialValue;
		int finalValue;
		int startTime;
		int endTime;
		float duration;
		float valueIncrement;
		ITimeInterpolator interpolator;

		public AlphaModifier(int initialValue, int finalValue, int startMillis, int endMillis) {
			this.initialValue = initialValue;
			this.finalValue = finalValue;
			startTime = startMillis;
			endTime = endMillis;
			duration = endTime - startTime;
			valueIncrement = finalValue - initialValue;
			interpolator = new LinearInterpolator();
		}

		public void Apply(Particle p, int milliseconds) {
			if (milliseconds < startTime) {
				p.Alpha = initialValue;
			} else if (milliseconds > endTime) {
				p.Alpha = finalValue;
			} else {
				float interpolaterdValue = interpolator.GetInterpolation((milliseconds - startTime) * 1f / duration);
				int newAlphaValue = (int)(initialValue + valueIncrement * interpolaterdValue);
				p.Alpha = newAlphaValue;
			}
		}
	}
}

