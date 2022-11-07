using Fx.IO.Exceptions;
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
        protected bool WaitForRequestDone()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (request != (int)eDeviceRequest.None)
            {
                if (stopwatch.Elapsed.TotalSeconds > requestTimeout)
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
            request = (int)eDeviceRequest.GetInfo;

            if (WaitForRequestDone())
                return (DeviceInfoEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <returns>Returns XML description</returns>
        private StringEx requestGetXMLDesc()
        {
            request = (int)eDeviceRequest.GetXmlDescription;

            if (WaitForRequestDone())
                return (StringEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get description
        /// </summary>
        /// <returns>Returns device description</returns>
        private DevParamsEx requestGetDescription()
        {
            request = (int)eDeviceRequest.GetDescription;

            if (WaitForRequestDone())
                return (DevParamsEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <returns>Returns Measurement</returns>
        private DevMeasValsEx requestGetMeasurement()
        {
            request = (int)eDeviceRequest.GetMeasurement;

            if (WaitForRequestDone())
                return (DevMeasValsEx)requestReply;
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
            requestValue = Param;
            request = (int)eDeviceRequest.GetParameter;

            if (WaitForRequestDone())
                return (DevParamValueEx)requestReply;
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
            requestValue = Param;
            request = (int)eDeviceRequest.GetParameters;

            if (WaitForRequestDone())
                return (DevParamValuesEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <returns>Returns parameter list</returns>
        private DevParamsEx requestGetAllParams()
        {
            request = (int)eDeviceRequest.GetAllParameters;

            if (WaitForRequestDone())
                return (DevParamsEx)requestReply;
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
            requestValue = Param;
            request = (int)eDeviceRequest.SetParameter;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = Param;
            request = (int)eDeviceRequest.SetParameters;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = password;
            request = (int)eDeviceRequest.Login;

            if (WaitForRequestDone())
                return (PermissionEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Logout function
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private PermissionEx requestLogout()
        {
            request = (int)eDeviceRequest.Logout;

            if (WaitForRequestDone())
                return (PermissionEx)requestReply;
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
            requestValue = password;
            request = (int)eDeviceRequest.ChangePassword;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <returns>Returns file list if read ok</returns>
        private StringArrayEx requestGetDir()
        {
            request = (int)eDeviceRequest.GetDirectory;

            if (WaitForRequestDone())
                return (StringArrayEx)requestReply;
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
            requestValue = FileName;
            request = (int)eDeviceRequest.GetFile;

            if (WaitForRequestDone())
                return (StringEx)requestReply;
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
            requestValue = FileName;
            request = (int)eDeviceRequest.DeleteFile;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestDelAllFiles()
        {
            request = (int)eDeviceRequest.DeleteAllFiles;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Get configuration XML
        /// </summary>
        /// <returns></returns>
        private StringEx requestGetConfig()
        {
            request = (int)eDeviceRequest.GetConfig;

            if (WaitForRequestDone())
                return (StringEx)requestReply;
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
            requestValue = FileName;
            request = (int)eDeviceRequest.SetConfig;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Reset configuration to Facroty settings
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx requestResetConfig()
        {
            request = (int)eDeviceRequest.ResetConfig;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Create factory configuration
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx requestCreateFactoryConfig()
        {
            request = (int)eDeviceRequest.CreateFactoryConfig;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
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
            requestValue = FileName;
            request = (int)eDeviceRequest.UpdateFirmware;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestRunApplication()
        {
            request = (int)eDeviceRequest.RunApplication;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestRunBootloader()
        {
            request = (int)eDeviceRequest.RunBootloader;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx requestStayInBootloader()
        {
            request = (int)eDeviceRequest.StayInBootloader;

            if (WaitForRequestDone())
                return (OkEx)requestReply;
            else
                return new TimeOutException();
        }
    }
}
