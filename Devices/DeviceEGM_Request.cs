﻿using Fx.IO.Exceptions;
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
            request = (int)eDeviceEGMRequest.GetEGMValue;

            if (WaitForRequestDone())
                return (GeigerValueEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns settings if read ok</returns>
        private GeigerSettingsEx requestGetEGMSettings()
        {
            request = (int)eDeviceEGMRequest.GetEGMSettings;

            if (WaitForRequestDone())
                return (GeigerSettingsEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns limits if read ok</returns>
        private GeigerLimitsEx requestGetEGMLimits()
        {
            request = (int)eDeviceEGMRequest.GetEGMLimits;

            if (WaitForRequestDone())
                return (GeigerLimitsEx)requestReply;
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
            requestValue = Time;
            request = (int)eDeviceEGMRequest.SetTime;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStart()
        {
            request = (int)eDeviceEGMRequest.Start;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStop()
        {
            request = (int)eDeviceEGMRequest.Stop;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = HV;
            request = (int)eDeviceEGMRequest.SetHV;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = new CalibHVPoint(domain, point, voltage);
            request = (int)eDeviceEGMRequest.CalibHV_SetPoint;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = domain;
            request = (int)eDeviceEGMRequest.CalibHV_Set;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }
    }
}