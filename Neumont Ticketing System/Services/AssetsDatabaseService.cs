using MongoDB.Driver;
using Neumont_Ticketing_System.Controllers.Exceptions;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Owners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class AssetsDatabaseService
    {
        private readonly IMongoCollection<AssetType> _types;
        private readonly IMongoCollection<AssetManufacturer> _manufacturers;
        private readonly IMongoCollection<AssetModel> _models;
        private readonly IMongoCollection<Asset> _assets;
        private readonly IMongoCollection<LoanerAsset> _loaners;

        public AssetsDatabaseService(IAssetsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _types = database.GetCollection<AssetType>(settings.TypesCollectionName);
            _manufacturers = database.GetCollection<AssetManufacturer>(settings.ManufacturersCollectionName);
            _models = database.GetCollection<AssetModel>(settings.ModelsCollectionName);
            _assets = database.GetCollection<Asset>(settings.AssetsCollectionName);
            _loaners = database.GetCollection<LoanerAsset>(settings.LoanersCollectionName);
        }

        #region Read
        #region Types
        public List<AssetType> GetTypes()
        {
            return GetTypes(type => true);
        }

        public AssetType GetTypeByName(string name)
        {
            string normalizedName = CommonFunctions.NormalizeString(name);
            var matches = GetTypes(r => r.NormalizedName.Equals(normalizedName));
            if (matches.Count > 0)
                return matches.First();
            else
                return null;
        }

        public List<AssetType> GetTypesByName(List<string> names)
        {
            List<string> normalizedNames = new List<string>();
            foreach(string name in names)
            {
                normalizedNames.Add(CommonFunctions.NormalizeString(name));
            }
            return GetTypes(t => normalizedNames.Contains(t.NormalizedName));
        }

        public AssetType GetTypeById(string id)
        {
            return _types.Find(t => t.Id.Equals(id)).First();
        }

        public List<AssetType> GetTypes(System.Linq.Expressions.Expression<Func<AssetType,
            bool>> expression,
            FindOptions options = null)
        {
            return _types.Find(expression, options).ToList();
        }
        #endregion Types

        #region Manufacturers
        public List<AssetManufacturer> GetManufacturers()
        {
            return GetManufacturers(manufacturer => true);
        }

        public AssetManufacturer GetManufacturerByName(string name)
        {
            string normalizedName = CommonFunctions.NormalizeString(name);
            var matches = GetManufacturers(r => r.NormalizedName.Equals(normalizedName));
            if (matches.Count > 0)
                return matches.First();
            else
                return null;
        }

        public List<AssetManufacturer> GetManufacturersByName(List<string> names)
        {
            List<string> normalizedNames = new List<string>();
            foreach (string name in names)
            {
                normalizedNames.Add(CommonFunctions.NormalizeString(name));
            }
            return GetManufacturers(m => normalizedNames.Contains(m.NormalizedName));
        }

        public AssetManufacturer GetManufacturerById(string id)
        {
            return _manufacturers.Find(m => m.Id.Equals(id)).First();
        }

        public List<AssetManufacturer> GetManufacturers(System.Linq.Expressions.Expression<Func<AssetManufacturer,
            bool>> expression,
            FindOptions options = null)
        {
            return _manufacturers.Find(expression, options).ToList();
        }
        #endregion Manufacturers

        #region Models
        public List<AssetModel> GetModels()
        {
            return GetModels(model => true);
        }

        public AssetModel GetModelByName(string name)
        {
            string normalizedName = CommonFunctions.NormalizeString(name);
            var matches = GetModels(m => m.NormalizedName.Equals(normalizedName));
            if (matches.Count > 0)
                return matches.First();
            else
                return null;
        }

        public AssetModel GetModelByModelNumber(string modelNumber)
        {
            string normalizedModelNumber = CommonFunctions.NormalizeString(modelNumber);
            var matches = GetModels(m => m.NormalizedModelNumber.Equals(normalizedModelNumber));
            if (matches.Count > 0)
                return matches.First();
            else
                return null;
        }

        public List<AssetModel> GetModelsByName(List<string> names)
        {
            List<string> normalizedNames = new List<string>();
            foreach (string name in names)
            {
                normalizedNames.Add(CommonFunctions.NormalizeString(name));
            }
            return GetModels(m => normalizedNames.Contains(m.NormalizedName));
        }

        public List<AssetModel> GetModelsByModelNumber(List<string> modelNumbers)
        {
            List<string> normalizedModelNumbers = new List<string>();
            foreach (string modelNumber in modelNumbers)
            {
                normalizedModelNumbers.Add(CommonFunctions.NormalizeString(modelNumber));
            }
            return GetModels(m => normalizedModelNumbers.Contains(m.NormalizedModelNumber));
        }

        public AssetModel GetModelById(string id)
        {
            return _models.Find(m => m.Id.Equals(id)).First();
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

        public Asset GetAssetById(string id)
        {
            return _assets.Find(a => a.Id.Equals(id)).First();
        }

        public List<Asset> GetAssetsByOwnerId(string ownerId)
        {
            return _assets.Find(a => a.OwnerId.Equals(ownerId)).ToList();
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
        #region Types
        public AssetType CreateType(AssetType type)
        {
            type.NormalizedName = CommonFunctions.NormalizeString(type.Name);
            var matchedTypes = GetTypes(t => t.NormalizedName.Equals(type.NormalizedName));
            if (matchedTypes.Count > 0)
            {
                // If we find another type with the same name
                throw new DuplicateException<AssetType>(matchedTypes);
            }
            else
            {
                _types.InsertOne(type);
                return type;
            }
        }
        #endregion Types


        #region Manufacturers
        public AssetManufacturer CreateManufacturer(AssetManufacturer manufacturer)
        {
            manufacturer.NormalizedName = CommonFunctions.NormalizeString(manufacturer.Name);
            var matchedMfrs = GetManufacturers(m => m.NormalizedName.Equals(manufacturer.NormalizedName));
            if (matchedMfrs.Count > 0)
            {
                // If we find another mfr with the same name
                throw new DuplicateException<AssetManufacturer>(matchedMfrs);
            }
            else
            {
                _manufacturers.InsertOne(manufacturer);
                return manufacturer;
            }
        }
        #endregion Manufacturers


        #region Models
        public AssetModel CreateModel(AssetModel model)
        {
            model.NormalizedName = CommonFunctions.NormalizeString(model.Name);
            model.NormalizedModelNumber = CommonFunctions.NormalizeString(model.ModelNumber);
            var matchedModels = GetModels(m => m.NormalizedName.Equals(model.NormalizedName) &&
                                                m.NormalizedModelNumber.Equals(model.NormalizedModelNumber));
            if (matchedModels.Count > 0)
            {
                // If we find another model with the same name & model number
                throw new DuplicateException<AssetModel>(matchedModels);
            } else {
            _models.InsertOne(model);
            return model;
            }
        }
        #endregion Models


        #region Assets
        public Asset Create(Asset asset)
        {
            asset.NormalizedSerialNumber = CommonFunctions.NormalizeString(asset.SerialNumber);
            var matchedAssets = GetAssets(a => a.NormalizedSerialNumber.Equals(asset.NormalizedSerialNumber));
            if (matchedAssets.Count > 0)
            {
                // If we find another asset with the same serial number
                throw new DuplicateException<Asset>(matchedAssets);
            }
            else
            {
                _assets.InsertOne(asset);
                return asset;
            }
        }
        #endregion Assets


        #region Loaners
        public LoanerAsset Create(LoanerAsset loaner)
        {
            loaner.NormalizedName = CommonFunctions.NormalizeString(loaner.Name);
            var matchedLoaners = GetLoaners(l => l.NormalizedName.Equals(loaner.NormalizedName));
            if(matchedLoaners.Count > 0)
            {
                // If we find another loaner with the same name
                throw new DuplicateException<LoanerAsset>(matchedLoaners);
            } else
            {
                _loaners.InsertOne(loaner);
                return loaner;
            }
        }
        #endregion Loaners
        #endregion Create

        #region Update
        #region Types
        public void UpdateType(AssetType type)
        {
            type.NormalizedName = CommonFunctions.NormalizeString(type.Name);
            _types.ReplaceOne(u => u.Id == type.Id, type);
        }

        public void ReplaceType(string id, AssetType type)
        {
            type.NormalizedName = CommonFunctions.NormalizeString(type.Name);
            var matchedTypes = GetTypes(t => t.NormalizedName.Equals(type.NormalizedName) &&
                                                !t.Id.Equals(id));
            if (matchedTypes.Count > 0)
            {
                // If we find another type with the same name THAT ISN'T THE ONE
                // WE'RE REPLACING
                throw new DuplicateException<AssetType>(matchedTypes);
            }
            else
            {
                _types.ReplaceOne(u => u.Id == id, type);
            }
        }
        #endregion Types


        #region Manufacturers
        public void UpdateManufacturer(AssetManufacturer manufacturer)
        {
            manufacturer.NormalizedName = CommonFunctions.NormalizeString(manufacturer.Name);
            _manufacturers.ReplaceOne(u => u.Id == manufacturer.Id, manufacturer);
        }

        public void ReplaceManufacturer(string id, AssetManufacturer manufacturer)
        {
            manufacturer.NormalizedName = CommonFunctions.NormalizeString(manufacturer.Name);
            var matchedMfrs = GetManufacturers(m => m.NormalizedName.Equals(manufacturer.NormalizedName) &&
                                                !m.Id.Equals(id));
            if (matchedMfrs.Count > 0)
            {
                // If we find another mfr with the same name THAT ISN'T THE ONE
                // WE'RE REPLACING
                throw new DuplicateException<AssetManufacturer>(matchedMfrs);
            }
            else
            {
                _manufacturers.ReplaceOne(u => u.Id == id, manufacturer);
            }
        }
        #endregion Manufacturers


        #region Models
        public void UpdateModel(AssetModel model)
        {
            model.NormalizedName = CommonFunctions.NormalizeString(model.Name);
            model.NormalizedModelNumber = CommonFunctions.NormalizeString(model.ModelNumber);
            _models.ReplaceOne(u => u.Id == model.Id, model);
        }

        public void ReplaceModel(string id, AssetModel model)
        {
            model.NormalizedName = CommonFunctions.NormalizeString(model.Name);
            model.NormalizedModelNumber = CommonFunctions.NormalizeString(model.ModelNumber);
            var matchedModels = GetModels(m => m.NormalizedName.Equals(model.NormalizedName) &&
                                                m.NormalizedModelNumber.Equals(model.NormalizedModelNumber) &&
                                                !m.Id.Equals(id));
            if (matchedModels.Count > 0)
            {
                // If we find another model with the same name & model number THAT ISN'T THE ONE
                // WE'RE REPLACING
                throw new DuplicateException<AssetModel>(matchedModels);
            }
            else
            {
                _models.ReplaceOne(u => u.Id == id, model);
            }
        }
        #endregion Models


        #region Assets
        public void UpdateAsset(Asset asset)
        {
            asset.NormalizedSerialNumber = CommonFunctions.NormalizeString(asset.SerialNumber);
            _assets.ReplaceOne(u => u.Id == asset.Id, asset);
        }

        public void ReplaceAsset(string id, Asset asset)
        {
            asset.NormalizedSerialNumber = CommonFunctions.NormalizeString(asset.SerialNumber);
            var matchedAssets = GetAssets(a => a.NormalizedSerialNumber.Equals(asset.NormalizedSerialNumber) &&
                                                a.ModelId.Equals(asset.ModelId) &&
                                                !a.Id.Equals(id));
            if(matchedAssets.Count > 0)
            {
                // If we find another asset with the same name & model THAT ISN'T THE ONE
                // WE'RE REPLACING
                throw new DuplicateException<Asset>(matchedAssets);
            } else
            {
                _assets.ReplaceOne(u => u.Id == id, asset);
            }
        }
        #endregion Assets


        #region Loaners
        public void UpdateLoaner(LoanerAsset loaner)
        {
            loaner.NormalizedName = CommonFunctions.NormalizeString(loaner.Name);
            _loaners.ReplaceOne(u => u.Id == loaner.Id, loaner);
        }

        public void ReplaceLoaner(string id, LoanerAsset loaner)
        {
            loaner.NormalizedName = CommonFunctions.NormalizeString(loaner.Name);
            var matchedLoaners = GetLoaners(l => l.NormalizedName.Equals(loaner.NormalizedName) &&
                                                !l.Id.Equals(id));
            if (matchedLoaners.Count > 0)
            {
                // If we find another loaner with the same name THAT ISN'T THE ONE
                // WE'RE REPLACING
                throw new DuplicateException<LoanerAsset>(matchedLoaners);
            }
            else
            {
                _loaners.ReplaceOne(u => u.Id == id, loaner);
            }
        }
        #endregion Loaners
        #endregion Update

        #region Delete
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
