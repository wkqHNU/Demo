using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.Space
{
    class TestSpace
    {
        public Theta thetaModel;
        public Phi phiModel;
        public bool SetTheta8Phi(int theta, int phi, Direction dir, Polar polar)
        {
            return true;
        }
        public void Convert2TheoreticalSpace(int thetaPhysical, int phiPhysical)
        {
            thetaModel.theta = thetaModel.ThetaConvert(thetaPhysical, phiPhysical);
            phiModel.phi = phiModel.PhiConvert(thetaPhysical, phiPhysical);
        }
    }
}
