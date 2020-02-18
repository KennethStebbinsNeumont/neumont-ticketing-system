using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Tickets
{
    public class Repair
    {
        [BsonElement("Definition")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DefinitionId { get; set; }

        public List<RepairStep> Steps { get; set; }
    }
}
