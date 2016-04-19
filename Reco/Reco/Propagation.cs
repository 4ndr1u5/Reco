using System;
using System.Collections.Generic;
using System.Linq;

namespace Reco
{

    public class Propagation
    {
        private Repository repo { get; set; }

        public Propagation()
        {
            repo=new Repository();
        }

        

        public void GeneratePropagatedTrust(Algorithm method)
        {
            var methodNname = "";
            switch (method)
            {
                case Algorithm.Multiplication:
                    methodNname = "Multiplication";
                    break;
                case Algorithm.ArithmeticMean:
                    methodNname = "ArithmeticMean";
                    break;
                case Algorithm.HArmonicMean:
                    methodNname = "HarmonicMean";
                    break;
                default:
                    methodNname = "Error";
                    break;
            }


            var users = repo.getAllUsers();

            foreach (var u1 in users)
            {
                foreach (var u2 in users.Where(x=>x.iduser != u1.iduser))
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
                                    if (Algorithm.Multiplication == method)
                                    {
                                        newTrust = Math.Round(newTrust * rel.TrustValue, 4);
                                    }
                                    if (Algorithm.ArithmeticMean == method)
                                    {
                                        list.Add(rel.TrustValue);
                                    }
                                    if (Algorithm.HArmonicMean == method)
                                    {
                                        newTrust = Math.Round(newTrust * rel.TrustValue, 4);
                                    }
                                }
                                if (Algorithm.ArithmeticMean == method)
                                {
                                    newTrust = list.Average();
                                }
                                if (Algorithm.HArmonicMean == method)
                                {
                                    newTrust = Math.Pow(newTrust, (double) 1/path.Relationships.Count());
                                }

                                repo.SaveTrust(u1.iduser, u2.iduser, c, methodNname, newTrust);
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

   
}