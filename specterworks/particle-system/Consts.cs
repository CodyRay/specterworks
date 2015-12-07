using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace specterworks
{
    public static class Consts
    {
        public const int DefaultWindowSize = 600;
        public static float Bounding = 300; //If particles go beyond this point in any direction, their velocities and accelerations will be reversed

        //Axes
        public const int AxesWidth = 3;
        public const float AxesLengthFraction = 0.1f;
        public const float AxesStartFraction = 1.1f;
        public const float AxesLength = 1000;

    }
}
