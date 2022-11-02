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
        int[] bauds = new int[] { 115200, 9600, 19200, 57600 };
        int portIndex = -1;
        int baudIndex = -1;


        /// <summary>
        /// Get next settings
        /// </summary>
        /// <param name="setting">Settings</param>
        /// <returns>New settings</returns>
        public ConnectionSetting Next(ConnectionSetting setting)
        {
            var connSettings = setting.Copy();

            // ----- Initial values -----
            if (portIndex == -1)
            {
                portIndex = 0;
                baudIndex = 0;
            }
            else
            {
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
