using System;

namespace DataService.Utils
{
    public static class LocationUtil
    {
        public static readonly double AcceptableDistance = 0.010; //in km
        
        //https://stackoverflow.com/questions/27928/calculate-distance-between-two-latitude-longitude-points-haversine-formula?page=1&tab=votes#tab-top
        public static double CalculateDistance(double long1, double lat1, double long2, double lat2)
        {
            const int earthR = 6371; // Radius of the earth in km
            var dLat = DegreeToRadius(lat2 - lat1); // deg2rad below
            var dLong = DegreeToRadius(long2 - long1);
            var a =
                    Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreeToRadius(lat1)) * Math.Cos(DegreeToRadius(lat2)) *
                    Math.Sin(dLong / 2) * Math.Sin(dLong / 2)
                ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = earthR * c; // Distance in km
            return d;
        }

        private static double DegreeToRadius(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}