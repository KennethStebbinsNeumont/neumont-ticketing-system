using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class Asset : IEquatable<Asset>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SerialNumber { get; set; }

        [BsonElement("Model")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ModelId { get; set; }

        [BsonElement("Owner")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; }

        public bool Equals([AllowNull] Asset other)
        {
            return other != null && other.Id == Id;
        }

        public AssetModel GetModel(List<AssetModel> allModels)
        {
            return GetModel(this, allModels);
        }

        public static AssetModel GetModel(Asset asset, List<AssetModel> allModels)
        {
            foreach(var model in allModels)
            {
                if (model.Id.Equals(asset.ModelId))
                    return model;
            }
            throw new KeyNotFoundException($"A model with an ID matching the asset with serial number " +
                $"\"{asset.SerialNumber}\" was not found.");
        }
    }
}
