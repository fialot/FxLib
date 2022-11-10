using Fx.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    /// <summary>
    /// Logger class
    /// </summary>
    public static class Log
    {
        #region Log Files

        /// <summary>
        /// Folder for logging
        /// </summary>
        public static string FileFolder { get; set; } = "";

        /// <summary>
        /// File Log level
        /// </summary>
        public static LogLevel FileLevel { get; set; } = LogLevel.Error | LogLevel.Warning | LogLevel.Info;

        /// <summary>
        /// File Suffix
        /// </summary>
        public static string FileSuffix { get; set; } = "";

        #endregion

        #region Log Errors

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="Message">Log Message</param>
        public static void E(string Message)
        {
            E(Message, "");
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="SubSuffix">File suffix</param>
        public static void E(string Message, string SubSuffix)
        {
            string msg = CreateMessage('E', Message);

            // ----- If Error level -----
            if ((FileLevel & LogLevel.Error) == LogLevel.Error)
            {
                SaveToFile(msg, SubSuffix);
            }
        }

        #endregion

        #region Log Warnings

        /// <summary>
        /// Log Warning
        /// </summary>
        /// <param name="Message">Log message</param>
        public static void W(string Message)
        {
            W(Message, "");
        }

        /// <summary>
        /// Log Warning
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="SubSuffix">File suffix</param>
        public static void W(string Message, string SubSuffix)
        {
            string msg = CreateMessage('W', Message);

            // ----- If Warning level -----
            if ((FileLevel & LogLevel.Warning) == LogLevel.Warning)
            {
                SaveToFile(msg, SubSuffix);
            }
        }

        #endregion

        #region Log Info

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="Message">Log message</param>
        public static void I(string Message)
        {
            I(Message, "");
        }

        /// <summary>
        /// Log Info
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="SubSuffix">File suffix</param>
        public static void I(string Message, string SubSuffix)
        {
            string msg = CreateMessage('I', Message);

            // ----- If Info level -----
            if ((FileLevel & LogLevel.Info) == LogLevel.Info)
            {
                SaveToFile(msg, SubSuffix);
            }
        }

        #endregion

        #region Log Debug

        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="Message"></param>
        public static void D(string Message)
        {
            D(Message, "");
        }


        /// <summary>
        /// Log Debug
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="SubSuffix">File suffix</param>
        public static void D(string Message, string SubSuffix)
        {
            string msg = CreateMessage('D', Message);

            // ----- If Debug level -----
            if ((FileLevel & LogLevel.Debug) == LogLevel.Debug)
            {
                SaveToFile(msg, SubSuffix);
            }
        }

        #endregion

        #region Log eXtended

        /// <summary>
        /// Log eXtended
        /// </summary>
        /// <param name="Message">Log message</param>
        public static void X(string Message)
        {
            X(Message, "");
        }

        /// <summary>
        /// Log eXtended
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="SubSuffix">File suffix</param>
        public static void X(string Message, string SubSuffix)
        {
            string msg = CreateMessage('X', Message);

            // ----- If eXtended level -----
            if ((FileLevel & LogLevel.eXtended) == LogLevel.eXtended)
            {
                SaveToFile(msg, SubSuffix);
            }
        }

        #endregion

        #region Process

        /// <summary>
        /// Create Message string
        /// </summary>
        /// <param name="LogLevel">Log level</param>
        /// <param name="Message">Message text</param>
        /// <returns>Message string</returns>
        private static string CreateMessage(char LogLevel, string Message)
        {
            return Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + LogLevel + "  " + Message;
        }

        /// <summary>
        /// Save log to file
        /// </summary>
        /// <param name="Message">Message string</param>
        /// <param name="SubSuffix">File sun suffix</param>
        private static void SaveToFile(string Message, string SubSuffix)
        {
            // ----- Get file name -----
            string fileName = FileFolder;
            if (FileFolder != "") fileName += System.IO.Path.DirectorySeparatorChar;
            fileName += DateTime.Now.ToString("yyyy-MM-dd") + FileSuffix + SubSuffix + ".log";
            

            // ----- Create header -----
            if (!System.IO.File.Exists(fileName)) {
                Files.Save(fileName, "-----------------------------------------------------" + Environment.NewLine +
                    "     LOG file" + Environment.NewLine +
                    "-----------------------------------------------------"
                );
            }

            // ----- Save message -----
            Files.Save(fileName, Message,true);
        }

        #endregion
    }

    [Flags]
    public enum LogLevel
    {
        Error       = 0b0000_0001,
        Warning     = 0b0000_0010,
        Info        = 0b0000_0100,
        Debug       = 0b0000_1000,
        eXtended    = 0b0001_0000,
    }
}
