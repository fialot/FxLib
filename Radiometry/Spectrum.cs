using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Radiometry
{

    public enum EnergyCalibrationType
    {
        Linear = 0,
        Polynomial = 1
    }

    public enum FWHMCalibrationType
    {
        GenieSQRT = 0,
        GeniePolynom = 1,
        Polynomial = 2,
        DH = 3,
        SRQADR = 4,
        Linear = 5,
        Constant = 6

    }

    public struct EnergyCalibration
    {
        public EnergyCalibrationType Type;
        public double A;
        public double B;
        public double C;
    }

    public struct FWHMCalibration
    {
        public FWHMCalibrationType Type;
        public double A;
        public double B;
        public double C;
    }

    public class Spectrum
    {
        public Spectrum()
        {
            Channels = new uint[0];
        }

        /// <summary>
        /// Real time of spectrum
        /// </summary>
        public float RealTime { get; set; }

        /// <summary>
        /// Live time of spectrum
        /// </summary>
        public float LiveTime { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Spectrum channels
        /// </summary>
        public uint[] Channels { get; set; }

        /// <summary>
        /// Energy calibration
        /// </summary>
        public EnergyCalibration Energy { get; set; }

        /// <summary>
        /// FWHM calibration
        /// </summary>
        public FWHMCalibration FWHM { get; set; }
    }

}
