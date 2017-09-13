using System.Globalization;

namespace WikiImagesProcessor.Abstractions.Model
{
    public struct Coordinates
    {
        public readonly double Longitude;
        public readonly double Latitude;

        public Coordinates(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public string ToGpsCoords()
        {
            var numFormatInfo = new NumberFormatInfo() {NumberDecimalSeparator = "."};

            return $"{Latitude.ToString("F6", numFormatInfo)}|{Longitude.ToString("F6", numFormatInfo)}";
        }
    }
}