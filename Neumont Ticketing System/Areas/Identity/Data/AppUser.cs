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
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public string SecurityStamp { get; set; }

        public int FailedLoginAttempts { get; set; }

        public bool LockedOut { get; set; }

        public DateTime? LockedOutUntil { get; set; }

        public bool Equals([AllowNull] AppUser other)
        {
            return other != null && other.Id == Id;
        }
    }
}
