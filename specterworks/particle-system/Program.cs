using specterworks.ParticleTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks
{
    class Program
    {
        static void Main(string[] args)
        {
            var first = new StartEmitterTwoFountains();
            using (var game = new ParticleWindow(first))
            {
                game.Run(30);
            }
        }
    }
}
