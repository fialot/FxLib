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
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(0, true);
            }
            else
            {
                nuvia.StartSpectrum();
            }
        }

        /// <summary>
        /// Stop Spectrum Measurement
        /// </summary>
        protected void stopSpectrum()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(0, false);
            }
            else
            {
                nuvia.StopSpectrum();
            }
        }

        /// <summary>
        /// Clear Spectrum
        /// </summary>
        protected void clearSpectrum()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(1, true);
            }
            else
            {
                nuvia.ClearSpectrum();
            }
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <returns>Returns Spectrum</returns>
        protected Spectrum getSpectrum()
        {
            Spectrum spectrum;

            if (UsedProtocol == eProtocol.MODBUS)
            {
                spectrum = mbGetSpectrum();
            }
            else
            {
                spectrum = nuvia.GetSpectrum();
            }

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
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(2, on);
            }
            else
            {
                nuvia.SwitchHV(on);
            }
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
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetMCAValue();
            }
            else
            {
                return nuviaGetMCAValue();
            }
        }

        /// <summary>
        /// Get SCA measurement from parameter dictionary
        /// </summary>
        /// <param name="dict">Parameter dictionary</param>
        /// <returns>SCA measurement</returns>
        private SCAValue GetMCAValFromDict(Dictionary<int, string> dict)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetMCAValFromDict(dict);
            }
            else
            {
                return nuviaGetMCAValFromDict(dict);
            }
        }

        /// <summary>
        /// Get SCA settings
        /// </summary>
        /// <returns>SCA Settings</returns>
        protected SCASettings getMCASettings()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetMCASettings();
            }
            else
            {
                return nuviaGetMCASettings();
            }
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time"></param>
        protected void setTime(float Time)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                uint ms = (uint)(Time * 1000);
                uint time = mb.ReadUInt(54);
                if (time != ms)
                    mb.WriteUInt(54, ms);
            }
            else
            {
                int ms = (int)(Time * 1000);
                int time = Conv.ToInt(nuvia.GetParam("27").Replace("27=", ""), 0);
                if (time != ms)
                    nuvia.SetParam("27=" + ms.ToString());
            }
        }

        /// <summary>
        /// Get MCA Calibration
        /// </summary>
        /// <returns></returns>
        protected MCACalibration getCalibration()
        {
            MCACalibration cal = new MCACalibration();
            cal.Energy.Type = EnergyCalibrationType.Polynomial;
            byte[] data;


            if (UsedProtocol == eProtocol.MODBUS)
            {
                return cal;
            }
            else
            {
                data = nuvia.GetUserData(0, 62);
            }


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
