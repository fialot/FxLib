using Fx.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO
{
    public class CommBuffer
    {
        public int Position { get; set; } = 0;


        List<byte> buffer = new List<byte>();

        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        public CommBuffer()
        {

        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="array">Input array</param>
        public CommBuffer(byte[] array)
        {
            buffer = array.ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="list">Input list</param>
        public CommBuffer(List<byte> list)
        {
            buffer = list.ToList();
        }

        #endregion

        public int Length()
        {
            return buffer.Count;
        }

        public void Clear()
        {
            buffer.Clear();
        }

        public List<byte> ToList()
        {
            return buffer.ToList();
        }

        public byte[] ToArray()
        {
            return buffer.ToArray();
        }


        void Resize(int size)
        {
            int length = size - buffer.Count;
            if (length > 0)
            {
                buffer.AddRange(new byte[length]);
            }
        }

        #region Put

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(byte number)
        {
            var len = PutAt(Position, number);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(short number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(ushort number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int PutInt24(int number, bool bigEndian = false)
        {
            var len = PutInt24At(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int PutUInt24(uint number, bool bigEndian = false)
        {
            var len = PutUInt24At(Position, number, bigEndian);
            Position += len;
            return len;
        }
        
        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(int number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(uint number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }
        
        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(long number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(ulong number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(float number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="number">Number</param>
        public int Put(double number, bool bigEndian = false)
        {
            var len = PutAt(Position, number, bigEndian);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="array">Byte array</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="length">Length</param>
        public int Put(byte[] array, int startIndex = 0, int length = -1)
        {
            int len = PutAt(Position, array, startIndex, length);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="text">Text</param>
        public int Put(string text)
        {
            var len = Put(text, Encoding.UTF8);
            Position += len;
            return len;
        }

        /// <summary>
        /// Put to buffer
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="encoding">Used encoding</param>
        public int Put(string text, Encoding encoding)
        {
            var len = PutAt(Position, text, Encoding.UTF8);
            Position += len;
            return len;
        }

        #endregion

        #region Put At

        public int PutAt(int position, byte number)
        {
            int length = 1;

            // ----- Resize buffer -----
            Resize(position + length);

            // ----- Write to buffer -----
            buffer[position] = number;

            // ----- Retrun writed length -----
            return length;
        }

        public int PutAt(int position, short number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);
            
            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, ushort number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutInt24At(int position, int number, bool bigEndian = false)
        {
            int offset = 0;
            // ----- Convert to bytes -----
            if (bigEndian)
            {
                number = Conv.SwapBytes(number);
                offset = 1;
            }

            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + 3);

            // ----- Write to buffer -----
            for (int i = 0; i < 3; i++)
            {
                buffer[position + i] = array[i + offset];
            }

            // ----- Return writed length -----
            return 3;
        }

        public int PutUInt24At(int position, uint number, bool bigEndian = false)
        {
            int offset = 0;
            // ----- Convert to bytes -----
            if (bigEndian)
            {
                number = Conv.SwapBytes(number);
                offset = 1;
            }
                
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + 3);

            // ----- Write to buffer -----
            for (int i = 0; i < 3; i++)
            {
                buffer[position + i] = array[i + offset];
            }

            // ----- Return writed length -----
            return 3;
        }

        public int PutAt(int position, int number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, uint number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, long number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, ulong number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, float number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, double number, bool bigEndian = false)
        {
            // ----- Convert to bytes -----
            if (bigEndian)
                number = Conv.SwapBytes(number);
            var array = BitConverter.GetBytes(number);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, byte[] array, int startIndex = 0, int length = -1)
        {
            // ----- Check size -----
            if ((length < 0) || (length > array.Length - startIndex))
            {
                length = array.Length - startIndex;
            }

            // ----- Resize buffer -----
            Resize(position + length);

            // ----- Write to buffer -----
            for (int i = 0; i < length; i++)
            {
                buffer[position + i] = array[startIndex + i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        public int PutAt(int position, string text)
        {
            return PutAt(position, text, Encoding.UTF8);
        }

        public int PutAt(int position, string text, Encoding encoding)
        {
            // ----- Convert to bytes -----
            var array = encoding.GetBytes(text);

            // ----- Resize buffer -----
            Resize(position + array.Length);

            // ----- Write to buffer -----
            for (int i = 0; i < array.Length; i++)
            {
                buffer[position + i] = array[i];
            }

            // ----- Return writed length -----
            return array.Length;
        }

        #endregion

        #region Get

        // ----- Reading functions -----
        public byte Get()
        {
            return GetAt(Position);
        }

        public short GetInt16(bool bigEndian = false)
        {
            return GetInt16At(Position, bigEndian);
        }

        public ushort GetUInt16(bool bigEndian = false)
        {
            return GetUInt16At(Position, bigEndian);
        }

        public int GetInt24(bool bigEndian = false)
        {
            return GetInt24At(Position, bigEndian);
        }

        public uint GetUInt24(bool bigEndian = false)
        {
            return GetUInt24At(Position, bigEndian);
        }

        public int GetInt32(bool bigEndian = false)
        {
            return GetInt32At(Position, bigEndian);
        }

        public uint GetUInt32(bool bigEndian = false)
        {
            return GetUInt32At(Position, bigEndian);
        }

        public long GetInt64(bool bigEndian = false)
        {
            return GetInt64At(Position, bigEndian);
        }

        public ulong GetUInt64(bool bigEndian = false)
        {
            return GetUInt64At(Position, bigEndian);
        }

        public float GetFloat(bool bigEndian = false)
        {
            return GetFloatAt(Position, bigEndian);
        }

        public double GetDouble(bool bigEndian = false)
        {
            return GetDoubleAt(Position, bigEndian);
        }

        public string GetString(int length)
        {
            return GetStringAt(Position, length);
        }

        public string GetString(int length, Encoding encoding)
        {
            return GetStringAt(Position, length, encoding);
        }

        public byte[] GetBytes(int length = -1)
        {
            return GetBytesAt(Position, length);
        }

        #endregion

        public byte GetAt(int position)
        {
            // ----- Check corect position -----
            if (position + 1 > buffer.Count) return 0;

            return buffer[position];
        }

        public short GetInt16At(int position, bool bigEndian = false)
        {
            int length = 2;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToInt16(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public ushort GetUInt16At(int position, bool bigEndian = false)
        {
            int length = 2;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToUInt16(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public int GetInt24At(int position, bool bigEndian = false)
        {
            int length = 3;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var list = buffer.GetRange(position, length);
            if (bigEndian)
                list.Insert(0, 0);
            else
                list.Add(0);
                var number = BitConverter.ToInt32(list.ToArray(), 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public uint GetUInt24At(int position, bool bigEndian = false)
        {
            int length = 3;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var list = buffer.GetRange(position, length);
            if (bigEndian)
                list.Insert(0, 0);
            else
                list.Add(0);
            var number = BitConverter.ToUInt32(list.ToArray(), 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public int GetInt32At(int position, bool bigEndian = false)
        {
            int length = 4;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToInt32(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public uint GetUInt32At(int position, bool bigEndian = false)
        {
            int length = 4;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToUInt32(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public long GetInt64At(int position, bool bigEndian = false)
        {
            int length = 8;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToInt64(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public ulong GetUInt64At(int position, bool bigEndian = false)
        {
            int length = 8;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToUInt64(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public float GetFloatAt(int position, bool bigEndian = false)
        {
            int length = 4;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToSingle(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public double GetDoubleAt(int position, bool bigEndian = false)
        {
            int length = 8;
            // ----- Check corect position -----
            if (position + length > buffer.Count) return 0;

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();
            var number = BitConverter.ToDouble(bytes, 0);

            // ----- Swap if big endian -----
            if (bigEndian)
                number = Conv.SwapBytes(number);

            // ----- Return number -----
            return number;
        }

        public string GetStringAt(int position, int length)
        {
            return GetStringAt(position, length, Encoding.UTF8);
        }

        public string GetStringAt(int position, int length, Encoding encoding)
        {
            // ----- Check corect position -----
            if (position + length > buffer.Count) return "";

            // ----- Get numeber -----
            var bytes = buffer.GetRange(position, length).ToArray();

            return encoding.GetString(bytes);
        }

        public byte[] GetBytesAt(int position, int length = -1)
        {
            if (length < 0) length = buffer.Count - position;

            return buffer.GetRange(position, length).ToArray();
        }



    }
}
