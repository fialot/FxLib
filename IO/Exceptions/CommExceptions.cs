using Fx.Conversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO.Exceptions
{
    public static class CommExceptions
    {
        readonly static Dictionary<int, string> CommExceptionMessagesEN = new Dictionary<int, string>
        {
            { 0xE001, "General communication exception."},
            { 0xE002, "Connection attempt failed."},
            { 0xE003, "A response was not received within the specified time."}
        };

        readonly static Dictionary<int, string> CommExceptionMessagesCZ = new Dictionary<int, string>
        {
            { 0xE001, "Obecná chyba komunikace."},
            { 0xE002, "Selhal pokus o připojení."},
            { 0xE003, "Odpověď nedorazila ve stanoveném čase."}
        };

        public static string GetMessage(int Code)
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            try
            {
                if (ci.Name == "cs-CZ")
                    return CommExceptionMessagesCZ[Code];
                else
                    return CommExceptionMessagesEN[Code];
            }
            catch
            {
                return Code.ToString("X4");
            }
        }
    }




    public interface ICommException
    {
        int Code { get; }
        string SendedString { get; }
        string ReceivedString { get; }
        byte[] SendedData { get; }
        byte[] ReceivedData { get; }
    }

    /// <summary>
    /// Communication Exception (with sended packets)
    /// </summary>
    public class CommException : Exception, ICommException
    {
        public int Code { get; } = 0xE001;
        public string SendedString { get; } = "";
        public string ReceivedString { get; } = "";
        public byte[] SendedData { get; } = new byte[0];
        public byte[] ReceivedData { get; } = new byte[0];

        public CommException() : base(CommExceptions.GetMessage(0xE001)) { }
        public CommException(Exception innerException) : base(CommExceptions.GetMessage(0xE001), innerException) { }

        public CommException(string message) : base(message) { }
        public CommException(string message, Exception innerException) : base(message, innerException) { }
        public CommException(string message, string sended, string received) : base(message)
        {
            this.SendedString = sended;
            this.ReceivedString = received;
        }
        public CommException(string message, byte[] sended, byte[] received) : base(message)
        {
            this.SendedData = sended;
            this.ReceivedData = received;
            this.SendedString = "0x" + BitConverter.ToString(sended).Replace("-", "");
            this.ReceivedString = "0x" + BitConverter.ToString(received).Replace("-", "");
        }
    }

    public class ConnectionFailedException : Exception, ICommException
    {
        public int Code { get; } = 0xE002;
        public string SendedString { get; } = "";
        public string ReceivedString { get; } = "";
        public byte[] SendedData { get; } = new byte[0];
        public byte[] ReceivedData { get; } = new byte[0];


        public ConnectionFailedException() : base() { }
        //public NoResponseCommException(string message) : base(message) { }
        //public NoResponseCommException(string message, Exception innerException) : base(message, innerException) { }
        public ConnectionFailedException(Exception innerException) : base(CommExceptions.GetMessage(0xE002), innerException) { }

    }

    /// <summary>
    /// Communication Exception (with sended packets)
    /// </summary>
    public class TimeOutException : Exception, ICommException
    {
        public int Code { get; } = 0xE003;
        public string SendedString { get; } = "";
        public string ReceivedString { get; } = "";
        public byte[] SendedData { get; } = new byte[0];
        public byte[] ReceivedData { get; } = new byte[0];

        public TimeOutException() : base(CommExceptions.GetMessage(0xE003)) { }
        public TimeOutException(Exception innerException) : base(CommExceptions.GetMessage(0xE003), innerException) { }

        public TimeOutException(string sended, string received) : base(CommExceptions.GetMessage(0xE003))
        {
            this.SendedString = sended;
            this.ReceivedString = received;
        }
        public TimeOutException(byte[] sended, byte[] received) : base(CommExceptions.GetMessage(0xE003))
        {
            this.SendedData = sended;
            this.ReceivedData = received;
            this.SendedString = "0x" + BitConverter.ToString(sended).Replace("-", "");
            this.ReceivedString = "0x" + BitConverter.ToString(received).Replace("-", "");
        }
    }
}
