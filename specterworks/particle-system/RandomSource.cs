using System;

namespace specterworks
{
    internal class RandomSource
    {
        private Random random;

        public RandomSource()
        {
            random = new Random();
        }

        public void Seed(int seed)
        {
            random = new Random(seed);
        }

        public float Next(double floor, double celing)
        {
            return (float)(((celing - floor) * random.NextDouble()) + floor);
        }

        internal int NextInt(int floor, int celing)
        {
            return random.Next(floor, celing);
        }
    }
}