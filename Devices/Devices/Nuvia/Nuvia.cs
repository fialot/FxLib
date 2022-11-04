using Fx.Conversion;
using Fx.IO;
using Fx.IO.Exceptions;
using Fx.IO.Protocols;
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

        Dictionary<int, string> lastMeas;                   // Last measurement (dictionary values)

        List<DevMeasVals> MeasList;                         // Measurement description list (from XML)
        List<DevParams> ParamList;                          // Parameter description list (from XML)
        List<DevParams> DescList;                         // Description list (from XML)

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

                // ----- Try Nuvia protocol -----

                for (int i = 0; i < autoSett.Count; i++)
                {
                    Settings = autoSett.Next(Settings);

                    try
                    {
                        nuvia.Connect(Settings);

                        try
                        {
                            nuvia.GetDevVersion();
                            UsedProtocol = eProtocol.Nuvia;
                            return;
                        } catch { }

                        nuvia.Disconnect();
                    }
                    catch { }
                }

                // ----- Try MODBUS protocol -----

                autoSett.Reset();

                for (int i = 0; i < autoSett.Count; i++)
                {
                    Settings = autoSett.Next(Settings);

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
            com.Close(); // nuvia.Disconnect();
        }

        /// <summary>
        /// Check if connected
        /// </summary>
        /// <returns></returns>
        protected override bool isConnected()
        {
            return com.IsOpen();  // nuvia.isConnected();
        }


        #endregion

        #region Get info

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <returns></returns>
        protected override DeviceInfo getInfo()
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
        protected override string getXML()
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
        protected override List<DevMeasVals> getMeasurement()
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

                if (MeasList == null) MeasList = CreateMeasList(getXML(), mode);     // Create Measurement list

                return FillMeas(MeasList, lastMeas);                           // Fill Measurement values
            }       
        }

        /// <summary>
        /// Get all description data
        /// </summary>
        /// <returns>Measurement list with description</returns>
        protected override List<DevParams> getDescription()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetDescription();
            }
            else
            {
                lastMeas = nuvia.GetDescription();

                if (DescList == null) DescList = CreateDescriptionList(getXML(), mode);     // Create Measurement list

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
        protected override DevParamVals getParam(DevParamVals param)
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
        protected override List<DevParamVals> getParams(List<DevParamVals> param)
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
        protected override List<DevParams> getAllParams()
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                return mbGetAllParams();
            }
            else
            {
                //if (ParamList == null)
                ParamList = CreateParamList(getXML(), mode, Permission);

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
        protected override void setParam(int id, string param)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                mbSetParam(id, param);
            }
            else
            {
                nuvia.SetParam(id.ToString() + "=" + param);
            }
            
        }

        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="param">Parameters list</param>
        protected override void setParams(List<DevParamVals> param)
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
        protected override string[] getDir()
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
        protected override string getFile(string fileName)
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
        protected override bool delFile(string fileName)
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
        protected override bool delAllFiles()
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
        protected override string getConfig()
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
        protected override void setConfig(string fileName)
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
        protected override bool resetConfig()
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
        protected override bool createFactoryConfig()
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

        protected override DevPermission login(string password)
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
        protected override DevPermission logout()
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
        protected override eChangePassReply changePass(string password)
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
        protected override void updateFirmware(string fileName)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                throw new CommException("Not supported command!");
            }
            else
            {
                byte[] bin;             // Binary file data
                int binIndex = 0;       // Binary index


                // ----- Clear process log -----
                /*ProcessLog.ClearMsg();
                ProcessLog.SetProgress(0);
                ProcessLog.SetName(Lng("firmwareUpdate", "Upload firmware file") + "...");

                ProcessLog.AddMsg(Lng("firmwareStartUpload", "Start updating firmware") + "...", false);
                ProcessLog.AddMsg("------------------------------------------------------------", false);*/

                // ----- Stay in bootloader -----
                nuvia.StayInBootloader();

                // ----- Read firmware file -----
                try
                {
                    bin = System.IO.File.ReadAllBytes(fileName);
                }
                catch (Exception err)
                {
                    /*ProcessLog.AddMsg(Lng("firmwareFileError", "Read firmware file error!"));
                    ProcessLog.SetProgress(0);
                    ProcessLog.SetName("");*/
                    throw new Exception(Lng("firmwareFileError", "Read firmware file error!"), err);
                }

                /*ProcessLog.AddMsg(Lng("firmwareReadFile", "Read bin file") + ": " + fileName + " ... " + bin.Length.ToString() + " " + Lng("firmwareBytes", "bytes"));*/

                // ----- Switch to bootloader -----
                //prot.SwitchToBootloader();

                // ----- Wait for switching -----
                //System.Threading.Thread.Sleep(10_000);

                // ----- Get device buff size -----
                int buffSize = (int)nuvia.GetBufferSize();
                if (buffSize <= 0)
                {
                    /*ProcessLog.AddMsg(Lng("firmwareBuffError", "Buff size is zero!"));
                    ProcessLog.SetProgress(0);
                    ProcessLog.SetName("");*/
                    throw new Exception(Lng("firmwareBuffError", "Buff size is zero!"));
                }

                // ----- Preprare data to sending -----
                byte[] sendBin = new byte[buffSize];
                int length = (bin.Length - binIndex);
                if (length > buffSize) length = buffSize;

                //List<byte> binDebug = new List<byte>();

                // ----- Sending firmware packets -----
                while (length > 0)
                {
                    // ----- Create packet & send -----
                    if (sendBin.Length != length)
                        sendBin = new byte[length];

                    Array.Copy(bin, binIndex, sendBin, 0, length);

                    // debug 
                    //for (int x = 0; x < length; x++)  binDebug.Add(sendBin[x]);



                    /*ProcessLog.AddMsg(Lng("firmwareSendData", "Sending data") + "... " + length.ToString() + " " + Lng("firmwareBytes", "bytes") + " ... (" + (binIndex + length).ToString() + "/" + bin.Length.ToString() + " " + Lng("firmwareBytes", "bytes") + ")");
                    ProcessLog.SetProgress(((binIndex + length) * 100) / bin.Length);*/


                    var crc = nuvia.STM32Checksum(sendBin, 0, sendBin.Length);
                    /*ProcessLog.AddMsg("CRC... 0x" + crc.ToString("X"));*/

                    // Debug
                    //ProcessLog.AddMsg("CRC2... 0x" + prot.STM32Checksum(binDebug.ToArray(), 0, binDebug.Count).ToString("X"));


                    nuvia.SendAppData(binIndex, bin.Length, sendBin);
                    binIndex += buffSize;

                    length = (bin.Length - binIndex);
                    if (length > buffSize) length = buffSize;
                }

                /*ProcessLog.SetProgress(100);
                ProcessLog.SetName(Lng("firmwareVerifing", "Verifyng firmware file") + "...");
                ProcessLog.AddMsg(Lng("firmwareSendingDone", "Sending data done") + "...");*/



                // ----- Check bin app CRC -----

                System.Threading.Thread.Sleep(1000);

                /*ProcessLog.AddMsg(Lng("firmwareCheckingCRC", "Checking CRC") + "...");*/

                var compureCRC = nuvia.STM32Checksum(bin);
                var CRC = nuvia.GetAppCRC(compureCRC);


                /*ProcessLog.SetProgress(0);
                ProcessLog.SetName("");*/

                if (CRC != compureCRC)
                {
                    /*ProcessLog.AddMsg(Lng("firmwareCRCFailed", "Checking CRC failed") + ": " + "0x" + CRC.ToString("X") + "/" + "0x" + compureCRC.ToString("X") + "!");
                    ProcessLog.AddMsg("------------------------------------------------------------", false);
                    ProcessLog.AddMsg(Lng("firmwareUpdateFailed", "Updating firmware failed") + ".", false);*/
                    throw new Exception(Lng("firmwareWrongCRC", "Wrong application CRC") + "!");
                }

                /*ProcessLog.AddMsg(Lng("firmwareCRCSuccess", "Checking CRC succesfull") + ".");
                ProcessLog.AddMsg("------------------------------------------------------------", false);
                ProcessLog.AddMsg(Lng("firmwareUpdateDone", "Uploading done, you can start application") + ".", false);*/


                // ----- Run app -----
                //prot.RunApp();

                // ----- Wait for switching -----
                //System.Threading.Thread.Sleep(10_000);
            }


        }


        /// <summary>
        /// Run application 
        /// (from bootloader)
        /// </summary>
        protected override void runApp()
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
                /*ProcessLog.SetName(Lng("bootStartApp", "Starting App") + "...");*/
                for (int i = 0; i < maxLen; i++)
                {
                    /*ProcessLog.SetProgress((i * 100) / maxLen);*/
                    System.Threading.Thread.Sleep(100);
                }
                /*ProcessLog.SetName("");

                ProcessLog.SetProgress(0);*/
            }
        }

        /// <summary>
        /// Run bootloader
        /// (from application)
        /// </summary>
        protected override void runBootloader()
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
                    /*ProcessLog.SetName(Lng("bootStartBoot", "Starting Bootloader") + "...");*/
                    for (int i = 0; i < maxLen; i++)
                    {
                        /*ProcessLog.SetProgress((i * 100) / maxLen);*/
                        System.Threading.Thread.Sleep(100);
                    }
                    /*ProcessLog.SetName("");
                    ProcessLog.SetProgress(0);*/

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
        protected override void stayInBootloader()
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


        protected override void refreshDevData()
        {

        }

        

        #region HV 

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        protected void setHV(int HV)
        {
            if (UsedProtocol == eProtocol.MODBUS)
            {
                setParam(32, HV.ToString());
            }
            else
            {
                setParam(13, HV.ToString());
            }
        }

        protected void setCalibHVPoint(byte domain, byte point, float voltage)
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

        protected void setCalibHV(byte domain)
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
        protected void start()
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
        protected void stop()
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
        protected void latch()
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
        protected void clear()
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
