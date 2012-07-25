﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data;
using Tools.Xml.Sources;
using System.Data;
using System.IO;
using Tools.Math.Geo;
using Osm.Routing.Raw;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Core.Xml;

namespace Osm.Routing.Test.Matrix
{
    class MatrixTest
    {
        public static void TestSmall()
        {
            MatrixTest.Test("matrix");
        }
        //public static void TestLebbeke()
        //{
        //    MatrixTest.Test("lebbeke");
        //}

        public static void TestBigArea()
        {
            MatrixTest.Test("matrix_big_area");
        }

        internal static void TestAtomicSmall()
        {
            MatrixTest.Test("atomic");
        }

        internal static void TestMoscow()
        {
            MatrixTest.Test("moscow");
        }

        internal static void TestTiny()
        {
            MatrixTest.Test("tiny");
        }

        public static void Test(string name)
        {
            MatrixTest.Test(name, 10);
        }

        public static void Test(string name, int test_count)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null, 
                new System.IO.FileInfo(info.FullName + string.Format("\\Matrix\\{0}.csv",name)), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            long ticks_total_total = 0;
            long ticks_total_resolved = 0;
            long ticks_total_calculated = 0;

            int current_count = test_count;
            while (current_count > 0)
            {
                // initialize data.
                IDataSourceReadOnly data_source = new OsmDataSource(
                    new OsmDocument(new XmlFileSource(info.FullName + string.Format("\\Matrix\\{0}.osm",name))));
                data_source = new Osm.Data.Cache.DataSourceCache(data_source, 12);

                // create router.
                Router router = new Router(data_source);
                //router.RegisterProgressReporter(new ConsoleProgressReporter());
                // calculate matrix.
                long ticks = DateTime.Now.Ticks;
                ResolvedPoint[] points = router.Resolve(coordinates.ToArray());
                long ticks_resolved = DateTime.Now.Ticks;
                Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_resolved - ticks).TotalSeconds);
                ticks_total_resolved = ticks_total_resolved + (ticks_resolved - ticks);

                // check connectivity for all points.
                bool[] connectivity = router.CheckConnectivity(points, 1000);

                //router.CalculateManyToManyWeightsSparse(points);
                float[][] matrix = router.CalculateManyToManyWeight(points, points);
                long ticks_calculated = DateTime.Now.Ticks;
                ticks_total_calculated = ticks_total_calculated + (ticks_calculated - ticks_resolved);
                Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_calculated - ticks_resolved).TotalSeconds);
                Console.WriteLine("Total {0} seconds!", new TimeSpan(ticks_calculated - ticks).TotalSeconds);
                ticks_total_total = ticks_total_total + (ticks_calculated - ticks);
                current_count--;
            }

            Console.WriteLine("Resolved in {0} seconds!", new TimeSpan(ticks_total_resolved / test_count).TotalSeconds);
            Console.WriteLine("Calculated in {0} seconds!", new TimeSpan(ticks_total_calculated / test_count).TotalSeconds);
            Console.WriteLine("Total {0} seconds!", new TimeSpan(ticks_total_total / test_count).TotalSeconds);
            Console.ReadLine();

         }
    }
}
