using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpBroadcastCapture
{
    class TemperaturClass
    {
        public int Id { get; set; }
        public double Temperatur { get; set; }
        public string TimeStamp{ get; set; }
        public TemperaturClass(double temp)
        {
            
            Temperatur = temp;
            TimeStamp = $"{System.DateTime.Now}";
        }

        public TemperaturClass()
        {
            
        }

    }
}
