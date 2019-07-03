using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Space
{
    public class Phi
    {
        public int phi=0;
        public virtual int PhiConvert(int thetaPhysical, int phiPhysical, bool phiStopFlag)
        {
            return phiPhysical;
        }
    }
}
