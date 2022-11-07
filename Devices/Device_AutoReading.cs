using Fx.Components;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{

    public delegate void NewDataEventHandler(object source, List<DevMeasVals> newData);


    public abstract partial class Device : IDevice
    {


        public bool RunningMeasurement { get; private set; } = false;
        public int RefreshInterval { get; set; } = 1000;

        protected int request = 0;
        protected object requestValue = null;
        protected object requestReply = null;

        protected List<DevMeasVals> lastMeasValues = null;

        AbortableBackgroundWorker worker = new AbortableBackgroundWorker();

        
        public event NewDataEventHandler NewData = null;

        private bool startReading()
        {
            if (RunningMeasurement) return false;
            try
            {
                worker.RunWorkerAsync();                                       // Start Job
            }
            catch { return false; }
            
            return true;
        }

        private bool stopReading()
        {
            worker.CancelAsync();

            //if (worker.IsBusy) worker.Abort();
            return true;
        }

        private void WorkProcess(object sender, DoWorkEventArgs e)
        {
            RunningMeasurement = true;

            DateTime lastRefreshTime = DateTime.MinValue;
            DateTime nowTime = DateTime.Now;

            while (true)
            {
                // ----- Do request -----
                if (request > 0)
                {
                    doRequest();
                    request = 0;
                }

                // ----- Refresh device data -----
                if ((nowTime - lastRefreshTime).TotalMilliseconds > RefreshInterval)
                {
                    lastRefreshTime = nowTime;
                    try
                    {
                        if (checkConnection())
                            refreshData();
                    }
                    catch { }
                }
                
                // ----- Check cancel request -----
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                // ----- Sleep -----
                System.Threading.Thread.Sleep(10);
            }
        }

        private void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private bool checkConnection()
        {
            return IsConnected();
        }

        private void refreshData()
        {
            lastMeasValues = GetMeasurement().Match(
                values => {
                    refreshDevData();
                    if (NewData != null)
                        NewData(this, values);
                    return values;
                },
                error => { return lastMeasValues; }
            );
        }

        private void doRequest()
        {
            bool isGlobalDeviceRequest = false;
            eDeviceRequest devRequest = eDeviceRequest.None;
            try
            {
                devRequest = (eDeviceRequest)request;
                isGlobalDeviceRequest = true;
            }
            catch { }

            if (isGlobalDeviceRequest)
            {
                try
                {
                    switch (devRequest)
                    {
                        case eDeviceRequest.None:
                            break;
                        case eDeviceRequest.GetInfo:
                            requestReply = devGetInfo();
                            break;
                        case eDeviceRequest.GetXmlDescription:
                            requestReply = devGetXML();
                            GetSupport((string)requestReply);
                            break;
                        case eDeviceRequest.GetDescription:
                            requestReply = devGetDescription();
                            break;
                        case eDeviceRequest.GetMeasurement:
                            requestReply = devGetMeasurement();
                            break;
                        case eDeviceRequest.GetParameter:
                            requestReply = devGetParam((DevParamVals)requestValue);
                            break;
                        case eDeviceRequest.GetParameters:
                            requestReply = devGetParams((List<DevParamVals>)requestValue);
                            break;
                        case eDeviceRequest.GetAllParameters:
                            requestReply = devGetAllParams();
                            break;
                        case eDeviceRequest.SetParameter:
                            devSetParam((DevParamVals)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceRequest.SetParameters:
                            devSetParams((List<DevParamVals>)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceRequest.Login:
                            requestReply = devLogin((string)requestValue);
                            break;
                        case eDeviceRequest.Logout:
                            requestReply = devLogout();
                            break;
                        case eDeviceRequest.ChangePassword:
                            var reply = devChangePass((string)requestValue);

                            if (reply == eChangePassReply.BadLength)
                                requestReply = new BadLengthException();
                            else if (reply == eChangePassReply.NoPermissions)
                                requestReply = new NoPermissionException();
                            else
                                requestReply = true;
                            break;
                        case eDeviceRequest.GetDirectory:
                            requestReply = devGetDir();
                            break;
                        case eDeviceRequest.GetFile:
                            requestReply = devGetFile((string)requestValue);
                            break;
                        case eDeviceRequest.DeleteFile:
                            requestReply = devDelFile((string)requestValue);
                            break;
                        case eDeviceRequest.DeleteAllFiles:
                            requestReply = devDelAllFiles();
                            break;
                        case eDeviceRequest.GetConfig:
                            requestReply = devGetConfig();
                            break;
                        case eDeviceRequest.SetConfig:
                            devSetConfig((string)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceRequest.ResetConfig:
                            if (!devResetConfig())
                                requestReply = new NoPermissionException();
                            else
                                requestReply = true;
                            break;
                        case eDeviceRequest.CreateFactoryConfig:
                            if (!devCreateFactoryConfig())
                                requestReply = new NoPermissionException();
                            else
                                requestReply = true;
                            break;
                        case eDeviceRequest.UpdateFirmware:
                            devUpdateFirmware((string)requestValue);
                            requestReply = true;
                            break;
                        case eDeviceRequest.RunApplication:
                            devRunApp();
                            requestReply = true;
                            break;
                        case eDeviceRequest.RunBootloader:
                            devRunBootloader();
                            requestReply = true;
                            break;
                        case eDeviceRequest.StayInBootloader:
                            devStayInBootloader();
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
            else
            {
                doDevRequest();
            }
        }

        protected abstract void refreshDevData();
        protected abstract void doDevRequest();
    }
}
