using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class LoanerAsset : IEquatable<LoanerAsset>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public Asset Asset { get; set; }

        public bool InInventory { get; set; }

        public bool Available { get; set; }

        public bool Equals([AllowNull] LoanerAsset other)
        {
            return other != null && other.Id == Id;
        }
    }
}
