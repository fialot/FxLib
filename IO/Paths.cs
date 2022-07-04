using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO
{
    public static class Paths
    {

        /// <summary>
        /// Get full path  with default AppPath folder
        /// (replace variables)
        /// </summary>
        /// <param name="path">Path string</param>
        /// <returns>Path</returns>
        public static string GetFullPath(string path)
        {
            return GetFullPath(path, "%AppPath%");
        }

        /// <summary>
        /// Get full path (replace variables)
        /// </summary>
        /// <param name="path">Path string</param>
        /// <param name="workPath">Working dir</param>
        /// <returns>Path</returns>
        public static string GetFullPath(string path, string workPath)
        {
            // ----- Replace home dir -----
            string firstChar = "." + Path.DirectorySeparatorChar;

            if (path.IndexOf(firstChar) == 0)
            {
                path = workPath + Path.DirectorySeparatorChar + path.Remove(0, firstChar.Length);
            }


            path = path.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            path = path.Replace("%AppPath%", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            return path;
        }

        /// <summary>
        /// Get short path with default AppPath folder
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns></returns>
        public static string GetShortPath(string path)
        {
            return GetShortPath(path, GetAppPath());
        }

        /// <summary>
        /// Get short path
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="workPath">Working path</param>
        /// <returns>Path</returns>
        public static string GetShortPath(string path, string workPath)
        {
            // ----- Check if work folder -----

            int index = path.IndexOf(workPath);
            if (index == 0)
            {
                path = path.Remove(0, workPath.Length);
                path = "." + Path.DirectorySeparatorChar + path;
            }

            // ----- Check if Data folder -----
            string dataPath = GetAppDataPath();
            index = path.IndexOf(dataPath);
            if (index == 0)
            {
                path = path.Remove(0, dataPath.Length);
                path = "%AppData%" + Path.DirectorySeparatorChar + path;
            }

            return path;
        }

        /// <summary>
        /// Get application dir
        /// </summary>
        /// <returns></returns>
        public static string GetAppPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Get data dir
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar;
        }

    }
}
