using CallDemo.Infrastructure.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.TestPlan
{
    class PassiveAgilent5071C: PassiveEmulator
    {
        public override double GetAntennaPattern(double theta, double phi, Polar polar, double dataOffset)
        {
            double data = 0;
            if (enableEmulator)
            {
                data = base.GetAntennaPattern(theta, phi, polar, dataOffset);
                return data;
            }
            else
            {
                return data;
            }
        }
        public override bool IsConnected()
        {
            bool flag = false;
            if (enableEmulator)
                return true;
            return flag;
        }
    }
}
