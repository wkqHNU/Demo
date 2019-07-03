using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Space
{
    class PhiDaMoDa:Phi
    {
        /// <summary>
        /// phi是转台转角，逆时针转
        /// 人正面对暗室正面，左手(人往左转90度向前走)为x轴，暗室中心指向暗室门为y轴，垂直地面向上为z轴
        /// </summary>
        /// <param name="thetaPhysical"></param>
        /// <param name="phiPhysical"></param>
        /// <param name="phiStopFlag"></param>
        /// <returns></returns>
        public override int PhiConvert(int thetaPhysical, int phiPhysical, bool phiStopFlag)
        {
            int phi = 0;
            if ((thetaPhysical / 15) % 2 == 1) // 15的整数倍
            {
                if (phiStopFlag)
                {
                    if ((phiPhysical >= 0) && (phiPhysical <= 90))
                        phi = phiPhysical + 270;
                    else
                        phi = phiPhysical - 90;
                }
                else
                {
                    if ((phiPhysical >= 0) && (phiPhysical < 90))
                        phi = phiPhysical + 270;
                    else
                        phi = phiPhysical - 90;
                }
            }
            else // 30的整数倍
            {
                if (phiStopFlag)
                {
                    if ((phiPhysical >= 0) && (phiPhysical <= 270))
                        phi = phiPhysical + 90;
                    else
                        phi = phiPhysical - 270;
                }
                else
                {
                    if ((phiPhysical >= 0) && (phiPhysical < 270))
                        phi = phiPhysical + 90;
                    else
                        phi = phiPhysical - 270;
                }
            }
            return phi;
        }
    }
}
