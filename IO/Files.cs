using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;

namespace Fx.IO
{
    
    /// <summary>
    /// Files functions
    /// Version:    1.2
    /// Date:       2022-07-04 
    /// </summary>
    public static class Files
    {

        #region Read File

        /// <summary>
        /// Read text file
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns>Return text string</returns>
        public static string Read(string fileName)
        {
            string res = "";

            try
            {
                using (StreamReader reader = new StreamReader(fileName, true))
                {
                    res = reader.ReadToEnd();
                }
            }
            catch
            {
            }
            return res;
        }

        /// <summary>
        /// Read text file
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="enc">Encoding of file</param>
        /// <returns>Return text string</returns>
        public static string Read(string fileName, Encoding enc)
        {
            string res = "";

            try
            {
                using (StreamReader reader = new StreamReader(fileName, enc, true))
                {
                    res = reader.ReadToEnd();
                }
            }
            catch
            {
            }
            return res;
        }

        /// <summary>
        /// Read text from stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Return text string</returns>
        public static string Read(Stream stream)
        {
            string res = "";

            try
            {
                using (StreamReader reader = new StreamReader(stream, true))
                {
                    res = reader.ReadToEnd();
                }
            }
            catch
            {
            }
            return res;
        }

        /// <summary>
        /// Read text from stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="enc">Encoding of file</param>
        /// <returns>Return text string</returns>
        public static string Read(Stream stream, Encoding enc)
        {
            string res = "";

            try
            {
                using (StreamReader reader = new StreamReader(stream, enc, true))
                {
                    res = reader.ReadToEnd();
                }
            }
            catch
            {
            }
            return res;
        }

        /// <summary>
        /// Read binary file
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string fileName)
        {
            try
            {
                return System.IO.File.ReadAllBytes(fileName);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        

/// <summary>
/// Read text file lines
/// </summary>
/// <param name="fileName">Filename</param>
/// <returns>Array of lines</returns>
public static string[] ReadLines(string fileName, bool removeEmptyLines = false)
        {
            string res = Read(fileName);

            if (res.Length > 0)
                if (removeEmptyLines)
                    return res.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                else
                    return res.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            else return new string[0];
        }

        #endregion

        #region Save Files

        /// <summary>
        /// Save binary file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="bin">Binary data</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(string filename, byte[] bin)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(bin);
                        writer.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save binary file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="bin">Binary data</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(Stream stream, byte[] bin)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(bin);
                    writer.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="text">Text to saving</param>
        /// <param name="append">Append to file</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(string filename, string text, bool append = false)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, append))
                {
                    writer.Write(text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="text">Text to saving</param>
        /// <param name="append">Append to file</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(string filename, string[] text, bool append = false)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, append))
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] != null) 
                        {
                            if (i > 0) writer.Write("\n");
                            writer.Write(text[i]);
                        }
                        
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="text">Text to saving</param>
        /// <param name="enc">Encoding</param>
        /// <param name="append">Append to file</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(string filename, string text, Encoding enc, bool append = false)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, append, enc))
                {
                    writer.Write(text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="text">Text to saving</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(Stream stream, string text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save text file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="text">Text to saving</param>
        /// <param name="enc">Encoding</param>
        /// <param name="append">Append to file</param>
        /// <returns>Return True if save Ok</returns>
        public static bool Save(Stream stream, string text, Encoding enc)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(stream, enc))
                {
                    writer.Write(text);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }

}
