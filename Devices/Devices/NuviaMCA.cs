using Fx.Conversion;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    [Flags]
    public enum NuviaSCA_Status : uint
    {
        NuSCA_STATUS_NOTRUN1 = 0x00000001,      // ROI 1 not run
        NuSCA_STATUS_NOTRUN2 = 0x00000002,      // ROI 2 not run
        NuSCA_STATUS_NOTRUN3 = 0x00000004,      // ROI 3 not run
        NuSCA_STATUS_NOTRUN4 = 0x00000008,      // ROI 4 not run
        NuSCA_STATUS_OVERLOAD = 0x00000010,     // Overload
        NuSCA_STATUS_HV = 0x00000020,           // HV not up
        //NuSCA_ERROR_RTC = 0x00000040,         // RTC Error
        //NuSCA_ERROR_TEMP = 0x00000080,        // Temperature sensor Error
        NuSCA_STATUS_SD = 0x00000100,           // SD present
        NuSCA_STATUS_SPECTRUM = 0x00000200,     // Spectrum run
        //NuSCA_ERROR_DEVICE = 0x00000400,     // Device Error
        /*
        VF_STATUS_BSU2 = 0x00000800,    //Blokována SU2
        VF_STATUS_DSU1OK = 0x00001000,  //Překročení DSU1 1 ok. hodnoty
        VF_STATUS_DSU2OK = 0x00002000,  //Překročení DSU2 1 ok. hodnoty
        VF_STATUS_HSU1OK = 0x00004000,  //Překročení HSU1 1 ok. hodnoty
        VF_STATUS_HSU2OK = 0x00008000,  //Překročení HSU2 1 ok. hodnoty
        VF_STATUS_BACK = 0x00010000,    //Hodnota menší než pozadí*/
    }

    [Flags]
    public enum NuviaSCA_Error : uint
    {
        NuSCA_ERROR_RTC = 0x00000040,       // RTC Error
        NuSCA_ERROR_TEMP = 0x00000080,      // Temperature sensor Error
        NuSCA_ERROR_COMM = 0x00000400,      // Communication Error
        NuSCA_ERROR_DEVICE = 0x00000800     // Device Error
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

        MCACalibration Calib = new MCACalibration();

        #region Status

        /// <summary>
        /// Get Error list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Error list</returns>
        private string[] getMCAErrorList(int code)
        {
            List<string> list = new List<string>();
            NuviaSCA_Error NuErr = (NuviaSCA_Error)code;
            if (NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_RTC))
                list.Add(Lng("errRTC", "RTC Error"));
            if (NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_TEMP))
                list.Add(Lng("errTemp", "Temperature Error"));
            if (NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_COMM))
                list.Add(Lng("errComm", "Communication Error"));
            if (NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_DEVICE))
                list.Add(Lng("errDev", "Device Error"));

            if (list.Count == 0)
                list.Add(Lng("noErr", "Device working properly"));

            return list.ToArray();
        }

        /// <summary>
        /// Get Status list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Status list</returns>
        private string[] getMCAStatusList(int code)
        {

            List<string> list = new List<string>();
            NuviaSCA_Status NuStat = (NuviaSCA_Status)code;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN1))
                list.Add(Lng("ROI1NotRun", "ROI1 not running"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN2))
                list.Add(Lng("ROI2NotRun", "ROI2 not running"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN3))
                list.Add(Lng("ROI3NotRun", "ROI3 not running"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN4))
                list.Add(Lng("ROI4NotRun", "ROI4 not running"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_OVERLOAD))
                list.Add(Lng("overloading", "Overloading"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_HV))
                list.Add(Lng("noHV", "HV not up"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_SD))
                list.Add(Lng("isSD", "SD present"));
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_SPECTRUM))
                list.Add(Lng("specRun", "Spectrum running"));


            if (list.Count == 0)
            {
                NuviaSCA_Error NuErr = (NuviaSCA_Error)code;
                if (NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_RTC) || NuErr.HasFlag(NuviaSCA_Error.NuSCA_ERROR_TEMP))
                {
                    list.Add(Lng("error", "Error"));
                }
                else
                {
                    list.Add(Lng("standard", "Standard"));
                }
            }

            return list.ToArray();
        }

        #endregion

        #region Spectrum

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        protected void startSpectrum()
        {
            prot.StartSpectrum();
        }

        /// <summary>
        /// Stop Spectrum Measurement
        /// </summary>
        protected void stopSpectrum()
        {
            prot.StopSpectrum();
        }

        /// <summary>
        /// Clear Spectrum
        /// </summary>
        protected void clearSpectrum()
        {
            prot.ClearSpectrum();
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <returns>Returns Spectrum</returns>
        protected Spectrum getSpectrum()
        {
            Spectrum spectrum = prot.GetSpectrum();

            // ----- Set spectrum calibration -----
            spectrum.Energy = Calib.Energy;

            return spectrum;
        }

        #endregion

        #region HV
        /// <summary>
        /// Switch HV
        /// </summary>
        /// <param name="on">Turn On/Off</param>
        protected void switchHV(bool on)
        {
            prot.SwitchHV(on);
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Read last values
        /// </summary>
        /// <returns>SCA measurement</returns>
        protected SCAValue readMCAValue()
        {
            return GetMCAValFromDict(lastMeas);
        }

        /// <summary>
        /// Get SCA Value from device
        /// </summary>
        /// <returns>SCA measurement</returns>
        protected SCAValue getMCAValue()
        {
            string Values = prot.GetParam("9999");

            try
            {
                lastMeas = prot.ParseParam(Values);

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

        /// <summary>
        /// Get SCA measurement from parameter dictionary
        /// </summary>
        /// <param name="dict">Parameter dictionary</param>
        /// <returns>SCA measurement</returns>
        private SCAValue GetMCAValFromDict(Dictionary<int, string> dict)
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

        /// <summary>
        /// Get SCA settings
        /// </summary>
        /// <returns>SCA Settings</returns>
        protected SCASettings getMCASettings()
        {
            mcaSettings = new SCASettings();
            prot.SetParam("1=0");    // Select ROI1
            string param = prot.GetParam("1,3,4,5,6,13,14");
            Dictionary<int, string> dict = prot.ParseParam(param);

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

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time"></param>
        protected void setTime(float Time)
        {
            int ms = (int)(Time * 1000);
            int time = Conv.ToInt(prot.GetParam("27").Replace("27=", ""), 0);
            if (time != ms)
                prot.SetParam("27=" + ms.ToString());
        }

        /// <summary>
        /// Get MCA Calibration
        /// </summary>
        /// <returns></returns>
        protected MCACalibration getCalibration()
        {
            MCACalibration cal = new MCACalibration();
            cal.Energy.Type = EnergyCalibrationType.Polynomial;

            byte[] data = prot.GetUserData(0, 62);

            string header = Encoding.GetEncoding(1250).GetString(data, 0, 8);

            if (header == "GAMWIN  ")
            {
                // ----- Energy Calibration -----
                try
                {
                    cal.Energy.A = BitConverter.ToDouble(data, 8);
                }
                catch { }
                try
                {
                    cal.Energy.B = BitConverter.ToDouble(data, 16);
                }
                catch { }
                try
                {
                    cal.Energy.C = BitConverter.ToDouble(data, 24);
                }
                catch { }

                // ----- FWHM Calibration -----
                try
                {
                    cal.FWHM.A = BitConverter.ToDouble(data, 32);
                }
                catch { }
                try
                {
                    cal.FWHM.B = BitConverter.ToDouble(data, 40);
                }
                catch { }
                try
                {
                    cal.FWHM.C = BitConverter.ToDouble(data, 48);
                }
                catch { }

                cal.FWHM.Type = (FWHMCalibrationType)data[56];
            }

            Calib = cal;

            return cal;
        }

        #endregion

    }
}
