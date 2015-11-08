using System;

namespace specterworks
{
    [Flags]
    public enum PointColorMode
    {
        Red = 0x01,
        Green = 0x02,
        Blue = 0x04,
        RedGreen = Red | Green,
        RedBlue = Red | Blue,
        GreenBlue = Green | Blue,
        RedGreenBlue = Red | Green | Blue,
    }
}