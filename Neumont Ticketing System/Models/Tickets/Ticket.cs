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

        public Asset Asset { get; set; }

        public RepairDefinition Repair { get; set; }

        public List<AppUser> Technicians { get; set; }

        public List<LoanerAsset> Loaners { get; set; }

        public string Description { get; set; }

        public List<AdditionalField> AdditionalFields { get; set; }

        public List<TrackedString> Comments { get; set; }

        public bool Equals([AllowNull] Ticket other)
        {
            return other != null && other.Id == Id;
        }
    }
}
