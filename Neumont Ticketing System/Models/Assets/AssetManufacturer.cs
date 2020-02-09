using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class AssetManufacturer : IEquatable<AssetManufacturer>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public List<string> EmailAddresses { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public bool Equals([AllowNull] AssetManufacturer other)
        {
            return other != null && other.Id.Equals(Id);
        }
    }
}
