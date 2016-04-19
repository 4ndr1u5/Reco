using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reco.Model
{
    public class Rated : Relationship
                , IRelationshipAllowingSourceNode<User>
                    , IRelationshipAllowingTargetNode<Product>
    {
        public Rated() : base(-1, null) { }
        public override string RelationshipTypeKey
        {
            get { return "Rated"; }
        }
        public int Rating { get; set; }
        public double PredictedRatingBase { get; set; }
        public double PredictedRatingMulti { get; set; }
        public double PredictedRatingArit { get; set; }
        public double PredictedRatingHarm { get; set; }

        public int Step { get; set; }
    }



    public class Trust : Relationship
        , IRelationshipAllowingSourceNode<User>
            , IRelationshipAllowingTargetNode<User>
    {
        public static readonly string TypeKey = "SIMILARITY";
        public Trust() : base(-1, null) { }
        public Trust(NodeReference targetNode)
            : base(targetNode)
        { }

        public override string RelationshipTypeKey
        {
            get { return TypeKey; }
        }
        public float TrustValue { get; set; }
        public int Category { get; set; }
        public string Method { get; set; }
    }
}
