﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Tools.Xml.Gpx
{
    /// <summary>
    /// Represents a gpx document.
    /// </summary>
    public class GpxDocument
    {
        /// <summary>
        /// The actual gpx object.
        /// </summary>
        private object _gpx_object;

        /// <summary>
        /// The xml source this documents comes from.
        /// </summary>
        private IXmlSource _source;

        /// <summary>
        /// Returns the gpx version.
        /// </summary>
        private GpxVersion _version;

        /// <summary>
        /// Creates a new kml document based on an xml source.
        /// </summary>
        /// <param name="source"></param>
        public GpxDocument(IXmlSource source)
        {
            _source = source;
            _version = GpxVersion.Unknown;
        }


        /// <summary>
        /// Returns the name of this document.
        /// </summary>
        public string Name
        {
            get
            {
                return _source.Name;
            }
        }

        /// <summary>
        /// Returns the readonly flag.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _source.IsReadOnly;
            }
        }

        /// <summary>
        /// Returns the gpx version.
        /// </summary>
        public GpxVersion Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Gets/Sets the gpx object.
        /// </summary>
        public object Gpx
        {
            get
            {
                this.DoReadGpx();

                return _gpx_object;
            }
            set
            {
                _gpx_object = value;

                this.FindVersionFromObject();
            }
        }

        /// <summary>
        /// Saves this gpx back to it's source.
        /// </summary>
        public void Save()
        {
            this.DoWriteGpx();
        }

        #region Private Serialize/Desirialize functions

        private void FindVersionFromObject()
        {
            _version = GpxVersion.Unknown;
            if (_gpx_object is Tools.Xml.Gpx.v1_0.gpx)
            {
                _version = GpxVersion.Gpxv1_0;
            }
            else if (_gpx_object is Tools.Xml.Gpx.v1_1.gpxType)
            {
                _version = GpxVersion.Gpxv1_1;
            }
        }

        private void FindVersionFromSource()
        {
            // try to find the xmlns and the correct version to use.
            XmlReader reader = _source.GetReader();
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.Name == "gpx")
                {
                    string ns = reader.GetAttribute("xmlns");
                    switch (ns)
                    {
                        case "http://www.topografix.com/GPX/1/0":
                            _version = GpxVersion.Gpxv1_0;
                            break;
                        case "http://www.topografix.com/GPX/1/1":
                            _version = GpxVersion.Gpxv1_1;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    throw new XmlException("First element expected: gpx!");
                }

                // check end conditions.
                if (_version != GpxVersion.Unknown)
                {
                    reader.Close();
                    reader = null;
                    break;
                }

                reader.Read();
            }
        }

        private void DoReadGpx()
        {
            if (_gpx_object == null)
            {
                // find the version.
                Type version_type = null;

                // determine the version from source.
                this.FindVersionFromSource();
                switch (_version)
                {
                    case GpxVersion.Gpxv1_0:
                        version_type = typeof(Tools.Xml.Gpx.v1_0.gpx);
                        break;
                    case GpxVersion.Gpxv1_1:
                        version_type = typeof(Tools.Xml.Gpx.v1_1.gpxType);
                        break;
                    case GpxVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }

                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);

                XmlReader reader = _source.GetReader();
                _gpx_object = xmlSerializer.Deserialize(reader);

                reader.Close();
            }
        }

        private void DoWriteGpx()
        {
            if (_gpx_object != null)
            {                
                // find the version.
                Type version_type = null;

                // version should already be determined.
                switch (_version)
                {
                    case GpxVersion.Gpxv1_0:
                        version_type = typeof(Tools.Xml.Gpx.v1_0.gpx);
                        break;
                    case GpxVersion.Gpxv1_1:
                        version_type = typeof(Tools.Xml.Gpx.v1_1.gpxType);
                        break;
                    case GpxVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }
                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);

                XmlWriter writer = _source.GetWriter();
                xmlSerializer.Serialize(writer, _gpx_object);
                writer.Flush();
                writer.Close();

                xmlSerializer = null;
                writer = null;
            }
        }

        #endregion

        /// <summary>
        /// Closes this gpx document.
        /// </summary>
        public void Close()
        {

        }
    }

    /// <summary>
    /// The possible gpx document versions.
    /// </summary>
    public enum GpxVersion
    {
        Gpxv1_0,
        Gpxv1_1,
        Unknown
    }
}
