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
        /// Get Geiger Value
        /// </summary>
        /// <returns>Returns EGM value if read ok</returns>
        private GeigerValueEx getEGMValue()
        {
            try
            {
                return devGetEGMValue();
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
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns settings if read ok</returns>
        private GeigerSettingsEx getEGMSettings()
        {
            try
            {
                return devGetEGMSettings();
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
        /// Get limits
        /// </summary>
        /// <returns>Returns limits if read ok</returns>
        private GeigerLimitsEx getEGMLimits()
        {
            try
            {
                return devGetEGMLimits();
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
        /// <returns>Returns true if communication ok</returns>
        private OkEx setTime(int Time)
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
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx start()
        {
            try
            {
                devStart();
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
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx stop()
        {
            try
            {
                devStop();
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
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx setHV(int HV)
        {
            try
            {
                devSetHV(HV);
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
        /// HV calibration - Set point
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="point">Point index</param>
        /// <param name="voltage">Real voltage</param>
        /// <returns>Returns true if ok</returns>
        private OkEx calibHV_SetPoint(byte domain, byte point, float voltage)
        {
            try
            {
                devSetCalibHVPoint(domain, point, voltage);
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
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <returns>Returns true if ok</returns>
        private OkEx calibHV_Set(byte domain)
        {
            try
            {
                devSetCalibHV(domain);
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
    }
}
