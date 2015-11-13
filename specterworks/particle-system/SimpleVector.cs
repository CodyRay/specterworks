using System;

namespace specterworks
{
    public class SimpleVector
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Z { get; set; } = 0;

        public void PositionChange(SimpleVector velocity, SimpleVector acceleration, float time)
        {
            X = X + velocity.X * time + acceleration.X * time * time;
            Y = Y + velocity.Y * time + acceleration.Y * time * time;
            Z = Z + velocity.Z * time + acceleration.Z * time * time;
        }

        public void VelocityChange(SimpleVector acceleration, float time)
        {
            X = X + acceleration.X * time;
            Y = Y + acceleration.Y * time;
            Z = Z + acceleration.Z * time;
        }

        public static SimpleVector operator -(SimpleVector left, SimpleVector right)
        {
            SimpleVector vec = new SimpleVector();
            vec.X = left.X - right.X;
            vec.Y = left.Y - right.Y;
            vec.Z = left.Z - right.Z;
            return vec;
        }

        public static SimpleVector operator *(float left, SimpleVector right)
        {
            SimpleVector vec = new SimpleVector();
            vec.X = left * right.X;
            vec.Y = left * right.Y;
            vec.Z = left * right.Z;
            return vec;
        }

        public static SimpleVector operator -(SimpleVector right)
        {
            SimpleVector vec = new SimpleVector();
            vec.X = -right.X;
            vec.Y = -right.Y;
            vec.Z = -right.Z;
            return vec;
        }

        internal void Copy(SimpleVector other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        internal void Vary(float anglea, float angleb, float distance)
        {
            X = X + distance * (float)Math.Sin(anglea) * (float)Math.Cos(angleb);
            Y = Y + distance * (float)Math.Sin(anglea) * (float)Math.Sin(angleb);
            Z = Z + distance * (float)Math.Cos(anglea);
        }

        public static SimpleVector operator +(SimpleVector left, SimpleVector right)
        {
            SimpleVector vec = new SimpleVector();
            vec.X = left.X + right.X;
            vec.Y = left.Y + right.Y;
            vec.Z = left.Z + right.Z;
            return vec;
        }

        public SimpleVector Cross(SimpleVector that)
        {
            SimpleVector result = new SimpleVector();
            result.X = (Y * that.Z) - (Z * that.Y);
            result.Y = (Z * that.X) - (X * that.Z);
            result.Z = (X * that.Y) - (Y * that.X);
            return result;
        }

        public float Dot(SimpleVector that)
        {
            return X * that.X + Y * that.Y + Z * that.X;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }


        //internal void GravityEffect(SimpleVector location, SimpleVector gravityLocation, int gravityIntensity)
        //{
        //    //Assume our particle has a mass of one and the GravityLocation has a mass of GravityIntensity
        //    //F_gravity = (m_1 * m_2)/d^2
        //    X = (gravityIntensity / ((gravityLocation.X - location.X) * (gravityLocation.X - location.X))) * Math.Sign(gravityLocation.X - location.X);
        //    if (float.IsInfinity(X) || float.IsNaN(X) || Math.Abs(gravityLocation.X - location.X) < 1) //If it is close, we don't want the accelleration to be infinite!
        //        X = 0;
        //    Y = (gravityIntensity / ((gravityLocation.Y - location.Y) * (gravityLocation.Y - location.Y))) * Math.Sign(gravityLocation.Y - location.Y);
        //    if (float.IsInfinity(Y) || float.IsNaN(Y) || Math.Abs(gravityLocation.Y - location.Y) < 1)
        //        Y = 0;
        //    Z = (gravityIntensity / ((gravityLocation.Z - location.Z) * (gravityLocation.Z - location.Z))) * Math.Sign(gravityLocation.Z - location.Z);
        //    if (float.IsInfinity(Z) || float.IsNaN(Z) || Math.Abs(gravityLocation.Z - location.Z) < 1)
        //        Z = 0;
        //}
        internal void GravityEffect(SimpleVector location, SimpleVector gravityLocation, int gravityIntensity)
        {
            //Assume our particle has a mass of one and the GravityLocation has a mass of GravityIntensity
            //F_gravity = (m_1 * m_2)/d^2
            SimpleVector dvec = location - gravityLocation;
            var d = dvec.Length();
            if (float.IsInfinity(Z) || float.IsNaN(Z) || Math.Abs(d) < 1)
            {
                X = 0;
                Y = 0;
                Z = 0;
                return; //Don't want the acceleration to go to infinity!
            }
            var g = gravityIntensity / (d * d);
            var scale = -g / d; //Scale factor for our vector
            SimpleVector newa = scale * dvec;
            X = newa.X;
            Y = newa.Y;
            Z = newa.Z;
        }
    }
}