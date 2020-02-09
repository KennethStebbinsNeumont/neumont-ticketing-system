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
        private readonly IMongoCollection<LoanerAsset> _loaners;

        public AssetsDatabaseService(IAssetsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _manufacturers = database.GetCollection<AssetManufacturer>(settings.ManufacturersCollectionName);
            _types = database.GetCollection<AssetType>(settings.TypesCollectionName);
            _models = database.GetCollection<AssetModel>(settings.ModelsCollectionName);
            _assets = database.GetCollection<Asset>(settings.AssetsCollectionName);
            _loaners = database.GetCollection<LoanerAsset>(settings.LoanersCollectionName);
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

        public AssetManufacturer GetManufacturerById(string id)
        {
            return _manufacturers.Find(m => m.Id.Equals(id)).First();
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

        public AssetType GetTypeById(string id)
        {
            return _types.Find(t => t.Id.Equals(id)).First();
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

        public AssetModel GetModelById(string id)
        {
            return _models.Find(m => m.Id.Equals(id)).First();
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

        public Asset GetAssetById(string id)
        {
            return _assets.Find(a => a.Id.Equals(id)).First();
        }
        #endregion Assets

        #region Loaners
        public List<LoanerAsset> GetLoaners()
        {
            return GetLoaners(loaner => true);
        }

        public List<LoanerAsset> GetLoaners(System.Linq.Expressions.Expression<Func<LoanerAsset,
            bool>> expression,
            FindOptions options = null)
        {
            return _loaners.Find(expression, options).ToList();
        }

        public LoanerAsset GetLoanerById(string id)
        {
            return _loaners.Find(l => l.Id.Equals(id)).First();
        }
        #endregion Loaners
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
            asset.NormalizedSerialNumber = asset.SerialNumber.RemoveSpecialCharacters().ToUpper();
            _assets.InsertOne(asset);
            return asset;
        }
        #endregion Assets


        #region Loaners
        public LoanerAsset Create(LoanerAsset loaner)
        {
            _loaners.InsertOne(loaner);
            return loaner;
        }
        #endregion Loaners
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

        public void ReplaceManufacturer(System.Linq.Expressions.Expression<Func<AssetManufacturer, bool>> 
            expression, AssetManufacturer manufacturer)
        {
            _manufacturers.ReplaceOne(expression, manufacturer);
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

        public void ReplaceType(System.Linq.Expressions.Expression<Func<AssetType, bool>>
            expression, AssetType type)
        {
            _types.ReplaceOne(expression, type);
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

        public void ReplaceModel(System.Linq.Expressions.Expression<Func<AssetModel, bool>>
            expression, AssetModel model)
        {
            _models.ReplaceOne(expression, model);
        }
        #endregion Models


        #region Assets
        public void UpdateAsset(Asset asset)
        {
            asset.NormalizedSerialNumber = asset.SerialNumber.RemoveSpecialCharacters().ToUpper();
            _assets.ReplaceOne(u => u.Id == asset.Id, asset);
        }

        public void ReplaceAsset(string id, Asset asset)
        {
            asset.NormalizedSerialNumber = asset.SerialNumber.RemoveSpecialCharacters().ToUpper();
            _assets.ReplaceOne(u => u.Id == id, asset);
        }

        public void ReplaceAsset(System.Linq.Expressions.Expression<Func<Asset, bool>>
            expression, Asset asset)
        {
            asset.NormalizedSerialNumber = asset.SerialNumber.RemoveSpecialCharacters().ToUpper();
            _assets.ReplaceOne(expression, asset);
        }
        #endregion Assets


        #region Loaners
        public void UpdateLoaner(LoanerAsset loaner)
        {
            _loaners.ReplaceOne(u => u.Id == loaner.Id, loaner);
        }

        public void ReplaceLoaner(string id, LoanerAsset loaner)
        {
            _loaners.ReplaceOne(u => u.Id == id, loaner);
        }

        public void ReplaceLoanerAsset(System.Linq.Expressions.Expression<Func<LoanerAsset, bool>>
            expression, LoanerAsset loaner)
        {
            _loaners.ReplaceOne(expression, loaner);
        }
        #endregion Loaners
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

        public void RemoveManufacturers(System.Linq.Expressions.Expression<Func<AssetManufacturer, bool>> expression)
        {
            _manufacturers.DeleteMany(expression);
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

        public void RemoveTypes(System.Linq.Expressions.Expression<Func<AssetType, bool>> expression)
        {
            _types.DeleteMany(expression);
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

        public void RemoveModels(System.Linq.Expressions.Expression<Func<AssetModel, bool>> expression)
        {
            _models.DeleteMany(expression);
        }
        #endregion Models

        #region Assets
        public void RemoveAsset(Asset asset)
        {
            _assets.DeleteOne(u => u.Id == asset.Id);
        }

        public void RemoveAsset(string id)
        {
            _assets.DeleteOne(u => u.Id == id);
        }

        public void RemoveAssets(System.Linq.Expressions.Expression<Func<Asset, bool>> expression)
        {
            _assets.DeleteMany(expression);
        }
        #endregion Assets

        #region Loaners
        public void RemoveLoaner(LoanerAsset loaner)
        {
            _loaners.DeleteOne(u => u.Id == loaner.Id);
        }

        public void RemoveLoaner(string id)
        {
            _loaners.DeleteOne(u => u.Id == id);
        }

        public void RemoveLoaners(System.Linq.Expressions.Expression<Func<LoanerAsset, bool>> expression)
        {
            _loaners.DeleteMany(expression);
        }
        #endregion Loaners
        #endregion Delete
    }
}
