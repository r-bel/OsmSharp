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

using OsmSharp.Routing.Graph;
using System.Collections.Generic;
namespace OsmSharp.Routing.BasicRouter
{
    /// <summary>
    /// Abstracts a writable data source for a basic router.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public interface IBasicRouterDataSource<TEdgeData> : IDynamicGraph<TEdgeData>, IBasicRouterDataSourceReadOnly<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Adds the given vehicle as a supported vehicle profile.
        /// </summary>
        /// <param name="vehicle"></param>
        void AddSupportedProfile(Vehicle vehicle);

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        uint AddVertexLocation(float latitude, float longitude);

        /// <summary>
        /// Returns all vertices that have meta-data associated.
        /// </summary>
        /// <returns></returns>
        IEnumerable<uint> GetVerticesMeta();

        /// <summary>
        /// Adds arc meta data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="tags"></param>
        void AddArcMeta(uint from, uint to, uint tags);
    }
}