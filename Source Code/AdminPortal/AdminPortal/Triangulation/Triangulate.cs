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

        public Triangulate()
        {
            roomArray = new int[xMatrixSize, yMatrixSize];
        }

        public void add3Beacons(Reciever b1, Reciever b2, Reciever b3)
        {
            beacon1 = b1;
            beacon2 = b2;
            beacon3 = b3;
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
            float signalStrength1, signalStrength2, signalStrength3;
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



    }
}
