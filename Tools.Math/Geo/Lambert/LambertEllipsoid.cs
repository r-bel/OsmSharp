﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Geo.Lambert.Ellipsoids;

namespace Tools.Math.Geo.Lambert
{
    /// <summary>
    /// Represents an ellipsoid used with the lamber projection.
    /// </summary>
    public abstract class LambertEllipsoid
    {
        /// <summary>
        /// The distance from the center of the ellipsoid to one of the focus points.
        /// </summary>
        private double _semi_major_axis;

        /// <summary>
        /// The flattening of this ellipsoid.
        /// </summary>
        private double _flattening;

        /// <summary>
        /// The eccentricity of this ellipsoid, calculated upon creation.
        /// </summary>
        private double _eccentricity;

        /// <summary>
        /// Creates a new ellipsoid.
        /// </summary>
        /// <param name="semi_major_axis"></param>
        /// <param name="flattening"></param>
        protected LambertEllipsoid(double semi_major_axis,
            double flattening)
        {
            _semi_major_axis = semi_major_axis;
            _flattening = flattening;

            // calculate eccentricity.
            _eccentricity = System.Math.Sqrt(_flattening * (2 - _flattening));
        }

        #region Properties

        /// <summary>
        /// Returns the calculated eccentricity of this ellipsoid.
        /// </summary>
        public double Eccentricity
        {
            get
            {
                return _eccentricity;
            }
        }

        /// <summary>
        /// Returns the semi major axis size.
        /// </summary>
        public double SemiMajorAxis
        {
            get
            {
                return _semi_major_axis;
            }
        }

        /// <summary>
        /// Returns the flattening of this ellipsoid.
        /// </summary>
        public double Flattening
        {
            get
            {
                return _flattening;
            }
        }

        #endregion

        #region Static Default Ellipsoids

        #region Hayford 1924

        private static Hayford1924Ellipsoid _hayford_1924_ellipsoid;

        /// <summary>
        /// Returns the hayford 1924 ellisoid.
        /// </summary>
        public static Hayford1924Ellipsoid Hayford1924Ellipsoid
        {
            get
            {
                if (_hayford_1924_ellipsoid == null)
                {
                    _hayford_1924_ellipsoid = new Hayford1924Ellipsoid();
                }
                return _hayford_1924_ellipsoid;
            }
        }

        #endregion

        #region Wgs 1984

        private static Wgs1984Ellipsoid _wgs_1984_ellipsoid;

        /// <summary>
        /// Returns the wgs 1984 ellisoid.
        /// </summary>
        public static Wgs1984Ellipsoid Wgs1984Ellipsoid
        {
            get
            {
                if (_wgs_1984_ellipsoid == null)
                {
                    _wgs_1984_ellipsoid = new Wgs1984Ellipsoid();
                }
                return _wgs_1984_ellipsoid;
            }
        }

        #endregion

        #endregion
    }
}
