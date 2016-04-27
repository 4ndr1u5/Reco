using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Reco.Model;

namespace Reco
{
    public class Similarities
    {
        Repository repo { get; set; }
        double Threshold { get; set; }
        public Similarities(double threshold)
        {
            repo = new Repository();
            Threshold = threshold;
        }

        private string GetSubMethod(string method)
        {
            var submethod = "";
            if (method == "PBASE")
            {
                submethod = "BASE";
            }


            if (method == "PMultiplication")
            {
                submethod = "Multiplication";
            }
            if (method == "PArithmetic")
            {
                submethod = "Arithmetic";
            }
            if (method == "PHarmonic")
            {
                submethod = "Harmonic";
            }

            if (method == "PMultiplicationU")
            {
                submethod = "Multiplication";
            }
            if (method == "PArithmeticU")
            {
                submethod = "Arithmetic";
            }
            if (method == "PHarmonicU")
            {
                submethod = "Harmonic";
            }
            return submethod;
        }

        public void EvaluateUserLevelDomainTrust(string method, string dsMethod)
        {
            var users = repo.getAllUsers();
            foreach (var user in users)
            {
                var domainSimilarities = EvaluateSingleUserDomainSimilaritiesPearson(user.iduser);
                var submethod = GetSubMethod(method);
                
                if (dsMethod == "Max")
                {
                    MaxTrustDomainSimilarities(user, domainSimilarities, submethod, method);
                }
                else if (dsMethod == "Avg")
                {
                    AverageTrustDomainSimilarities(user, domainSimilarities, submethod, method);
                }
            }
        }

        private void AverageTrustDomainSimilarities(User user, List<Tuple<int, int, double>> sims, string submethod, string method)
        {
            var trusts = repo.GetUsersTrusts(user.iduser, submethod);
            var usersTrusts = trusts.GroupBy(x => x.Item2.iduser);
            foreach (var trust in usersTrusts)
            {
                var missingTrustCategories = new List<int>() { 1, 2, 3, 4, 5 };
                var cats = trust.Select(x => x.Item1.Category).ToList();
                //missingTrustCategories = (List<int>) missingTrustCategories.Except(cats);
                foreach (var cat in cats)
                {
                    missingTrustCategories.Remove(cat);
                }
                foreach (var cat in missingTrustCategories)
                {
                    var total = 0d;
                    var divideBy = 0d;
                    foreach (var c in cats)
                    {
                        var similarity = sims.First(x => x.Item1 == cat && x.Item2 == c).Item3;
                        divideBy = divideBy + similarity;
                        total = total + similarity*similarity*trust.First(x => x.Item1.Category == c).Item1.TrustValue;
                        
                    }
                    //sim between max and missing trust
                    var evaluatedTrust = total/divideBy;
                    if (evaluatedTrust > Threshold)
                    {
                        repo.SaveTrust(user.iduser, trust.First().Item2.iduser, cat, method, evaluatedTrust);
                    }

                }
            }

        }

        private void MaxTrustDomainSimilarities(User user, List<Tuple<int, int, double>> sims, string submethod, string method)
        {
            var trusts = repo.GetUsersTrusts(user.iduser, submethod);
            var usersTrusts = trusts.GroupBy(x => x.Item2.iduser);
            foreach (var trust in usersTrusts)
            {
                var missingTrustCategories = new List<int>() { 1, 2, 3, 4, 5 };
                var maxtrust = trust.OrderByDescending(x => x.Item1.TrustValue).First();
                var cats = trust.Select(x => x.Item1.Category).ToList();
                //missingTrustCategories = (List<int>) missingTrustCategories.Except(cats);
                foreach (var cat in cats)
                {
                    missingTrustCategories.Remove(cat);
                }
                foreach (var cat in missingTrustCategories)
                {
                    //sim between max and missing trust
                    var domainSimilarity =
                        sims.Where(x => x.Item1 == maxtrust.Item1.Category && x.Item2 == cat).Select(x => x.Item3).First();
                    var evaluatedTrust = maxtrust.Item1.TrustValue * domainSimilarity;
                    if (evaluatedTrust > Threshold)
                    {
                        repo.SaveTrust(user.iduser, trust.First().Item2.iduser, cat, method, evaluatedTrust);
                    }

                }
            }
        }

        public void EvaluateDomainTrust(string method)
        {
            var sims = new List<Tuple<int, int, double>>();
            if (method.Substring(0,1) == "P")
            {
                sims = EvaluateDomainSimilaritiesPearson();
            }
            //else if (method.Substring(0, 1) == "C")
            //{
            //    sims = EvaluateDomainSimilaritiesCosine();
            //}
            // sims.Add(new Tuple<int, int, double>(1, 2, 0.62));
            // sims.Add(new Tuple<int, int, double>(1, 3, 0.37));
            // sims.Add(new Tuple<int, int, double>(1, 4, 0.48));
            // sims.Add(new Tuple<int, int, double>(1, 5, 0.63));
            // sims.Add(new Tuple<int, int, double>(2, 1, 0.62));
            // sims.Add(new Tuple<int, int, double>(2, 3, 0.08));
            // sims.Add(new Tuple<int, int, double>(2, 4, 0.31));
            // sims.Add(new Tuple<int, int, double>(2, 5, 0.43));
            // sims.Add(new Tuple<int, int, double>(3, 1, 0.37));
            // sims.Add(new Tuple<int, int, double>(3, 2, 0.08));
            // sims.Add(new Tuple<int, int, double>(3, 4, 0.53));
            // sims.Add(new Tuple<int, int, double>(3, 5, 0.45));
            // sims.Add(new Tuple<int, int, double>(4, 1, 0.48));
            // sims.Add(new Tuple<int, int, double>(4, 2, 0.30));
            // sims.Add(new Tuple<int, int, double>(4, 3, 0.53));
            // sims.Add(new Tuple<int, int, double>(4, 5, 0.26));
            // sims.Add(new Tuple<int, int, double>(5, 1, 0.63));
            // sims.Add(new Tuple<int, int, double>(5, 2, 0.43));
            // sims.Add(new Tuple<int, int, double>(5, 3, 0.46));
            // sims.Add(new Tuple<int, int, double>(5, 4, 0.26));
            var submethod = GetSubMethod(method);
            var users = repo.getAllUsers();
            foreach (var user in users)
            {
                AverageTrustDomainSimilarities(user, sims, submethod, method);
                //EvalUsersTrust(method, user, sims);
                //var submethod = method.Substring(1);
                //var trusts = repo.GetUsersTrusts(user.iduser, submethod);
                //var usersTrusts = trusts.GroupBy(x => x.Item2.iduser);

                //foreach (var trust in usersTrusts)
                //{
                //    var missingTrustCategories = new List<int>() { 1, 2, 3, 4, 5 };
                //    //signle user trusts
                //    var maxtrust = trust.OrderByDescending(x => x.Item1.TrustValue).First();
                //    var cats = trust.Select(x => x.Item1.Category).ToList();
                //    //missingTrustCategories = (List<int>) missingTrustCategories.Except(cats);
                //    foreach (var cat in cats)
                //    {
                //        missingTrustCategories.Remove(cat);
                //    }
                //    foreach (var cat in missingTrustCategories)
                //    {
                //        //sim between max and missing trust
                //        var domainSimilarity =
                //            sims.Where(x => x.Item1 == maxtrust.Item1.Category && x.Item2 == cat).Select(x => x.Item3).First();
                //        var evaluatedTrust = maxtrust.Item1.TrustValue*domainSimilarity;
                //        repo.SaveTrust(user.iduser, trust.First().Item2.iduser, cat, method, evaluatedTrust);

                //    }
                //}

            }
         
        }


        public List<Tuple<int, int, double>> EvaluateDomainSimilaritiesPearson()
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

        public List<Tuple<int, int, double>> EvaluateSingleUserDomainSimilaritiesPearson(int uid)
        {
            var result = new List<Tuple<int, int, double>>();
            for (var c1 = 1; c1 <= 5; c1++)
            {
                for (var c2 = 1; c2 <= 5; c2++)
                {
                    if (c1 != c2)
                    {
                        var trustsForTwoCategories = repo.GetUsersTrustsForTwoCategories(uid, c1, c2);
                        var trust1 = trustsForTwoCategories.Select(x => x.Item1).ToList();
                        var trust2 = trustsForTwoCategories.Select(x => x.Item2).ToList();
                        var domsim = new Tuple<int, int, double>(c1, c2, Correlation.Pearson(trust1, trust2));
                        result.Add(domsim);
                    }
                }
            }
            return result;
        }

        public List<Tuple<int, int, double>> EvaluateDomainSimilaritiesCosine()
        {
            var result = new List<Tuple<int, int, double>>();
            for (var c1 = 1; c1 <= 5; c1++)
            {
                for (var c2 = 1; c2 <= 5; c2++)
                {
                    if (c1 != c2)
                    {
                        var trustsForTwoCategories = repo.GetTrustsForTwoCategories(c1, c2);
                        var trust1 = trustsForTwoCategories.Select(x => x.Item1).ToArray();
                        var trust2 = trustsForTwoCategories.Select(x => x.Item2).ToArray();
                        var domsim = new Tuple<int, int, double>(c1, c2, MathNet.Numerics.Distance.Cosine(trust1, trust2));
                        result.Add(domsim);
                    }
                }
            }
            return result;
        }
    }
}
