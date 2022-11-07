using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public abstract partial class DeviceEGM : Device, IDeviceEGM
    {
        /// <summary>
        /// Read Geiger Value from last Measurement
        /// </summary>
        /// <returns>Returns EGM value</returns>
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
            CommException error = null;
            GeigerValue value = new GeigerValue();
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
            CommException error = null;
            GeigerValue value = new GeigerValue();
            var reply = GetEGMValue().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }


        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns settings if read ok</returns>
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
            CommException error = null;
            GeigerSettings value = new GeigerSettings();
            var reply = GetEGMSettings().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }


        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns limits if read ok</returns>
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
            CommException error = null;
            GeigerLimits value = new GeigerLimits();
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


        #endregion

        #region HV 


        // ----- HV -----


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

        #region Auto Measurement
        protected override void refreshDevData() { }

        protected override void doDevRequest()
        {
            try
            {
                switch ((eDeviceEGMRequest)request)
                {
                    case eDeviceEGMRequest.None:
                        break;
                    case eDeviceEGMRequest.GetEGMValue:
                        requestReply = devGetEGMValue();
                        break;
                    case eDeviceEGMRequest.GetEGMSettings:
                        requestReply = devGetEGMSettings();
                        break;
                    case eDeviceEGMRequest.GetEGMLimits:
                        requestReply = devGetEGMLimits();
                        break;
                    case eDeviceEGMRequest.SetTime:
                        devSetTime((int)requestValue);
                        requestReply = true;
                        break;
                    case eDeviceEGMRequest.Start:
                        devStart();
                        requestReply = true;
                        break;
                    case eDeviceEGMRequest.Stop:
                        devStop();
                        requestReply = true;
                        break;
                    case eDeviceEGMRequest.SetHV:
                        devSetHV((int)requestValue);
                        requestReply = true;
                        break;
                    case eDeviceEGMRequest.CalibHV_SetPoint:
                        CalibHVPoint point = (CalibHVPoint)requestValue;
                        devSetCalibHVPoint(point.Domain, point.Point, point.HV);
                        requestReply = true;
                        break;
                    case eDeviceEGMRequest.CalibHV_Set:
                        devSetCalibHV((byte)requestValue);
                        requestReply = true;
                        break;

                }
            }
            catch (CommException err)
            {
                requestReply = err;
            }
            catch (Exception err)
            {
                requestReply = new CommException(err.Message, err);
            }
        }

        #endregion

        protected abstract GeigerValue devReadEGMValue();
        protected abstract GeigerValue devGetEGMValue();
        protected abstract GeigerSettings devGetEGMSettings();
        protected abstract GeigerLimits devGetEGMLimits();
        protected abstract void devSetTime(int time);

        protected abstract void devStart();
        protected abstract void devStop();

        protected abstract void devSetHV(int HV);

        protected abstract void devSetCalibHVPoint(byte domain, byte point, float voltage);
        protected abstract void devSetCalibHV(byte domain);

    }
}
