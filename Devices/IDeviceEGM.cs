using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    enum eDeviceEGMRequest { None, SetTime, Start, Stop, GetSettings, GetDescription, Password, Files, StartSpec, StopSpec, HVOn, HVOff, SetHV, SetHVForce, Firmware, ConfigSet, ConfigGet, ConfigReset, ConfigCreateFactory, ChangeMode, CalibHVSetPoint, CalibHV_SetMaxHV, CalibHV_SetMaxDAC }



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
