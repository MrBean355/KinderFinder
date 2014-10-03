using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Triangulation
{//initialise the beacons here, this program is where the beacons can just adapt to the rest of the program
    class AdapterToReciever
    {
        private Reciever beacon = new Reciever();

        public void addBeaconNumber(int _beaconNumber)
        {
            beacon.addBeaconNumber(_beaconNumber);
        }
        public void addSignalStrength(float signal_streangth)
        {
            beacon.addSignalStrength(signal_streangth);
        }
        public Reciever addBeacon()
        {
            Console.WriteLine("beacon added...:");
            return beacon;
        }
    }
}
