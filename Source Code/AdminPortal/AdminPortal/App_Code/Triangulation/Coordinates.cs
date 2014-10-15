using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Code.Triangulation
{
    class Coordinates
    {
        private double x_coord;
        private double y_coord;

        public double getXCoord()
        {
            return x_coord;
        }

        public double getYCoord()
        {
            return y_coord;
        }

        public void setYCoord(double y)
        {
            y_coord = y;
        }

        public void setXCoord(double x)
        {
            x_coord = x;
        }

    }
}
