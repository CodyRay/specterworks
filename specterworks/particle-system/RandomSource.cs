using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace specterworks
{
    internal static class RS
    {
        private static ConcurrentQueue<byte> randomBytes = new ConcurrentQueue<byte>();

        public static int Next(int floor, int celing)
        {
            return Print((int)((celing - floor) * GetDouble() + floor));
        }

        public static byte Next(byte floor, byte celing)
        {
            return Print((byte)((celing - floor) * ((float)GetByte() / byte.MaxValue) + floor));
        }

        public static float Next(float floor, float celing)
        {
            return Print((celing - floor) * (float)GetDouble() + floor);
        }

        public static double Next(double floor, double celing)
        {
            return Print((celing - floor) * GetDouble() + floor);
        }

        public static float NextAngle()
        {
            return Print(Next(0, 2 * (float)Math.PI));
        }

        private static byte GetByte()
        {
            byte o;
            while (!randomBytes.TryDequeue(out o))
            {
                byte[] bytes = new byte[1024 * 8];
                using (var randoms = new RNGCryptoServiceProvider())
                {
                    randoms.GetBytes(bytes);
                }
                foreach (var b in bytes)
                {
                    randomBytes.Enqueue(b);
                }
            }
            return o;
        }
        private static double GetDouble()
        {
            var i = BitConverter.ToInt32(new[] { GetByte(), GetByte(), GetByte(), GetByte() }, 0);
            return (i - (double)int.MinValue) / ((double)int.MaxValue - (double)int.MinValue);
        }

        private static T Print<T>(T value)
        {
            //Console.WriteLine(value);
            return value;
        }
    }
};