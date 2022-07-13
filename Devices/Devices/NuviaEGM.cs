using Fx.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    [Flags]
    public enum NuviaEGM_Status : uint
    {
        NuEGM_STATUS_NOTRUN = 0x00000001,      // Measurement not run
        NuEGM_STATUS_OVERLOAD = 0x00000002, // CPS Overload
        //NuEGM_STATUS_HV = 0x00000004,       // HV Error
        /*VF_STATUS_KAL = 0x00000008,     //Kalibrace (blenkr) 
        VF_STATUS_DRM = 0x00000010,     //Hodnota < DRM
        VF_STATUS_HRM = 0x00000020,     //Hodnota > HRM
        VF_STATUS_DSU1 = 0x00000040,    //Překročení DSU1
        VF_STATUS_DSU2 = 0x00000080,    //Překročení DSU2
        VF_STATUS_HSU1 = 0x00000100,    //Překročení HSU1
        VF_STATUS_HSU2 = 0x00000200,    //Překročení HSU2
        VF_STATUS_BSU1 = 0x00000400,    //Blokována SU1
        VF_STATUS_BSU2 = 0x00000800,    //Blokována SU2
        VF_STATUS_DSU1OK = 0x00001000,  //Překročení DSU1 1 ok. hodnoty
        VF_STATUS_DSU2OK = 0x00002000,  //Překročení DSU2 1 ok. hodnoty
        VF_STATUS_HSU1OK = 0x00004000,  //Překročení HSU1 1 ok. hodnoty
        VF_STATUS_HSU2OK = 0x00008000,  //Překročení HSU2 1 ok. hodnoty
        VF_STATUS_BACK = 0x00010000,    //Hodnota menší než pozadí*/
    }

    [Flags]
    public enum NuviaEGM_Error : uint
    {
        NuEGM_ERROR_HV = 0x00000004,   // HV Error
        NuEGM_ERROR_DET1 = 0x00000008,   // Detector 1 Error
        NuEGM_ERROR_DET2 = 0x00000010,   // Detector 2 Error
        NuEGM_ERROR_DET3 = 0x00000020,   // Detector 3 Error
        NuEGM_ERROR_DET4 = 0x00000040,   // Detector 4 Error
        NuEGM_ERROR_TEMP = 0x00000080,   // Detector 4 Error
        NuEGM_ERROR_DEVICE = 0x00000400
        /*VF_ERROR_NONE = 0,              //Bez chyby
        VF_ERROR_R10000 = 10000,        //Rezerva
        VF_ERROR_INITI2C = 10001,       //Chyba inicializace I2C
        VF_ERROR_INITRTC = 10002,       //Chyba inicializace RTC
        VF_ERROR_CONFIG = 10003,        //Chyba při načítání konfigurace
        VF_ERROR_CALTAB = 10004,        //Chyba při načítání kalibrace
        VF_ERROR_TOUTIPMJK = 10005,     //Timeout impulsu jemného kanálu
        VF_ERROR_TOUTIPMHK = 10006,     //Timeout impulsu hrubého kanálu
        VF_ERROR_VREF = 10007,          //Chyba referenčního napětí
        VF_ERROR_LOTEMP = 10008,        //Překročena minimální pracovní teplota
        VF_ERROR_HITEMP = 10009,        //Překročena maximální pracovní teplota*/
    }

    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {


        /// <summary>
        /// Get Error list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Error list</returns>
        private string[] getEGMErrorList(int code)
        {
            List<string> list = new List<string>();
            NuviaEGM_Error NuErr = (NuviaEGM_Error)code;
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_HV))
                list.Add(Lng("errHV", "HV Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_DET1))
                list.Add(Lng("errDet1", "Detector 1 Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_DET2))
                list.Add(Lng("errDet2", "Detector 2 Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_DET3))
                list.Add(Lng("errDet3", "Detector 3 Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_DET4))
                list.Add(Lng("errDet4", "Detector 4 Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_TEMP))
                list.Add(Lng("errTemp", "Temperature sensor Error"));
            if (NuErr.HasFlag(NuviaEGM_Error.NuEGM_ERROR_DEVICE))
                list.Add(Lng("errDev", "Device Error"));

            /*if (list.Count == 0)
                list.Add(Lng("noErr", "Device working properly"));*/

            return list.ToArray();
        }

        /// <summary>
        /// Get Status list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Status list</returns>
        private string[] getEGMStatusList(int code)
        {

            List<string> list = new List<string>();
            NuviaEGM_Status NuStat = (NuviaEGM_Status)code;
            if (NuStat.HasFlag(NuviaEGM_Status.NuEGM_STATUS_NOTRUN))
                list.Add(Lng("measNotRun", "Measurement not running"));
            if (NuStat.HasFlag(NuviaEGM_Status.NuEGM_STATUS_OVERLOAD))
                list.Add(Lng("overloading", "Overloading"));

            if (list.Count == 0)
            {
                if (GetErrorList(code).Length == 0)
                    list.Add(Lng("standard", "Standard"));
                else
                    list.Add(Lng("error", "Error"));
            }


            return list.ToArray();
        }



        #region Measurement functions



        /// <summary>
        /// Read last values
        /// </summary>
        /// <returns></returns>
        protected GeigerValue readEGMValue()
        {
            return GetEGMValFromDict(lastMeas);
        }

        /// <summary>
        /// Get Geiger Value from device
        /// </summary>
        /// <returns>Geiger measurement</returns>
        protected GeigerValue getEGMValue()
        {
            string Values = prot.GetParam("9999");

            try
            {
                lastMeas = prot.ParseParam(Values);
                return GetEGMValFromDict(lastMeas);
            }
            catch
            {
                throw new Exception("Data Parsing Error");
            }
        }

        /// <summary>
        /// Get EGM measurement from parameter dictionary
        /// </summary>
        /// <param name="dict">Parameter dictionary</param>
        /// <returns>EGM measurement</returns>
        private GeigerValue GetEGMValFromDict(Dictionary<int, string> dict)
        {
            GeigerValue val = new GeigerValue();

            // ----- Permissions -----
            try
            {
                var perm = prot.ParseParam(prot.GetParam("10105"))[10105];    // get Model

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

            val.Deviation = Conv.ToFloatI(CPSarr[trustedROI], 0);
            val.ActualDeviation = Conv.ToFloatI(aCPSarr[trustedROI], 0);

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

        #endregion


        #region Info & Settings



        /// <summary>
        /// Get EGM settings
        /// </summary>
        /// <returns>EGM Settings</returns>
        protected GeigerSettings getEGMSettings()
        {
            egmSettings = new GeigerSettings();
            string param = prot.GetParam("27");
            Dictionary<int, string> dict = prot.ParseParam(param);

            egmSettings.MeasureTime = Conv.ToInt(dict[27], 1000);

            return egmSettings;
        }

        /// <summary>
        /// Dose Rate Limits
        /// </summary>
        /// <returns>Limits</returns>
        protected GeigerLimits getEGMLimits()
        {
            egmLimits = new GeigerLimits();
            string param = prot.GetParam("54,55,56");
            Dictionary<int, string> dict = prot.ParseParam(param);

            egmLimits.Low1 = Conv.ToInt(dict[55], 0);
            egmLimits.Low2 = Conv.ToInt(dict[55], 0);
            egmLimits.High1 = Conv.ToInt(dict[56], 0);
            egmLimits.High2 = Conv.ToInt(dict[56], 0);

            return egmLimits;
        }





        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time"></param>
        protected void setTime(int Time)
        {
            int ms = (Time * 1000);
            int time = Conv.ToInt(prot.GetParam("27").Replace("27=", ""), 0);
            if (time != ms)
                prot.SetParam("27=" + ms.ToString());
        }

        #endregion

    }
}
