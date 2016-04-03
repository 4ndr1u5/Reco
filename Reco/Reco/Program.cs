using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;

namespace Reco
{
    class Program
    {
        static void Main(string[] args)
        {
            const int userCount = 100;
            const int productCount = 300;
            var repo = new Repository();
            GenerateGraph(repo, userCount, productCount);
            GeneratePredictions(repo);
            

            //evauate MEA, RMSE
            //get rating relationships where predictedRating is not null
            var ratingsForEvaluation = repo.GetRatingsForEvaluation();
            var badPredictions = ratingsForEvaluation.Count(x => Helpers.Modulo(x.Item1, x.Item2) > 2);
            var mae = Helpers.CalculateMAE(ratingsForEvaluation);
            var rmse = Helpers.CalculateRMSE(ratingsForEvaluation);
        }

        public static void GeneratePredictions(Repository repo)
        {
            var users = repo.getAllUsers();
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
                        repo.CreatePredictedRating(u.iduser, prod.idproduct, predictedRating);
                    }

                }
                //get list of ratings and foreach

                //get trustees who have rated this item (trust, rating)
                //and calculate weighted average
            }
        }
        private static void GenerateGraph(Repository repo, int userCount, int productCount)
        {
            repo.CreateUsers(userCount);
            repo.CreateProducts(productCount);
            //generate ratings
            //each user gives a random amount of ratings to random items (imporvement - bias towards what he likes!!)
            var users = repo.getAllUsers();

            foreach (var u in users)
            {
                var trustCount = (int)Normal.Sample(new Random(u.iduser), 10, 9);
                var ratingsCount = (int)Normal.Sample(new Random(u.iduser), 30, 27);
                //connect to users (create trust)

                var trustees = repo.PickRandomUsers(trustCount, userCount, u.iduser);
                var category = new Random().Next(1, 5);
                double trust = 0;
                
                foreach (var trustee in trustees)
                {
                    switch (category)
                    {
                        case 1:
                            trust = 1 - Helpers.Modulo(trustee.l1, u.l1); 
                            break;
                        case 2:
                            trust = 1 - Helpers.Modulo(trustee.l2, u.l2);
                            break;                              
                        case 3:                                 
                            trust = 1 - Helpers.Modulo(trustee.l3, u.l3);
                            break;                             
                        case 4:                                 
                            trust = 1 - Helpers.Modulo(trustee.l4, u.l4);
                            break;                              
                        case 5:                                 
                            trust = 1 - Helpers.Modulo(trustee.l5, u.l5);
                            break;
                        default:
                            break;
                    }
                    repo.CreateTrust(u.iduser, trustee.iduser, category, Math.Round(trust,4));
                }
                //connect to products (generate ratings)
                var prods = repo.PickRandomProducts(ratingsCount, productCount);
                var rndU = new Random(u.iduser);
                foreach (var prod in prods)
                {
                    var quotient = (int)Normal.Sample(rndU, 7, 4);
                    //var rating = Math.Round(quotient * Math.Pow(u.l1 * prod.c1 + u.l2 * prod.c2 + u.l3 * prod.c3 + u.l4 * prod.c4 + u.l5 * prod.c5, 0.5));
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
                    var rating = quotient * Math.Sqrt(corr) + u.quality * prod.quality;
                    rating = rating > 5 ? 5 : rating;
                    rating = rating < 1 ? 1 : rating;
                    repo.CreateRating(u.iduser, prod.idproduct, (int)Math.Round(rating));
                }
                
            }

            
        }





























        //private static List<double> TestGraphCoverage(Repository repo, int userCount)
        //{
        //    var testUsers = repo.PickRandomUsers(10, userCount);
        //    var result = new List<List<int>>();
        //    foreach (var user in testUsers)
        //    {
        //        var userResult = new List<int>();
        //        var productsOfUser = repo.GetProductsRatedByUser(user.iduser);
        //        foreach (var prod in productsOfUser)
        //        {
        //            var number = repo.GetNumberOfTrusteesWhoHaveRatedThisProduct(user.iduser, prod.idproduct);
        //            userResult.Add(number);
        //        }
        //        result.Add(userResult);
        //    }

        //    //evaluate results
        //    var resultAvg = new List<double>();
        //    foreach (var subresult in result.Where(x => x.Count > 0))
        //    {
        //        var avg = subresult.Average();
        //        resultAvg.Add(avg);
        //    }
        //    return resultAvg;
        //}

        //private static void GenerateGraph(Repository repo, int userCount, int productcount, int categoryCount, int maxTrusts, int maxRatings)
        //{
        //    //var userCount = 100;
        //    //var productcount = 100;
        //    //var categoryCount = 5;

        //    //generate users
        //    repo.CreateUsers(userCount);
        //    repo.CreateProducts(productcount);
        //    repo.CreateCategories(categoryCount);

        //    var products = repo.getAllProducts();
        //    var cats = repo.getAllCategories();


        //    foreach (var p in products)
        //    {
        //        foreach (var c in cats)
        //        {
        //            repo.AssignProdToCat(p.idproduct, c.idcategory);
        //        }

        //    }

        //    var users = repo.getAllUsers();

        //    //var maxTrusts = 10;
        //    //var maxRatings = 10;
        //    var tr = 0;
        //    var pr = 0;

        //    foreach (var u in users)
        //    {
        //        if (tr > maxTrusts)
        //        {
        //            tr = 0;
        //        }
        //        if (pr > maxRatings)
        //        {
        //            pr = 0;
        //        }
        //        //connect to users (create trust)

        //        var rndTrust = new Random();
        //        //var rndUserIds = new Random();
        //        //var uids = new List<int>();
        //        //for (var i = 0; i < tr; i++)
        //        //{
        //        //    uids.Add(rndUserIds.Next(1, userCount));
        //        //}
        //        var trustees = repo.PickRandomUsers(tr, userCount);
        //        var rndCat = new Random();
        //        foreach (var trustee in trustees)
        //        {
        //            repo.CreateTrust(u.iduser, trustee.iduser, rndCat.Next(1, categoryCount), rndTrust.NextDouble());
        //        }
        //        //connect to products (generate ratings)
        //        var rndRating = new Random();


        //        //var rndProdIds = new Random();
        //        //var pids = new List<int>();
        //        //for (var i = 0; i < pr; i++)
        //        //{
        //        //    pids.Add(rndProdIds.Next(1, productcount));
        //        //}

        //        var prods = repo.PickRandomProducts(pr, productcount);
        //        foreach (var prod in prods)
        //        {
        //            repo.CreateRating(u.iduser, prod.idproduct, rndRating.Next(1, 5));
        //        }

        //        tr++;
        //        pr++;
        //    }

        //}

        //private static void PropagateTrusts(Repository repository, int categorycount)
        //{
        //    //var topusers = repository.getTopUsers();
        //    var users = repository.getAllUsers();

        //    foreach (var u1 in users)
        //    {
        //        foreach (var u2 in users)
        //        {
        //            if (u1 != u2)
        //            {

        //                try
        //                {
        //                    for (var c = 1; c <= categorycount; c++)
        //                    {
        //                        var newTrust = 1.0;
        //                        var path = repository.GetShortestPaths(u1.iduser, u2.iduser, c).FirstOrDefault();
        //                        var allPositive = true;

        //                        if (path != null && path.Relationships.Count() > 1)
        //                        {
        //                            foreach (var rel in path.Relationships)
        //                            {
        //                                if (rel.TrustValue < 0) allPositive = false;
        //                            }
        //                            if (allPositive)
        //                            {
        //                                foreach (var rel in path.Relationships)
        //                                {
        //                                    newTrust = Math.Round(newTrust * rel.TrustValue, 4);
        //                                }
        //                                repository.SaveTrust(u1.iduser, u2.iduser, "ShortestPath", newTrust);

        //                            }

        //                        }
        //                    }

        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine("Exception");
        //                }

        //            }
        //        }
        //    }
    }
}
