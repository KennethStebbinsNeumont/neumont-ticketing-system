using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public string SecurityStamp { get; set; }

        public int FailedLoginAttempts { get; set; }

        public bool LockedOut { get; set; }
    }
}
