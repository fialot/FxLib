using Fx.IO;
using Fx.IO.Exceptions;
using Fx.IO.Protocols;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        #region Constructor

        public DeviceNuvia()
        {
            nuvia = new NuviaProtocol(com);
            mb = new ModbusProtocolExt(com);

            DeviceName = "Nuvia";
            Type = DeviceType.General;
            //SupportFlag.SetFlag(DevSupport.Spectrum);
            Support &= ~DevSupport.Spectrum;
            Support |= DevSupport.File;
            Support |= DevSupport.Start;
            Support &= ~DevSupport.Firmware;
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
            if (Type == DeviceType.EGM)
                return getEGMErrorList(code);
            else
                return getMCAErrorList(code);
        }

        /// <summary>
        /// Get Status list from Status code
        /// </summary>
        /// <param name="code">Status Code</param>
        /// <returns>Status list</returns>
        public override string[] GetStatusList(int code)
        {
            if (Type == DeviceType.EGM)
                return getEGMStatusList(code);
            else
                return getMCAStatusList(code);
        }

        #endregion

        
        #region Measurement

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Start()
        {
            if (Start(out CommException Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Start(out CommException Error)
        {
            Error = null;
            try
            {
                start();
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
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Stop()
        {
            if (Stop(out CommException Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Stop(out CommException Error)
        {
            Error = null;
            try
            {
                stop();
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
        /// Latch measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool Latch()
        {
            return Latch(out CommException Error);
        }

        /// <summary>
        /// Latch measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Latch(out CommException Error)
        {
            Error = null;
            try
            {
                latch();
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
        /// Clear measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool Clear()
        {
            return Clear(out CommException Error);
        }

        /// <summary>
        /// Clear measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Clear(out CommException Error)
        {
            Error = null;
            try
            {
                clear();
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

        #region Spectrum

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool StartSpectrum()
        {
            return StartSpectrum(out CommException Error);
        }

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool StartSpectrum(out CommException Error)
        {
            Error = null;
            try
            {
                startSpectrum();
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
        /// Stop Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool StopSpectrum()
        {
            return StopSpectrum(out CommException Error);
        }

        /// <summary>
        /// Stop Spectrum measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool StopSpectrum(out CommException Error)
        {
            Error = null;
            try
            {
                stopSpectrum();
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
        /// Clear spectrum
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool ClearSpectrum()
        {
            return ClearSpectrum(out CommException Error);
        }

        /// <summary>
        /// Clear spectrum
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool ClearSpectrum(out CommException Error)
        {
            Error = null;
            try
            {
                clearSpectrum();
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
        /// Get Spectrum
        /// </summary>
        /// <param name="Spectrum">Spectrum</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetSpectrum(out Spectrum Spectrum)
        {
            CommException Error;
            return GetSpectrum(out Spectrum, out Error);
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <param name="Spectrum">Spectrum</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetSpectrum(out Spectrum spectrum, out CommException Error)
        {
            Error = null;
            spectrum = new Spectrum();
            try
            {
                spectrum = getSpectrum();
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
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <param name="Calib">Calibration Coefficients</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetCalibration(out MCACalibration Calib)
        {
            CommException Error;
            return GetCalibration(out Calib, out Error);
        }

        /// <summary>
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <param name="Calib">Calibration Coefficients</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetCalibration(out MCACalibration Calib, out CommException Error)
        {
            Error = null;
            Calib = new MCACalibration();
            try
            {
                Calib = getCalibration();
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


        #region HV 


        // ----- HV -----
        /// <summary>
        /// Switch HV On/Off
        /// </summary>
        /// <param name="On">Turn On/Off</param>
        /// <returns>Returns true if communication ok</returns>
        public OkEx SwitchHV(bool On)
        {
            if (SwitchHV(On, out CommException Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Switch HV On/Off
        /// </summary>
        /// <param name="On">Turn On/Off</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SwitchHV(bool On, out CommException Error)
        {
            Error = null;
            try
            {
                switchHV(On);
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
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        public OkEx SetHV(int HV)
        {
            if (SetHV(HV, out CommException Error))
                return true;
            else
                return Error;
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
        public OkEx CalibHV_SetPoint(byte domain, byte point, float voltage)
        {
            if (CalibHV_SetPoint(domain, point, voltage, out CommException Error))
                return true;
            else
                return Error;
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
        public OkEx CalibHV_Set(byte domain)
        {
            if (CalibHV_Set(domain, out CommException Error))
                return true;
            else
                return Error;
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


        #region EGM functions

        /// ----- EGM -----
        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <returns>Returns true if read ok</returns>
        public GeigerValueEx ReadValue()
        {
            GeigerValue Value;
            CommException Error;
            if (ReadValue(out Value, out Error))
                return Value;
            else
                return Error;
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
                Value = readEGMValue();
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
        /// <returns>Returns true if read ok</returns>
        public GeigerValueEx GetValue()
        {
            GeigerValue Value;
            CommException Error = new CommException();
            if (GetValue(out Value, out Error))
                return Value;
            else
                return Error;
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
                Value = getEGMValue();
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
        public GeigerSettingsEx GetSettings()
        {
            GeigerSettings Value;
            CommException Error;
            if (GetSettings(out Value, out Error))
                return Value;
            else
                return Error;
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
                Value = getEGMSettings();
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
        public GeigerLimitsEx GetLimits()
        {
            GeigerLimits Value;
            CommException Error;
            if (GetLimits(out Value, out Error))
                return Value;
            else
                return Error;
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
                Value = getEGMLimits();
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
        public OkEx SetTime(int Time)
        {
            if (SetTime(Time, out CommException Error))
                return true;
            else
                return Error;
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

        #region MCA functions

        /// <summary>
        /// Get SCA Settings
        /// </summary>
        /// <param name="Value">SCA settings values</param>
        /// <returns>Returns true if get ok></returns>
        public bool GetSettings(out SCASettings Value)
        {
            return GetSettings(out Value, out CommException Error);
        }

        /// <summary>
        /// Get SCA Settings
        /// </summary>
        /// <param name="Value">SCA settings values</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if get ok</returns>
        public bool GetSettings(out SCASettings Value, out CommException Error)
        {
            Error = null;
            Value = new SCASettings();
            try
            {
                Value = getMCASettings();
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
        /// Read last SCA Value
        /// </summary>
        /// <param name="Value">SCA Value</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadValue(out SCAValue Value)
        {
            return ReadValue(out Value, out CommException Error);
        }

        /// <summary>
        /// Read last SCA Value
        /// </summary>
        /// <param name="Value">SCA Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadValue(out SCAValue Value, out CommException Error)
        {
            Error = null;
            Value = new SCAValue();
            try
            {
                Value = readMCAValue();
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
        /// Get SCA value
        /// </summary>
        /// <param name="Value">SCA value</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetValue(out SCAValue Value)
        {
            return GetValue(out Value, out CommException Error);
        }

        /// <summary>
        /// Get SCA value
        /// </summary>
        /// <param name="Value">SCA value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetValue(out SCAValue Value, out CommException Error)
        {
            Error = null;
            Value = new SCAValue();
            try
            {
                Value = getMCAValue();
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
        public bool SetTime(float Time)
        {
            return SetTime(Time, out CommException Error);
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetTime(float Time, out CommException Error)
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
    }
}
