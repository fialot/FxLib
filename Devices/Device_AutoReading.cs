using Fx.Components;
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
            doDevRequest();
        }

        protected abstract void refreshDevData();
        protected abstract void doDevRequest();
    }
}
