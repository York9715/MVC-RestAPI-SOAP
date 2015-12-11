using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeatherReport.Models;
using WeatherReport.services;

namespace WeatherReport.Controllers
{
    public class WeatherController : ApiController
    {
        // POST api/weather
        public string Post([FromBody]ZipCode zip)
        {
            string wkCode = "";
            ZipCode wkZipCode = new ZipCode();
            SoapWeatherWSClient ws = new SoapWeatherWSClient();

            if (zip != null) wkCode = zip.zip_code;

            wkZipCode.zip_code = wkCode;

            wkCode = ws.WeatherReport(wkZipCode);

            return wkCode;

        }
    }
}
