﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Data.Redis
{

    public static class GeoHash
    {
        private static readonly char[] chars = {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm',
        'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
        'y', 'z'
        };
        private static readonly Dictionary<char, int> map = new Dictionary<char, int>();
        private const int precision = 12;
        private static readonly int[] bits = { 16, 8, 4, 2, 1 };

        static GeoHash()
        {
            for (int i = 0; i < chars.Length; i++)
                map.Add(chars[i], i);
        }

        public static String Encode(double latitude, double longitude)
        {
            double[] latInterval = { -90.0, 90.0 };
            double[] lonInterval = { -180.0, 180.0 };

            var geohash = new StringBuilder();
            bool isEven = true;
            int bit = 0, ch = 0;

            while (geohash.Length < precision)
            {
                double mid;
                if (isEven)
                {
                    mid = (lonInterval[0] + lonInterval[1]) / 2;
                    if (longitude > mid)
                    {
                        ch |= bits[bit];
                        lonInterval[0] = mid;
                    }
                    else
                    {
                        lonInterval[1] = mid;
                    }

                }
                else
                {
                    mid = (latInterval[0] + latInterval[1]) / 2;
                    if (latitude > mid)
                    {
                        ch |= bits[bit];
                        latInterval[0] = mid;
                    }
                    else
                    {
                        latInterval[1] = mid;
                    }
                }

                isEven = isEven ? false : true;

                if (bit < 4)
                    bit++;
                else
                {
                    geohash.Append(chars[ch]);
                    bit = 0;
                    ch = 0;
                }
            }

            return geohash.ToString();
        }

        public static double[] Decode(String geohash)
        {
            double[] ge = DecodeExactly(geohash);
            double lat = ge[0];
            double lon = ge[1];
            double latErr = ge[2];
            double lonErr = ge[3];

            double latPrecision = Math.Max(1, Math.Round(-Math.Log10(latErr))) - 1;
            double lonPrecision = Math.Max(1, Math.Round(-Math.Log10(lonErr))) - 1;

            lat = GetPrecision(lat, latPrecision);
            lon = GetPrecision(lon, lonPrecision);

            return new[] { lat, lon };
        }

        public static double[] DecodeExactly(String geohash)
        {
            double[] latInterval = { -90.0, 90.0 };
            double[] lonInterval = { -180.0, 180.0 };

            double latErr = 90.0;
            double lonErr = 180.0;
            bool isEven = true;
            int sz = geohash.Length;
            int bsz = bits.Length;
            for (int i = 0; i < sz; i++)
            {

                int cd = map[geohash[i]];

                for (int z = 0; z < bsz; z++)
                {
                    int mask = bits[z];
                    if (isEven)
                    {
                        lonErr /= 2;
                        if ((cd & mask) != 0)
                            lonInterval[0] = (lonInterval[0] + lonInterval[1]) / 2;
                        else
                            lonInterval[1] = (lonInterval[0] + lonInterval[1]) / 2;

                    }
                    else
                    {
                        latErr /= 2;

                        if ((cd & mask) != 0)
                            latInterval[0] = (latInterval[0] + latInterval[1]) / 2;
                        else
                            latInterval[1] = (latInterval[0] + latInterval[1]) / 2;
                    }
                    isEven = isEven ? false : true;
                }

            }
            double latitude = (latInterval[0] + latInterval[1]) / 2;
            double longitude = (lonInterval[0] + lonInterval[1]) / 2;

            return new[] { latitude, longitude, latErr, lonErr };
        }

        public static double GetPrecision(double x, double precision)
        {
            double @base = Math.Pow(10, -precision);
            double diff = x % @base;
            return x - diff;
        }
    }
}
