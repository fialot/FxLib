using Fx.Conversion;
using Fx.IO;
using Fx.IO.Exceptions;
using Fx.Radiometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fx.Devices
{
    public partial class DeviceNuvia : Device, IDeviceEGM, IDeviceMCA
    {
        string XMLdesc = "";
        bool encoding1250 = false;
        int protocolVersion = 1;
        ushort dataPacketStart = 296;
        ushort dataPacketLength = 35;


        #region XML EGM

        const string mbEGM_XMLdesc_EN = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<device name = ""EGM - Geiger-Muller probe"" >
 <system>
  <language id=""2"" name=""English""/>
  <hwdesc>
   <userdata>512</userdata>
   <maxroi>4</maxroi>
   <pwr usb=""Yes"" extmin=""9"" extmax=""30""/>
  </hwdesc>
 </system>
 <setups>
  <setup id=""main"" desc=""Setup"">
   <group name=""Measurement"" >
    <enum id=""56"" name=""Start mode"">
     <item name = ""Single"" value=""0""/>
     <item name = ""Autostart"" value=""1""/>
    </enum>
    <edit id = ""54"" name=""Preset time [ms]"" format=""%u""/>
   </group>
   <group name = ""Generic"" >
    <edit id=""41"" name=""Date and time (Unix)"" format=""%u""/>
   </group>
   <group name = ""Data save"" >
    <enum id=""30"" name=""Write to file"">
     <item name = ""Off"" value=""0""/>
     <item name = ""On"" value=""1""/>
    </enum>
   </group>
   <group name = ""Connection"" >
    <enum id=""1"" name=""Select interface"">
     <item name=""RS232"" value=""2""/>
     <item name=""RS485"" value=""4""/>
    </enum>
    <enum id=""4"" name=""Comm Speed [Bd]"">
     <item name=""9600"" value=""0""/>
     <item name=""19200"" value=""1""/>
     <item name=""38400"" value=""2""/>
     <item name=""57600"" value=""5""/>
     <item name=""115200"" value=""3""/>
    </enum>
    <enum id=""18"" name=""Parity"">
     <item name=""None"" value=""0""/>
     <item name=""Odd"" value=""1""/>
     <item name=""Even"" value=""2""/>
    </enum>
    <enum id=""2"" name=""Protocol"">
     <item name=""Nuvia"" value=""0""/>
     <item name=""Modbus"" value=""1""/>
    </enum>
    <edit id=""3"" name=""Device address""/>
   </group>   
   <group name=""Ethernet"">
    <edit id=""5"" name=""IP1""/>
    <edit id=""6"" name=""IP2""/>
    <edit id=""7"" name=""IP3""/>
    <edit id=""8"" name=""IP4""/>
    <edit id=""9"" name=""Network mask 1""/>
    <edit id=""10"" name=""Network mask 2""/>
    <edit id=""11"" name=""Network mask 3""/>
    <edit id=""12"" name=""Network mask 4""/>
    <edit id=""13"" name=""Default gateway 1""/>
    <edit id=""14"" name=""Default gateway 2""/>
    <edit id=""15"" name=""Default gateway 3""/>
    <edit id=""16"" name=""Default gateway 4""/>
    <edit id=""17"" name=""Port"" min=""0"" max=""65535""/>
   </group>
  </setup>
 </setups>
 <measurements>
  <measurement id=""standard"" desc=""Standard"">
   <group name=""Device"">
    <info id=""10296"" name=""Alarms"" format=""%u"" />
    <info id=""10298"" name=""Status"" format=""%u"" />
    <info id=""10032"" name=""High Voltage [V]"" format=""%f"" />
    <info id=""10035"" name=""Temperature [°C]"" format=""%f"" />
    <info id=""10040"" name=""Trusted tube"" />
    <info id=""10300"" name=""Tubes count"" />
   </group>
   <group name=""Measurement"">
    <info id=""10301"" name=""Dose Rate [uSV]"" format=""%f"" />
    <info id=""10303"" name=""Deviation [%]"" format=""%f"" />
    <info id=""10305"" name=""Tube 0 Counts per second [CPS]"" format=""%f"" />
    <info id=""10307"" name=""Tube 1 Counts per second [CPS]"" format=""%f"" />
    <info id=""10309"" name=""Tube 2 Counts per second [CPS]"" format=""%f"" />
    <info id=""10311"" name=""Tube 3 Counts per second [CPS]"" format=""%f"" />
    <info id=""10313"" name=""Measurement stamp"" format=""%u"" />
   </group>
   <group name=""Short-term Measurement"">
    <info id=""10317"" name=""Short-term Dose Rate [uSV]"" format=""%f"" />
    <info id=""10319"" name=""Short-term odchylka [%]"" format=""%f"" />
    <info id=""10321"" name=""Tube 0 Short-term Counts per second [CPS]"" format=""%f"" />
    <info id=""10323"" name=""Tube 1 Short-term Counts per second [CPS]"" format=""%f"" />
    <info id=""10325"" name=""Tube 2 Short-term Counts per second [CPS]"" format=""%f"" />
    <info id=""10327"" name=""Tube 3 Short-term Counts per second [CPS]"" format=""%f"" />
   </group>
  </measurement>
 </measurements>
</device>";

        const string mbEGM_XMLdesc_CZ = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<device name = ""EGM - Geiger-Muller probe"" >
 <system>
  <language id=""1"" name=""Czech""/>
  <hwdesc>
   <userdata>512</userdata>
   <maxroi>4</maxroi>
   <pwr usb=""Yes"" extmin=""9"" extmax=""30""/>
  </hwdesc>
 </system>
 <setups>
  <setup id=""main"" desc=""Nastavení"">
   <group name=""Měření"" >
    <enum id=""56"" name=""Spouštění"">
     <item name = ""Jednorázově"" value=""0""/>
     <item name = ""Autostart"" value=""1""/>
    </enum>
    <edit id = ""54"" name=""Doba čítání [ms]"" format=""%u""/>
   </group>
   <group name = ""Obecné"" >
    <edit id=""41"" name=""Datum a čas (Unix)"" format=""%u""/>
   </group>
   <group name = ""Ukládání dat"" >
    <enum id=""30"" name=""Zápis do souboru"">
     <item name = ""Vypnuto"" value=""0""/>
     <item name = ""Zapnuto"" value=""1""/>
    </enum>
   </group>
   <group name = ""Připojení"" >
    <enum id=""1"" name=""Vybrané rozhraní"">
     <item name=""RS232"" value=""2""/>
     <item name=""RS485"" value=""4""/>
    </enum>
    <enum id=""4"" name=""Přenosová rychlost [Bd]"">
     <item name=""9600"" value=""0""/>
     <item name=""19200"" value=""1""/>
     <item name=""38400"" value=""2""/>
     <item name=""57600"" value=""5""/>
     <item name=""115200"" value=""3""/>
    </enum>
    <enum id=""18"" name=""Parita"">
     <item name=""Není"" value=""0""/>
     <item name=""Lichá"" value=""1""/>
     <item name=""Sudá"" value=""2""/>
    </enum>
    <enum id=""2"" name=""Protokol"">
     <item name=""Nuvia"" value=""0""/>
     <item name=""Modbus"" value=""1""/>
    </enum>
    <edit id=""3"" name=""Adresa zařízení""/>
   </group>   
   <group name=""Ethernet"">
    <edit id=""5"" name=""IP1""/>
    <edit id=""6"" name=""IP2""/>
    <edit id=""7"" name=""IP3""/>
    <edit id=""8"" name=""IP4""/>
    <edit id=""9"" name=""Síťová maska 1""/>
    <edit id=""10"" name=""Síťová maska 2""/>
    <edit id=""11"" name=""Síťová maska 3""/>
    <edit id=""12"" name=""Síťová maska 4""/>
    <edit id=""13"" name=""Výchozí brána 1""/>
    <edit id=""14"" name=""Výchozí brána 2""/>
    <edit id=""15"" name=""Výchozí brána 3""/>
    <edit id=""16"" name=""Výchozí brána 4""/>
    <edit id=""17"" name=""Port"" min=""0"" max=""65535""/>
   </group>
  </setup>
 </setups>
 <measurements>
  <measurement id=""standard"" desc=""Standardní"">
   <group name=""Zařízení"">
    <info id=""10296"" name=""Alarmy"" format=""%u"" />
    <info id=""10298"" name=""Status"" format=""%u"" />
    <info id=""10032"" name=""Vysoké napětí [V]"" format=""%f"" />
    <info id=""10035"" name=""Teplota [°C]"" format=""%f"" />
    <info id=""10040"" name=""Důvěryhodná trubice"" />
    <info id=""10300"" name=""Počet trubic"" />
   </group>
   <group name=""Měření"">
    <info id=""10301"" name=""Dávkový příkon [uSV]"" format=""%f"" />
    <info id=""10303"" name=""Odchylka [%]"" format=""%f"" />
    <info id=""10305"" name=""Trubice 0 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10307"" name=""Trubice 1 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10309"" name=""Trubice 2 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10311"" name=""Trubice 3 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10313"" name=""Číslo měření"" format=""%u"" />
   </group>
   <group name=""Plovoucí průměr"">
    <info id=""10317"" name=""Krátkodobý dávkový příkon [uSV]"" format=""%f"" />
    <info id=""10319"" name=""Krátkodobá odchylka [%]"" format=""%f"" />
    <info id=""10321"" name=""Trubice 0 Krátkodobých impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10323"" name=""Trubice 1 Krátkodobých impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10325"" name=""Trubice 2 Krátkodobých impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10327"" name=""Trubice 3 Krátkodobých impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10329"" name=""Číslo měření"" format=""%u"" />
   </group>
  </measurement>
 </measurements>
</device>";

        #endregion
        
        #region XML MCA

        const string mbMCA_XMLdesc_EN = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<device name = ""SCA4 - Quad channel counter"" >
 <system>
  <language id=""2"" name=""English""/>
  <sn id=""10000"" name=""Serial number""></sn>
  <fw id=""10001"" name=""Firmware version""></fw>
  <setupver id=""10002"" name=""Setup version""></setupver>
  <hwdesc>
   <userdata>512</userdata>
   <chan mca=""pseudo"">128</chan>
   <maxroi>4</maxroi>
   <roisteps>1024</roisteps>
   <hv id=""10004"" name=""Maximal high voltage"">1250</hv>
   <pwr usb=""Yes"" extmin=""9"" extmax=""30""/>
   <com>
    <interface>USB</interface>
    <interface>RS232</interface>
    <interface>RS485</interface>
    <interface>Bluetooth</interface>
    <interface>Ethernet 10/100</interface>
    <ethmaxconn>4</ethmaxconn>
   </com>
  </hwdesc>
 </system>
 <setups>
  <setup id=""main"" desc=""Setup"">
   <group name=""ROI"" >
    <enum id=""33"" name=""Index"">
     <item name = ""ROI 1"" value=""0""/>
     <item name = ""ROI 2"" value=""1""/>
     <item name = ""ROI 3"" value=""2""/>
     <item name = ""ROI 4"" value=""3""/>
    </enum>
    <enum id=""40"" name=""Start mode"">
     <item name = ""Single"" value=""0""/>
     <item name = ""Autostart"" value=""1""/>
    </enum>
    <edit id = ""34"" name=""LLD""/>
    <edit id = ""35"" name=""ULD""/>
    <edit id = ""38"" name=""Preset time [ms]"" format=""%u""/>
   </group>
   <group name = ""Spectrum"" >
    <enum id=""45"" name= ""Conversion gain"">
     <item name = ""32"" value=""0""/>
     <item name = ""64"" value=""1""/>
     <item name = ""128"" value=""2""/>
    </enum>
    <edit id = ""46"" name=""LLD"" min=""0"" max=""128""/>
    <edit id = ""47"" name=""ULD"" min=""0"" max=""128""/>
    <edit id = ""48"" name=""Time to channel [ms]"" min=""0"" max=""4294967296"" format=""%u""/>
   </group>
   <group name = ""High voltage"" >
    <edit id=""32"" name=""Preset [V]"" min=""0"" max=""1250""/>
   </group>
   <group name = ""Generic"" >
    <edit id=""41"" name=""Date and time (Unix)"" format=""%u""/>
   </group>
   <group name = ""Data save"" >
    <enum id=""30"" name=""Write to file"">
     <item name = ""Off"" value=""0""/>
     <item name = ""On"" value=""1""/>
    </enum>
   </group>
   <group name = ""Connection"" >
    <enum id=""1"" name=""Select interface"">
     <item name=""RS232"" value=""2""/>
     <item name=""RS485"" value=""4""/>
    </enum>
    <enum id=""4"" name=""Comm Speed [Bd]"">
     <item name=""9600"" value=""0""/>
     <item name=""19200"" value=""1""/>
     <item name=""38400"" value=""2""/>
     <item name=""57600"" value=""5""/>
     <item name=""115200"" value=""3""/>
    </enum>
    <enum id=""18"" name=""Parity"">
     <item name=""None"" value=""0""/>
     <item name=""Odd"" value=""1""/>
     <item name=""Even"" value=""2""/>
    </enum>
    <enum id=""2"" name=""Protocol"">
     <item name=""Nuvia"" value=""0""/>
     <item name=""Modbus"" value=""1""/>
    </enum>
    <edit id=""3"" name=""Device address""/>
   </group>   
   <group name=""Ethernet"">
    <edit id=""5"" name=""IP1""/>
    <edit id=""6"" name=""IP2""/>
    <edit id=""7"" name=""IP3""/>
    <edit id=""8"" name=""IP4""/>
    <edit id=""9"" name=""Network mask 1""/>
    <edit id=""10"" name=""Network mask 2""/>
    <edit id=""11"" name=""Network mask 3""/>
    <edit id=""12"" name=""Network mask 4""/>
    <edit id=""13"" name=""Default gateway 1""/>
    <edit id=""14"" name=""Default gateway 2""/>
    <edit id=""15"" name=""Default gateway 3""/>
    <edit id=""16"" name=""Default gateway 4""/>
    <edit id=""17"" name=""Port"" min=""0"" max=""65535""/>
   </group>
  </setup>
 </setups>
 <measurements>
  <measurement id=""standard"" desc=""Standard"">
   <group name=""Device"">
													   
    <info id=""10039"" name=""Status"" format=""%u"" />
    <info id=""10032"" name=""High Voltage [V]"" format=""%f"" />
    <info id=""10035"" name=""Temperature [°C]"" format=""%f"" />
											   
											  
   </group>
   <group name=""Measurement"">
																
															  
    <info id=""10223"" name=""ROI1 Counts per second [CPS]"" format=""%f"" />
    <info id=""10225"" name=""ROI2 Counts per second [CPS]"" format=""%f"" />
    <info id=""10227"" name=""ROI3 Counts per second [CPS]"" format=""%f"" />
    <info id=""10229"" name=""ROI4 Counts per second [CPS]"" format=""%f"" />
    <info id=""10221"" name=""Measurement stamp"" format=""%u"" />
   </group>
   <group name=""Actual measurement"">
																		   
																		
    <info id=""10239"" name=""ROI1 Actual Counts per second [CPS]"" format=""%f"" />
    <info id=""10241"" name=""ROI2 Actual Counts per second [CPS]"" format=""%f"" />
    <info id=""10243"" name=""ROI3 Actual Counts per second [CPS]"" format=""%f"" />
    <info id=""10245"" name=""ROI4 Actual Counts per second [CPS]"" format=""%f"" />
   </group>
  </measurement>
 </measurements>
</device>";

        const string mbMCA_XMLdesc_CZ = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<device name = ""SCA4 - Quad channel counter"" >
 <system>
  <language id=""1"" name=""Czech""/>
  <sn id=""10000"" name=""Sériové číslo""></sn>
  <fw id=""10001"" name=""Firmware version""></fw>
  <setupver id=""10002"" name=""Setup version""></setupver>
  <hwdesc>
   <userdata>512</userdata>
   <chan mca=""pseudo"">128</chan>
   <maxroi>4</maxroi>
   <roisteps>1024</roisteps>
   <hv id=""10004"" name=""Maximal high voltage"">1250</hv>
   <pwr usb=""Yes"" extmin=""9"" extmax=""30""/>
   <com>
    <interface>USB</interface>
    <interface>RS232</interface>
    <interface>RS485</interface>
    <interface>Bluetooth</interface>
    <interface>Ethernet 10/100</interface>
    <ethmaxconn>4</ethmaxconn>
   </com>
  </hwdesc>
 </system>
 <setups>
  <setup id=""main"" desc=""Nastavení"">
   <group name=""ROI"" >
    <enum id=""33"" name=""Index"">
     <item name = ""ROI 1"" value=""0""/>
     <item name = ""ROI 2"" value=""1""/>
     <item name = ""ROI 3"" value=""2""/>
     <item name = ""ROI 4"" value=""3""/>
    </enum>
    <enum id=""40"" name=""Spouštění"">
     <item name = ""Jednorázově"" value=""0""/>
     <item name = ""Autostart"" value=""1""/>
    </enum>
    <edit id = ""34"" name=""LLD""/>
    <edit id = ""35"" name=""ULD""/>
    <edit id = ""38"" name=""Doba čítání [ms]"" format=""%u""/>
   </group>
   <group name = ""Spektrum"" >
    <enum id=""45"" name= ""Počet kanálů"">
     <item name = ""32"" value=""0""/>
     <item name = ""64"" value=""1""/>
     <item name = ""128"" value=""2""/>
    </enum>
    <edit id = ""46"" name=""LLD"" min=""0"" max=""128""/>
    <edit id = ""47"" name=""ULD"" min=""0"" max=""128""/>
    <edit id = ""48"" name=""Čas na kanál [ms]"" min=""0"" max=""4294967296"" format=""%u""/>
   </group>
   <group name = ""Vysoké napětí"" >
    <edit id=""32"" name=""Vysoké napětí [V]"" min=""0"" max=""1250""/>
   </group>
   <group name = ""Obecné"" >
    <edit id=""41"" name=""Datum a čas (Unix)"" format=""%u""/>
   </group>
   <group name = ""Ukládání dat"" >
    <enum id=""30"" name=""Zápis do souboru"">
     <item name = ""Vypnuto"" value=""0""/>
     <item name = ""Zapnuto"" value=""1""/>
    </enum>
   </group>
   <group name = ""Připojení"" >
    <enum id=""1"" name=""Vybrané rozhraní"">
     <item name=""RS232"" value=""2""/>
     <item name=""RS485"" value=""4""/>
    </enum>
    <enum id=""4"" name=""Přenosová rychlost [Bd]"">
     <item name=""9600"" value=""0""/>
     <item name=""19200"" value=""1""/>
     <item name=""38400"" value=""2""/>
     <item name=""57600"" value=""5""/>
     <item name=""115200"" value=""3""/>
    </enum>
    <enum id=""18"" name=""Parita"">
     <item name=""Není"" value=""0""/>
     <item name=""Lichá"" value=""1""/>
     <item name=""Sudá"" value=""2""/>
    </enum>
    <enum id=""2"" name=""Protokol"">
     <item name=""Nuvia"" value=""0""/>
     <item name=""Modbus"" value=""1""/>
    </enum>
    <edit id=""3"" name=""Adresa zařízení""/>
   </group>   
   <group name=""Ethernet"">
    <edit id=""5"" name=""IP1""/>
    <edit id=""6"" name=""IP2""/>
    <edit id=""7"" name=""IP3""/>
    <edit id=""8"" name=""IP4""/>
    <edit id=""9"" name=""Síťová maska 1""/>
    <edit id=""10"" name=""Síťová maska 2""/>
    <edit id=""11"" name=""Síťová maska 3""/>
    <edit id=""12"" name=""Síťová maska 4""/>
    <edit id=""13"" name=""Výchozí brána 1""/>
    <edit id=""14"" name=""Výchozí brána 2""/>
    <edit id=""15"" name=""Výchozí brána 3""/>
    <edit id=""16"" name=""Výchozí brána 4""/>
    <edit id=""17"" name=""Port"" min=""0"" max=""65535""/>
   </group>
  </setup>
 </setups>
 <measurements>
  <measurement id=""standard"" desc=""Standardní"">
   <group name=""Zařízení"">
													   
    <info id=""10039"" name=""Status"" format=""%u"" />
    <info id=""10032"" name=""Vysoké napětí [V]"" format=""%f"" />
    <info id=""10035"" name=""Teplota [°C]"" format=""%f"" />
														 
												
   </group>
   <group name=""Měření"">
																		 
															 
    <info id=""10223"" name=""ROI1 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10225"" name=""ROI2 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10227"" name=""ROI3 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10229"" name=""ROI4 Impulsů za sekundu [CPS]"" format=""%f"" />
    <info id=""10221"" name=""Číslo měření"" format=""%u"" />
   </group>
   <group name=""Probíhající měření"">
																					  
																		  
    <info id=""10239"" name=""ROI1 Aktuální impulsy za sekundu [CPS]"" format=""%f"" />
    <info id=""10241"" name=""ROI2 Aktuální impulsy za sekundu [CPS]"" format=""%f"" />
    <info id=""10243"" name=""ROI3 Aktuální impulsy za sekundu [CPS]"" format=""%f"" />
    <info id=""10245"" name=""ROI4 Aktuální impulsy za sekundu [CPS]"" format=""%f"" />
																  
   </group>
  </measurement>
 </measurements>
</device>";

        #endregion


        #region Get info

        DeviceInfo mbGetInfo()
        {
            info = new DeviceInfo();
            ushort[] regs = mb.ReadInputRegisters(1, 22);
            info.Version = mb.GetString8(regs, 0, 10);          // get FW version
            info.SN = mb.GetString8(regs, 10, 12);              // get SN

            if (info.Version.Contains("EGM") || info.Version.Contains("GMS"))
            {
                info.Model = "EGM";
                Type = DeviceType.EGM;
            }
            else if (info.Version.Contains("SCA") || info.Version.Contains("MCB"))
            {
                if (info.Version.Contains("SCA"))
                    info.Model = "SCA";                                 // get Model
                else
                    info.Model = "MCB";                                 // get Model
                Type = DeviceType.MCA;
                Support |= DevSupport.Spectrum;
            }
            
            info.Date = "";                                     // get date
            return info;
        }

        string mbGetXML(byte lng)
        {
            if (XMLdesc == null || XMLdesc == "")
            {
                mb.SetTimeOuts(5000, 1000);
                try
                {
                    byte[] rawData = mb.ReadFunction(70, new byte[] { 0, lng });
                    XMLdesc = ParseXML(rawData);
                    getProtocolVersion(XMLdesc);
                }
                catch (Exception Err)
                {
                    if (Err.Message.Contains("Illegal function"))
                    {
                        if (Type == DeviceType.EGM)
                        {
                            if (lng == 1)
                                XMLdesc = mbEGM_XMLdesc_CZ;
                            else
                                XMLdesc = mbEGM_XMLdesc_EN;
                        }
                        else
                        {
                            if (lng == 1)
                                XMLdesc = mbMCA_XMLdesc_CZ;
                            else
                                XMLdesc = mbMCA_XMLdesc_EN;
                        }
                    }
                    else
                        throw Err;
                }
                mb.SetTimeOuts();
            }
            return XMLdesc;
        }

        string ParseXML(byte[] data)
        {
            string res = Encoding.UTF8.GetString(data, 2, data.Length - 3);

            if (res.Contains("windows-1250"))
            {
                res = Encoding.GetEncoding(1250).GetString(data, 2, data.Length - 3);
                encoding1250 = true;
            }


            while (res[res.Length - 1] == 0) res = res.Remove(res.Length - 1);

            return res;
        }

        private void getProtocolVersion(string XMLdesc)
        {
            // ----- Parse XML -----
            var xml = XDocument.Parse(XMLdesc);
            IEnumerable<XElement> system = xml.Elements().Elements("system").Elements();

            foreach (var value in system)
            {
                try
                {
                    if (value.Name == "protocolver")
                    {
                        if (value.Attribute("id") != null)
                            protocolVersion = Conv.ToInt(value.Attribute("id").Value, 1);

                    }
                    else if (value.Name == "datapacket")
                    {
                        if (value.Attribute("start") != null)
                        {
                            dataPacketStart = Conv.ToUShort(value.Attribute("start").Value, 10290);
                        }

                        if (value.Attribute("length") != null)
                            dataPacketLength = Conv.ToUShort(value.Attribute("length").Value, 39);
                    }
                }
                catch { }
            }
        }

        List<DevMeasVals> mbGetEGMMeasurement()
        {
            List<DevMeasVals> valList = new List<DevMeasVals>();

            Dictionary<int, string> valDict = new Dictionary<int, string>();

            if (MeasList == null) MeasList = CreateMeasList(devGetXML(), mode);                     // Create Measurement list

            ushort[] egmStatus = new ushort[0];
            ushort[] egmData = new ushort[0];

            //ushort[] egmStatus = MB.ReadRegisters(32, 9, true);
            //ushort[] egmData = MB.ReadInputRegisters(296, 35);

            // ----- Get Measurement -----
            if (protocolVersion == 2)
            {
                ushort dataStart = dataPacketStart;
                if (dataStart > 10000) dataStart -= 10000;
                egmData = mb.ReadRegisters(dataStart, dataPacketLength, true);
            }
            else
            {
                egmStatus = mb.ReadRegisters(32, 9, true);
                egmData = mb.ReadInputRegisters(296, 35);
            }

            if (protocolVersion == 2)
            {
                foreach (var item in MeasList)
                {
                    if ((item.ID >= dataPacketStart) && (item.ID <= dataPacketStart + dataPacketLength))
                    {
                        int pos = item.ID - dataPacketStart;
                        if (item.Format == "%f")
                        {
                            valDict.Add(item.ID, mb.GetFloat(egmData, pos).ToString());
                        }
                        else if (item.Format == "%u")
                        {
                            valDict.Add(item.ID, mb.GetUInt(egmData, pos).ToString());
                        }
                        else
                        {
                            if (egmData[pos] != 65535)
                                valDict.Add(item.ID, egmData[pos].ToString());
                        }
                    }
                }
            }
            else
            {

                // ----- Parse Measurement -----
                valDict.Add(10032, egmStatus[0].ToString());                // HV
                                                                            //valDict.Add(10039, MB.GetInt(egmStatus, 7).ToString());     // Status
                if (egmStatus[3] != 65535)
                    valDict.Add(10035, egmStatus[3].ToString());            // Temperature
                valDict.Add(10040, egmStatus[0].ToString());                // Trusted ROI

                valDict.Add(10296, mb.GetInt(egmData, 0).ToString());       // Alarm
                valDict.Add(10298, mb.GetInt(egmData, 2).ToString());       // Status
                valDict.Add(10300, egmData[4].ToString());                  // Tubes count

                valDict.Add(10301, mb.GetFloat(egmData, 5).ToString());     // DR
                valDict.Add(10303, mb.GetFloat(egmData, 7).ToString());     // Deviation
                valDict.Add(10305, mb.GetFloat(egmData, 9).ToString());     // CPS 1
                valDict.Add(10307, mb.GetFloat(egmData, 11).ToString());     // CPS 2
                valDict.Add(10309, mb.GetFloat(egmData, 13).ToString());     // CPS 3
                valDict.Add(10311, mb.GetFloat(egmData, 15).ToString());    // CPS 4
                valDict.Add(10313, mb.GetInt(egmData, 17).ToString());      // TimaStamp

                valDict.Add(10317, mb.GetFloat(egmData, 21).ToString());    // Actual DR
                valDict.Add(10319, mb.GetFloat(egmData, 23).ToString());    // Actual Deviation
                valDict.Add(10321, mb.GetFloat(egmData, 25).ToString());    // Actual CPS 1
                valDict.Add(10323, mb.GetFloat(egmData, 27).ToString());    // Actual CPS 2
                valDict.Add(10325, mb.GetFloat(egmData, 29).ToString());    // Actual CPS 3
                valDict.Add(10327, mb.GetFloat(egmData, 31).ToString());    // Actual CPS 4
                valDict.Add(10329, mb.GetInt(egmData, 33).ToString());      // Actual TimaStamp
            }


            lastMeas = valDict;


            // ----- Permissions -----
            try
            {
                var perm = valDict[10295];    // get Model

                if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
            }
            catch { }

            try
            {
                return FillMeas(MeasList, valDict);                                   // Fill Measurement values
            }
            catch
            {
                return new List<DevMeasVals>();
            }
        }

        List<DevMeasVals> mbGetMCAMeasurement()
        {
            List<DevMeasVals> valList = new List<DevMeasVals>();

            Dictionary<int, string> valDict = new Dictionary<int, string>();

            if (MeasList == null) MeasList = CreateMeasList(devGetXML(), mode);                     // Create Measurement list

            ushort[] scaData;
            ushort[] scaStatus = new ushort[0];




            // ----- Get Measurement -----
            if (protocolVersion == 2)
            {
                ushort dataStart = dataPacketStart;
                if (dataStart > 10000) dataStart -= 10000;
                scaData = mb.ReadRegisters(dataStart, dataPacketLength, true);
            }
            else
            {
                scaStatus = mb.ReadRegisters(32, 9, true);
                scaData = mb.ReadRegisters(221, 26, true);
            }

            // ----- Parse Measurement -----

            if (protocolVersion == 2)
            {
                foreach (var item in MeasList)
                {
                    if ((item.ID >= dataPacketStart) && (item.ID <= dataPacketStart + dataPacketLength))
                    {
                        int pos = item.ID - dataPacketStart;
                        if (item.Format == "%f")
                        {
                            valDict.Add(item.ID, mb.GetFloat(scaData, pos).ToString());
                        }
                        else if (item.Format == "%u")
                        {
                            valDict.Add(item.ID, mb.GetUInt(scaData, pos).ToString());
                        }
                        else
                        {
                            if (scaData[pos] != 65535)
                                valDict.Add(item.ID, scaData[pos].ToString());
                        }
                    }
                }





                /*valDict.Add(10032, scaStatus[0].ToString());                // HV
                valDict.Add(10039, MB.GetInt(scaStatus, 7).ToString());     // Status
                if (scaStatus[3] != 65535)
                    valDict.Add(10035, scaStatus[3].ToString());            // Temperature
																						  

                valDict.Add(10295, scaData[0].ToString());                  // Tranzit
                valDict.Add(10296, MB.GetInt(scaData, 1).ToString());       // Alarms
                //valDict.Add(10298, MB.GetInt(scaData, 3).ToString());     // Status
                //valDict.Add(10300, scaData[5].ToString());                // Num. ROI
                valDict.Add(10301, MB.GetFloat(scaData, 6).ToString());     // Backgroung
                valDict.Add(10303, MB.GetFloat(scaData, 8).ToString());     // Critical limit

																				 
																						
                valDict.Add(10305, MB.GetFloat(scaData, 10).ToString());     // CPS
                valDict.Add(10307, MB.GetFloat(scaData, 12).ToString());
                valDict.Add(10309, MB.GetFloat(scaData, 14).ToString());
                valDict.Add(10311, MB.GetFloat(scaData, 16).ToString());
                valDict.Add(10313, MB.GetInt(scaData, 18).ToString());       // TimaStamp

																						
																							   
                valDict.Add(10321, MB.GetFloat(scaData, 26).ToString());    // Actual CPS
                valDict.Add(10323, MB.GetFloat(scaData, 28).ToString());
                valDict.Add(10325, MB.GetFloat(scaData, 30).ToString());
                valDict.Add(10327, MB.GetFloat(scaData, 32).ToString());*/

            }
            else
            {
                valDict.Add(10032, scaStatus[0].ToString());                // HV
                valDict.Add(10039, mb.GetInt(scaStatus, 7).ToString());     // Status
                if (scaStatus[3] != 65535)
                    valDict.Add(10035, scaStatus[3].ToString());            // Temperature
                valDict.Add(10223, mb.GetFloat(scaData, 2).ToString());     // CPS
                valDict.Add(10225, mb.GetFloat(scaData, 4).ToString());
                valDict.Add(10227, mb.GetFloat(scaData, 6).ToString());
                valDict.Add(10229, mb.GetFloat(scaData, 8).ToString());
                valDict.Add(10221, mb.GetInt(scaData, 0).ToString());       // TimaStamp

                valDict.Add(10239, mb.GetFloat(scaData, 18).ToString());    // Actual CPS
                valDict.Add(10241, mb.GetFloat(scaData, 20).ToString());
                valDict.Add(10243, mb.GetFloat(scaData, 22).ToString());
                valDict.Add(10245, mb.GetFloat(scaData, 24).ToString());
            }

            lastMeas = valDict;


            // ----- Permissions -----
            try
            {
                var perm = valDict[10295];    // get Model

                if (perm.IndexOf("0") == 0) Permission = DevPermission.None;
                if (perm.IndexOf("1") == 0) Permission = DevPermission.Advanced;
                if (perm.IndexOf("2") == 0) Permission = DevPermission.Service;
                if (perm.IndexOf("3") == 0) Permission = DevPermission.SuperUser;
            }
            catch { }


            try
            {
                return FillMeas(MeasList, valDict);                                   // Fill Measurement values
            }
            catch
            {
                return new List<DevMeasVals>();
            }
        }

        List<DevParams> mbGetDescription()
        {
            List<DevParams> list = new List<DevParams>();

            DeviceInfo status = mbGetInfo();

            DevParams val = new DevParams();
            val.ID = 1;
            val.Name = "Version";
            val.Value = status.Version;
            list.Add(val);

            val = new DevParams();
            val.ID = 2;
            val.Name = "SN";
            val.Value = status.SN;
            list.Add(val);

            val = new DevParams();
            val.ID = 3;
            val.Name = "Model";
            val.Value = status.Model;
            list.Add(val);

            return list;
        }
        
        #endregion

        #region Parameters

        DevParamVals mbGetParam(DevParamVals param)
        {
            if (ParamList == null) ParamList = CreateParamList(devGetXML(), mode, Permission);

            bool find = false;
            foreach (var item in ParamList)
            {
                if (item.ID == param.ID)
                {
                    find = true;
                    if (item.ValueType == VariableType.String)
                    {
                        if (item.ValueLength > 0)
                            param.Value = mb.ReadString8((ushort)param.ID, (ushort)(item.ValueLength / 2));
                        else
                            param.Value = "";
                    }
                    else if (item.ValueType == VariableType.Int)
                        param.Value = mb.ReadInt((ushort)param.ID).ToString();
                    else if (item.ValueType == VariableType.UInt)
                        param.Value = mb.ReadUInt((ushort)param.ID).ToString();
                    else if (item.ValueType == VariableType.Float)
                        param.Value = mb.ReadFloat((ushort)param.ID).ToString();
                    else if (item.ValueType == VariableType.Double)
                        param.Value = mb.ReadDouble((ushort)param.ID).ToString();
                    else if (item.ValueType == VariableType.Short)
                        param.Value = ((short)mb.ReadRegister((ushort)param.ID)).ToString();
                    else
                        param.Value = mb.ReadRegister((ushort)param.ID).ToString();
                    break;
                }
            }
            if (!find)
            {
                param.Value = mb.ReadRegister((ushort)param.ID).ToString();
            }
            return param;
        }

        List<DevParamVals> mbGetParams(List<DevParamVals> param)
        {
            if (ParamList == null) ParamList = CreateParamList(devGetXML(), mode, Permission);
            List<DevParamVals> list = new List<DevParamVals>();


            for (int i = 0; i < param.Count; i++)
            {
                bool find = false;
                foreach (var item in ParamList)
                {
                    if (item.ID == param[i].ID)
                    {
                        find = true;
                        if (item.ValueType == VariableType.String)
                        {
                            if (item.ValueLength > 0)
                                param[i].Value = mb.ReadString8((ushort)param[i].ID, (ushort)(item.ValueLength / 2));
                            else
                                param[i].Value = "";
                        }
                        else if (item.ValueType == VariableType.Int)
                            param[i].Value = mb.ReadInt((ushort)param[i].ID).ToString();
                        else if (item.ValueType == VariableType.UInt)
                            param[i].Value = mb.ReadUInt((ushort)param[i].ID).ToString();
                        else if (item.ValueType == VariableType.Float)
                            param[i].Value = mb.ReadFloat((ushort)param[i].ID).ToString();
                        else if (item.ValueType == VariableType.Double)
                            param[i].Value = mb.ReadDouble((ushort)param[i].ID).ToString();
                        else if (item.ValueType == VariableType.Short)
                            param[i].Value = ((short)mb.ReadRegister((ushort)param[i].ID)).ToString();
                        else
                            param[i].Value = mb.ReadRegister((ushort)param[i].ID).ToString();
                        list.Add(param[i]);
                        break;
                    }
                }
                if (!find)
                {
                    param[i].Value = mb.ReadRegister((ushort)param[i].ID).ToString();
                    list.Add(param[i]);
                }
            }
            return list;
        }

        List<DevParams> mbGetAllParams()
        {
            ushort regLen = 100;

            List<DevParams> list = new List<DevParams>();

            mb.SetTimeOuts(2000, 50);
            ushort[] regs = mb.ReadRegisters(0, regLen);
            mb.SetTimeOuts();

            if (ParamList == null) ParamList = CreateParamList(devGetXML(), mode, Permission);

            foreach (var item in ParamList)
            {
                try
                {
                    if (item.ValueType == VariableType.String)
                    {
                        if (item.ValueLength > 0)
                            item.Value = mb.GetString8(regs, item.ID, (ushort)(item.ValueLength / 2));
                        else
                            item.Value = "";
                    }
                    else if (item.ValueType == VariableType.Int)
                        item.Value = mb.GetInt(regs, item.ID).ToString();
                    else if (item.ValueType == VariableType.UInt)
                        item.Value = mb.GetUInt(regs, item.ID).ToString();
                    else if (item.ValueType == VariableType.Float)
                        item.Value = mb.GetFloat(regs, item.ID).ToString();
                    else if (item.ValueType == VariableType.Double)
                        item.Value = mb.GetDouble(regs, item.ID).ToString();
                    else if (item.ValueType == VariableType.Short)
                        item.Value = ((short)regs[item.ID]).ToString();
                    else
                        item.Value = regs[item.ID].ToString();
                    list.Add(item);
                }
                catch { }
            }

            return list;
        }

        void mbSetParam(int id, string param)
        {
            if (ParamList == null) ParamList = CreateParamList(devGetXML(), mode, Permission);

            bool find = false;
            foreach (var item in ParamList)
            {
                if (item.ID == id)
                {
                    find = true;
                    if (item.ValueType == VariableType.String)
                    {
                        if (item.ValueLength > 0)
                            mb.WriteString8((ushort)id, param, (ushort)(item.ValueLength / 2));
                    }
                    else if (item.ValueType == VariableType.Int)
                        mb.WriteInt((ushort)id, Conv.ToInt(param, 0));
                    else if (item.ValueType == VariableType.UInt)
                        mb.WriteUInt((ushort)id, Conv.ToUInt(param, 0));
                    else if (item.ValueType == VariableType.Float)
                        mb.WriteFloat((ushort)id, Conv.ToFloat(param, 0));
                    else if (item.ValueType == VariableType.Double)
                        mb.WriteDouble((ushort)id, Conv.ToDouble(param, 0));
                    else if (item.ValueType == VariableType.Short)
                        mb.WriteRegister((ushort)id, (ushort)Conv.ToShort(param, 0));
                    else
                        mb.WriteRegister((ushort)id, Conv.ToUShort(param, 0));
                    break;
                }
            }
            if (!find)
                mb.WriteRegister((ushort)id, Conv.ToUShort(param, 0));

        }

        void mbSetParams(List<DevParamVals> param)
        {
            if (ParamList == null) ParamList = CreateParamList(devGetXML(), mode, Permission);

            for (int i = 0; i < param.Count; i++)
            {
                bool find = false;
                foreach (var item in ParamList)
                {
                    if (item.ID == param[i].ID)
                    {
                        find = true;
                        if (item.ValueType == VariableType.String)
                        {
                            if (item.ValueLength > 0)
                                mb.WriteString8((ushort)param[i].ID, param[i].Value, (ushort)(item.ValueLength / 2));
                        }
                        else if (item.ValueType == VariableType.Int)
                            mb.WriteInt((ushort)param[i].ID, Conv.ToInt(param[i].Value, 0));
                        else if (item.ValueType == VariableType.UInt)
                            mb.WriteUInt((ushort)param[i].ID, Conv.ToUInt(param[i].Value, 0));
                        else if (item.ValueType == VariableType.Float)
                            mb.WriteFloat((ushort)param[i].ID, Conv.ToFloat(param[i].Value, 0));
                        else if (item.ValueType == VariableType.Double)
                            mb.WriteDouble((ushort)param[i].ID, Conv.ToDouble(param[i].Value, 0));
                        else if (item.ValueType == VariableType.Short)
                            mb.WriteRegister((ushort)param[i].ID, (ushort)Conv.ToShort(param[i].Value, 0));
                        else
                            mb.WriteRegister((ushort)param[i].ID, Conv.ToUShort(param[i].Value, 0));
                        break;
                    }
                }
                if (!find)
                    mb.WriteRegister((ushort)param[i].ID, Conv.ToUShort(param[i].Value, 0));
            }
        }

        #endregion

        Spectrum mbGetSpectrum()
        {
            mb.SetTimeOuts(2000, 200);
            byte[] rawData = mb.ReadFunction(70, new byte[] { 1, 0 });
            mb.SetTimeOuts();
            return mbParseSpectrum(rawData);
        }

        Spectrum mbParseSpectrum(byte[] data)
        {
            Spectrum Spectrum = new Spectrum();

            ushort byteLen = Conv.SwapBytes(BitConverter.ToUInt16(data, 0));
            ushort Compress = data[2];
            ushort LLD = Conv.SwapBytes(BitConverter.ToUInt16(data, 3));
            ushort ULD = Conv.SwapBytes(BitConverter.ToUInt16(data, 5));
            Spectrum.RealTime = Conv.SwapBytes(BitConverter.ToUInt32(data, 7)) / 1000.0f;
            Spectrum.LiveTime = Conv.SwapBytes(BitConverter.ToUInt32(data, 11)) / 1000.0f;
            ushort ChannelCount = Conv.SwapBytes(BitConverter.ToUInt16(data, 15));
            uint reserved = Conv.SwapBytes(BitConverter.ToUInt32(data, 17));

            int rangeCount = ULD - LLD + 1;
            Spectrum.Channels = new uint[ChannelCount];

            if (byteLen + 2 != data.Length) throw new CommException("Error parse packet!");


            int j = 21;
            for (int i = LLD; i <= ULD; i++)
            {
                if (Compress == 1)
                {
                    Spectrum.Channels[i] = data[j];
                }
                else if (Compress == 2)
                {
                    Spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt16(data, j));
                }
                else if (Compress == 3)
                {
                    byte[] tempBytes = new byte[4];
                    Array.Copy(data, j, tempBytes, 1, 3);
                    Spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt32(tempBytes, 0)) & 0xFFFFFF;
                }
                else if (Compress == 4)
                {
                    Spectrum.Channels[i] = Conv.SwapBytes(BitConverter.ToUInt32(data, j));
                }
                j += Compress;
            }

            return Spectrum;
        }

        SCAValue mbGetMCAValue()
        {
            SCAValue val = new SCAValue();
            Dictionary<int, string> valDict = new Dictionary<int, string>();

            // ----- Get Measurement -----
            ushort[] scaStatus = mb.ReadRegisters(32, 9, true);
            ushort[] scaData = mb.ReadRegisters(221, 26, true);

            // ----- Parse Measurement -----

            // ----- CPS -----
            ushort detNum = 4;
            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];



            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = mb.GetFloat(scaData, 2 + 2 * i);
                val.ActualCPS[i] = mb.GetFloat(scaData, 18 + 2 * i);
            }

            // ----- TimeStamp -----
            val.timeStamp = mb.GetInt(scaData, 0);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;



            // ----- Temperature -----
            if (scaStatus[3] == 65535)
                val.Temperature = float.NaN;
            else val.Temperature = scaStatus[3];

            // ----- Status -----
            val.Error = mb.GetInt(scaStatus, 7);
            val.Status = mb.GetInt(scaStatus, 7);

            val.isROIRunning = true;
            NuviaSCA_Status NuStat = (NuviaSCA_Status)val.Status;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN1))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN2))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN3))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN4))
                val.isROIRunning = false;

            return val;

        }

        SCAValue mbGetMCAValFromDict(Dictionary<int, string> dict)
        {
            SCAValue val = new SCAValue();

            ushort regCPS = 10223;
            ushort regActCPS = 10239;
            ushort regTimeStamp = 10221;
            ushort regTemp = 10035;
            ushort regStatus = 10039;

            if (protocolVersion == 2)
            {
                regCPS = 10305;
                regActCPS = 10321;
                regTimeStamp = 10313;
                regTemp = 10291;
                regStatus = 10298;
            }


            // ----- ROI CPS -----

            ushort detNum = 4;
            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];

            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = Conv.ToFloat(dict[regCPS + 2 * i], 0);
                try
                {
                    val.ActualCPS[i] = Conv.ToFloat(dict[regActCPS + 2 * i], 0);
                }
                catch { }
            }




            // ----- Measurement Timestamp -----
            val.timeStamp = Conv.ToInt(dict[regTimeStamp], 0);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;

            // ----- Temperature -----
            try
            {
                int temp = Conv.ToInt(dict[regTemp], 0);
                if (temp == 65535)
                    val.Temperature = float.NaN;
                else val.Temperature = temp;
            }
            catch
            {
                val.Temperature = float.NaN;
            }

            // ----- Status -----
            val.Error = Conv.ToInt(dict[regStatus], 0);
            val.Status = Conv.ToInt(dict[regStatus], 0);


            val.isROIRunning = true;
            NuviaSCA_Status NuStat = (NuviaSCA_Status)val.Status;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN1))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN2))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN3))
                val.isROIRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_NOTRUN4))
                val.isROIRunning = false;

            val.isSpectRunning = false;
            if (NuStat.HasFlag(NuviaSCA_Status.NuSCA_STATUS_SPECTRUM))
                val.isSpectRunning = true;

            return val;
        }

        SCASettings mbGetMCASettings()
        {
            mcaSettings = new SCASettings();
            mcaSettings.MeasureTime = mb.ReadHoldingInt(54);
            uint status = mb.ReadUInt(39, true);


            mcaSettings.HV = mb.ReadRegister(32);
            mcaSettings.HV_up = mb.ReadCoil(2);

            // ----- ROI Settings -----
            /*settings.Channels = new SCAChannel[4];
            for (ushort i = 0; i < 4; i++)
            {
                MB.WriteRegister(33, i);                             // Select ROI
                ushort[] scaVals = MB.ReadRegisters(34, 7);
                settings.Channels[i].LLD = scaVals[0];              // Fill LLD
                settings.Channels[i].ULD = scaVals[1];              // Fill ULD
                settings.Channels[i].Time = MB.GetUInt(scaVals, 4);        // Fill Time
                if (scaVals[6] == 1) settings.Channels[i].Autostart = true;    // Fill Autostart
                else settings.Channels[i].Autostart = false;
            }*/

            return mcaSettings;
        }


        GeigerValue mbGetEGMValue()
        {
            GeigerValue val = new GeigerValue();


            ushort[] regs = mb.ReadInputRegisters(300, 31);


            ushort detNum = regs[0];
            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];

            val.DR = mb.GetFloat(regs, 1) / 1000000f;
            val.ActualDR = mb.GetFloat(regs, 17) / 1000000f;

            val.Deviation = mb.GetFloat(regs, 3);
            val.ActualDeviation = mb.GetFloat(regs, 19);

            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = mb.GetFloat(regs, 5 + 2 * i);
                val.ActualCPS[i] = mb.GetFloat(regs, 21 + 2 * i);
            }


            val.timeStamp = mb.GetInt(regs, 13);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;

            val.Temperature = float.NaN;



            return val;

        }

        GeigerValue mbGetEGMValFromDict(Dictionary<int, string> dict)
        {
            GeigerValue val = new GeigerValue();



            ushort regTemp = 10035;


            if (protocolVersion == 2)
            {



                regTemp = 10291;

            }


            // ----- ROI CPS -----
            uint detNum = Conv.ToUInt(dict[10300], 4);
            if (detNum > 4) detNum = 4;
            val.CPS = new float[detNum];
            val.ActualCPS = new float[detNum];

            for (int i = 0; i < detNum; i++)
            {
                val.CPS[i] = Conv.ToFloat(dict[10305 + 2 * i], 0);


                val.ActualCPS[i] = Conv.ToFloat(dict[10321 + 2 * i], 0);


            }

            // ----- DR -----
            val.DR = Conv.ToFloat(dict[10301], 0) / 1000000f;
            val.ActualDR = Conv.ToFloat(dict[10317], 0) / 1000000f;
            val.Deviation = Conv.ToFloat(dict[10303], 0);
            val.ActualDeviation = Conv.ToFloat(dict[10319], 0);

            // ----- Measurement Timestamp -----
            val.timeStamp = Conv.ToInt(dict[10313], 0);
            if (val.timeStamp > 0) val.Valid = true;
            else val.Valid = false;

            // ----- Temperature -----
            try
            {
                int temp = Conv.ToInt(dict[regTemp], 0);
                if (temp == 65535)
                    val.Temperature = float.NaN;
                else val.Temperature = temp;
            }
            catch
            {
                val.Temperature = float.NaN;
            }

            // ----- Status -----
            val.Error = Conv.ToInt(dict[10298], 0);
            val.Status = Conv.ToInt(dict[10298], 0);



            NuviaEGM_Status NuStat = (NuviaEGM_Status)val.Status;
            if (NuStat.HasFlag(NuviaEGM_Status.NuEGM_STATUS_NOTRUN))
                val.isRunning = false;
            else
                val.isRunning = true;


            return val;
        }

        GeigerSettings mbGetEGMSettings()
        {
            egmSettings = new GeigerSettings();
            egmSettings.MeasureTime = mb.ReadHoldingInt(54);
            return egmSettings;
        }
    }
}
