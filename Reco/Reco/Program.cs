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

        static void Main(string[] args)
        {
            
            var repo = new Repository();
            //Tidal(repo);
            var reuseSameGraph = false;
            if (!reuseSameGraph)
            {

                // generate graph modes (step 1)
                GenerateGraph(repo, userCount, productCount);
                // generate actual ratings (true)
                GenerateRatings(repo, true, 30, 27);
                // generate hidden ratings for trust evaluation (false)
                //GenerateRatings(repo, false, 30, 0);
                GenerateTrust(repo);
                //generate predictions for rated items
                var usersProds1 = GeneratePredictions(repo, 0);

                //evaluate
                var ratingsForEval = repo.GetRatingsForEvaluation(0);
                var mae1 = Helpers.CalculateMAE(ratingsForEval);
                var maue1 = Helpers.CalculateMAUE(ratingsForEval);
                var rmse1 = Helpers.CalculateRMSE(ratingsForEval);
                // check domain similarities before propagation
                var domainSimsBefore = EvaluateDomainSimilarities(repo);
                //generate graph (step 2)
                GeneratePropagatedTrust(repo, false);
                //generate predictions
                var usersProds2 = GeneratePredictions(repo, 1);
                //evaluate
                ratingsForEval = repo.GetRatingsForEvaluation(1);
                var mae2 = Helpers.CalculateMAE(ratingsForEval);
                var maue2 = Helpers.CalculateMAUE(ratingsForEval);
                var rmse2 = Helpers.CalculateRMSE(ratingsForEval);
            }
           
            
            
            
            
            
            var domainSims = EvaluateDomainSimilarities(repo);
            //evauate MEA, RMSE
            //get rating relationships where predictedRating is not null
            var ratingsForEvaluation = repo.GetRatingsForEvaluation(1);
            var badPredictions = ratingsForEvaluation.Count(x => Helpers.Modulo(x.Item1, x.Item2) > 2);
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

        public static void Tidal(Repository repo)
        {
            var users = repo.getAllUsers();

            foreach (var u1 in users)
            {
                foreach (var u2 in users)
                {
                    if (u1 != u2)
                    {
                        for (var c = 1; c <= 5; c++)
                        {
                            var paths = repo.GetAllPaths(u1.iduser, u2.iduser, c);

                        }
                    }
                }
            }
        }

        public static void GeneratePropagatedTrust(Repository repo, bool multi)
        {
            var users = repo.getAllUsers();

            foreach (var u1 in users)
            {
                foreach (var u2 in users)
                {
                    if (u1 != u2)
                    {

                        try
                        {
                            for (var c = 1; c <= 5; c++)
                            {
                                var newTrust = 1.0;
                                var path = repo.GetShortestPaths(u1.iduser, u2.iduser, c).FirstOrDefault();

                                if (path != null && path.Relationships.Count() > 1)
                                {
                                    var list = new List<double>();
                                    foreach (var rel in path.Relationships)
                                    {
                                        if (multi)
                                        {
                                            newTrust = Math.Round(newTrust*rel.TrustValue, 4);
                                        }
                                        else
                                        {
                                            list.Add(rel.TrustValue);
                                        }
                                    }
                                    if (!multi)
                                    {
                                        newTrust = list.Average();
                                    }
                                    repo.SaveTrust(u1.iduser, u2.iduser, c, "ShortestPath", newTrust);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception");
                        }

                    }
                }
            }
        }

        public static List<Tuple<User, Product>> GeneratePredictions(Repository repo, int step)
        {
            var users = repo.getAllUsers();
            var ratingsEvaluated = new List<Tuple<User, Product>>();
            foreach (var u in users)
            {
                var ratings = repo.GetUsersRatings(u.iduser);
                foreach (var rat in ratings)
                {
                    var prod = rat.Item2;
                    //get trustees in that category who have rated this item (trust, rating)
                    var trustRating = repo.GetTrusteesWhoHaveRatedThisProduct(u.iduser, prod.category, prod.idproduct);
                    if (trustRating.Count > 0)
                    {
                        var predictedRating = Math.Round(Helpers.WeightedAverage(trustRating), 4);
                        predictedRating = predictedRating > 5 ? 5 : predictedRating;
                        predictedRating = predictedRating < 1 ? 1 : predictedRating;
                        if(predictedRating>1 && predictedRating < 5)
                        {
                            Console.WriteLine("For user {0} item {1} predicted {2}", u.iduser, prod.idproduct, predictedRating);
                            repo.CreatePredictedRating(u.iduser, prod.idproduct, predictedRating, step);
                            ratingsEvaluated.Add(new Tuple<User, Product>(u, prod));
                        }
                        
                    }

                }
                
                //get list of ratings and foreach

                //get trustees who have rated this item (trust, rating)
                //and calculate weighted average
            }
            return ratingsEvaluated;
        }
        private static void GenerateGraph(Repository repo, int userCount, int productCount)
        {
            repo.CreateUsers(userCount);
            repo.CreateProducts(productCount);
            //generate ratings
            //each user gives a random amount of ratings to random items (imporvement - bias towards what he likes!!)
            //var users = repo.getAllUsers();
            //var rndCat = new Random();
            //var rndCatNumber = new Random();
            //foreach (var u in users)
            //{
            //    var trustCount = (int)Normal.Sample(new Random(u.iduser), 10, 9);
            //    var ratingsCount = (int)Normal.Sample(new Random(u.iduser), 30, 27);
            //    //connect to users (create trust)

            //    var trustees = repo.PickRandomUsers(trustCount, userCount, u.iduser);
               

            //    double trust = 0;
                
            //    foreach (var trustee in trustees)
            //    {
            //        //tam kad vienas useris galetu tureti pasitikejima kitu keliose kateforijos
            //        var categories = new List<int>();
            //        var numberofcats = rndCatNumber.Next(0, 4);
            //        for (var i = 0; i <= numberofcats; i++)
            //        {
            //            var category = rndCat.Next(1, 6);
            //            categories.Add(category);
            //        }
            //        categories = categories.Distinct().ToList();
            //        foreach (var category in categories)
            //        {
            //            switch (category)
            //            {
            //                case 1:
            //                    trust = 1 - Helpers.Div(trustee.l1, u.l1);
            //                    break;
            //                case 2:
            //                    trust = 1 - Helpers.Div(trustee.l2, u.l2);
            //                    break;
            //                case 3:
            //                    trust = 1 - Helpers.Div(trustee.l3, u.l3);
            //                    break;
            //                case 4:
            //                    trust = 1 - Helpers.Div(trustee.l4, u.l4);
            //                    break;
            //                case 5:
            //                    trust = 1 - Helpers.Div(trustee.l5, u.l5);
            //                    break;
            //                default:
            //                    break;
            //            }
            //            repo.CreateTrust(u.iduser, trustee.iduser, category, Math.Round(trust, 4));
            //        }
                    


            //    }
                ////connect to products (generate ratings)
                //var prods = repo.PickRandomProducts(ratingsCount, productCount);
                //var rndU = new Random(u.iduser);
                //var rndCoef = new Random();
                //foreach (var prod in prods)
                //{
                //    var quotient = (int)Normal.Sample(rndU, 7, 4);
                //    //var rating = Math.Round(quotient * Math.Pow(u.l1 * prod.c1 + u.l2 * prod.c2 + u.l3 * prod.c3 + u.l4 * prod.c4 + u.l5 * prod.c5, 0.5));
                //    var userParams = new List<double>();
                //    userParams.Add(u.l1);
                //    userParams.Add(u.l2);
                //    userParams.Add(u.l3);
                //    userParams.Add(u.l4);
                //    userParams.Add(u.l5);

                //    var prodParams = new List<double>();
                //    prodParams.Add(prod.c1);
                //    prodParams.Add(prod.c2);
                //    prodParams.Add(prod.c3);
                //    prodParams.Add(prod.c4);
                //    prodParams.Add(prod.c5);
                //    var corr = Helpers.Positive(Correlation.Pearson(userParams, prodParams));
                //    //var rating = quotient * Math.Sqrt(corr) + 0.5 * u.quality * prod.quality;
                //    var coeff = rndCoef.NextDouble();

                //    var rating = 5*(coeff * Math.Sqrt(corr) + (1- coeff) * u.quality*prod.quality);
                //    rating = rating > 5 ? 5 : rating;
                //    rating = rating < 1 ? 1 : rating;
                //    repo.CreateRating(u.iduser, prod.idproduct, (int)Math.Round(rating));
                //}
                
            //}

            
        }

        private static void GenerateRatings(Repository repo, bool actual, int mean, int dev)
        {
            var users = repo.getAllUsers();
            foreach (var u in users)
            {
                var ratingsCount = (int) Normal.Sample(new Random(u.iduser), mean, dev);
                var prods = repo.PickRandomProducts(ratingsCount, productCount);
                var rndU = new Random(u.iduser);
                var rndCoef = new Random();
                foreach (var prod in prods)
                {
                    //var rating = Math.Round(quotient * Math.Pow(u.l1 * prod.c1 + u.l2 * prod.c2 + u.l3 * prod.c3 + u.l4 * prod.c4 + u.l5 * prod.c5, 0.5));
                    //var userParams = new List<double>();
                    //userParams.Add(u.l1);
                    //userParams.Add(u.l2);
                    //userParams.Add(u.l3);
                    //userParams.Add(u.l4);
                    //userParams.Add(u.l5);

                    //var prodParams = new List<double>();
                    //prodParams.Add(prod.c1);
                    //prodParams.Add(prod.c2);
                    //prodParams.Add(prod.c3);
                    //prodParams.Add(prod.c4);
                    //prodParams.Add(prod.c5);
                    //var corr = Helpers.Positive(Correlation.Pearson(userParams, prodParams));
                    ////var rating = quotient * Math.Sqrt(corr) + 0.5 * u.quality * prod.quality;
                    //var coeff = rndCoef.NextDouble();

                    //var rating = 5*(coeff*Math.Sqrt(corr) + (1 - coeff)*u.quality*prod.quality);
                    //rating = rating > 5 ? 5 : rating;
                    //rating = rating < 1 ? 1 : rating;
                    var rating = CreateRating(u, prod, rndCoef);
                    Console.WriteLine("User {0} rated item {1} by {2}", u.iduser, prod.idproduct, rating);
                    repo.CreateRating(u.iduser, prod.idproduct, (int) Math.Round(rating), actual);
                }
            }
        }

        public static double CreateRating(User u, Product prod, Random rndCoef)
        {
            var userParams = new List<double>();
            userParams.Add(u.l1);
            userParams.Add(u.l2);
            userParams.Add(u.l3);
            userParams.Add(u.l4);
            userParams.Add(u.l5);

            var prodParams = new List<double>();
            prodParams.Add(prod.c1);
            prodParams.Add(prod.c2);
            prodParams.Add(prod.c3);
            prodParams.Add(prod.c4);
            prodParams.Add(prod.c5);
            var corr = Helpers.Positive(Correlation.Pearson(userParams, prodParams));
            //var rating = quotient * Math.Sqrt(corr) + 0.5 * u.quality * prod.quality;
            var coeff = rndCoef.NextDouble();

            var rating = 5 * (coeff * Math.Sqrt(corr) + (1 - coeff) * u.quality * prod.quality);
            rating = rating > 5 ? 5 : rating;
            rating = rating < 1 ? 1 : rating;

            return rating;
        }

        private static void GenerateTrust(Repository repo)
        {
            var users = repo.getAllUsers();
            var rndCat = new Random();
            var rndCatNumber = new Random();
            foreach (var u in users)
            {
                var trustCount = (int) Normal.Sample(new Random(u.iduser), 3, 6);
                trustCount = trustCount < 0 ? 0 : trustCount;
                var trustees = repo.PickRandomUsers(trustCount, userCount, u.iduser);
                double trust = 0;

                foreach (var trustee in trustees)
                {
                    // pick n items in selected category and evaluate them
                    // get the ratings and calculate similarity
                    //then there is no need for simulated ratings


                    //tam kad vienas useris galetu tureti pasitikejima kitu keliose kateforijos
                    var categories = new List<int>();
                    var numberofcats = rndCatNumber.Next(0, 4);
                    for (var i = 0; i <= numberofcats; i++)
                    {
                        var category = rndCat.Next(1, 6);
                        categories.Add(category);
                    }
                    categories = categories.Distinct().ToList();
                    foreach (var category in categories)
                    {
                        // evaluare n items for user and trustee in this category
                        //pick random items
                        var items = repo.PickRandomProducts(12, productCount, category, rndCat);
                        if (items.Count() == 12)
                        {
                            //get rating for thse items
                            var userRatings = new List<double>();
                            var trusteeRatings = new List<double>();
                            foreach (var item in items)
                            {
                                var userRating = CreateRating(u, item, rndCat);
                                var trusteeRating = CreateRating(trustee, item, rndCat);

                                userRatings.Add(userRating);
                                trusteeRatings.Add(trusteeRating);
                            }
                            trust = Correlation.Pearson(userRatings, trusteeRatings);
                            var positiveTrust = Math.Round(Helpers.Positive(trust), 4);
                            Console.WriteLine("User {0} trusts user {1} by {2}", u.iduser, trustee.iduser, positiveTrust);
                            repo.CreateTrust(u.iduser, trustee.iduser, category, positiveTrust);
                        }
                    }


            }
            }

        }




        
    }
}
