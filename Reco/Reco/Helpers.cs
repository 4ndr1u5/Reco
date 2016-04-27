using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Reco
{
    public static class Helpers
    {
        public static double Modulo(double x, double y)
        {
            if (x > y)
            {
                return x - y;
            }
            else
            {
                return y - x;
            }
        }

        public static double Div(double x, double y)
        {
            if (x > y)
            {
                return Math.Round(y/x,4);
            }
            else
            {
                return Math.Round(x / y, 4);
            }
        }

        public static double Floor(double x)
        {
            if (x < 0)
            {
                x = 0;
            }
            return x;
        }

        public static double BetweenZeroAndOne(double x)
        {
            if (x < 0)
            {
                x = 0;
            }
            else if (x > 1)
            {
                x = 1;
            }
            return x;
        }

        public static double Positive(double x)
        {
            return (x + 1.0)/2;
        }
        public static double WeightedAverage(List<Tuple<double, int>> input)
        {
            var factors = new List<double>();
            foreach (var i in input)
            {
                var factor = i.Item1 * i.Item2;
                factors.Add(factor);
            }
            return factors.Sum();
        }

        public static string TranslateMethodsFromEnum(Algorithm alg)
        {
            switch (alg)
            {
                case Algorithm.CN:
                    return "CN";
                case Algorithm.Base:
                    return "BASE";
                case Algorithm.Multiplication:
                    return "Multiplication";
                case Algorithm.ArithmeticMean:
                    return "ArithmeticMean";
                case Algorithm.HArmonicMean:
                    return "HarmonicMean";
                case Algorithm.PBase:
                    return "PBASE";
                case Algorithm.PMultiplication:
                    return "PMultiplication";
                case Algorithm.PArithmeticMean:
                    return "PArithmeticMean";
                case Algorithm.PHArmonicMean:
                    return "PHarmonicMean";

                case Algorithm.PUMultiplication:
                    return "PMultiplicationU";
                case Algorithm.PUArithmeticMean:
                    return "PArithmeticMeanU";
                case Algorithm.PUHArmonicMean:
                    return "PHarmonicMeanU";
                default:
                    return "error";
            }
        }

        public static string[] GetMethods(string method)
        {
            switch (method)
            {
                case "CN": return new string[1] { "CN" };
                case "BASE": return new string[1] { "BASE" };
                case "Multiplication": return new string[2] { "BASE", "Multiplication" };
                case "ArithmeticMean": return new string[2] { "BASE", "ArithmeticMean" };
                case "HarmonicMean": return new string[2] { "BASE", "HarmonicMean" };

                case "PBASE": return new string[2] { "BASE" , "PBASE" };
                case "PMultiplication": return new string[3] { "BASE", "Multiplication" , "PMultiplication" };
                case "PArithmeticMean": return new string[3] { "BASE", "ArithmeticMean", "PArithmeticMean" };
                case "PHarmonicMean": return new string[3] { "BASE", "HarmonicMean", "PHarmonicMean" };

                case "PMultiplicationU": return new string[3] { "BASE", "Multiplication", "PMultiplicationU" };
                case "PArithmeticMeanU": return new string[3] { "BASE", "ArithmeticMean", "PArithmeticMeanU" };
                case "PHarmonicMeanU": return new string[3] { "BASE", "HarmonicMean", "PHarmonicMeanU" };
                default: return new string[0] {};
            }
        }
    }
}
