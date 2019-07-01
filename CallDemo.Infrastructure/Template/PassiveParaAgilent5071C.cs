using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Template
{
    class PassiveParameter
    {
        public string instrument;
        public string testSystem;
        public string traceName;
        public int IF_Bandwidth;
        public int factorAverage;
        public double power;
        public TestMode testMode;
        public bool skipCalibration;

        public int thetaStart;
        public int thetaStep;
        public int thetaStop;
        public int phiStart;
        public int phiStep;
        public int phiStop;

        public TestMethod Test_Method;

        public List<Angle> Segm_Freq;
        public List<double> List_Freq;
        public double CW_Freq;
    }
    public enum TestMode
    {
        LogAndPhase = 0,
        RealAndImag,
        Log,
    }
    public enum TestMethod
    {
        Linear = 0,
        ListFreq,
        CW,
    }
    public class Angle
    {
        public double Start;
        public double End;
        public double Step;
    }
}
