using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure
{
    public class Operation
    {
        public Operation(int FunctionID)
        {

        }
       
        //添加事件
        public delegate void DataArrivedHanlder(FeedBack data);
        public event DataArrivedHanlder OnDataArrived;
        private void DataArrived(FeedBack data)
        {
            if (OnDataArrived != null)
                OnDataArrived(data);
        }
        //初始化
        public bool Init()
        {
            return true;
        }
        //开始测试
        public bool StartTest()
        {
            //中间为测试过程
            SingleStep();
            return true;
        }
        //dll私有方法
        private void SingleStep()
        {
            FeedBack data = new FeedBack();//应该回传给上层的数据
            data.ThetaAngle = 15;
            data.PhiAngle = 30;
            data.ThetaPower = 10.2;
            data.PhiPower = 5;
            //底层得到可以回传的数据后触发事件
            DataArrived(data);
        }
    }
}
