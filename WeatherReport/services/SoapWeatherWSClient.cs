using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherReport.Models;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Runtime.Serialization;

namespace WeatherReport.services
{
    public class SoapWeatherWSClient
    {

        public string WeatherReport(ZipCode zip)
        {
            SoapWeatherWSClient weatherSoapClient = new SoapWeatherWSClient();

            string ns = "http://ws.cdyne.com/WeatherWS/";
            string wsmethod = "http://wsf.cdyne.com/WeatherWS/Weather.asmx?op=GetCityForecastByZIP";

            string zipCode = zip.zip_code;
            string xml = weatherSoapClient.CallWeatherWS(zipCode, ns, wsmethod);

            ForecastResult forcasts = weatherSoapClient.SoapXML2DataObj(xml);

            string json = weatherSoapClient.DataObj2Jason(forcasts);

            return json;
            //System.Web.HttpContext.Current.Response.Write(json);

        }


        public string DataObj2Jason(ForecastResult result)
        {
            ArrayList myforcasts = result.Forcasts;

            if (result.Success.Equals("true"))
            {
                result.Success = "0";
            }
            else
            {
                result.Success = "Failed to get correct response from http://wsf.cdyne.com/WeatherWS/Weather.asmx?WSDL. Pleaese use another zip code and try again.";
            }


            string ret = "{ ";
            ret += "\"result_code\":\"" + result.Success + "\",";
            ret += "\"result_content\":{";
            ret += "\"data\": {";
            ret += "\"city_name\": \"" + result.City + "\",";

            int idx = 0;

            foreach (Forecast myforcat in myforcasts)
            {
                ret += "\"" + myforcat.Date + "\": {";
                ret += "\"morning_low\":\"" + myforcat.MorningLow + "\",";
                ret += "\"daytime_high\":\"" + myforcat.DaytimeHigh + "\"";

                if (myforcasts.Count - 1 == idx)
                    ret += "}";
                else
                    ret += "},";

                idx++;
            }

            ret += "}";
            ret += "}";
            ret += "}";

            return ret;
        }


        public ForecastResult SoapXML2DataObj(string soapxml)
        {
            int nStart = soapxml.IndexOf("<GetCityForecastByZIPResult>");
            int nEnd = soapxml.IndexOf("</GetCityForecastByZIPResult>");
            string xmlweatherresults = soapxml.Substring(nStart, nEnd - nStart + "</GetCityForecastByZIPResult>".Length);

            ForecastResult result = new ForecastResult();
            result.Forcasts = new ArrayList();
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xmlweatherresults);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/GetCityForecastByZIPResult");
            String Success = nodes[0].SelectSingleNode("Success").InnerText;

            if (Success.Equals("true"))
            {
                result.Success = "true";
                result.ResponseText = nodes[0].SelectSingleNode("ResponseText").InnerText;
                result.State = nodes[0].SelectSingleNode("State").InnerText;
                result.City = nodes[0].SelectSingleNode("City").InnerText;
                result.WeatherStationCity = nodes[0].SelectSingleNode("WeatherStationCity").InnerText;

                nodes = doc.DocumentElement.SelectNodes("/GetCityForecastByZIPResult/ForecastResult/Forecast");
                foreach (XmlNode node in nodes)
                {
                    Forecast myforcat = new Forecast();
                    myforcat.Date = node.SelectSingleNode("Date").InnerText;
                    myforcat.WeatherID = node.SelectSingleNode("WeatherID").InnerText;
                    myforcat.MorningLow = node.SelectSingleNode("Temperatures/MorningLow").InnerText;
                    myforcat.DaytimeHigh = node.SelectSingleNode("Temperatures/DaytimeHigh").InnerText;
                    myforcat.Nighttime = node.SelectSingleNode("ProbabilityOfPrecipiation/Nighttime").InnerText;
                    myforcat.Daytime = node.SelectSingleNode("ProbabilityOfPrecipiation/Daytime").InnerText;
                    result.Forcasts.Add(myforcat);
                }

            }
            else
            {
                result.Success = "404";
                result.ResponseText = nodes[0].SelectSingleNode("ResponseText").InnerText;
                result.State = nodes[0].SelectSingleNode("State").InnerText;
                result.City = nodes[0].SelectSingleNode("City").InnerText;
                result.WeatherStationCity = nodes[0].SelectSingleNode("WeatherStationCity").InnerText;
            }

            //Console.WriteLine(DataObj2Jason(result));
            return result;


        }

        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public string CallWeatherWS(string zip, string ns, string wsmethod)
        {
            string xml = "";
            HttpWebRequest request = CreateWebRequest(wsmethod);
            XmlDocument soapEnvelopeXml = new XmlDocument();

            xml += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xml += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            xml += "   <soap:Body>";
            xml += "     <GetCityForecastByZIP xmlns=\"" + ns + "\" >";
            xml += "        <ZIP>" + zip + "</ZIP>";
            xml += "     </GetCityForecastByZIP>";
            xml += "   </soap:Body>";
            xml += "</soap:Envelope>";

            soapEnvelopeXml.LoadXml(xml);

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();

                    return soapResult;
                }
            }
        }


        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        public HttpWebRequest CreateWebRequest(string wsmethod)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(wsmethod);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

    }
}