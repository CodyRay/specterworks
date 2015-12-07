using System;
using System.Linq;
using System.Threading;
using System.Windows.Media;

namespace specterworks.ParticleTypes
{
    /// <summary>
    /// A Binary Particle Cloud releases many 'ion' particles, and every so often, two new bpcs will be created. Or if there become to many bpcs, a branch of the tree may die out.
    /// </summary>
    public class BinaryParticleCloud : EmitingParticle
    {
        public BinaryParticleCloud()
        {
            ParticleColor = Color.FromRgb(RS.Next((byte)128, (byte)255), RS.Next((byte)128, (byte)255), RS.Next((byte)128, (byte)255));
        }
        private static Semaphore AvailableClouds = new Semaphore(512, 512);
        public override double? EmitPeriod { get; set; } = 0.1;
        public int SplitChance { get; set; } = 75;
        public float ChildVelocityVariation { get; set; } = 8;
        public float ChildLocationVariation { get; set; } = 2;

        public int ColorDecrement { get; set; } = 5;
        public double ColorIncrement { get; set; } = (double)5 / (double)255;
        public int SplitPeriod { get; set; } = 20;
        private ColorEvolutionMask ColorMask = RandomColorMask();
        private int SplitCounter = 0;

        protected override void Emit(Action<Particle> emitter)
        {
            if (RS.Next(0, SplitChance) == 0)
            {
                SplitCounter--;
                if (SplitCounter <= 0)
                {
                    SplitCounter = RS.Next((int)(SplitPeriod/2), SplitPeriod);
                    if (AvailableClouds.WaitOne(0))
                    {
                        emitter(SpawnChild());
                        emitter(SpawnChild());
                        IsAlive = false;
                    }
                    else
                    {
                        //No children, not enough room
                        AvailableClouds.Release();
                        emitter(SpawnRip());
                        IsAlive = false;
                    }
                }
                else
                {
                    emitter(SpawnColorChild());
                    IsAlive = false;
                }
            }
            else
            {
                if(EmitIons)
                    emitter(SpawnIon());
            }
        }

        private Particle SpawnRip()
        {
            var p = new Cloud1Emiter();
            //p.GravityLocation = Location;
            //p.HasGravity = true;
            p.ParticleColor = Color.FromRgb(255, 255, 255);
            p.LifeSpan = .5;
            p.MaxParticlesVelocity = 50;
            p.ChildParticleLifeSpan = .5f;

            p.Location.Copy(Location);

            return p;
        }

        private Particle SpawnIon()
        {
            var p = new Particle();
            //p.GravityLocation = Location;
            //p.HasGravity = true;
            p.ParticleColor = ParticleColor;
            p.LifeSpan = 2;

            p.Location.Copy(Location);
            p.Velocity.Copy(Velocity);

            p.Location.Vary(RS.NextAngle(), RS.NextAngle(), RS.Next(0, 5));
            p.Velocity.Vary(RS.NextAngle(), RS.NextAngle(), RS.Next(0, 10));

            return p;
        }

        private Particle SpawnChild()
        {
            var p = SpawnColorChild();
            ChangeMask(p);
            p.Location.Vary(RS.NextAngle(), RS.NextAngle(), RS.Next(0, ChildLocationVariation));
            p.Velocity.Vary(RS.NextAngle(), RS.NextAngle(), RS.Next(0, ChildVelocityVariation));

            return p;
        }

        private BinaryParticleCloud SpawnColorChild()
        {
            var p = new BinaryParticleCloud();
            p.Location.Copy(Location);
            p.Velocity.Copy(Velocity);
            p.Acceleration.Copy(Acceleration);
            p.SplitCounter = SplitCounter;
            p.ColorMask = ColorMask;
            p.ParticleColor = ParticleColor;
            p.SplitChance = SplitChance;
            p.ColorIncrement = ColorIncrement;
            p.ColorDecrement = ColorDecrement;
            p.ChildLocationVariation = ChildLocationVariation;
            p.ChildVelocityVariation = ChildVelocityVariation;
            p.EmitPeriod = EmitPeriod;
            p.SplitPeriod = SplitPeriod;

            //p.ParticleColor = Color.FromRgb((byte)((ParticleColor.R + p.ParticleColor.R) / 2), (byte)((ParticleColor.G + p.ParticleColor.G) / 2), (byte)((ParticleColor.B + p.ParticleColor.B) / 2));

            var R = p.ParticleColor.R;
            var G = p.ParticleColor.G;
            var B = p.ParticleColor.B;

            //Convert RGB to CYMK

            var C = 1 - ((double)R / 255);
            var M = 1 - ((double)G / 255);
            var Y = 1 - ((double)B / 255);
            var K = (double)1;

            if (C < K)
                K = C;
            if (M < K)
                K = M;
            if (Y < K)
                K = Y;
            if (K == 1)
            { //Black
                C = 0;
                M = 0;
                Y = 0;
            }
            else
            {
                C = (C - K) / (1 - K);
                M = (M - K) / (1 - K);
                Y = (Y - K) / (1 - K);
            }

            //if (Y > 0.9)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Yellow);
            //}
            //if (M > 0.9)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Magenta);
            //}
            //if (C > 0.9)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Cyan);
            //}
            //if (K > 0.5)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Black);
            //    K = 0.1;
            //}
            //if (R > 230)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Red);
            //}
            //if (G > 230)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Green);
            //}
            //if (B > 230)
            //{
            //    p.ColorMask &= ~(ColorEvolutionMask.Blue);
            //}
            if (p.ColorMask == 0)
            {
                ChangeMask(p);
            }
            else
            {
                ColorEvolutionMask mask;
                while (!p.ColorMask.HasFlag(mask = RandomColorMask()))
                {

                }

                switch (mask)
                {
                    //RGB we reduce
                    case ColorEvolutionMask.Red:
                        R = (byte)Math.Max(R + (byte)ColorDecrement, R); //Max is for possible overflow
                        p.ParticleColor = Color.FromRgb(R, G, B);
                        break;
                    case ColorEvolutionMask.Green:
                        G = (byte)Math.Max(G + (byte)ColorDecrement, G);
                        p.ParticleColor = Color.FromRgb(R, G, B);
                        break;
                    case ColorEvolutionMask.Blue:
                        B = (byte)Math.Max(B + (byte)ColorDecrement, B);
                        p.ParticleColor = Color.FromRgb(R, G, B);
                        break;

                    //CYMK we increase
                    case ColorEvolutionMask.Cyan:
                        C = Math.Min(1, C + ColorIncrement);
                        p.ParticleColor = CymkToColor(C, Y, M, K);
                        break;
                    case ColorEvolutionMask.Yellow:
                        Y = Math.Min(1, Y + ColorIncrement);
                        p.ParticleColor = CymkToColor(C, Y, M, K);
                        break;
                    case ColorEvolutionMask.Magenta:
                        M = Math.Min(1, M + ColorIncrement);
                        p.ParticleColor = CymkToColor(C, Y, M, K);
                        break;
                }
            }
            return p;
        }

        private static void ChangeMask(BinaryParticleCloud p)
        {
            var c = RandomColorMask();
            if (p.ColorMask.HasFlag(c))
            {
                p.ColorMask &= ~c;
            }
            else
            {
                p.ColorMask |= c;
            }
        }

        static Color CymkToColor(double C, double Y, double M, double K)
        {
            C = (C * (1 - K) + K);
            M = (M * (1 - K) + K);
            Y = (Y * (1 - K) + K);
            var R = (byte)((1 - C) * 255);
            var G = (byte)((1 - M) * 255);
            var B = (byte)((1 - Y) * 255);
            return Color.FromRgb(R, G, B);
        }

        static ColorEvolutionMask[] ColorMaskValues = new[] { ColorEvolutionMask.Cyan, ColorEvolutionMask.Yellow, ColorEvolutionMask.Magenta,
                                                                ColorEvolutionMask.Red, ColorEvolutionMask.Green, ColorEvolutionMask.Blue};
        public static bool EmitIons { get; set; } = true;

        public static ColorEvolutionMask RandomColorMask()
        {
            return ColorMaskValues[RS.Next(0, ColorMaskValues.Length)];
        }
    }

    [Flags]
    public enum ColorEvolutionMask
    {
        //Addative Colors
        Red = 1,
        Green = 2,
        Blue = 4,

        //Subtractive Colors
        Cyan = 8,
        Yellow = 16,
        Magenta = 32,
        //Black = 64,

    }
}
