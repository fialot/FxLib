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
            { 0xE003, "A response was not received within the specified time."},
            { 0xE101, "Wrong length."},
            { 0xE111, "No permission to make change."},
            { 0xE112, "Not supported command."}

        };

        readonly static Dictionary<int, string> CommExceptionMessagesCZ = new Dictionary<int, string>
        {
            { 0xE001, "Obecná chyba komunikace."},
            { 0xE002, "Selhal pokus o připojení."},
            { 0xE003, "Odpověď nedorazila ve stanoveném čase."},
            { 0xE101, "Špatná délka."},
            { 0xE111, "Nedostatečné oprávnění na provedení změny."},
            { 0xE112, "Nepodporovaný příkaz."}

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
        public int Code { get; protected set; } = 0xE001;
        public string SendedString { get; protected set; } = "";
        public string ReceivedString { get; protected set; } = "";
        public byte[] SendedData { get; protected set; } = new byte[0];
        public byte[] ReceivedData { get; protected set; } = new byte[0];

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

    public class ConnectionFailedException : CommException, ICommException
    {
        public ConnectionFailedException() : base(CommExceptions.GetMessage(0xE002)) { Code = 0xE002; }
        public ConnectionFailedException(Exception innerException) : base(CommExceptions.GetMessage(0xE002), innerException) { Code = 0xE002; }
    }

    /// <summary>
    /// Communication Exception (with sended packets)
    /// </summary>
    public class TimeOutException : CommException, ICommException
    {
        public TimeOutException() : base(CommExceptions.GetMessage(0xE003)) { Code = 0xE003; }
        public TimeOutException(Exception innerException) : base(CommExceptions.GetMessage(0xE003), innerException) { Code = 0xE003; }
        public TimeOutException(string sended, string received) : base(CommExceptions.GetMessage(0xE003), sended, received) { Code = 0xE003; }
        public TimeOutException(byte[] sended, byte[] received) : base(CommExceptions.GetMessage(0xE003), sended, received) { Code = 0xE003; }
    }

    public class BadLengthException : CommException, ICommException
    {
        public BadLengthException() : base(CommExceptions.GetMessage(0xE101)) { Code = 0xE101; }
        public BadLengthException(Exception innerException) : base(CommExceptions.GetMessage(0xE101), innerException) { Code = 0xE101; }
        public BadLengthException(string sended, string received) : base(CommExceptions.GetMessage(0xE101), sended, received) { Code = 0xE101; }
        public BadLengthException(byte[] sended, byte[] received) : base(CommExceptions.GetMessage(0xE101), sended, received) { Code = 0xE101; }
    }

    public class NoPermissionException : CommException, ICommException
    {
        public NoPermissionException() : base(CommExceptions.GetMessage(0xE111)) { Code = 0xE111; }
        public NoPermissionException(Exception innerException) : base(CommExceptions.GetMessage(0xE111), innerException) { Code = 0xE111; }
        public NoPermissionException(string sended, string received) : base(CommExceptions.GetMessage(0xE111), sended, received) { Code = 0xE111; }
        public NoPermissionException(byte[] sended, byte[] received) : base(CommExceptions.GetMessage(0xE111), sended, received) { Code = 0xE111; }
    }

    public class NotSupportedCommandException : CommException, ICommException
    {
        public NotSupportedCommandException() : base(CommExceptions.GetMessage(0xE112)) { Code = 0xE112; }
        public NotSupportedCommandException(Exception innerException) : base(CommExceptions.GetMessage(0xE112), innerException) { Code = 0xE112; }
        public NotSupportedCommandException(string sended, string received) : base(CommExceptions.GetMessage(0xE112), sended, received) { Code = 0xE112; }
        public NotSupportedCommandException(byte[] sended, byte[] received) : base(CommExceptions.GetMessage(0xE112), sended, received) { Code = 0xE112; }
    }
}
