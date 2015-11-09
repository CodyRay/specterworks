using System;
using System.Drawing;

namespace specterworks
{
    public class Particle
    {
        private double Age = 0;

        public SetXYZ Acceleration { get; set; } = new SetXYZ();
        public Color Color { get; set; } = Color.White;
        public bool IsAlive { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public double? LifeSpan { get; set; }
        public SetXYZ Location { get; set; } = new SetXYZ();
        public SetXYZ Velocity { get; set; } = new SetXYZ();

        /// <summary>
        /// Move the particle and emit the particles
        /// </summary>
        /// <param name="time">The time that will pass in this frame</param>
        /// <param name="emitter">Callback for storing new particles, particles will be set for this frame (no need to call Forward on new particles)</param>
        public virtual void Forward(double time, Action<Particle> emitter)
        {
            UpdateLocation(ref Location.X, Velocity.X, Acceleration.X, time);
            UpdateLocation(ref Location.Y, Velocity.Y, Acceleration.Y, time);
            UpdateLocation(ref Location.Z, Velocity.Z, Acceleration.Z, time);

            UpdateVelocity(ref Velocity.X, Acceleration.X, time);
            UpdateVelocity(ref Velocity.Y, Acceleration.Y, time);
            UpdateVelocity(ref Velocity.Z, Acceleration.Z, time);

            Age += time;
            time -= time;

            if (LifeSpan.HasValue && Age > LifeSpan)
                IsAlive = false;
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