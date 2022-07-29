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
        
        /// <summary>
        /// Real time of spectrum
        /// </summary>
        public float RealTime { get; set; } = 0;

        /// <summary>
        /// Live time of spectrum
        /// </summary>
        public float LiveTime { get; set; } = 0;

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Spectrum channels
        /// </summary>
        public uint[] Channels { get; set; } = new uint[0];

        /// <summary>
        /// Spectrum channels
        /// </summary>
        public float[] ChannelsEnergy { get; set; } = new float[0];

        /// <summary>
        /// Energy calibration
        /// </summary>
        public EnergyCalibration Energy
        {
            get
            {
                return _Energy;
            }
            set
            {
                _Energy = value;
                RecalcEnergy();
            }
        }
        
        /// <summary>
        /// FWHM calibration
        /// </summary>
        public FWHMCalibration FWHM { get; set; }


        private EnergyCalibration _Energy = new EnergyCalibration() { B = 1 };

        public Spectrum()
        {

        }

        /// <summary>
        /// Recalculate energy in channels
        /// </summary>
        public void RecalcEnergy()
        {
            // ----- Compute Energy to each channel -----
            for (int i = 0; i < Channels.Length; i++)
            {
                float energy;
                if ((_Energy.A == 0) && (_Energy.B == 0) && (_Energy.C == 0))
                    energy = i;
                else
                    energy = (float)(_Energy.A + (_Energy.B * i) + (_Energy.C * i * i));
                ChannelsEnergy[i] = energy;
            }
        }
    }

}
