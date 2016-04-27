using System;
using System.Collections.Generic;
using MathNet.Numerics;
using Reco.Model;

namespace Reco
{
    public class Prediction
    {

        private Repository repo { get; set; }

        public Prediction()
        {
            repo = new Repository();
        }

        public void GeneratePredictions(string methodName)
        {
            var users = repo.getAllUsers();
            foreach (var u in users)
            {
                var ratings = repo.GetUsersRatings(u.iduser);
                foreach (var rat in ratings)
                {
                    var prod = rat.Item2;
                    //get trustees in that category who have rated this item (trust, rating)
                   
                    var trustRating = repo.GetTrusteesWhoHaveRatedThisProduct(u.iduser, prod.category, prod.idproduct, Helpers.GetMethods(methodName));
                    if (trustRating.Count > 0)
                    {
                        var predictedRating = Math.Round(Helpers.WeightedAverage(trustRating), 0);
                        predictedRating = predictedRating > 5 ? 5 : predictedRating;
                        predictedRating = predictedRating < 1 ? 1 : predictedRating;
                        if (predictedRating >= 1 && predictedRating <= 5)
                        {
                            Console.WriteLine("For user {0} item {1} predicted {2}", u.iduser, prod.idproduct, predictedRating);
                            repo.CreatePredictedRating(u.iduser, prod.idproduct, predictedRating, methodName);
                        }

                    }

                }

            }
        }

       
    }
}