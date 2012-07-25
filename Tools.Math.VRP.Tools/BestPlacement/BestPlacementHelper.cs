﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes;

namespace Tools.Math.VRP.Core.BestPlacement
{
    /// <summary>
    /// Implements some generic functions for best-placement.
    /// </summary>
    public class BestPlacementHelper
    {
        /// <summary>
        /// The minimum difference allowed.
        /// </summary>
        private static double _epsilon = 0;

        /// <summary>
        /// Calculates the best possible best-placement result for each customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static float[] CalculateBestValues(
            IProblemWeights problem,
            IEnumerable<int> customers)
        {
            float[][] weights = problem.WeightMatrix;

            float[] solutions = new float[customers.Count<int>()];
            int idx = 0;
            foreach (int customer in customers)
            {
                float solution = float.MaxValue;
                foreach (int first in customers)
                {
                    if (first != customer)
                    {
                        foreach (int second in customers)
                        {
                            if (second != customer)
                            {
                                float new_solution = weights[first][customer] + weights[customer][second];
                                if (new_solution < solution)
                                {
                                    solution = new_solution;
                                }

                                if (solution <= 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (solution <= 0)
                        {
                            break;
                        }
                    }
                }

                solutions[idx] = solution;
                idx++;
            }

            return solutions;
        }

        ///// <summary>
        ///// Returns the customer that least increases the length of the given route.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="route"></param>
        ///// <param name="customers"></param>
        ///// <returns></returns>
        //public static BestPlacementResult CalculateBestPlacement(
        //    IProblemWeights problem,
        //    IRoute route,
        //    IEnumerable<int> customers)
        //{
        //    return BestPlacementHelper.CalculateBestPlacement(problem, route, customers, null);
        //}

        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            IEnumerable<int> customers)
            //float[] solutions)
        {  // initialize the best placement result.
            BestPlacementResult best = new BestPlacementResult();
            best.Increase = float.MaxValue;
            if (!route.IsEmpty)
            {
                foreach (int customer in customers)
                {
                    BestPlacementResult result = BestPlacementHelper.CalculateBestPlacement(
                        problem, route, customer);
                    if (result.Increase < best.Increase)
                    {
                        best = result;

                        if (result.Increase < _epsilon)
                        {
                            break;
                        }
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return best;
        }

        //public static BestPlacementResult CalculateBestPlacement(
        //    IProblemWeights problem,
        //    IRoute route,
        //    int customer)
        //{
        //    return BestPlacementHelper.CalculateBestPlacement(problem, route, customer, null);
        //}

        /// <summary>
        /// Searches for the best place to insert the given customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int customer)
            //float[] solutions)
        {  // initialize the best placement result.
            float[][] weights = problem.WeightMatrix;

            BestPlacementResult result
                = new BestPlacementResult();
            result.Customer = customer;
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = float.MaxValue;

            float difference = float.MaxValue;
            if (!route.IsEmpty)
            {
                float new_weight = float.MaxValue;
                float old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // the previous customer exists.
                        // only if the previous is known.

                        // calculate the old weights.
                        //old_weight = problem.Weight(previous, current);
                        old_weight = weights[previous][current];

                        // calculate the new weights.
                        //new_weight = problem.Weight(previous, customer);
                        new_weight = weights[previous][customer];

                        //new_weight = new_weight +
                        //    problem.Weight(customer, current);
                        new_weight = new_weight +
                            weights[customer][current];

                        // calculate the difference.
                        difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;

                            // if the difference is equal to or smaller than epsilon we have search enough.
                            if (difference <= _epsilon)
                            {
                                // result is the best we will be able to get.
                                return result;
                            }
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // set the pervious to the last.
                previous = route.Last;

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    //new_weight = problem.Weight(previous, customer)
                    //        + (problem.Weight(customer, first));
                    new_weight = weights[previous][customer]
                        + weights[customer][first];

                    // calculate the old weights.
                    //old_weight = problem.Weight(previous, first);
                    old_weight = weights[previous][first];

                    // calculate the difference.
                    difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerAfter = first;
                        result.CustomerBefore = previous;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }

        /// <summary>
        /// Searches for the best place to insert the given two customers abstracting the distance between them.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="genomes"></param>
        /// <param name="city_to_place"></param>
        /// <returns></returns>
        public static BestPlacementResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int from,
            int to)
        {  // initialize the best placement result.
            BestPlacementResult result
                = new BestPlacementResult();
            result.Customer = -1; // this property is useless here!
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = float.MaxValue;

            if (!route.IsEmpty)
            {
                float new_weight = float.MaxValue;
                float old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // only if the previous is known.
                        // calculate the new weights.
                        new_weight = problem.Weight(previous, from)
                                + (problem.Weight(to, current));

                        // calculate the old weights.
                        old_weight = problem.Weight(previous, current);

                        // calculate the difference.
                        float difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    new_weight = problem.Weight(previous, from)
                            + (problem.Weight(to, first));

                    // calculate the old weights.
                    old_weight = problem.Weight(previous, first);

                    // calculate the difference.
                    float difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerAfter = previous;
                        result.CustomerBefore = first;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }
    }
}
