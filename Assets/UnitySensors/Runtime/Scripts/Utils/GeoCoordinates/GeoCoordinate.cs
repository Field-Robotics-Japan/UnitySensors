namespace UnitySensors.Utils.GeoCoordinate
{
    [System.Serializable]
    public struct GeoCoordinate
    {
        public GeoCoordinate(double lat, double lon, double alt)
        {
            latitude = lat;
            longitude = lon;
            altitude = alt;
        }
        public double latitude;
        public double longitude;
        public double altitude;
    }
}
