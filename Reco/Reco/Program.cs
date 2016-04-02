using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reco
{
    class Program
    {
        static void Main(string[] args)
        {
            const int userCount = 100;
            const int productCount = 300;
            var repo = new Repository();
            GenerateGraph(repo, userCount, productCount);
        }

        private static void GenerateGraph(Repository repo, int userCount, int productCount)
        {
            repo.CreateUsers(userCount);
            repo.CreateProducts(productCount);
        }
    }
}
