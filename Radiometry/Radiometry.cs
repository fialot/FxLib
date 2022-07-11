using Fx.Conversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fx.Radiometry
{
    public static class Radiometry
    {
        public static Spectrum ParseN42(string text)
        {
            Spectrum spect = new Spectrum();

            // ----- Parse XML to Structure -----
            var xml = XDocument.Parse(text);
            IEnumerable<XElement> spectrum;
            spectrum = xml.Elements().Elements("Measurement").Elements("Spectrum").Elements();


            foreach (var data in spectrum)
            {
                if (data.Name == "StartTime")
                {
                    try
                    {
                        spect.StartTime = DateTime.Parse(data.Value);//.ParseExact(data.Value, "yyyy-MM-ddTHH:mm:ss", null); //CultureInfo.InvariantCulture
                    }
                    catch
                    {
                        spect.StartTime = DateTime.MinValue;
                    }
                }
                else if (data.Name == "RealTime")
                {
                    spect.RealTime = (float)Conv.ToRealNumber(data.Value);
                }
                else if (data.Name == "LiveTime")
                {
                    spect.LiveTime = (float)Conv.ToRealNumber(data.Value);
                }
                else if (data.Name == "Calibration")
                {
                    if (data.Attribute("Type") != null)
                    {
                        
                        XElement equation = data.Element("Equation");
                        if (data.Attribute("Type").Value == "Energy")
                        {
                            EnergyCalibration cal = new EnergyCalibration();
                            if (equation.Attribute("Model") != null)
                            {
                                if (equation.Attribute("Model").Value == "Linear")
                                    cal.Type = EnergyCalibrationType.Linear;
                                else if (equation.Attribute("Model").Value == "Polynomial")
                                    cal.Type = EnergyCalibrationType.Polynomial;
                                XElement coef = equation.Element("Coefficients");
                                if (coef != null)
                                {
                                    string[] strCoef = coef.Value.Split(new string[] { " " }, StringSplitOptions.None);
                                    if (strCoef.Length >= 1) cal.A = Conv.ToFloatI(strCoef[0], 0);
                                    if (strCoef.Length >= 2) cal.B = Conv.ToFloatI(strCoef[1], 0);
                                    if (strCoef.Length >= 3) cal.C = Conv.ToFloatI(strCoef[2], 0);
                                }

                            }
                            spect.Energy = cal;
                        }
                        else if (data.Attribute("Type").Value == "FWHM")
                        {
                            FWHMCalibration cal = new FWHMCalibration();

                            if (equation.Attribute("Model") != null)
                            {
                                if (equation.Attribute("Model").Value == "GENIE-SQRT")
                                    cal.Type = FWHMCalibrationType.GenieSQRT;
                                else if (equation.Attribute("Model").Value == "GENIE-POLYNOM")
                                    cal.Type = FWHMCalibrationType.GeniePolynom;
                                else if (equation.Attribute("Model").Value == "POLYNOMIAL")
                                    cal.Type = FWHMCalibrationType.Polynomial;
                                else if (equation.Attribute("Model").Value == "DH")
                                    cal.Type = FWHMCalibrationType.DH;
                                else if (equation.Attribute("Model").Value == "SRQADR")
                                    cal.Type = FWHMCalibrationType.SRQADR;
                                else if (equation.Attribute("Model").Value == "LINEAR")
                                    cal.Type = FWHMCalibrationType.Linear;
                                else if (equation.Attribute("Model").Value == "CONSTANT")
                                    cal.Type = FWHMCalibrationType.Constant;

                                XElement coef = equation.Element("Coefficients");
                                if (coef != null)
                                {
                                    string[] strCoef = coef.Value.Split(new string[] { " " }, StringSplitOptions.None);
                                    if (strCoef.Length >= 1) cal.A = Conv.ToFloatI(strCoef[0], 0);
                                    if (strCoef.Length >= 2) cal.B = Conv.ToFloatI(strCoef[1], 0);
                                    if (strCoef.Length >= 3) cal.C = Conv.ToFloatI(strCoef[2], 0);
                                }

                            }
                            spect.FWHM = cal;
                        }
                    }
                }
                else if (data.Name == "ChannelData")
                {
                    string[] strChannels = data.Value.Split(new string[] { " ", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    spect.Channels = new uint[strChannels.Length];
                    for (int i = 0; i < spect.Channels.Length; i++) spect.Channels[i] = Conv.ToUInt(strChannels[i], 0);
                }
            }

            return spect;

        }

        public static string CreateN42(Spectrum spectrum)
        {
            string ChannelData = "";

            string EnType = "Polynomial";
            string EnCoef = ((float)spectrum.Energy.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.Energy.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.Energy.C).ToString(CultureInfo.InvariantCulture);

            string FWHMType = "POLYNOMIAL";
            string FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.C).ToString(CultureInfo.InvariantCulture);

            // ----- Convert Channels to string -----
            for (int i = 0; i < spectrum.Channels.Length; i++)
            {
                if (i % 12 == 0) ChannelData += Environment.NewLine + "        ";
                else ChannelData += " ";

                ChannelData += spectrum.Channels[i].ToString();
            }
            if (ChannelData != "") ChannelData += Environment.NewLine + "      ";

            // ----- Convert Energy constants -----
            if (spectrum.Energy.Type == EnergyCalibrationType.Linear)
            {
                EnType = "Linear";
                EnCoef = ((float)spectrum.Energy.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.Energy.B).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.Energy.Type == EnergyCalibrationType.Polynomial)
            {
                EnType = "Polynomial";
                EnCoef = ((float)spectrum.Energy.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.Energy.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.Energy.C).ToString(CultureInfo.InvariantCulture);
            }

            // ----- Convert FWHM constants -----
            if (spectrum.FWHM.Type == FWHMCalibrationType.GenieSQRT)
            {
                FWHMType = "GENIE-SQRT";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.GeniePolynom)
            {
                FWHMType = "GENIE-POLYNOM";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.C).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.Polynomial)
            {
                FWHMType = "POLYNOMIAL";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.C).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.DH)
            {
                FWHMType = "DH";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.SRQADR)
            {
                FWHMType = "SRQADR";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.C).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.Linear)
            {
                FWHMType = "LINEAR";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture) + " " + ((float)spectrum.FWHM.B).ToString(CultureInfo.InvariantCulture);
            }
            else if (spectrum.FWHM.Type == FWHMCalibrationType.Constant)
            {
                FWHMType = "CONSTANT";
                FWHMCoef = ((float)spectrum.FWHM.A).ToString(CultureInfo.InvariantCulture);
            }

            XDocument doc = new XDocument(
              new XDeclaration("1.0", "utf-8", null),
              new XElement("N42InstrumentData",
                new XElement("Measurement",
                  new XElement("Spectrum",
                    new XElement("StartTime", spectrum.StartTime.ToString("yyyy-MM-ddTHH:mm:ss")),
                    new XElement("RealTime", "PT" + spectrum.RealTime.ToString(CultureInfo.InvariantCulture) + "S"),
                    new XElement("LiveTime", "PT" + spectrum.LiveTime.ToString(CultureInfo.InvariantCulture) + "S"),
                    new XElement("Calibration", new XAttribute("Type", "Energy"), new XAttribute("EnergyUnits", "keV"),
                      new XElement("Equation", new XAttribute("Model", EnType),
                        new XElement("Coefficients", EnCoef)
                      )
                    ),
                    new XElement("Calibration", new XAttribute("Type", "FWHM"), new XAttribute("EnergyUnits", "channel"), new XAttribute("ID", "fw"),
                      new XElement("Equation", new XAttribute("Model", FWHMType),
                        new XElement("Coefficients", FWHMCoef)
                      )
                    ),

                    new XElement("ChannelData", ChannelData)
                  )
                )
              )
            );

            var wr = new Utf8StringWriter();
            doc.Save(wr);
            return wr.ToString();
        }
    }
}
