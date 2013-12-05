// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System;
using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Structures;
using OsmSharp.Math.Structures.QTree;
using OsmSharp.Routing.Graph;

namespace OsmSharp.Routing.BasicRouter
{
    /// <summary>
    /// A router data source that uses a IDynamicGraph as it's main datasource.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class DynamicGraphRouterDataSource<TEdgeData> : IDynamicGraphRouterDataSource<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic graph.
        /// </summary>
        private readonly IDynamicGraph<TEdgeData> _graph;

        /// <summary>
        /// Holds the index of vertices per bounding box.
        /// </summary>
        private readonly ILocatedObjectIndex<GeoCoordinate, uint> _vertexIndex;

        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private readonly ITagsCollectionIndexReadonly _tagsIndex;

        /// <summary>
        /// Holds the supported vehicle profiles.
        /// </summary>
        private readonly HashSet<Vehicle> _supportedVehicles;

        /// <summary>
        /// Holds all vertice location.
        /// </summary>
        private readonly List<GeoCoordinate> _vertices;

        /// <summary>
        /// Holds all edge metadata.
        /// </summary>
        private readonly Dictionary<uint, List<KeyValuePair<uint, uint>>> _arcMetaData;

        /// <summary>
        /// Creates a new osm memory router data source.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public DynamicGraphRouterDataSource(ITagsCollectionIndexReadonly tagsIndex)
        {
            if (tagsIndex == null) throw new ArgumentNullException("tagsIndex");

            _graph = new MemoryDynamicGraph<TEdgeData>();
            _vertices = new List<GeoCoordinate>();
            _vertexIndex = new QuadTree<GeoCoordinate, uint>();
            _arcMetaData = new Dictionary<uint, List<KeyValuePair<uint, uint>>>();
            _tagsIndex = tagsIndex;

            _supportedVehicles = new HashSet<Vehicle>();
        }

        /// <summary>
        /// Returns true if the given vehicle profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(Vehicle vehicle)
        {
            return _supportedVehicles.Contains(vehicle); // for backwards compatibility.
        }

        /// <summary>
        /// Adds one more supported profile.
        /// </summary>
        /// <param name="vehicle"></param>
        public void AddSupportedProfile(Vehicle vehicle)
        {
            _supportedVehicles.Add(vehicle);
        }

        /// <summary>
        /// Returns all arcs inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>[] GetArcs(
            GeoCoordinateBox box)
        {
            // get all the vertices in the given box.
            IEnumerable<uint> vertices = _vertexIndex.GetInside(
                box);

            // loop over all vertices and get the arcs.
            var arcs = new List<KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>>();
            foreach (uint vertex in vertices)
            {
                KeyValuePair<uint, TEdgeData>[] localArcs = this.GetArcs(vertex);
                foreach (KeyValuePair<uint, TEdgeData> localArc in localArcs)
                {
                    arcs.Add(new KeyValuePair<uint, KeyValuePair<uint, TEdgeData>>(
                        vertex, localArc));
                }
            }
            return arcs.ToArray();
        }

        /// <summary>
        /// Adds a new vertex location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertexLocation(float latitude, float longitude)
        {
            uint id = (uint)_vertices.Count;
            _vertices.Add(new GeoCoordinate(latitude, longitude));
            return id;
        }

        /// <summary>
        /// Returns true if a given vertex is in the graph.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            latitude = 0;
            longitude = 0;
            if (_vertices.Count > id)
            {
                latitude = (float)_vertices[(int)id].Latitude;
                longitude = (float)_vertices[(int)id].Longitude;
            }
            return false;
        }

        /// <summary>
        /// Returns all arcs starting at a given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public KeyValuePair<uint, TEdgeData>[] GetArcs(uint vertexId)
        {
            return _graph.GetArcs(vertexId);
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasArc(uint vertexId, uint neighbour)
        {
            return _graph.HasArc(vertexId, neighbour);
        }

        /// <summary>
        /// Adds a new arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public void AddArc(uint from, uint to, TEdgeData data, IDynamicGraphEdgeComparer<TEdgeData> comparer)
        {
            _graph.AddArc(from, to, data, comparer);
        }

        /// <summary>
        /// Deletes an arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(uint from, uint to)
        {
            _graph.DeleteArc(from, to);
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndexReadonly TagsIndex
        {
            get
            {
                return _tagsIndex;
            }
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return (uint)_vertices.Count; }
        }

        /// <summary>
        /// Returns all vertices that have meta-data associated.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVerticesMeta()
        {
            return _arcMetaData.Keys;
        }

        /// <summary>
        /// Returns all available meta-data about arcs leaving the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public List<KeyValuePair<uint, uint>> GetArcsMeta(uint vertex)
        {
            List<KeyValuePair<uint, uint>> arcsMeta;
            if (_arcMetaData.TryGetValue(vertex, out arcsMeta))
            { // meta data found!
                return arcsMeta;
            }
            return null;
        }

        /// <summary>
        /// Adds an arc with meta-data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tags"></param>
        public void AddArcMeta(uint from, uint to, uint tags)
        {
            List<KeyValuePair<uint, uint>> arcsMeta;
            if (!_arcMetaData.TryGetValue(from, out arcsMeta))
            { // meta data found!
                arcsMeta = new List<KeyValuePair<uint, uint>>();
                _arcMetaData.Add(from, arcsMeta);
            }
            arcsMeta.Add(new KeyValuePair<uint, uint>(to, tags));
        }

        /// <summary>
        /// Detelets all meta-data related to the given arc.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArcMeta(uint from, uint to)
        {
            List<KeyValuePair<uint, uint>> arcsMeta;
            if (_arcMetaData.TryGetValue(from, out arcsMeta))
            { // meta data found!
                arcsMeta.RemoveAll(x => (x.Key == from && x.Value == to));

                if (arcsMeta.Count == 0)
                { // no more arcs left from the start vertex.
                    _arcMetaData.Remove(from);
                }
            }
        }

        #region Restriction

        /// <summary>
        /// Holds the restricted routes that apply to all vehicles.
        /// </summary>
        private Dictionary<uint, List<uint[]>> _restrictedRoutes;

        /// <summary>
        /// Holds the restricted routes that apply to one vehicle profile.
        /// </summary>
        private Dictionary<Vehicle, Dictionary<uint, List<uint[]>>> _restricedRoutesPerVehicle;

        /// <summary>
        /// Adds a restriction to this graph by prohibiting the given route.
        /// </summary>
        /// <param name="route"></param>
        public void AddRestriction(uint[] route)
        {
            if (route == null) { throw new ArgumentNullException(); }
            if (route.Length == 0) { throw new ArgumentOutOfRangeException("Restricted route has to contain at least one vertex."); }

            if (_restrictedRoutes == null)
            { // create dictionary.
                _restrictedRoutes = new Dictionary<uint, List<uint[]>>();
            }
            List<uint[]> routes;
            if (!_restrictedRoutes.TryGetValue(route[0], out routes))
            {
                routes = new List<uint[]>();
                _restrictedRoutes.Add(route[0], routes);
            }
            routes.Add(route);
        }

        /// <summary>
        /// Adds a restriction to this graph by prohibiting the given route for the given vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="route"></param>
        public void AddRestriction(Vehicle vehicle, uint[] route)
        {
            if (route == null) { throw new ArgumentNullException(); }
            if (route.Length == 0) { throw new ArgumentOutOfRangeException("Restricted route has to contain at least one vertex."); }

            if (_restricedRoutesPerVehicle == null)
            { // create dictionary.
                _restricedRoutesPerVehicle = new Dictionary<Vehicle, Dictionary<uint, List<uint[]>>>();
            }
            Dictionary<uint, List<uint[]>> restrictedRoutes;
            if (!_restricedRoutesPerVehicle.TryGetValue(vehicle, out restrictedRoutes))
            { // the vehicle does not have any restrictions yet.
                restrictedRoutes = new Dictionary<uint, List<uint[]>>();
                _restricedRoutesPerVehicle.Add(vehicle, restrictedRoutes);
            }
            List<uint[]> routes;
            if (!restrictedRoutes.TryGetValue(route[0], out routes))
            {
                routes = new List<uint[]>();
                restrictedRoutes.Add(route[0], routes);
            }
            routes.Add(route);
        }

        /// <summary>
        /// Returns all restricted routes that start in the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            Dictionary<uint, List<uint[]>> restrictedRoutes;
            routes = null;
            return _restricedRoutesPerVehicle.TryGetValue(vehicle, out restrictedRoutes) &&
                restrictedRoutes.TryGetValue(vertex, out routes);
        }

        /// <summary>
        /// Returns true if there is a restriction that ends with the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            routes = null;
            return false;
        }

        #endregion
    }
}