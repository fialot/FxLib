using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO
{
    public class SerialAutoConnect
    {
        public int Count { get { return ports.Length * bauds.Length; } }

        string[] ports = SerialPort.GetPortNames().Reverse().ToArray();
        int[] bauds = new int[] { 115200, 9600, 19200, 57600, 38400 };
        int portIndex = 0;
        int baudIndex = 0;

        public ConnectionSetting Get(ConnectionSetting setting)
        {
            var connSettings = setting.Copy();
            

            // ----- Set new connection data -----
            connSettings.SerialPort = ports[portIndex];
            connSettings.BaudRate = bauds[baudIndex];

            return connSettings;
        }

        /// <summary>
        /// Get next settings
        /// </summary>
        /// <param name="setting">Settings</param>
        /// <returns>New settings</returns>
        public ConnectionSetting Next(ConnectionSetting setting)
        {
            var connSettings = setting.Copy();

            // ----- If baud end -> new port -----
            baudIndex++;
            if (baudIndex == bauds.Length)
            {
                baudIndex = 0;
                portIndex++;

                // ----- If End ports -> to First -----
                if (portIndex == ports.Length)
                {
                    portIndex = 0;
                }
            }

            // ----- Set new connection data -----
            connSettings.SerialPort = ports[portIndex];
            connSettings.BaudRate = bauds[baudIndex];

            return connSettings;
        }


        /// <summary>
        /// Get next settings
        /// </summary>
        /// <param name="setting">Settings</param>
        /// <returns>New settings</returns>
        public ConnectionSetting NextPort(ConnectionSetting setting)
        {
            var connSettings = setting.Copy();

            baudIndex = 0;
            portIndex++;

            // ----- If End ports -> to First -----
            if (portIndex == ports.Length)
            {
                portIndex = 0;
            }

            // ----- Set new connection data -----
            connSettings.SerialPort = ports[portIndex];
            connSettings.BaudRate = bauds[baudIndex];

            return connSettings;
        }


        public void Reset()
        {
            portIndex = -1;
            baudIndex = -1;
        }
    }
}
