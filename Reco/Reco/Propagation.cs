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

        

        public void GeneratePropagatedTrust(Algorithm method, double threshold)
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            var methodNname = Helpers.TranslateMethodsFromEnum(method);
          

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
                                        newTrust = newTrust + (1/rel.TrustValue);
                                    }
                                }
                                if (Algorithm.ArithmeticMean == method)
                                {
                                    newTrust = list.Average();
                                }
                                if (Algorithm.HArmonicMean == method)
                                {
                                    newTrust = path.Relationships.Count()/newTrust;
                                }
                                if (newTrust > threshold)
                                {
                                    
                                    Console.WriteLine(String.Format("Save trust user {0} - user {1}, category {2}, value {3}", u1.iduser, u2.iduser, c, newTrust));
                                    repo.SaveTrust(u1.iduser, u2.iduser, c, methodNname, newTrust);
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception");
                    }
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
            }
        }
    }

   
}