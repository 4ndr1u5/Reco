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
            // evaluator.EvaluateDataset("BASE");
            //predictor.GeneratePredictions("BASE");
            //evaluator.EvaluateResults("Simple dataset", Algorithm.Base);
            //BASE + Domain similarity pearson
             similator.EvaluateDomainTrust("PBASE");
            similator.EvaluateUserLevelDomainTrust("PBASE", "Avg");
            predictor.GeneratePredictions("PBASE");
             evaluator.EvaluateResults("Simple dataset domain similarities", Algorithm.PBase);
            // 
            // //Multiplication
            // propagator.GeneratePropagatedTrust(Algorithm.Multiplication, 0.9);
            // evaluator.EvaluateDataset("Multiplication");
            // predictor.GeneratePredictions("Multiplication");
            // evaluator.EvaluateResults("Propagation SHORTMULTIPLICATION", Algorithm.Multiplication);
            // //Multiplication + Domain Similarity
             //similator.EvaluateDomainTrust("PMultiplication");
             //predictor.GeneratePredictions("PMultiplication");
             //evaluator.EvaluateResults("PMultiplication dataset domain similarities", Algorithm.PMultiplication);
            // //Multiplication + User Level domain similarity
             //similator.EvaluateUserLevelDomainTrust("PMultiplicationU", "Avg");
             //evaluator.EvaluateDataset("PMultiplicationU");
             //predictor.GeneratePredictions("PMultiplicationU");
             //evaluator.EvaluateResults("PMultiplicationU dataset domain similarities", Algorithm.PUMultiplication);

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


            //COMMON NEIGHBORS
            //create model
            // get all user pairs, evaluate their trust and jaccard quotient
            // generate linreg prediction model for data
            // evaluate new trusts for all users with know jaccard quotient
            //var threshold = 0;
            //var users = repo.getAllUsers();
            //var trainingDataTrust = new List<double>();
            //var trainingDataJaccard = new List<double>();
            //var categories = new List<int>() {1,2,3,4,5};
            ////foreach (var cat in categories)
            ////{
            //    foreach (var user1 in users)
            //    {
            //        foreach (var user2 in users.Where(x => x.iduser != user1.iduser))
            //        {
            //            var trust = repo.GetTrust(user1.iduser, user2.iduser);
            //            var jaccard = 0d;
            //            if (trust != null)
            //            {
            //                jaccard = repo.GetJaccard(user1.iduser, user2.iduser);
            //                if (jaccard > threshold)
            //                {
            //                    trainingDataTrust.Add(trust.TrustValue);
            //                    trainingDataJaccard.Add(jaccard);
            //                }
            //            }

            //        }
            //    }
            //    var linreg = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(trainingDataJaccard.ToArray(), trainingDataTrust.ToArray());
            //    foreach (var user1 in users)
            //    {
            //        foreach (var user2 in users.Where(x => x.iduser != user1.iduser))
            //        {

            //            var jaccard = repo.GetJaccard(user1.iduser, user2.iduser);
            //            var predictedTrust = linreg.Item1 * jaccard + linreg.Item2;
            //            repo.CreateTrust(user1.iduser, user2.iduser, 1, predictedTrust, "CN");
            //        }
            //    }
            ////}
            
            //predictor.GeneratePredictions("CN");
            //evaluator.EvaluateResults("Common neighbours", Algorithm.CN);
        }

    }
}
