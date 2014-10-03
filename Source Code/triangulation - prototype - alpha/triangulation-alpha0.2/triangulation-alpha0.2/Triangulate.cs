using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triangulation_alpha0._2
{
    class Triangulate
    {
        private int xMatrixSize = 10, yMatrixSize = 10;
        private Reciever beacon1, beacon2, beacon3;
        private int[,] roomArray;

        //new code
        //struct Coordinates
        //{
        //    public double x, y;
        //}

        private const int maxNumberOfRecievers = 3;//here you can add the max number of recievers to test
        private const int max_nodes = 3;
        private List<Coordinates> coordinates;
        private Coordinates beacon;
        private int index;


        public Triangulate()
        {
            roomArray = new int[xMatrixSize, yMatrixSize];

            coordinates = new List<Coordinates>();
            beacon = new Coordinates();
            index = 0;

            for (int b = 0; b < max_nodes; b++)
            {
                coordinates.Add(new Coordinates());
            }
        }

        public void add3Recievers(Reciever b1, Reciever b2, Reciever b3)
        {
            beacon1 = b1;
            beacon2 = b2;
            beacon3 = b3;

            // to fix later
            coordinates[0].setXCoord(0);
            coordinates[0].setYCoord(0);
            coordinates[1].setXCoord(0);
            coordinates[1].setYCoord(10);
            coordinates[2].setXCoord(10);
            coordinates[2].setYCoord(0);

        }

        public void moveBeacon(double x, double y)
        {
            beacon.setXCoord(x);
            beacon.setYCoord(y);
        }

        public double[] run(double str1, double str2, double str3)
        {
            var px = ((str1 * str1) - (str2 * str2) + (coordinates[1].getXCoord() * coordinates[1].getXCoord())) / (2.0 * coordinates[1].getXCoord());
            var py = ((str1 * str1) - (str3 * str3) + (coordinates[2].getXCoord() * coordinates[2].getXCoord()) + (coordinates[2].getYCoord() * coordinates[2].getYCoord())) / (2.0 * coordinates[2].getYCoord()) - (coordinates[2].getXCoord() / coordinates[2].getYCoord()) * px;
            return new double[] { px, py };
        }

        public double[] locate()
        {
            double str1 = GetSignalStrength(coordinates[0]);
			double str2 = GetSignalStrength(coordinates[1]);
			double str3 = GetSignalStrength(coordinates[2]);
            return run(str1, str2, str3);
        }

        private double GetSignalStrength(Coordinates node)
        {
            double xd = node.getXCoord() - beacon.getXCoord();
            double yd = node.getYCoord() - beacon.getYCoord();

            var signalStrength = Math.Sqrt((xd * xd) + (yd * yd));

            return -signalStrength;
        }


        public void createMatrix()
        {
            for (int x = 0; x < xMatrixSize; x++)
            {
                for (int y = 0; y < yMatrixSize; y++)
                    roomArray[x,y] = 0;
            }
        }

        public void printArray()
        {
            Console.WriteLine("The contents of the array are: ");
            for (int x = 0; x < xMatrixSize; x++)
            {
                for (int y = 0; y < yMatrixSize; y++) 
                { 
                    Console.Write(roomArray[x, y] + " ");
                }Console.WriteLine();
            } 
        }

        public void triangulateCoord()
        {
            double signalStrength1, signalStrength2, signalStrength3;
            signalStrength1 = beacon1.getSignalStrength();
            signalStrength2 = beacon2.getSignalStrength();
            signalStrength3 = beacon3.getSignalStrength();

            //adjusting matrix for signal 1

            for (int x = 0; x < signalStrength1; x++)
            {
                for (int y = 0; y < signalStrength1; y++)
                {
                    roomArray[x, y] = 1;
                } 
            }
            //adjusting matrix for signal 2
            for (int x = xMatrixSize -1; x > xMatrixSize - 1 - signalStrength2; x--)
            {
                for (int y = 0; y < signalStrength2; y++)
                {
                    if (roomArray[x, y] == 1)
                        roomArray[x, y] = 5;
                    else
                        roomArray[x, y] = 2;
                }
            } 
            //adjusting matrix for signal 3
            for (int x = 0; x < signalStrength3; x++)
            {
                for (int y = yMatrixSize - 1; y > yMatrixSize - 1 - signalStrength3; y--)
                {
                    if (roomArray[x, y] == 1)
                        roomArray[x, y] = 5;
                    else
                        if (roomArray[x, y] == 2)
                            roomArray[x, y] = 5;
                        else
                            if (roomArray[x, y] == 5)
                                roomArray[x, y] = 6;
                            else
                                roomArray[x, y] = 3;
                }
            } 


        }

        public Coordinates getCoordinatesForAdapter()
        {
            Coordinates co = new Coordinates();
            int max = roomArray[0, 0];
            float x_coord = 0, y_coord = 0;
            for (int x = 0; x < xMatrixSize; x++)
            {
                for (int y = 0; y < yMatrixSize; y++)
                {
                    //max number
                    if (max < roomArray[x, y])
                    {
                        max = roomArray[x, y];
                        x_coord = x;
                        y_coord = y;
                    }
                }
            }

            co.setYCoord(y_coord);
            co.setXCoord(x_coord);

            return co;

        }

        //add code here



    }
}
