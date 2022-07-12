using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fx.IO.Protocols
{
    /// <summary>
    /// MODBUS protocol extension class
    /// (Support reading other number types from registers)
    /// Version:        1.0
    /// Date:           2017-05-10
    /// Name:           Martin Fiala
    /// </summary>
    public class ModbusProtocolExt:ModbusProtocol
    {

        #region Integer

        /// <summary>
        /// Read Input registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public int ReadInputInt(ushort address)
        {
            int res = 0;
            ushort[] registers = ReadInputRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetInt(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public int ReadHoldingInt(ushort address)
        {
            int res = 0;
            ushort[] registers = ReadHoldingRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetInt(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input register</param>
        /// <returns></returns>
        public int ReadInt(ushort address, bool input = false)
        {
            if (input)
                return ReadInputInt(address);
            else
                return ReadHoldingInt(address);
        }

        /// <summary>
        /// Write Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Integer value</param>
        public void WriteInt(ushort address, int value)
        {

            WriteRegisters(address, GetRegisters(value));

        }

        #endregion
        
        #region UInteger

        /// <summary>
        /// Read Input registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public uint ReadInputUInt(ushort address)
        {
            uint res = 0;
            ushort[] registers = ReadInputRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetUInt(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public uint ReadHoldingUInt(ushort address)
        {
            uint res = 0;
            ushort[] registers = ReadHoldingRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetUInt(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input register</param>
        /// <returns></returns>
        public uint ReadUInt(ushort address, bool input = false)
        {
            if (input)
                return ReadInputUInt(address);
            else
                return ReadHoldingUInt(address);
        }

        /// <summary>
        /// Write Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Integer value</param>
        public void WriteUInt(ushort address, uint value)
        {

            WriteRegisters(address, GetRegisters(value));

        }

        #endregion
        
        #region Float

        /// <summary>
        /// Read Input registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public float ReadInputFloat(ushort address)
        {
            float res = 0;
            ushort[] registers = ReadInputRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetFloat(registers, 0); 
            }
            return res;
        }

        /// <summary>
        /// Read Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public float ReadHoldingFloat(ushort address)
        {
            float res = 0;
            ushort[] registers = ReadHoldingRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetFloat(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read registers as Float
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input register</param>
        /// <returns></returns>
        public float ReadFloat(ushort address, bool input = false)
        {
            if (input)
                return ReadInputFloat(address);
            else
                return ReadHoldingFloat(address);
        }

        /// <summary>
        /// Write Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Integer value</param>
        public void WriteFloat(ushort address, float value)
        {

            WriteRegisters(address, GetRegisters(value));

        }

        #endregion

        #region Double

        /// <summary>
        /// Read Input registers as Double
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public double ReadInputDouble(ushort address)
        {
            double res = 0;
            ushort[] registers = ReadInputRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetDouble(registers, 0);
            }
            return res;
        }

        /// <summary>
        /// Read Holding registers as Double
        /// </summary>
        /// <param name="address">Register address</param>
        /// <returns>Integer value of registers</returns>
        public double ReadHoldingDouble(ushort address)
        {
            double res = 0;
            ushort[] registers = ReadHoldingRegisters(address, 2);
            if (registers.Length == 2)
            {
                res = GetDouble(registers, 0);
            }
            return res;
        }


        /// <summary>
        /// Read registers as Double
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input register</param>
        /// <returns></returns>
        public double ReadDouble(ushort address, bool input = false)
        {
            if (input)
                return ReadInputDouble(address);
            else
                return ReadHoldingDouble(address);
        }

        /// <summary>
        /// Write Holding registers as Double
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Double value</param>
        public void WriteDouble(ushort address, double value)
        {

            WriteRegisters(address, GetRegisters(value));

        }

        #endregion

        #region String8

        /// <summary>
        /// Read Input registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="length">Register length</param>
        /// <returns>String value of registers</returns>
        public string ReadInputString8(ushort address, ushort length)
        {
            string res = "";
            ushort[] registers = ReadInputRegisters(address, length);
            if (registers.Length == length)
            {
                res = GetString8(registers, 0, length);
            }
            return res;
        }

        /// <summary>
        /// Read Holding registers as String8
        /// </summary>
        /// <param name="address">Register addres</param>
        /// <param name="length">Register length</param>
        /// <returns></returns>
        public string ReadHoldingString8(ushort address, ushort length)
        {
            string res = "";
            ushort[] registers = ReadHoldingRegisters(address, length);
            if (registers.Length == length)
            {
                res = GetString8(registers, 0, length);
            }
            return res;
        }

        /// <summary>
        /// Read registers as String8
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="input">Select Holding/Input register</param>
        /// <returns></returns>
        public string ReadString8(ushort address, ushort length, bool input = false)
        {
            if (input)
                return ReadInputString8(address, length);
            else
                return ReadHoldingString8(address, length);
        }

        /// <summary>
        /// Write Holding registers as String8
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">String value</param>
        /// <param name="maxLength">Max register count</param>
        public void WriteString8(ushort address, string value, ushort maxLength = 0)
        {
            // ----- write registers -----
            WriteRegisters(address, GetRegisters(value, maxLength));

        }

        #endregion

        #region Bytes 


        /// <summary>
        /// Read Input registers as Byte array
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="length">Register length</param>
        /// <returns>String value of registers</returns>
        public byte[] ReadInputBytes(ushort address, ushort length)
        {
            byte[] res = new byte[0];
            ushort[] registers = ReadInputRegisters(address, length);
            if (registers.Length == length)
            {
                res = GetBytes(registers, 0, length);
            }
            return res;
        }


        /// <summary>
        /// Read Holding registers as String8
        /// </summary>
        /// <param name="address">Register addres</param>
        /// <param name="length">Register length</param>
        /// <returns></returns>
        public byte[] ReadHoldingBytes(ushort address, ushort length)
        {
            byte[] res = new byte[0];
            ushort[] registers = ReadHoldingRegisters(address, length);
            if (registers.Length == length)
            {
                res = GetBytes(registers, 0, length);
            }
            return res;
        }

        /// <summary>
        /// Write Holding registers as Int
        /// </summary>
        /// <param name="address">Register address</param>
        /// <param name="value">Integer value</param>
        public void WriteBytes(ushort address, byte[] value)
        {
            // ----- write registers -----
            WriteRegisters(address, GetRegisters(value));
        }


        #endregion

        #region Convert Functions
        
        
        /// <summary>
        /// Get Integer from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <returns>Integer value</returns>
        public int GetInt(ushort[] registers, int position)
        {
            if (registers.Length > (position + 1))
                return (int)((registers[position] << 16) + registers[position + 1]);
            else
                throw new Exception("Wrong position!");
        }

        /// <summary>
        /// Get Integer from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <returns>Integer value</returns>
        public uint GetUInt(ushort[] registers, int position)
        {
            if (registers.Length > (position + 1))
                return (uint)((registers[position] << 16) + registers[position + 1]);
            else
                throw new Exception("Wrong position!");
        }

        /// <summary>
        /// Get Float from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <returns>Float value</returns>
        public float GetFloat(ushort[] registers, int position)
        {
            if (registers.Length > (position + 1))
            {
                byte[] barr = new byte[4];
                barr[0] = (byte)(registers[position + 1] & 0xFF);
                barr[1] = (byte)((registers[position + 1] >> 8) & 0xFF);
                barr[2] = (byte)(registers[position + 0] & 0xFF);
                barr[3] = (byte)((registers[position + 0] >> 8) & 0xFF);
                return System.BitConverter.ToSingle(barr, 0);

            } else
                throw new Exception("Wrong position!");
        }

        /// <summary>
        /// Get Double from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <returns>Float value</returns>
        public double GetDouble(ushort[] registers, int position)
        {
            if (registers.Length > (position + 1))
            {
                byte[] barr = new byte[8];
                barr[0] = (byte)(registers[position + 3] & 0xFF);
                barr[1] = (byte)((registers[position + 3] >> 8) & 0xFF);
                barr[2] = (byte)(registers[position + 2] & 0xFF);
                barr[3] = (byte)((registers[position + 2] >> 8) & 0xFF);
                barr[4] = (byte)(registers[position + 1] & 0xFF);
                barr[5] = (byte)((registers[position + 1] >> 8) & 0xFF);
                barr[6] = (byte)(registers[position + 0] & 0xFF);
                barr[7] = (byte)((registers[position + 0] >> 8) & 0xFF);
                return System.BitConverter.ToDouble(barr, 0);
            }
            else
                throw new Exception("Wrong position!");
        }

        /// <summary>
        /// Get String 8 from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">String length (Register count)</param>
        /// <returns>String value</returns>
        public string GetString8(ushort[] registers, int position, ushort length)
        {
            string res = "";
            byte[] barr = new byte[2];
            if (registers.Length < length + position)
                length = (ushort)(registers.Length - position);
            for (int i = 0; i < length; i++)
            {
                barr[0] = (byte)((registers[i + position] >> 8) & 0xFF);
                if (barr[0] == 0) break;
                res += System.Text.Encoding.Default.GetString(barr, 0, 1);

                barr[1] = (byte)(registers[i + position] & 0xFF);
                if (barr[1] == 0) break;
                res += System.Text.Encoding.Default.GetString(barr, 1, 1);

            }
            return res;
        }

        /// <summary>
        /// Get bytes from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">Byte array length (Register count)</param>
        /// <returns>String value</returns>
        public byte[] GetBytes(ushort[] registers, int position, ushort length)
        {
            byte[] res =  new byte[length*2];
            byte[] barr = new byte[2];
            if (registers.Length < length + position)
                length = (ushort)(registers.Length - position);
            for (int i = 0; i < length; i++)
            {
                barr[0] = (byte)((registers[i + position] >> 8) & 0xFF);
                res[2 * i] += barr[0];

                barr[1] = (byte)(registers[i + position] & 0xFF);
                res[2 * i + 1] += barr[1];
            }
            return res;
        }


        /// <summary>
        /// Get Short array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">Short array length</param>
        /// <returns>String value</returns>
        public short[] GetShortArray(ushort[] registers, int position, ushort length)
        {

            short[] res = new short[length];
            int index = position;

            if (registers.Length < (position + length)) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                res[i] = (short)registers[index++];            }
            return res;
        }

        /// <summary>
        /// Get UShort array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">UShort array length</param>
        /// <returns>String value</returns>
        public ushort[] GetUShortArray(ushort[] registers, int position, ushort length)
        {

            ushort[] res = new ushort[length];
            int index = position;

            if (registers.Length < (position + length)) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                res[i] = registers[index++];
            }
            return res;
        }

        /// <summary>
        /// Get Int array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">Int array length</param>
        /// <returns>String value</returns>
        public int[] GetIntArray(ushort[] registers, int position, ushort length)
        {

            int[] res = new int[length];
            int index = position;

            if (registers.Length < (position + (length * 2))) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                res[i] = (registers[index] << 16) + registers[index + 1];
                index += 2;
            }
            return res;
        }

        /// <summary>
        /// Get UInt array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">UInt array length</param>
        /// <returns>String value</returns>
        public uint[] GetUIntArray(ushort[] registers, int position, ushort length)
        {

            uint[] res = new uint[length];
            int index = position;

            if (registers.Length < (position + (length * 2))) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                res[i] = (uint)((registers[index] << 16) + registers[index + 1]);
                index += 2;
            }
            return res;
        }


        /// <summary>
        /// Get Float array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">UInt array length</param>
        /// <returns>String value</returns>
        public float[] GetFloatArray(ushort[] registers, int position, ushort length)
        {

            float[] res = new float[length];
            int index = position;

            if (registers.Length < (position + (length * 2))) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                byte[] barr = new byte[4];
                barr[0] = (byte)(registers[index + 1] & 0xFF);
                barr[1] = (byte)((registers[index + 1] >> 8) & 0xFF);
                barr[2] = (byte)(registers[index + 0] & 0xFF);
                barr[3] = (byte)((registers[index + 0] >> 8) & 0xFF);
                res[i] = System.BitConverter.ToSingle(barr, 0);
                index += 2;
            }
            return res;
        }


        /// <summary>
        /// Get Double array from Register array
        /// </summary>
        /// <param name="registers">Register Array</param>
        /// <param name="position">Start position</param>
        /// <param name="length">Double array length</param>
        /// <returns>String value</returns>
        public double[] GetDoubleArray(ushort[] registers, int position, ushort length)
        {

            double[] res = new double[length];
            int index = position;

            if (registers.Length < (position + (length * 4))) throw new Exception("Wrong position!");

            for (int i = 0; i < length; i++)
            {
                byte[] barr = new byte[8];
                barr[0] = (byte)(registers[position + 3] & 0xFF);
                barr[1] = (byte)((registers[position + 3] >> 8) & 0xFF);
                barr[2] = (byte)(registers[position + 2] & 0xFF);
                barr[3] = (byte)((registers[position + 2] >> 8) & 0xFF);
                barr[4] = (byte)(registers[position + 1] & 0xFF);
                barr[5] = (byte)((registers[position + 1] >> 8) & 0xFF);
                barr[6] = (byte)(registers[position + 0] & 0xFF);
                barr[7] = (byte)((registers[position + 0] >> 8) & 0xFF);
                res[i] = System.BitConverter.ToDouble(barr, 0);
                index += 4;
            }
            return res;
        }


        /// <summary>
        /// Get Register Array from integer
        /// </summary>
        /// <param name="value">Integer Value</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(int value)
        {
            ushort[] registers = new ushort[2];
            registers[0] = (ushort)((value >> 16) & 0xFFFF);
            registers[1] = (ushort)(value & 0xFFFF);
            return registers;
        }

        /// <summary>
        /// Get Register Array from integer
        /// </summary>
        /// <param name="value">Integer Value</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(uint value)
        {
            ushort[] registers = new ushort[2];
            registers[0] = (ushort)((value >> 16) & 0xFFFF);
            registers[1] = (ushort)(value & 0xFFFF);
            return registers;
        }

        /// <summary>
        /// Get Register Array from float
        /// </summary>
        /// <param name="value">Float Value</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(float value)
        {
            ushort[] registers = new ushort[2];
            byte[] barr = System.BitConverter.GetBytes(value);
            registers[0] = (ushort)((barr[3] << 8) + barr[2]);
            registers[1] = (ushort)((barr[1] << 8) + barr[0]);
            return registers;
        }

        /// <summary>
        /// Get Register Array from double
        /// </summary>
        /// <param name="value">Float Value</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(double value)
        {
            ushort[] registers = new ushort[4];
            byte[] barr = System.BitConverter.GetBytes(value);
            registers[0] = (ushort)((barr[7] << 8) + barr[6]);
            registers[1] = (ushort)((barr[5] << 8) + barr[4]);
            registers[2] = (ushort)((barr[3] << 8) + barr[2]);
            registers[3] = (ushort)((barr[1] << 8) + barr[0]);
            return registers;
        }

        /// <summary>
        /// Get Register Array from String
        /// </summary>
        /// <param name="value">String Value</param>
        /// <param name="maxLength">Max. length (Register count) of string (0 = auto from string length) </param>
        /// <param name="string8">String type (True = 8 bit, False = 16 bit)</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(string value, ushort maxLength = 0,  bool string8 = true)
        {
            // ----- string to byte array -----
            byte[] barr = System.Text.Encoding.Default.GetBytes(value);
            List<byte> list = barr.ToList();
            list.Add(0);
            barr = list.ToArray();

            // ----- get register Length -----
            int len = (barr.Length / 2) + (barr.Length % 2);
            if (maxLength > 0 && len > maxLength)
                len = maxLength;
            ushort[] registers = new ushort[len];

            // ----- fill registers -----
            for (int i = 0; i < len; i++)
            {
                registers[i] = (ushort)(barr[i * 2] << 8);
                try
                {
                    registers[i] += barr[1 + i * 2];
                }
                catch (Exception) { }
            }
            return registers;
        }

        /// <summary>
        /// Get Register Array from Byte array
        /// </summary>
        /// <param name="value">Byte array Value</param>
        /// <returns>Register Array</returns>
        public ushort[] GetRegisters(byte[] value)
        {
            // ----- get register Length -----
            int len = (value.Length / 2) + (value.Length % 2);
            ushort[] registers = new ushort[len];

            // ----- fill registers -----
            for (int i = 0; i < len; i++)
            {
                registers[i] = (ushort)(value[i * 2] << 8);
                try
                {
                    registers[i] += value[1 + i * 2];
                }
                catch (Exception) { }
            }
            return registers;
        }


        #endregion
    }
}
