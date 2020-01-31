using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Neumont_Ticketing_System.Models.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Tickets
{
    public class Repair : IEquatable<Repair>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public List<RepairStep> Steps { get; set; }

        [BsonElement("AppliesTo.Types")]
        public List<AssetType> AppliesToTypes { get; set; } = new List<AssetType>();

        [BsonElement("AppliesTo.Manufacturers")]
        public List<AssetManufacturer> AppliesToManufacturers { get; set; } =
            new List<AssetManufacturer>();

        [BsonElement("AppliesTo.Models")]
        public List<AssetModel> AppliesToModels { get; set; } = new List<AssetModel>();

        public bool AppliesTo(AssetType type)
        {
            return AppliesToTypes.Count == 0 || AppliesToTypes.Contains(type);
        }

        public bool AppliesTo(AssetManufacturer manufacturer)
        {
            return AppliesToManufacturers.Count == 0 || 
                AppliesToManufacturers.Contains(manufacturer);
        }

        public bool AppliesTo(AssetModel model)
        {
            return AppliesToModels.Count == 0 || AppliesToModels.Contains(model);
        }

        public bool Equals([AllowNull] Repair other)
        {
            return other != null && other.Id == Id;
        }
    }
}
