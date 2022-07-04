using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO.FileTypes
{
    public struct CsvData
    {
        public string[] Head;
        public List<string[]> Data;
    }

    public static class CsvFile
    {
        public static CsvData Read(string fileName, string separator = ";")
        {
            string text = Files.Read(fileName);
            return Parse(text, separator);
        }

        /// <summary>
        /// Parsing CSV files to CSV structure
        /// </summary>
        /// <param name="text">Text to parsing</param>
        /// <returns>CSVfile structure</returns>
        public static CsvData Parse(string text, string separator = ";")
        {
            CsvData table;
            table.Head = new string[0];
            table.Data = new List<string[]>();


            string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                    table.Head = lines[i].Split(new string[] { separator }, StringSplitOptions.None);
                else
                {
                    string[] values = lines[i].Split(new string[] { separator }, StringSplitOptions.None);
                    table.Data.Add(values);
                }
            }
            return table;
        }

        /// <summary>
        /// Load CSV string to string table
        /// </summary>
        /// <param name="text">CSV string</param>
        /// <returns></returns>
        public static string[,] ParseTable(string text, string separator = ";")
        {
            string[] lines = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] line = lines[0].Split(new string[] { separator }, StringSplitOptions.None);
            string[,] data = new string[line.Length, lines.Length];

            int n = line.Length;

            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Split(new string[] { separator }, StringSplitOptions.None);
                for (int j = 0; j < n; j++)
                {
                    if (j < line.Length) data[j, i] = line[j];
                }
            }
            return data;
        }
    }
}
