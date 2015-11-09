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

        public SetXYZ Location { get; set; } = new SetXYZ();
        public SetXYZ Velocity { get; set; } = new SetXYZ();
        public SetXYZ Acceleration { get; set; } = new SetXYZ();
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
            var parts = EmitParticle?.Invoke(this);
            if (parts != null)
            {
                foreach (var p in parts)
                {
                    //d = d_0 + v_0*t + a*t^2
                    emitter(p);
                }
            }

            Location.X = Location.X + Velocity.X * time + Acceleration.X * time * time;
            Location.Y = Location.Y + Velocity.Y * time + Acceleration.Y * time * time;
            Location.Z = Location.Z + Velocity.Z * time + Acceleration.Z * time * time;

            Velocity.X = Velocity.X + Acceleration.X * time;
            Velocity.Y = Velocity.Y + Acceleration.Y * time;
            Velocity.Z = Velocity.Z + Acceleration.Z * time;

            var acc_scale = 0.008f;
            Acceleration.X = -Location.X * Location.X * acc_scale * Location.X / (Math.Abs(Location.X) + 1);
            Acceleration.Y = -Location.Y * Location.Y * acc_scale * Location.Y / (Math.Abs(Location.Y) + 1);
            Acceleration.Z = -Location.Z * Location.Z * acc_scale * Location.Z / (Math.Abs(Location.Z) + 1);

            TimeAge += time;
        }
    }
}