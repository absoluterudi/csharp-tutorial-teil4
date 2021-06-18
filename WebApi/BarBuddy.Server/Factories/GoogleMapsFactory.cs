using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;

namespace BarBuddy.Server.Factories
{
    public class GoogleMapsFactory
    {
        public const int SRID = 4326;

        private IConfiguration _configuration;

        public GoogleMapsFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Point GetLocation(Entities.Adress location)
        {
            var googleUrl = _configuration["GoogleMapsAPI"];
            var url = $"{googleUrl}&address={location.Street},{location.PostalCode},{location.City},{location.Country}";

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;

                    JObject jObject = JObject.Parse(webClient.DownloadString(url));

                    var status = jObject["status"];
                    if (status.Value<string>() != "OK")
                    {
                        return null;
                    }

                    foreach (JToken result in jObject["results"])
                    {
                        var lat = result["geometry"]["location"]["lat"];
                        var latValue = lat.Value<double>();

                        var lng = result["geometry"]["location"]["lng"];
                        var lngValue = lng.Value<double>();

                        return new Point(lngValue, latValue) { SRID = SRID };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
