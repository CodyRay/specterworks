using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks.ParticleTypes
{
    public class StartEmitterTwoFountains : EmitingParticle
    {
        public StartEmitterTwoFountains()
        {
            EmitPeriod = 0.001f;
            IsVisible = false;
        }

        protected override void Emit(Action<Particle> emitter)
        {
            var s1 = new SphereEmiter();
            s1.Location.Y = -100;
            s1.Location.X = 200;
            s1.Velocity.X = -5;
            emitter(s1);

            var s2 = new SphereEmiter();
            s2.Location.Y = -100;
            s2.Location.X = -200;
            s2.Velocity.X = 5;
            emitter(s2);

            var f = new Fountain();
            f.Location.Y = -100;
            f.Location.X = 200;
            f.Velocity.X = -5;
            f.Acceleration.X = 0.03f;
            emitter(f);

            var rf = new RedFountain();
            rf.Location.Y = -100;
            rf.Location.X = -200;
            rf.Velocity.X = 5;
            rf.Acceleration.X = -0.03f;
            emitter(rf);

            IsAlive = false;
        }
    }
}
