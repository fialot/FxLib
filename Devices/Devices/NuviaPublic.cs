using Fx.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM
    {
        #region Constructor

        public DeviceNuvia()
        {
            DeviceName = "Nuvia";
            Type = DeviceType.General;
            SpectrumSupport = false;
            FileSupport = true;
            StartSupport = true;
            FirmwareSupport = false;
        }

        #endregion


        #region Status

        /// <summary>
        /// Get Error list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Error list</returns>
        public override string[] GetErrorList(int code)
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
        public override string[] GetStatusList(int code)
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

        #endregion


        #region EGM functions

        /// ----- EGM -----
        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadValue(out GeigerValue Value)
        {
            CommException Error;
            return ReadValue(out Value, out Error);
        }

        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadValue(out GeigerValue Value, out CommException Error)
        {
            Error = null;
            Value = new GeigerValue();
            try
            {
                Value = readValue();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetValue(out GeigerValue Value)
        {
            CommException Error = new CommException();
            return GetValue(out Value, out Error);
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetValue(out GeigerValue Value, out CommException Error)
        {
            Error = null;
            Value = new GeigerValue();
            try
            {
                Value = getValue();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }


        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <param name="Value">Geiger Settings</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetSettings(out GeigerSettings Value)
        {
            CommException Error;
            return GetSettings(out Value, out Error);
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <param name="Value">Geiger Settings</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetSettings(out GeigerSettings Value, out CommException Error)
        {
            Error = null;
            Value = new GeigerSettings();
            try
            {
                Value = getSettings();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }


        /// <summary>
        /// Get limits
        /// </summary>
        /// <param name="Value">DR limits</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetLimits(out GeigerLimits Value)
        {
            CommException Error;
            return GetLimits(out Value, out Error);
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <param name="Value">DR limits</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetLimits(out GeigerLimits Value, out CommException Error)
        {
            Error = null;
            Value = new GeigerLimits();
            try
            {
                Value = getLimits();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetTime(int Time)
        {
            return SetTime(Time, out CommException Error);
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetTime(int Time, out CommException Error)
        {
            Error = null;
            try
            {
                setTime(Time);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }


        #endregion


        #region Other Commands

        public bool Start()
        {
            try
            {
                prot.StartROIs();
                return true;
            }
            catch (CommException)
            {
                //Error = err;
            }
            catch (Exception)
            {
                //Error = new CommException(err.Message, err);
            }
            return false;
        }

        public bool Stop()
        {
            try
            {
                prot.StopROIs();
                return true;
            }
            catch (CommException)
            {
                //Error = err;
            }
            catch (Exception)
            {
                //Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion


        #region HV 


        // ----- HV -----

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetHV(int HV)
        {
            return SetHV(HV, out CommException Error);
        }

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetHV(int HV, out CommException Error)
        {
            Error = null;
            try
            {
                setHV(HV);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// HV calibration - Set point
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="point">Point index</param>
        /// <param name="voltage">Real voltage</param>
        /// <returns>Returns true if ok</returns>
        public bool CalibHV_SetPoint(byte domain, byte point, float voltage)
        {
            CommException Error;
            return CalibHV_SetPoint(domain, point, voltage, out Error);
        }

        /// <summary>
        /// HV calibration - Set point
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="point">Point index</param>
        /// <param name="voltage">Real voltage</param>
        /// <param name="Error">Communication error</param>
        /// <returns>Returns true if ok</returns>
        public bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error)
        {
            Error = null;
            try
            {
                setCalibHVPoint(domain, point, voltage);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <returns>Returns true if ok</returns>
        public bool CalibHV_Set(byte domain)
        {
            CommException Error;
            return CalibHV_Set(domain, out Error);
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="Error">Communication error</param>
        /// <returns>Returns true if ok</returns>
        public bool CalibHV_Set(byte domain, out CommException Error)
        {
            Error = null;
            try
            {
                setCalibHV(domain);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion


    }
}
