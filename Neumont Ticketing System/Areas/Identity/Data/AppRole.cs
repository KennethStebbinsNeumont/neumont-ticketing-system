using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppRole
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; private set; }
        
        // This value must be unique
        public string Name { get; set; }

        // This doens't need to be unique
        public string DisplayName { get; set; }

        public List<AppUser> Users { get; private set; }
    }
}
