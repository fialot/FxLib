using Fx.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    public interface IDevice
    {
        string DeviceName { get; }
        DeviceType Type { get; }
        ConnectionSetting Settings { get; }

        // ----- Support functions -----
        DevSupport Support { get; }
        /*bool FileSupport { get;  }
        bool StartSupport { get;  }
        bool SpectrumSupport { get;  }
        bool BootloaderSupport { get;  }
        bool FirmwareSupport { get;  }
        bool PermissionSupport { get;  }
        bool CalibHVSupport { get;  }*/
        uint HVDomainsCount { get;  }

        // ----- Status -----
        bool InBootloaderMode { get; }

        DevPermission Permission { get;}

        // ----- Connection -----
        bool Connect();
        bool Connect(out Exception Error);

        bool Connect(ConnectionSetting settings);
        bool Connect(ConnectionSetting settings, out Exception Error);

        bool Disconnect();
        bool IsConnected();
        void SetConnectionSettings(ConnectionSetting settings);

        // ----- Info -----
        bool GetInfo(out DeviceInfo Value);
        bool GetInfo(out DeviceInfo Value, out CommException Error);

        bool GetXMLDesc();
        bool GetXMLDesc(out string XML);
        bool GetXMLDesc(out string XML, out CommException Error);
        void SetLanguage(string Language);

        void SetMode(DevMode Mode);

        bool GetDescription(out List<DevParams> Value);
        bool GetDescription(out List<DevParams> Value, out CommException Error);

        string[] GetStatusList(int code);
        string[] GetErrorList(int code);

        // ----- Measurement -----
        bool GetMeasurement(out List<DevMeasVals> Value);
        bool GetMeasurement(out List<DevMeasVals> Value, out CommException Error);

        // ----- Parameters -----
        bool GetParam(ref DevParamVals Param);
        bool GetParam(ref DevParamVals Param, out CommException Error);
        bool GetParams(ref List<DevParamVals> Param);
        bool GetParams(ref List<DevParamVals> Param, out CommException Error);

        bool GetAllParams(out List<DevParams> Param);
        bool GetAllParams(out List<DevParams> Param, out CommException Error);
        bool SetParam(int ID, string Param);
        bool SetParam(int ID, string Param, out CommException Error);
        bool SetParams(List<DevParamVals> Param);
        bool SetParam(List<DevParamVals> Param, out CommException Error);

        // ----- Login -----
        bool Login(string password, out DevPermission permission);
        bool Login(string password, out DevPermission permission, out CommException Error);
        bool Logout();
        bool Logout(out CommException Error);
        bool ChangePassword(string password, out eChangePassReply reply);
        bool ChangePassword(string password, out eChangePassReply reply, out CommException Error);


        // ----- Files -----
        bool GetDir(out string[] FileList);
        bool GetDir(out string[] FileList, out CommException Error);
        bool GetFile(string FileName, out string text);
        bool GetFile(string FileName, out string text, out CommException Error);
        bool DelFile(string FileName);
        bool DelFile(string FileName, out CommException Error);

        bool DelAllFiles();

        bool DelAllFiles(out CommException Error);

        // ----- Configuration -----
        bool GetConfig(out string text);
        bool GetConfig(out string text, out CommException Error);
        bool SetConfig(string FileName);
        bool SetConfig(string FileName, out CommException Error);

        bool ResetConfig();
        bool ResetConfig(out CommException Error);
        bool CreateFactoryConfig();
        bool CreateFactoryConfig(out CommException Error);

        // ----- Firmware -----
        bool UpdateFirmware(string FileName);
        bool UpdateFirmware(string FileName, out CommException Error);
        bool RunApplication();
        bool RunApplication(out CommException Error);
        bool RunBootloader();
        bool RunBootloader(out CommException Error);
        bool StayInBootloader();
        bool StayInBootloader(out CommException Error);


    }
}
