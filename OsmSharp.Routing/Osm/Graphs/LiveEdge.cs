using OsmSharp.Routing.Graph;
using OsmSharp.Routing.BasicRouter.Dykstra;

namespace OsmSharp.Routing.Osm.Graphs
{
    /// <summary>
    /// A simple edge containing the orignal OSM-tags and a flag indicating the direction of this edge relative to the 
    /// OSM-direction.
    /// </summary>
    public class LiveEdge : IDykstraEdge
    {
        /// <summary>
        /// Flag indicating if this is a forward or backward edge relative to the tag descriptions.
        /// </summary>
        public bool Forward 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The properties of this edge.
        /// </summary>
        public uint Tags
        {
            get;
            set;
        }

        /// <summary>
        /// Returns true if this edge represents a neighbour-relation.
        /// </summary>
        public bool RepresentsNeighbourRelations
        {
            get { return true; }
        }
    }
}