using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private static void GenerateGraph(Repository repo, int userCount, int productCount)
        {
            repo.CreateUsers(userCount);
            repo.CreateProducts(productCount);
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
