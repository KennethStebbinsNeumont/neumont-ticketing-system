using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Neumont_Ticketing_System.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models
{
    public class TrackedString
    {
        public string Value { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [BsonElement("Author")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }
    }
}
