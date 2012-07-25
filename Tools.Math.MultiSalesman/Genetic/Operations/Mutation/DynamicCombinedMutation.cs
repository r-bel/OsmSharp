﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.Random;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.MultiSalesman.Genetic;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class DynamicCombinedMutation : CombinedMutation<Genome, Problem, Fitness>
    {
        private IList<double> _initial;
        private IList<double> _increase_rounds;
        private IList<double> _decrease_rounds;
        private IList<double> _decrease_time;

        private bool _regime_reached = false;

        public DynamicCombinedMutation(
            IList<IMutationOperation<Genome,Problem,Fitness>> operations,
            IList<double> initial,
            IList<double> increase_rounds,
            IList<double> decrease_rounds,
            IList<double> decrease_time)
            :base(StaticRandomGenerator.Get(),
            operations,
            initial)
        {
            _initial = initial;
            _decrease_rounds = decrease_rounds;
            _increase_rounds = increase_rounds;
            _decrease_time = decrease_time;
        }

        public override Individual<Genome,Problem,Fitness> Mutate(
            Solver<Genome,Problem,Fitness> solver, Individual<Genome,Problem,Fitness> mutating)
        {
            // determine correct probalities.
            if (mutating.Fitness.Feasable)
            { // decrease total time.
                if (_regime_reached == false)
                {
                    Tools.Core.Output.OutputTextStreamHost.WriteLine(
                        "Regime Reached!");
                }
                //Tools.Core.Output.OutputTextStreamHost.Write("DT");
                this.Probabilities = _decrease_time;
                _regime_reached = true;
                if (solver.Fittest.Fitness.LargestRoundCategory == 0
                    || solver.Fittest.Fitness.SmallestRoundCategory == 0)
                {
                    if (solver.Fittest.Fitness.LargestRoundCategory >
                        solver.Fittest.Fitness.SmallestRoundCategory)
                    { // reduce the number of rounds.
                        this.Probabilities = _decrease_rounds;
                    }
                    else
                    { // increase the number of rounds.
                        this.Probabilities = _increase_rounds;
                    }
                }
            }
            else
            {
                //Tools.Core.Output.OutputTextStreamHost.Write("N");
                this.Probabilities = _initial;
            }

            // actually select one.
 	        Individual<Genome,Problem,Fitness> result = base.Mutate(solver, mutating);
            result.Validate(solver.Problem);
            return result;
        }
    }
}
