using Fx.Conversion;
using Fx.IO;
using Fx.IO.Exceptions;
using Fx.IO.Protocols;
using Logger;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{

    enum eProtocol {Auto = 0, Nuvia = 1, MODBUS = 2 }


    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {

        #region Translation

        /// <summary>
        /// Czech Language of device
        /// </summary>
        static Dictionary<string, string> lang_CZ = new Dictionary<string, string>
        {
            // ----- Status -----
            {"measNotRun", "Měření neběží"},
            {"overloading", "Přetížení"},

            {"ROI1NotRun", "ROI1 neběží"},
            {"ROI2NotRun", "ROI2 neběží"},
            {"ROI3NotRun", "ROI3 neběží"},
            {"ROI4NotRun", "ROI4 neběží"},
            {"noHV", "HV není nahozeno"},
            {"isSD", "Vložena SD karta"},
            {"specRun", "Sbírání spektra"},

            {"standard", "Standard"},
            {"error", "Chyba"},

            // ----- Error -----
            {"noErr", "Zažízení funguje v pořádku"},
            {"errHV", "Chyba HV zdroje"},
            {"errDet1", "Chyba detektoru 1"},
            {"errDet2", "Chyba detektoru 2"},
            {"errDet3", "Chyba detektoru 3"},
            {"errDet4", "Chyba detektoru 4"},
            {"errTemp", "Chyba teplotního čidla"},
            {"errDev", "Chyba zařízení"},
            {"errRTC", "Chyba RTC"},
            {"errComm", "Chyba komunikace"},
            {"errInputV", "Chyba napájení"},

            { "firmwareUpdate","Aktualizace firmwaru" },
            { "firmwareStartUpload","Start aktualizace firmwaru" },
            { "firmwareFileError", "Chyba čtení souboru s firmwarem!" },
            { "firmwareBuffError", "Velikost bufferu je nulová!" },
            { "firmwareSendData", "Odesílání dat" },
            { "firmwareBytes", "bajtů" },
            { "firmwareReadFile", "Načten soubor"},
            { "firmwareVerifing", "Ověřování souboru s firmwarem"},
            { "firmwareSendingDone", "Odesílání dat dokončeno" },
            { "firmwareCRCFailed", "Selhala kontrola CRC" },
            { "firmwareUpdateFailed", "Aktualizace firmware selhala" },
            { "firmwareWrongCRC", "Chybné CRC aplikace" },
            { "firmwareCRCSuccess", "Kontrola CRC byla úspěšná" },
            { "firmwareUpdateDone", "Aktualizace byla úspěšná, můžete spustit aplikaci" },
            { "firmwareCheckingCRC", "Kontrola CRC" },


            { "bootStartApp", "Spouštění aplikace" },
            { "bootStartBoot", "Spouštění bootloaderu" }
        };

        /// <summary>
        /// Get device translation
        /// </summary>
        /// <param name="key">String key</param>
        /// <param name="def">Default text</param>
        /// <returns>Translation</returns>
        private string Lng(string key, string def)
        {
            if (language == "CZ")
            {
                try
                {
                    return lang_CZ[key];
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                return def;
            }

        }

        #endregion

        #region Variables

        Communication com = new Communication();
        eProtocol UsedProtocol = eProtocol.Auto;

        NuviaProtocol nuvia;          // Protocol Class
        ModbusProtocolExt mb;
        

        DeviceInfo info;                                       // Device Info
        GeigerSettings egmSettings;                            // Device Settings
        SCASettings mcaSettings;                            // Device Settings
        GeigerLimits egmLimits;                                // Measurement Limits


        #endregion
        
        // ----- Global device -----
        #region Connection

        /// <summary>
        /// Connection to default COM port
        /// </summary>
        protected override void connect()
        {
            // ----- If serial auto connect -----
            if (Settings.Type == ConnectionType.Serial && Settings.SerialPort.ToLower() == "auto")
            {
                Settings.StopBits = StopBits.One;
                Settings.DataBits = 8;
                Settings.Parity = Parity.None;

                SerialAutoConnect autoSett = new SerialAutoConnect();
                Settings = autoSett.Get(Settings);
                // ----- Try Nuvia protocol -----

                for (int i = 0; i < autoSett.Count; i++)
                {

                    try
                    {
                        nuvia.Connect(Settings);

                        try
                        {
                            nuvia.GetDevVersion();
                            UsedProtocol = eProtocol.Nuvia;
                            return;
                        } catch {

                            nuvia.Disconnect();

                            // ----- Try MODBUS protocol -----
                            try
                            {
                                mb.Connect(Settings);

                                try
                                {
                                    ushort[] regs = mb.ReadInputRegisters(1, 22);
                                    UsedProtocol = eProtocol.MODBUS;
                                    return;
                                }
                                catch { }

                                mb.Disconnect();
                            }
                            catch { }

                        }

                        
                    }
                    catch { }


                    Settings = autoSett.Next(Settings);
                }
                
                // ----- If not found device -> throw exception -----
                throw new ConnectionFailedException();

            }

            try
            {
                //prot.SetAddress((byte)address);
                nuvia.Connect(Settings);

                // ----- Check used protocol -----
                try
                {
                    nuvia.GetDevVersion();
                    UsedProtocol = eProtocol.Nuvia;
                }
                catch
                {
                    nuvia.Disconnect();
                    mb.Connect(Settings);

                    try
                    {
                        ushort[] regs = mb.ReadInputRegisters(1, 22);
                        UsedProtocol = eProtocol.MODBUS;
                    }
                    catch
                    {
                        mb.Disconnect();
                        nuvia.Connect(Settings);
                        UsedProtocol = eProtocol.Nuvia;
                    }
                }
            }
            catch (Exception err)
            {
                throw new ConnectionFailedException(err);
            }
        }
        
        /// <summary>
        /// Disconnect
        /// </summary>
        protected override void disconnect()
        {
            MeasList = null;
            ParamList = null;
            DescList = null;

            if (UsedProtocol == eProtocol.MODBUS)
                mb.Disconnect();
            else
                nuvia.Disconnect();
        }

        /// <summary>
        /// Check if connected
        /// </summary>
        /// <returns></returns>
        protected override bool isConnected()
        {
            if (UsedProtocol == eProtocol.MODBUS)
                return mb.isConnected();
            else
                return nuvia.isConnected();
        }


        #endregion

        #region Get info

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <returns></returns>
        protected override DeviceInfo devGetInfo()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetInfo();
            }
            else
            {
                return nuviaGetInfo();
            }
        }

        /// <summary>
        /// Get XML description
        /// </summary>
        /// <returns></returns>
        protected override string devGetXML()
        {

            byte lng;
            if (language == "CZ") lng = 1;
            else lng = 2;

            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetXML(lng);
            }
            else
            {
                return nuvia.GetXML(lng);
            }

            
        }

        /// <summary>
        /// Get all measurement data
        /// </summary>
        /// <returns>Measurement list with description</returns>
        protected override List<DevMeasVals> devGetMeasurement()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                if (Type == DeviceType.EGM)
                    return mbGetEGMMeasurement();
                else
                    return mbGetMCAMeasurement();
            } 
            else
            {
                lastMeas = nuvia.GetMeasurement();

                if (MeasList == null) MeasList = CreateMeasList(devGetXML(), mode);     // Create Measurement list
                
                return FillMeas(MeasList, lastMeas);                           // Fill Measurement values
            }       
        }

        /// <summary>
        /// Get all description data
        /// </summary>
        /// <returns>Measurement list with description</returns>
        protected override List<DevParams> devGetDescription()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetDescription();
            }
            else
            {
                lastMeas = nuvia.GetDescription();

                if (DescList == null) DescList = CreateDescriptionList(devGetXML(), mode);     // Create Measurement list

                return FillParam(DescList, lastMeas);                           // Fill Measurement values
            }
        }

        #endregion

        #region Parameters

        /// <summary>
        /// Get device parameter
        /// </summary>
        /// <param name="param">Parameter ID</param>
        /// <returns>Parameter</returns>
        protected override DevParamVals devGetParam(DevParamVals param)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetParam(param);
            }
            else
            {
                List<DevParamVals> parList;
                string reply = nuvia.GetParam(param.ID.ToString());
                parList = nuvia.ParseParamToList(reply);
                return parList[0];
            }
            
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="param">Parameters list</param>
        /// <returns>Parameters list</returns>
        protected override List<DevParamVals> devGetParams(List<DevParamVals> param)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetParams(param);
            }
            else
            {
                List<DevParamVals> parList = new List<DevParamVals>();
                if (param != null && param.Count > 0)
                {
                    string request = "";
                    foreach (var item in param)
                    {
                        if (request.Length > 0) request += ",";
                        request += item.ID.ToString();
                    }
                    string reply = nuvia.GetParam(request);
                    parList = nuvia.ParseParamToList(reply);
                }
                return parList;
            }
            
        }

        /// <summary>
        /// Get All device parameter with description
        /// </summary>
        /// <returns>Parameter list</returns>
        protected override List<DevParams> devGetAllParams()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetAllParams();
            }
            else
            {
                //if (ParamList == null)
                ParamList = CreateParamList(devGetXML(), mode, Permission);

                List<DevParams> list = new List<DevParams>();
                string reply = nuvia.GetParam("0");

                Dictionary<int, string> dict = nuvia.ParseParam(reply);

                foreach (var item in ParamList)
                {
                    try
                    {
                        item.Value = dict[item.ID];
                        list.Add(item);
                    }
                    catch { }
                }
                return list;
            }
            
        }

        /// <summary>
        /// Set parameter
        /// </summary>
        /// <param name="id">Parameter ID</param>
        /// <param name="param">Parameter value</param>
        protected override void devSetParam(DevParamVals param)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                try
                {
                    mbSetParam(param.ID, param.Value);
                }
                catch (Exception error)
                {
                    disconnect();
                    connect();
                    // If not change protocol -> throw error
                    if (param.ID != 2)
                        throw error;
                }
            }
            else
            {
                try
                {
                    nuvia.SetParam(param.ID.ToString() + "=" + param.Value);
                }
                catch (Exception error)
                {
                    disconnect();
                    connect();
                    // If not change protocol -> throw error
                    if (param.ID != 61)
                        throw error;
                } 
            }
            
        }

        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="param">Parameters list</param>
        protected override void devSetParams(List<DevParamVals> param)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mbSetParams(param);
            }
            else
            {
                if (param != null && param.Count > 0)
                {
                    string request = "";
                    foreach (var item in param)
                    {
                        if (request.Length > 0) request += ",";
                        request += item.ID.ToString() + "=" + item.Value;
                    }
                    nuvia.SetParam(request);
                }
            }
        }

        #endregion
        
        #region Files

        /// <summary>
        /// Get directory (file list)
        /// </summary>
        /// <returns>Returns File list</returns>
        protected override string[] devGetDir()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.GetDir();
            }
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Returns File</returns>
        protected override string devGetFile(string fileName)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.GetFile(fileName);
            }
        }


        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override bool devDelFile(string fileName)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.DelFile(fileName);
            }
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override bool devDelAllFiles()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.DelAllFiles();
            }
            
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Download Config from device
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override string devGetConfig()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                string text = "";
                try
                {
                    text += nuvia.GetConfig(0);
                }
                catch { }
                try
                {
                    text += nuvia.GetConfig(1);
                }
                catch { }
                try
                {
                    text += nuvia.GetConfig(2);
                }
                catch { }

                text = text.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");
                text = text.Replace("<config>\n", "");
                text = text.Replace("</config>\n", "");

                text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<config>\n" + text + "</config>\n";

                return text;
            }
        }

        /// <summary>
        /// Update Config from file
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override void devSetConfig(string fileName)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                string text = "";
                try
                {
                    text = System.IO.File.ReadAllText(fileName);
                }
                catch (Exception)
                {
                    throw new CommException("Bad file!");
                }

                nuvia.SetConfig(text);
            }
        }

        /// <summary>
        /// Reset Config to factory default
        /// </summary>
        /// <returns>Returns true if succesfully reset</returns>
        protected override bool devResetConfig()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.ResetConfig();
            }
        }

        /// <summary>
        /// Create factory config
        /// </summary>
        /// <returns></returns>
        protected override bool devCreateFactoryConfig()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.CreateFactoryConfig();
            }
        }

        #endregion

        #region Login

        protected override DevPermission devLogin(string password)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.Login(password);
            }
            
        }
        protected override DevPermission devLogout()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                return nuvia.Logout();
            }
        }
        protected override eChangePassReply devChangePass(string password)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                try
                {
                    nuvia.ChangePass(password);

                }
                catch (Exception Err)
                {
                    if (Err.Message.Contains("Bad password length")) return eChangePassReply.BadLength;
                    else if (Err.Message.Contains("No permission")) return eChangePassReply.NoPermissions;
                    else throw Err;
                }

                return eChangePassReply.OK;
            }
        }

        #endregion

        #region Firmware

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="fileName">Firmware file name</param>
        protected override void devUpdateFirmware(string fileName)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                byte[] bin;             // Binary file data
                int binIndex = 0;       // Binary index


                setLogTitle(Lng("firmwareUpdate", "Upload firmware file") + "...");

                log(Lng("firmwareStartUpload", "Start updating firmware") + "..." + Environment.NewLine, 0);
                log("------------------------------------------------------------" + Environment.NewLine);

                // ----- Stay in bootloader -----
                nuvia.StayInBootloader();

                // ----- Read firmware file -----
                try
                {
                    bin = System.IO.File.ReadAllBytes(fileName);
                }
                catch (Exception err)
                {
                    setLogTitle("");
                    log(Lng("firmwareFileError", "Read firmware file error!") + Environment.NewLine, 0);
                    throw new Exception(Lng("firmwareFileError", "Read firmware file error!"), err);
                }

                log(Lng("firmwareReadFile", "Read bin file") + ": " + fileName + " ... " + bin.Length.ToString() + " " + Lng("firmwareBytes", "bytes") + Environment.NewLine);
                
                // ----- Get device buff size -----
                int buffSize = (int)nuvia.GetBufferSize();
                if (buffSize <= 0)
                {
                    setLogTitle("");
                    log(Lng("firmwareBuffError", "Buff size is zero!") + Environment.NewLine, 0);
                    throw new Exception(Lng("firmwareBuffError", "Buff size is zero!"));
                }

                // ----- Preprare data to sending -----
                byte[] sendBin = new byte[buffSize];
                int length = (bin.Length - binIndex);
                if (length > buffSize) length = buffSize;

                // ----- Sending firmware packets -----
                while (length > 0)
                {
                    // ----- Create packet & send -----
                    if (sendBin.Length != length)
                        sendBin = new byte[length];

                    Array.Copy(bin, binIndex, sendBin, 0, length);


                    log(Lng("firmwareSendData", "Sending data") + "... " + length.ToString() + " " 
                        + Lng("firmwareBytes", "bytes") + " ... (" + (binIndex + length).ToString() + "/" + bin.Length.ToString() + " " + Lng("firmwareBytes", "bytes") + ")" + Environment.NewLine,
                        ((binIndex + length) * 100) / bin.Length);


                    var crc = nuvia.STM32Checksum(sendBin, 0, sendBin.Length);
                    log("CRC... 0x" + crc.ToString("X") + Environment.NewLine);

                    // Debug
                    //log("CRC2... 0x" + nuvia.STM32Checksum(binDebug.ToArray(), 0, binDebug.Count).ToString("X"));


                    nuvia.SendAppData(binIndex, bin.Length, sendBin);
                    binIndex += buffSize;

                    length = (bin.Length - binIndex);
                    if (length > buffSize) length = buffSize;
                }

                setLogTitle(Lng("firmwareVerifing", "Verifyng firmware file") + "...");
                log(Lng("firmwareSendingDone", "Sending data done") + "..." + Environment.NewLine, 100);



                // ----- Check bin app CRC -----

                System.Threading.Thread.Sleep(1000);

                log(Lng("firmwareCheckingCRC", "Checking CRC") + "..." + Environment.NewLine);

                var compureCRC = nuvia.STM32Checksum(bin);
                var CRC = nuvia.GetAppCRC(compureCRC);

                setLogTitle("");
                log("", 0);


                if (CRC != compureCRC)
                {
                    log(Lng("firmwareCRCFailed", "Checking CRC failed") + ": " + "0x" + CRC.ToString("X") + "/" + "0x" + compureCRC.ToString("X") + "!" + Environment.NewLine);
                    log("------------------------------------------------------------" + Environment.NewLine);
                    log(Lng("firmwareUpdateFailed", "Updating firmware failed") + "." + Environment.NewLine);
                    throw new Exception(Lng("firmwareWrongCRC", "Wrong application CRC") + "!");
                }

                log(Lng("firmwareCRCSuccess", "Checking CRC succesfull") + "." + Environment.NewLine);
                log("------------------------------------------------------------" + Environment.NewLine);
                log(Lng("firmwareUpdateDone", "Uploading done, you can start application") + "." + Environment.NewLine);

            }

        }


        /// <summary>
        /// Run application 
        /// (from bootloader)
        /// </summary>
        protected override void devRunApp()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                // ----- Run app -----
                nuvia.RunApp();

                // ----- Wait for switching -----
                int maxLen = 30;
                setLogTitle(Lng("bootStartApp", "Starting App"));
                for (int i = 0; i < maxLen; i++)
                {
                    log("", (i * 100) / maxLen);
                    System.Threading.Thread.Sleep(100);
                }
                setLogTitle("");
                log("", 0);
            }
        }

        /// <summary>
        /// Run bootloader
        /// (from application)
        /// </summary>
        protected override void devRunBootloader()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(100, true);
            }
            else
            {
                // ----- Run bootloader -----
                if (nuvia.SwitchToBootloader())
                {
                    // ----- Wait for switching -----
                    int maxLen = 30;
                    setLogTitle(Lng("bootStartBoot", "Starting Bootloader") + "...");
                    for (int i = 0; i < maxLen; i++)
                    {
                        log("", (i * 100) / maxLen);
                        System.Threading.Thread.Sleep(100);
                    }
                    setLogTitle("");
                    log("", 0);

                }
                else
                {
                    throw new CommException("No permission!");
                }
            }
            
        }

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        protected override void devStayInBootloader()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                // ----- Run bootloader -----
                nuvia.StayInBootloader();
            }
        }


        #endregion



        

        #region HV 

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        protected void devSetHV(int HV)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                devSetParam(new DevParamVals(32, HV.ToString()));
            }
            else
            {
                devSetParam(new DevParamVals(13, HV.ToString()));
            }
        }

        protected void devSetCalibHVPoint(byte domain, byte point, float voltage)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                if (!nuvia.CalibrationHVSetPoint(domain, point, voltage))
                    throw new CommException("Can't set HV point!");
            }
            
        }

        protected void devSetCalibHV(byte domain)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                if (!nuvia.CalibrationHVSet(domain))
                    throw new CommException("Can't set HV calibration!");
            }
           
        }


        #endregion

        #region Measurement

        /// <summary>
        /// Start measuring
        /// </summary>
        protected void devStart()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(6, true);
            }
            else
            {
                nuvia.StartROIs();
            }
        }

        /// <summary>
        /// Stop Measuring
        /// </summary>
        protected void devStop()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(6, false);
            }
            else
            {
                nuvia.StopROIs();
            }
        }

        /// <summary>
        /// Stop Measuring
        /// </summary>
        protected void devLatch()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(8, true);
            }
            else
            {
                nuvia.LatchROIs();
            }
        }

        /// <summary>
        /// Clear Measuring
        /// </summary>
        protected void devClear()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mb.WriteCoil(7, true);
            }
            else
            {
                nuvia.ClearROIs();
            }
            
        }

        #endregion


    }

}
