﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo;
using Osm.Core;
using Osm.Core.Factory;
using Osm.Core.Sources;
using System.Globalization;

namespace Osm.Core.Xml.v0_6
{
    public static class Extensions
    {
        #region Xml <-> Osm.Core Conversions

        #region Convert From Xml

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bounds bounds)
        {
            if (bounds != null)
            {
                return new GeoCoordinateBox(
                    new GeoCoordinate((double)bounds.maxlat, (double)bounds.maxlon),
                    new GeoCoordinate((double)bounds.minlat, (double)bounds.minlon));
            }
            return null;
        }

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bound bound)
        {
            if (bound != null)
            {
                string[] bounds = bound.box.Split(',');
                return new GeoCoordinateBox(
                    new GeoCoordinate(double.Parse(bounds[0], CultureInfo.InstalledUICulture), 
                        double.Parse(bounds[1], CultureInfo.InstalledUICulture)),
                    new GeoCoordinate(double.Parse(bounds[2], CultureInfo.InstalledUICulture),
                        double.Parse(bounds[3], CultureInfo.InstalledUICulture)));
            }
            return null;
        }

        /// <summary>
        /// Converts an Xml relation to an Osm domain model relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Relation ConvertFrom(this relation xml_obj, INodeSource node_source, IWaySource way_source, IRelationSource relation_source)
        {
            // create a new node and immidiately set the id.
            Relation new_obj = OsmBaseFactory.CreateRelation(xml_obj.id);

            // set the members
            for (int idx = 0; idx < xml_obj.member.Length; idx++)
            {
                member member = xml_obj.member[idx];
                if (member.refSpecified && member.typeSpecified)
                {
                    switch (member.type)
                    {
                        case memberType.node:
                            long node_id = member.@ref;
                            Node member_node = node_source.GetNode(node_id);
                            if (member_node != null)
                            {
                                RelationMember node_member = new RelationMember();
                                node_member.Member = member_node;
                                node_member.Role = member.role;
                                new_obj.Members.Add(node_member);
                            }
                            else
                            { // relation cannot be converted; node was not found!
                                return null;
                            }
                            break;
                        case memberType.relation:
                            long relation_id = member.@ref;
                            Relation member_relation = relation_source.GetRelation(relation_id);
                            if (member_relation != null)
                            {
                                RelationMember relation_member = new RelationMember();
                                relation_member.Member = member_relation;
                                relation_member.Role = member.role;
                                new_obj.Members.Add(relation_member);
                            }
                            else
                            { // relation cannot be converted; relation was not found!
                                return null;
                            }
                            break;
                        case memberType.way:
                            long way_id = member.@ref;
                            Way member_way = way_source.GetWay(way_id);
                            if (member_way != null)
                            {
                                RelationMember way_member = new RelationMember();
                                way_member.Member = member_way;
                                way_member.Role = member.role;
                                new_obj.Members.Add(way_member);
                            }
                            else
                            { // relation cannot be converted; way was not found!
                                return null;
                            }
                            break;
                    }
                }
                else
                { // way cannot be converted; member could not be created!
                    return null;
                }
            }

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Osm.Core.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = xml_obj.uid;
            }
            new_obj.User = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;

        }

        /// <summary>
        /// Converts an Xml way to an Osm domain model way.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Way ConvertFrom(this way xml_obj, INodeSource node_source)
        {
            // create a new node and immidiately set the id.
            Way new_obj = OsmBaseFactory.CreateWay((int)xml_obj.id);

            // set the nodes
            for (int idx = 0; idx < xml_obj.nd.Length;idx++)
            {
                nd node = xml_obj.nd[idx];
                if (node.refSpecified)
                {
                    int node_id = (int)node.@ref;
                    Node child_node = node_source.GetNode(node_id);
                    if (child_node != null)
                    {
                        new_obj.Nodes.Add(child_node);
                    }
                    else
                    { // way cannot be converted; node was not found!
                        return null;
                    }
                }
                else
                { // way cannot be converted; node was not found!
                    return null;
                }
            }

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Osm.Core.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = (int)xml_obj.uid;
            }
            new_obj.User = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = (int)xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;
        }

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Node ConvertFrom(this node xml_obj)
        {
            // create a new node an immidiately set the id.
            Node new_obj = OsmBaseFactory.CreateNode((int)xml_obj.id);

            // set the long- and latitude
            if (!xml_obj.latSpecified || !xml_obj.lonSpecified)
            {
                throw new ArgumentNullException("Latitude and/or longitude cannot be null!");
            }
            new_obj.Coordinate = new GeoCoordinate((double)xml_obj.lat, (double)xml_obj.lon);

            // set the tags.
            if (xml_obj.tag != null)
            {
                foreach (Osm.Core.Xml.v0_6.tag tag in xml_obj.tag)
                {
                    new_obj.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xml_obj.uidSpecified)
            {
                new_obj.UserId = (int)xml_obj.uid;
            }
            new_obj.User = xml_obj.user;

            // set the changeset info.
            if (xml_obj.changesetSpecified)
            {
                new_obj.ChangeSetId = (int)xml_obj.changeset;
            }

            // set the timestamp flag.
            if (xml_obj.timestampSpecified)
            {
                new_obj.TimeStamp = xml_obj.timestamp;
            }

            // set the visible flag.
            new_obj.Visible = xml_obj.visible;
            return new_obj;
        }

        #endregion

        #region Convert To Xml

        /// <summary>
        /// Extensions method for a geocoordinatebox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bounds ConvertTo(this GeoCoordinateBox box)
        {
            bounds b = new bounds();

            b.maxlat = box.MaxLat;
            b.maxlatSpecified = true;
            b.maxlon = box.MaxLon;
            b.maxlonSpecified = true;
            b.minlat = box.MinLat;
            b.minlatSpecified = true;
            b.minlon = box.MinLon;
            b.minlonSpecified = true;

            return b;
        }

        /// <summary>
        /// Converts an domain model changeset to and Xml changeset.
        /// </summary>
        /// <param name="changeset"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static changeset ConvertTo(this ChangeSet changeset)
        {
            return null;
        }

        /// <summary>
        /// Converts a domain model relation to an Xml relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static relation ConvertTo(this Relation dom_obj)
        {
            relation xml_obj = new relation();

            // set the changeset.
            if (dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            xml_obj.id = dom_obj.Id;
            xml_obj.idSpecified = true;

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                IList<KeyValuePair<string, string>> tags_list = dom_obj.Tags.ToList<KeyValuePair<string, string>>();
                for (int idx = 0; idx < tags_list.Count; idx++)
                {
                    KeyValuePair<string, string> tag_pair = tags_list[idx];
                    tag t = new tag();
                    t.k = tag_pair.Key;
                    t.v = tag_pair.Value;
                    xml_obj.tag[idx] = t;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            xml_obj.visible = dom_obj.Visible;
            xml_obj.visibleSpecified = true;

            // set the way-specific properties.
            xml_obj.member = new member[dom_obj.Members.Count];
            for (int idx = 0; idx < dom_obj.Members.Count; idx++)
            {
                RelationMember dom_member = dom_obj.Members[idx];
                member m = new member();
                
                switch(dom_member.Member.Type)
                {
                    case OsmType.Node:
                        m.type = memberType.node;
                        m.typeSpecified = true;
                        break;
                    case OsmType.Relation:
                        m.type = memberType.relation;
                        m.typeSpecified = true;
                        break;
                    case OsmType.Way:
                        m.type = memberType.way;
                        m.typeSpecified = true;
                        break;
                }

                m.@ref = dom_member.Member.Id;
                m.refSpecified = true;
                m.role = dom_member.Role;

                xml_obj.member[idx] = m;
            }

            return xml_obj;
        }

        /// <summary>
        /// Converts a domain model way to an Xml way.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static way ConvertTo(this Way dom_obj)
        {
            way xml_obj = new way();

            // set the changeset.
            if (dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            xml_obj.id = dom_obj.Id;
            xml_obj.idSpecified = true;

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                IList<KeyValuePair<string, string>> tags_list = dom_obj.Tags.ToList<KeyValuePair<string, string>>();
                for (int idx = 0; idx < tags_list.Count; idx++)
                {
                    KeyValuePair<string, string> tag_pair = tags_list[idx];
                    tag t = new tag();
                    t.k = tag_pair.Key;
                    t.v = tag_pair.Value;
                    xml_obj.tag[idx] = t;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            xml_obj.visible = dom_obj.Visible;
            xml_obj.visibleSpecified = true;

            // set the way-specific properties.
            xml_obj.nd = new nd[dom_obj.Nodes.Count];
            for(int idx = 0;idx < dom_obj.Nodes.Count;idx++)
            {
                nd n = new nd();
                n.@ref =  dom_obj.Nodes[idx].Id;
                n.refSpecified = true;
                xml_obj.nd[idx] = n;
            }

            return xml_obj;
        }

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static node ConvertTo(this Node dom_obj)
        {
            node xml_obj = new node();
            
            // set the changeset.
            if(dom_obj.ChangeSetId.HasValue)
            {
                xml_obj.changeset = dom_obj.ChangeSetId.Value;
                xml_obj.changesetSpecified = true;
            }

            // set the id.
            xml_obj.id = dom_obj.Id;
            xml_obj.idSpecified = true;

            if (dom_obj.Tags != null)
            {
                xml_obj.tag = new tag[dom_obj.Tags.Count];
                IList<KeyValuePair<string, string>> tags_list = dom_obj.Tags.ToList<KeyValuePair<string, string>>();
                for(int idx = 0;idx < tags_list.Count;idx++)
                {
                    KeyValuePair<string, string> tag_pair = tags_list[idx];
                    tag t = new tag();
                    t.k = tag_pair.Key;
                    t.v = tag_pair.Value;
                    xml_obj.tag[idx] = t;
                }
            }

            // set the timestamp.
            if (dom_obj.TimeStamp.HasValue)
            {
                xml_obj.timestamp = dom_obj.TimeStamp.Value;
                xml_obj.timestampSpecified = true;
            }

            // set the user data.
            if (dom_obj.UserId.HasValue)
            {
                xml_obj.uid = dom_obj.UserId.Value;
                xml_obj.uidSpecified = true;
            }
            xml_obj.user = xml_obj.user;

            // set the version.
            if (dom_obj.Version.HasValue)
            {
                xml_obj.version = (ulong)dom_obj.Version.Value;
                xml_obj.versionSpecified = true;
            }

            // set the visible.
            xml_obj.visible = dom_obj.Visible;
            xml_obj.visibleSpecified = true;

            // set the node-specific properties.
            xml_obj.lat = dom_obj.Coordinate.Latitude;
            xml_obj.latSpecified = true;
            xml_obj.lon = dom_obj.Coordinate.Longitude;
            xml_obj.lonSpecified = true;

            return xml_obj;
        }

        #endregion

        #endregion


    }
}
