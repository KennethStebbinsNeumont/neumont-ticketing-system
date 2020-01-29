using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class Asset
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SerialNumber { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Model { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Owner { get; set; }
    }
}
