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
        //Used for showing if particle is out of bounds, i.e., it goes too far away from the origin
        public float XBound { get; }
        public float YBound { get; }
        public float ZBound { get; }

        public Particle(float x, float y, float z)
        {
            if (x <= 0 || y <= 0 || z <= 0)
                throw new InvalidOperationException("Particle Bounds Must be positive");
            XBound = x;
            YBound = y;
            ZBound = z;
        }
        public Vector3 StartLocation { get; set; } = new Vector3();
        public Vector3 StartVelocity { get; set; } = new Vector3();
        public Vector3 StartAcceleration { get; set; } = new Vector3();
        public Color StartColor { get; set; } = Color.White;
        public double? TimeLifeSpan { get; set; }
        public Func<Particle, IEnumerable<Particle>> EmitParticle { get; set; }

        public Vector3 CurrentLocation { get; set; } = new Vector3();
        public Vector3 CurrentVelocity { get; set; } = new Vector3();
        public Vector3 CurrentAcceleration { get; set; } = new Vector3();
        public Color CurrentColor { get; set; } = Color.White;
        public double TimeAge { get; set; } = 0;

        public bool IsAlive => !TimeLifeSpan.HasValue || TimeAge < TimeLifeSpan.Value;
        public bool IsWithinBounds => (CurrentLocation.X > -XBound && CurrentLocation.X < XBound) &&
                                      (CurrentLocation.Y > -YBound && CurrentLocation.Y < YBound) &&
                                      (CurrentLocation.Z > -ZBound && CurrentLocation.Z < ZBound);

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
                p.Start();
                emitter(p);
            }

            CurrentLocation = new Vector3(CurrentLocation.X + CurrentVelocity.X * time + CurrentAcceleration.X * time * time,
                                          CurrentLocation.Y + CurrentVelocity.Y * time + CurrentAcceleration.Y * time * time,
                                          CurrentLocation.Z + CurrentVelocity.Z * time + CurrentAcceleration.Z * time * time);

            CurrentVelocity = new Vector3(CurrentVelocity.X + CurrentAcceleration.X * time,
                                          CurrentVelocity.Y + CurrentAcceleration.Y * time,
                                          CurrentVelocity.Z + CurrentAcceleration.Z * time);

            TimeAge += time;
        }

        public void Start()
        {
            CurrentAcceleration = new Vector3(StartAcceleration.X, StartAcceleration.Y, StartAcceleration.Z);
            CurrentVelocity = new Vector3(StartVelocity.X, StartVelocity.Y, StartVelocity.Z);
            CurrentLocation = new Vector3(StartLocation.X, StartLocation.Y, StartLocation.Z);
            CurrentColor = Color.FromArgb(StartColor.A, StartColor.R, StartColor.G, StartColor.B);
        }
    }
}