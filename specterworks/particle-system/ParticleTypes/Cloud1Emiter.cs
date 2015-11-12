using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks.ParticleTypes
{
    public class Cloud1Emiter : EmitingParticle
    {
        public int MaxParticlesPerEmit { get; set; } = 20;
        public int MaxParticlesVelocity { get; set; } = 20;
        public float ChildParticleLifeSpan { get; set; } = 10;
        public bool ChildParticleHasGravity { get; set; } = true;
        public Cloud1Emiter()
        {
            EmitPeriod = 0.12;
        }

        protected override void Emit(Action<Particle> emitter)
        {
            foreach (var p in Enumerable.Range(0, RS.NextInt(0, MaxParticlesPerEmit)).Select(i => new Particle()))
            {
                p.Location.X = Location.X;
                p.Location.Y = Location.Y;
                p.Location.Z = Location.Z;
                p.Velocity.X = RS.Next(-MaxParticlesVelocity, MaxParticlesVelocity);
                p.Velocity.Y = RS.Next(-MaxParticlesVelocity, MaxParticlesVelocity);
                p.Velocity.Z = RS.Next(-MaxParticlesVelocity, MaxParticlesVelocity);
                p.LifeSpan = ChildParticleLifeSpan;
                if (ChildParticleHasGravity)
                    p.HasGravity = true;
                emitter(p);
            }
        }
    }
}
