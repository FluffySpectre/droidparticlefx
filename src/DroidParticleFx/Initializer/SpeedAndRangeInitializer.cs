using System;

namespace DroidParticleFx.Initializer {
	public class SpeedAndRangeInitializer : IParticleInitializer {
		float speedMin;
		float speedMax;
		int minAngle;
		int maxAngle;

		public SpeedAndRangeInitializer(float speedMin, float speedMax, int minAngle, int maxAngle) {
			this.speedMin = speedMin;
			this.speedMax = speedMax;
			this.minAngle = minAngle;
			this.maxAngle = maxAngle;

			// make sure the angles are in the [0-360] range
			while (minAngle < 0) {
				minAngle += 360;
			}
			while (maxAngle < 0) {
				maxAngle += 360;
			}

			// also make sure that mMinAngle is the smaller
			if (minAngle > maxAngle) {
				int tmp = minAngle;
				minAngle = maxAngle;
				maxAngle = tmp;
			}
		}

		public void InitParticle(Particle p, Random r) {
			float speed = (float)r.NextDouble() * (speedMax - speedMin) + speedMin;
			int angle;

			if (maxAngle == minAngle) {
				angle = minAngle;
			} else {
				angle = r.Next(minAngle, maxAngle);
			}

			float angleInRads = (float)(angle * Math.PI / 180f);
			p.SpeedX = (float)(speed * Math.Cos(angleInRads));
			p.SpeedY = (float)(speed * Math.Sin(angleInRads));
		}
	}
}

