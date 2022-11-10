using Fx.IO.Exceptions;
using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public abstract partial class Device : IDevice
    {
        const int requestSleep = 50;
        const int requestTimeout = 10;

        /// <summary>
        /// Wait until the request is processed
        /// </summary>
        /// <returns></returns>
        protected bool WaitForRequestDone(int timeout = requestTimeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (request != (int)eDeviceRequest.None)
            {
                if (stopwatch.Elapsed.TotalSeconds > timeout)
                    return false;

                System.Threading.Thread.Sleep(requestSleep);
            }

            return true;
        }

        /// <summary>
        /// Request Get Info
        /// </summary>
        /// <returns>Device info</returns>
        private DeviceInfoEx requestGetInfo()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetInfo;

            if (WaitForRequestDone())
                return DeviceInfoEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <returns>Returns XML description</returns>
        private StringEx requestGetXMLDesc()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetXmlDescription;

            if (WaitForRequestDone())
                return StringEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get description
        /// </summary>
        /// <returns>Returns device description</returns>
        private DevParamsEx requestGetDescription()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetDescription;

            if (WaitForRequestDone())
                return DevParamsEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <returns>Returns Measurement</returns>
        private DevMeasValsEx requestGetMeasurement()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetMeasurement;

            if (WaitForRequestDone())
                return DevMeasValsEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Returns Device parameter values</returns>
        private DevParamValueEx requestGetParam(DevParamVals Param)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Param;
            request = (int)eDeviceRequest.GetParameter;

            if (WaitForRequestDone())
                return DevParamValueEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <returns>Returns parameter list</returns>
        private DevParamValuesEx requestGetParams(List<DevParamVals> Param)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Param;
            request = (int)eDeviceRequest.GetParameters;

            if (WaitForRequestDone())
                return DevParamValuesEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <returns>Returns parameter list</returns>
        private DevParamsEx requestGetAllParams()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetAllParameters;

            if (WaitForRequestDone())
            {
                return DevParamsEx.Convert(requestReply);
            }
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Return true if ok</returns>
        private OkEx requestSetParam(DevParamVals Param)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Param;
            request = (int)eDeviceRequest.SetParameter;

            if (WaitForRequestDone())
            {
                return OkEx.Convert(requestReply);
            }
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <returns>Returns true if write ok</returns>
        private OkEx requestSetParams(List<DevParamVals> Param)
        {
            if (request > 0) return new IsBusyException();

            requestValue = Param;
            request = (int)eDeviceRequest.SetParameters;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Login function
        /// </summary>
        /// <param name="password">Passowrd</param>
        /// <returns>Return actual permissions</returns>
        private PermissionEx requestLogin(string password)
        {
            if (request > 0) return new IsBusyException();

            requestValue = password;
            request = (int)eDeviceRequest.Login;

            if (WaitForRequestDone())
                return PermissionEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Logout function
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private PermissionEx requestLogout()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.Logout;

            if (WaitForRequestDone())
                return PermissionEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>Return true if ok</returns>
        private OkEx requestChangePassword(string password)
        {
            if (request > 0) return new IsBusyException();

            requestValue = password;
            request = (int)eDeviceRequest.ChangePassword;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <returns>Returns file list if read ok</returns>
        private StringArrayEx requestGetDir()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetDirectory;

            if (WaitForRequestDone())
                return StringArrayEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns file if ok</returns>
        private StringEx requestGetFile(string FileName)
        {
            if (request > 0) return new IsBusyException();

            requestValue = FileName;
            request = (int)eDeviceRequest.GetFile;

            if (WaitForRequestDone())
                return StringEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        private OkEx requestDelFile(string FileName)
        {
            if (request > 0) return new IsBusyException();

            requestValue = FileName;
            request = (int)eDeviceRequest.DeleteFile;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestDelAllFiles()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.DeleteAllFiles;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get configuration XML
        /// </summary>
        /// <returns></returns>
        private StringEx requestGetConfig()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.GetConfig;

            if (WaitForRequestDone())
                return StringEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Set configuration XML
        /// </summary>
        /// <param name="FileName">Path to configuration XML file</param>
        /// <returns>Return true if OK</returns>
        private OkEx requestSetConfig(string FileName)
        {
            if (request > 0) return new IsBusyException();

            requestValue = FileName;
            request = (int)eDeviceRequest.SetConfig;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Reset configuration to Facroty settings
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx requestResetConfig()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.ResetConfig;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Create factory configuration
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx requestCreateFactoryConfig()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.CreateFactoryConfig;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        private OkEx requestUpdateFirmware(string FileName)
        {
            if (request > 0) return new IsBusyException();

            requestValue = FileName;
            request = (int)eDeviceRequest.UpdateFirmware;

            if (WaitForRequestDone(5000))
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestRunApplication()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.RunApplication;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestRunBootloader()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.RunBootloader;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestStayInBootloader()
        {
            if (request > 0) return new IsBusyException();

            request = (int)eDeviceRequest.StayInBootloader;

            if (WaitForRequestDone())
                return OkEx.Convert(requestReply);
            else
                return new TimeOutException();
        }
    }
}
