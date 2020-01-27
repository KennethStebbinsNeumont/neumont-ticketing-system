using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppRole : IdentityRole<string>, IEquatable<AppRole>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public override string Id { get; set; }
        
        // This value must be unique
        public override string Name { get; set; }

        // This doens't need to be unique
        public string DisplayName { get; set; }

        public List<AppUser> Users { get; set; }

        public bool Equals([AllowNull] AppRole other)
        {
            return other != null && other.Id == Id;
        }
    }
}
