using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks.ParticleTypes
{
    class RedFountain :Fountain
    {
        protected override void SetColor(Particle p)
        {
            p.Color = System.Drawing.Color.FromArgb(RS.NextInt(0, 255), RS.NextInt(128, 255), RS.NextInt(0, 128), RS.NextInt(0, 128));
        }
    }
}
