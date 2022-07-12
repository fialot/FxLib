using Fx.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public interface IDeviceEGM : IDevice
    {
        bool SetHV(int HV);
        bool SetHV(int HV, out CommException Error);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error);
        bool CalibHV_Set(byte domain);
        bool CalibHV_Set(byte domain, out CommException Error);

    }
}
