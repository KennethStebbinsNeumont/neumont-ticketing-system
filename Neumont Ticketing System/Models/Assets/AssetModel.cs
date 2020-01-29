using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class AssetModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Type { get; set; }

        public string ModelNumber { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Manufacturer { get; set; }
    }
}
