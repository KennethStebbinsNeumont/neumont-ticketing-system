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

        #region Read
        #region Manufacturers
        public List<AssetManufacturer> GetManufacturers()
        {
            return GetManufacturers(manufacturer => true);
        }

        public List<AssetManufacturer> GetManufacturers(System.Linq.Expressions.Expression<Func<AssetManufacturer, 
            bool>> expression,
            FindOptions options = null)
        {
            return _manufacturers.Find(expression, options).ToList();
        }
        #endregion Manufacturers

        #region Types
        public List<AssetType> GetTypes()
        {
            return GetTypes(type => true);
        }

        public List<AssetType> GetTypes(System.Linq.Expressions.Expression<Func<AssetType,
            bool>> expression,
            FindOptions options = null)
        {
            return _types.Find(expression, options).ToList();
        }
        #endregion Types

        #region Models
        public List<AssetModel> GetModels()
        {
            return GetModels(model => true);
        }

        public List<AssetModel> GetModels(System.Linq.Expressions.Expression<Func<AssetModel,
            bool>> expression,
            FindOptions options = null)
        {
            return _models.Find(expression, options).ToList();
        }
        #endregion Models

        #region Assets
        public List<Asset> GetAssets()
        {
            return GetAssets(asset => true);
        }

        public List<Asset> GetAssets(System.Linq.Expressions.Expression<Func<Asset,
            bool>> expression,
            FindOptions options = null)
        {
            return _assets.Find(expression, options).ToList();
        }
        #endregion Assets
        #endregion Read

        #region Create
        #region Manufacturers
        public AssetManufacturer CreateManufacturer(AssetManufacturer manufacturer)
        {
            _manufacturers.InsertOne(manufacturer);
            return manufacturer;
        }
        #endregion Manufacturers


        #region Types
        public AssetType CreateType(AssetType type)
        {
            _types.InsertOne(type);
            return type;
        }
        #endregion Types


        #region Models
        public AssetModel CreateModel(AssetModel model)
        {
            _models.InsertOne(model);
            return model;
        }
        #endregion Models


        #region Assets
        public Asset Create(Asset asset)
        {
            _assets.InsertOne(asset);
            return asset;
        }
        #endregion Assets
        #endregion Create

        #region Update
        #region Manufacturers
        public void UpdateManufacturer(AssetManufacturer manufacturer)
        {
            _manufacturers.ReplaceOne(u => u.Id == manufacturer.Id, manufacturer);
        }

        public void ReplaceManufacturer(string id, AssetManufacturer manufacturer)
        {
            _manufacturers.ReplaceOne(u => u.Id == id, manufacturer);
        }
        #endregion Manufacturers


        #region Types
        public void UpdateType(AssetType type)
        {
            _types.ReplaceOne(u => u.Id == type.Id, type);
        }

        public void ReplaceType(string id, AssetType type)
        {
            _types.ReplaceOne(u => u.Id == id, type);
        }
        #endregion Types


        #region Models
        public void UpdateModel(AssetModel model)
        {
            _models.ReplaceOne(u => u.Id == model.Id, model);
        }

        public void ReplaceModel(string id, AssetModel model)
        {
            _models.ReplaceOne(u => u.Id == id, model);
        }
        #endregion Models


        #region Assets
        public void UpdateAsset(Asset asset)
        {
            _assets.ReplaceOne(u => u.Id == asset.Id, asset);
        }

        public void ReplaceAsset(string id, Asset asset)
        {
            _assets.ReplaceOne(u => u.Id == id, asset);
        }
        #endregion Assets
        #endregion Update

        #region Delete
        #region Manufacturers
        public void RemoveManufacturer(AssetManufacturer manufacturer)
        {
            _manufacturers.DeleteOne(u => u.Id == manufacturer.Id);
        }

        public void RemoveManufacturer(string id)
        {
            _manufacturers.DeleteOne(u => u.Id == id);
        }
        #endregion Manufacturers

        #region Types
        public void RemoveType(AssetType type)
        {
            _types.DeleteOne(u => u.Id == type.Id);
        }

        public void RemoveType(string id)
        {
            _types.DeleteOne(u => u.Id == id);
        }
        #endregion Types

        #region Models
        public void RemoveModel(AssetModel model)
        {
            _models.DeleteOne(u => u.Id == model.Id);
        }

        public void RemoveModel(string id)
        {
            _models.DeleteOne(u => u.Id == id);
        }
        #endregion Models

        #region Assets
        public void RemoveAsset(Asset asset)
        {
            _models.DeleteOne(u => u.Id == asset.Id);
        }

        public void RemoveAsset(string id)
        {
            _models.DeleteOne(u => u.Id == id);
        }
        #endregion Assets
        #endregion Delete
    }
}
