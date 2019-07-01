using CallDemo.Infrastructure.Emulator;
using CallDemo.Infrastructure.Space;
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
    public class ProjectOTA
    {
        TestSpace ts = new TestSpace();
        TestResult result;
        public ProjectOTA ota = null;
        public int thetaStart = 0; // 1800/2800暗室不能取0，如果取0，路损文件也要跟着修改
        public int thetaStep = 15;
        public int thetaEnd = 180;
        public int phiStart = 0;
        public int phiStep = 15;
        public int phiEnd = 360;
        public TestMode testMode = TestMode.Passive;
        public Passive Spara = Passive.S21;
        Pathloss pathloss;

        public ProjectOTA()
        {

        }
        public ProjectOTA(int FunctionID)
        {

        }
       
        //添加事件
        public delegate void DataArrivedHanlder(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain);
        public event DataArrivedHanlder GetTestPoint;
        private void SendTestPoint(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain)
        {
            if (GetTestPoint != null)
                GetTestPoint(freq, thetaPhysical, phiPhysical, polar, gain);
        }
        //初始化
        public bool Init()
        {
            if ((phiStart == 0) && (phiEnd == 360))
                phiEnd -= phiStep;
            else if ((phiStart == 360) && (phiEnd == 360))
                phiEnd = 0;
            //----------------------------------------------
            // 初始化理论空间：theta，phi
            Theta1800 theta1800 = new Theta1800();
            ts.thetaModel = theta1800;
            PhiDaMoDa phiDaMoDa = new PhiDaMoDa();
            ts.phiModel = phiDaMoDa;
            //----------------------------------------------
            result = new TestResult();
            ota = new EmulatorPassive();
            pathloss = new Pathloss();
            return true;
        }
        //开始测试
        public bool StartTest()
        {
            PatternTest();
            RtsTest();
            return true;
        }
        public void PatternTest()
        {
            bool connected = true;
            double pathlossTemp = 0;
            double pathlossOffset = 0;
            double externalGain = 0;
            double freq = 690.2;
            double dataOffset;
            InitTheta8Phi();
            SetInstrumentComm();
            while (!connected)
            {
                Page();
                connected = IsConnected();
            }
            for (int phiPhysical = phiStart; phiPhysical <= phiEnd; phiPhysical = phiPhysical + phiStep)
            {
                for (int thetaPhysical = thetaStart; thetaPhysical <= thetaEnd; thetaPhysical = thetaPhysical + thetaStep)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Polar polar;
                        if (i == 0)
                            polar = Polar.Theta;
                        else
                            polar = Polar.Phi;
                        SetInstrumentTest();
                        if ((testMode == TestMode.TRP)|| (testMode == TestMode.TRP))
                            ts.SetTheta8Phi(thetaPhysical, phiPhysical, Direction.Uplink, polar);
                        else
                            ts.SetTheta8Phi(thetaPhysical, phiPhysical, Direction.Downlink, polar);
                        //角度转换
                        ts.Convert2TheoreticalSpace(thetaPhysical,phiPhysical);
                        //pathloss = getPathloss(freq,theta, phi, HV);
                        pathlossTemp = pathloss.GetPathloss(freq, thetaPhysical, polar);
                        switch (testMode)
                        {
                            case TestMode.Passive:
                                // dut作为接收天线
                                // txPower + externalGain - pathloss + dutGain = rxPower;
                                // dutGain = S21(rxPower-txPower) + pathloss - externalGain;
                                // dut作为发射天线
                                // txPower + dutGain - pathloss + externalGain = rxPower;
                                // dutGain = S21(rxPower-txPower) + pathloss - externalGain;
                                // rawData = S21
                                dataOffset = pathlossTemp + pathlossOffset - externalGain;
                                result.dataRawTemp_dB = ota.GetAntennaPattern(ts.thetaModel.theta,ts.phiModel.phi, polar, dataOffset);
                                result.dataTemp_dB = result.dataRawTemp_dB + dataOffset;
                                result.AddRow(result.dtDataRaw_dB, freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataRawTemp_dB);
                                break;
                            case TestMode.TRP:
                                // dut作为发射端
                                // txPower + dutGain - pathloss + internalGain + externalGain = rxPower
                                // txPower + dutGain = rxPower + pathloss - internalGain - externalGain
                                break;
                            case TestMode.TIS:
                                // dut作为接收端
                                // txPower + internalGain + externalGain - pathloss + dutGain = rxPower;
                                // rxPower - dutGain = txPower - pathloss + internalGain + externalGain
                                break;
                            case TestMode.EIRP:
                                break;
                            case TestMode.EIS:
                                break;
                            default:
                                throw new Exception("");
                        }
                        result.AddRow(result.dtData_dB, freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataTemp_dB);
                        SendTestPoint(freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataTemp_dB);
                    }
                }
            }
            // 根据多个字段排序
            result.dtData_dB.DefaultView.Sort = "Polar DESC, Theta ASC, Phi ASC";
            result.dtData_dB = result.dtData_dB.DefaultView.ToTable();
            CsvFileHelper.DataTable2Csv(result.dtData_dB, @"C:\Users\17213\Desktop\dtGain.csv");
            result.CalGainOrTrpOrTis(
                thetaStart, thetaStep, thetaEnd, phiStart, phiStep, phiEnd,
                result.dtData_dB, true);
        }
        public void RtsTest()
        {
        }
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
