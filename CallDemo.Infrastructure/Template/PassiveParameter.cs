using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.Template
{
    public class PassiveParameter: TopParameter
    {
        public readonly SystemTestMode systemTestMode = SystemTestMode.Passive;
        private Instrument instrument;
        private string testSystem;
        private string traceName;
        private int IF_Bandwidth;
        private int factorAverage;
        private double power;
        private PassiveTestMode testMode;
        private bool skipCalibration;

        private int thetaStart;
        private int thetaStep;
        private int thetaStop;
        private int phiStart;
        private int phiStep;
        private int phiStop;

        private PassiveTestMode Test_Method = PassiveTestMode.LogAndPhase;

        private List<Angle> Segm_Freq;
        private List<double> List_Freq;
        private double CW_Freq;

        public PassiveParameter()
        {
            //Instrument = Instrument.Emulator;
            Instrument = Instrument.Agilent_5071C;
            thetaStart = 0;
            thetaStep = 15;
            thetaStop = 180;
            phiStart = 0;
            phiStep = 15;
            phiStop = 360;
        }
        public double Power {
            get => power;
            set
            {
                if (value <= 5)
                    power = value;
                else
                    power = 5;
            }
        }
        
        public string TestSystem { get => testSystem; set => testSystem = value; }
        public string TraceName { get => traceName; set => traceName = value; }
        public int IF_Bandwidth1 { get => IF_Bandwidth; set => IF_Bandwidth = value; }
        public int ThetaStart { get => thetaStart; set => thetaStart = value; }
        public int ThetaStep { get => thetaStep; set => thetaStep = value; }
        public int ThetaStop { get => thetaStop; set => thetaStop = value; }
        public int PhiStart { get => phiStart; set => phiStart = value; }
        public int PhiStep { get => phiStep; set => phiStep = value; }
        public int PhiStop { get => phiStop; set => phiStop = value; }
        public int FactorAverage {
            get => factorAverage;
            set
            {
                if(value>0)
                    factorAverage = value;
                else
                    factorAverage = 1;
            }
        }

        public Instrument Instrument { get => instrument; set => instrument = value; }
    }
    public enum PassiveTestMode
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
