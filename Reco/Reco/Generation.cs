using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Reco.Model;

namespace Reco
{
    public class Generation
    {
        private int NumberOfUsers { get; set; }
        private int NumberOfProducts { get; set; }
        private int NumberOfRatingsMean { get; set; }
        private int NumberOfRatingsDeviation { get; set; }

        private Repository repo { get; set; }

        public Generation(int numberOfUsers, int numberOfProducts, int numberOfRatingsMean, int numberOfRatingsDeviation)
        {
            NumberOfUsers = numberOfUsers;
            NumberOfProducts = numberOfProducts;
            NumberOfRatingsMean = numberOfRatingsMean;
            NumberOfRatingsDeviation = numberOfRatingsDeviation;
            repo = new Repository();
        }

        public void GenerateGraph()
        {
            repo.CreateUsers(NumberOfUsers);
            repo.CreateProducts(NumberOfProducts);
            GenerateRatings();
            GenerateTrust();
        }

        public void GenerateRatings()
        {
            var users = repo.getAllUsers();
            foreach (var u in users)
            {
                var ratingsCount = (int)Normal.Sample(new Random(u.iduser), NumberOfRatingsMean, NumberOfRatingsDeviation);
                var prods = repo.PickRandomProducts(ratingsCount, NumberOfProducts);
                var rndCoef = new Random();
                foreach (var prod in prods)
                {
                    var rating = CreateRating(u, prod, rndCoef);
                    Console.WriteLine("User {0} rated item {1} by {2}", u.iduser, prod.idproduct, rating);
                    repo.CreateRating(u.iduser, prod.idproduct, (int)Math.Round(rating));
                }
            }
        }

        private double CreateRating(User u, Product prod, Random rndCoef)
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

            //var rating = 5 * (coeff * Math.Sqrt(corr) + (1 - coeff) * u.quality * prod.quality);
            var rating = 5 * (u.quality * Math.Sqrt(corr) + (1 - u.quality) * prod.quality);
            rating = rating > 5 ? 5 : rating;
            rating = rating < 1 ? 1 : rating;

            return rating;
        }

        public void GenerateTrust()
        {
            var users = repo.getAllUsers();
            var rndCat = new Random();
            var rndCatNumber = new Random();
            foreach (var u in users)
            {
                var trustCount = (int)Normal.Sample(new Random(u.iduser), 3, 6);
                trustCount = trustCount < 0 ? 0 : trustCount;
                var trustees = repo.PickRandomUsers(trustCount, NumberOfUsers, u.iduser);
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
                        var items = repo.PickRandomProducts(12, NumberOfProducts, category, rndCat);
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
                            if (positiveTrust > 0)
                            {
                                Console.WriteLine("User {0} trusts user {1} by {2}", u.iduser, trustee.iduser, positiveTrust);
                                repo.CreateTrust(u.iduser, trustee.iduser, category, positiveTrust, "BASE");
                            }
                            
                        }
                    }
                }
            }
        }
    }
}