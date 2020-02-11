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

        public string NormalizedName { get; set; }

        public List<RepairStep> Steps { get; set; }

        public AppliesTo AppliesTo { get; set; }

        public bool DoesApplyTo(AssetType type)
        {
            return AppliesTo.DoesApplyTo(type);
        }

        public bool DoesApplyTo(AssetManufacturer manufacturer)
        {
            return AppliesTo.DoesApplyTo(manufacturer);
        }

        public bool DoesApplyTo(AssetModel model)
        {
            return AppliesTo.DoesApplyTo(model);
        }

        public bool Equals([AllowNull] Repair other)
        {
            return other != null && other.Id == Id;
        }
    }

    public class AppliesTo
    {
        [BsonElement("Types")]
        public List<string> TypeIds { get; set; }

        [BsonElement("Manufacturers")]
        public List<string> ManufacturerIds { get; set; }

        [BsonElement("Models")]
        public List<string> ModelIds { get; set; }

        public bool DoesApplyTo(AssetType type)
        {
            return TypeIds.Contains(type.Id);
        }

        public bool DoesApplyTo(AssetManufacturer mfr)
        {
            return ManufacturerIds.Contains(mfr.Id);
        }

        public bool DoesApplyTo(AssetModel model)
        {
            return ModelIds.Contains(model.Id);
        }
    }
}
