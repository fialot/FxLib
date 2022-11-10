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
        private GeigerValueEx requestGetEGMValue()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceEGMRequest.GetEGMValue;

            if (WaitForRequestDone())
                return GeigerValueEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns settings if read ok</returns>
        private GeigerSettingsEx requestGetEGMSettings()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceEGMRequest.GetEGMSettings;

            if (WaitForRequestDone())
                return GeigerSettingsEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns limits if read ok</returns>
        private GeigerLimitsEx requestGetEGMLimits()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceEGMRequest.GetEGMLimits;

            if (WaitForRequestDone())
                return GeigerLimitsEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestSetTime(int Time)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Time;
            request = (int)eDeviceEGMRequest.SetTime;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStart()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceEGMRequest.Start;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStop()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceEGMRequest.Stop;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestSetHV(int HV)
        {
            if (request > 0) return new IsBusyException();

            requestValue = HV;
            request = (int)eDeviceEGMRequest.SetHV;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// HV calibration - Set point
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="point">Point index</param>
        /// <param name="voltage">Real voltage</param>
        /// <returns>Returns true if ok</returns>
        private OkEx requestCalibHV_SetPoint(byte domain, byte point, float voltage)
        {
            if (request > 0) return new IsBusyException();

            requestValue = new CalibHVPoint(domain, point, voltage);
            request = (int)eDeviceEGMRequest.CalibHV_SetPoint;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <returns>Returns true if ok</returns>
        private OkEx requestCalibHV_Set(byte domain)
        {
            if (request > 0) return new IsBusyException();

            requestValue = domain;
            request = (int)eDeviceEGMRequest.CalibHV_Set;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }
    }
}
