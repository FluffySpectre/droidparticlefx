using System;

namespace DroidParticleFx.Initializer {
	public class RotationSpeedInitializer : IParticleInitializer {
		float minRotationSpeed;
		float maxRotationSpeed;

		public RotationSpeedInitializer(float minRotationSpeed, float maxRotationSpeed) {
			this.minRotationSpeed = minRotationSpeed;
			this.maxRotationSpeed = maxRotationSpeed;
		}

		public void InitParticle(Particle p, Random r) {
			float rotationSpeed = (float)r.NextDouble() * (maxRotationSpeed - minRotationSpeed) + minRotationSpeed;
			p.RotationSpeed = rotationSpeed;
		}
	}
}

