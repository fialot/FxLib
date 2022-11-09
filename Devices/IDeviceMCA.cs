using Fx.IO;
using Fx.IO.Exceptions;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    enum eDeviceMCARequest { None, SetTime, Start, Stop, GetSettings, GetDescription, Password, Files, StartSpec, StopSpec, HVOn, HVOff, SetHV, SetHVForce, Firmware, ConfigSet, ConfigGet, ConfigReset, ConfigCreateFactory, ChangeMode, CalibHVSetPoint, CalibHV_SetMaxHV, CalibHV_SetMaxDAC }


    public delegate void NewSpectrumEventHandler(object source, Spectrum newSpectrum);

    public interface IDeviceMCA : IDevice
    {

        event NewSpectrumEventHandler NewSpectrum;

        SCASettingsEx GetSCASettings();
        bool GetSCASettings(out SCASettings Value, out CommException Error);
        SCAValueEx ReadSCAValue();
        bool ReadSCAValue(out SCAValue Value, out CommException Error);
        SCAValueEx GetSCAValue();
        bool GetSCAValue(out SCAValue Value, out CommException Error);


        OkEx SetTime(float Time);
        bool SetTime(float Time, out CommException Error);


        OkEx Start();
        bool Start(out CommException Error);
        OkEx Stop();
        bool Stop(out CommException Error);
        OkEx Latch();
        bool Latch(out CommException Error);
        OkEx Clear();
        bool Clear(out CommException Error);
        OkEx SwitchHV(bool On);
        bool SwitchHV(bool On, out CommException Error);

        OkEx StartSpectrum();
        bool StartSpectrum(out CommException Error);
        OkEx StopSpectrum();
        bool StopSpectrum(out CommException Error);
        OkEx ClearSpectrum();
        bool ClearSpectrum(out CommException Error);
        SpectrumEx GetSpectrum();
        bool GetSpectrum(out Spectrum spectrum, out CommException Error);
        MCACalibrationEx GetMCACalibration();
        bool GetMCACalibration(out MCACalibration Calib, out CommException Error);


        OkEx SetHV(int HV);
        bool SetHV(int HV, out CommException Error);
        OkEx CalibHV_SetPoint(byte domain, byte point, float voltage);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error);
        OkEx CalibHV_Set(byte domain);
        bool CalibHV_Set(byte domain, out CommException Error);
    }
}
