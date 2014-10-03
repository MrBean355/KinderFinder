using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Triangulation
{
    public class Coordinates
    {
        private float x_coord;
        private float y_coord;

        public float getXCoord()
        {
            return x_coord;
        }

        public float getYCoord()
        {
            return y_coord;
        }

        public void setYCoord(float y)
        {
            y_coord = y;
        }

        public void setXCoord(float x)
        {
            x_coord = x;
        }

    }
}
