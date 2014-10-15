using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPortal.Code.Triangulation
{//initialise the beacons here, this program is where the beacons can just adapt to the rest of the program
    class AdapterToReciever
    {
        private Reciever beacon = new Reciever();

        public void addRecieverNumber(int _beaconNumber)
        {
            beacon.addBeaconNumber(_beaconNumber);
        }
        public void addSignalStrength(double signal_streangth)
        {
            beacon.addSignalStrength(signal_streangth);
        }
        public Reciever addReciever()
        {
            //Console.WriteLine("beacon added...:");
            return beacon;
        }
    }
}
