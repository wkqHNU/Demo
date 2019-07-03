using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Space
{
    public class Theta
    {
        public int theta=0;
        public virtual int ThetaConvert(int thetaPhysical, int phiPhysical)
        {
            return thetaPhysical;
        }
    }
}
