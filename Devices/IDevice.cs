using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    enum eDeviceRequest { None, GetInfo, GetXmlDescription, GetDescription, GetMeasurement,
        GetParameter, GetParameters, GetAllParameters, SetParameter, SetParameters,
        Login, Logout, ChangePassword,
        GetDirectory, GetFile, DeleteFile, DeleteAllFiles,
        GetConfig, SetConfig, ResetConfig, CreateFactoryConfig,
        UpdateFirmware, RunApplication, RunBootloader, StayInBootloader
    }


    public interface IDevice
    {
        
        event NewDataEventHandler NewData;
        int RefreshInterval { get; set; }
        bool RunningMeasurement { get; }
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
        OkEx Connect();
        bool Connect(out CommException Error);

        OkEx Connect(ConnectionSetting settings);
        bool Connect(ConnectionSetting settings, out CommException Error);

        OkEx Disconnect();
        bool Disconnect(out CommException Error);
        bool IsConnected();
        void SetConnectionSettings(ConnectionSetting settings);

        // ----- Auto Reading -----
        bool StartReading();
        bool StopReading();

        // ----- Info -----
        DeviceInfoEx GetInfo();
        bool GetInfo(out DeviceInfo Value, out CommException Error);

        StringEx GetXMLDesc();
        bool GetXMLDesc(out string XML, out CommException Error);
        void SetLanguage(string Language);

        void SetMode(DevMode Mode);

        DevParamsEx GetDescription();
        bool GetDescription(out List<DevParams> Value, out CommException Error);

        string[] GetStatusList(int code);
        string[] GetErrorList(int code);

        // ----- Measurement -----
        DevMeasValsEx GetMeasurement();
        bool GetMeasurement(out List<DevMeasVals> Value, out CommException Error);

        // ----- Parameters -----
        DevParamValueEx GetParam(int ID);
        DevParamValueEx GetParam(DevParamVals Param);
        bool GetParam(ref DevParamVals Param, out CommException Error);
        DevParamValuesEx GetParams(List<DevParamVals> Param);
        bool GetParams(ref List<DevParamVals> Param, out CommException Error);

        DevParamsEx GetAllParams();
        bool GetAllParams(out List<DevParams> Param, out CommException Error);
        OkEx SetParam(int ID, string Param);
        OkEx SetParam(DevParamVals Param);
        bool SetParam(int ID, string Param, out CommException Error);
        OkEx SetParams(List<DevParamVals> Param);
        bool SetParams(List<DevParamVals> Param, out CommException Error);

        // ----- Login -----
        PermissionEx Login(string password);
        bool Login(string password, out DevPermission permission, out CommException Error);
        PermissionEx Logout();
        bool Logout(out DevPermission permission, out CommException Error);
        OkEx ChangePassword(string password);
        bool ChangePassword(string password, out CommException Error);


        // ----- Files -----
        StringArrayEx GetDir();
        bool GetDir(out string[] FileList, out CommException Error);
        StringEx GetFile(string FileName);
        bool GetFile(string FileName, out string text, out CommException Error);
        OkEx DelFile(string FileName);
        bool DelFile(string FileName, out CommException Error);

        OkEx DelAllFiles();

        bool DelAllFiles(out CommException Error);

        // ----- Configuration -----
        StringEx GetConfig();
        bool GetConfig(out string text, out CommException Error);
        OkEx SetConfig(string FileName);
        bool SetConfig(string FileName, out CommException Error);

        OkEx ResetConfig();
        bool ResetConfig(out CommException Error);
        OkEx CreateFactoryConfig();
        bool CreateFactoryConfig(out CommException Error);

        // ----- Firmware -----
        OkEx UpdateFirmware(string FileName);
        bool UpdateFirmware(string FileName, out CommException Error);
        OkEx RunApplication();
        bool RunApplication(out CommException Error);
        OkEx RunBootloader();
        bool RunBootloader(out CommException Error);
        OkEx StayInBootloader();
        bool StayInBootloader(out CommException Error);


    }
}
