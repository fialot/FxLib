using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        protected override void doDevRequest()
        {
            if (Type == DeviceType.EGM)
            {
                var egmRequest = (eDeviceEGMRequest)request;

                switch (egmRequest)
                {
                    case eDeviceEGMRequest.SetTime:
                        requestReply = setTime((int)requestValue); break;
                    case eDeviceEGMRequest.Start:
                        requestReply = Start(); break;
                    case eDeviceEGMRequest.Stop:
                        requestReply = Start(); break;

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


        
    }
}
