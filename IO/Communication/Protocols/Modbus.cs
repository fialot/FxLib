using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Fx.IO;


/* 
 * Není vyzkoušen ANSII protokol!!!
 * Ani nejsou vyzkoušené záporná čísla
 * 
 */

namespace Fx.IO.Protocols
{
    enum mbMode { RTU, ASCII, TCP}

    /// <summary>
    /// MODBUS protocol class
    /// Version:        1.1
    /// Date:           2018-05-24
    /// Name:           Martin Fiala
    /// </summary>
    public class ModbusProtocol
    {
        #region Structures

        struct commands                 // command structure
        {
            //public string cmd;
            public byte function;       // function
            public ushort regAddress;   // register address
            public ushort length;       // data length (coils, registers...)
            public ushort[] param;      // data (coil/register values)
        }

        #endregion

        #region Variables


        mbMode Mode = mbMode.RTU;
              
        bool TCP = false;               // True = MODBUS TCP/IP; False = Serial Port
        bool FirstSending = true;

        int address = 1;                // Device address

        string port = "";               // Connected port
        string COM = "COM1";             // default COM port
        string IP = "192.168.1.10";     // default IP
        int netPort = 17000;            // default TCP port

        commands cmd;

        byte[] packetSended;            // packet data
        byte[] packetReceived;
        string ASCIIpacketSended;
        string ASCIIpacketReceived;

        ushort[] regList;               // Values from device
        bool[] coilList;
        byte[] byteList;

        protected Communication com;              // Communication class

        const int SerialTimeOut = 300;
        const int TCPTimeOut = 2000;

        const int SerialEndPacketTimeOut = 50;
        const int TCPEndPacketTimeOut = 100;

        int timeOut = SerialTimeOut;              // timeout for first received char
        int endPacketTimeOut = SerialEndPacketTimeOut;      // timeout for last received char

        #endregion

        /// <summary>
        /// Constructor MODBUS protocol
        /// </summary>
        public ModbusProtocol()
        {
            com = new Communication();
        }

        public ModbusProtocol(Communication com)
        {
            this.com = com;
        }

        #region CRC

        /// <summary>
        /// Compute the MODBUS RTU CRC
        /// </summary>
        /// <param name="buffer">Buffer to compute CRC</param>
        /// <param name="length">Buffer length</param>
        /// <returns></returns>
        private ushort CRC(byte[] buffer, int length)
        {
            ushort crc = 0xFFFF;

            for (int pos = 0; pos < length; pos++)
            {
                crc ^= (ushort)buffer[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            return crc;
        }

        /// <summary>
        /// Compute the MODBUS ASCII LRC
        /// </summary>
        /// <param name="txt">Buffer to compute CRC</param>
        /// <param name="length">Buffer length</param>
        /// <returns></returns>
        private byte LRC(byte[] txt, uint length)
        {
            byte lrc = 0x00;

            for (int i = 0; i < length; i++) lrc += txt[i];

            return lrc = (byte)(256 - lrc);
        }

        #endregion

        #region Settings

        /// <summary>
        /// Set MODBUS protocol (RTU/ANSII)
        /// </summary>
        /// <param name="RTU">True = RTU protocol, False = ANSII protocol</param>
        public void SetProtocol(bool RTU)
        {
            if (!TCP)
            {
                if (RTU)
                    this.Mode = mbMode.RTU;
                else
                    this.Mode = mbMode.ASCII;
            }
        }

        /// <summary>
        /// Set device address
        /// </summary>
        /// <param name="address">Device address</param>
        public void SetAddress(ushort address)
        {
            this.address = address;
        }

        /// <summary>
        /// Set Serial Port parameters
        /// </summary>
        /// <param name="baud">Baud rate</param>
        /// <param name="parity">Parity</param>
        /// <param name="databits">Data bits count</param>
        /// <param name="stopbits">Stop bits</param>
        public void SetSerialParam(int baud, Parity parity = Parity.Even, int databits = 8, StopBits stopbits = StopBits.One)
        {
            com.SetSPParams(baud, parity, databits, stopbits);
        }

        /// <summary>
        /// Set Serial Port
        /// </summary>
        /// <param name="port">COM port</param>
        public void SetSerialPort(string port)
        {
            TCP = false;
            COM = port;
        }

        /// <summary>
        /// Set TCP Connection
        /// </summary>
        /// <param name="IP">Device IP address</param>
        /// <param name="port">Device TCP port</param>
        public void SetTCP(string IP, int port)
        {
            TCP = true;
            Mode = mbMode.TCP;
            this.IP = IP;
            netPort = port;
        }

        /// <summary>
        /// Set Default Received TimeOuts
        /// </summary>
        public void SetTimeOuts()
        {
            if (com.OpenedInterface ==  interfaces.COM)
            {
                timeOut = SerialTimeOut;
                endPacketTimeOut = SerialEndPacketTimeOut;
            } else
            {
                timeOut = TCPTimeOut;
                endPacketTimeOut = TCPEndPacketTimeOut;
            }
            
        }

        /// <summary>
        /// Set Received TimeOuts
        /// </summary>
        /// <param name="ReceivedTimeOut">Reply timeout</param>
        /// <param name="LastCharTimeOut">Waiting for last char timeout</param>
        public void SetTimeOuts(int ReceivedTimeOut, int LastCharTimeOut)
        {
            timeOut = ReceivedTimeOut;
            endPacketTimeOut = LastCharTimeOut;
        }

        #endregion

        #region Connection

        /// <summary>
        /// Connect to default com port "COM1"
        /// </summary>
        public void Connect()
        {
            if (TCP)
                com.ConnectTcp(IP, netPort);
            else
                com.ConnectSP(COM);
            FirstSending = true;
        }

        public void Connect(ConnectionSetting settings)
        {
            if (settings.Type == ConnectionType.Serial)
            {
                TCP = false;
                port = settings.SerialPort;
            }
            else
            {
                TCP = true;
                Mode = mbMode.TCP;
                port = settings.IP + ":" + settings.Port.ToString();
            }
            com.Connect(settings);
            FirstSending = true;
        }

        /// <summary>
        /// Connect to serial port
        /// </summary>
        /// <param name="port">Serial Port</param>
        public void Connect(string port)
        {
            this.port = port;
            com.ConnectSP(port);
            TCP = false;
            FirstSending = true;
        }

        /// <summary>
        /// Connect to MODBUS TCP (default port 502)
        /// </summary>
        /// <param name="IP">MODBUS Server IP</param>
        public void ConnectTCP(string IP)
        {
            this.port = IP + ":" + netPort.ToString();
            com.ConnectTcp(IP, netPort);
            TCP = true;
            Mode = mbMode.TCP;
            FirstSending = true;
        }

        /// <summary>
        /// Connect to MODBUS TCP
        /// </summary>
        /// <param name="IP">MODBUS Server IP</param>
        /// <param name="port">MODBUS Server Port (dafault 502)</param>
        public void ConnectTCP(string IP, int port)
        {
            this.port = IP + ":" + port.ToString();
            com.ConnectTcp(IP, port);
            TCP = true;
            Mode = mbMode.TCP;
            FirstSending = true;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
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
        
        /// <summary>
        /// Check if TCP / Serial port connection
        /// </summary>
        /// <returns>TCP connection status</returns>
        public bool isTCPConnection()
        {
            return TCP;
        }


        /// <summary>
        /// Get actual port
        /// </summary>
        /// <returns></returns>
        public string getPort()
        {
            return port;
        }

        #endregion

        #region Read Functions

        /// <summary>
        /// Read Coil (Holding) (function 1)
        /// </summary>
        /// <param name="address">Coil address</param>
        /// <returns>Coil value</returns>
        public bool ReadCoil(ushort address)
        {
            bool[] coils = ReadCoils(address, 1);
            return coils[0];
        }

        /// <summary>
        /// Read Coils (Holding) (function 1)
        /// </summary>
        /// <param name="address">First coil address</param>
        /// <param name="length">Coil count</param>
        /// <returns>Coil values</returns>
        public bool[] ReadCoils(ushort address, ushort length)
        {
            try
            {
                cmd.function = 1;
                cmd.regAddress = address;
                cmd.length = length;

                sendAndWait();

                return coilList;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Read Discrete Input (function 2)
        /// </summary>
        /// <param name="address">Discrete input address</param>
        /// <returns>Discete input value</returns>
        public bool ReadDiscreteInput(ushort address)
        {
            bool[] coils = ReadDiscreteInputs(address, 1);
            return coils[0];
        }

        /// <summary>
        /// Read Discrete Inputs (function 2)
        /// </summary>
        /// <param name="address">First discrete input address</param>
        /// <param name="length">Discete inputs count</param>
        /// <returns>Discete input values</returns>
        public bool[] ReadDiscreteInputs(ushort address, ushort length)
        {
            try
            {
                cmd.function = 2;
                cmd.regAddress = address;
                cmd.length = length;

                sendAndWait();

                return coilList;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Read Coils/Discrete Inputs (function 1, 2)
        /// </summary>
        /// <param name="address">First discrete input address</param>
        /// <param name="length">Discete inputs count</param>
        /// <param name="input">Select Coils/Discrete Input</param>
        /// <returns>Discete input values</returns>
        public bool[] ReadBools(ushort address, ushort length, bool input = false)
        {
            if (input)
                return ReadDiscreteInputs(address, length);
            else
                return ReadCoils(address, length);
        }

        /// <summary>
        /// Read Coil/Discrete Input (function 1, 2)
        /// </summary>
        /// <param name="address">First discrete input address</param>
        /// <param name="input">Select Coils/Discrete Input</param>
        /// <returns>Discete input values</returns>
        public bool ReadBool(ushort address, bool input = false)
        {
            if (input)
                return ReadDiscreteInput(address);
            else
                return ReadCoil(address);
        }

        /// <summary>
        /// Read Holding register (function 3)
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Register Value</returns>
        public ushort ReadHoldingRegister(ushort address)
        {
            ushort[] res = ReadHoldingRegisters(address, 1);
            return res[0];
        }

        /// <summary>
        /// Read Holding registers (function 3)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <returns>Register values</returns>
        public ushort[] ReadHoldingRegisters(ushort address, ushort length)
        {
            try
            {
                ushort toSend = length;
                ushort[] array = new ushort[0];
                ushort[] tmpArray;

                while (toSend > 0)
                {
                    cmd.function = 3;
                    cmd.regAddress = address;
                    if (toSend > 120)
                    {
                        cmd.length = 120;
                        address += 120;
                    }
                    else
                    {
                        cmd.length = toSend;
                    }
                    toSend -= cmd.length;

                    sendAndWait();

                    tmpArray = new ushort[array.Length + regList.Length];
                    array.CopyTo(tmpArray, 0);
                    regList.CopyTo(tmpArray, array.Length);
                    array = tmpArray;
                }
                return array;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        /// <summary>
        /// Read Holding registers Extended (function 65)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <returns>Register values</returns>
        public ushort[] ReadHoldingRegistersExt(ushort address, ushort length)
        {
            try
            {
                cmd.function = 65;
                cmd.regAddress = address;
                cmd.length = length;

                sendAndWait();

                return regList;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        /// <summary>
        /// Read Input register (function 4)
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Register Value</returns>
        public ushort ReadInputRegister(ushort address)
        {
            ushort[] res = ReadInputRegisters(address, 1);
            return res[0];
        }

        /// <summary>
        /// Read Input registers (function 4)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <returns>Register values</returns>
        public ushort[] ReadInputRegisters(ushort address, ushort length)
        {
            try
            {
                ushort toSend = length;
                ushort[] array = new ushort[0];
                ushort[] tmpArray;

                while (toSend > 0)
                {
                    cmd.function = 4;
                    cmd.regAddress = address;
                    if (toSend > 120)
                    {
                        cmd.length = 120;
                        address += 120;
                    } else
                    {
                        cmd.length = toSend;
                    }
                    toSend -= cmd.length;

                    sendAndWait();

                    tmpArray = new ushort[array.Length + regList.Length];
                    array.CopyTo(tmpArray, 0);
                    regList.CopyTo(tmpArray, array.Length);
                    array = tmpArray;
                }
                return array;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Read Input registers Extended (function 66)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <returns>Register values</returns>
        public ushort[] ReadInputRegistersExt(ushort address, ushort length)
        {
            try
            {
                cmd.function = 66;
                cmd.regAddress = address;
                cmd.length = length;

                sendAndWait();

                return regList;
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Read register (function 3, 4)
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input registers</param>
        /// <returns>Register Value</returns>
        public ushort ReadRegister(ushort address, bool input = false)
        {
            if (input)
                return ReadInputRegister(address);
            else
                return ReadHoldingRegister(address);
        }

        /// <summary>
        /// Read registers (function 3, 4)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <param name="input">Select Holding/Input registers</param>
        /// <returns></returns>
        public ushort[] ReadRegisters(ushort address, ushort length, bool input = false)
        {
            if (input)
                return ReadInputRegisters(address, length);
            else
                return ReadHoldingRegisters(address, length);
        }

        /// <summary>
        /// Read registers Extended (function 65, 66)
        /// </summary>
        /// <param name="address">Address of first Register</param>
        /// <param name="length">Register count</param>
        /// <param name="input">Select Holding/Input registers</param>
        /// <returns></returns>
        public ushort[] ReadRegistersExt(ushort address, ushort length, bool input = false)
        {
            if (input)
                return ReadInputRegistersExt(address, length);
            else
                return ReadHoldingRegistersExt(address, length);
        }

        /// <summary>
        /// Read Function
        /// </summary>
        /// <param name="function">Function number</param>
        /// <returns>Byte values</returns>
        public byte[] ReadFunction(byte function)
        {
            byte[] data = new byte[0];
            return ReadFunction(function, data);
        }


        /// <summary>
        /// Read Function
        /// </summary>
        /// <param name="function">Function number</param>
        /// <param name="data">Data</param>
        /// <returns>Byte values</returns>
        public byte[] ReadFunction(byte function, byte[] data)
        {
            try
            {
                try
                {
                    cmd.function = function;
                    cmd.regAddress = 0;
                    if (data != null)
                    {
                        cmd.param = new ushort[data.Length];
                        for (int i = 0; i < data.Length; i++) cmd.param[i] = data[i];
                    }
                    else cmd.param = new ushort[0];

                    cmd.length = 0;

                    sendAndWait();

                    return byteList;
                }
                catch (Exception err)
                {
                    CommException Error = new CommException(err.Message, packetSended, packetReceived);
                    throw Error;
                }
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }


        #endregion

        #region Write Functions

        /// <summary>
        /// Write Coil (function 5)
        /// </summary>
        /// <param name="address">Coil address</param>
        /// <param name="coil">Coil Value</param>
        public void WriteCoil(ushort address, bool coil)
        {
            try
            {
                cmd.function = 5;
                cmd.regAddress = address;
                cmd.length = 1;
                cmd.param = new ushort[1];
                if (coil)
                    cmd.param[0] = 1;
                else cmd.param[0] = 0;

                sendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Write Coils (function 15)
        /// </summary>
        /// <param name="address">First Coil address</param>
        /// <param name="coils">Coils Value</param>
        public void WriteCoils(ushort address, bool[] coils)
        {
            try
            {
                cmd.function = 15;
                cmd.regAddress = address;
                cmd.length = (ushort)coils.Length;
                int paramLen = cmd.length / 8;
                if (cmd.length % 8 > 0) paramLen++;
                cmd.param = new ushort[paramLen];
                for (int i = 0; i < cmd.length; i++)
                {
                    cmd.param[i / 8] += (byte)(Convert.ToByte(coils[i]) << (i % 8));
                }
                sendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Write Coil (function 5)
        /// </summary>
        /// <param name="address">Coil address</param>
        /// <param name="coil">Coil Value</param>
        public void WriteBool(ushort address, bool coil)
        {
            WriteCoil(address, coil);
        }

        /// <summary>
        /// Write Coils (function 15)
        /// </summary>
        /// <param name="address">First Coil address</param>
        /// <param name="coils">Coils Value</param>
        public void WriteBools(ushort address, bool[] coils)
        {
            WriteCoils(address, coils);
        }

        /// <summary>
        /// Write Register (function 6)
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="register">Register value</param>
        public void WriteRegister(ushort address, ushort register)
        {
            try
            {
                cmd.function = 6;
                cmd.regAddress = address;
                cmd.length = 1;
                cmd.param = new ushort[1];
                cmd.param[0] = register;

                sendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        /// <summary>
        /// Write Registers (function 16)
        /// </summary>
        /// <param name="address">First Register address</param>
        /// <param name="registers">Register values</param>
        public void WriteRegisters(ushort address, ushort[] registers)
        {
            try
            {
                cmd.function = 16;
                cmd.regAddress = address;
                cmd.length = (ushort)registers.Length;
                cmd.param = registers;
                sendAndWait();
            }
            catch (Exception err)
            {
                CommException Error = new CommException(err.Message, packetSended, packetReceived);
                throw Error;
            }
        }

        #endregion

        #region Creating & Sending packet

        /// <summary>
        /// Sending packet and process reply
        /// </summary>
        void sendAndWait()
        {
            createPacket();
            send();
            try
            {
                packetReceived = new byte[0];
                getData();
                if (!processData())
                {
                    getData();
                    if (!processData())
                    {
                        throw new System.Exception("Uncomplete packet!");
                    }
                }
            } catch (Exception err)
            {
                if (FirstSending && TCP)
                {
                    FirstSending = false;
                    Mode = mbMode.RTU;
                    sendAndWait();
                }
                else
                {
                    if (TCP)
                        Mode = mbMode.TCP;
                        
                    throw err;
                }
            }
            FirstSending = false;
        }

        /// <summary>
        /// Sending packet
        /// </summary>
        void send()
        {
            com.ClearInput();
            com.Send(packetSended);

            //Files.SaveFile("com.log", DateTime.Now.ToLongTimeString() + " << " + BitConverter.ToString(packetSended) + Environment.NewLine, true);
        }

        /// <summary>
        /// Create packet for sending
        /// </summary>
        /// <returns>Packet</returns>
        byte[] createPacket()
        {
            byte[] barr = new byte[2];

            // ----- Get Packet Length -----
            int byteCount = 2;
            if (Mode == mbMode.TCP) byteCount += 6;            // TCP Length
            else if (Mode == mbMode.RTU) byteCount += 2;       // RTU Length
            else byteCount += 1;                // ASCII Length

            switch (cmd.function)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 65:
                case 66:
                    byteCount += 4; break;
                case 15:
                    byteCount += 5 + cmd.param.Length; break;
                case 16:
                    byteCount += 5 + 2 * cmd.param.Length; break;
                default:
                    byteCount += cmd.param.Length;
                    break;
            }
            packetSended = new byte[byteCount];

            // ----- Fill Packet -----
            int idx = 0;
            if (Mode == mbMode.TCP)
            {
                packetSended[idx++] = 0;
                packetSended[idx++] = 1;
                packetSended[idx++] = 0;
                packetSended[idx++] = 0;
                packetSended[idx++] = 0;
                packetSended[idx++] = 0;
            }
            packetSended[idx++] = (byte)address;
            packetSended[idx++] = cmd.function;

            switch (cmd.function)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 65:
                case 66:
                    packetSended[idx++] = (byte)((cmd.regAddress >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.regAddress & 0xFF);

                    packetSended[idx++] = (byte)((cmd.length >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.length & 0xFF);
                    break;
                case 5:
                    packetSended[idx++] = (byte)((cmd.regAddress >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.regAddress & 0xFF);

                    if (cmd.param[0] > 0)
                        packetSended[idx++] = (byte)(0xFF);
                    else
                        packetSended[idx++] = 0;
                    packetSended[idx++] = 0;
                    break;
                case 6:
                    packetSended[idx++] = (byte)((cmd.regAddress >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.regAddress & 0xFF);

                    barr = System.BitConverter.GetBytes(cmd.param[0]);
                    packetSended[idx++] = barr[1];
                    packetSended[idx++] = barr[0];
                    break;

                case 15:
                    packetSended[idx++] = (byte)((cmd.regAddress >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.regAddress & 0xFF);

                    packetSended[idx++] = (byte)((cmd.length >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.length & 0xFF);       // pocet civek
                    packetSended[idx++] = (byte)(cmd.param.Length);    // pocet bytu

                    for (int i = 0; i < cmd.param.Length; i++)
                    {
                        packetSended[idx++] = (byte)cmd.param[i];
                    }
                    break;
                case 16:
                    packetSended[idx++] = (byte)((cmd.regAddress >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.regAddress & 0xFF);

                    packetSended[idx++] = (byte)((cmd.length >> 8) & 0xFF);
                    packetSended[idx++] = (byte)(cmd.length & 0xFF); // pocet registru
                    packetSended[idx++] = (byte)(2 * cmd.param.Length);    // pocet bytu

                    for (int i = 0; i < cmd.param.Length; i++)
                    {
                        barr = System.BitConverter.GetBytes(cmd.param[i]);
                        packetSended[idx++] = barr[1];
                        packetSended[idx++] = barr[0];
                    }
                    break;
                default:
                    //throw new Exception("Unimplemented function!");
                    for (int i = 0; i < cmd.param.Length; i++)
                    {
                        packetSended[idx++] = (byte)cmd.param[i];
                    }
                    break;
            }

            if (Mode == mbMode.TCP)
            {
                packetSended[4] = (byte)(((idx - 6) >> 8) & 0xFF);
                packetSended[5] = (byte)((idx - 6) & 0xFF);
            }
            else if (Mode == mbMode.ASCII)
            {
                packetSended[idx++] = LRC(packetSended, (uint)idx);
                ASCIIpacketSended = ":";
                
                for (int i = 0; i < idx; i++) ASCIIpacketSended += packetSended[i].ToString("X2"); //IntToHex(chk[i],2);
                ASCIIpacketSended += "\r\n";
                packetSended = Encoding.ASCII.GetBytes(ASCIIpacketSended);
            }
            else
            {
                ushort check = CRC(packetSended, idx);
                packetSended[idx++] = (byte)(check & 0xFF);
                packetSended[idx] = (byte)((check & 0xFF00) >> 8);
            }

            return packetSended;
        }

        /// <summary>
        /// Wait for reply data
        /// </summary>
        void getData()
        {
            var Rx = packetReceived.ToList();
            Rx.AddRange(com.Read(timeOut, endPacketTimeOut).ToList());
            packetReceived = Rx.ToArray();

            if (packetReceived.Length == 0)
            {
                throw new Exception("Receive Timeout");
            }
            if(Mode == mbMode.ASCII)
            {
                ASCIIpacketReceived = Encoding.ASCII.GetString(packetReceived);
            }

            //Files.SaveFile("com.log", DateTime.Now.ToLongTimeString() + " >> " + BitConverter.ToString(packetReceived) + Environment.NewLine, true);
        }

        #endregion

        #region Process Reply

        /// <summary>
        /// Process reply
        /// </summary>
        bool processData()
        {
            if (packetReceived.Length == 0) throw new System.Exception("Uncomplete packet!");

            // ----- Convert TCP -> RTU -----
            if (Mode == mbMode.TCP)
            {
                List<byte> list = packetReceived.ToList();

                var dataLength = (packetReceived[4] << 8) + packetReceived[5];

                if (packetReceived.Length < 6 + dataLength)
                    return false;

                list.RemoveRange(0, 6);                     // remove TCP/IP bytes
                list.Add(0); list.Add(0);                   // add checksum bytes
                packetReceived = list.ToArray();

            }
            // ----- Convert ASCII -> RTU -----
            else if (Mode == mbMode.ASCII)
            {
                try
                {
                    List<byte> list = packetReceived.ToList();
                    while (list.Count > 0 && list[0] != ':')                    // remove chars before message
                    {
                        list.RemoveRange(0, 1);
                    }
                    while (list.Count > 0 && list[list.Count - 1] != '\n')      // remove chars after message
                    {
                        list.RemoveRange(list.Count - 1, 1);
                    }

                    if (list.Count > 0) list.RemoveRange(0, 1);                 // remove ':'
                    if (list.Count > 1) list.RemoveRange(list.Count - 2, 2);    // remove "\r\n"

                    string hexStr = "";
                    List<byte> RTUpacket = new List<byte>();

                    for (int i = 0; i < (list.Count / 2); i++)                  // convert hex to byte
                    {
                        hexStr = "" + (char)list[2 * i];
                        hexStr = hexStr + (char)list[2 * i + 1];
                        RTUpacket.Add(Convert.ToByte(hexStr, 16));
                    }
                    packetReceived = RTUpacket.ToArray();
                }
                catch
                {
                    throw new System.Exception("Hex Converting Error!");
                }
            }

            if (packetReceived.Length < 5) { throw new System.Exception("Uncomplete packet!"); }

            cmd.function = packetReceived[1];

            // ----- Check checksum -----
            switch (cmd.function)
            {
                case 1:
                case 2:
                    processRead();
                    break;
                case 3:
                case 4:
                    if (packetReceived.Length < 2 * cmd.length + 4) { throw new System.Exception("Packet Error!"); }
                    processRead();
                    break;
                case 5:
                case 6:
                case 15:
                case 16:
                    if (packetReceived.Length < 7) { throw new System.Exception("Packet Error!"); }
                    processReply();
                    break;

                case 65:
                case 66:
                    if (packetReceived.Length < 2 * cmd.length + 5) { throw new System.Exception("Packet Error!"); }
                    processRead(true);
                    break;

                default:
                    if (cmd.function > 0x80)
                        processException();
                    else
                        processGetBytes();          //throw new System.Exception("Unknown message!");
                    break;
            }

            return true;
        }

        /// <summary>
        /// Process read data
        /// </summary>
        void processRead(bool extended = false)
        {
            ushort crc = 0, computeCRC = 0;
            int ofs = 0;

            coilList = new bool[0];
            regList = new ushort[0];

            int dataLength = packetReceived[2];
            if (extended)
            {
                dataLength = (ushort)((packetReceived[2] << 8) + packetReceived[3]);
                ofs = 1;
            }
                

            // ----- Check Checksum -----
            if (Mode != mbMode.TCP)
            {
                try
                {
                    if (!extended && dataLength == 0xFF)
                    {
                        crc = (ushort)((packetReceived[packetReceived.Length - 2]) + (packetReceived[packetReceived.Length - 1] << 8));
                        computeCRC = CRC(packetReceived, packetReceived.Length - 2);
                    }
                    else
                    {
                        crc = (ushort)((packetReceived[dataLength + 3 + ofs]) + (packetReceived[dataLength + 4 + ofs] << 8));
                        computeCRC = CRC(packetReceived, dataLength + 3 + ofs);
                    }
                }
                catch
                {
                    throw new System.Exception("Error parse packet!");
                }

                if (computeCRC != crc)
                    throw new System.Exception("Wrong checksum!");
            }

            try
            {
                switch (cmd.function)
                {
                    case 1:         // Read Coils
                    case 2:         // Read Discrete Inputs
                        //coilList = new bool[8 * dataLength];
                        coilList = new bool[cmd.length];
                        for (int i = 0; i < dataLength; i++)
                        {
                            byte oneByte = packetReceived[3 + i + ofs];
                            for (int j = 0; j < 8; j++)
                            {
                                if (8 * i + j >= coilList.Length) break;
                                if ((oneByte & 1) == 1)
                                    coilList[8 * i + j] = true;
                                else coilList[8 * i + j] = false;
                                oneByte >>= 1;
                            }
                        }
                        break;
                    case 3:         // Read Holding Registers
                    case 4:         // Read Input Registers
                    case 65:        // Read Holding Registers Extended
                    case 66:        // Read Input Registers Extended
                        if (dataLength != 0xFF)
                            regList = new ushort[dataLength/2];
                        else
                            regList = new ushort[dataLength / 2];
                        for (int i = 0; i < regList.Length; i++)
                        {
                            regList[i] = (ushort)((packetReceived[3 + ofs + 2 * i] << 8) + (packetReceived[4 + ofs + 2 * i]));
                        }
                        break;

                    default:

                        break;
                }
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }
            
        }

        /// <summary>
        /// Process Reply
        /// </summary>
        void processReply()
        {
            ushort crc = 0, computeCRC = 0;

            // ----- Check Checksum -----
            if (Mode != mbMode.TCP)
            {
                try
                {

                    if (Mode == mbMode.RTU)
                    {
                        crc = (ushort)((packetReceived[6]) + (packetReceived[7] << 8));
                        computeCRC = CRC(packetReceived, 6);
                    }
                    else
                    {
                        crc = packetReceived[6];
                        computeCRC = LRC(packetReceived, 6);
                    }

                }
                catch
                {
                    throw new System.Exception("Error parse packet!");
                }
                if (computeCRC != crc)
                    throw new System.Exception("Wrong checksum!");
            }
        }

        /// <summary>
        /// Process get data bytes
        /// </summary>
        void processGetBytes()
        {
            ushort crc = 0, computeCRC = 0;
            byteList = new byte[0];
            int lenCRC = 0;

            // ----- Check Checksum -----
            if (Mode != mbMode.TCP)
            {
                try
                {
                    if (Mode == mbMode.RTU)
                    {
                        crc = (ushort)((packetReceived[packetReceived.Length - 2]) + (packetReceived[packetReceived.Length - 1] << 8));
                        computeCRC = CRC(packetReceived, packetReceived.Length - 2);
                        lenCRC = 2;
                    }
                    else
                    {
                        crc = packetReceived[packetReceived.Length - 1];
                        computeCRC = LRC(packetReceived, (uint)(packetReceived.Length - 1));
                        lenCRC = 1;
                    }
                }
                catch
                {
                    throw new System.Exception("Error parse packet!");
                }

                if (computeCRC != crc)
                    throw new System.Exception("Wrong checksum!");
            }
            else lenCRC = 2;

            try
            {
                byteList = new byte[packetReceived.Length - 2 - lenCRC];
                int idx = 0;
                for (int i = 2; i < packetReceived.Length - lenCRC; i++)
                {
                    byteList[idx++] = packetReceived[i];
                }
            }
            catch (Exception)
            {
                throw new System.Exception("Error parse packet!");
            }

        }

        /// <summary>
        /// Process Exception
        /// </summary>
        void processException()
        {
            ushort crc = 0, computeCRC = 0;


            // ----- Check Checksum -----
            if (Mode != mbMode.TCP)
            {
                try
                {

                    if (Mode == mbMode.RTU)
                    {
                        crc = (ushort)((packetReceived[3]) + (packetReceived[4] << 8));
                        computeCRC = CRC(packetReceived, 3);
                    }
                    else
                    {
                        crc = packetReceived[3];
                        computeCRC = LRC(packetReceived, 3);
                    }

                }
                catch
                {
                    throw new System.Exception("Error parse packet!");
                }
                if (computeCRC != crc)
                    throw new System.Exception("Wrong checksum!");
            }

            switch (packetReceived[2])
            {
                case 0x01:
                    throw new System.Exception("MODBUS Error: Illegal function");
                case 0x02:
                    throw new System.Exception("MODBUS Error: Illegal data address");
                case 0x03:
                    throw new System.Exception("MODBUS Error: Illegal data value");
                case 0x04:
                    throw new System.Exception("MODBUS Error: Illegal device failure");
                case 0x05:
                    throw new System.Exception("MODBUS Error: Confirmation");
                case 0x06:
                    throw new System.Exception("MODBUS Error: Busy");
                case 0x08:
                    throw new System.Exception("MODBUS Error: Parity error");
                case 0x0A:
                    throw new System.Exception("MODBUS Error: Gateway - Transmission path unavailable");
                case 0x0B:
                    throw new System.Exception("MODBUS Error: Gateway - Target device is not responding");
            }
        }

        #endregion

    }
}
