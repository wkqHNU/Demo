using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.Space
{
    public class TestSpace
    {
        public Theta thetaModel;
        public Phi phiModel;
        public int thetaStart = 0; // 1800/2800暗室不能取0，如果取0，路损文件也要跟着修改
        public int thetaStep = 15;
        public int thetaStop = 180;
        public int phiStart = 0;
        public int phiStep = 15;
        public int phiStop = 360;
        public bool phiStopFlag = false;
        public bool SetTheta8Phi(int theta, int phi, Direction dir, Polar polar)
        {
            return true;
        }
        public void Convert2TheoreticalSpace(int thetaPhysical, int phiPhysical)
        {
            thetaModel.theta = thetaModel.ThetaConvert(thetaPhysical, phiPhysical);
            phiModel.phi = phiModel.PhiConvert(thetaPhysical, phiPhysical, phiStopFlag);
        }
    }
}
