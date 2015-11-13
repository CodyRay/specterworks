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
            p.ParticleColor = Color.FromArgb((byte)RS.Next(0, 255), (byte)RS.Next(128, 255), (byte)RS.Next(0, 128), (byte)RS.Next(0, 128));
        }
    }
}
