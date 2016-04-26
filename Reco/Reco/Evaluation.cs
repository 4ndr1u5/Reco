using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MathNet.Numerics.Statistics;
using Reco.Model;

namespace Reco
{
    public class Evaluation
    {
        private class Results
        {
            public int count { get; set; }
            public double MAE { get; set; }
            public double MAUE { get; set; }
            public double RMSE { get; set; }
            public double RC { get; set; }
            public double UC { get; set; }

        }
        // MAE - mean average error
        // MAE = sqrt 1/|T| sum |r*_ui -r_ui|
        // MAUE - mean average user error
        // RMSE - root mean squared error
        // RMSE = sqrt 1/|T| sum (r*_ui -r_ui|)^2
        private Repository repo { get; set; }
        public Evaluation()
        {
            repo = new Repository();
        }
        public void EvaluateResults(string description, Algorithm algorithm)
        {
            var result = new List<Results>();
            // evaluate for diferent types of datasets
            // rating, predictedRating, userId
            var allUsers = repo.GetAllUsersRatingsForEvaluation(Helpers.TranslateMethodsFromEnum(algorithm));

            var coldUsers = FilterColdUsers(allUsers);
            var opinionatedUsers = FilterOpinionatedUsers(allUsers);

            var datasets = new List<List<Tuple<int, double?, int>>>();
            datasets.Add(coldUsers);
            datasets.Add(opinionatedUsers);
            datasets.Add(allUsers);

            foreach (var ds in datasets)
            {
               
                    var predictedDs = ds.Where(x => x.Item2 != null).Select(x => new Tuple<int, double, int>(x.Item1, (double)x.Item2, x.Item3)).ToList();
                    var usersCount = ds.GroupBy(x => x.Item3).Count();
                if (predictedDs.Count > 0)
                {
                    var res = new Results()
                    {
                        MAE = CalculateMAE(predictedDs),
                        MAUE = CalculateMAUE(predictedDs),
                        RMSE = CalculateRMSE(predictedDs),
                        UC = Math.Round(predictedDs.GroupBy(x => x.Item3).Count() / (double)usersCount, 4),
                        RC = Math.Round(predictedDs.Count() / (double)ds.Count(), 4),
                        count = usersCount
                    };
                    result.Add(res);
                }
                
            }

            //print out results 
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Github stuff\Reco\Reco\doc\Results.txt", true))
            {
                file.WriteLine("----------------");
                file.WriteLine(description);
                foreach (var res in result)
                {
                    file.WriteLine($"{res.MAE};{res.MAUE};{res.RMSE};{res.RC};{res.UC};{res.count}");
                }
            }
        }

        

        private double CalculateMAE(List<Tuple<int, double, int>> input)
        {
            var sum = input.Select(x => Helpers.Modulo(x.Item1, x.Item2)).Sum();
            var factor = input.Count;
            return Math.Sqrt(sum / factor);
        }
        private double CalculateMAUE(List<Tuple<int, double, int>> input)
        {
            var grouped = input.GroupBy(x => x.Item3).ToList();
            var list = new List<double>();
            foreach (var userRatings in grouped)
            {
                var element = CalculateMAE(userRatings.ToList());
                list.Add(element);
            }
            return list.Average();
        }

        private double CalculateRMSE(List<Tuple<int, double, int>> input)
        {
            var sum = input.Select(x => Math.Pow((x.Item1 - x.Item2), 2)).Sum();
            var factor = input.Count;
            return Math.Sqrt(sum / factor);
        }
        //Tuple description - rating, predictedRating, userId
        private List<Tuple<int, double?, int>> FilterColdUsers(List<Tuple<int, double?, int>> allUsers)
        {
            var coldUsers = new List<int>();
            var grouped = allUsers.GroupBy(x => x.Item3);
            var userRatings = new List<int>();
            var userId = 0;
            foreach (var user in grouped)
            {
                userId = user.Select(x => x.Item3).Distinct().First();
                userRatings = user.Select(x => x.Item1).ToList();
                if (userRatings.Count() <= 15)
                {
                    coldUsers.Add(userId);
                }
            }
            return allUsers.Where(x => coldUsers.Contains(x.Item3)).ToList();
        }
        private List<Tuple<int, double?, int>> FilterOpinionatedUsers(List<Tuple<int, double?, int>> allUsers)
        {
            var opinionatedUsers = new List<int>();
            var grouped = allUsers.GroupBy(x => x.Item3);
            var userRatings = new List<int>();
            var userId = 0;
            foreach (var user in grouped)
            {
                userId = user.Select(x => x.Item3).Distinct().First();
                userRatings = user.Select(x => x.Item1).ToList();
                if (userRatings.Select(x=>(double) x).StandardDeviation()>2)
                {
                    opinionatedUsers.Add(userId);
                }
            }
            return allUsers.Where(x => opinionatedUsers.Contains(x.Item3)).ToList();
        }

        public void EvaluateDataset(string method)
        {
            //number of users
            //number of ratings (distribution)
            //number of average ratings per user
            //average user's ratings variance
            var numberOfUsers = repo.getAllUsers().Count();
            var numberOfRatings = repo.GetRatingsCount();
            var numberOfRatings1 = repo.GetRatingsCount(1);
            var numberOfRatings2 = repo.GetRatingsCount(2);
            var numberOfRatings3 = repo.GetRatingsCount(3);
            var numberOfRatings4 = repo.GetRatingsCount(4);
            var numberOfRatings5 = repo.GetRatingsCount(5);
            var trust = repo.GetTrustCount();
            var trust05 = repo.GetTrustCount(0.5, method);
            var trust08 = repo.GetTrustCount(0.8, method);
            var lst = new List<double>();
            var lst1 = Enumerable.Repeat(1d, numberOfRatings1);
            var lst2 = Enumerable.Repeat(2d, numberOfRatings2);
            var lst3 = Enumerable.Repeat(3d, numberOfRatings3);
            var lst4 = Enumerable.Repeat(4d, numberOfRatings4);
            var lst5 = Enumerable.Repeat(5d, numberOfRatings5);
            lst.AddRange(lst1);
            lst.AddRange(lst2);
            lst.AddRange(lst3);
            lst.AddRange(lst4);
            lst.AddRange(lst5);


            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Github stuff\Reco\Reco\doc\Results.txt", true))
            {
                file.WriteLine("----------------");
                file.WriteLine(method);
                file.WriteLine("users;ratings,1,2,3,4,5,std,trust,trust0.5,trust0.8");
                file.WriteLine($"{numberOfUsers};{numberOfRatings};{numberOfRatings1};" +
                               $"{numberOfRatings2};{numberOfRatings3};{numberOfRatings4};" +
                               $"{numberOfRatings5};{lst.StandardDeviation()};{trust};{trust05};{trust08}");
            }
        }
    }
}