using System;

namespace DroidParticleFx.Initializer {
	public class RandomScaleInitializer : IParticleInitializer {
		float maxScale;
		float minScale;

		public RandomScaleInitializer(float minScale, float maxScale) {
			this.minScale = minScale;
			this.maxScale = maxScale;
		}

		public void InitParticle(Particle p, Random r) {
			float scale = (float)r.NextDouble() * (maxScale - minScale) + minScale;
			p.Scale = scale;
		}
	}
}

