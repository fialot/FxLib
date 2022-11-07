using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        /// <summary>
        /// Start measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx start()
        {
            try
            {
                devStart();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Stop measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx stop()
        {
            try
            {
                devStop();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Latch measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx latch()
        {
            try
            {
                devLatch();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Clear measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx clear()
        {
            try
            {
                devClear();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Start Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx startSpectrum()
        {
            try
            {
                devStartSpectrum();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Stop Spectrum measuring
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx stopSpectrum()
        {
            try
            {
                devStopSpectrum();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Clear spectrum
        /// </summary>
        /// <returns>Returns true if communication ok</returns>
        private OkEx clearSpectrum()
        {
            try
            {
                devClearSpectrum();
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get Spectrum
        /// </summary>
        /// <returns>Returns Spectrum if communication ok</returns>
        private SpectrumEx getSpectrum()
        {
            try
            {
                return devGetSpectrum();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get calibration coeficients (Energy, FWHM)
        /// </summary>
        /// <returns>Returns Calibration if communication ok</returns>
        private MCACalibrationEx getMCACalibration()
        {
            try
            {
                return devGetCalibration();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Switch HV On/Off
        /// </summary>
        /// <param name="On">Turn On/Off</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx switchHV(bool On)
        {
            try
            {
                devSwitchHV(On);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx setHV(int HV)
        {
            try
            {
                devSetHV(HV);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// HV calibration - Set point
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <param name="point">Point index</param>
        /// <param name="voltage">Real voltage</param>
        /// <returns>Returns true if ok</returns>
        private OkEx calibHV_SetPoint(byte domain, byte point, float voltage)
        {
            try
            {
                devSetCalibHVPoint(domain, point, voltage);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// HV calibration - Set new calibration
        /// </summary>
        /// <param name="domain">Domain index</param>
        /// <returns>Returns true if ok</returns>
        private OkEx calibHV_Set(byte domain)
        {
            try
            {
                devSetCalibHV(domain);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get Geiger Value
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerValueEx getEGMValue()
        {
            try
            {
                return devGetEGMValue();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get Device Settings
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerSettingsEx getEGMSettings()
        {
            try
            {
                return devGetEGMSettings();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Get limits
        /// </summary>
        /// <returns>Returns true if read ok</returns>
        private GeigerLimitsEx getEGMLimits()
        {
            try
            {
                return devGetEGMLimits();
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }

        /// <summary>
        /// Set Measurement Time
        /// </summary>
        /// <param name="Time">Time</param>
        /// <returns>Returns true if communication ok</returns>
        private OkEx setTime(int Time)
        {
            try
            {
                devSetTime(Time);
                return true;
            }
            catch (CommException err)
            {
                return err;
            }
            catch (Exception err)
            {
                return new CommException(err.Message, err);
            }
        }
    }
}
