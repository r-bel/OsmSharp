﻿//using Antlr.Runtime;
//using Osm.DataProcessor.Core;
//using Osm.DataProcessor.Filter.BoundingBox;
//using Osm.DataProcessor.Filter.Sort;
//using Osm.DataProcessor.OracleSimple;
//using Osm.DataProcessor.Xml;
//using Tools.Math.Geo;
//using System;
//using Osm.DataProcessor.Core.ChangeSets;
//using Osm.DataProcessor.OracleSimple.ChangeSets;

//namespace Osm.Data.Processor.Main
//{
//    /// <summary>
//    /// Static class implementing all parsing functions to support the parser generated by ANTLR.
//    /// </summary>
//    public static class ArgumentsParser
//    {
//        /// <summary>
//        /// Parses the command line arguments and returns the actual initialized target.
//        /// </summary>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        public static object GetTargetToPull(string[] args)
//        {
//            // create char-stream.
//            string input = string.Join(" ",args);
//            ICharStream char_stream = new ANTLRStringStream(input);

//            // create lexer and token stream.
//            osmosisLexer lexer = new osmosisLexer(char_stream);
//            ITokenStream tokens = new CommonTokenStream(lexer);

//            // create parser and parse.
//            osmosisParser parser = new osmosisParser(tokens);
//            osmosisParser.parse_return ret = parser.parse();

//            // initialize target.
//            if (ret.target is DataProcessorTarget)
//            {
//                (ret.target as DataProcessorTarget).Initialize();
//            }
//            else
//            {
//                throw new NotImplementedException();
//            }

//            return ret.target;
//        }

//        #region OsmGeo Data

//        #region Xml

//        /// <summary>
//        /// Returns an xml target.
//        /// </summary>
//        /// <param name="filename"></param>
//        /// <returns></returns>
//        internal static DataProcessorTarget CreateXmlTarget(string filename)
//        {
//            return new XmlDataProcessorTarget(StripQuotes(filename));
//        }

//        /// <summary>
//        /// Returns an xml source.
//        /// </summary>
//        /// <param name="filename"></param>
//        /// <returns></returns>
//        internal static DataProcessorSource CreateXmlSource(string filename)
//        {
//            return new XmlDataProcessorSource(StripQuotes(filename));
//        }

//        #endregion

//        #region Oracle

//        /// <summary>
//        /// Creates an oracle source.
//        /// </summary>
//        /// <param name="connection_string"></param>
//        /// <returns></returns>
//        internal static DataProcessorSource CreateOracleSource(string connection_string)
//        {
//            return new OracleSimpleDataProcessorSource(StripQuotes(connection_string));
//        }

//        /// <summary>
//        /// Creates an oracle target.
//        /// </summary>
//        /// <param name="connection_string"></param>
//        /// <returns></returns>
//        internal static DataProcessorTarget CreateOracleTarget(string connection_string)
//        {
//            return new OracleSimpleDataProcessorTarget(StripQuotes(connection_string));
//        }

//        #endregion

//        #region Filters

//        /// <summary>
//        /// Creates a bounding box filter.
//        /// </summary>
//        /// <param name="left"></param>
//        /// <param name="right"></param>
//        /// <param name="top"></param>
//        /// <param name="bottom"></param>
//        /// <returns></returns>
//        internal static DataProcessorFilter CreateBoundingBoxFilter(
//            double left, double right, double top, double bottom)
//        {
//            GeoCoordinateBox box = new GeoCoordinateBox(new GeoCoordinate(top,left),new GeoCoordinate(bottom,right));
//            return new DataProcessorFilterBoundingBox(box);
//        }

//        /// <summary>
//        /// Creates a sorting filter.
//        /// </summary>
//        /// <returns></returns>
//        internal static DataProcessorFilter CreateSortingFilter()
//        {
//            return new DataProcessorFilterSort();
//        }

//        #endregion

//        #region Links

//        /// <summary>
//        /// Links a source to a target.
//        /// </summary>
//        /// <param name="dataProcessorSource"></param>
//        /// <param name="dataProcessorTarget"></param>
//        internal static void LinkSourceToTarget(DataProcessorSource dataProcessorSource, DataProcessorTarget dataProcessorTarget)
//        {
//            dataProcessorTarget.RegisterSource(dataProcessorSource);
//        }

//        /// <summary>
//        /// Links a filter to a source.
//        /// </summary>
//        /// <param name="dataProcessorFilter"></param>
//        /// <param name="dataProcessorSource"></param>
//        /// <returns></returns>
//        internal static DataProcessorFilter LinkFilterToSource(DataProcessorFilter dataProcessorFilter, DataProcessorSource dataProcessorSource)
//        {
//            dataProcessorFilter.RegisterSource(dataProcessorSource);
//            return dataProcessorFilter;
//        }

//        #endregion

//        #region Default .NET type parsing

//        /// <summary>
//        /// Parses a double using the InvariantCulture.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        internal static double ParseDouble(string value)
//        {
//            return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
//        }

//        /// <summary>
//        /// Removes all quotes from a string.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        internal static string StripQuotes(string value)
//        {
//            return value.Trim('"');
//        }

//        #endregion

//        #endregion

//        #region osmChange Data
//        /// <summary>
//        /// Links the given filter to the given source.
//        /// </summary>
//        /// <param name="filter"></param>
//        /// <param name="source"></param>
//        /// <returns></returns>
//        internal static DataProcessorChangeSetFilter LinkChangeFilterToSource(
//            DataProcessorChangeSetFilter filter, DataProcessorChangeSetSource source)
//        {
//            filter.RegisterSource(filter);
//            return filter;
//        }

//        /// <summary>
//        /// Links the given source to the target.
//        /// </summary>
//        /// <param name="dataProcessorChangeSetSource"></param>
//        /// <param name="dataProcessorChangeSetTarget"></param>
//        internal static void LinkChangeSourceToTarget(
//            DataProcessorChangeSetSource source,
//            DataProcessorChangeSetTarget target)
//        {
//            target.RegisterSource(source);
//        }

//        /// <summary>
//        /// Creates a new xml changeset source.
//        /// </summary>
//        /// <param name="p"></param>
//        /// <returns></returns>
//        internal static DataProcessorChangeSetSource CreateChangeXmlSource(string file_name)
//        {
//            return null;
//        }

//        internal static DataProcessorChangeSetTarget CreateChangeXmlTarget(string p)
//        {
//            return null;
//        }

//        internal static DataProcessorChangeSetFilter CreateChangeBoundingBoxFilter(double p, double p_2, double p_3, double p_4)
//        {
//            return null;
//        }

//        #endregion

//        internal static DataProcessorChangeSetTarget CreateChangeApplyOracleTarget(string connection_string)
//        {
//            return new OracleSimpleChangeSetApplyTarget(StripQuotes(connection_string),true);
//        }
//    }
//}
