using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//this is the class where you can add the attributes of the beacon and basically use the attributes to the rest of the project
namespace AdminPortal.Triangulation
{
    class Reciever
    {
        private int beacon_number;
        private float signal_strength;

        public void addBeaconNumber(int _beaconNumber)
        {
            beacon_number = _beaconNumber;
        }

        public void addSignalStrength(float _signalStrength)
        {
            signal_strength = _signalStrength;
        }

        public int getBeaconNumber()
        {
            return beacon_number;
        }

        public float getSignalStrength()
        {
            return signal_strength;
        }
    }
}
