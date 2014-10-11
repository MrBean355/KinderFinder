using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triangulation_alpha0._2
{
    class Program
    {
        static void Main(string[] args)
        {
            //creating beacons
            Reciever r1 = new Reciever();
            Reciever r2 = new Reciever();
            Reciever r3 = new Reciever();
            
            //creating adapters
            AdapterToReciever adapter1 = new AdapterToReciever();
            AdapterToReciever adapter2 = new AdapterToReciever();
            AdapterToReciever adapter3 = new AdapterToReciever();

            //assiging reciever numbers
            adapter1.addRecieverNumber(1);
            adapter2.addRecieverNumber(2);
            adapter3.addRecieverNumber(3);
            
            //assigning signal strengths
            adapter1.addSignalStrength(2);
            adapter2.addSignalStrength(3);
            adapter3.addSignalStrength(10);

            //assigning adapters
            r1 = adapter1.addReciever();
            r2 = adapter2.addReciever();
            r3 = adapter3.addReciever();

            //creating triagulation object
            Triangulate triangulate = new Triangulate(10,11);

            //adding beacons...
            triangulate.add3Recievers(r1, r2, r3);

            //creating matrix
            triangulate.createMatrix();

            //showing empty matrix            
            triangulate.printArray();

            //triagulating
            triangulate.triangulateCoord();

            //printing out new matrix
            Console.WriteLine("new matrix");
            triangulate.printArray();

            //creating coordinates point for the rest of the program
            Coordinates coordinates = new Coordinates();
            coordinates = triangulate.getCoordinatesForAdapter();

            //printing out the to the console the coordinates of the beacon
            Console.WriteLine("The coordinates of the becon are: x = " + coordinates.getXCoord() + " y = " + coordinates.getYCoord());

           Console.ReadLine();
        }
    }
}
