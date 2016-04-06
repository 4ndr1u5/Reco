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

        public static double WeightedAverage(List<Tuple<double, int>>  input)
        {
            var factors = new List<double>();
            foreach (var i in input)
            {
                var factor = i.Item1*i.Item2;
                factors.Add(factor);
            }
            return factors.Sum();
        }

        public static double CalculateMAE(List<Tuple<int, double>> input)
        {
            // MAE - mean average error
            // MAE = sqrt 1/|T| sum |r*_ui -r_ui|
            // MAUE - mean average user error
            // RMSE - root mean squared error
            // RMSE = sqrt 1/|T| sum (r*_ui -r_ui|)^2
            var sum = input.Select(x => Modulo(x.Item1, x.Item2)).Sum();
            var factor = input.Count;
            return Math.Sqrt(sum/factor);
        }

        public static double CalculateRMSE(List<Tuple<int, double>> input)
        {
            // MAE - mean average error
            // MAE = sqrt 1/|T| sum |r*_ui -r_ui|
            // MAUE - mean average user error
            // RMSE - root mean squared error
            // RMSE = sqrt 1/|T| sum (r*_ui -r_ui|)^2
            var sum = input.Select(x => Math.Pow((x.Item1 - x.Item2),2)).Sum();
            var factor = input.Count;
            return Math.Sqrt(sum / factor);
        }
    }
}
