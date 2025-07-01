using Fx.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        DeviceInfo nuviaGetInfo()
        {

            info = new DeviceInfo();
            info.Version = nuvia.GetDevVersion();                                // get FW version
            try
            {
                /*var nuvParams = nuvia.ParseParam(nuvia.GetParam("12, 10027, 20004, 10060, 10105"));

                info.SN = nuvParams[12];             // get SN
                info.Model = nuvParams[10027];    // get Model
                info.Date = "";
                info.Chip = nuvParams[20004];
                info.TypeString = nuvParams[10060];*/
                info.SN = nuvia.ParseParam(nuvia.GetParam("12"))[12];             // get SN
                info.Model = nuvia.ParseParam(nuvia.GetParam("10027"))[10027];    // get Model
                info.Date = "";
                info.Chip = nuvia.ParseParam(nuvia.GetParam("20004"))[20004];
                info.TypeString = nuvia.ParseParam(nuvia.GetParam("10060"))[10060];
                info.BoardID = nuvia.ParseParam(nuvia.GetParam("10061"))[10061];
                if (info.Version.Contains("EGM") || info.Version.Contains("GMS"))
                {
                    info.TypeString = "EGM";
                    Type = DeviceType.EGM;
                }
                else if (info.Version.Contains("SCA"))
                {
                    info.TypeString = "SCA";
                    Type = DeviceType.MCA;
                    Support |= DevSupport.Spectrum;
                }
                else if (info.Version.Contains("MCB") || info.Version.ToLower().Contains("mobrams"))
                {
                    info.TypeString = "MCB";
                    Type = DeviceType.MCA;
                    Support |= DevSupport.Spectrum;
                }


                info.Type = Type;

                Support |= DevSupport.Firmware;

                var perm = nuvia.ParseParam(nuvia.GetParam("10105"))[10105];    // get Model

                if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                else if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                else if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                else if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
                
                if (perm == "Unknown")
                {
                    Permission = DevPermission.SuperUser;
                } 
                else
                {
                    Support |= DevSupport.Permission;
                }
                    
            }

            catch { }

            return info;
        }

        SCAValue nuviaGetMCAValue()
        {

            string Values = nuvia.GetParam("9999");

            try
            {
                lastMeas = nuvia.ParseParam(Values);

                // ----- Permissions -----
                try
                {
                    var perm = lastMeas[10105];    // get Model

                    if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                    if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                    if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                    if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
                }
                catch { }

                return GetMCAValFromDict(lastMeas);
            }
            catch
            {
                throw new Exception("Data Parsing Error");
            }
        }

        SCAValue nuviaGetMCAValFromDict(Dictionary<int, string> dict)
        {
            SCAValue val = new SCAValue();



            // ----- ROI CPS -----
            string CPS = dict[10302];
            string[] CPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);
            string aCPS = dict[10306];
            string[] aCPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);

            int detNum = CPSarr.Length;                 // ROI count


            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];

            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = Conv.ToFloatI(CPSarr[i], 0);
                val.ActualCPS[i] = Conv.ToFloatI(aCPSarr[i], 0);
            }

            // ----- Measurement Timestamp -----
            val.timeStamp = Conv.ToInt(dict[10309], 0);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;

            // ----- Temperature -----
            try
            {
                if ((dict[10014] == "") || (dict[10014] == "nan") || (dict[10014] == "255"))
                    val.Temperature = float.NaN;
                else
                    val.Temperature = Conv.ToFloatI(dict[10014], 0);
            }
            catch
            {
                val.Temperature = float.NaN;
            }

            // ----- Read ext temperature -----
            try
            {
                if (dict[10015] != "nan") val.Temperature = Conv.ToFloatI(dict[10015], 0);
            }
            catch { }
            try
            {
                if (dict[10016] != "nan") val.Temperature = Conv.ToFloatI(dict[10016], 0);
            }
            catch { }

            // ----- Status -----
            try
            {
                val.Error = Conv.ToInt(dict[10100], 0);
                val.Status = Conv.ToInt(dict[10100], 0);
            }
            catch
            {
                val.Error = 0;
                val.Status = 0;
            }

            val.isROIRunning = true;
            NuviaSCA_Status NuStat = (NuviaSCA_Status)val.Status;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN1))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN2))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN3))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN4))
                val.isROIRunning = false;

            val.isSpectRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_SPECTRUM))
                val.isSpectRunning = true;

            return val;
        }

        SCASettings nuviaGetMCASettings()
        {
            
            mcaSettings = new SCASettings();
            nuvia.SetParam("1=0");    // Select ROI1
            string param = nuvia.GetParam("1,3,4,5,6,13,14");
            Dictionary<int, string> dict = nuvia.ParseParam(param);

            mcaSettings.MeasureTime = Conv.ToInt(dict[6], 0);
            //settings.isRunning = 

            mcaSettings.HV = Conv.ToInt(dict[13], 0);
            if (dict[14] == "1") mcaSettings.HV_up = true;
            else mcaSettings.HV_up = false;

            /*
            settings.Channels = new SCAChannel[4];
            
            for (int i = 0; i < 4; i++)
            {
                if (i > 0)
                {
                    prot.SetParam("1=" + i.ToString());                             // Select ROI
                    param = prot.GetParam("3,4,5,6");
                    dict = prot.ParseParam(param);
                }
                settings.Channels[i].LLD = (uint)Conv.ToIntDef(dict[4], 0);         // Fill LLD
                settings.Channels[i].ULD = (uint)Conv.ToIntDef(dict[5], 0);         // Fill ULD
                settings.Channels[i].Time = (uint)Conv.ToIntDef(dict[6], 0);        // Fill Time
                if (prot.ParseParam(param)[3] == "1") settings.Channels[i].Autostart = true;    // Fill Autostart
                else settings.Channels[i].Autostart = false;
            }*/


            return mcaSettings;
        }

        GeigerValue nuviaGetEGMValue()
        {

            string Values = nuvia.GetParam("9999");

            try
            {
                lastMeas = nuvia.ParseParam(Values);

                // ----- Permissions -----
                try
                {
                    var perm = lastMeas[10105];    // get Model

                    if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                    if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                    if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                    if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
                }
                catch { }

                return GetEGMValFromDict(lastMeas);
            }
            catch
            {
                throw new Exception("Data Parsing Error");
            }
        }

        GeigerValue nuviaGetEGMValFromDict(Dictionary<int, string> dict)
        {
            GeigerValue val = new GeigerValue();

            // ----- Permissions -----
            try
            {
                var perm = dict[10105];    // get Model

                if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
            }
            catch { }

            if (InBootloaderMode)
            {
                val.DR = 0;
                val.ActualDR = 0;
                val.CPS = new float[0];
                val.ActualCPS = new float[0];
                val.Valid = false;

                try
                {
                    if (dict[10014] == "")
                        val.Temperature = float.NaN;
                    else
                        val.Temperature = Conv.ToFloatI(dict[10014], 0);
                }
                catch
                {
                    val.Temperature = float.NaN;
                }


                val.isRunning = false;

                return val;
            }

            // ----- ROI CPS -----
            string CPS = dict[10302];
            string[] CPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);
            string aCPS = dict[10306];
            string[] aCPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);

            int detNum = CPSarr.Length;
            int trustedROI = Conv.ToInt(dict[10310], 0);


            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];

            val.DR = Conv.ToFloatI(dict[10301], 0) / 1000000f;
            val.ActualDR = Conv.ToFloatI(dict[10305], 0) / 1000000f;

            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = Conv.ToFloatI(CPSarr[i], 0);
                val.ActualCPS[i] = Conv.ToFloatI(aCPSarr[i], 0);
            }

            // ----- Deviation -----
            CPS = dict[10303];
            CPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);
            aCPS = dict[10307];
            aCPSarr = CPS.Split(new string[] { ";" }, StringSplitOptions.None);

            if (trustedROI < CPSarr.Count())
            {
                val.Deviation = Conv.ToFloatI(CPSarr[trustedROI], 0);
                val.ActualDeviation = Conv.ToFloatI(aCPSarr[trustedROI], 0);
            }

            // ----- Measurement Timestamp -----
            val.timeStamp = Conv.ToInt(dict[10309], 0);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;

            // ----- Temperature -----
            try
            {
                if (dict[10014] == "")
                    val.Temperature = float.NaN;
                else
                    val.Temperature = Conv.ToFloatI(dict[10014], 0);
            }
            catch
            {
                val.Temperature = float.NaN;
            }

            // ----- Status -----
            val.Error = Conv.ToInt(dict[10100], 0);
            val.Status = Conv.ToInt(dict[10100], 0);

            NuviaEGM_Status NuStat = (NuviaEGM_Status)val.Status;
            if (NuStat.HasFlag(NuviaEGM_Status.NuEGM_STATUS_NOTRUN))
                val.isRunning = false;
            else
                val.isRunning = true;


            return val;
        }

        GeigerSettings nuviaGetEGMSettings()
        {
            egmSettings = new GeigerSettings();
            string param = nuvia.GetParam("27");
            Dictionary<int, string> dict = nuvia.ParseParam(param);

            egmSettings.MeasureTime = Conv.ToInt(dict[27], 1000);

            return egmSettings;
        }
    }
}
