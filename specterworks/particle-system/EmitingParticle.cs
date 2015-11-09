using System;
using System.Drawing;

namespace specterworks
{
    public abstract class EmitingParticle : Particle
    {
        private double? EmitTime;
        public double? EmitPeriod { get; set; }

        protected abstract void Emit(Action<Particle> emitter);

        /// <summary>
        /// Move the particle and emit the particles
        /// </summary>
        /// <param name="time">The time that will pass in this frame</param>
        /// <param name="emitter">Callback for storing new particles, particles will be set for this frame (no need to call Forward on new particles)</param>
        public override void Forward(double time, Action<Particle> emitter)
        {
            EmitTime = EmitTime ?? EmitPeriod;

            while (time > 0)
            {
                double mintime = time;
                bool emit = false;
                if (EmitPeriod.HasValue && EmitTime.Value < mintime && IsAlive)
                {
                    mintime = (float)EmitTime.Value;
                    emit = true;
                }

                base.Forward(mintime, emitter);
                time -= mintime;

                if (emit && IsAlive)
                {
                    Emit(p =>
                    {
                        p.Forward(time, emitter);
                        emitter(p);
                    });
                    EmitTime = EmitPeriod.Value;
                }
                else
                {
                    EmitTime -= mintime;
                }
            }
        }

        private static void UpdateLocation(ref float P, float V, float A, double time)
        {
            P = (float)(P + V * time + A * time * time);
        }

        private static void UpdateVelocity(ref float V, float A, double time)
        {
            V = (float)(V + A * time);
        }
    }
}