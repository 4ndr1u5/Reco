using System;
using Neo4jClient;
using System.Collections.Generic;
using Neo4jClient.Cypher;
using System.Linq;
using System.Runtime.Remoting;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Integration;
using Reco.Model;

namespace Reco
{

    public class Repository
    {
        public GraphClient client { get; set; }

        public Repository()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "a");
            client.Connect();
        }



        public IEnumerable<User> getTopUsers()
        {
            var users = client.Cypher
                .Match("(user:User)-[t:TRUSTS]->(:User)")
                .Where((User user, Trust t) => user.iduser > 4 && t.TrustValue != 0.4)
                .Return((user, t) => new {usr = user.As<User>(), count = t.Count()}).OrderBy("count desc")
                .Limit(100)
                .Results;

            return users.ToList().Select(x => x.usr);
        }

        public IEnumerable<User> getAllUsers()
        {
            var users = client.Cypher
                .Match("(user:User)")
                .Return((user) => user.As<User>())
                .Results;

            return users.ToList();
        }

        public IEnumerable<User> PickUsers(List<int> ids)
        {
            var users = new List<User>();

            foreach (var id in ids)
            {
                var us = client.Cypher
                    .Match("(user:User)")
                    .Where((User user) => user.iduser == id)
                    .Return((user) => user.As<User>())
                    .Results;
                users.AddRange(us);
            }


            return users.ToList();
        }

        public IEnumerable<User> PickRandomUsers(int number, int max, int uid)
        {
            var rndUserIds = new Random();
            var uids = new List<int>();
            for (var i = 0; i < number; i++)
            {
                uids.Add(rndUserIds.Next(1, max));
            }
            var thisUser = new List<int>();
            thisUser.Add(uid);
            var uniqueuids = uids.Distinct().Except(thisUser).ToList();
            return PickUsers(uniqueuids);
        }

        public IEnumerable<Product> getAllProducts()
        {
            var prods = client.Cypher
                .Match("(p:Product)")
                .Return((p) => p.As<Product>())
                .Results;

            return prods.ToList();
        }

        public IEnumerable<Product> PickProducts(List<int> ids)
        {
            var prods = new List<Product>();
            foreach (var id in ids)
            {
                var pr = client.Cypher
                    .Match("(prod:Product)")
                    .Where((Product prod) => prod.idproduct == id)
                    .Return((prod) => prod.As<Product>())
                    .Results;
                prods.AddRange(pr);

            }


            return prods.ToList();
        }

        public IEnumerable<Product> PickRandomProducts(int number, int max)
        {
            var rndProdIds = new Random();
            var pids = new List<int>();
            for (var i = 0; i < number; i++)
            {
                pids.Add(rndProdIds.Next(1, max));
            }
            var uniquepids = pids.Distinct().ToList();
            return PickProducts(uniquepids);
        }


        public IEnumerable<Product> GetProductsRatedByUser(int uid)
        {
            var products = client.Cypher
                .Match("(u:User)-[r:Rated]->(prod:Product)")
                .Where((User u) => u.iduser == uid)
                .Return((prod) => prod.As<Product>())
                .Results;

            return products.ToList();
        }

        //public IEnumerable<Category> getAllCategories()
        //{
        //    var cats = client.Cypher
        //        .Match("(cat:Category)")
        //            .Return((cat) => cat.As<Category>())
        //            .Results;

        //    return cats.ToList();
        //}

        public class PathsResult<TNode>
        {
            public IEnumerable<Node<TNode>> Nodes { get; set; }
            public IEnumerable<Trust> Relationships { get; set; }
        }

        public IEnumerable<PathsResult<User>> GetShortestPaths(int u1, int u2, int cid)
        {
            var pathsQuery =
                client.Cypher
                    .Match("p = shortestPath((u:User)-[t:Trusts*..3]-(v:User)) WHERE(u.iduser = " + u1 +
                           ") AND(v.iduser =  " + u2 + ") AND all(x IN rels(p) WHERE x.Category =  " + cid + ")")
                    //.Match("p = shortestPath((u:User)-[t:Trusts*..5]-(v:User))")
                    //.Where((User u) => u.iduser == u1)
                    //.AndWhere((User v) => v.iduser == u2)
                    //all(x IN rels(p) WHERE x.Category =1)
                    //.AndWhere((Trust t) => t.Category == cid)
                    .Return(p => new PathsResult<User>
                    {
                        Nodes = Return.As<IEnumerable<Node<User>>>("nodes(p)"),
                        Relationships = Return.As<IEnumerable<Trust>>("rels(p)")
                    });

            var res = pathsQuery.Results;
            return res;
        }

        public void SaveTrust(int u1, int u2, int cat, string method, double value)
        {
            //var trust = new Trust () { TrustValue = value, Method = method };
            try
            {
                client.Cypher
                    .Match("(user1:User {iduser:{u1}}),  (user2:User {iduser:{u2}})")
                    .Create("user1-[:Trusts {TrustValue:{tv},Method:{meth},Category:{cat}}]->user2")
                    //.WithParam("iduser1", new { iduser = similarityData.Item1.iduser})
                    //.WithParam("iduser2", new { iduser = similarityData.Item2})
                    .WithParam("u1", u1)
                    .WithParam("u2", u2)
                    .WithParam("tv", value)
                    .WithParam("cat", cat)
                    .WithParam("meth", method)
                    .ExecuteWithoutResults();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void CreateUsers(int number)
        {
            //var rnd = new Random();

            for (var i = 1; i <= number; i++)
            {
                var rnd = new Random(i);
                var n1 = rnd.NextDouble();
                var n2 = rnd.NextDouble();
                var n3 = rnd.NextDouble();
                var n4 = rnd.NextDouble();
                var n5 = rnd.NextDouble();
                var sum = n1 + n2 + n3 + n4 + n5;
                var l1 = Math.Round(n1/sum, 4);
                var l2 = Math.Round(n2/sum, 4);
                var l3 = Math.Round(n3/sum, 4);
                var l4 = Math.Round(n4/sum, 4);
                var l5 = Math.Round(n5/sum, 4);
                var quality = new Random().NextDouble();
                //var quality = Normal.Sample(new Random(), 0.5, 0.5);
                quality = quality > 1 ? 1 : quality;
                quality = quality <0 ? 0 : quality;
                client
                    .Cypher
                    .Create(("(user:User{iduser:{id}, l1:{l1}, l2:{l2}, l3:{l3}, l4:{l4}, l5:{l5}, quality:{quality}})"))
                    .WithParam("id", i)
                    .WithParam("l1", l1)
                    .WithParam("l2", l2)
                    .WithParam("l3", l3)
                    .WithParam("l4", l4)
                    .WithParam("l5", l5)
                    .WithParam("quality", quality)
                    .ExecuteWithoutResults();
            }
        }

        public void CreateProducts(int number)
        {

            for (var i = 1; i <= number; i++)
            {
                var cat = new Random().Next(1, 5);
                double w1 = 0;
                double w2 = 0;
                double w3 = 0;
                double w4 = 0;
                double w5 = 0;
                switch (cat)
                {
                    case 1:
                        w1 = 0.55;
                        w2 = 0.2;
                        w3 = 0.05;
                        w4 = 0.1;
                        w5 = 0.1;
                        break;
                    case 2:
                        w1 = 0.2;
                        w2 = 0.6;
                        w3 = 0.05;
                        w4 = 0.05;
                        w5 = 0.1;
                        break;
                    case 3:
                        w1 = 0.2;
                        w2 = 0.05;
                        w3 = 0.35;
                        w4 = 0.2;
                        w5 = 0.2;
                        break;
                    case 4:
                        w1 = 0.2;
                        w2 = 0.05;
                        w3 = 0.1;
                        w4 = 0.65;
                        w5 = 0;
                        break;
                    case 5:
                        w1 = 0.3;
                        w2 = 0.1;
                        w3 = 0.05;
                        w4 = 0.05;
                        w5 = 0.5;
                        break;
                    default:
                        break;
                }
                //jeigu produktas is tam tikros kategorijos jis turetu buti tikrai su daugiausia tos kategorijos pozymio
                //o ne tik truputi ir tai nebutinai
                var rnd = new Random(i);
                //variantas su svoriais - sudas nes komedija gali buti is daugiau siaubo nei komedijos - per didele random dydzio itaka
                //var n1 = rnd.NextDouble()*w1;
                //var n2 = rnd.NextDouble()*w2;
                //var n3 = rnd.NextDouble()*w3;
                //var n4 = rnd.NextDouble()*w4;
                //var n5 = rnd.NextDouble()*w5;
                var n1 = Helpers.BetweenZeroAndOne(Normal.Sample(rnd, w1, 0.3));
                var n2 = Helpers.BetweenZeroAndOne(Normal.Sample(rnd, w2, 0.3));
                var n3 = Helpers.BetweenZeroAndOne(Normal.Sample(rnd, w3, 0.3));
                var n4 = Helpers.BetweenZeroAndOne(Normal.Sample(rnd, w4, 0.3));
                var n5 = Helpers.BetweenZeroAndOne(Normal.Sample(rnd, w5, 0.3));

                var sum = n1 + n2 + n3 + n4 + n5;
                var c1 = Math.Round(n1/sum, 4);
                var c2 = Math.Round(n2/sum, 4);
                var c3 = Math.Round(n3/sum, 4);
                var c4 = Math.Round(n4/sum, 4);
                var c5 = Math.Round(n5/sum, 4);
                var quality = (int)Math.Round(Normal.Sample(new Random(), 4, 2));
                quality = quality > 5 ? 5 : quality;
                quality = quality < 1 ? 1 : quality;
                client
                    .Cypher
                    .Create(
                        ("(prod:Product{idproduct:{id}, category:{category}, c1:{c1}, c2:{c2}, c3:{c3}, c4:{c4}, c5:{c5}, quality:{quality}})"))
                    .WithParam("id", i)
                    .WithParam("category", cat)
                    .WithParam("c1", c1)
                    .WithParam("c2", c2)
                    .WithParam("c3", c3)
                    .WithParam("c4", c4)
                    .WithParam("c5", c5)
                    .WithParam("quality", quality)
                    .ExecuteWithoutResults();
            }
        }

        //public void CreateCategories(int number)
        //{
        //    for (var i = 1; i <= number; i++)
        //    {
        //        client.Cypher.Create(("(cat:Category{idcategory:" + i + "})")).ExecuteWithoutResults();
        //    }
        //}

        //public void AssignProdToCat(int idprod, int idcat)
        //{
        //    client.Cypher.Match("(p:Product), (c:Category)")
        //        .Where((Product p) => p.idproduct == idprod)
        //        .AndWhere((Category c) => c.idcategory == idcat)
        //        .Create("(p)-[b:BelongsTo]->(c)")
        //        .ExecuteWithoutResults();
        //}

        public void CreateTrust(int u1id, int u2id, int cid, double trust)
        {
            client.Cypher.Match("(u1:User), (u2:User)")
                .Where((User u1) => u1.iduser == u1id)
                .AndWhere((User u2) => u2.iduser == u2id)
                .Create("(u1)-[t:Trusts{TrustValue:{trust}, Category: {cid}}]->(u2)")
                .WithParam("trust", Math.Round(trust, 4))
                .WithParam("cid", cid)
                .ExecuteWithoutResults();
        }

        public void CreateRating(int uid, int pid, int rating)
        {
            client.Cypher.Match("(u:User), (p:Product)")
                .Where((User u) => u.iduser == uid)
                .AndWhere((Product p) => p.idproduct == pid)
                .Create("(u)-[t:Rated{Rating:{rating}}]->(p)")
                .WithParam("rating", rating)
                .ExecuteWithoutResults();
        }

        public void CreatePredictedRating(int uid, int pid, double rating, int step)
        {
            client.Cypher.Match("(u:User)-[r:Rated]->(p:Product)")
                .Where((User u) => u.iduser == uid)
                .AndWhere((Product p) => p.idproduct == pid)
                .Set("r.PredictedRating = {rating}, r.Step = {step}")
                //.Create("(u)-[r:Rated{PredictedRating:{rating}}]->(p)")
                .WithParam("rating", rating)
                .WithParam("step", step)
                .ExecuteWithoutResults();
        }

        public int GetNumberOfTrusteesWhoHaveRatedThisProduct(int uid, int pid)
        {
            var number =
                client.Cypher
                    .Match("(u:User)-[r:Rated]->(p:Product)<-[rt:Rated]-(v:User), (u:User)-[t:Trusts]->(v:User)")
                    .Where((User u) => u.iduser == uid)
                    .AndWhere((Product p) => p.idproduct == pid)
                    .Return(v => v.Count())
                    .Results;

            return (int) (number.First());
        }

        public int GetTrustCount()
        {
            var number =
                client.Cypher
                    .Match("(u:User)-[r:Trusts]->(v:User)")
                    //.Where((User u) => u.iduser == uid)
                    .Return(r => r.Count())
                    .Results;

            return (int) (number.First());
        }

        public List<Tuple<Rated, Product>> GetUsersRatings(int uid)
        {
            var ratings = client.Cypher
                .Match("(u:User)-[r:Rated]->(prod:Product)")
                .Where((User u) => u.iduser == uid)
                .Return((r, prod) => new Tuple<Rated, Product>(r.As<Rated>(), prod.As<Product>()))
                .Results;

            return ratings.ToList();
        }

        public List<Tuple<double, int>> GetTrusteesWhoHaveRatedThisProduct(int uid, int cid, int pid)
        {
            var query =
                client.Cypher
                    .Match("(u:User)-[r:Rated]->(p:Product)<-[rt:Rated]-(v:User), (u:User)-[t:Trusts]->(v:User)")
                    .Where((User u) => u.iduser == uid)
                    .AndWhere((Product p) => p.idproduct == pid)
                    .AndWhere((Trust t) => t.Category == cid)
                    .Return((t, rt) => new Tuple<double, int>(t.As<Trust>().TrustValue, rt.As<Rated>().Rating));
            var result = query.Results;
            return result.ToList();
        }

        public List<Tuple<int, double>> GetRatingsForEvaluation(int step)
        {
            var query =
                client.Cypher
                    .Match("(u:User)-[r:Rated]->(p:Product) where has(r.PredictedRating) and r.Step = {step}")
                    .WithParam("step", step)
                    .Return((r) => new Tuple<int, double>(r.As<Rated>().Rating, r.As<Rated>().PredictedRating));
            var result = query.Results;
            return result.ToList();
        }

        public List<Tuple<double, double>>  GetTrustsForTwoCategories(int c1, int c2)
        {
             var query =
                client.Cypher
                    .Match("(u:User)-[t1:Trusts]->(v:User), (u:User)-[t2:Trusts]->(v:User)")
                    .Where((Trust t1)=>t1.Category==c1)
                    .AndWhere((Trust t2) => t2.Category == c2)
                    .Return((t1, t2) => new Tuple<double, double>(t1.As<Trust>().TrustValue, t2.As<Trust>().TrustValue));
        var result = query.Results;
            return result.ToList();
        }
    } 

}
