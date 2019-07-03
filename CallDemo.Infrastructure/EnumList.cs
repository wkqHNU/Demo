using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure
{
    public class EnumList
    {
        public enum TestSystem
        {
            LTE_SISO,
            LTE_MIMO,
            WIFI,
            Passive,
            NULL,
            LTE_MIMO_4x4,
        }
        public enum Instrument
        {
            Emulator,
            Agilent_UXM,
            Agilent_5071C,
        }
        public enum Polar
        {
            Theta = 0,  
            Phi = 1,  
            Both = 2,  
        }
        public enum SystemTestMode
        {
            TRP = 0,
            TIS = 1,
            EIRP = 2,
            EIS = 3,
            Passive = 4,
        }
        public enum Passive
        {
            S11 = 0,
            S21 = 1,
        }
        public enum Direction
        {
            Uplink = 0,
            Downlink = 1,
        }
    }
}
