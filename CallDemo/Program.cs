using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallDemo.Infrastructure;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //实例化一个底层类
            ProjectOTA ota = new ProjectOTA(1);
            //注册消息反馈事件
            ota.GetTestPoint += GetTestPoint;
            //初始化
            ota.Init();
            ota.StartTest();
            Console.ReadLine();
        }

        //底层触发事件后上层可以在注册的事件中得到
        private static void GetTestPoint(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain)
        {
            //if ((thetaPhysical / 15) % 2 == 1)
            Console.WriteLine("freq={0}\ttheta={1}\tphi={2}\t\tgain{3}= {4:#.0}", freq, thetaPhysical, phiPhysical, polar, gain);
        }
    }
}
