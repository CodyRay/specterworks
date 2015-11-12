using System;

namespace specterworks
{
    internal static class RS
    {
        private static Random random;

        static RS()
        {
            random = new Random();
        }

        public static void Seed(int seed)
        {
            random = new Random(seed);
        }

        public static float Next(double floor, double celing)
        {
            return (float)(((celing - floor) * random.NextDouble()) + floor);
        }

        public static int NextInt(int floor, int celing)
        {
            return random.Next(floor, celing);
        }
        
        public static byte NextByte(byte floor, byte celing)
        {
            return (byte)random.Next(floor, celing);
        }
    }
}