using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallDemo.Infrastructure;

namespace CallDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //实例化一个底层类
            Operation opt = new Operation(1);
            //注册消息反馈事件
            opt.OnDataArrived += Opt_OnDataArrived;
            //初始化
            opt.Init();
            opt.StartTest();
            Console.ReadLine();
        }

        //底层触发事件后上层可以在注册的事件中得到
        private static void Opt_OnDataArrived(FeedBack data)
        {
            Console.WriteLine("ThetaPower = " + data.ThetaPower.ToString());
            Console.WriteLine("PhiPower = " + data.PhiPower.ToString());
        }
    }
}
