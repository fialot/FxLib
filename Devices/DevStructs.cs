using Fx.IO.Exceptions;
using Fx.Radiometry;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    // ----- Devices -----
    public enum DeviceType { General, EGM, MCA}

    public enum DevMode { Basic = 0, Advanced = 1, Service = 2, Debug = 3 }
    public enum DevPermission { None = 0, Advanced = 1, Service = 2, SuperUser = 3 }


    public class OkEx : OneOfBase<bool, CommException>
    {
        OkEx(OneOf<bool, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator OkEx(bool _) => new OkEx(_);
        public static implicit operator OkEx(CommException _) => new OkEx(_);

    }

    public class StringEx : OneOfBase<string, CommException>
    {
        StringEx(OneOf<string, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator StringEx(string _) => new StringEx(_);
        public static implicit operator StringEx(CommException _) => new StringEx(_);
    }

    public class StringArrayEx : OneOfBase<string[], CommException>
    {
        StringArrayEx(OneOf<string[], CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator StringArrayEx(string[] _) => new StringArrayEx(_);
        public static implicit operator StringArrayEx(CommException _) => new StringArrayEx(_);
    }

    public class DeviceInfoEx : OneOfBase<DeviceInfo, CommException>
    {
        DeviceInfoEx(OneOf<DeviceInfo, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator DeviceInfoEx(DeviceInfo _) => new DeviceInfoEx(_);
        public static implicit operator DeviceInfoEx(CommException _) => new DeviceInfoEx(_);

    }

    public class DevParamsEx : OneOfBase<List<DevParams>, CommException>
    {
        DevParamsEx(OneOf<List<DevParams>, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator DevParamsEx(List<DevParams> _) => new DevParamsEx(_);
        public static implicit operator DevParamsEx(CommException _) => new DevParamsEx(_);

    }

    public class DevMeasValsEx : OneOfBase<List<DevMeasVals>, CommException>
    {
        DevMeasValsEx(OneOf<List<DevMeasVals>, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator DevMeasValsEx(List<DevMeasVals> _) => new DevMeasValsEx(_);
        public static implicit operator DevMeasValsEx(CommException _) => new DevMeasValsEx(_);

    }

    public class DevParamValueEx : OneOfBase<DevParamVals, CommException>
    {
        DevParamValueEx(OneOf<DevParamVals, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator DevParamValueEx(DevParamVals _) => new DevParamValueEx(_);
        public static implicit operator DevParamValueEx(CommException _) => new DevParamValueEx(_);

    }

    public class DevParamValuesEx : OneOfBase<List<DevParamVals>, CommException>
    {
        DevParamValuesEx(OneOf<List<DevParamVals>, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator DevParamValuesEx(List<DevParamVals> _) => new DevParamValuesEx(_);
        public static implicit operator DevParamValuesEx(CommException _) => new DevParamValuesEx(_);

    }

    public class PermissionEx : OneOfBase<DevPermission, CommException>
    {
        PermissionEx(OneOf<DevPermission, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator PermissionEx(DevPermission _) => new PermissionEx(_);
        public static implicit operator PermissionEx(CommException _) => new PermissionEx(_);

    }


    public class GeigerValueEx : OneOfBase<GeigerValue, CommException>
    {
        GeigerValueEx(OneOf<GeigerValue, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator GeigerValueEx(GeigerValue _) => new GeigerValueEx(_);
        public static implicit operator GeigerValueEx(CommException _) => new GeigerValueEx(_);
    }

    public class GeigerSettingsEx : OneOfBase<GeigerSettings, CommException>
    {
        GeigerSettingsEx(OneOf<GeigerSettings, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator GeigerSettingsEx(GeigerSettings _) => new GeigerSettingsEx(_);
        public static implicit operator GeigerSettingsEx(CommException _) => new GeigerSettingsEx(_);
    }

    public class GeigerLimitsEx : OneOfBase<GeigerLimits, CommException>
    {
        GeigerLimitsEx(OneOf<GeigerLimits, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator GeigerLimitsEx(GeigerLimits _) => new GeigerLimitsEx(_);
        public static implicit operator GeigerLimitsEx(CommException _) => new GeigerLimitsEx(_);
    }

    public class SpectrumEx : OneOfBase<Spectrum, CommException>
    {
        SpectrumEx(OneOf<Spectrum, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator SpectrumEx(Spectrum _) => new SpectrumEx(_);
        public static implicit operator SpectrumEx(CommException _) => new SpectrumEx(_);
    }

    public class MCACalibrationEx : OneOfBase<MCACalibration, CommException>
    {
        MCACalibrationEx(OneOf<MCACalibration, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator MCACalibrationEx(MCACalibration _) => new MCACalibrationEx(_);
        public static implicit operator MCACalibrationEx(CommException _) => new MCACalibrationEx(_);
    }

    public class SCASettingsEx : OneOfBase<SCASettings, CommException>
    {
        SCASettingsEx(OneOf<SCASettings, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator SCASettingsEx(SCASettings _) => new SCASettingsEx(_);
        public static implicit operator SCASettingsEx(CommException _) => new SCASettingsEx(_);
    }

    public class SCAValueEx : OneOfBase<SCAValue, CommException>
    {
        SCAValueEx(OneOf<SCAValue, CommException> _) : base(_) { }

        // optionally, define implicit conversions
        // you could also make the constructor public
        public static implicit operator SCAValueEx(SCAValue _) => new SCAValueEx(_);
        public static implicit operator SCAValueEx(CommException _) => new SCAValueEx(_);
    }
    

    //public enum DeviceGroups { EGM, MCA, Linux }
    //public enum DevConnType { COM, TCP, UDP }

    [Flags]
    public enum DevSupport : uint
    {
        File = 0x00000001,
        Start = 0x00000002,
        Spectrum = 0x00000004,
        Bootloader = 0x00000008,
        Firmware = 0x00000010,
        Permission = 0x00000020,
        CalibHV = 0x00000040,

        /*  
        Support = 0x00000080,   
        Support = 0x00000100,  
        Support = 0x00000200,  
        Support = 0x00000400,  
        Support = 0x00000800,  
        Support = 0x00001000,  
        Support = 0x00002000, 
        Support = 0x00004000, 
        Support = 0x00008000,
        Support = 0x00010000, 
        */
    }

    // ----- Parameters -----
    public enum DevParamsType { Edit, Enum, Info, Text }
    public enum VariableType { Bool, Byte, SByte, Short, UShort, Int, UInt, Long, ULong, Float, Double, String }

    public enum eChangePassReply { OK, BadLength, NoPermissions };




    public class DevParams
    {
        public string Name { get; set; }
        public string Group { get; set; }

        public int ID { get; set; }
        public int Place { get; set; }


        public DevParamsType Type { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }

        public VariableType ValueType { get; set; }

        public int ValueLength { get; set; }

        public string Value { get; set; }
        public bool CheckRange { get; set; }

        public string File { get; set; }
        public string Section { get; set; }
        public string Key { get; set; }

        public Dictionary<string, string> ValCombo { get; set; }
        public Dictionary<string, string> ComboVal { get; set; }

        public DevParams()
        {
            Name = "";
            Group = "";
            ID = 1;
            Place = 0;

            Type = DevParamsType.Info;
            Min = float.NaN;
            Max = float.NaN;
            ValueType = VariableType.Short;        // Default for Modbus
            ValueLength = -1;
            Value = "";
            CheckRange = false;
            File = "";
            Section = "";
            Key = "";

            ValCombo = null;
            ComboVal = null;
        }
    }

    /// <summary>
    /// Device Measurement Values structure
    /// </summary>
    public class DevMeasVals
    {
        /// <summary>
        /// The name of the measured quantity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the measured quantity
        /// </summary>
        public int ID { get; set; }

        public int Place { get; set; }

        /// <summary>
        /// Measured quantity value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Measured quantity value format
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Measured quantity group
        /// </summary>
        public string Group { get; set; }

        public string File { get; set; }
        public string Section { get; set; }
        public string Key { get; set; }
    }



    /// <summary>
    /// Device Params structure
    /// </summary>
    public class DevParamVals
    {
        /// <summary>
        /// The ID of the measured quantity
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Measured quantity value
        /// </summary>
        public string Value { get; set; }

        public DevParamVals()
        {
            this.ID = 1;
            this.Value = "";
        }

        public DevParamVals(int ID, string Value)
        {
            this.ID = ID;
            this.Value = Value;
        }

    }


    /// <summary>
    /// Device info
    /// </summary>
    public struct DeviceInfo
    {
        public string Model;            // device model
        public string Version;          // FW version
        public string Date;             // Date
        public string SN;               // serial number
        public DeviceType Type;
    }

    public struct GeigerValue
    {
        public float ActualDR;          // actual DR (1 min avg)
        public float DR;                // measurement DR

        public int timeStamp;           // measurement time stamp

        public float[] ActualCPS;       // actual CPS
        public float[] CPS;             // measurement CPS

        public float ActualDeviation;   // actual deviation
        public float Deviation;         // measurement deviation

        public bool Valid;              // measurement valid

        public int Status;              // device status
        public int Error;               // device error

        public bool isRunning;          // measurement running

        public float Temperature;       // temperature
    }

    /// <summary>
    /// Device measurement settings
    /// </summary>
    public struct GeigerSettings
    {
        public int MeasureTime;         // measurement time [s]
        //public bool isRunning;          // "is running" indication
    }

    /// <summary>
    /// Device alarm limits
    /// </summary>
    public struct GeigerLimits
    {
        public float Low1;              // DR limit - low 1
        public float Low2;              // DR limit - low 2
        public float High1;             // DR limit - high 1
        public float High2;             // DR limit - high 2
    }

    /// <summary>
    /// SCA measurement values
    /// </summary>
    public struct SCAValue
    {
        //public float ActualDR;          // actual DR (1 min avg)
        //public float DR;                // measurement DR

        public int timeStamp;           // measurement time stamp

        public float[] ActualCPS;       // actual CPS
        public float[] CPS;             // measurement CPS

        //public float ActualDeviation;   // actual deviation
        //public float Deviation;         // measurement deviation


        public bool Valid;              // measurement valid

        public int Status;              // device status
        public int Error;               // device error

        public bool isROIRunning;          // measurement running
        public bool isSpectRunning;          // measurement running

        public float Temperature;       // temperature
    }

    public struct SCAChannel
    {
        public uint LLD;
        public uint ULD;
        public bool Autostart;
        public uint Time;
    }

    /// <summary>
    /// Device measurement settings
    /// </summary>
    public struct SCASettings
    {
        public int MeasureTime;         // measurement time [s]
        public int HV;                  // HV
        public bool HV_up;              // HV turned on
        public SCAChannel[] Channels;          // SCA Channels
    }

    public struct MCACalibration
    {
        public EnergyCalibration Energy;
        public FWHMCalibration FWHM;
        public int HV;

        public MCACalibration(int HV)
        {
            Energy = new EnergyCalibration(EnergyCalibrationType.Linear, 0, 1, 0);
            FWHM = new FWHMCalibration(FWHMCalibrationType.Linear, 0, 1, 0);
            this.HV = HV;
        }
    }

    public static class DevicesFunc
    {
        public static Dictionary<int, string> GetValDict(List<DevMeasVals> values)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            foreach (DevMeasVals item in values)
            {
                dict.Add(item.ID, item.Value);
            }

            return dict;
        }
    }


}
