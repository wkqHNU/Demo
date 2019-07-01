using CallDemo.Infrastructure.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure
{
    class Pathloss
    {
        DataTable dtPathloss;
        public Pathloss()
        {
            string filepath;
            filepath = @"..\..\..\doc\all_Dipole_2800(congcong).csv.";
            dtPathloss = CsvFileHelper.Csv2DataTable(filepath); // 耗时200ms
            int length = dtPathloss.Columns.Count / 2;
            for (int i = 1; i < length; i++)
            {
                dtPathloss.Columns.Remove("Column"+i.ToString());
            }
        }
        public double GetPathloss(double freq, int phi, Polar polar)
        {
            double data;
            DataRow dr;
            object[] objs;
            DataColumn[] cols;
            string colName;
            int antennaNum;
            antennaNum = (phi + 15) / 30;
            if (polar == Polar.Theta)
            {
                colName = antennaNum.ToString() + "H";
            }
            else
            {
                colName = antennaNum.ToString() + "V";
            }
            cols = new DataColumn[] { dtPathloss.Columns["Freq(MHz)"] };
            dtPathloss.PrimaryKey = cols;
            objs = new object[] { freq };
            dr = dtPathloss.Rows.Find(objs);
            data = double.Parse(dr[colName].ToString());
            return data;
        }
    }
}
