using System;

namespace DroidParticleFx.Initializer {
	public class RandomSpeedMultiplierInitializer : IParticleInitializer {
		float minSpeed;
		float maxSpeed;

		public RandomSpeedMultiplierInitializer(float minSpeed, float maxSpeed) {
			this.minSpeed = minSpeed;
			this.maxSpeed = maxSpeed;
		}

		public void InitParticle(Particle p, Random r) {
			float speedMultiplier = (float)r.NextDouble() * (maxSpeed - minSpeed) + minSpeed;
			p.SpeedX *= speedMultiplier;
			p.SpeedY *= speedMultiplier;
		}
	}
}

