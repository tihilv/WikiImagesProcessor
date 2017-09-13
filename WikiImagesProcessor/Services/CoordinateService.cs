using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WikiImagesProcessor.Abstractions.Model;
using WikiImagesProcessor.Abstractions.Services;

namespace WikiImagesProcessor.Services
{
    class CoordinateService : ICoordinateService
    {
        private readonly ILogService _logService;
        private readonly IJsonHttpService _jsonProvider;

        public CoordinateService(IJsonHttpService jsonProvider, ILogService logService)
        {
            _jsonProvider = jsonProvider;
            _logService = logService;
        }

        public async Task<Coordinates> GetCoordinatesByName(string locationName)
        {
            var locationInfo = await _jsonProvider.GetJson<LocationInfo>($"http://maps.google.com/maps/api/geocode/json?address={locationName}");

            if (!locationInfo.Status.Equals("OK", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Location '{locationName}' is not found.");

            if (locationInfo.Results.Length > 1)
                _logService.Info($"{locationInfo.Results} results found. Taking the first one.");

            var result = locationInfo.Results.First();

            _logService.Info($"Address is '{result.FormattedAddress}'.");

            return result.Geometry.Location.ToCoordinates();
        }

        private class LocationInfo
        {
            [JsonProperty("results")]
            public Result[] Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            internal class Result
            {
                [JsonProperty("formatted_address")]
                public string FormattedAddress { get; set; }

                [JsonProperty("geometry")]
                public Geometry Geometry { get; set; }
            }

            internal class Geometry
            {
                [JsonProperty("location")]
                public Location Location { get; set; }
            }

            internal class Location
            {
                [JsonProperty("lat")]
                public double Lat { get; set; }

                [JsonProperty("lng")]
                public double Lng { get; set; }

                public Coordinates ToCoordinates()
                {
                    return new Coordinates(Lng, Lat);
                }
            }
        }
    }
}
