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
        protected GeigerValue devReadEGMValue()
        {
            return GetEGMValFromDict(lastMeas);
        }

        /// <summary>
        /// Get Geiger Value from device
        /// </summary>
        /// <returns>Geiger measurement</returns>
        protected GeigerValue devGetEGMValue()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetEGMValue();
            }
            else
            {
                return nuviaGetEGMValue();
            }
        }

        /// <summary>
        /// Get EGM measurement from parameter dictionary
        /// </summary>
        /// <param name="dict">Parameter dictionary</param>
        /// <returns>EGM measurement</returns>
        private GeigerValue GetEGMValFromDict(Dictionary<int, string> dict)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetEGMValFromDict(dict);
            }
            else
            {
                return nuviaGetEGMValFromDict(dict);
            }
        }

        #endregion


        #region Info & Settings



        /// <summary>
        /// Get EGM settings
        /// </summary>
        /// <returns>EGM Settings</returns>
        protected GeigerSettings devGetEGMSettings()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetEGMSettings();
            }
            else
            {
                return nuviaGetEGMSettings();
            }
        }

        /// <summary>
        /// Dose Rate Limits
        /// </summary>
        /// <returns>Limits</returns>
        protected GeigerLimits devGetEGMLimits()
        {
            egmLimits = new GeigerLimits();

            if (UsedProtocol == eProtocol.MODBUS)
            {
                return egmLimits;
            }
            else
            {
                
                string param = nuvia.GetParam("54,55,56");
                Dictionary<int, string> dict = nuvia.ParseParam(param);

                egmLimits.Low1 = Conv.ToInt(dict[55], 0);
                egmLimits.Low2 = Conv.ToInt(dict[55], 0);
                egmLimits.High1 = Conv.ToInt(dict[56], 0);
                egmLimits.High2 = Conv.ToInt(dict[56], 0);

                return egmLimits;
            }
        }





        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time"></param>
        protected void devSetTime(int Time)
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

                int ms = (Time * 1000);
                int time = Conv.ToInt(nuvia.GetParam("27").Replace("27=", ""), 0);
                if (time != ms)
                    nuvia.SetParam("27=" + ms.ToString());
            }
        }

        #endregion

    }
}
