﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace Fx.Components
{
    public class AbortableBackgroundWorker : BackgroundWorker
    {
        private Thread workerThread;

        public AbortableBackgroundWorker() : base()
        {
            this.WorkerSupportsCancellation = true;
        }

        public AbortableBackgroundWorker(DoWorkEventHandler WorkProcess, RunWorkerCompletedEventHandler WorkComplete) : base()
        {
            this.WorkerSupportsCancellation = true;

            this.DoWork += WorkProcess;                                     // Select Update Job
            this.RunWorkerCompleted += WorkComplete;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; //We must set Cancel property to true!
                Thread.ResetAbort(); //Prevents ThreadAbortException propagation
            }
        }


        public void Abort()
        {
            if (workerThread != null)
            {
                workerThread.Abort();
                workerThread = null;
            }
        }
    }
}
