using MongoDB.Driver;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class AssetsDatabaseService
    {
        private readonly IMongoCollection<AssetManufacturer> _manufacturers;
        private readonly IMongoCollection<AssetType> _types;
        private readonly IMongoCollection<AssetModel> _models;
        private readonly IMongoCollection<Asset> _assets;

        public AssetsDatabaseService(IAssetsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _manufacturers = database.GetCollection<AssetManufacturer>(settings.ManufacturersCollectionName);
            _types = database.GetCollection<AssetType>(settings.TypesCollectionName);
            _models = database.GetCollection<AssetModel>(settings.ModelsCollectionName);
            _assets = database.GetCollection<Asset>(settings.AssetsCollectionName);
        }
    }
}
