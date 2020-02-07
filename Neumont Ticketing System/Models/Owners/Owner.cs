using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Owners
{
    public class Owner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public List<string> EmailAddresses { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public PreferredName PreferredName { get; set; }

        public List<Asset> GetOwnedAssets(List<Asset> allAssets)
        {
            return GetOwnedAssets(this, allAssets);
        }

        public static List<Asset> GetOwnedAssets(Owner owner, List<Asset> allAssets)
        {
            List<Asset> result = new List<Asset>();
            foreach(var asset in allAssets)
            {
                if(owner.Id.Equals(asset.OwnerId))
                    result.Add(asset);
            }
            return result;
        }
    }
}
