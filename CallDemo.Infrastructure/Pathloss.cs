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
    public class Pathloss
    {
        DataTable dtPathloss;
        public Pathloss(string filePath)
        {
            dtPathloss = CsvFileHelper.Csv2DataTable(filePath); // 耗时200ms
            int length = dtPathloss.Columns.Count / 2;
            for (int i = 1; i < length; i++)
            {
                dtPathloss.Columns.Remove("Column"+i.ToString());
            }
            DataColumn[] cols;
            cols = new DataColumn[] { dtPathloss.Columns["Freq(MHz)"] };
            dtPathloss.PrimaryKey = cols;
        }
        /// <summary>
        /// 路损表中正好有输入的频率值
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="theta"></param>
        /// <param name="polar"></param>
        /// <returns></returns>
        private double GetPathlossBase(double freq, int theta, Polar polar)
        {
            double data;
            DataRow dr;
            object[] objs;
            string colName;
            int antennaNum;
            antennaNum = (theta + 15) / 30;
            if (polar == Polar.Theta)
            {
                colName = antennaNum.ToString() + "H";
            }
            else
            {
                colName = antennaNum.ToString() + "V";
            }
            objs = new object[] { freq };
            dr = dtPathloss.Rows.Find(objs);
            data = double.Parse(dr[colName].ToString());
            return data;
        }
        /// <summary>
        /// 路损表中没有有输入的频率值
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="theta"></param>
        /// <param name="polar"></param>
        /// <returns></returns>
        public double GetPathloss(double freq, int theta, Polar polar)
        {
            double data = 0;
            string colName;
            int antennaNum;
            int rowsCount;
            double freqMin;
            double freqMax;
            antennaNum = (theta + 15) / 30;
            rowsCount = dtPathloss.Rows.Count;
            double[] colFreq = new double[rowsCount];
            if (polar == Polar.Theta)
            {
                colName = antennaNum.ToString() + "H";
            }
            else
            {
                colName = antennaNum.ToString() + "V";
            }
            freqMin = double.Parse(dtPathloss.Rows[0]["Freq(MHz)"].ToString());
            freqMax = double.Parse(dtPathloss.Rows[rowsCount-1]["Freq(MHz)"].ToString());

            try
            {
                if (freq < freqMin)
                    throw new FreqOutOfRangeException("error: freq < freqMin");
                if (freq > freqMax)
                    throw new FreqOutOfRangeException("error: freq > freqMax");
            }
            catch (FreqOutOfRangeException me)
            {
                Console.WriteLine(me.Message);
                return 0;
            }
            for (int i = 0; i < dtPathloss.Rows.Count; i++)
            {
                colFreq[i] = double.Parse(dtPathloss.Rows[i]["Freq(MHz)"].ToString());
                if(freq == colFreq[i])
                {
                    data = GetPathlossBase(colFreq[i], theta, polar);
                    break;
                }
                else if(freq < colFreq[i])
                {
                    double slope;
                    slope = (GetPathlossBase(colFreq[i], theta, polar) - GetPathlossBase(colFreq[i - 1], theta, polar))
                        / (colFreq[i] - colFreq[i-1]);
                    data = slope * (freq - colFreq[i - 1]) + GetPathlossBase(colFreq[i - 1], theta, polar);
                    break;
                }
            }
            return data;
        }
        class FreqOutOfRangeException : Exception
        {
            public FreqOutOfRangeException(string message) : base(message) { }

            public override string Message
            {
                get
                {
                    return base.Message;
                }
            }
        }
    }
}
