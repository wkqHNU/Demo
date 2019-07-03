using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CallDemo.Infrastructure;
using CallDemo.Infrastructure.Template;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread threadOneProcess = new Thread(new ThreadStart(OneProcess));
            threadOneProcess.Start();
        }

        private static void OneProcess()
        {
            DateTime dt1;
            DateTime dt2;
            DateTime dt3;
            DateTime dtEnd;
            double initTime;
            double totalTime;
            double allTotalTime;
            dt1 = DateTime.Now;
            TestProcess ota = new TestProcess();
            // 系统初始化
            GlobalParameter globalPara = new GlobalParameter();
            // 路损文件
            globalPara.PathlossFilePath = @"..\..\..\doc\all_Dipole_2800(congcong).csv.";
            // Theta，phi的具体实现方式
            globalPara.ThetaModel = "Theta1800";
            globalPara.PhiModel = "PhiDaMoDa";
            ota.SystemInit(globalPara);
            //注册消息反馈事件
            ota.Testing += Testing;
            ota.TestEnd += TestEnd;
            for (int i = 0; i < 1; i++)
            {
                if (i == 0)
                {
                    PassiveParameter para = new PassiveParameter();
                    para.Instrument = Instrument.Emulator;
                    para.ThetaStart = 15;
                    para.ThetaStep = 15;
                    para.ThetaStop = 180;
                    para.PhiStart = 15;
                    para.PhiStep = 15;
                    para.PhiStop = 360;
                    ota.Init(para);
                }
                else
                {
                    PassiveParameter para = new PassiveParameter();
                    para.Instrument = Instrument.Agilent_5071C;
                    para.enableEmulator = true;
                    para.ThetaStart = 0;
                    para.ThetaStep = 15;
                    para.ThetaStop = 180;
                    para.PhiStart = 0;
                    para.PhiStep = 15;
                    para.PhiStop = 360;
                    ota.Init(para);
                }
                dt2 = DateTime.Now;
                initTime = dt2.Subtract(dt1).TotalSeconds;
                Console.WriteLine("initTime: {0:#.0}s", initTime);
                ota.StartTest();
                dt3 = DateTime.Now;
                totalTime = dt3.Subtract(dt1).TotalSeconds;
                Console.WriteLine("totalTime:{0:#.0}s", totalTime);
            }
            dtEnd = DateTime.Now;
            allTotalTime = dtEnd.Subtract(dt1).TotalSeconds;
            Console.WriteLine("allTotalTime:{0:#.0}s", allTotalTime);
            Console.ReadLine();
        }
        //底层触发事件后上层可以在注册的事件中得到
        private static void Testing(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain)
        {
            //if ((thetaPhysical / 15) % 2 == 1)
            //Console.WriteLine("freq={0}\ttheta={1} \tphi={2}\t\tgain{3}= {4:#.0}", freq, thetaPhysical, phiPhysical, polar, gain);
        }
        private static void TestEnd(TestResult result)
        {
            //Console.WriteLine("Efficiency(Theta):{0:#}%\tEfficiency(Theta):{1:#}%\tEfficiency(Theta):{2:#}%", 
            //    result.aveTotal[0]*100, result.aveTotal[1] * 100, result.aveTotal[2] * 100);
        }
    }
}
