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
            if (request > 0) return new IsBusyException();

            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.Start;
            else
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

            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.Stop;
            else
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
            if (request > 0) return new IsBusyException();

            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.Latch;
            else
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
            if (request > 0) return new IsBusyException();

            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.Clear;
            else
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
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceMCARequest.StartSpectrum;

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
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceMCARequest.StopSpectrum;

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
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceMCARequest.ClearSpectrum;

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
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceMCARequest.GetSpectrum;

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
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceMCARequest.GetMCACalibration;

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
            if (request > 0) return new IsBusyException();

            requestValue = On;
            request = (int)eDeviceMCARequest.SwitchHV;

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
            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.SetHV;
            else
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
            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.CalibHV_SetPoint;
            else
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
            if (Type == DeviceType.MCA)
                request = (int)eDeviceMCARequest.CalibHV_Set;
            else
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
        /// <returns>Returns true if read ok</returns>
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
        /// <returns>Returns true if read ok</returns>
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
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx requestSetTime(float Time)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Time;
            request = (int)eDeviceMCARequest.SetTime;

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
                        case eDeviceEGMRequest.Latch:
                            devLatch();
                            requestReply = true;
                            break;
                        case eDeviceEGMRequest.Clear:
                            devClear();
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
                        case eDeviceMCARequest.None:
                            break;
                        case eDeviceMCARequest.GetSCAValue:
                            requestReply = devGetMCAValue();
                            break;
                        case eDeviceMCARequest.GetSCASettings:
                            requestReply = devGetMCASettings();
                            break;
                        case eDeviceMCARequest.SetTime:
                            devSetTime((float)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.Start:
                            devStart();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.Stop:
                            devStop();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.Latch:
                            devLatch();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.Clear:
                            devClear();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.StartSpectrum:
                            devStartSpectrum();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.StopSpectrum:
                            devStopSpectrum();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.ClearSpectrum:
                            devClearSpectrum();
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.GetSpectrum:
                            requestReply = devGetSpectrum();
                            break;
                        case eDeviceMCARequest.GetMCACalibration:
                            requestReply = devGetCalibration();
                            break;
                        case eDeviceMCARequest.SwitchHV:
                            devSwitchHV((bool)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.SetHV:
                            devSetHV((int)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.CalibHV_SetPoint:
                            CalibHVPoint point = (CalibHVPoint)requestValue;
                            devSetCalibHVPoint(point.Domain, point.Point, point.HV);
                            requestReply = true;
                            break;
                        case eDeviceMCARequest.CalibHV_Set:
                            devSetCalibHV((byte)requestValue);
                            requestReply = true;
                            break;

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


        protected override void refreshDevData()
        {
            if (Type == DeviceType.MCA)
            {
                ReadSCAValue().Switch(
                    value =>
                    {
                        
                        if (value.isSpectRunning)
                        {
                            getSpectrum().Switch(
                                spectrum =>
                                {
                                    if (NewSpectrum != null)
                                    {
                                        Task.Run(() => NewSpectrum(this, spectrum));
                                    }

                                },
                                error => { }
                            );
                        }
                    },
                    error => { }
                );
            }
            else if (Type == DeviceType.General)
            {
                
                if (Support.HasFlag(DevSupport.Spectrum))
                {
                    getSpectrum().Switch(
                        spectrum =>
                        {
                            if (NewSpectrum != null)
                            {
                                Task.Run(() => NewSpectrum(this, spectrum));
                            }
                        },
                        error => { }
                    );
                }
            }
        }

    }
}
