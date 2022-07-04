using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace Fx.Conversion
{

    /// <summary>
    /// Conversion
    /// Version:    1.2
    /// Date:       2017-05-03   
    /// </summary>
    public static class ArrayConv
    {
        #region Remove 

        #region RemoveAt

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static byte[] RemoveAt(byte[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static short[] RemoveAt(short[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static ushort[] RemoveAt(ushort[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static int[] RemoveAt(int[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static uint[] RemoveAt(uint[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static long[] RemoveAt(long[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }
        
        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static ulong[] RemoveAt(ulong[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }
        
        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static float[] RemoveAt(float[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static double[] RemoveAt(double[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }
        
        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static char[] RemoveAt(char[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }
        
        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="index">Index</param>
        /// <returns>Array</returns>
        public static string[] RemoveAt(string[] array, int index)
        {
            var list = array.ToList();
            try
            {
                list.RemoveAt(index);
            }
            catch { }
            return list.ToArray();
        }

        #endregion

        #region Remove Values

        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static byte[] RemoveValues(byte[] array, byte value)
        {
            List<byte> list = new List<byte>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static short[] RemoveValues(short[] array, short value)
        {
            List<short> list = new List<short>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static ushort[] RemoveValues(ushort[] array, ushort value)
        {
            List<ushort> list = new List<ushort>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static int[] RemoveValues(int[] array, int value)
        {
            List<int> list = new List<int>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static uint[] RemoveValues(uint[] array, uint value)
        {
            List<uint> list = new List<uint>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static long[] RemoveValues(long[] array, long value)
        {
            List<long> list = new List<long>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static ulong[] RemoveValues(ulong[] array, ulong value)
        {
            List<ulong> list = new List<ulong>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static float[] RemoveValues(float[] array, float value)
        {
            List<float> list = new List<float>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static double[] RemoveValues(double[] array, double value)
        {
            List<double> list = new List<double>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }
        
        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static char[] RemoveValues(char[] array, char value)
        {
            List<char> list = new List<char>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Remove all values at array
        /// </summary>
        /// <param name="array">Source array</param>
        /// <param name="value">Value</param>
        /// <returns>Array</returns>
        public static string[] RemoveValues(string[] array, string value)
        {
            List<string> list = new List<string>();

            foreach (var item in array)
            {
                if (item != value) list.Add(item);
            }

            return list.ToArray();
        }

        #endregion

        #endregion



        #region Image

        /// <summary>
        /// Convert Image to byte array
        /// </summary>
        /// <param name="imageIn">Image</param>
        /// <returns>Array</returns>
        public static byte[] ToArray(Image imageIn)
        {
            try
            {
                if (imageIn != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        imageIn.Save(ms, imageIn.RawFormat);
                        return ms.ToArray();
                    }
                }
                else return new byte[0];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Convert Array to image
        /// </summary>
        /// <param name="array">Array</param>
        /// <returns>Image</returns>
        public static Image ToImage(byte[] array)
        {
            try
            {
                if (array != null)
                {
                    using (var ms = new MemoryStream(array))
                    {
                        return Image.FromStream(ms);
                    }
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Convert

        /// <summary>
        /// Convert byte array to ushort array
        /// </summary>
        /// <param name="byteArr">Byte array</param>
        /// <returns>UShort array</returns>
        public static ushort[] ToUShort(byte[] byteArr)
        {
            try
            {
                if (byteArr.Length % 2 == 1)
                {
                    Array.Resize(ref byteArr, byteArr.Length + 1);
                    for (int i = byteArr.Length - 1; i > 0; i--)
                    {
                        byteArr[i] = byteArr[i - 1];
                    }
                    byteArr[0] = 0;
                }
                ushort[] res = new ushort[byteArr.Length / 2];

                for (int i = 0; i < byteArr.Length / 2; i++)
                {
                    res[i] = BitConverter.ToUInt16(byteArr, i * 2);
                }
                return res;
            }
            catch
            {
                return new ushort[0];
            }

        }

        #endregion

        #region Array

        /// <summary>
        /// Array to string separated by defined char
        /// </summary>
        /// <param name="Expression">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns>String</returns>
        public static string ArrToStr(object Expression, string separator = "; ")
        {
            string res = "";
            if (Expression == null) return "";
            if (Expression.GetType() == typeof(ushort[]))
            {
                ushort[] exp = (ushort[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(uint[]))
            {
                uint[] exp = (uint[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(int[]))
            {
                int[] exp = (int[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(float[]))
            {
                float[] exp = (float[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(bool[]))
            {
                string boolStr;
                bool[] exp = (bool[])Expression;
                if (exp.Length > 0)
                {
                    if (exp[0] == true)
                        boolStr = "1";
                    else
                        boolStr = "0";
                    res = boolStr;
                }
                for (int i = 1; i < exp.Length; i++)
                {
                    if (exp[i] == true)
                        boolStr = "1";
                    else
                        boolStr = "0";
                    res += separator + boolStr;
                }
            }
            else if (Expression.GetType() == typeof(string[]))
            {
                string[] exp = (string[])Expression;
                if (exp.Length > 0) res = exp[0];
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i];
            }
            else
                res = Expression.ToString();
            return res;
        }

        /// <summary>
        /// String to Short array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Short array</returns>
        public static short[] ToInt16Arr(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            short[] res = new short[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToInt16(separate[i]);
                }
            }
            catch
            {
                res = new short[0];
            }

            return res;
        }

        /// <summary>
        /// String to UShort array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>UShort array</returns>
        public static ushort[] ToUInt16Arr(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            ushort[] res = new ushort[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToUInt16(separate[i]);
                }
            }
            catch
            {
                res = new ushort[0];
            }

            return res;
        }

        /// <summary>
        /// String to Int array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Int array</returns>
        public static int[] ToInt32Arr(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            int[] res = new int[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToInt32(separate[i]);
                }
            }
            catch
            {
                res = new int[0];
            }

            return res;
        }

        /// <summary>
        /// String to UInt array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>UInt array</returns>
        public static uint[] ToUInt32Arr(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            uint[] res = new uint[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToUInt32(separate[i]);
                }
            }
            catch
            {
                res = new uint[0];
            }

            return res;
        }

        /// <summary>
        /// String to Float array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Float array</returns>
        public static float[] ToFloatArr(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            float[] res = new float[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToSingle(separate[i]);
                }
            }
            catch
            {
                res = new float[0];
            }

            return res;
        }

        /// <summary>
        /// String to Bool array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Bool array</returns>
        public static bool[] StrToBool(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            bool[] res = new bool[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    if (separate[i].ToLower().Trim() == "true" || separate[i].Trim() == "1")
                        res[i] = true;
                    else
                        res[i] = false;
                }
            }
            catch
            {
                res = new bool[0];
            }

            return res;
        }

        #endregion

    }
}
