

using Fx.Conversion;
using Fx.Devices;
using Fx.IO;
using Fx.IO.Exceptions;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Xml.Linq;




namespace Fx.IO.Protocols
{

    enum nuviaCmds
    {
        GetVersion = 0x01,
        GetXML = 0xD2,
        GetParam = 0xC2,
        SetParam = 0xC3,
        GetUserData = 0x10,
        SetUserData = 0x0F,

        StartROIs = 0xB5,
        StopROIs = 0xB6,
        ClearROIs = 0xB7,
        LatchROIs = 0xBF,

        StartSpectrum = 0x05,
        StopSpectrum = 0x06,
        ClearSpectrum = 0x07,
        GetSpectrum = 0x03,

        GetDir = 0x80,
        GetFile = 0x81,
        DelFile = 0x82,
        DelAllFiles = 0x83,

        GetConfig = 0xCA,
        SetConfig = 0xCB,
        ResetConfig = 0xCC,
        CreateFactoryConfig = 0xCD,

        SwitchToBootloader = 0xD7,
        BootloaderCommands = 0xD8,

        CalibrationHV = 0x30,
        SwitchHV = 0x0B,

        Login = 0xE1,

        Error = 0xE0
    }


    /// <summary>
    /// NUVIA communication protocol class
    /// Version:        1.0
    /// Date:           2017-05-10
    /// Name:           Martin Fiala
    /// </summary>
    public class NuviaProtocol
    {
        #region Structures

        /// <summary>
        /// Packet Command structure
        /// </summary>
        struct commands
        {
            public byte function;       // function
            public byte? subFunction;   // subFunction
            public byte[] data;         // data
            public byte[] recvData;     // received data
        }


        #endregion

        #region Variables

        // ----- Device settings -----
        ConnectionSetting Settings = new ConnectionSetting("COM1", 115200, Parity.None, 8, StopBits.One);

        // ----- Communication class -----
        Communication com;                  // Communication class

        // ----- Protocol -----
        commands cmd;                       // Packet command

        // ----- Sending buffers -----
        byte[] packetSended;                // packet data
        byte[] packetReceived;

        // ----- XML description -----
        string XMLdesc;
        byte lastLang = 0;
        bool windows1250 = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor NUVIA protocol
        /// </summary>
        public NuviaProtocol()
        {
            com = new Communication();
            com.SetSPParams(Settings.BaudRate, Parity.None, 8, StopBits.One);
        }

        public NuviaProtocol(Communication com)
        {
            this.com = com;
        }

        #endregion

        #region CRC

        /// <summary>
        /// Protocol Checksum
        /// </summary>
        /// <param name="data">Packet data</param>
        /// <param name="len">Packet length</param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int len)
        {
            int i;
            uint sum = 0;
            if (len == 0) len = data.Length;
            for (i = 0; i < len; i++) sum += data[i];
            return (byte)((~sum + 1) & 0xff);
        }

        /// <summary>
        /// STM32 Checksum
        /// </summary>
        /// <param name="inputData">Input data</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Data length</param>
        /// <returns></returns>
        public UInt32 STM32Checksum(byte[] inputData, int startIndex = 0, int length = 0)
        {
            // ----- Compute size -----
            if (length <= 0) length = inputData.Length;
            var size = length / 4;
            var fullSize = size;
            var remain = length % 4;
            if (remain > 0) fullSize++;

            // ----- Convert Byte to UInt array -----
            var intData = new uint[fullSize];
            for (var i = 0; i < size; i++)
            {
                intData[i] = BitConverter.ToUInt32(inputData, (i * 4) + startIndex);
            }

            // ----- Add remainder -----
            if (remain > 0)
            {
                byte[] remainInt = new byte[4];

                if (remain >= 1) remainInt[0] = inputData[size * 4];
                if (remain >= 2) remainInt[1] = inputData[size * 4 + 1];
                if (remain >= 3) remainInt[2] = inputData[size * 4 + 2];
                intData[fullSize - 1] = BitConverter.ToUInt32(remainInt, 0);
            }


            return STM32Checksum(intData);
        }

        /// <summary>
        /// STM32 Checksum
        /// </summary>
        /// <param name="inputData">Input data</param>
        /// <param name="initial">Initial value</param>
        /// <param name="polynomial">Polynom</param>
        /// <returns></returns>
        private UInt32 STM32Checksum(UInt32[] inputData, UInt32 initial = 0xFFFFFFFF, UInt32 polynomial = 0x04C11DB7)
        {
            UInt32 crc = initial;
            foreach (UInt32 current in inputData)
            {
                crc ^= current;
                // Process all the bits in input data.
                for (uint bitIndex = 0; (bitIndex < 32); ++bitIndex)
                {
                    // If the MSB for CRC == 1
                    if ((crc & 0x80000000) != 0)
                    {
                        crc = ((crc << 1) ^ polynomial);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            crc = ~crc;
            return crc;
        }

        #endregion

        #region Settings
        
        /// <summary>
        /// Set device address
        /// </summary>
        /// <param name="address">Device address</param>
        public void SetAddress(byte address)
        {
            Settings.Address = address;
        }

        /// <summary>
        /// Set Serial Port parameters
        /// </summary>
        /// <param name="baud">Baud rate</param>
        /// <param name="parity">Parity</param>
        /// <param name="databits">Data bits count</param>
        /// <param name="stopbits">Stop bits</param>
        public void SetSerialParam(int baud, Parity parity = Parity.None, int databits = 8, StopBits stopbits = StopBits.One)
        {
            Settings.BaudRate = baud;
            com.SetSPParams(baud, parity, databits, stopbits);
        }
        
        /// <summary>
        /// Set Serial Port
        /// </summary>
        /// <param name="port">COM port</param>
        public void SetSerialPort(string port)
        {
            Settings.Type = ConnectionType.Serial;
            Settings.SerialPort = port;
        }

        /// <summary>
        /// Set TCP Connection
        /// </summary>
        /// <param name="IP">Device IP address</param>
        /// <param name="port">Device TCP port</param>
        public void SetTCP(string IP, int port)
        {
            Settings.Type = ConnectionType.TCP;
            Settings.IP = IP;
            Settings.Port = port;
        }
        
        #endregion

        #region Connection


        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <param name="port">Serial Port</param>
        public void Connect()
        {
            com.Connect(Settings);
        }

        public void Connect(ConnectionSetting settings)
        {
            Settings = settings;
            com.Connect(Settings);
        }

        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <param name="port">Serial Port</param>
        public void Connect(string port)
        {
            Settings.Type = ConnectionType.Serial;
            Settings.SerialPort = port;
            com.ConnectSP(port);
        }

        /// <summary>
        /// Connect to TCP (default port 17000)
        /// </summary>
        /// <param name="IP">Device IP</param>
        public void ConnectTCP(string IP)
        {
            Settings.Type = ConnectionType.TCP;
            Settings.IP = IP;
            com.ConnectTcp(IP, Settings.Port);
        }

        /// <summary>
        /// Connect to TCP
        /// </summary>
        /// <param name="IP">Device IP</param>
        /// <param name="port">Device Port (default 17000)</param>
        public void ConnectTCP(string IP, int port)
        {
            Settings.Type = ConnectionType.TCP;
            Settings.IP = IP;
            Settings.Port = port;
            com.ConnectTcp(IP, port);
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
            XMLdesc = "";
            com.Close();
        }

        /// <summary>
        /// Check if connected
        /// </summary>
        /// <returns>Connection status</returns>
        public bool isConnected()
        {
            return com.IsOpen();
        }

        #endregion

        #region Nuvia Commands Functions

        // ----- Info -----
        #region Info

        /// <summary>
        /// Get device version
        /// </summary>
        /// <returns>Device version</returns>
        public string GetDevVersion()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetVersion;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                string res = (string)SendAndWait();
                return res;

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Get device XML with settings parameters
        /// </summary>
        /// <param name="lang">Language (1 = Czech, 2 = English, Other = default lang)</param>
        /// <returns>Device XML</returns>
        public string GetXML(byte lang)
        {
            if (lang != lastLang || XMLdesc == "")
            {
                try
                {
                    cmd.function = (byte)nuviaCmds.GetXML;
                    cmd.subFunction = null;
                    cmd.data = new byte[6];
                    cmd.data[0] = lang;

                    XMLdesc = (string)SendAndWait(5000);

                    lastLang = lang;

                    return XMLdesc;
                }
                catch (Exception err)
                {
                    CommException Error = new CommException(err.Message, packetSended, packetReceived);
                    throw Error;
                }
            } else
            {
                return XMLdesc;
            }
            
        }

        #endregion

        // ----- Login -----
        #region Login

        /// <summary>
        /// Login to change device permission level
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>Actual permissions</returns>
        public DevPermission Login(string password)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.Login;
                cmd.subFunction = 0x01;

                List<byte> list = new List<byte>();
                list.Add(0xFF);
                list.AddRange(Encoding.UTF8.GetBytes(password).ToList());
                list.Add(0);
               
                cmd.data = list.ToArray();

                DevPermission res = (DevPermission)SendAndWait(2000);

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns>Actual permissions</returns>
        public DevPermission Logout()
        {
            try
            {

                cmd.function = (byte)nuviaCmds.Login;
                cmd.subFunction = 0x02;

                cmd.data = new byte[0];

                DevPermission res = (DevPermission)SendAndWait(2000);

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Change password for User level
        /// </summary>
        /// <param name="password">New password</param>
        public void ChangePass(string password)
        {
            try
            {

                cmd.function = (byte)nuviaCmds.Login;
                cmd.subFunction = 0x03;

                List<byte> list = new List<byte>();
                list.Add(0xFF);
                list.AddRange(Encoding.UTF8.GetBytes(password).ToList());
                list.Add(0);

                cmd.data = list.ToArray();

                SendAndWait(2000);

                return;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        // ----- Parameters -----
        #region Parameters

        /// <summary>
        /// Get device parameter
        /// </summary>
        /// <param name="paramName">Device parameter, format: "1, 2, 3"</param>
        /// <returns>Device parameters</returns>
        public string GetParam(string paramName)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetParam;
                cmd.subFunction = null;
                if (windows1250)
                    cmd.data = Encoding.GetEncoding(1250).GetBytes(paramName);
                else
                    cmd.data = Encoding.UTF8.GetBytes(paramName);

                List<byte> list = cmd.data.ToList();
                list.Add(0);
                cmd.data = list.ToArray();

                string res = (string)SendAndWait(2000);

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Set device parameter
        /// </summary>
        /// <param name="paramName">Device parameter, format: "1=4, 2=5, 3=12"</param>
        /// <returns>Device parameters</returns>
        public string SetParam(string paramName)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.SetParam;
                cmd.subFunction = null;
                if (windows1250)
                    cmd.data = Encoding.GetEncoding(1250).GetBytes(paramName);
                else
                    cmd.data = Encoding.UTF8.GetBytes(paramName);
                List<byte> list = cmd.data.ToList();
                list.Add(0);
                cmd.data = list.ToArray();

                string res = (string)SendAndWait();

                /*if (paramName != res)
                {
                    CommException Error = new CommException("You don't have permissions to change this parameter or value is out of range!", packetSended, packetReceived);
                    throw Error;
                }*/

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Get all measurement data
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetMeasurement()
        {
            // ----- Get All Measured Values -----
            string param = GetParam("9999");
            return ParseParam(param);
        }

        /// <summary>
        /// Get all measurement data
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetDescription()
        {
            // ----- Get All Description Values -----
            string param = GetParam("9998");
            return ParseParam(param);
        }

        #endregion

        #region User data

        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="address">Data address</param>
        /// <param name="length">Length to read</param>
        /// <returns>User data</returns>
        public byte[] GetUserData(int address, int length)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetUserData;
                cmd.subFunction = null;
                cmd.data = new byte[4];
                cmd.data[0] = (byte)(address & 0xFF);
                cmd.data[1] = (byte)((address >> 8) & 0xFF);
                cmd.data[2] = (byte)(length & 0xFF);
                cmd.data[3] = (byte)((length >> 8) & 0xFF);

                byte[] res = (byte[]) SendAndWait();
                res = res.Skip(4).ToArray();
                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Set user data
        /// </summary>
        /// <param name="address">Data address</param>
        /// <param name="data">Byte array to write</param>
        public void SetUserData(int address, byte[] data)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.SetUserData;
                cmd.subFunction = null;
                cmd.data = new byte[4 + data.Length];
                cmd.data[0] = (byte)(address & 0xFF);
                cmd.data[1] = (byte)((address >> 8) & 0xFF);
                cmd.data[2] = (byte)(data.Length & 0xFF);
                cmd.data[3] = (byte)((data.Length >> 8) & 0xFF);
                Array.Copy(data, 0, cmd.data, 4, data.Length);
                SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        

        // ----- ROIs -----
        #region ROIs

        /// <summary>
        /// Start All ROIs
        /// </summary>
        public void StartROIs()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.StartROIs;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Stop All ROIs
        /// </summary>
        public void StopROIs()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.StopROIs;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Clear All ROIs
        /// </summary>
        public void ClearROIs()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.ClearROIs;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }
        
        /// <summary>
        /// Latch All ROIs
        /// </summary>
        public void LatchROIs()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.LatchROIs;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        #endregion

        // ----- Spectrum -----
        #region Spectrum

        /// <summary>
        /// Start spectrum
        /// </summary>
        public void StartSpectrum()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.StartSpectrum;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }
        
        /// <summary>
        /// Stop spectrum
        /// </summary>
        public void StopSpectrum()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.StopSpectrum;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void ClearSpectrum()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.ClearSpectrum;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                SendAndWait();

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public Spectrum GetSpectrum()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetSpectrum;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                Spectrum res = (Spectrum)SendAndWait(2000);

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }
       
        #endregion

        // ----- Files -----
        #region Files

        /// <summary>
        /// Get file list
        /// </summary>
        public string[] GetDir()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetDir;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                string[] res = (string[])SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns></returns>
        public string GetFile(string FileName)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetFile;
                cmd.subFunction = null;
                if (windows1250)
                    cmd.data = Encoding.GetEncoding(1250).GetBytes(FileName);
                else
                    cmd.data = Encoding.UTF8.GetBytes(FileName);
                List<byte> list = cmd.data.ToList();
                list.Add(0);
                cmd.data = list.ToArray();

                string res = (string)SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="FileName">File name</param>
        /// <returns>Returns true if delete ok</returns>
        public bool DelFile(string FileName)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.DelFile;
                cmd.subFunction = null;
                if (windows1250)
                    cmd.data = Encoding.GetEncoding(1250).GetBytes(FileName);
                else
                    cmd.data = Encoding.UTF8.GetBytes(FileName);
                List<byte> list = cmd.data.ToList();
                list.Add(0);
                cmd.data = list.ToArray();

                bool res = (bool)SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if delete ok</returns>
        public bool DelAllFiles()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.DelAllFiles;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                bool res = (bool)SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Get configuration from device
        /// </summary>
        /// <param name="configType">Configuration type (0 = bootloader, 1 = shared, 2 = app)</param>
        /// <returns>Configuration XML</returns>
        public string GetConfig(byte configType)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.GetConfig;
                cmd.subFunction = null;
                cmd.data = new byte[1];
                cmd.data[0] = configType;
                
                return (string)SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Delete all files
        /// </summary>
        /// <returns>Returns true if delete ok</returns>
        public void SetConfig(string configuration)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.SetConfig;
                cmd.subFunction = null;

                var list = Encoding.UTF8.GetBytes(configuration).ToList();
                list.Add(0);

                cmd.data = list.ToArray();

                SendAndWait(5000);

            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        /// <summary>
        /// Reset device configuration
        /// </summary>
        /// <returns></returns>
        public bool ResetConfig()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.ResetConfig;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                return (bool)SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Create Factory Config
        /// </summary>
        /// <returns></returns>
        public bool CreateFactoryConfig()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.CreateFactoryConfig;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                return (bool)SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        #endregion

        // ----- Firmware -----
        #region Firmware

        /// <summary>
        /// Switch to bootloader
        /// </summary>
        /// <returns></returns>
        public bool SwitchToBootloader()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.SwitchToBootloader;
                cmd.subFunction = null;
                cmd.data = new byte[0];

                bool res = (bool)SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        public void StayInBootloader()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.BootloaderCommands;
                cmd.subFunction = 0x01;
                cmd.data = new byte[0];

                SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Get buffer size
        /// </summary>
        /// <returns></returns>
        public uint GetBufferSize()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.BootloaderCommands;
                cmd.subFunction = 0x02;
                cmd.data = new byte[0];

                uint res = (uint)SendAndWait();

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Send application data
        /// </summary>
        /// <param name="index">Data index</param>
        /// <param name="totalLength">Total length</param>
        /// <param name="data">Data packet</param>
        public void SendAppData(int index, int totalLength, byte[] data)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.BootloaderCommands;
                cmd.subFunction = 0x04;
                cmd.data = new byte[0];

                // ----- Data Index -----
                byte[] byteVals = BitConverter.GetBytes(index);
                cmd.data = cmd.data.Concat(byteVals).ToArray();

                // ----- Total length -----
                byteVals = BitConverter.GetBytes(totalLength);
                cmd.data = cmd.data.Concat(byteVals).ToArray();

                // ----- Data length -----
                byteVals = BitConverter.GetBytes(data.Length);
                cmd.data = cmd.data.Concat(byteVals).ToArray();

                // ----- Data -----
                cmd.data = cmd.data.Concat(data).ToArray();

                //string hex = BitConverter.ToString(cmd.data).Replace("-", string.Empty);
                //Files.SaveFile(@"C:\data\testhex.txt", hex);


                // ----- CRC32 -----
                /*uint crc = Crc32Algorithm.Compute(cmd.data, 1, data.Length - 1);
                byteVals = BitConverter.GetBytes(crc);*/

                // ----- STM CRC -----
                var crc = STM32Checksum(cmd.data, 0, cmd.data.Length);
                byteVals = BitConverter.GetBytes(crc);

                cmd.data = cmd.data.Concat(byteVals).ToArray();

                SendAndWait(5000);
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Get app CRC
        /// </summary>
        /// <returns></returns>
        public uint GetAppCRC(uint CRC)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.BootloaderCommands;
                cmd.subFunction = 0x05;
                cmd.data = new byte[0];

                // ----- Add computed CRC -----
                byte[] byteVals = BitConverter.GetBytes(CRC);
                cmd.data = cmd.data.Concat(byteVals).ToArray();


                uint res = (uint)SendAndWait(10000);

                return res;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Run App
        /// </summary>
        /// <returns></returns>
        public void RunApp()
        {
            try
            {
                cmd.function = (byte)nuviaCmds.BootloaderCommands;
                cmd.subFunction = 0x08;
                cmd.data = new byte[0];

                SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        #region Calibration 


        public bool CalibrationHVSetPoint(byte domain, byte point, float voltage)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.CalibrationHV;
                cmd.subFunction = 0x00;
                cmd.data = new byte[6];

                cmd.data[0] = domain;   // HV domain
                cmd.data[1] = point;    // Point

                Array.Copy(BitConverter.GetBytes(voltage), 0, cmd.data, 2, 4);  // voltage

                return (bool) SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        public bool CalibrationHVSet(byte domain)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.CalibrationHV;
                cmd.subFunction = 0x01;
                cmd.data = new byte[] { domain };

                return (bool) SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        // ----- HV -----
        /// <summary>
        /// Switch HV
        /// </summary>
        /// <param name="turnOn">Turn On/Off HV</param>
        public void SwitchHV(bool turnOn)
        {
            try
            {
                cmd.function = (byte)nuviaCmds.SwitchHV;
                cmd.subFunction = null;
                cmd.data = new byte[1];
                if (turnOn)
                    cmd.data[0] = 1;
                else cmd.data[0] = 0;

                SendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        #endregion

        #region Support functions

        /// <summary>
        /// Parse parameter string to dictionary by ID
        /// </summary>
        /// <param name="param">Parameters string</param>
        /// <returns></returns>
        public Dictionary<int, string> ParseParam(string param)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            string[] array = param.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in array)
            {
                string[] twist = item.Split(new string[] { "=" }, StringSplitOptions.None);
                if (twist.Length == 2)
                {
                    int ID = Conv.ToInt(twist[0], 0);
                    if (ID > 0)
                    {
                        try
                        {
                            dict.Add(ID, twist[1]);
                        }
                        catch (Exception) { };
                    }
                }
            }
            return dict;
        }

        /// <summary>
        /// Parse parameter string to dictionary by ID
        /// </summary>
        /// <param name="param">Parameters string</param>
        /// <returns></returns>
        public List<DevParamVals> ParseParamToList(string param)
        {
            List<DevParamVals> list = new List<DevParamVals>();

            string[] array = param.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in array)
            {
                DevParamVals parItem = new DevParamVals();
                string[] twist = item.Split(new string[] { "=" }, StringSplitOptions.None);
                if (twist.Length == 2)
                {
                    if (Conv.IsInt(twist[0]))
                    {
                        parItem.ID = Conv.ToInt(twist[0], 0);
                        parItem.Value = twist[1];
                        list.Add(parItem);
                    }
                }
            }
            return list;
        }

        #endregion


        #region Creating & Sending packet

        /// <summary>
        /// Sending packet and process reply
        /// </summary>
        private object SendAndWait(int timeout = 1000)
        {
            CreatePacket();
            Send();
            GetData(timeout);
            return ProcessData();
        }

        /// <summary>
        /// Sending packet
        /// </summary>
        private void Send()
        {
            com.Send(packetSended);
        }

        /// <summary>
        /// Create packet for sending
        /// </summary>
        /// <returns>Packet</returns>
        private byte[] CreatePacket()
        {
            int length = 3 + cmd.data.Length;
            if (cmd.subFunction != null) length++;

            List<byte> packet = new List<byte>();

            // ----- Length -----
            packet.Add((byte)((length >> 16) & 0xFF));
            packet.Add((byte)((length >> 8) & 0xFF));
            packet.Add((byte)(length & 0xFF));

            // ----- Address -----
            packet.Add((byte)Settings.Address);

            // ----- Function -----
            packet.Add(cmd.function);
            if (cmd.subFunction != null)
                packet.Add(cmd.subFunction ?? 0);

            // ----- Data -----
            for (int i = 0; i < cmd.data.Length; i++)
            {
                packet.Add(cmd.data[i]);
            }

            // ----- Checksum -----
            packet.Add(CheckSum(packet.ToArray(), packet.Count));

            packetSended = packet.ToArray();
            
            return packetSended;
        }

        /// <summary>
        /// Wait for reply data
        /// </summary>
        private void GetData(int timeoutLastChar = 1000)
        {
            int timeout = 10000; // 10s

            if (timeout < timeoutLastChar) timeout = timeoutLastChar;

            bool packetOk = false;
            int packetLength = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch stopwatchLastChar = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            stopwatchLastChar.Start();

            packetReceived = com.Read(200);
            
            
            while ((!packetOk) && (stopwatch.ElapsedMilliseconds < timeout) && (stopwatchLastChar.ElapsedMilliseconds < timeoutLastChar))
            {
                
                if (packetReceived.Length > 3)
                {
                    packetLength = (packetReceived[0] << 16) + (packetReceived[1] << 8) + packetReceived[2];
                    if (packetReceived.Length >= packetLength + 3)
                    {
                        packetOk = true;
                        break;
                    }
                }
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(5);

                byte[] res = com.Read(200);

                if (res.Length > 0)
                    stopwatchLastChar.Restart();

                packetReceived = com.AddArray(packetReceived, res);
                
            }


            if (!packetOk)
            {
                string test = System.Text.Encoding.Default.GetString(packetReceived);
                throw new Exception("Receive Timeout");
            }
        }

        #endregion

        #region Process Reply

        /// <summary>
        /// Process reply
        /// </summary>
        private object ProcessData()
        {
            if (packetReceived.Length < 6)
                throw new Exception("To short reply packet");
            // ----- Check packet length -----
            int length = (packetReceived[0] << 16) + (packetReceived[1] << 8) + packetReceived[2];
            if (length != packetReceived.Length - 3)
                throw new Exception("Corrupted packet");
            // ----- Check CheckSum -----
            byte CRC1 = packetReceived[packetReceived.Length - 1];
            byte CRC2 = CheckSum(packetReceived, packetReceived.Length - 1);

            if (packetReceived[packetReceived.Length - 1] != CheckSum(packetReceived, packetReceived.Length - 1))
                throw new Exception("Invalid checksum");
            // ----- Check Function-----
            if ((packetReceived[4] != cmd.function) && (packetReceived[4] != 0xE0))
            {
                throw new Exception("Invalid function");
            }


            int dataLength = packetReceived.Length - 6;
            cmd.recvData = new byte[dataLength];
            if (dataLength > 0)
                Array.Copy(packetReceived, 5, cmd.recvData, 0, dataLength);

            switch ((nuviaCmds)cmd.function)
            {
                case nuviaCmds.GetVersion:
                case nuviaCmds.GetParam:
                case nuviaCmds.SetParam:
                    return processGetString();

                case nuviaCmds.GetXML:                  // Get XML
                case nuviaCmds.GetConfig:
                    return processGetString(true);
                case nuviaCmds.Login: 
                    return processLogin();

                case nuviaCmds.GetDir:
                    return processGetFileList();
                case nuviaCmds.GetFile:
                    return processGetFile();
                case nuviaCmds.DelFile:
                case nuviaCmds.DelAllFiles:
                case nuviaCmds.SwitchHV:                  // Switch VH
                case nuviaCmds.ClearSpectrum:                  // Clear spectrum
                case nuviaCmds.CalibrationHV:                  // HV calibration
                case nuviaCmds.SwitchToBootloader:                  // Go to bootloader
                case nuviaCmds.ResetConfig:
                case nuviaCmds.CreateFactoryConfig:
                    return processGetBool();
                case nuviaCmds.GetSpectrum:
                    return processGetSpectrum();
                case nuviaCmds.StartSpectrum:                  // Start spectrum
                case nuviaCmds.StopSpectrum:                  // Stop spectrum
                case nuviaCmds.StartROIs:                  // Start ROIs
                case nuviaCmds.StopROIs:                  // Stop ROIs
                case nuviaCmds.ClearROIs:                  // Clear ROIs
                case nuviaCmds.LatchROIs:                  // Latch ROIs
                case nuviaCmds.SetUserData:                  // Set User data
                
                case nuviaCmds.SetConfig:                  // Update config from xml file

                    break;

                case nuviaCmds.GetUserData:                  // Get User data
                    return processGetByteArray();

                case nuviaCmds.BootloaderCommands:                  // Bootloader command
                    return processBootloader();
                case nuviaCmds.Error:
                    return processError();
                default:
                    throw new System.Exception("Unknown message!");
            }

            return null;
        }

        /// <summary>
        /// Process read data
        /// </summary>
        private object processGetString(bool setEnc = false)
        {
            try
            {
                string data = Encoding.UTF8.GetString(cmd.recvData);

                if (setEnc)
                {
                    // ----- If windows-1250 XML -> change encoding -----
                    if (data.ToLower().Contains("windows-1250"))
                    {
                        windows1250 = true;
                        data = Encoding.GetEncoding(1250).GetString(cmd.recvData);
                    } else
                        windows1250 = false;
                }
                else
                {
                    if (windows1250)
                        data = Encoding.GetEncoding(1250).GetString(cmd.recvData);
                }
                

                if (data[data.Length - 1] == 0)
                    data = data.Remove(data.Length - 1);
                return data;
               
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
        }

        /// <summary>
        /// Process get file list
        /// </summary>
        private object processGetFileList()
        {
            try
            {
                string strData;
                if (windows1250)
                    strData = Encoding.GetEncoding(1250).GetString(cmd.recvData, 1, cmd.recvData.Length-2);
                else
                    strData = Encoding.UTF8.GetString(cmd.recvData, 1, cmd.recvData.Length - 2);
                string[] data = strData.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
                return data;
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }

        }

        /// <summary>
        /// Process get file
        /// </summary>
        private object processGetFile()
        {
            try
            {
                string data;
                if (windows1250)
                    data = Encoding.GetEncoding(1250).GetString(cmd.recvData, 1, cmd.recvData.Length - 1);
                else
                    data = Encoding.UTF8.GetString(cmd.recvData, 1, cmd.recvData.Length - 1);

                if (data.Length > 0 && data[data.Length - 1] == 0)
                    data = data.Remove(data.Length - 1);
                return data;
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
        }

        /// <summary>
        /// Process get file
        /// </summary>
        private object processGetBool()
        {
            try
            {
                bool data = false;
                if (cmd.recvData.Length > 0)
                    if (cmd.recvData[0] == 1) data = true;
                return data;
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
        }

        /// <summary>
        /// Process Get spectrum
        /// </summary>
        private object processGetSpectrum()
        {
            try
            {
                Spectrum spectrum = new Spectrum();

                int Compress = cmd.recvData[0] & 0x0F;
                if (Compress == 0) Compress = 4;

                ushort LLD = Conv.SwapBytes(BitConverter.ToUInt16(cmd.recvData, 1));
                ushort ULD = Conv.SwapBytes(BitConverter.ToUInt16(cmd.recvData, 3));
                float realTime = Conv.SwapBytes(BitConverter.ToUInt32(cmd.recvData, 5)) / 1000.0f;
                float liveTime = Conv.SwapBytes(BitConverter.ToUInt32(cmd.recvData, 9)) / 1000.0f;
                ushort count = Conv.SwapBytes(BitConverter.ToUInt16(cmd.recvData, 13));
                uint timeStamp = Conv.SwapBytes(BitConverter.ToUInt32(cmd.recvData, 15));
                int rangeCount = ULD - LLD + 1;

                spectrum.LiveTime = liveTime;
                spectrum.RealTime = realTime;
                spectrum.Channels = new uint[count];

                int check = (rangeCount * Compress) + 19;
                if (check != cmd.recvData.Length) throw new System.Exception("Error parse packet!");

                
                int j = 19;
                for (int i = LLD; i <= ULD; i++)
                {
                    if (Compress == 1)
                    {
                        spectrum.Channels[i] = cmd.recvData[j];
                    }
                    else if (Compress == 2)
                    {
                        spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt16(cmd.recvData, j));
                    }
                    else if (Compress == 3)
                    {
                        byte[] tempBytes = new byte[4];
                        Array.Copy(cmd.recvData, j, tempBytes, 1, 3);
                        spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt32(tempBytes, 0)) & 0xFFFFFF;
                    }
                    else if (Compress == 4)
                    {
                        spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt32(cmd.recvData, j));
                    }
                    j += Compress;
                }

                return spectrum;


            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
        }
        
        private object processGetByteArray()
        {
            try
            {
                byte[] data = new byte[cmd.recvData.Length];
                Array.Copy(cmd.recvData, data, cmd.recvData.Length);
                return data;
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
        }

        private object processBootloader()
        {
            if (cmd.recvData.Length < 1) throw new Exception("No subcommand found!");

            try
            {
                byte subCommand = cmd.recvData[0];

                switch (subCommand)
                {
                    case 0x01:      // Stay in bootloader
                    case 0x04:      // Send application data
                    case 0x08:      // Run Application
                        return null;
                    case 0x02:      // Get buff size
                    case 0x05:      // Get Application CRC
                        if (cmd.recvData.Length < 5) throw new Exception("No data!");
                        uint uintData = BitConverter.ToUInt32(cmd.recvData, 1);
                        return uintData;
                    default:
                        throw new Exception("Unknown subcommand!");
                }
            }
            catch (Exception)
            {
                throw new Exception("Error parse packet!");
            }
        }

        /// <summary>
        /// Process get file
        /// </summary>
        private object processLogin()
        {
            DevPermission data = DevPermission.None;


            if (cmd.recvData.Length < 3) throw new System.Exception("Error parse packet!");

            if (cmd.recvData[2] == 1) data = DevPermission.Advanced;
            else if (cmd.recvData[2] == 2) data = DevPermission.Service;
            else if (cmd.recvData[2] == 3) data = DevPermission.SuperUser;

            byte subCommand = cmd.recvData[0];


            switch (subCommand)
            {
                case 0x01:      // Login
                    if (cmd.recvData[1] > 0) throw new System.Exception("Bad password!");
                    break;
                case 0x02:      // Logout
                    if (cmd.recvData[1] > 0) throw new System.Exception("Logout error!");
                    break;
                case 0x03:      // Change password
                    switch (cmd.recvData[1])
                    {
                        case 0:
                            break;
                        case 1:
                            throw new System.Exception("Bad parameter!");
                        case 2:
                            throw new System.Exception("Bad password length!");
                        case 3:
                            throw new System.Exception("No permission!");
                        default:
                            throw new System.Exception("Unknown error!");
                    }
                    break;
                default:
                    throw new Exception("Unknown subcommand!");
            }

            return data;
        }
        
        private object processError()
        {
            try
            {
                switch (cmd.recvData[0])
                {
                    case 0x01:
                        throw new Exception("Unknown command!");
                    case 0x02:
                        throw new Exception("Unsupported command!");
                    case 0x03:
                        throw new Exception("Bad incomming checksum!");
                    default:
                        throw new Exception("Unknown error!");
                }
            }
            catch (Exception)
            {
                throw new Exception("Error parse packet!");
            }
        }
        #endregion
    }

    
}
