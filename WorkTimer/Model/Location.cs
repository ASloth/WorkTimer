using System;

namespace WorkTimer.Model
{
    public class Location
    {
        public double Longitude { get; set; } 

        public double Latitude { get; set; }

        public double DistanceTo(Location location) => DistanceTo(location.Latitude, location.Longitude);

        public double DistanceTo(double lat, double lon)
        {
            double rlat1 = Math.PI * Latitude / 180;
            double rlat2 = Math.PI * lat / 180;
            double theta = Longitude - lon;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515; 

            return dist * 1.609344;
        }
    }  
}