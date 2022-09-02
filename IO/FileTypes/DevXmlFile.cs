using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.IO.Ports;
using Fx.IO;
using Fx.Conversion;
using Fx.Security;

namespace Fx.IO.FileTypes
{


    public static class ConnectionXmlFile
    {

        public static List<ConnectionSetting> Load(string path)
        {
            List<ConnectionSetting> connList = new List<ConnectionSetting>();
            connList.Clear();

            // ----- Load Device list from XML -----
            connList = ImportDevices(path);

            return connList;
        }

        public static List<ConnectionSetting> Load(string path, List<ConnectionSetting> defaultList)
        {
            List<ConnectionSetting> connList = new List<ConnectionSetting>();
            connList.Clear();

            // ----- Load Device list from XML -----
            connList = ImportDevices(path);

            // ----- If no list -> create device -----
            if (connList == null)
            {
                connList = new List<ConnectionSetting>();
                foreach (var item in defaultList)
                    connList.Add(item.Copy());

                ExportDevices(connList, path);
            }
            return connList;
        }

        public static bool Save(List<ConnectionSetting> list, string path)
        {
            return ExportDevices(list, path);
        }

        private static List<ConnectionSetting> ImportDevices(string path)
        {
            List<ConnectionSetting> list = new List<ConnectionSetting>();

            bool needSave = false;

            try
            {

                string XMLtext = Files.Read(path);
                // ----- Parse XML to Structure -----
                var xml = XDocument.Parse(XMLtext);
                IEnumerable<XElement> elements = xml.Elements().Elements();

                foreach (var item in elements)
                {
                    ConnectionSetting newData = new ConnectionSetting();
                    needSave = newData.Load(item);
                    list.Add(newData);
                }

                if (needSave)
                    ExportDevices(list, path);

                return list;

            }
            catch (Exception)
            {
                return list;
            }
        }

        private static bool ExportDevices(List<ConnectionSetting> list, string path)
        {

            try
            {
                // ----- Parse XML to Structure -----
                var xml = new XDocument(new XDeclaration("1.0", "utf-8", null));
                var mainElement = new XElement("devices");
                xml.Add(mainElement);


                foreach (var item in list)
                {
                    mainElement.Add(item.GetXmlElement());
                }

                // ----- Create directory if not exist -----
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                xml.Save(path);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
