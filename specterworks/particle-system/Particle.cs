using System;
using System.ComponentModel;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace specterworks
{
    public class Particle
    {
        private double Age = 0;

        [Category("Color")]
        [Editor(typeof(ColorEditor), typeof(ColorEditor))]
        public Color ParticleColor { get; set; } = Color.FromRgb(255, 255, 255);

        [Category("Lifetime")]
        public bool IsAlive { get; set; } = true;
        
        [Category("Lifetime")]
        public bool IsVisible { get; set; } = true;

        [Category("Lifetime")]
        public double? LifeSpan { get; set; }

        [Category("Motion")]
        [ExpandableObject]
        public SimpleVector Acceleration { get; set; } = new SimpleVector();

        [Category("Motion")]
        [ExpandableObject]
        public SimpleVector Location { get; set; } = new SimpleVector();

        [Category("Motion")]
        [ExpandableObject]
        public SimpleVector Velocity { get; set; } = new SimpleVector();

        [Category("Motion")]
        public bool HasGravity { get; set; } = false;

        [Category("Motion")]
        [ExpandableObject]
        public SimpleVector GravityLocation { get; set; } = new SimpleVector();

        [Category("Motion")]
        public int GravityIntensity { get; set; } = 1000;



        /// <summary>
        /// Move the particle and emit the particles
        /// </summary>
        /// <param name="time">The time that will pass in this frame</param>
        /// <param name="emitter">Callback for storing new particles, particles will be set for this frame (no need to call Forward on new particles)</param>
        public virtual void Forward(float time, Action<Particle> emitter)
        {
            if(HasGravity)
            {
                //Assume our particle has a mass of one and the GravityLocation has a mass of GravityIntensity
                //F_gravity = (m_1 * m_2)/d^2
                Acceleration.GravityEffect(Location, GravityLocation, GravityIntensity);
            }

            Location.PositionChange(Velocity, Acceleration, time);
            Velocity.VelocityChange(Acceleration, time);

            Age += time;
            time -= time;

            if (LifeSpan.HasValue && Age > LifeSpan)
                IsAlive = false;

            if (Location.X > Consts.Bounding)
            {
                Velocity.X = -Math.Abs(Velocity.X);
                Acceleration.X = -Math.Abs(Acceleration.X);
            }
            if (Location.X < -Consts.Bounding)
            {
                Velocity.X = Math.Abs(Velocity.X);
                Acceleration.X = Math.Abs(Acceleration.X);
            }
            if (Location.Y > Consts.Bounding)
            {
                Velocity.Y = -Math.Abs(Velocity.Y);
                Acceleration.Y = -Math.Abs(Acceleration.Y);
            }
            if (Location.Y < -Consts.Bounding)
            {
                Velocity.Y = Math.Abs(Velocity.Y);
                Acceleration.Y = Math.Abs(Acceleration.Y);
            }
            if (Location.Z > Consts.Bounding)
            {
                Velocity.Z = -Math.Abs(Velocity.Z);
                Acceleration.Z = -Math.Abs(Acceleration.Z);
            }
            if (Location.Z < -Consts.Bounding)
            {
                Velocity.Z = Math.Abs(Velocity.Z);
                Acceleration.Z = Math.Abs(Acceleration.Z);
            }
        }
    }
}