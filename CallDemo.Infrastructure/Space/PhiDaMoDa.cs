using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Space
{
    class PhiDaMoDa:Phi
    {
        public override int PhiConvert(int thetaPhysical, int phiPhysical)
        {
            int phi = 0;
            if ((thetaPhysical / 15) % 2 == 1)
            {
                if ((phiPhysical >= 0) && (phiPhysical < 270))
                    phi = phiPhysical + 90;
                else
                    phi = phiPhysical - 270;
            }
            else
            {
                if ((phiPhysical >= 0) && (phiPhysical < 90))
                    phi = phiPhysical + 270;
                else
                    phi = phiPhysical - 90;
            }
            //if (phi == 360)
            //    phi = 0;
            return phi;
        }
    }
}
