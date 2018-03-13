using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace fnc.fnc
{
    class Utils
    {
        public static Vector3 Spherical(float i_angleX, float i_angleY)
        {
            float cx = (float)Math.Cos(i_angleX);
            float sx = (float)Math.Sin(i_angleX);
            float cy = (float)Math.Cos(i_angleY);
            float sy = (float)Math.Sin(i_angleY);
            return new Vector3(sx, sy, cy*cx);
        }

        public static float TriangleWave(float i_x, float i_period)
        {
            float a = (float)Math.IEEERemainder(i_x, i_period) - i_period / 2;
            float b = (float)Math.Abs(a) - i_period / 4;
            return b / (i_period / 2);
        }
    }
}
