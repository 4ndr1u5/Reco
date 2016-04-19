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
            var allUsers = repo.GetAllUsersRatingsForEvaluation(Helpers.TranslateMethodsFromEnum(algorithm));

            var coldUsers = FilterColdUsers(allUsers);
            var opinionatedUsers = FilterOpinionatedUsers(allUsers);

            var numberOfUsers = repo.GetUsersWhoHaveRatedOneOrMoreItemsCount();
            var numberOfRatings = repo.GetRatingsCount();

            var datasets = new List<List<Tuple<int, double, int>>>();
            datasets.Add(coldUsers);
            datasets.Add(opinionatedUsers);
            datasets.Add(allUsers);

            foreach (var ds in datasets)
            {
                var res = new Results()
                {
                    MAE = CalculateMAE(ds),
                    MAUE = CalculateMAUE(ds),
                    RMSE = CalculateRMSE(ds),
                    UC = Math.Round((ds.Select(x=>x.Item1).Distinct().Count() / (double) numberOfUsers), 4),
                    RC = Math.Round(ds.Count() / (double) numberOfRatings, 4)
                };
                result.Add(res);
            }

            //print out results 
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Github stuff\Reco\Reco\doc\Results.txt", true))
            {
                file.WriteLine("----------------");
                file.WriteLine(description);
                foreach (var res in result)
                {
                    file.WriteLine($"{res.MAE};{res.MAUE};{res.RMSE};{res.RC};{res.UC}");
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
        private List<Tuple<int, double, int>> FilterColdUsers(List<Tuple<int, double, int>> allUsers)
        {
            var coldUsers = new List<int>();
            var grouped = allUsers.GroupBy(x => x.Item3);
            var userRatings = new List<int>();
            var userId = 0;
            foreach (var user in grouped)
            {
                userId = user.Select(x => x.Item3).Distinct().First();
                userRatings = user.Select(x => x.Item1).ToList();
                if (userRatings.Count() <= 3)
                {
                    coldUsers.Add(userId);
                }
            }
            return allUsers.Where(x => coldUsers.Contains(x.Item3)).ToList();
        }
        private List<Tuple<int, double, int>> FilterOpinionatedUsers(List<Tuple<int, double, int>> allUsers)
        {
            var opinionatedUsers = new List<int>();
            var grouped = allUsers.GroupBy(x => x.Item3);
            var userRatings = new List<int>();
            var userId = 0;
            foreach (var user in grouped)
            {
                userId = user.Select(x => x.Item3).Distinct().First();
                userRatings = user.Select(x => x.Item1).ToList();
                if (userRatings.Select(x=>(double) x).StandardDeviation()>1)
                {
                    opinionatedUsers.Add(userId);
                }
            }
            return allUsers.Where(x => opinionatedUsers.Contains(x.Item3)).ToList();
        }
    }
}