﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Filters;

namespace Osm.Interpreter.Features.Tree.Map.Physical.Land.ManMade.Parking
{
    
    public class ParkingFeature : Feature
    {
        /// <summary>
        /// Creates a new existence feature.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="meta"></param>
        private ParkingFeature(Filter filter, IDictionary<string, IList<string>> meta)
            : base("Map.ExistenceFeature.PhysicalFeature.LandFeature.ManMadeFeature.ParkingFeature", filter, meta)
        {

        }

        #region Singleton

        /// <summary>
        /// Holds the one and only instance of this feature.
        /// </summary>
        private static ParkingFeature _instance;

        /// <summary>
        /// Creates the existence feature.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="meta"></param>
        public static void Create(Filter filter, IDictionary<string, IList<string>> meta)
        {
            _instance = new ParkingFeature(filter, meta);
        }

        /// <summary>
        /// Returns the one and only instance of this feature.
        /// </summary>
        public static ParkingFeature Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Child Features

        /// <summary>
        /// Creates the list with child features.
        /// </summary>
        /// <returns></returns>
        internal override IList<Feature> GetChildFeatures()
        {
            IList<Feature> features = new List<Feature>();

            // TODO: Add features here!

            return features;
        }

        #endregion
    }
}
