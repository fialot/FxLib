using Fx.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public abstract class DeviceEGM : Device, IDeviceEGM
    {
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

        #region Measurement 


        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        public bool Start()
        {
            return Start(out CommException Error);
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
        public bool Stop()
        {
            return Stop(out CommException Error);
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


        protected abstract GeigerValue readEGMValue();
        protected abstract GeigerValue getEGMValue();
        protected abstract GeigerSettings getEGMSettings();
        protected abstract GeigerLimits getEGMLimits();
        protected abstract void setTime(int time);

        protected abstract void start();
        protected abstract void stop();

        protected abstract void setHV(int HV);

        protected abstract void setCalibHVPoint(byte domain, byte point, float voltage);
        protected abstract void setCalibHV(byte domain);

    }
}
