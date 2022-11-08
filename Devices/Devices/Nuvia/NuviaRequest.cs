using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStart()
        {
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
            request = (int)eDeviceEGMRequest.Stop;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Latch measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestLatch()
        {
            request = (int)eDeviceEGMRequest.Latch;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Clear measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestClear()
        {
            request = (int)eDeviceEGMRequest.Clear;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStartSpectrum()
        {
            request = (int)eDeviceMCARequest.StartSpec;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Stop Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestStopSpectrum()
        {
            request = (int)eDeviceMCARequest.StopSpec;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Clear spectrum
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestClearSpectrum()
        {
            //request = (int)eDeviceMCARequest.ClearSpec;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <returns>Returns Spectrum if communication ok</returns>
        private SpectrumEx requestGetSpectrum()
        {
            //request = (int)eDeviceMCARequest.GetSpectrum;

            if (WaitForRequestDone())
                return SpectrumEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <returns>Returns Calibration if communication ok</returns>
        private MCACalibrationEx requestGetMCACalibration()
        {
            //request = (int)eDeviceMCARequest.GetSpectrum;

            if (WaitForRequestDone())
                return MCACalibrationEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Switch HV On/Off
        /// </summary>
        /// <param name="On">Turn On/Off</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestSwitchHV(bool On)
        {
            //request = (int)eDeviceMCARequest.SwitchHV;

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
            request = (int)eDeviceEGMRequest.CalibHV_Set;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerValueEx requestGetEGMValue()
        {
            request = (int)eDeviceEGMRequest.GetEGMValue;

            if (WaitForRequestDone())
                return GeigerValueEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerSettingsEx requestGetEGMSettings()
        {
            request = (int)eDeviceEGMRequest.GetEGMSettings;

            if (WaitForRequestDone())
                return GeigerSettingsEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerLimitsEx requestGetEGMLimits()
        {
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
            request = (int)eDeviceEGMRequest.SetTime;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        protected override void doDevRequest()
        {
            try
            {

                if (Type == DeviceType.EGM)
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
                            devSetTime((float)(int)requestValue);
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
                        case eDeviceEGMRequest.Latch:
                            devLatch();
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
                else if (Type == DeviceType.MCA)
                {
                    var mcaRequest = (eDeviceMCARequest)request;

                    switch (mcaRequest)
                    {
                        case eDeviceMCARequest.SetTime:
                            requestReply = SetTime((float)requestValue); break;
                        case eDeviceMCARequest.Start:
                            requestReply = Start(); break;

                    }
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



    }
}
