using System;

namespace DroidParticleFx.Initializer {
	public interface IParticleInitializer {
		void InitParticle(Particle p, Random r);
	}
}

