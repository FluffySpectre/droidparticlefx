using System;
using System.Collections.Generic;
using Android.Graphics;

namespace DroidParticleFx.Initializer {
	public class RandomColorInitializer : IParticleInitializer {
		List<Color> colors;

		public RandomColorInitializer(List<Color> colors) {
			this.colors = colors;
		}

		public void InitParticle(Particle p, Random r) {
			int index = r.Next(colors.Count);
			Color color = colors[index];
			PorterDuffColorFilter filter = new PorterDuffColorFilter(color, PorterDuff.Mode.SrcAtop);
			p.Paint.SetColorFilter(filter);
		}
	}
}

