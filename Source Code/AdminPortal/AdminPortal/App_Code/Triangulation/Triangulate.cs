using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Code.Triangulation
{
    class Triangulate
    {
        private int xMatrixSize = 10, yMatrixSize = 10;
        private Reciever reciever1, reciever2, reciever3;
        private int[,] roomArray;
        bool outOfBounds;

        private const int maxNumberOfRecievers = 3;//here you can add the max number of recievers to test


        public Triangulate()//constructor that takes no parameters for testing
        {
            roomArray = new int[xMatrixSize, yMatrixSize];
            outOfBounds = false;
        }

        public Triangulate(int x_WidthOfRoom, int y_HeightOfRoom)//when calling the constructor add the dimenstions of the room, x first, then y
        {
            xMatrixSize = x_WidthOfRoom;
            yMatrixSize = y_HeightOfRoom;
            roomArray = new int[xMatrixSize, yMatrixSize];
            outOfBounds = false;

        }

        public void add3Recievers(Reciever b1, Reciever b2, Reciever b3)
        {
            reciever1 = b1;
            reciever2 = b2;
            reciever3 = b3;           
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
            signalStrength1 = reciever1.getSignalStrength();
            signalStrength2 = reciever2.getSignalStrength();
            signalStrength3 = reciever3.getSignalStrength();

            if ((signalStrength1 > xMatrixSize))
            {
                signalStrength1 = xMatrixSize;
                outOfBounds = true;
            }
            if ((signalStrength1 > yMatrixSize))
            {
                signalStrength1 = yMatrixSize;
                outOfBounds = true;
            }
            if ((signalStrength2 > xMatrixSize))
            {
                signalStrength1 = xMatrixSize;
                outOfBounds = true;
            }
            if ((signalStrength2 > yMatrixSize))
            {
                signalStrength1 = yMatrixSize;
                outOfBounds = true;
            }
            if ((signalStrength3 > xMatrixSize))
            {
                signalStrength1 = xMatrixSize;
                outOfBounds = true;
            }
            if ((signalStrength3 > yMatrixSize))
            {
                signalStrength1 = yMatrixSize;
                outOfBounds = true;
            }

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
                        roomArray[x, y] = 4;
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
            double x_coord = 0, y_coord = 0;
            double xAverage = 0,yAverage = 0;

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

            int counter=0;
            for (int x = 0; x < xMatrixSize; x++)
            {
                for (int y = 0; y < yMatrixSize; y++)
                {
                    if (max == roomArray[x, y])
                    {
                        xAverage += x + 1;
                        yAverage += y + 1;
                        counter++;
                    }
                }
            }
            xAverage = xAverage / counter;
            yAverage = yAverage / counter;


            //if (x_coord == 0 || y_coord == 0)
            //{
            //    x_coord = -1;
            //    y_coord = -1;
            //}

            co.setYCoord(y_coord);
            co.setXCoord(x_coord);

            //sets them to the averages for more accuracy
            co.setYCoord(xAverage);
            co.setXCoord(yAverage);


            return co;

        }


    }
}
