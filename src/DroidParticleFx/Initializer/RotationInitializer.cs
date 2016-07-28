using System;

namespace DroidParticleFx.Initializer {
	public class RotationInitializer : IParticleInitializer {
		int minAngle;
		int maxAngle;

		public RotationInitializer(int minAngle, int maxAngle) {
			this.minAngle = minAngle;
			this.maxAngle = maxAngle;
		}

		public void InitParticle(Particle p, Random r) {
			int value = minAngle == maxAngle ? minAngle : r.Next(minAngle, maxAngle);
			p.InitialRotation = value;
		}
	}
}

