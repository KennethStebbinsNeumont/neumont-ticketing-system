using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Tickets
{
    public class Ticket : IEquatable<Ticket>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int TicketId { get; set; }

        public string Title { get; set; }

        [BsonElement("Asset")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssetId { get; set; }

        public Repair Repair { get; set; }

        [BsonElement("Technicians")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> TechnicianIds { get; set; }

        [BsonElement("Loaners")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> LoanerIds { get; set; }

        public string Description { get; set; }

        public List<AdditionalField> AdditionalFields { get; set; }

        public List<TrackedString> Comments { get; set; }

        public DateTime Opened { get; set; }

        public DateTime? Closed { get; set; }

        public bool Equals([AllowNull] Ticket other)
        {
            return other != null && other.Id == Id;
        }
    }
}
