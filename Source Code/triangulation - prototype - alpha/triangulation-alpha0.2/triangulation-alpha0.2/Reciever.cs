using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//this is the class where you can add the attributes of the beacon and basically use the attributes to the rest of the project
namespace triangulation_alpha0._2
{
    class Reciever
    {
        private int beacon_number;
        private double signal_strength;

        public void addBeaconNumber(int _beaconNumber)
        {
            beacon_number = _beaconNumber;
        }

        public void addSignalStrength(double _signalStrength)
        {
            signal_strength = _signalStrength;
        }

        public int getBeaconNumber()
        {
            return beacon_number;
        }

        public double getSignalStrength()
        {
            return signal_strength;
        }
    }
}
