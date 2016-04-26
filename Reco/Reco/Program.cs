using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Reco.Model;

namespace Reco
{
    internal class Program
    {

        private const int userCount = 100;
        private const int productCount = 300;

        private static void Main(string[] args)
        {
            var repo = new Repository();
            // params describe generated dataset
            var generator = new Generation(100, 300, 30, 27);
            var evaluator = new Evaluation();
            var propagator = new Propagation();
            var predictor = new Prediction();
            var similator = new Similarities(0.3);
            //generate simple graph and evaluate
            //BASE
            // generator.GenerateGraph();
            //evaluator.EvaluateDataset("BASE");
            //predictor.GeneratePredictions("BASE");
            //evaluator.EvaluateResults("Simple dataset", Algorithm.Base);
            //BASE + Domain similarity pearson
            // similator.EvaluateDomainTrust("PBASE");
            // predictor.GeneratePredictions("PBASE");
            // evaluator.EvaluateResults("Simple dataset domain similarities", Algorithm.PBase);
            // 
            // //Multiplication
            // propagator.GeneratePropagatedTrust(Algorithm.Multiplication, 0.9);
            // evaluator.EvaluateDataset("Multiplication");
            // predictor.GeneratePredictions("Multiplication");
            // evaluator.EvaluateResults("Propagation SHORTMULTIPLICATION", Algorithm.Multiplication);
            // //Multiplication + Domain Similarity
             similator.EvaluateDomainTrust("PMultiplication");
             predictor.GeneratePredictions("PMultiplication");
             evaluator.EvaluateResults("PMultiplication dataset domain similarities", Algorithm.PMultiplication);
            // //Multiplication + User Level domain similarity
            // similator.EvaluateUserLevelDomainTrust("PMultiplicationU", "Avg");
            // evaluator.EvaluateDataset("PMultiplicationU");
            // predictor.GeneratePredictions("PMultiplicationU");
            // evaluator.EvaluateResults("PMultiplicationU dataset domain similarities", Algorithm.PUMultiplication);

            // propagator.GeneratePropagatedTrust(Algorithm.ArithmeticMean, 0.9);
            // evaluator.EvaluateDataset("ArithmeticMean");
            // predictor.GeneratePredictions("ArithmeticMean");
            // evaluator.EvaluateResults("Propagation SHORTARITMETIC", Algorithm.ArithmeticMean);
            //similator.EvaluateDomainTrust("PArithmeticMean");
            //predictor.GeneratePredictions("PArithmeticMean");
            //evaluator.EvaluateResults("PArithmeticMean dataset domain similarities", Algorithm.PArithmeticMean);
            //
            // propagator.GeneratePropagatedTrust(Algorithm.HArmonicMean, 0.6);
            // evaluator.EvaluateDataset("HarmonicMean");
            // predictor.GeneratePredictions("HarmonicMean");
            // evaluator.EvaluateResults("Propagation SHORTHARMONIC", Algorithm.HArmonicMean);
            //similator.EvaluateDomainTrust("PHarmonicMean");
            //predictor.GeneratePredictions("PHarmonicMean");
            //evaluator.EvaluateResults("PHArmonicMean dataset domain similarities", Algorithm.PHArmonicMean);

            //var domainSimsBefore = EvaluateDomainSimilarities(repo);

            //GeneratePredictions(repo, 1);
            //var domainSimsAfter = EvaluateDomainSimilarities(repo);
         }

    }
}
