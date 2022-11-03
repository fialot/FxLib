using Fx.Conversion;
using Fx.IO;
using Fx.IO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fx.Devices
{

    public abstract partial class Device : IDevice
    {
        
        #region Public


        public string DeviceName { get; protected set; }
        public DeviceType Type { get; protected set; }
        
        public ConnectionSetting Settings { get; protected set; }

        public DevSupport Support { get; protected set; }

        /*public bool FileSupport { get; protected set; }
        public bool StartSupport { get; protected set; }
        public bool SpectrumSupport { get; protected set; }
        public bool BootloaderSupport { get; protected set; }
        public bool FirmwareSupport { get; protected set; }
        public bool PermissionSupport { get; protected set; }
        public bool CalibHVSupport { get; protected set; }*/

        public uint HVDomainsCount { get; protected set; }

        public bool InBootloaderMode { get; protected set; }

        public DevPermission Permission { get; protected set; }


        #region Constructor
        public Device()
        {
            // ----- Auto reading -----
            worker.DoWork += WorkProcess;                                  // Select Process Job
            worker.RunWorkerCompleted += WorkComplete;                     // Select Done Job

            Support = 0;
            /*FileSupport = false;
            StartSupport = false;
            SpectrumSupport = false;
            BootloaderSupport = false;
            FirmwareSupport = false;
            PermissionSupport = false;
            CalibHVSupport = false;*/
            InBootloaderMode = false;

            Permission = DevPermission.None;
        }

        #endregion

        #region Connection

        /// <summary>
        /// Connect to device
        /// </summary>
        /// <returns>Returns true if connection ok</returns>
        public OkEx Connect()
        {
            CommException Error;

            if (Connect(out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Connect to device
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if connection ok</returns>
        public bool Connect(out CommException Error)
        {
            Error = null;
            try
            {
                connect();
                return true;
            }
            catch (Exception err)
            {
                Error = new ConnectionFailedException(err);
            }
            return false;
        }

        /*public bool Connect(ConnectionSetting settings)
        {
            return Connect(settings, out Exception Error);
        }*/

        public OkEx Connect(ConnectionSetting settings)
        {
            CommException Error;

            if (Connect(settings, out Error))
                return true;
            else
                return Error;
        }

        public bool Connect(ConnectionSetting settings, out CommException Error)
        {
            Error = null;
            try
            {
                Settings = settings;
                connect();
                return true;
            }
            catch (Exception err)
            {
                Error = new ConnectionFailedException(err);
            }
            return false;
        }


        /// <summary>
        /// Disconnect device
        /// </summary>
        /// <returns>Returns true if connection ok</returns>
        public bool Disconnect()
        {
            return Disconnect(out CommException Error);
        }

        /// <summary>
        /// Disconnect device
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if connection ok</returns>
        public bool Disconnect(out CommException Error)
        {
            Error = null;
            try
            {
                disconnect();
                return true;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message);
            }
            return false;
        }

        /// <summary>
        /// Check if device connected
        /// </summary>
        /// <returns>Returns true if connected</returns>
        public bool IsConnected()
        {
            return isConnected();
        }

        public void SetConnectionSettings(ConnectionSetting settings)
        {
            Settings = settings;
        }

        #endregion

        #region Auto reading

        public bool StartReading()
        {
            return startReading();
        }

        public bool StopReading()
        {

            return stopReading();
        }

        #endregion


        #region Device Info

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <param name="Value">Device Info</param>
        /// <returns>Returns true if read ok</returns>
        public DeviceInfoEx GetInfo()
        {
            DeviceInfo Value;
            CommException Error;

            if (GetInfo(out Value, out Error))
                return Value;
            else
                return Error;
        }

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <param name="Value">Device Info</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetInfo(out DeviceInfo Value, out CommException Error)
        {
            Error = null;
            Value = new DeviceInfo();
            try
            {
                Value = getInfo();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }



        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <param name="XML">XML string</param>
        /// <returns>Returns true if read ok</returns>
        public StringEx GetXMLDesc()
        {
            string XML;
            CommException Error;
            if (GetXMLDesc(out XML, out Error))
                return XML;
            else
                return Error;
        }

        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <param name="XML">XML description</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetXMLDesc(out string XML, out CommException Error)
        {
            Error = null;
            XML = "";
            try
            {
                XML = getXML();
                GetSupport(XML);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Set Device Language
        /// </summary>
        /// <param name="Language">Language (EN / CZ)</param>
        public void SetLanguage(string Language)
        {
            if (Language == "cs-CZ" || Language == "cs")
                Language = "CZ";
            language = Language;
        }

        /// <summary>
        /// Set Device mode
        /// </summary>
        /// <param name="Mode">Mode of settings</param>
        public void SetMode(DevMode Mode)
        {
            this.mode = Mode;
        }

        
        /// <summary>
        /// Create measurement list from XML description
        /// </summary>
        /// <param name="XMLdesc">XML description</param>
        /// <returns>Measurement list</returns>
        protected List<DevParams> CreateDescriptionList(string XMLdesc, DevMode Mode)
        {
            List<DevParams> MeasList = new List<DevParams>();

            // ----- Parse XML -----
            var xml = XDocument.Parse(XMLdesc);

            IEnumerable<XElement> setups;
            if (Mode == DevMode.Basic)
                setups = xml.Elements().Elements("descriptions").Elements("description").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard"));
            else if (Mode == DevMode.Advanced)
                setups = xml.Elements().Elements("descriptions").Elements("description").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard" || (string)x.Attribute("id") == "advanced"));
            else if (Mode == DevMode.Service)
                setups = xml.Elements().Elements("descriptions").Elements("description").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard" || (string)x.Attribute("id") == "advanced" || (string)x.Attribute("id") == "service"));
            else
                setups = xml.Elements().Elements("descriptions").Elements("description");

            IEnumerable<XElement> values = setups.Elements("group").Elements();

            int place = 0;

            foreach (var value in values)
            {
                try
                {
                    DevParams item = new DevParams();
                    item.Place = place;

                    if (value.Attribute("name") != null)
                        item.Name = value.Attribute("name").Value;
                    if (value.Attribute("id") != null)
                        item.ID = Conv.ToInt(value.Attribute("id").Value, 0);
                    if (value.Attribute("format") != null)
                    {
                        if (value.Attribute("format").Value == "%b")
                            item.ValueType = VariableType.Bool;
                        else if (value.Attribute("format").Value == "%d" || value.Attribute("format").Value == "%i")
                            item.ValueType = VariableType.Int;
                        else if (value.Attribute("format").Value == "%u")
                            item.ValueType = VariableType.UInt;
                        else if (value.Attribute("format").Value == "%ld" || value.Attribute("format").Value == "%li")
                            item.ValueType = VariableType.Long;
                        else if (value.Attribute("format").Value == "%lu")
                            item.ValueType = VariableType.ULong;
                        else if (value.Attribute("format").Value == "%f")
                            item.ValueType = VariableType.Float;
                        else if (value.Attribute("format").Value == "%lf")
                            item.ValueType = VariableType.Double;
                        else if (value.Attribute("format").Value == "%hd" || value.Attribute("format").Value == "%hi")
                            item.ValueType = VariableType.Short;
                        else if (value.Attribute("format").Value == "%hu")
                            item.ValueType = VariableType.UShort;
                        else if (value.Attribute("format").Value.Contains("s"))
                        {
                            item.ValueType = VariableType.String;
                            var format = value.Attribute("format").Value.Replace("%", "").Replace("s", "");
                            item.ValueLength = Conv.ToInt(format, -1);
                        }

                    }


                    if (value.Attribute("file") != null)
                        item.File = value.Attribute("file").Value;
                    if (value.Attribute("section") != null)
                        item.Section = value.Attribute("section").Value;
                    if (value.Attribute("key") != null)
                        item.Key = value.Attribute("key").Value;

                    if (value.Parent.Attribute("name") != null)
                        item.Group = value.Parent.Attribute("name").Value;
                    if (item.ID > 0)
                    {
                        item.Value = "";
                        place++;
                        MeasList.Add(item);
                    }
                }
                catch { }
            }

            return MeasList;
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <returns>Returns true if connection ok</returns>
        public DevParamsEx GetDescription()
        {
            List<DevParams> Value;
            CommException Error;

            if (GetDescription(out Value, out Error))
                return Value;
            else
                return Error;
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetDescription(out List<DevParams> Value, out CommException Error)
        {
            Error = null;
            Value = new List<DevParams>();
            try
            {
                Value = getDescription();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Create measurement list from XML description
        /// </summary>
        /// <param name="XMLdesc">XML description</param>
        /// <returns>Measurement list</returns>
        protected List<DevMeasVals> CreateMeasList(string XMLdesc, DevMode Mode)
        {
            List<DevMeasVals> MeasList = new List<DevMeasVals>();

            // ----- Parse XML -----
            var xml = XDocument.Parse(XMLdesc);

            IEnumerable<XElement> setups;
            if (Mode == DevMode.Basic)
                setups = xml.Elements().Elements("measurements").Elements("measurement").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard"));
            else if (Mode == DevMode.Advanced)
                setups = xml.Elements().Elements("measurements").Elements("measurement").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard" || (string)x.Attribute("id") == "advanced"));
            else if (Mode == DevMode.Service)
                setups = xml.Elements().Elements("measurements").Elements("measurement").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "standard" || (string)x.Attribute("id") == "advanced" || (string)x.Attribute("id") == "service"));
            else
                setups = xml.Elements().Elements("measurements").Elements("measurement");

            IEnumerable<XElement> values = setups.Elements("group").Elements();

            int place = 0;

            foreach (var value in values)
            {
                try
                {
                    DevMeasVals item = new DevMeasVals();
                    item.Place = place;

                    if (value.Attribute("name") != null)
                        item.Name = value.Attribute("name").Value;
                    if (value.Attribute("id") != null)
                        item.ID = Conv.ToInt(value.Attribute("id").Value, 0);
                    if (value.Attribute("format") != null)
                        item.Format = value.Attribute("format").Value;

                    if (value.Attribute("file") != null)
                        item.File = value.Attribute("file").Value;
                    if (value.Attribute("section") != null)
                        item.Section = value.Attribute("section").Value;
                    if (value.Attribute("key") != null)
                        item.Key = value.Attribute("key").Value;

                    if (value.Parent.Attribute("name") != null)
                        item.Group = value.Parent.Attribute("name").Value;
                    if (item.ID > 0)
                    {
                        item.Value = "";
                        place++;
                        MeasList.Add(item);
                    }
                }
                catch { }
            }

            return MeasList;
        }

        /// <summary>
        /// Fill Measurement 
        /// </summary>
        /// <param name="MeasList">Measurement list</param>
        /// <param name="valDict">Values list</param>
        /// <returns>Measured data</returns>
        protected List<DevMeasVals> FillMeas(List<DevMeasVals> MeasList, Dictionary<int, string> valDict)
        {
            List<DevMeasVals> valList = new List<DevMeasVals>();
            for (int i = 0; i < MeasList.Count; i++)
            {
                try
                {
                    DevMeasVals item = MeasList[i];
                    item.Value = valDict[item.ID];
                    valList.Add(item);
                }
                catch { }
            }
            return valList;
        }

        /// <summary>
        /// Fill Measurement 
        /// </summary>
        /// <param name="MeasList">Measurement list</param>
        /// <param name="valDict">Values list</param>
        /// <returns>Measured data</returns>
        protected List<DevParams> FillParam(List<DevParams> MeasList, Dictionary<int, string> valDict)
        {
            List<DevParams> valList = new List<DevParams>();
            for (int i = 0; i < MeasList.Count; i++)
            {
                try
                {
                    DevParams item = MeasList[i];
                    item.Value = valDict[item.ID];
                    valList.Add(item);
                }
                catch { }
            }
            return valList;
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <returns>Returns true if connection ok</returns>
        public DevMeasValsEx GetMeasurement()
        {
            List<DevMeasVals> Value;
            CommException Error;
            if (GetMeasurement(out Value, out Error))
                return Value;
            else
                return Error;
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetMeasurement(out List<DevMeasVals> Value, out CommException Error)
        {
            Error = null;
            Value = new List<DevMeasVals>();
            try
            {
                Value = getMeasurement();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion

        #region Parameters & Settings

        protected void GetSupport(string XMLdesc)
        {
            // ----- Clear flags -----
            Support &= ~DevSupport.Firmware;
            Support &= ~DevSupport.Bootloader;
            InBootloaderMode = false;

            var xml = XDocument.Parse(XMLdesc);

            var system = xml.Element("device").Element("system");

            if (system != null)
            {
                var fw = system.Element("fw");
                if (fw != null)
                {
                    var attr = fw.Attribute("has_bld");
                    if (attr != null)
                    {
                        if (attr.Value == "1")
                        {
                            Support &= ~DevSupport.Start;
                            Support |= DevSupport.Firmware;
                            InBootloaderMode = true;
                        }
                        else if (attr.Value == "2")
                        {
                            Support |= DevSupport.Bootloader;
                        }
                    }

                    attr = fw.Attribute("hv_domains");
                    if (attr != null)
                    {
                        HVDomainsCount = Conv.ToUInt(attr.Value, 0);

                        if (HVDomainsCount > 0)
                        {
                            Support |= DevSupport.CalibHV;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create Parameters list from XML description
        /// </summary>
        /// <param name="XMLdesc">XML Description</param>
        /// <param name="Mode">Mode of settings menu</param>
        /// <returns>Parameters list</returns>
        protected List<DevParams> CreateParamList(string XMLdesc, DevMode Mode, DevPermission Permission)
        {
            List<DevParams> ParamList = new List<DevParams>();

            // ----- Parse XML to Structure -----
            var xml = XDocument.Parse(XMLdesc);
            IEnumerable<XElement> setups;

            // ----- Check if new permission xml versions -----
            setups = xml.Elements().Elements("setups").Elements("setup").Where(x => ((string)x.Attribute("id") == "superuser"));

            if (setups.Any())
            {
                if (Mode == DevMode.Basic || Permission == DevPermission.None)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => (string)x.Attribute("id") == "main");
                else if (Mode == DevMode.Advanced || Permission <= DevPermission.Advanced)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "advanced"));
                else if (Mode == DevMode.Service || Permission <= DevPermission.Service)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "advanced" || (string)x.Attribute("id") == "service"));
                else
                    setups = xml.Elements().Elements("setups").Elements("setup");
            }
            else
            {
                if (Mode == DevMode.Basic)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => (string)x.Attribute("id") == "main");
                else if (Mode == DevMode.Advanced)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "advanced"));
                else if (Mode == DevMode.Service)
                    setups = xml.Elements().Elements("setups").Elements("setup").Where(x => ((string)x.Attribute("id") == "main" || (string)x.Attribute("id") == "advanced" || (string)x.Attribute("id") == "service"));
                else
                    setups = xml.Elements().Elements("setups").Elements("setup");
            }



            IEnumerable<XElement> settings = setups.Elements("group").Elements();
            // Read the entire XML

            int place = 0;


            foreach (var seting in settings)
            {
                DevParams item = new DevParams();
                item.Place = place;
                item.CheckRange = false;

                if (seting.Attribute("name") != null)
                    item.Name = seting.Attribute("name").Value;
                if (seting.Attribute("id") != null)
                    item.ID = Conv.ToInt(seting.Attribute("id").Value, 0);

                if (seting.Attribute("format") != null)
                {
                    if (seting.Attribute("format").Value == "%b")
                        item.ValueType = VariableType.Bool;
                    else if (seting.Attribute("format").Value == "%d" || seting.Attribute("format").Value == "%i")
                        item.ValueType = VariableType.Int;
                    else if (seting.Attribute("format").Value == "%u")
                        item.ValueType = VariableType.UInt;
                    else if (seting.Attribute("format").Value == "%ld" || seting.Attribute("format").Value == "%li")
                        item.ValueType = VariableType.Long;
                    else if (seting.Attribute("format").Value == "%lu")
                        item.ValueType = VariableType.ULong;
                    else if (seting.Attribute("format").Value == "%f")
                        item.ValueType = VariableType.Float;
                    else if (seting.Attribute("format").Value == "%lf")
                        item.ValueType = VariableType.Double;
                    else if (seting.Attribute("format").Value == "%hd" || seting.Attribute("format").Value == "%hi")
                        item.ValueType = VariableType.Short;
                    else if (seting.Attribute("format").Value == "%hu")
                        item.ValueType = VariableType.UShort;
                    else if (seting.Attribute("format").Value.Contains("s"))
                    {
                        item.ValueType = VariableType.String;
                        var format = seting.Attribute("format").Value.Replace("%", "").Replace("s", "");
                        item.ValueLength = Conv.ToInt(format, -1);
                    }
                }

                if (seting.Attribute("max") != null)
                {
                    item.Max = Conv.ToFloatI(seting.Attribute("max").Value, 0);
                    item.CheckRange = true;
                }
                else item.Max = float.MaxValue;

                if (seting.Attribute("min") != null)
                {
                    item.Min = Conv.ToFloatI(seting.Attribute("min").Value, 0);
                    item.CheckRange = true;
                }
                else item.Min = float.MinValue;

                if (seting.Attribute("file") != null)
                    item.File = seting.Attribute("file").Value;
                if (seting.Attribute("section") != null)
                    item.Section = seting.Attribute("section").Value;
                if (seting.Attribute("key") != null)
                    item.Key = seting.Attribute("key").Value;

                if (seting.Name == "edit")
                    item.Type = DevParamsType.Edit;
                else if (seting.Name == "enum")
                    item.Type = DevParamsType.Enum;
                else if (seting.Name == "info")
                    item.Type = DevParamsType.Info;
                else if (seting.Name == "text")
                    item.Type = DevParamsType.Text;
                if (seting.Parent.Attribute("name") != null)
                {
                    item.Group = seting.Parent.Attribute("name").Value;
                }

                item.Value = "";
                if (item.Type == DevParamsType.Enum)
                {
                    IEnumerable<XElement> enums = seting.Elements();
                    item.ValCombo = new Dictionary<string, string>();
                    item.ComboVal = new Dictionary<string, string>();
                    foreach (var en in enums)
                    {
                        if ((en.Attribute("name") != null) && (en.Attribute("name") != null))
                        {
                            item.ValCombo.Add(en.Attribute("value").Value, en.Attribute("name").Value);
                            item.ComboVal.Add(en.Attribute("name").Value, en.Attribute("value").Value);
                        }
                    }
                }
                if (item.ID > 0)
                {
                    place++;
                    ParamList.Add(item);
                }

            }
            return ParamList;
        }
        public DevParamValueEx GetParam(int ID)
        {
            CommException Error;
            DevParamVals Param = new DevParamVals(ID, "");
            if (GetParam(ref Param, out Error))
                return Param;
            else
                return Error;
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Returns true if read ok</returns>
        public DevParamValueEx GetParam(DevParamVals Param)
        {
            CommException Error;
            if (GetParam(ref Param, out Error))
                return Param;
            else
                return Error;
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetParam(ref DevParamVals Param, out CommException Error)
        {
            Error = null;
            try
            {
                Param = getParam(Param);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <returns>Returns true if read ok</returns>
        public DevParamValuesEx GetParams(List<DevParamVals> Param)
        {
            CommException Error;
            if (GetParams(ref Param, out Error))
                return Param;
            else
                return Error;
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetParams(ref List<DevParamVals> Param, out CommException Error)
        {
            Error = null;
            try
            {
                Param = getParams(Param);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <returns>Returns true if read ok</returns>
        public DevParamsEx GetAllParams()
        {
            List<DevParams> Param;
            CommException Error;
            if (GetAllParams(out Param, out Error))
                return Param;
            else
                return Error;
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetAllParams(out List<DevParams> Param, out CommException Error)
        {
            Error = null;
            Param = null;
            try
            {
                Param = getAllParams();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="ID">Parameter ID</param>
        /// <param name="Param">Parameter Value</param>
        /// <returns></returns>
        public OkEx SetParam(int ID, string Param)
        {
            CommException Error;
            if (SetParam(ID, Param, out Error))
                return true;
            else
                return Error;
        }

        public OkEx SetParam(DevParamVals Param)
        {
            CommException Error;
            if (SetParam(Param.ID, Param.Value, out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="ID">Parameter ID</param>
        /// <param name="Value">Parameter Value</param>
        /// <param name="Error">Error</param>
        /// <returns></returns>
        public bool SetParam(int ID, string Value, out CommException Error)
        {
            Error = null;
            try
            {
                setParam(ID, Value);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <returns>Returns true if write ok</returns>
        public OkEx SetParams(List<DevParamVals> Param)
        {
            CommException Error;
            if (SetParam(Param, out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if write ok</returns>
        public bool SetParam(List<DevParamVals> Param, out CommException Error)
        {
            Error = null;
            try
            {
                setParams(Param);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public PermissionEx Login(string password)
        {
            DevPermission permission;
            CommException Error;
            if (Login(password, out permission, out Error))
                return permission;
            else
                return Error;
        }

        public bool Login(string password, out DevPermission permission, out CommException Error)
        {
            Error = null;
            permission = DevPermission.None;
            try
            {
                permission = login(password);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public OkEx Logout()
        {
            CommException Error;
            if (Logout(out Error))
                return true;
            else
                return Error;
        }

        public bool Logout(out CommException Error)
        {
            Error = null;
            try
            {
                logout();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public OkEx ChangePassword(string password)
        {
            CommException Error;
            eChangePassReply reply;
            if (ChangePassword(password, out reply, out Error))
            {
                if (reply == eChangePassReply.BadLength)
                    return new BadLengthException();
                else if (reply == eChangePassReply.NoPermissions)
                    return new NoPermissionException();
                else
                    return true;
            }
            else
                return Error;

        }

        public bool ChangePassword(string password, out eChangePassReply reply, out CommException Error)
        {
            Error = null;
            reply = eChangePassReply.NoPermissions;
            try
            {
                reply = changePass(password);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion

        // ----- Files -----
        #region Files

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <param name="FileList">File list</param>
        /// <returns>Returns true if read ok</returns>
        public StringArrayEx GetDir()
        {
            string[] FileList;
            CommException Error;
            if (GetDir(out FileList, out Error))
                return FileList;
            else
                return Error;
        }

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <param name="FileList">File list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetDir(out string[] FileList, out CommException Error)
        {
            Error = null;
            FileList = new string[0];
            try
            {
                FileList = getDir();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="text">text</param>
        /// <returns>Returns true if ok</returns>
        public StringEx GetFile(string FileName)
        {
            string text;
            CommException Error;
            if (GetFile(FileName, out text, out Error))
                return text;
            else
                return Error;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="text">text</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool GetFile(string FileName, out string text, out CommException Error)
        {
            Error = null;
            text = "";
            try
            {
                text = getFile(FileName);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        public OkEx DelFile(string FileName)
        {
            CommException Error;
            if (DelFile(FileName, out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool DelFile(string FileName, out CommException Error)
        {
            Error = null;
            try
            {
                return delFile(FileName);
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx DelAllFiles()
        {
            CommException Error;
            if (DelAllFiles(out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool DelAllFiles(out CommException Error)
        {
            Error = null;
            try
            {
                return delAllFiles();
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public StringEx GetConfig()
        {
            string text;
            CommException Error;
            if (GetConfig(out text, out Error))
                return text;
            else
                return Error;
        }

        public bool GetConfig(out string text, out CommException Error)
        {
            text = "";
            Error = null;
            try
            {
                text = getConfig();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public OkEx SetConfig(string FileName)
        {
            CommException Error;
            if (SetConfig(FileName, out Error))
                return true;
            else
                return Error;
        }

        public bool SetConfig(string FileName, out CommException Error)
        {
            Error = null;
            try
            {
                setConfig(FileName);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public OkEx ResetConfig()
        {
            CommException Error;
            if (ResetConfig(out Error))
                return true;
            else
                return Error;
        }

        public bool ResetConfig(out CommException Error)
        {
            Error = null;
            try
            {
                if (!resetConfig())
                {
                    Error = new CommException("No permissions!");
                    return false;
                }
                else
                    return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        public OkEx CreateFactoryConfig()
        {
            CommException Error;
            if (CreateFactoryConfig(out Error))
                return true;
            else
                return Error;
        }

        public bool CreateFactoryConfig(out CommException Error)
        {
            Error = null;
            try
            {
                if (!createFactoryConfig())
                {
                    Error = new CommException("No permissions!");
                    return false;
                }
                else
                    return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        #endregion

        // ----- Firmware -----
        #region Firmware

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        public OkEx UpdateFirmware(string FileName)
        {
            CommException Error;
            if (UpdateFirmware(FileName, out Error))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool UpdateFirmware(string FileName, out CommException Error)
        {
            Error = null;
            try
            {
                updateFirmware(FileName);
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx RunApplication()
        {
            CommException Error;
            if (RunApplication(out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool RunApplication(out CommException Error)
        {
            Error = null;
            try
            {
                runApp();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx RunBootloader()
        {
            CommException Error;
            if (RunBootloader(out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool RunBootloader(out CommException Error)
        {
            Error = null;
            try
            {
                runBootloader();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx StayInBootloader()
        {
            CommException Error;
            if (StayInBootloader(out Error))
                return true;
            else
                return Error;
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool StayInBootloader(out CommException Error)
        {
            Error = null;
            try
            {
                stayInBootloader();
                return true;
            }
            catch (CommException err)
            {
                Error = err;
            }
            catch (Exception err)
            {
                Error = new CommException(err.Message, err);
            }
            return false;
        }
        #endregion



        #endregion

        #region Settings

        protected string language = "EN";

        protected DevMode mode = DevMode.Basic;


        #endregion


        #region Abstract

        // ----- Connection -----
        protected abstract void connect();
        protected abstract void disconnect();
        protected abstract bool isConnected();

        // ----- Get info -----
        protected abstract DeviceInfo getInfo();
        protected abstract string getXML();
        protected abstract List<DevMeasVals> getMeasurement();
        protected abstract List<DevParams> getDescription();

        // ----- Parameters -----
        protected abstract DevParamVals getParam(DevParamVals param);
        protected abstract List<DevParamVals> getParams(List<DevParamVals> param);
        protected abstract List<DevParams> getAllParams();
        protected abstract void setParam(int id, string param);
        protected abstract void setParams(List<DevParamVals> param);

        // ----- Files -----
        protected abstract string[] getDir();
        protected abstract string getFile(string fileName);
        protected abstract bool delFile(string fileName);
        protected abstract bool delAllFiles();


        // ----- Configuration -----
        protected abstract string getConfig();
        protected abstract void setConfig(string fileName);
        protected abstract bool resetConfig();
        protected abstract bool createFactoryConfig();

        // ----- Login -----
        protected abstract DevPermission login(string password);
        protected abstract DevPermission logout();
        protected abstract eChangePassReply changePass(string password);


       
        // ----- Firmware -----
        protected abstract void updateFirmware(string fileName);
        protected abstract void runApp();
        protected abstract void runBootloader();
        protected abstract void stayInBootloader();

        // ----- Status -----
        public abstract string[] GetStatusList(int code);
        public abstract string[] GetErrorList(int code);



        #endregion
    }

}
