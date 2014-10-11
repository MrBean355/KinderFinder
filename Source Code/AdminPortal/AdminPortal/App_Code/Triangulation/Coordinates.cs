using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triangulation_alpha0._2
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
