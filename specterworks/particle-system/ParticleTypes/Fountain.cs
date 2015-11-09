using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks.ParticleTypes
{
    class Fountain : EmitingParticle
    {
        public float ChildParticlLifeSpan { get; set; } = 15;
        public Fountain()
        {
            EmitPeriod = 0.01f;
        }

        protected override void Emit(Action<Particle> emitter)
        {
            var p = new Particle();
            p.Acceleration.Y = -5; //'Gravity'

            var distance = RS.Next(-5, 5);
            var angle = RS.Next(0, 2 * Math.PI);

            p.Location.X = Location.X;
            p.Location.Y = Location.Y;
            p.Location.Z = Location.Z;

            p.Velocity.X = (float)Math.Sin(angle) * distance;
            p.Velocity.Y = RS.Next(40, 65);
            p.Velocity.Z = (float)Math.Cos(angle) * distance;
            SetColor(p);

            p.LifeSpan = ChildParticlLifeSpan;
            emitter(p);
        }

        protected virtual void SetColor(Particle p)
        {
            p.Color = System.Drawing.Color.FromArgb(RS.NextInt(0, 255), RS.NextInt(0, 128), RS.NextInt(0, 128), RS.NextInt(128, 255));
        }
    }
}
