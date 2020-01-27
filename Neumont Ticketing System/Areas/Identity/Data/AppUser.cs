using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppUser  : IdentityUser<string>, IEquatable<AppUser>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }

        public string Username { get; set; }

        public override string Email { get; set; }

        public override bool EmailConfirmed { get; set; }

        public string FullName { get; set; }

        public override string PasswordHash { get; set; }

        public override string SecurityStamp { get; set; }

        public override int AccessFailedCount { get; set; }

        public override bool LockoutEnabled { get; set; }

        public DateTime? LockoutEndDate { get; set; }

        public bool Equals([AllowNull] AppUser other)
        {
            return other != null && other.MongoId == MongoId;
        }
    }
}
