using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth_Afstandsbepaling_Pi
{
    class Distance
    {
        public double CalculateDistance(int rssi)
        {
            //double t = ((-48 - (rssi)) / (10 * 2));
            double y = (10 * 3);
            double x = (-48 - (rssi));

            double halfResult = x / y;
            double result = Math.Pow(10, halfResult);

            return result;
        }
    }
}
