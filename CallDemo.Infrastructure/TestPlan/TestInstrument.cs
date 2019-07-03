using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.TestPlan
{
    public class TestInstrument
    {
        public bool enableEmulator = false;
        public virtual void InitTheta8Phi() { }
        public virtual void Page() { }
        public virtual bool IsConnected() { return true; }
        public virtual void InitDraw() { }
        public virtual void SetInstrumentComm() { }
        public virtual void SetInstrumentTest() { }
        public virtual void SetTheta8Phi(int theta, int phi, Polar polar) { }
        public virtual void SetPower() { }
        public virtual void PushData() { }
        public virtual void GetData() { }
        public virtual void PopData() { }
        public virtual double GetAntennaPattern(double theta, double phi, Polar polar, double dataOffset) { return 1.0; }

    }
}
