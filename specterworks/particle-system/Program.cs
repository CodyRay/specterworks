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
            var first = new Particle();
            first.ColorMode = PointColorMode.Red;
            using (var game = new ParticleWindow(first))
            {
                first.EmitParticle = game.CoolEmitter;

                game.Run(30);
            }
        }
    }
}
