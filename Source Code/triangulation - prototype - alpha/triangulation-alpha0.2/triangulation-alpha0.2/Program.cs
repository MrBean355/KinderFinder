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
            Reciever b1 = new Reciever();
            Reciever b2 = new Reciever();
            Reciever b3 = new Reciever();
            
            //creating adapters
            AdapterToReciever adapter1 = new AdapterToReciever();
            AdapterToReciever adapter2 = new AdapterToReciever();
            AdapterToReciever adapter3 = new AdapterToReciever();

            //assiging signal strengths
            adapter1.addBeaconNumber(1);
            adapter2.addBeaconNumber(2);
            adapter3.addBeaconNumber(3);
            
            //assigning signal strengths
            adapter1.addSignalStrength(2);
            adapter2.addSignalStrength(3);
            adapter3.addSignalStrength(8);

            //assigning adapters
            b1 = adapter1.addBeacon();
            b2 = adapter2.addBeacon();
            b3 = adapter3.addBeacon();

            //creating triagulation object
            Triangulate triangulate = new Triangulate();

            //adding beacons...
            triangulate.add3Beacons(b1, b2, b3);

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
            Console.WriteLine("The coordinates of the becon are: x - " + coordinates.getXCoord() + " y - " + coordinates.getYCoord());

            
            Console.ReadLine();
        }
    }
}
