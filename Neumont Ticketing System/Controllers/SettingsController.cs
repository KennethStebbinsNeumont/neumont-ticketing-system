using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Views.Settings;

namespace Neumont_Ticketing_System.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        private readonly AssetsDatabaseService _assetDatabaseService;

        public SettingsController(ILogger<SettingsController> logger,
            AssetsDatabaseService assetsDatabaseService)
        {
            _logger = logger;

            _assetDatabaseService = assetsDatabaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetDef()
        {
            var assetDefModel = new AssetDefModel(_assetDatabaseService.GetTypes().ToList(),
                _assetDatabaseService.GetManufacturers().ToList(),
                _assetDatabaseService.GetModels().ToList());

            return View(assetDefModel);
        }

        // https://stackoverflow.com/questions/21578814/how-to-receive-json-as-an-mvc-5-action-method-parameter
        [HttpPost]
        public JsonResult AssetDef([FromBody] AssetDefReturn returned)
        {
            Console.WriteLine("We're in, boys!");
            try
            {
                SaveReturnAssetDefinitions(returned);
                return new JsonResult(new
                {
                    Successful = true,
                    Message = "Database successfully updated"
                });
            } catch(ArgumentException e)
            {
                _logger.LogError(e, "Argument exception when attempting to save asset definitions to database.");
                return new JsonResult(new
                {
                    Successful = true,
                    Message = $"Input error: {e.Message}"
                });
            } catch(Exception e)
            {
                _logger.LogError(e, "Unexpected error while attemping to save asset definitions to database.");
                return new JsonResult(new
                {
                    Successful = true,
                    Message = $"Unexpected error: {e.ToString()}"
                });
            }
        }

        private void SaveReturnAssetDefinitions(AssetDefReturn returned)
        {
            // Verify that the models defined in the returned data only use types and
            // manufacturers that will exist after the database is updated
            List<string> typeNames = new List<string>();
            returned.types.ForEach(type => typeNames.Add(type.Name));
            List<string> mfrNames = new List<string>();
            returned.manufacturers.ForEach(mfr => mfrNames.Add(mfr.Name));
            foreach (var model in returned.models)
            {
                if (!typeNames.Contains(model.TypeName))
                    throw new ArgumentException($"Model definition with name \"{model.Name}\" references an " +
                        $"unknown asset type: \"{model.TypeName}\"");

                if (!mfrNames.Contains(model.ManufacturerName))
                    throw new ArgumentException($"Model definition with name \"{model.Name}\" references an " +
                        $"unknown asset manufacturer: \"{model.ManufacturerName}\"");
            }

            // Prepare types
            List<AssetType> currentTypes = _assetDatabaseService.GetTypes();
            List<AssetType> updatedTypes = new List<AssetType>();
            List<AssetType> newTypes = new List<AssetType>();
            AssetType matchedType = null;
            foreach(var type in returned.types)
            {
                if(type.OriginalName != null && !type.OriginalName.Equals(""))
                {
                    matchedType = currentTypes.Find(t => t.Name.Equals(type.OriginalName));
                    matchedType.Name = type.Name;
                    matchedType.Description = type.Description;
                    updatedTypes.Add(matchedType);
                } else
                {
                    newTypes.Add(new AssetType
                    {
                        Name = type.Name,
                        Description = type.Description
                    });
                }
            }

            // Prepare manufacturers
            List<AssetManufacturer> currentMfrs = _assetDatabaseService.GetManufacturers();
            List<AssetManufacturer> updatedMfrs = new List<AssetManufacturer>();
            List<AssetManufacturer> newMfrs = new List<AssetManufacturer>();
            AssetManufacturer matchedMfr = null;
            foreach (var mfr in returned.manufacturers)
            {
                if (mfr.OriginalName != null && !mfr.OriginalName.Equals(""))
                {
                    matchedMfr = currentMfrs.Find(m => m.Name.Equals(mfr.OriginalName));
                    matchedMfr.Name = mfr.Name;
                    matchedMfr.EmailAddresses = mfr.EmailAddresses;
                    matchedMfr.PhoneNumbers = mfr.PhoneNumbers;
                    updatedMfrs.Add(matchedMfr);
                } else
                {
                    newMfrs.Add(new AssetManufacturer
                    {
                        Name = mfr.Name,
                        EmailAddresses = mfr.EmailAddresses,
                        PhoneNumbers = mfr.PhoneNumbers
                    });
                }
            }

            // Save types
            foreach (var type in updatedTypes)
            {
                _assetDatabaseService.UpdateType(type);
            }
            foreach (var type in newTypes)
            {
                _assetDatabaseService.CreateType(type);
            }
            // Remove all types that should no longer exist (weren't referenced in return data)
            _assetDatabaseService.RemoveTypes(type => !typeNames.Contains(type.Name));
            // Update currentTypes with the most recent records
            currentTypes = _assetDatabaseService.GetTypes();

            // Save manufacturers
            foreach (var mfr in updatedMfrs)
            {
                _assetDatabaseService.UpdateManufacturer(mfr);
            }
            foreach (var mfr in newMfrs)
            {
                _assetDatabaseService.CreateManufacturer(mfr);
            }
            // Remove all manufacturers that should no longer exist (weren't referenced in return data)
            _assetDatabaseService.RemoveManufacturers(mfr => !mfrNames.Contains(mfr.Name));
            // Update currentMfrs with the most recent records
            currentMfrs = _assetDatabaseService.GetManufacturers();

            // Prepare models
            List<AssetModel> currentModels = _assetDatabaseService.GetModels();
            List<AssetModel> updatedModels = new List<AssetModel>();
            List<string> modelNames = new List<string>();
            List<AssetModel> newModels = new List<AssetModel>();
            AssetModel matchedModel = null;
            foreach (var model in returned.models)
            {
                matchedType = currentTypes.Find(type => type.Name.Equals(model.TypeName));
                if(matchedType == null)
                {
                    throw new KeyNotFoundException($"Asset type with name \"{model.TypeName}\" was " +
                        $"unexpectedly missing.");
                }
                matchedMfr = currentMfrs.Find(mfr => mfr.Name.Equals(model.ManufacturerName));
                if(matchedMfr == null)
                {
                    throw new KeyNotFoundException($"Asset manufacturer with name \"{model.ManufacturerName}\" " +
                        $"was unexpectedly missing.");
                }
                if (model.OriginalName != null && !model.OriginalName.Equals(""))
                {
                    matchedModel = currentModels.Find(m => m.Name.Equals(model.OriginalName));
                    matchedModel.Name = model.Name;
                    matchedModel.ModelNumber = model.ModelNumber;
                    matchedModel.TypeId = matchedType.Id;
                    matchedModel.ManufacturerId = matchedMfr.Id;
                    updatedModels.Add(matchedModel);
                    modelNames.Add(model.Name);
                }
                else
                {
                    newModels.Add(new AssetModel
                    {
                        Name = model.Name,
                        ModelNumber = model.ModelNumber,
                        TypeId = matchedType.Id,
                        ManufacturerId = matchedMfr.Id
                    });
                    modelNames.Add(model.Name);
                }
            }

            // Save models
            foreach (var model in updatedModels)
            {
                _assetDatabaseService.UpdateModel(model);
            }
            foreach (var model in newModels)
            {
                _assetDatabaseService.CreateModel(model);
            }
            _assetDatabaseService.RemoveModels(model => !modelNames.Contains(model.Name));
        }
    }

    public class AssetDefReturnType
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Description { get; set; }
    }

    public class AssetDefReturnManufacturer
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public List<string> EmailAddresses { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }

    public class AssetDefReturnModel
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ModelNumber { get; set; }
        public string TypeName { get; set; }
        public string ManufacturerName { get; set; }
    }

    public class AssetDefReturn
    {
        public List<AssetDefReturnType> types { get; set; }
        public List<AssetDefReturnManufacturer> manufacturers { get; set; }
        public List<AssetDefReturnModel> models { get; set; }
    }
}