﻿using Fx.Conversion;
using Fx.IO;
using Fx.IO.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Devices
{
    


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
            {"standard", "Standard"},
            {"error", "Chyba"},

            // ----- Error -----
            {"errHV", "Chyba HV zdroje"},
            {"errDet1", "Chyba detektoru 1"},
            {"errDet2", "Chyba detektoru 2"},
            {"errDet3", "Chyba detektoru 3"},
            {"errDet4", "Chyba detektoru 4"},
            {"errTemp", "Chyba teplotního čidla"},
            {"errDev", "Chyba zařízení"},

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
        NuviaProtocol prot;          // Protocol Class

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
            //prot.SetAddress((byte)address);
            prot.Connect(Settings);
        }
        
        /// <summary>
        /// Disconnect
        /// </summary>
        protected override void disconnect()
        {
            prot.Disconnect();
        }

        /// <summary>
        /// Check if connected
        /// </summary>
        /// <returns></returns>
        protected override bool isConnected()
        {
            return prot.isConnected();
        }


        #endregion

        #region Get info

        /// <summary>
        /// Get Device Info
        /// </summary>
        /// <returns></returns>
        protected override DeviceInfo getInfo()
        {
            info = new DeviceInfo();
            info.Version = prot.GetDevVersion();                                // get FW version
            try
            {
                info.SN = prot.ParseParam(prot.GetParam("12"))[12];             // get SN
                info.Model = prot.ParseParam(prot.GetParam("10027"))[10027];    // get Model
                info.Date = "";

                if (info.Version.Contains("EGM") || info.Version.Contains("GMS"))
                    Type = DeviceType.EGM;
                else if (info.Version.Contains("SCA") || info.Version.Contains("MCB"))
                {
                    Type = DeviceType.MCA;
                    Support |= DevSupport.Spectrum;
                }
                    

                info.Type = Type;

                Support |= DevSupport.Firmware;

                var perm = prot.ParseParam(prot.GetParam("10105"))[10105];    // get Model

                if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;

                Support |= DevSupport.Permission;
            }

            catch { }

            return info;
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

            return prot.GetXML(lng);
        }

        /// <summary>
        /// Get all measurement data
        /// </summary>
        /// <returns>Measurement list with description</returns>
        protected override List<DevMeasVals> getMeasurement()
        {
            lastMeas = prot.GetMeasurement();

            if (MeasList == null) MeasList = CreateMeasList(getXML(), mode);     // Create Measurement list

            return FillMeas(MeasList, lastMeas);                           // Fill Measurement values
        }

        /// <summary>
        /// Get all description data
        /// </summary>
        /// <returns>Measurement list with description</returns>
        protected override List<DevParams> getDescription()
        {
            lastMeas = prot.GetDescription();

            if (DescList == null) DescList = CreateDescriptionList(getXML(), mode);     // Create Measurement list

            return FillParam(DescList, lastMeas);                           // Fill Measurement values
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
            List<DevParamVals> parList;
            string reply = prot.GetParam(param.ID.ToString());
            parList = prot.ParseParamToList(reply);
            return parList[0];
        }

        /// <summary>
        /// Get device parameters
        /// </summary>
        /// <param name="param">Parameters list</param>
        /// <returns>Parameters list</returns>
        protected override List<DevParamVals> getParams(List<DevParamVals> param)
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
                string reply = prot.GetParam(request);
                parList = prot.ParseParamToList(reply);
            }
            return parList;
        }

        /// <summary>
        /// Get All device parameter with description
        /// </summary>
        /// <returns>Parameter list</returns>
        protected override List<DevParams> getAllParams()
        {
            //if (ParamList == null)
            ParamList = CreateParamList(getXML(), mode, Permission);

            List<DevParams> list = new List<DevParams>();
            string reply = prot.GetParam("0");

            Dictionary<int, string> dict = prot.ParseParam(reply);

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

        /// <summary>
        /// Set parameter
        /// </summary>
        /// <param name="id">Parameter ID</param>
        /// <param name="param">Parameter value</param>
        protected override void setParam(int id, string param)
        {
            prot.SetParam(id.ToString() + "=" + param);
        }

        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="param">Parameters list</param>
        protected override void setParams(List<DevParamVals> param)
        {
            if (param != null && param.Count > 0)
            {
                string request = "";
                foreach (var item in param)
                {
                    if (request.Length > 0) request += ",";
                    request += item.ID.ToString() + "=" + item.Value;
                }
                prot.SetParam(request);
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
            return prot.GetDir();
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Returns File</returns>
        protected override string getFile(string fileName)
        {
            return prot.GetFile(fileName);
        }


        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override bool delFile(string fileName)
        {
            return prot.DelFile(fileName);
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override bool delAllFiles()
        {
            return prot.DelAllFiles();
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Download Config from device
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override string getConfig()
        {
            string text = "";
            try
            {
                text += prot.GetConfig(0);
            }
            catch { }
            try
            {
                text += prot.GetConfig(1);
            }
            catch { }
            try
            {
                text += prot.GetConfig(2);
            }
            catch { }

            text = text.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");
            text = text.Replace("<config>\n", "");
            text = text.Replace("</config>\n", "");

            text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<config>\n" + text + "</config>\n";

            return text;
        }

        /// <summary>
        /// Update Config from file
        /// </summary>
        /// <returns>Returns true if succesfully deleted</returns>
        protected override void setConfig(string fileName)
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

            prot.SetConfig(text);
        }

        /// <summary>
        /// Reset Config to factory default
        /// </summary>
        /// <returns>Returns true if succesfully reset</returns>
        protected override bool resetConfig()
        {
            return prot.ResetConfig();
        }

        /// <summary>
        /// Create factory config
        /// </summary>
        /// <returns></returns>
        protected override bool createFactoryConfig()
        {
            return prot.CreateFactoryConfig();
        }

        #endregion

        #region Login

        protected override DevPermission login(string password)
        {
            return prot.Login(password);
        }
        protected override DevPermission logout()
        {
            return prot.Logout();
        }
        protected override eChangePassReply changePass(string password)
        {
            try
            {
                prot.ChangePass(password);

            }
            catch (Exception Err)
            {
                if (Err.Message.Contains("Bad password length")) return eChangePassReply.BadLength;
                else if (Err.Message.Contains("No permission")) return eChangePassReply.NoPermissions;
                else throw Err;
            }

            return eChangePassReply.OK;
        }

        #endregion

        #region Firmware

        /// <summary>
        /// Update firmware
        /// </summary>
        /// <param name="fileName">Firmware file name</param>
        protected override void updateFirmware(string fileName)
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
            prot.StayInBootloader();

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
            int buffSize = (int)prot.GetBufferSize();
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


                var crc = prot.STM32Checksum(sendBin, 0, sendBin.Length);
                /*ProcessLog.AddMsg("CRC... 0x" + crc.ToString("X"));*/

                // Debug
                //ProcessLog.AddMsg("CRC2... 0x" + prot.STM32Checksum(binDebug.ToArray(), 0, binDebug.Count).ToString("X"));


                prot.SendAppData(binIndex, bin.Length, sendBin);
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

            var compureCRC = prot.STM32Checksum(bin);
            var CRC = prot.GetAppCRC(compureCRC);


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


        /// <summary>
        /// Run application 
        /// (from bootloader)
        /// </summary>
        protected override void runApp()
        {
            // ----- Run app -----
            prot.RunApp();

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

        /// <summary>
        /// Run bootloader
        /// (from application)
        /// </summary>
        protected override void runBootloader()
        {
            // ----- Run bootloader -----
            if (prot.SwitchToBootloader())
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

        /// <summary>
        /// Stay in bootloader
        /// </summary>
        protected override void stayInBootloader()
        {
            // ----- Run bootloader -----
            prot.StayInBootloader();
        }


        #endregion

        #region HV 

        /// <summary>
        /// Set HV
        /// </summary>
        /// <param name="HV">Voltage</param>
        protected void setHV(int HV)
        {
            setParam(13, HV.ToString());
        }

        protected void setCalibHVPoint(byte domain, byte point, float voltage)
        {
            if (!prot.CalibrationHVSetPoint(domain, point, voltage))
                throw new Exception("Can't set HV point!");
        }

        protected void setCalibHV(byte domain)
        {
            if (!prot.CalibrationHVSet(domain))
                throw new Exception("Can't set HV calibration!");
        }

        #endregion

        #region Measurement

        /// <summary>
        /// Start measuring
        /// </summary>
        protected void start()
        {
            prot.StartROIs();
        }

        /// <summary>
        /// Stop Measuring
        /// </summary>
        protected void stop()
        {
            prot.StopROIs();
        }

        /// <summary>
        /// Stop Measuring
        /// </summary>
        protected void latch()
        {
            prot.LatchROIs();
        }

        /// <summary>
        /// Clear Measuring
        /// </summary>
        protected void clear()
        {
            prot.ClearROIs();
        }

        #endregion


    }

}