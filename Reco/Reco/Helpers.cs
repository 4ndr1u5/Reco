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
                case Algorithm.Base:
                    return "BASE";
                case Algorithm.Multiplication:
                    return "SHORTMULTI";
                case Algorithm.ArithmeticMean:
                    return "SHORTARIT";
                case Algorithm.HArmonicMean:
                    return "SHORTHARM";
                default:
                    return "error";
            }
        }

        public static string[] GetMethods(string method)
        {
            switch (method)
            {
                case "BASE": return new string[1] { "BASE" };
                case "SHORTMULTI": return new string[2] { "BASE", "SHORTMULTI" };
                case "SHORTARIT": return new string[2] { "BASE", "SHORTARIT" };
                case "SHORTHARM": return new string[2] { "BASE", "SHORTHARM" };
                default: return new string[0] {};
            }
        }
    }
}
