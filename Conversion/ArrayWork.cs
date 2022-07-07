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
    public static class ArrayWork
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


    }
}
