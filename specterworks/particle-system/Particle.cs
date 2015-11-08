using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace specterworks
{
    public class Particle
    {
        public bool CanHasTwoBebes { get; set; } = false;

        public double? TimeLifeSpan { get; set; }
        public Func<Particle, IEnumerable<Particle>> EmitParticle { get; set; }

        public Vector3 Location { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector3 Acceleration { get; set; } = new Vector3();
        public Color Color { get; set; } = Color.White;
        public double TimeAge { get; set; } = 0;

        public bool IsDead { get; set; } = false;

        public bool IsAlive => (!TimeLifeSpan.HasValue || TimeAge < TimeLifeSpan.Value) && !IsDead;

        internal PointColorMode ColorMode { get; set; }

        /// <summary>
        /// Move the particle and emit the particles
        /// </summary>
        /// <param name="time">The time that will pass in this frame</param>
        /// <param name="emitter">Callback for storing new particles, particles will be set for this frame (no need to call Forward on new particles)</param>
        public void Forward(float time, Action<Particle> emitter)
        {
            var parts = EmitParticle?.Invoke(this) ?? Enumerable.Empty<Particle>();
            foreach (var p in parts)
            {
                //d = d_0 + v_0*t + a*t^2
                emitter(p);
            }

            Location = new Vector3(Location.X + Velocity.X * time + Acceleration.X * time * time,
                                          Location.Y + Velocity.Y * time + Acceleration.Y * time * time,
                                          Location.Z + Velocity.Z * time + Acceleration.Z * time * time);

            Velocity = new Vector3(Velocity.X + Acceleration.X * time,
                                          Velocity.Y + Acceleration.Y * time,
                                          Velocity.Z + Acceleration.Z * time);

            TimeAge += time;
        }
    }
}