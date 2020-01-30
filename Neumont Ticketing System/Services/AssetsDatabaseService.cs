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
            return GetManufacturers(user => true);
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
            return GetTypes(user => true);
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
            return GetModels(user => true);
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
            return GetAssets(user => true);
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
        public void UpdateManufacturer(AssetManufacturer user)
        {
            _manufacturers.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void ReplaceManufacturer(string id, AssetManufacturer user)
        {
            _manufacturers.ReplaceOne(u => u.Id == id, user);
        }
        #endregion Manufacturers


        #region Types
        public void UpdateType(AssetType user)
        {
            _types.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void ReplaceType(string id, AssetType user)
        {
            _types.ReplaceOne(u => u.Id == id, user);
        }
        #endregion Types


        #region Models
        public void UpdateModel(AssetModel user)
        {
            _models.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void ReplaceModel(string id, AssetModel user)
        {
            _models.ReplaceOne(u => u.Id == id, user);
        }
        #endregion Models


        #region Assets
        public void UpdateAsset(Asset user)
        {
            _assets.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void ReplaceAsset(string id, Asset user)
        {
            _assets.ReplaceOne(u => u.Id == id, user);
        }
        #endregion Assets
        #endregion Update

        #region Delete
        #region Manufacturers
        public void RemoveManufacturer(AssetManufacturer user)
        {
            _manufacturers.DeleteOne(u => u.Id == user.Id);
        }

        public void RemoveManufacturer(string id)
        {
            _manufacturers.DeleteOne(u => u.Id == id);
        }
        #endregion Manufacturers

        #region Types
        public void RemoveType(AssetType user)
        {
            _types.DeleteOne(u => u.Id == user.Id);
        }

        public void RemoveType(string id)
        {
            _types.DeleteOne(u => u.Id == id);
        }
        #endregion Types

        #region Models
        public void RemoveModel(AssetModel user)
        {
            _models.DeleteOne(u => u.Id == user.Id);
        }

        public void RemoveModel(string id)
        {
            _models.DeleteOne(u => u.Id == id);
        }
        #endregion Models

        #region Assets
        public void RemoveAsset(Asset user)
        {
            _models.DeleteOne(u => u.Id == user.Id);
        }

        public void RemoveAsset(string id)
        {
            _models.DeleteOne(u => u.Id == id);
        }
        #endregion Assets
        #endregion Delete
    }
}
