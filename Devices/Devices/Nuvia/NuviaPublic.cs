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
        /// <param name="Code">Status Code</param>
        /// <returns>Error list</returns>
        public override string[] GetErrorList(int Code)
        {
            if (Type == DeviceType.EGM)
                return getEGMErrorList(Code);
            else
                return getMCAErrorList(Code);
        }

        /// <summary>
        /// Get Status list from Status code
        /// </summary>
        /// <param name="Code">Status Code</param>
        /// <returns>Status list</returns>
        public override string[] GetStatusList(int Code)
        {
            if (Type == DeviceType.EGM)
                return getEGMStatusList(Code);
            else
                return getMCAStatusList(Code);
        }

        #endregion

        
        #region Measurement

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Start()
        {
            if (!RunningMeasurement)
                return start();
            else
                return requestStart();
        }

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Start(out CommException Error)
        {
            CommException error = null;
            var reply = Start().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Stop()
        {
            if (!RunningMeasurement)
                return stop();
            else
                return requestStop();
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Stop(out CommException Error)
        {
            CommException error = null;
            var reply = Stop().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Latch measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Latch()
        {
            if (!RunningMeasurement)
                return latch();
            else
                return requestLatch();
        }

        /// <summary>
        /// Latch measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Latch(out CommException Error)
        {
            CommException error = null;
            var reply = Latch().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Clear measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx Clear()
        {
            if (!RunningMeasurement)
                return clear();
            else
                return requestClear();
        }

        /// <summary>
        /// Clear measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool Clear(out CommException Error)
        {
            CommException error = null;
            var reply = Clear().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        #endregion

        #region Spectrum

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx StartSpectrum()
        {
            if (!RunningMeasurement)
                return startSpectrum();
            else
                return requestStartSpectrum();
        }

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool StartSpectrum(out CommException Error)
        {
            CommException error = null;
            var reply = StartSpectrum().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Stop Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx StopSpectrum()
        {
            if (!RunningMeasurement)
                return stopSpectrum();
            else
                return requestStopSpectrum();
        }

        /// <summary>
        /// Stop Spectrum measuring
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool StopSpectrum(out CommException Error)
        {
            CommException error = null;
            var reply = StopSpectrum().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Clear spectrum
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public OkEx ClearSpectrum()
        {
            if (!RunningMeasurement)
                return clearSpectrum();
            else
                return requestClearSpectrum();
        }

        /// <summary>
        /// Clear spectrum
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool ClearSpectrum(out CommException Error)
        {
            CommException error = null;
            var reply = ClearSpectrum().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <returns>Returns Spectrum if communication ok</returns>
        public SpectrumEx GetSpectrum()
        {
            if (!RunningMeasurement)
                return getSpectrum();
            else
                return requestGetSpectrum();
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <param name="Spectrum">Spectrum</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetSpectrum(out Spectrum spectrum, out CommException Error)
        {
            Spectrum spect = null;
            CommException error = null;
            var reply = GetSpectrum().Match(ok => { spect = ok; return true; }, err => { error = err; return false; });
            Error = error;
            spectrum = spect;
            return reply;
        }

        /// <summary>
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <returns>Returns Calibration if communication ok</returns>
        public MCACalibrationEx GetMCACalibration()
        {
            if (!RunningMeasurement)
                return getMCACalibration();
            else
                return requestGetMCACalibration();
        }

        /// <summary>
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <param name="Calib">Calibration Coefficients</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool GetMCACalibration(out MCACalibration Calib, out CommException Error)
        {
            MCACalibration calib = new MCACalibration();
            CommException error = null;
            var reply = GetMCACalibration().Match(ok => { calib = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Calib = calib;
            return reply;
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
            if (!RunningMeasurement)
                return switchHV(On);
            else
                return requestSwitchHV(On);
        }

        /// <summary>
        /// Switch HV On/Off
        /// </summary>
        /// <param name="On">Turn On/Off</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SwitchHV(bool On, out CommException Error)
        {
            CommException error = null;
            var reply = SwitchHV(On).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        public OkEx SetHV(int HV)
        {
            if (!RunningMeasurement)
                return setHV(HV);
            else
                return requestSetHV(HV);
        }

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetHV(int HV, out CommException Error)
        {
            CommException error = null;
            var reply = SetHV(HV).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
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
            if (!RunningMeasurement)
                return calibHV_SetPoint(domain, point, voltage);
            else
                return requestCalibHV_SetPoint(domain, point, voltage);
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
            CommException error = null;
            var reply = CalibHV_SetPoint(domain, point, voltage).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <returns>Returns true if ok</returns>
        public OkEx CalibHV_Set(byte domain)
        {
            if (!RunningMeasurement)
                return calibHV_Set(domain);
            else
                return requestCalibHV_Set(domain);
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="Error">Communication error</param>
        /// <returns>Returns true if ok</returns>
        public bool CalibHV_Set(byte domain, out CommException Error)
        {
            CommException error = null;
            var reply = CalibHV_Set(domain).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        #endregion


        #region EGM functions

        /// ----- EGM -----
        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        public GeigerValueEx ReadEGMValue()
        {
            try
            {
                return devReadEGMValue();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadEGMValue(out GeigerValue Value, out CommException Error)
        {
            GeigerValue value = new GeigerValue();
            CommException error = null;
            var reply = ReadEGMValue().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        public GeigerValueEx GetEGMValue()
        {
            if (!RunningMeasurement)
                return getEGMValue();
            else
                return requestGetEGMValue();
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <param name="Value">Geiger Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetEGMValue(out GeigerValue Value, out CommException Error)
        {
            GeigerValue value = new GeigerValue();
            CommException error = null;
            var reply = GetEGMValue().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }


        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        public GeigerSettingsEx GetEGMSettings()
        {
            if (!RunningMeasurement)
                return getEGMSettings();
            else
                return requestGetEGMSettings();
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <param name="Value">Geiger Settings</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetEGMSettings(out GeigerSettings Value, out CommException Error)
        {
            GeigerSettings value = new GeigerSettings();
            CommException error = null;
            var reply = GetEGMSettings().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }


        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        public GeigerLimitsEx GetEGMLimits()
        {
            if (!RunningMeasurement)
                return getEGMLimits();
            else
                return requestGetEGMLimits();
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <param name="Value">DR limits</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetEGMLimits(out GeigerLimits Value, out CommException Error)
        {
            GeigerLimits value = new GeigerLimits();
            CommException error = null;
            var reply = GetEGMLimits().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        public OkEx SetTime(int Time)
        {
            if (!RunningMeasurement)
                return setTime(Time);
            else
                return requestSetTime(Time);
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetTime(int Time, out CommException Error)
        {
            CommException error = null;
            var reply = SetTime(Time).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }


        #endregion

        #region MCA functions

        /// <summary>
        /// Get SCA Settings
        /// </summary>
        /// <param name="Value">SCA settings values</param>
        /// <returns>Returns true if get ok></returns>
        public SCASettingsEx GetSCASettings()
        {
            try
            {
                return devGetMCASettings();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get SCA Settings
        /// </summary>
        /// <param name="Value">SCA settings values</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if get ok</returns>
        public bool GetSCASettings(out SCASettings Value, out CommException Error)
        {
            SCASettings value = new SCASettings();
            CommException error = null;
            var reply = GetSCASettings().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }


        /// <summary>
        /// Read last SCA Value
        /// </summary>
        /// <param name="Value">SCA Value</param>
        /// <returns>Returns true if read ok</returns>
        public SCAValueEx ReadSCAValue()
        {
            try
            {
                return devReadMCAValue();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Read last SCA Value
        /// </summary>
        /// <param name="Value">SCA Value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool ReadSCAValue(out SCAValue Value, out CommException Error)
        {
            SCAValue value = new SCAValue();
            CommException error = null;
            var reply = ReadSCAValue().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }

        /// <summary>
        /// Get SCA value
        /// </summary>
        /// <param name="Value">SCA value</param>
        /// <returns>Returns true if read ok</returns>
        public SCAValueEx GetSCAValue()
        {
            try
            {
                return devGetMCAValue();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get SCA value
        /// </summary>
        /// <param name="Value">SCA value</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetSCAValue(out SCAValue Value, out CommException Error)
        {
            SCAValue value = new SCAValue();
            CommException error = null;
            var reply = GetSCAValue().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        public OkEx SetTime(float Time)
        {
            try
            {
                devSetTime(Time);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if communication ok</returns>
        public bool SetTime(float Time, out CommException Error)
        {
            CommException error = null;
            var reply = SetTime(Time).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        #endregion
    }
}
