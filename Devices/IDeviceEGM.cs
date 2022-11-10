using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    enum eDeviceEGMRequest { None, GetEGMValue = 101, GetEGMSettings = 102, GetEGMLimits = 103, SetTime = 104, Start = 105, Stop = 106, Latch = 107, Clear = 108,
        SetHV = 302, CalibHV_SetPoint = 303, CalibHV_Set = 304
    }



    public interface IDeviceEGM : IDevice
    {
        GeigerValueEx ReadEGMValue();
        bool ReadEGMValue(out GeigerValue Value, out CommException Error);
        GeigerValueEx GetEGMValue();
        bool GetEGMValue(out GeigerValue Value, out CommException Error);
        GeigerSettingsEx GetEGMSettings();
        bool GetEGMSettings(out GeigerSettings Value, out CommException Error);
        GeigerLimitsEx GetEGMLimits();
        bool GetEGMLimits(out GeigerLimits Value, out CommException Error);
        OkEx SetTime(int Time);
        bool SetTime(int Time, out CommException Error);


        OkEx Start();
        OkEx Stop();


        OkEx SetHV(int HV);
        bool SetHV(int HV, out CommException Error);
        OkEx CalibHV_SetPoint(byte domain, byte point, float voltage);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error);
        OkEx CalibHV_Set(byte domain);
        bool CalibHV_Set(byte domain, out CommException Error);

    }
}
