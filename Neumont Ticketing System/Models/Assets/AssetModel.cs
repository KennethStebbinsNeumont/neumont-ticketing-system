﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Assets
{
    public class AssetModel : IEquatable<AssetModel>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public AssetType Type { get; set; }

        public string ModelNumber { get; set; }

        public AssetManufacturer Manufacturer { get; set; }

        public bool Equals([AllowNull] AssetModel other)
        {
            return other != null && other.Id == Id;
        }
    }
}
