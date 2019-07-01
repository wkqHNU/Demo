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
    class EmulatorPassive:ProjectOTA
    {
        DataTable dtGainTheta;
        DataTable dtGainPhi;
        public EmulatorPassive() 
        {
            string filepath;
            filepath = @"..\..\..\CallDemo.Infrastructure\Emulator\Dipole_dB(GainTheta).csv";
            dtGainTheta = CsvFileHelper.Csv2DataTable(filepath); // 耗时200ms
            
            filepath = @"..\..\..\CallDemo.Infrastructure\Emulator\Dipole_dB(GainPhi).csv";
            dtGainPhi = CsvFileHelper.Csv2DataTable(filepath); // 耗时200ms

        }
        public EmulatorPassive(int FunctionID) : base(FunctionID)
        {

        }
        public override bool IsConnected() { return true; }
        public override double GetAntennaPattern(double theta, double phi, Polar polar, double dataOffset)
        {
            double data;
            DataRow dr;
            object[] objs;
            DataColumn[] cols;
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
            // 3.1.联合两个字段为一个主键
            cols = new DataColumn[] { dt.Columns["Phi"], dt.Columns["Theta"] };
            dt.PrimaryKey = cols;
            // 3.2.查询
            objs = new object[] { phi, theta };
            dr = dt.Rows.Find(objs);
            data = double.Parse(dr[colName].ToString());
            data = data - dataOffset;
            return data;
        }
    }
}
