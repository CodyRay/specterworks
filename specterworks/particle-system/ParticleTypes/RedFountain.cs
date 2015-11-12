using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace specterworks.ParticleTypes
{
    class RedFountain :Fountain
    {
        protected override void SetColor(Particle p)
        {
            p.ParticleColor = Color.FromArgb(RS.NextByte(0, 255), RS.NextByte(128, 255), RS.NextByte(0, 128), RS.NextByte(0, 128));
        }
    }
}
