using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using System.Xml.Linq;
using Fx.Conversion;

namespace Fx.IO.FileTypes
{
    /// <summary>
    /// Namespace for XML Parser
    /// </summary>
    static class XmlNamespaceDoc { }

    /// <summary>
    /// Create a New INI file to store or load data
    /// Version:    1.0.1
    /// Date:       2017-03-30
    /// </summary>
    
    public class XmlParser
    {
        

        #region Variables

        private string path;
        private string Text;

        #endregion

        #region Private functions

        /// <summary>
        /// IniParser Constructor
        /// </summary>
        /// <param name="filename">INI Filename</param>
        public XmlParser(string filename = "")
        {
            path = filename;
            if (filename != "") LoadFile(filename);
        }

        /// <summary>
        /// Parse INI string to section list
        /// </summary>
        /// <param name="text">INI string</param>
        private void Parse(string text)
        {
            this.Text = text;
        }

        

        #endregion

        #region Public functions

        #region Load & Save
        /// <summary>
        /// Load INI settings from file. 
        /// </summary>
        /// <param name="filename">INI File name to load. If the filename is empty, then will be load default file specified in the constructor parameter</param>
        public void LoadFile(string filename = "", Encoding enc = null)
        {
            if (filename == "") filename = path;
            if (filename != "")
            {
                path = filename;
                if (File.Exists(filename))
                {
                    try
                    {
                        // ----- LOAD INI FILE -----
                        StreamReader objReader;
                        if (enc == null)
                            objReader = new StreamReader(filename, true);
                        else
                            objReader = new StreamReader(filename, enc);
                        string text = objReader.ReadToEnd();
                        objReader.Close();
                        objReader.Dispose();

                        // ----- PARSE INI FILE -----
                        Parse(text);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }


        /// <summary>
        /// Save INI settings to file
        /// </summary>
        /// <param name="filename">INI filename</param>
        /// <param name="endl">Line separator. If the line separator is empty, then will be used default Environment.NewLine separator</param>
        public void SaveFile(string filename = "")
        {
            if (filename == "") filename = path;
            if (filename != "")
            {
                try
                {
                    string txt = this.Text;

                    // ----- SAVE TO FILE -----
                    StreamWriter objWriter = new StreamWriter(filename); // append
                    objWriter.Write(txt);
                    objWriter.Close();
                    objWriter.Dispose();
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Load INI settings from string
        /// </summary>
        /// <param name="text">INI string</param>
        public void Load(string text)
        {
            Parse(text);
        }

        /// <summary>
        /// Load INI settings from string
        /// </summary>
        /// <param name="text">INI Stream</param>
        /// <param name="enc">Encoding</param>
        public void Load(Stream text, Encoding enc = null)
        {
            StreamReader textStream;
            text.Position = 0;
            if (enc == null) 
                textStream  = new StreamReader(text, true);
            else
                textStream = new StreamReader(text, enc);
            string txt = textStream.ReadToEnd();
            Parse(txt);
        }

        /// <summary>
        /// Create INI settings string
        /// </summary>
        /// <returns>INI string</returns>
        public string Save()
        {
            return Text;
        }

        /// <summary>
        /// Create INI settings string
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="enc">Encoding</param>
        /// <param name="endl">Line separator. If the line separator is empty, then will be used default Environment.NewLine separator</param>
        /// <returns>INI string</returns>
        public string Save(Stream stream, Encoding enc = null)
        {
            string text =  Text;

            stream.Position = 0;
            stream.SetLength(0);

            StreamWriter sw;
            if (enc == null)
                sw = new StreamWriter(stream);
            else
                sw = new StreamWriter(stream, enc);
            sw.Write(text);
            sw.Flush();
            stream.Position = 0;

            return text;
        }

        #endregion

        #region Change registers

        /// <summary>
        /// Set item boolean value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, bool Value, string Comment = "")
        {
            string val;
            if (Value) val = "1";
            else val = "0";
            Write(Section, Key, val, Comment);
        }

        /// <summary>
        /// Set item byte value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, byte Value, string Comment = "")
        {
            Write(Section, Key, Value.ToString(), Comment);
        }

        /// <summary>
        /// Set item short value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, short Value, string Comment = "")
        {
            Write(Section, Key, Value.ToString(), Comment);
        }

        /// <summary>
        /// Set item unsigned short value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, ushort Value, string Comment = "")
        {
            Write(Section, Key, Value.ToString(), Comment);
        }

        /// <summary>
        /// Set item integer value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, int Value, string Comment = "")
        {
            Write(Section, Key, Value.ToString(), Comment);
        }

        /// <summary>
        /// Set item unsigned integer value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, uint Value, string Comment = "")
        {
            Write(Section, Key, Value.ToString(), Comment);
        }

        /// <summary>
        /// Set item object value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, object Value, string Comment = "")
        {
            string val;
            if (Value is bool)
            {
                if ((bool)Value)
                    val = "1";
                else
                    val = "0";
            }
            else val = Value.ToString();


            Write(Section, Key, val, Comment);
        }

        /// <summary>
        /// Set item string value
        /// </summary>
        /// <param name="Section">Section name</param>
        /// <param name="Key">Key name</param>
        /// <param name="Value">Value</param>
        /// <param name="Comment">Comment</param>
        public void Write(string Section, string Key, string Value, string Comment = "")
        {
            string[] sections = Section.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string[] sectionName;

            try
            {
                // ----- Parse XML to Structure -----
                var xml = XDocument.Parse(Text);
                IEnumerable<XElement> element = xml.Elements();

                for (int i = 0; i < sections.Length; i++)
                {
                    sectionName = sections[i].Split(new string[] { "#" }, StringSplitOptions.None);
                    element = element.Elements(sectionName[0]);
                }

                sectionName = sections[sections.Length - 1].Split(new string[] { "#" }, StringSplitOptions.None);
                int num = 1;
                if (sectionName.Length > 1)
                {
                    num = Conv.ToInt(sectionName[1], 1);
                }
                int index = 1;
                foreach (var item in element)
                {

                    if (item.Name == sectionName[0])
                    {
                        if (num == index)
                        {
                            if (Key != "")
                            {
                                item.SetAttributeValue(Key, Value);
                                //item.Attribute(Key).Value = Value;
                            }
                            else
                            {
                                item.Value = Value;
                            }
                            // ----- Save to string -----
                            var wr = new Utf8StringWriter();
                            xml.Save(wr);
                            Text = wr.ToString();
                            return;
                        }
                        else
                        {
                            index++;
                        }
                    }
                }

            }
            catch (Exception)
            {
                return;
            }
        }


        /// <summary>
        /// Get item settings
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefValue">Default value</param>
        /// <returns>Key value</returns>
        public string Read(string Section, string Key, string DefValue)
        {
            string[] sections = Section.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string[] sectionName;
            string value;

            try
            {
                // ----- Parse XML to Structure -----
                var xml = XDocument.Parse(Text);
                IEnumerable<XElement> element = xml.Elements();

                for (int i = 0; i < sections.Length; i++)
                {
                    sectionName = sections[i].Split(new string[] { "#" }, StringSplitOptions.None);
                    element = element.Elements(sectionName[0]);
                }


                sectionName = sections[sections.Length - 1].Split(new string[] { "#" }, StringSplitOptions.None);
                int num = 1;
                if (sectionName.Length > 1)
                {
                    num = Conv.ToInt(sectionName[1], 1);
                }
                int index = 1;
                foreach (var item in element)
                {
                    if (item.Name == sectionName[0])
                    {
                        if (num == index)
                        {
                            if (Key != "")
                            {
                                value = item.Attribute(Key).Value;
                            }
                            else
                            {
                                value = item.Value;
                            }
                            if (value != null) return value;
                            else return DefValue;
                        }
                        else
                        {
                            index++;
                        }
                        
                    }
                }
                return DefValue;

            } catch (Exception)
            {
                return DefValue;
            }
            
        }

        /// <summary>
        /// Get item settings
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefValue">Default value</param>
        /// <returns>Key value</returns>
        public int ReadInt(string Section, string Key, int DefValue)
        {
            try
            {
                string value = Read(Section, Key, DefValue.ToString());
                return Convert.ToInt32(value);
            }
            catch { return DefValue; }
        }

        /// <summary>
        /// Get item settings
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefValue">Default value</param>
        /// <returns>Key value</returns>
        public uint ReadUInt(string Section, string Key, uint DefValue)
        {
            try
            {
                string value = Read(Section, Key, DefValue.ToString());
                return Convert.ToUInt32(value);
            }
            catch { return DefValue; }
        }

        /// <summary>
        /// Get item settings
        /// </summary>
        /// <param name="Section">Section</param>
        /// <param name="Key">Key</param>
        /// <param name="DefValue">Default value</param>
        /// <returns>Key value</returns>
        public bool ReadBool(string Section, string Key, bool DefValue)
        {
            try
            {
                string value = Read(Section, Key, DefValue.ToString());

                if (value.ToLower() == "true" || value.ToLower() == "1")
                    return true;
                else if (value.ToLower() == "false" || value.ToLower() == "0")
                    return false;
                else return DefValue;
            }
            catch { return DefValue; }
        }
        #endregion


        #endregion

    }
}