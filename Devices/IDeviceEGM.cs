using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public interface IDeviceEGM : IDevice
    {
        bool ReadValue(out GeigerValue Value);
        bool ReadValue(out GeigerValue Value, out CommException Error);
        bool GetValue(out GeigerValue Value);
        bool GetValue(out GeigerValue Value, out CommException Error);
        bool GetSettings(out GeigerSettings Value);
        bool GetSettings(out GeigerSettings Value, out CommException Error);
        bool GetLimits(out GeigerLimits Value);
        bool GetLimits(out GeigerLimits Value, out CommException Error);
        bool SetTime(int Time);
        bool SetTime(int Time, out CommException Error);


        bool Start();
        bool Stop();


        bool SetHV(int HV);
        bool SetHV(int HV, out CommException Error);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error);
        bool CalibHV_Set(byte domain);
        bool CalibHV_Set(byte domain, out CommException Error);

    }
}
