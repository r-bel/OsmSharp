using System;

namespace OsmSharp.Math.Geo
{
    public static class GeoMath
    {
        public struct Degree
        {
            private bool isNormalized;
            private double value;

            public Degree(double value)
            {
                isNormalized = false;
                this.value = value;
            }

            public static Degree operator -(Degree a, Degree b)
            {
                return new Degree(a.value - b.value);
            }

            public static implicit operator double(Degree degree)
            {
                return degree.value;
            }

            public static implicit operator Degree(double value)
            {
                return new Degree(value);
            }

            public Radian ToRadian()
            {
                return new Radian((value / 180d) * System.Math.PI);
            }
        }
        public struct Radian
        {
            private bool isNormalized;
            private double value;

            public Radian(double value)
            {
                isNormalized = false;
                this.value = value;
            }

            public static Radian operator -(Radian a, Radian b)
            {
                return new Radian(a.value - b.value);
            }

            public static implicit operator double(Radian radian)
            {
                return radian.value;
            }
        }
        public interface ILatLonInDegrees
        {
            Degree Latitude { get; }
            Degree Longitude { get; }
        }
        public interface ILatLonInRadians
        {
            Radian Latitude { get; }
            Radian Longitude { get; }
        }
        public struct SimpleCoordinate : ILatLonInDegrees, ILatLonInRadians
        {
            private Degree latitude, longitude;

            public SimpleCoordinate(Degree latitude, Degree longitude) : this()
            {
                this.latitude = latitude;
                this.longitude = longitude;
            }

            Degree ILatLonInDegrees.Latitude
            {
                get { return latitude; }
            }

            Degree ILatLonInDegrees.Longitude
            {
                get { return longitude; }
            }

            Radian ILatLonInRadians.Latitude
            {
                get { return latitude.ToRadian(); }
            }

            Radian ILatLonInRadians.Longitude
            {
                get { return longitude.ToRadian(); }
            }
        }
        public struct Meter
        {
            private double value;

            public Meter(double value)
            {
                this.value = value;
            }

            public static implicit operator double(Meter meter)
            {
                return meter.value;
            }
        }
        public static Meter DistanceReal(ILatLonInRadians pointA, ILatLonInRadians pointB)
        {
            var lat1_rad = pointA.Latitude;
            var lon1_rad = pointA.Longitude;
            var lat2_rad = pointB.Latitude;
            var lon2_rad = pointB.Longitude;
            
            var dLat = (lat2_rad - lat1_rad);
            var dLon = (lon2_rad - lon1_rad);

            var a = System.Math.Pow(System.Math.Sin(dLat / 2), 2) +
                       System.Math.Cos(lat1_rad) * System.Math.Cos(lat2_rad) *
                       System.Math.Pow(System.Math.Sin(dLon / 2), 2);

            var c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

            return new Meter(Constants.RadiusOfEarth * c);
        }
    }
}
