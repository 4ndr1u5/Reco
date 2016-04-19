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
    class Program
    {

        const int userCount = 100;
        const int productCount = 300;

        // rezultatu aprasymas
        // generuojamo dataseto charkateristikos - surinkti rankiniu budu
        // kateforiju panasumo matrica

        // Eval
        // MAE, MAUE, RMSE, RC, UC 
        // visiems useriams, saltiems useriams, useriam su dideliu std nuokrypiu

        //propaguoti datasetai - SHORTMULTI, SHORTARI, SHORTGEO - Eval kiekvienam

        // kategoriju panasumas - koreliacijos
        // kategoriju panasumas isskaiciuotas

        // propag + panasumas  - Eval


        //TODO: start -fix type-method naming
        static void Main(string[] args)
        {
            var repo = new Repository();
            // params describe generated dataset
            var generator = new Generation(100, 300, 30, 27);
            var evaluator = new Evaluation();
            var propagator = new Propagation();
            var predictor = new Prediction();
            //generate simple graph and evaluate

            //generator.GenerateGraph();
            //predictor.GeneratePredictions("BASE");
            evaluator.EvaluateResults("Simple dataset", Algorithm.Base);

            propagator.GeneratePropagatedTrust(Algorithm.Multiplication);
            predictor.GeneratePredictions("SHORTMULTIPLICATION");
            evaluator.EvaluateResults("Propagation SHORTMULTIPLICATION", Algorithm.Multiplication);

            propagator.GeneratePropagatedTrust(Algorithm.ArithmeticMean);
            predictor.GeneratePredictions("SHORTARITMETIC");
            evaluator.EvaluateResults("Propagation SHORTARITMETIC", Algorithm.ArithmeticMean);

            propagator.GeneratePropagatedTrust(Algorithm.HArmonicMean);
            predictor.GeneratePredictions("SHORTHARMONIC");
            evaluator.EvaluateResults("Propagation SHORTHARMONIC", Algorithm.HArmonicMean);

            var domainSimsBefore = EvaluateDomainSimilarities(repo);

            //GeneratePredictions(repo, 1);
            var domainSimsAfter = EvaluateDomainSimilarities(repo);
        }


        public static List<Tuple<int, int, double>> EvaluateDomainSimilarities(Repository repo)
        {
            var result = new List<Tuple<int, int, double>>();
            for (var c1 = 1; c1 <= 5; c1++)
            {
                for (var c2 = 1; c2 <= 5; c2++)
                {
                    if (c1 != c2)
                    {
                        var trustsForTwoCategories = repo.GetTrustsForTwoCategories(c1, c2);
                        var trust1 = trustsForTwoCategories.Select(x => x.Item1).ToList();
                        var trust2 = trustsForTwoCategories.Select(x => x.Item2).ToList();
                        var domsim = new Tuple<int, int, double>(c1, c2, Correlation.Pearson(trust1, trust2));
                        result.Add(domsim);
                    }
                }
            }
            return result;
        }




        //public static List<Tuple<User, Product>> GeneratePredictions(Repository repo, int step)
        //{
        //    var users = repo.getAllUsers();
        //    var ratingsEvaluated = new List<Tuple<User, Product>>();
        //    foreach (var u in users)
        //    {
        //        var ratings = repo.GetUsersRatings(u.iduser);
        //        foreach (var rat in ratings)
        //        {
        //            var prod = rat.Item2;
        //            //get trustees in that category who have rated this item (trust, rating)
        //            var trustRating = repo.GetTrusteesWhoHaveRatedThisProduct(u.iduser, prod.category, prod.idproduct);
        //            if (trustRating.Count > 0)
        //            {
        //                var predictedRating = Math.Round(Helpers.WeightedAverage(trustRating), 4);
        //                predictedRating = predictedRating > 5 ? 5 : predictedRating;
        //                predictedRating = predictedRating < 1 ? 1 : predictedRating;
        //                if(predictedRating>1 && predictedRating < 5)
        //                {
        //                    Console.WriteLine("For user {0} item {1} predicted {2}", u.iduser, prod.idproduct, predictedRating);
        //                    repo.CreatePredictedRating(u.iduser, prod.idproduct, predictedRating, step);
        //                    ratingsEvaluated.Add(new Tuple<User, Product>(u, prod));
        //                }
                        
        //            }

        //        }
                
        //        //get list of ratings and foreach

        //        //get trustees who have rated this item (trust, rating)
        //        //and calculate weighted average
        //    }
        //    return ratingsEvaluated;
        //}
     
    }
}
