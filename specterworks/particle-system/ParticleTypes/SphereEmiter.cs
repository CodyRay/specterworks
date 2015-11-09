using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks.ParticleTypes
{
    public class SphereEmiter : EmitingParticle
    {
        public int NumberOfParticles { get; set; } = 1000;
        public int NumberOfEmits { get; set; } = 4;
        public float ParticlesVelocity { get; set; } = 10;
        public float PositionJitter { get; set; } = 3f;
        public float? ChildParticleLifeSpan { get; set; }
        public bool ChildColor { get; set; } = true;
        public SphereEmiter()
        {
            EmitPeriod = 0.05;
        }

        protected override void Emit(Action<Particle> emitter)
        {
            foreach (var p in Enumerable.Range(0, NumberOfParticles).Select(i => new Particle()))
            {
                //Spherical Coordinates used for determining the velocity
                var a1 = RS.Next(-Math.PI, Math.PI);
                var a2 = RS.Next(-Math.PI, Math.PI);
                p.Velocity.X = (float)(ParticlesVelocity * Math.Sin(a1) * Math.Cos(a2));
                p.Velocity.Y = (float)(ParticlesVelocity * Math.Sin(a1) * Math.Sin(a2));
                p.Velocity.Z = (float)(ParticlesVelocity * Math.Cos(a1));

                p.Location.X = Location.X + RS.Next(-PositionJitter, PositionJitter);
                p.Location.Y = Location.Y + RS.Next(-PositionJitter, PositionJitter);
                p.Location.Z = Location.Z + RS.Next(-PositionJitter, PositionJitter);

                if (ChildColor)
                    p.Color = System.Drawing.Color.FromArgb(255, (int)(255 * Math.Abs(p.Velocity.X) / ParticlesVelocity), (int)(255 * Math.Abs(p.Velocity.Y) / ParticlesVelocity), (int)(255 * Math.Abs(p.Velocity.Z) / ParticlesVelocity));

                p.Velocity.X += Velocity.X;
                p.Velocity.Y += Velocity.Y;
                p.Velocity.Z += Velocity.Z;

                p.LifeSpan = ChildParticleLifeSpan;
                emitter(p);
            }

            NumberOfEmits -= 1;
            if (NumberOfEmits <= 0)
                IsAlive = false; //This one will die after it emits
        }
    }
}
