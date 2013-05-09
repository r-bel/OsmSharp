﻿// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using System.Reflection;
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Osm.Data.Xml.Processor;

namespace OsmSharp.UnitTests.Data.Processing
{
    /// <summary>
    /// Summary description for XmlDataProcessorSourceTests
    /// </summary>
    [TestFixture]
    public class XmlDataProcessorSourceTests
    {
        /// <summary>
        /// A regression test in resetting and XML data source.
        /// </summary>
        [Test]
        public void XmlDataProcessorSourceReset()
        {
            // generate the source.
            var source = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.UnitTests.api.osm"));

            // pull the data out.
            var target = new OsmStreamWriterEmpty();
            target.RegisterSource(source);
            target.Pull();

            // reset the source.
            if (source.CanReset)
            {
                source.Reset();

                // pull the data again.
                target.Pull();
            }
        }
    }
}