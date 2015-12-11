using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Runtime.Serialization;

namespace WeatherReport.Models
{
    public class ZipCode
    {
        public string zip_code { get; set; }
    }

    public class ForecastResult
    {
        public string Success { get; set; }
        public string ResponseText { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string WeatherStationCity { get; set; }
        public ArrayList Forcasts { get; set; }
    }

    public class Forecast
    {
        public string Date { get; set; }
        public string WeatherID { get; set; }
        public string MorningLow { get; set; }
        public string DaytimeHigh { get; set; }
        public string Nighttime { get; set; }
        public string Daytime { get; set; }
    }

}