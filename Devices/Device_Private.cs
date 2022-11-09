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
        string logTitle = "";
        int logProgress = 0;

        protected void setLogTitle(string title)
        {
            logTitle = title;
        }

        protected void log(string text, int progress = -1)
        {
            if (progress >= 0)
                logProgress = progress;

            if (NewLog != null)
            {
                //NewLog(this, logTitle, text, logProgress);
                try
                {
                    Task.Run(() => NewLog(this, logTitle, text, logProgress));
                }
                catch { }
            }
        }

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private DeviceInfoEx getInfo()
        {
            try
            {
                return devGetInfo();
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
        /// Get device XML description
        /// </summary>
        /// <returns>Returns XML description</returns>
        private StringEx getXMLDesc()
        {
            string XML = "";
            try
            {
                XML = devGetXML();
                GetSupport(XML);
                return XML;
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
        /// Get description
        /// </summary>
        /// <returns>Returns device description</returns>
        private DevParamsEx getDescription()
        {
            try
            {
                return devGetDescription();
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
        /// Get measurement
        /// </summary>
        /// <returns>Returns Measurement</returns>
        private DevMeasValsEx getMeasurement()
        {
            try
            {
                return devGetMeasurement();
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
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Returns Device parameter values</returns>
        private DevParamValueEx getParam(DevParamVals Param)
        {
            try
            {
                return devGetParam(Param);
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
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <returns>Returns parameter list</returns>
        private DevParamValuesEx getParams(List<DevParamVals> Param)
        {
            try
            {
                return devGetParams(Param);
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
        /// Get all device parameters
        /// </summary>
        /// <returns>Returns parameter list</returns>
        private DevParamsEx getAllParams()
        {
            try
            {
                return devGetAllParams();
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
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Return true if ok</returns>
        private OkEx setParam(DevParamVals Param)
        {
            try
            {
                devSetParam(Param);
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
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <returns>Returns true if write ok</returns>
        private OkEx setParams(List<DevParamVals> Param)
        {
            try
            {
                devSetParams(Param);
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
        /// Login function
        /// </summary>
        /// <param name="password">Passowrd</param>
        /// <returns>Return actual permissions</returns>
        private PermissionEx login(string password)
        {
            try
            {
                return devLogin(password);
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
        /// Logout function
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private PermissionEx logout()
        {
            try
            {
                return devLogout();
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
        /// Change password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>Return true if ok</returns>
        private OkEx changePassword(string password)
        {
            try
            {
                var reply = devChangePass(password);

                if (reply == eChangePassReply.BadLength)
                    return new BadLengthException();
                else if (reply == eChangePassReply.NoPermissions)
                    return new NoPermissionException();
                else
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
        /// Get directory (file list)
        /// </summary>
        /// <returns>Returns file list if read ok</returns>
        private StringArrayEx getDir()
        {
            try
            {
                return devGetDir();
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
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns file if ok</returns>
        private StringEx getFile(string FileName)
        {
            try
            {
                return devGetFile(FileName);
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
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        private OkEx delFile(string FileName)
        {
            try
            {
                return devDelFile(FileName);
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
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx delAllFiles()
        {
            try
            {
                return devDelAllFiles();
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
        /// Get configuration XML
        /// </summary>
        /// <returns></returns>
        private StringEx getConfig()
        {
            try
            {
                return devGetConfig();
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
        /// Set configuration XML
        /// </summary>
        /// <param name="FileName">Path to configuration XML file</param>
        /// <returns>Return true if OK</returns>
        private OkEx setConfig(string FileName)
        {
            try
            {
                devSetConfig(FileName);
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
        /// Reset configuration to Facroty settings
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx resetConfig()
        {
            try
            {
                if (!devResetConfig())
                    return new NoPermissionException();
                else
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
        /// Create factory configuration
        /// </summary>
        /// <returns>Return true if OK</returns>
        private OkEx createFactoryConfig()
        {
            try
            {
                if (!devCreateFactoryConfig())
                    return new NoPermissionException();
                else
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
        /// Update firmware
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        private OkEx updateFirmware(string FileName)
        {
            try
            {
                devUpdateFirmware(FileName);
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
        /// Run Application
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx runApplication()
        {
            try
            {
                devRunApp();
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
        /// Run Bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx runBootloader()
        {
            try
            {
                devRunBootloader();
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
        /// Stay in bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        private OkEx stayInBootloader()
        {
            try
            {
                devStayInBootloader();
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
