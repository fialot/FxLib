using Fx.Conversion;
using Fx.IO;
using Fx.IO.Exceptions;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fx.Devices
{
    public delegate void NewLogEventHandler(object source, string title, string log, int progress);

    public abstract partial class Device : IDevice
    {

        #region Public

        public event NewLogEventHandler NewLog = null;

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

            SetLanguage(System.Globalization.CultureInfo.CurrentUICulture.Name);

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
            try
            {
                connect();
                return true;
            }
            catch (Exception err)
            {
                return new ConnectionFailedException(err);
            }
        }

        /// <summary>
        /// Connect to device
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if connection ok</returns>
        public bool Connect(out CommException Error)
        {
            CommException error = null;
            var reply = Connect().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /*public bool Connect(ConnectionSetting settings)
        {
            return Connect(settings, out Exception Error);
        }*/

        public OkEx Connect(ConnectionSetting settings)
        {
            try
            {
                Settings = settings;
                connect();
                return true;
            }
            catch (Exception err)
            {
                return new ConnectionFailedException(err);
            }
        }

        public bool Connect(ConnectionSetting settings, out CommException Error)
        {
            CommException error = null;
            var reply = Connect(settings).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }


        /// <summary>
        /// Disconnect device
        /// </summary>
        /// <returns>Returns true if connection ok</returns>
        public OkEx Disconnect()
        {
            setLogTitle("");
            log("", 0);

            try
            {
                disconnect();
                return true;
            }
            catch (Exception err)
            {
                return new CommException(err.Message);
            }
        }

        /// <summary>
        /// Disconnect device
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if connection ok</returns>
        public bool Disconnect(out CommException Error)
        {
            CommException error = null;
            var reply = Disconnect().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Reconnect device
        /// </summary>
        /// <returns>Returns true if connection ok</returns>
        public OkEx Reconnect()
        {
            try
            {
                disconnect();
                connect();
                return true;
            }
            catch (Exception err)
            {
                return new CommException(err.Message);
            }
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
        /// <returns>Returns true if read ok</returns>
        public DeviceInfoEx GetInfo()
        {
            if (!RunningMeasurement)
                return getInfo();
            else
                return requestGetInfo();
        }

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <param name="Value">Device Info</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetInfo(out DeviceInfo Value, out CommException Error)
        {
            CommException error = null;
            DeviceInfo value = new DeviceInfo();
            var reply = GetInfo().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }



        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <param name="XML">XML string</param>
        /// <returns>Returns true if read ok</returns>
        public StringEx GetXMLDesc()
        {
            if (!RunningMeasurement)
                return getXMLDesc();
            else
                return requestGetXMLDesc();
        }

        /// <summary>
        /// Get device XML description
        /// </summary>
        /// <param name="XML">XML description</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetXMLDesc(out string XML, out CommException Error)
        {
            CommException error = null;
            string value = "";
            var reply = GetXMLDesc().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            XML = value;
            return reply;
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
        /// <returns>Returns device description</returns>
        public DevParamsEx GetDescription()
        {
            if (!RunningMeasurement)
                return getDescription();
            else
                return requestGetDescription();
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetDescription(out List<DevParams> Value, out CommException Error)
        {
            CommException error = null;
            List<DevParams> value = new List<DevParams>();
            var reply = GetDescription().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
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

                    if (value.Name == "enum")
                    {
                        IEnumerable<XElement> enums = value.Elements();
                        item.Replace = new Dictionary<string, string>();

                        foreach (var en in enums)
                        {
                            if ((en.Attribute("name") != null) && (en.Attribute("name") != null))
                            {
                                item.Replace.Add(en.Attribute("value").Value, en.Attribute("name").Value);
                            }
                        }
                    }

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
                    if (item.Replace != null)
                    {
                        if (item.Replace.ContainsKey(item.Value))
                            item.Value = item.Replace[item.Value];
                    }
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
        /// <returns>Returns Measurement</returns>
        public DevMeasValsEx GetMeasurement()
        {
            if (!RunningMeasurement)
                return getMeasurement();
            else
                return requestGetMeasurement();
        }

        /// <summary>
        /// Get measurement
        /// </summary>
        /// <param name="Value">Measurement</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetMeasurement(out List<DevMeasVals> Value, out CommException Error)
        {
            CommException error = null;
            List<DevMeasVals> value = new List<DevMeasVals>();
            var reply = GetMeasurement().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Value = value;
            return reply;
        }

        public List<DevMeasVals> ReadMeasurement()
        {
            return FillMeas(MeasList, lastMeas);
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
                            Support |= DevSupport.Start;
                            Support |= DevSupport.Bootloader;
                        }
                    }

                    attr = fw.Attribute("has_spect");
                    if (attr != null)
                    {
                        if (attr.Value == "1")
                        {
                            Support |= DevSupport.Spectrum;
                        }
                        else
                        {
                            Support &= ~DevSupport.Spectrum;
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
            return GetParam(new DevParamVals(ID, ""));
        }
            

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <returns>Returns Device parameter values</returns>
        public DevParamValueEx GetParam(DevParamVals Param)
        {
            if (!RunningMeasurement)
                return getParam(Param);
            else
                return requestGetParam(Param);
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetParam(ref DevParamVals Param, out CommException Error)
        {
            CommException error = null;
            DevParamVals value = new DevParamVals();
            var reply = GetParam(Param).Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Param = value;
            return reply;
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <returns>Returns parameter list</returns>
        public DevParamValuesEx GetParams(List<DevParamVals> Param)
        {
            if (!RunningMeasurement)
                return getParams(Param);
            else
                return requestGetParams(Param);
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetParams(ref List<DevParamVals> Param, out CommException Error)
        {
            CommException error = null;
            List<DevParamVals> value = new List<DevParamVals>();
            var reply = GetParams(Param).Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Param = value;
            return reply;
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <returns>Returns parameter list</returns>
        public DevParamsEx GetAllParams()
        {
            if (!RunningMeasurement)
                return getAllParams();
            else
                return requestGetAllParams();
        }

        /// <summary>
        /// Get all device parameters
        /// </summary>
        /// <param name="Param">Parameter list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetAllParams(out List<DevParams> Param, out CommException Error)
        {
            CommException error = null;
            List<DevParams> value = new List<DevParams>();
            var reply = GetAllParams().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            Param = value;
            return reply;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="ID">Parameter ID</param>
        /// <param name="Param">Parameter Value</param>
        /// <returns>Return true if ok</returns>
        public OkEx SetParam(int ID, string Value)
        {
            return SetParam(new DevParamVals(ID, Value));
        }

        public OkEx SetParam(DevParamVals Param)
        {
            if (!RunningMeasurement)
                return setParam(Param);
            else
                return requestSetParam(Param);
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
            CommException error = null;
            var reply = SetParam(ID, Value).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <returns>Returns true if write ok</returns>
        public OkEx SetParams(List<DevParamVals> Param)
        {
            if (!RunningMeasurement)
                return setParams(Param);
            else
                return requestSetParams(Param);
        }

        /// <summary>
        /// Set device parameters
        /// </summary>
        /// <param name="Param">Parameters</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if write ok</returns>
        public bool SetParams(List<DevParamVals> Param, out CommException Error)
        {
            CommException error = null;
            var reply = SetParams(Param).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Login function
        /// </summary>
        /// <param name="password">Passowrd</param>
        /// <returns>Return actual permissions</returns>
        public PermissionEx Login(string password)
        {
            if (!RunningMeasurement)
                return login(password);
            else
                return requestLogin(password);
        }

        public bool Login(string password, out DevPermission permission, out CommException Error)
        {
            CommException error = null;
            DevPermission value = DevPermission.None;
            var reply = Login(password).Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            permission = value;
            return reply;
        }

        /// <summary>
        /// Logout function
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public PermissionEx Logout()
        {
            if (!RunningMeasurement)
                return logout();
            else
                return requestLogout();
        }

        public bool Logout(out DevPermission permission, out CommException Error)
        {
            CommException error = null;

            DevPermission value = DevPermission.None;
            var reply = Logout().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            permission = value;
            return reply;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>Return true if ok</returns>
        public OkEx ChangePassword(string password)
        {
            if (!RunningMeasurement)
                return changePassword(password);
            else
                return requestChangePassword(password);
        }

        public bool ChangePassword(string password, out CommException Error)
        {
            CommException error = null;
            var reply = ChangePassword(password).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        #endregion

        // ----- Files -----
        #region Files

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <returns>Returns file list if read ok</returns>
        public StringArrayEx GetDir()
        {
            if (!RunningMeasurement)
                return getDir();
            else
                return requestGetDir();
        }

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <param name="FileList">File list</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if read ok</returns>
        public bool GetDir(out string[] FileList, out CommException Error)
        {
            CommException error = null;
            string[] value = new string[0];
            var reply = GetDir().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            FileList = value;
            return reply;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns file if ok</returns>
        public StringEx GetFile(string FileName)
        {
            if (!RunningMeasurement)
                return getFile(FileName);
            else
                return requestGetFile(FileName);
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
            CommException error = null;
            string value = "";
            var reply = GetFile(FileName).Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            text = value;
            return reply;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if ok</returns>
        public OkEx DelFile(string FileName)
        {
            if (!RunningMeasurement)
                return delFile(FileName);
            else
                return requestDelFile(FileName);
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool DelFile(string FileName, out CommException Error)
        {
            CommException error = null;
            var reply = DelFile(FileName).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx DelAllFiles()
        {
            if (!RunningMeasurement)
                return delAllFiles();
            else
                return requestDelAllFiles();
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool DelAllFiles(out CommException Error)
        {
            CommException error = null;
            var reply = DelAllFiles().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Get configuration XML
        /// </summary>
        /// <returns></returns>
        public StringEx GetConfig()
        {
            if (!RunningMeasurement)
                return getConfig();
            else
                return requestGetConfig();
        }

        public bool GetConfig(out string text, out CommException Error)
        {
            CommException error = null;
            string value = "";
            var reply = GetConfig().Match(ok => { value = ok; return true; }, err => { error = err; return false; });
            Error = error;
            text = value;
            return reply;
        }

        /// <summary>
        /// Set configuration XML
        /// </summary>
        /// <param name="FileName">Path to configuration XML file</param>
        /// <returns>Return true if OK</returns>
        public OkEx SetConfig(string FileName)
        {
            if (!RunningMeasurement)
                return setConfig(FileName);
            else
                return requestSetConfig(FileName);
        }

        public bool SetConfig(string FileName, out CommException Error)
        {
            CommException error = null;
            var reply = SetConfig(FileName).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Reset configuration to Facroty settings
        /// </summary>
        /// <returns>Return true if OK</returns>
        public OkEx ResetConfig()
        {
            if (!RunningMeasurement)
                return resetConfig();
            else
                return requestResetConfig();
        }

        public bool ResetConfig(out CommException Error)
        {
            CommException error = null;
            var reply = ResetConfig().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Create factory configuration
        /// </summary>
        /// <returns>Return true if OK</returns>
        public OkEx CreateFactoryConfig()
        {
            if (!RunningMeasurement)
                return createFactoryConfig();
            else
                return requestCreateFactoryConfig();
        }

        public bool CreateFactoryConfig(out CommException Error)
        {
            CommException error = null;
            var reply = CreateFactoryConfig().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
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
            if (!RunningMeasurement)
                return updateFirmware(FileName);
            else
                return requestUpdateFirmware(FileName);
        }

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool UpdateFirmware(string FileName, out CommException Error)
        {
            CommException error = null;
            var reply = UpdateFirmware(FileName).Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx RunApplication()
        {
            if (!RunningMeasurement)
                return runApplication();
            else
                return requestRunApplication();
        }

        /// <summary>
        /// Run Application
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool RunApplication(out CommException Error)
        {
            CommException error = null;
            var reply = RunApplication().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx RunBootloader()
        {
            if (!RunningMeasurement)
                return runBootloader();
            else
                return requestRunBootloader();
        }

        /// <summary>
        /// Run Bootloader
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool RunBootloader(out CommException Error)
        {
            CommException error = null;
            var reply = RunBootloader().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <returns>Returns true if ok</returns>
        public OkEx StayInBootloader()
        {
            if (!RunningMeasurement)
                return stayInBootloader();
            else
                return requestStayInBootloader();
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        /// <param name="Error">Error</param>
        /// <returns>Returns true if ok</returns>
        public bool StayInBootloader(out CommException Error)
        {
            CommException error = null;
            var reply = StayInBootloader().Match(ok => true, err => { error = err; return false; });
            Error = error;
            return reply;
        }
        #endregion



        #endregion

        #region Settings

        protected string language = "EN";

        protected DevMode mode = DevMode.Basic;

        protected List<DevMeasVals> MeasList;                         // Measurement description list (from XML)
        protected List<DevParams> ParamList;                          // Parameter description list (from XML)
        protected List<DevParams> DescList;                         // Description list (from XML)

        protected Dictionary<int, string> lastMeas;                   // Last measurement (dictionary values)


        #endregion


        #region Abstract

        // ----- Connection -----
        protected abstract void connect();
        protected abstract void disconnect();
        protected abstract bool isConnected();

        // ----- Get info -----
        protected abstract DeviceInfo devGetInfo();
        protected abstract string devGetXML();
        protected abstract List<DevMeasVals> devGetMeasurement();
        protected abstract List<DevParams> devGetDescription();

        // ----- Parameters -----
        protected abstract DevParamVals devGetParam(DevParamVals param);
        protected abstract List<DevParamVals> devGetParams(List<DevParamVals> param);
        protected abstract List<DevParams> devGetAllParams();
        protected abstract void devSetParam(DevParamVals param);
        protected abstract void devSetParams(List<DevParamVals> param);

        // ----- Files -----
        protected abstract string[] devGetDir();
        protected abstract string devGetFile(string fileName);
        protected abstract bool devDelFile(string fileName);
        protected abstract bool devDelAllFiles();


        // ----- Configuration -----
        protected abstract string devGetConfig();
        protected abstract void devSetConfig(string fileName);
        protected abstract bool devResetConfig();
        protected abstract bool devCreateFactoryConfig();

        // ----- Login -----
        protected abstract DevPermission devLogin(string password);
        protected abstract DevPermission devLogout();
        protected abstract eChangePassReply devChangePass(string password);


       
        // ----- Firmware -----
        protected abstract void devUpdateFirmware(string fileName);
        protected abstract void devRunApp();
        protected abstract void devRunBootloader();
        protected abstract void devStayInBootloader();

        // ----- Status -----
        public abstract string[] GetStatusList(int code);
        public abstract string[] GetErrorList(int code);



        #endregion
    }

}
