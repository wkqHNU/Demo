using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CallDemo.Infrastructure.EnumList;

namespace CallDemo.Infrastructure
{
    public class TestResult
    {
        public double dataRawTemp_dB; // 无源：gain，有源上行：eirp，有源下行：eis
        public double dataTemp_dB;
        public DataTable dtDataRaw_dB = new DataTable();
        public DataTable dtData_dB = new DataTable();
        const int length = 3; // 0:单极化theta 1:单极化phi 2:总极化total
        double[] aveTotal = new double[length];     // 无源：效率，有源上行：TRP,有源下行：TIS
        double[] aveTotal_dB = new double[length];  // 以对数表示
        double[] aveNHP30 = new double[length]; // NHP: Near Horizon Parital
        double[] aveNHP30_dB = new double[length];
        double[] aveNHP45 = new double[length]; // NHP: Near Horizon Parital
        double[] aveNHP45_dB = new double[length];
        double[] dataMax_dB = new double[length];
        double[] dataMaxTheta = new double[length];
        double[] dataMaxPhi = new double[length];
        double[] dataMin_dB = new double[length];
        double[] dataMinTheta = new double[length];
        double[] dataMinPhi = new double[length];
        public TestResult()
        {
            DataColumn dc1;
            DataColumn dc2;
            string[] aryLine = new string[] { "Freq", "Theta", "Phi", "Polar", "dB(data)"};
            //创建列,并命名字段名称
            for (int i = 0; i < aryLine.Length; i++)
            {
                // 语法规定不能用同一个dc
                if ((i == 1)|| (i == 2))
                {
                    dc1 = new DataColumn(aryLine[i], typeof(int));
                    dc2 = new DataColumn(aryLine[i], typeof(int));
                }
                else
                {
                    dc1 = new DataColumn(aryLine[i]);
                    dc2 = new DataColumn(aryLine[i]);
                }
                dtDataRaw_dB.Columns.Add(dc1);
                dtData_dB.Columns.Add(dc2);
            }
        }
        public void AddRow(DataTable dt, double freq, int theta, int phi, Polar polar, double data)
        {
            DataRow dr = dt.NewRow();
            dr[0] = freq;
            dr[1] = theta;
            dr[2] = phi;
            dr[3] = polar;
            dr[4] = data;
            dt.Rows.Add(dr);
        }
        public double[] AverageInSphericalCoordinateSystem(
            int thetaStart, int thetaStep, int thetaEnd,
            int phiStart, int phiStep, int phiEnd, 
            int thetaCount, int phiCount, DataTable dt_dB, bool isTrpOrTis)
        {
            double[] ave = new double[length];
            //DateTime dt1 = DateTime.Now;
            bool TRPOrGain = true;
            //bool TIS = false;
            DataColumn[] cols;
            object[] objs;
            DataRow dr;
            double[] data_dB = new double[length];
            double[] data = new double[length];
            double[] sum = new double[length];
            for (int i = 0; i < length; i++)
            {
                dataMax_dB[i] = int.MinValue;
                dataMin_dB[i] = int.MaxValue;
            }
            for (int theta = thetaStart; theta <= thetaEnd; theta+=thetaStep)
            {
                for (int phi = phiStart; phi <= phiEnd; phi += phiStep)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Polar polar;
                        if (i == 0)
                            polar = Polar.Theta;
                        else
                            polar = Polar.Phi;
                        cols = new DataColumn[] {
                            dt_dB.Columns["Theta"], dt_dB.Columns["Phi"], dt_dB.Columns["Polar"] };
                        dt_dB.PrimaryKey = cols;
                        objs = new object[] { theta, phi, polar };
                        dr = dt_dB.Rows.Find(objs);
                        data_dB[i] = double.Parse(dr["dB(data)"].ToString());
                        data[i] = Math.Pow(10,data_dB[i]/10);
                    }
                    data[2] = data[0] + data[1];
                    data_dB[2] = 10 * Math.Log10(data[2]);
                    for (int i = 0; i < length; i++)
                    {
                        if (isTrpOrTis == TRPOrGain)
                            sum[i] += data[i] * Math.Sin(theta * Math.PI / 180);
                        else //if (isTrpOrTis == TIS)
                            sum[i] += Math.Sin(theta * Math.PI / 180) / data[i];
                    }

                    for (int i = 0; i < length; i++)
                    {
                        if (data_dB[i] > dataMax_dB[i])
                        {
                            dataMax_dB[i] = data_dB[i];
                            dataMaxTheta[i] = theta;
                            dataMaxPhi[i] = phi;
                        }
                        if (data_dB[i] < dataMin_dB[i])
                        {
                            dataMin_dB[i] = data_dB[i];
                            dataMinTheta[i] = theta;
                            dataMinPhi[i] = phi;
                        }
                    }
                }
            }
            for (int i = 0; i < length; i++)
            {
                if (isTrpOrTis == TRPOrGain)
                    ave[i] = Math.PI * sum[i] / (2 * thetaCount * phiCount);
                else //if (isTrpOrTis == TIS)
                    ave[i] = (2 * thetaCount * phiCount) / Math.PI * sum[i];
            }
            //DateTime dt2 = DateTime.Now;
            //double msecInterval1 = dt2.Subtract(dt1).TotalMilliseconds;
            return ave;
        }
        public void CalGainOrTrpOrTis(
            int thetaStart, int thetaStep, int thetaEnd,
            int phiStart, int phiStep, int phiEnd,
            DataTable dt_dB, bool isTrpOrTis)
        {
            int thetaStartTemp;
            int thetaEndTemp;
            int thetaCount = (thetaEnd - thetaStart) / thetaStep + 1;
            int phiCount = (phiEnd - phiStart) / phiStep + 1;

            // 接近水平正负30度的辐射功率
            thetaStartTemp = thetaStart;
            thetaEndTemp = thetaEnd;
            for (int theta = thetaEnd; theta >= thetaStart; theta -= thetaStep)
            {
                if (theta <= 90 - 30)
                {
                    thetaStartTemp = theta;
                    break;
                }
            }
            for (int theta = thetaStart; theta <= thetaEnd; theta += thetaStep)
            {
                if (theta >= 90 + 30)
                {
                    thetaEndTemp = theta;
                    break;
                }
            }
            aveNHP30 = AverageInSphericalCoordinateSystem(
                thetaStartTemp, thetaStep, thetaEndTemp,
                phiStart, phiStep, phiEnd,
                thetaCount, phiCount, dt_dB, isTrpOrTis);
            for (int i = 0; i < length; i++)
                aveNHP30_dB[i] = 10 * Math.Log10(aveNHP30[i]);

            // 接近水平正负45度的辐射功率
            thetaStartTemp = thetaStart;
            thetaEndTemp = thetaEnd;
            for (int theta = thetaEnd; theta >= thetaStart; theta -= thetaStep)
            {
                if (theta <= 90 - 45)
                {
                    thetaStartTemp = theta;
                    break;
                }
            }
            for (int theta = thetaStart; theta <= thetaEnd; theta += thetaStep)
            {
                if (theta >= 90 + 45)
                {
                    thetaEndTemp = theta;
                    break;
                }
            }
            aveNHP45 = AverageInSphericalCoordinateSystem(
                thetaStartTemp, thetaStep, thetaEndTemp,
                phiStart, phiStep, phiEnd,
                thetaCount, phiCount, dt_dB, isTrpOrTis);
            for (int i = 0; i < length; i++)
                aveNHP45_dB[i] = 10 * Math.Log10(aveNHP45[i]);

            // TRP
            aveTotal = AverageInSphericalCoordinateSystem(
                thetaStart, thetaStep, thetaEnd, 
                phiStart, phiStep, phiEnd,
                thetaCount, phiCount, dt_dB, isTrpOrTis);
            for (int i = 0; i < length; i++)
                aveTotal_dB[i] = 10 * Math.Log10(aveTotal[i]);
        }
    }
}
