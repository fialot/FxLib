using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Drawing;

namespace Fx.Conversion
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }

    /// <summary>
    /// Conversion
    /// Version:    1.2
    /// Date:       2017-05-03   
    /// </summary>
    public static class Conv
    {
        #region Testing types

        /// <summary>
        /// Check if string is Short
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is Short</returns>
        public static bool IsShort(string text)
        {
            short value;
            return short.TryParse(text, out value);
        }

        /// <summary>
        /// Check if string is positive Short
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is positive short</returns>
        public static bool IsPositiveShort(string text)
        {
            short value;
            bool res = short.TryParse(text, out value);
            if (res)
                if (value < 0) res = false;
            return res;
        }

        /// <summary>
        /// Check if string is Integer
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is integer</returns>
        public static bool IsInt(string text)
        {
            int value;
            return int.TryParse(text, out value);
        }

        /// <summary>
        /// Check if string is positive Int
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is positive integer</returns>
        public static bool IsPositiveInt(string text)
        {
            int value;
            bool res = int.TryParse(text, out value);
            if (res)
                if (value < 0) res = false;
            return res;
        }

        /// <summary>
        /// Check if string is Float
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is float</returns>
        public static bool IsFloat(string text)
        {
            float value;
            return float.TryParse(text, out value);
        }

        /// <summary>
        /// Check if string is positive Float
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Return True if string is positive float</returns>
        public static bool IsPositiveFloat(string text)
        {
            float value;
            bool res = float.TryParse(text, out value);
            if (res)
                if (value < 0) res = false;
            return res;
        }

        /// <summary>
        /// Check if string is DateTime
        /// </summary>
        /// <param name="text">String</param>
        /// <returns>Return True if string is DateTime</returns>
        public static bool IsDate(string text)
        {
            DateTime value;
            return DateTime.TryParse(text, out value);
        }

        /// <summary>
        /// Check if string is number
        /// </summary>
        /// <param name="Expression">Number string</param>
        /// <returns>Return True if string is number</returns>
        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// Check if string is integer (INT32)
        /// </summary>
        /// <param name="Expression">Number string</param>
        /// <returns>Return True if string is integer</returns>
        public static bool IsInteger(object Expression)
        {
            bool isNum;
            int retNum;

            isNum = Int32.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        #endregion

        #region Convert String -> X

        // ----- Boolean -----


        /// <summary>
        /// Convert String to Bool with false value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static bool ToBool(string text)
        {
            return ToBool(text, false);
        }
        
        /// <summary>
        /// Convert String to Bool with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Short number</returns>
        public static bool ToBool(string text, bool def)
        {
            bool value;
            if (bool.TryParse(text, out value))
                return value;
            else
            {
                if (text == "1")
                {
                    return true;
                }
                else if (text == "0")
                {
                    return false;
                }
                return def;
            } 
        }

        /// <summary>
        /// Convert String to Bool with null
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static bool? ToBoolNull(string text)
        {
            bool value;
            if (bool.TryParse(text, out value))
                return value;
            else
            {
                if (text == "1")
                {
                    return true;
                }
                else if (text == "0")
                {
                    return false;
                }
                return null;
            }
        }

        // ----- Byte -----

        /// <summary>
        /// Convert String to Byte with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static byte ToByte(string text)
        {
            return ToByte(text, 0);
        }

        /// <summary>
        /// Convert String to Byte with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Short number</returns>
        public static byte ToByte(string text, byte def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToByte(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToByte(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                byte value;
                if (byte.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }

        /// <summary>
        /// Convert String to Byte with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static byte? ToByteNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToByte(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToByte(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                byte value;
                if (byte.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- Short -----

        /// <summary>
        /// Convert String to Short with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static short ToShort(string text)
        {
            return ToShort(text, 0);
        }

        /// <summary>
        /// Convert String to Short with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Short number</returns>
        public static short ToShort(string text, short def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt16(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt16(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                short value;
                if (short.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }

        /// <summary>
        /// Convert String to Short with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static short? ToShortNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt16(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt16(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                short value;
                if (short.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- UShort -----

        /// <summary>
        /// Convert String to Unsigned Short with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static ushort ToUShort(string text)
        {
            return ToUShort(text, 0);
        }

        /// <summary>
        /// Convert String to Unsigned Short with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Short number</returns>
        public static ushort ToUShort(string text, ushort def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt16(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt16(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                ushort value;
                if (ushort.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }
        
        /// <summary>
        /// Convert String to Unsigned Short with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Short number</returns>
        public static ushort? ToUShortNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt16(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt16(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                ushort value;
                if (ushort.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- Int -----

        /// <summary>
        /// Convert String to Integer with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Integer number</returns>
        public static int ToInt(string text)
        {
            return ToInt(text, 0);
        }

        /// <summary>
        /// Convert String to Integer with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Integer number</returns>
        public static int ToInt(string text, int def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt32(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt32(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                int value;
                if (int.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }

        /// <summary>
        /// Convert String to Integer with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Integer number</returns>
        public static int? ToIntNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt32(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt32(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                int value;
                if (int.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- UInt -----

        /// <summary>
        /// Convert String to Unsigned Integer with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Integer number</returns>
        public static uint ToUInt(string text)
        {
            return ToUInt(text, 0);
        }

        /// <summary>
        /// Convert String to Unsigned Integer with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Integer number</returns>
        public static uint ToUInt(string text, uint def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt32(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt32(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                uint value;
                if (uint.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }
        
        /// <summary>
        /// Convert String to Unsigned Integer with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Integer number</returns>
        public static uint? ToUIntNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt32(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt32(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                uint value;
                if (uint.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- Long -----

        /// <summary>
        /// Convert String to Int64 with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Long number</returns>
        public static long ToLong(string text)
        {
            return ToLong(text, 0);
        }

        /// <summary>
        /// Convert String to Int64 with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Long number</returns>
        public static long ToLong(string text, long def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt64(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt64(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                long value;
                if (long.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }

        /// <summary>
        /// Convert String to Int64
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Long number</returns>
        public static long? ToLongNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToInt64(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToInt64(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                long value;
                if (long.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- ULong -----

        /// <summary>
        /// Convert String to UInt64 with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Long number</returns>
        public static ulong ToULong(string text)
        {
            return ToULong(text, 0);
        }

        /// <summary>
        /// Convert String to UInt64 with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Long number</returns>
        public static ulong ToULong(string text, ulong def)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt64(text, 2);
                }
                catch
                {
                    return def;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt64(text, 16);
                }
                catch
                {
                    return def;
                }
            }
            else
            {
                ulong value;
                if (ulong.TryParse(text, out value))
                    return value;
                else
                    return def;
            }
        }

        /// <summary>
        /// Convert String to UInt64
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Long number</returns>
        public static ulong? ToULongNull(string text)
        {
            if (text.ToLower().IndexOf("0b") == 0)
            {
                try
                {
                    text = text.Remove(0, 2);
                    return Convert.ToUInt64(text, 2);
                }
                catch
                {
                    return null;
                }
            }
            else if (text.ToLower().IndexOf("0x") == 0)
            {
                try
                {
                    return Convert.ToUInt64(text, 16);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                ulong value;
                if (ulong.TryParse(text, out value))
                    return value;
                else
                    return null;
            }
        }

        // ----- Float -----

        /// <summary>
        /// Convert String to Float with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Float number</returns>
        public static float ToFloat(string text)
        {
            return ToFloat(text, 0);
        }
        
        /// <summary>
        /// Convert String to Float with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Float number</returns>
        public static float ToFloat(string text, float def)
        {
            float value;
            if (float.TryParse(text, out value))
                return value;
            else
                return def;
        }

        /// <summary>
        /// Convert String to Float (invariant) with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Float number</returns>
        public static float ToFloatI(string text)
        {
            return ToFloatI(text, 0);
        }

        /// <summary>
        /// Convert String to Float (invariant) with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Float number</returns>
        public static float ToFloatI(string text, float def)
        {
            try
            {
                return Convert.ToSingle(text, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception)
            {
                return def;
            }
        }

        /// <summary>
        /// Convert String to Float
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Float number</returns>
        public static float? ToFloatNull(string text)
        {
            float value;
            if (float.TryParse(text, out value))
                return value;
            else
                return null;
        }

        /// <summary>
        /// Convert String to Float (invariant)
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Float number</returns>
        public static float? ToFloatINull(string text)
        {
            try
            {
                return Convert.ToSingle(text, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ----- Double -----

        /// <summary>
        /// Convert String to Double with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Double number</returns>
        public static double ToDouble(string text)
        {
            return ToDouble(text, 0);
        }

        /// <summary>
        /// Convert String to Double with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double ToDouble(string text, double def)
        {
            double value;
            if (double.TryParse(text, out value))
                return value;
            else
                return def;
        }
        
        /// <summary>
        /// Convert String to Double with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double ToDouble(string text, double def, NumberFormatInfo format)
        {
            if (IsNumeric(text))
                return Convert.ToDouble(text, format);
            else return def;
        }

        /// <summary>
        /// Convert String (Invariant number format - '.')  to Double with zero value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <returns>Double number</returns>
        public static double ToDoubleI(string text)
        {
            return ToDoubleI(text, 0);
        }

        /// <summary>
        /// Convert String (Invariant number format - '.')  to Double with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double ToDoubleI(string text, double def)
        {
            if (IsNumeric(text))
                return Convert.ToDouble(text, NumberFormatInfo.InvariantInfo);
            else return def;
        }
        
        /// <summary>
        /// Convert String to Double with default value on convert Error
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double? ToDoubleNull(string text)
        {
            double value;
            if (double.TryParse(text, out value))
                return value;
            else
                return null;
        }
        
        /// <summary>
        /// Convert String to Double
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double? ToDoubleNull(string text, NumberFormatInfo format)
        {
            if (IsNumeric(text))
                return Convert.ToDouble(text, format);
            else return null;
        }

        /// <summary>
        /// Convert String (Invariant number format - '.')  to Double
        /// </summary>
        /// <param name="text">Number string</param>
        /// <param name="def">Default value</param>
        /// <returns>Double number</returns>
        public static double? ToDoubleINull(string text)
        {
            if (IsNumeric(text))
                return Convert.ToDouble(text, NumberFormatInfo.InvariantInfo);
            else return null;
        }


        // ----- Numbers -----
        /// <summary>
        /// Convert to number without 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long ToNumber(string val)
        {
            string clean = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] >= '0' && val[i] <= '9')
                    clean += val[i];
            }
            return (long)ToLong(clean, 0);
        }

        /// <summary>
        /// Convert to number without 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double ToRealNumber(string val)
        {
            val = val.Replace(",", ".");
            string clean = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] >= '0' && val[i] <= '9' || val[i] == '.')
                    clean += val[i];
            }
            return ToDoubleI(clean, 0);
        }

        // ----- Other -----
        /// <summary>
        /// String to GUID
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>GUID</returns>
        public static Guid ToGuid(string text)
        {
            Guid ID;
            try
            {
                ID = Guid.Parse(text);
            }
            catch
            {
                ID = Guid.Empty;
            }
            return ID;
        }

        /// <summary>
        /// String to DateTime
        /// </summary>
        /// <param name="text">date time text</param>
        /// <returns></returns>
        public static DateTime ToDateTime(string text)
        {
            DateTime date;
            try
            {
                date = DateTime.Parse(text);
            }
            catch
            {
                date = DateTime.MinValue;
            }
            return date;
        }

        /// <summary>
        /// String to DateTime
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeNull(string text)
        {
            DateTime? date;
            try
            {
                date = DateTime.Parse(text);
            }
            catch
            {
                date = null;
            }
            return date;
        }

        /// <summary>
        /// String to DateTime Invariant ("yyyy-MM-dd HH:mm:ss")
        /// </summary>
        /// <param name="text">date time text</param>
        /// <returns></returns>
        public static DateTime ToDateTimeI(string text)
        {
            DateTime date;
            bool isDate = DateTime.TryParseExact(text, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
            if (!isDate) date = DateTime.MinValue;
            return date;
        }

        /// <summary>
        /// String to DateTime Invariant ("yyyy-MM-dd HH:mm:ss")
        /// </summary>
        /// <param name="text">date time text</param>
        /// <returns></returns>
        public static DateTime? ToDateTimeINull(string text)
        {
            DateTime date;
            bool isDate = DateTime.TryParseExact(text, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
            if (!isDate) return null;
            return date;
        }

        /// <summary>
        /// String to encoding
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Encoding ToEncoding(string value)
        {
            return ToEncoding(value, Encoding.UTF8);
        }

        /// <summary>
        /// String to encoding
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Encoding ToEncoding(string value, Encoding defaultValue)
        {
            try
            {
                return Encoding.GetEncoding(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// String to Color
        /// </summary>
        /// <param name="value">String color (Name or HEX)</param>
        /// <returns>Color</returns>
        public static Color ToColor(string value)
        {
            return ToColor(value, Color.White);
        }

        /// <summary>
        /// String to Color
        /// </summary>
        /// <param name="value">String color (Name or HEX)</param>
        /// <param name="def">Default color</param>
        /// <returns>Color</returns>
        public static Color ToColor(string value, Color def)
        {
            try
            {
                return Color.FromArgb(Int32.Parse(value, System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
                try
                {
                    return Color.FromName(value);
                }
                catch
                {
                    return def;
                }
            }
        }

        /// <summary>
        /// String to Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">String value</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (Exception)
            {
                return defaultValue;
            }

        }

        #endregion

        #region Convert X -> String 
        
        /// <summary>
        /// Return bool value like "0" or "1"
        /// </summary>
        /// <param name="value">Bool value</param>
        /// <returns>String representation of bool</returns>
        public static string ToString(bool value)
        {
            if (value == true)
                return "1";
            else
                return "0";
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(bool[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(short[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(ushort[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(int[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(uint[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(long[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(ulong[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(float[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(double[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }

        /// <summary>
        /// Array to string
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string ToString(string[] array, string separator = ";")
        {
            return ArrayToString(array, separator);
        }
        
        /// <summary>
        /// Return Net Mask from int value
        /// </summary>
        /// <param name="value">Integer value from mask</param>
        /// <returns></returns>
        public static string ToIPMask(int value)
        {
            string res = "";
            uint ires = 0;
            if (value < 0 || value > 32) return "";

            for (int i = 0; i < value; i++ )
            {
                ires <<= 1;
                ires += 1;
            }
            for (int i = 0; i < 32 - value; i++)
            {
                ires <<= 1;
            }
            //res = ires.ToString("x");

            byte[] x = BitConverter.GetBytes(ires);
            res = "";
            for (int i = x.Length-1; i >= 0; i--)
            {
                if (res.Length > 0) res += ".";
                res += x[i].ToString();
            }

            return res;
        }


        /// <summary>
        /// Array to string separated by defined char
        /// </summary>
        /// <param name="Expression">Array</param>
        /// <param name="separator">Separator</param>
        /// <returns>String</returns>
        private static string ArrayToString(object Expression, string separator = ";")
        {
            string res = "";
            if (Expression == null) return "";
            if (Expression.GetType() == typeof(bool[]))
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
            else if (Expression.GetType() == typeof(short[]))
            {
                short[] exp = (short[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(ushort[]))
            {
                ushort[] exp = (ushort[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(int[]))
            {
                int[] exp = (int[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(uint[]))
            {
                uint[] exp = (uint[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(long[]))
            {
                long[] exp = (long[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(ulong[]))
            {
                ulong[] exp = (ulong[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(float[]))
            {
                float[] exp = (float[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
            }
            else if (Expression.GetType() == typeof(double[]))
            {
                double[] exp = (double[])Expression;
                if (exp.Length > 0) res = exp[0].ToString();
                for (int i = 1; i < exp.Length; i++) res += separator + exp[i].ToString();
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

        #endregion

        #region Convert X -> Array

        /// <summary>
        /// String to Bool array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Bool array</returns>
        public static bool[] ToBoolArray(string value, string separator = ";")
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
        
        /// <summary>
        /// String to Byte array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Short array</returns>
        public static byte[] ToByteArray(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            byte[] res = new byte[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToByte(separate[i]);
                }
            }
            catch
            {
                res = new byte[0];
            }

            return res;
        }
        
        /// <summary>
        /// String to Short array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Short array</returns>
        public static short[] ToInt16Array(string value, string separator = ";")
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
        public static ushort[] ToUInt16Array(string value, string separator = ";")
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
        /// Convert byte array to ushort array
        /// </summary>
        /// <param name="byteArray">Byte array</param>
        /// <returns>UShort array</returns>
        public static ushort[] ToUInt16Array(byte[] byteArray, int startIndex = 0)
        {
            ushort[] res;
            try
            {
                // ----- Get length -----
                int length = byteArray.Length - startIndex;

                // ----- Check if even length -----
                if (length % 2 == 1)
                {
                    length -= 1;
                }

                res = new ushort[length / 2];
                for (int i = 0; i < length / 2; i++)
                {
                    res[i] = BitConverter.ToUInt16(byteArray, startIndex + i * 2);
                }
                return res;
            }
            catch
            {
                return new ushort[0];
            }
        }

        /// <summary>
        /// String to Int array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Int array</returns>
        public static int[] ToInt32Array(string value, string separator = ";")
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
        public static uint[] ToUInt32Array(string value, string separator = ";")
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
        /// String to Long array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Int array</returns>
        public static long[] ToInt64Array(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            long[] res = new long[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToInt64(separate[i]);
                }
            }
            catch
            {
                res = new long[0];
            }

            return res;
        }

        /// <summary>
        /// String to ULong array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Int array</returns>
        public static ulong[] ToUInt64Array(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            ulong[] res = new ulong[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToUInt64(separate[i]);
                }
            }
            catch
            {
                res = new ulong[0];
            }

            return res;
        }
        
        /// <summary>
        /// String to Float array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Float array</returns>
        public static float[] ToFloatArray(string value, string separator = ";")
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
        /// String to Double array
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="separator">Separator</param>
        /// <returns>Float array</returns>
        public static double[] ToDoubleArray(string value, string separator = ";")
        {
            string[] separate = value.Split(new string[] { separator }, StringSplitOptions.None);
            double[] res = new double[separate.Length];
            try
            {
                for (int i = 0; i < separate.Length; i++)
                {
                    res[i] = Convert.ToDouble(separate[i]);
                }
            }
            catch
            {
                res = new double[0];
            }

            return res;
        }

        #endregion

        #region Convert Image <-> Array

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


        #region Swap

        /// <summary>
        /// Swaping bytes in short
        /// </summary>
        /// <param name="x">Short number</param>
        /// <returns></returns>
        public static short SwapBytes(short x)
        {
            return (short)SwapBytes((ushort)x);
        }

        /// <summary>
        /// Swaping bytes in ushort
        /// </summary>
        /// <param name="x">UShort number</param>
        /// <returns></returns>
        public static ushort SwapBytes(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        /// <summary>
        /// Swaping bytes in int
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static int SwapBytes(int x)
        {
            return (int)SwapBytes((uint)x);
        }

        /// <summary>
        /// Swaping bytes in uint
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static uint SwapBytes(uint x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        /// <summary>
        /// Swaping bytes in long
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static long SwapBytes(long x)
        {
            return (long)SwapBytes((ulong)x);
        }

        /// <summary>
        /// Swaping bytes in ulong
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static ulong SwapBytes(ulong x)
        {
            // swap adjacent 32-bit blocks
            x = (x >> 32) | (x << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }


        /// <summary>
        /// Swaping bytes in float
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static float SwapBytes(float x)
        {
            var bytes = BitConverter.GetBytes(x);
            byte[] result = new byte[4];

            result[0] = bytes[3];
            result[1] = bytes[2];
            result[2] = bytes[1];
            result[3] = bytes[0];

            return BitConverter.ToSingle(result, 0);
        }

        /// <summary>
        /// Swaping bytes in float
        /// </summary>
        /// <param name="x">Number</param>
        /// <returns></returns>
        public static double SwapBytes(double x)
        {
            var bytes = BitConverter.GetBytes(x);
            byte[] result = new byte[8];

            result[0] = bytes[7];
            result[1] = bytes[6];
            result[2] = bytes[5];
            result[3] = bytes[4];
            result[4] = bytes[3];
            result[5] = bytes[2];
            result[6] = bytes[1];
            result[7] = bytes[0];

            return BitConverter.ToSingle(result, 0);
        }



        #endregion

        #region From Hex convert

        /// <summary>
        /// Convert Hex to Uint with default value
        /// </summary>
        /// <param name="hex">Hex number</param>
        /// <param name="def">Dafault value if convert error</param>
        /// <returns>UInt value</returns>
        public static uint HexToUInt(string hex, uint def = 0)
        {
            if (hex.Length > 8) return def;
            while (hex.Length % 8 > 0) hex = '0' + hex;

            try
            {
                return uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            } 
            catch (Exception)
            {
                return def;
            }
        }

        /// <summary>
        /// Convert Hex to byte array
        /// </summary>
        /// <param name="hex">Hex number</param>
        /// <returns>Byte array</returns>
        public static byte[] HexToBytes(string hex)
        {
            /*return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();*/
            if (hex.Length % 2 == 1) hex = '0' + hex;
            try
            {
                return Enumerable.Range(0, hex.Length / 2).Select(x => Byte.Parse(hex.Substring(2 * x, 2), System.Globalization.NumberStyles.HexNumber)).ToArray();
            } catch (Exception)
            {
                return new byte[0];
            }
            
        }
		
		/// <summary>
        /// Convert Hex to byte
        /// </summary>
        /// <param name="hex">Hex number</param>
        /// <returns>Byte number</returns>
        public static byte HexToByte(string hex)
        {
            /*return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();*/
            if (hex.Length % 2 == 1) hex = '0' + hex;
            try
            {
                byte[] res = Enumerable.Range(0, hex.Length / 2).Select(x => Byte.Parse(hex.Substring(2 * x, 2), System.Globalization.NumberStyles.HexNumber)).ToArray();
                if (res.Length > 0)
                    return res[0];
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// Convert Hex to ushort array
        /// </summary>
        /// <param name="hex">Hex number</param>
        /// <returns>UShort array</returns>
        public static ushort[] HexToUShorts(string hex)
        {
            while (hex.Length % 4 > 0) hex = '0' + hex;
            
            return Enumerable.Range(0, hex.Length / 4).Select(x => ushort.Parse(hex.Substring(4 * x, 4), System.Globalization.NumberStyles.HexNumber)).ToArray();
        }

        #endregion
    }
}
