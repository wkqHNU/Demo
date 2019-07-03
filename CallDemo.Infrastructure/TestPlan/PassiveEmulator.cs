using CallDemo.Infrastructure.TestPlan;
using CallDemo.Infrastructure.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure.Emulator
{
    class PassiveEmulator: TestInstrument
    {
        DataTable dtGainTheta;
        DataTable dtGainPhi;
        public PassiveEmulator() 
        {
            string filepath;
            filepath = @"..\..\..\CallDemo.Infrastructure\TestPlan\Dipole_dB(GainTheta).csv";
            dtGainTheta = CsvFileHelper.Csv2DataTable(filepath); // 耗时200ms
            DataColumn[] cols;
            cols = new DataColumn[] { dtGainTheta.Columns["Phi"], dtGainTheta.Columns["Theta"] };
            dtGainTheta.PrimaryKey = cols;

            filepath = @"..\..\..\CallDemo.Infrastructure\TestPlan\Dipole_dB(GainPhi).csv";
            dtGainPhi = CsvFileHelper.Csv2DataTable(filepath); // 耗时200ms
            cols = new DataColumn[] { dtGainPhi.Columns["Phi"], dtGainPhi.Columns["Theta"] };
            dtGainPhi.PrimaryKey = cols;
        }
        public override bool IsConnected() { return true; }
        public override double GetAntennaPattern(double theta, double phi, Polar polar, double dataOffset)
        {
            double data;
            DataRow dr;
            object[] objs;
            DataTable dt;
            string colName;
            if (polar == Polar.Theta)
            {
                dt = dtGainTheta;
                colName = "dB(GainTheta)";
            }
            else
            {
                dt = dtGainPhi;
                colName = "dB(GainPhi)";
            }
            objs = new object[] { phi, theta };
            dr = dt.Rows.Find(objs);
            data = double.Parse(dr[colName].ToString());
            data = data - dataOffset;
            return data;
        }
    }
}
