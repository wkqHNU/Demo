using CallDemo.Infrastructure.Emulator;
using CallDemo.Infrastructure.Space;
using CallDemo.Infrastructure.Template;
using CallDemo.Infrastructure.TestPlan;
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
    public class TestProcess
    {
        public SystemTestMode systemTestMode;
        public TestSpace ts = new TestSpace();
        public TestResult result = new TestResult();
        public Pathloss pathloss;
        public TestInstrument instrument = null;
        public TestProcess process = null;

        //系统初始化
        public bool SystemInit(GlobalParameter globalPara)
        {
            //----------------------------------------------
            // 初始化理论空间：theta，phi
            Theta thetaModel;
            switch (globalPara.ThetaModel)
            {
                case "Theta1800":
                    thetaModel = new Theta1800();
                    break;
                default:
                    thetaModel = new Theta1800();
                    break;
            }
            Phi phiModel;
            switch (globalPara.PhiModel)
            {
                case "PhiDaMoDa":
                    phiModel = new PhiDaMoDa();
                    break;
                default:
                    phiModel = new PhiDaMoDa();
                    break;
            }
            ts.thetaModel = thetaModel;
            ts.phiModel = phiModel;
            // 获取路损文件
            pathloss = new Pathloss(globalPara.PathlossFilePath);
            return true;
        }
        public TestProcess() { }
        public bool Init(PassiveParameter template)
        {
            if(template.Instrument == Instrument.Emulator)
                instrument = new PassiveEmulator();
            else if (template.Instrument == Instrument.Agilent_5071C)
                instrument = new PassiveAgilent5071C();
            instrument.enableEmulator = template.enableEmulator;
            ts.thetaStart = template.ThetaStart;
            ts.thetaStep = template.ThetaStep;
            ts.thetaStop = template.ThetaStop;
            ts.phiStart = template.PhiStart;
            ts.phiStep = template.PhiStep;
            ts.phiStop = template.PhiStop;
            ts.phiStopFlag = false;
            if ((ts.phiStart == 0) && (ts.phiStop == 360))
                ts.phiStop -= ts.phiStep;
            else if ((ts.phiStart == 360) && (ts.phiStop == 360))
                ts.phiStop = 0;
            else
                ts.phiStopFlag = true;
            systemTestMode = template.systemTestMode;
            return true;
        }
       
        //下层调用上层
        // 测试过程中返回每个角度每个极化下的值
        public delegate void DataArrivedHanlder1(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain);
        public event DataArrivedHanlder1 Testing;
        private void SendTestintData(double freq, int thetaPhysical, int phiPhysical, Polar polar, double gain)
        {
            if (Testing != null)
                Testing(freq, thetaPhysical, phiPhysical, polar, gain);
        }
        // 测试结束，返回最终测试结果
        public delegate void DataArrivedHanlder2(TestResult result);
        public event DataArrivedHanlder2 TestEnd;
        private void SendTestEndResult(TestResult result)
        {
            if (TestEnd != null)
                TestEnd(result);
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
            double freq = 690.32;
            double dataOffset;
            instrument.InitTheta8Phi();
            instrument.SetInstrumentComm();
            result.dtDataRaw_dB.Clear();
            result.dtData_dB.Clear();
            while (!connected)
            {
                instrument.Page();
                connected = instrument.IsConnected();
            }
            for (int phiPhysical = ts.phiStart; phiPhysical <= ts.phiStop; phiPhysical = phiPhysical + ts.phiStep)
            {
                for (int thetaPhysical = ts.thetaStart; thetaPhysical <= ts.thetaStop; thetaPhysical = thetaPhysical + ts.thetaStep)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Polar polar;
                        if (i == 0)
                            polar = Polar.Theta;
                        else
                            polar = Polar.Phi;
                        instrument.SetInstrumentTest();
                        if ((systemTestMode == SystemTestMode.TRP)|| (systemTestMode == SystemTestMode.TRP))
                            ts.SetTheta8Phi(thetaPhysical, phiPhysical, Direction.Uplink, polar);
                        else
                            ts.SetTheta8Phi(thetaPhysical, phiPhysical, Direction.Downlink, polar);
                        //角度转换
                        ts.Convert2TheoreticalSpace(thetaPhysical,phiPhysical);
                        //pathloss = getPathloss(freq,theta, phi, HV);
                        pathlossTemp = pathloss.GetPathloss(freq, thetaPhysical, polar);
                        switch (systemTestMode)
                        {
                            case SystemTestMode.Passive:
                                // dut作为接收天线
                                // txPower + externalGain - pathloss + dutGain = rxPower;
                                // dutGain = S21(rxPower-txPower) + pathloss - externalGain;
                                // dut作为发射天线
                                // txPower + dutGain - pathloss + externalGain = rxPower;
                                // dutGain = S21(rxPower-txPower) + pathloss - externalGain;
                                // rawData = S21
                                dataOffset = pathlossTemp + pathlossOffset - externalGain;
                                result.dataRawTemp_dB = instrument.GetAntennaPattern(ts.thetaModel.theta,ts.phiModel.phi, polar, dataOffset);
                                result.dataTemp_dB = result.dataRawTemp_dB + dataOffset;
                                result.AddRow(result.dtDataRaw_dB, freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataRawTemp_dB);
                                break;
                            case SystemTestMode.TRP:
                                // dut作为发射端
                                // txPower + dutGain - pathloss + internalGain + externalGain = rxPower
                                // txPower + dutGain = rxPower + pathloss - internalGain - externalGain
                                break;
                            case SystemTestMode.TIS:
                                // dut作为接收端
                                // txPower + internalGain + externalGain - pathloss + dutGain = rxPower;
                                // rxPower - dutGain = txPower - pathloss + internalGain + externalGain
                                break;
                            case SystemTestMode.EIRP:
                                break;
                            case SystemTestMode.EIS:
                                break;
                            default:
                                throw new Exception("");
                        }
                        result.AddRow(result.dtData_dB, freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataTemp_dB);
                        SendTestintData(freq, ts.thetaModel.theta, ts.phiModel.phi, polar, result.dataTemp_dB);
                    }
                }
            }
            // 根据多个字段排序
            result.dtDataRaw_dB.DefaultView.Sort = "Polar DESC, Theta ASC, Phi ASC";
            result.dtDataRawOrder_dB = result.dtDataRaw_dB.DefaultView.ToTable();
            result.dtData_dB.DefaultView.Sort = "Polar DESC, Theta ASC, Phi ASC";
            result.dtDataOrder_dB = result.dtData_dB.DefaultView.ToTable();
            CsvFileHelper.DataTable2Csv(result.dtDataOrder_dB, @"C:\Users\17213\Desktop\dtDataOrder_dB.csv");
            result.CalGainOrTrpOrTis(
                ts.thetaStart, ts.thetaStep, ts.thetaStop, ts.phiStart, ts.phiStep, ts.phiStop,
                result.dtData_dB, true);
            SendTestEndResult(result);
        }
        public void RtsTest()
        {
        }




    }
}
