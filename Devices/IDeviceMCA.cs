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
    public interface IDeviceMCA : IDevice
    {
        bool GetSettings(out SCASettings Value);
        bool GetSettings(out SCASettings Value, out CommException Error);
        bool ReadValue(out SCAValue Value);
        bool ReadValue(out SCAValue Value, out CommException Error);
        bool GetValue(out SCAValue Value);
        bool GetValue(out SCAValue Value, out CommException Error);


        bool SetTime(float Time);
        bool SetTime(float Time, out CommException Error);


        bool Start();
        bool Start(out CommException Error);
        bool Stop();
        bool Stop(out CommException Error);
        bool Latch();
        bool Latch(out CommException Error);
        bool Clear();
        bool Clear(out CommException Error);
        bool SwitchHV(bool On);
        bool SwitchHV(bool On, out CommException Error);

        bool StartSpectrum();
        bool StartSpectrum(out CommException Error);
        bool StopSpectrum();
        bool StopSpectrum(out CommException Error);
        bool ClearSpectrum();
        bool ClearSpectrum(out CommException Error);
        bool GetSpectrum(out Spectrum spectrum);
        bool GetSpectrum(out Spectrum spectrum, out CommException Error);
        bool GetCalibration(out MCACalibration Calib);
        bool GetCalibration(out MCACalibration Calib, out CommException Error);


        bool SetHV(int HV);
        bool SetHV(int HV, out CommException Error);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage);
        bool CalibHV_SetPoint(byte domain, byte point, float voltage, out CommException Error);
        bool CalibHV_Set(byte domain);
        bool CalibHV_Set(byte domain, out CommException Error);
    }
}
