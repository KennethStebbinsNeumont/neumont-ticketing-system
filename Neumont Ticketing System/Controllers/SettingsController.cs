using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Controllers.Exceptions;
using Neumont_Ticketing_System.Models;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Views.Settings;

namespace Neumont_Ticketing_System.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        private readonly AssetsDatabaseService _assetDatabaseService;
        private readonly OwnersDatabaseService _ownersDatabaseService;

        public SettingsController(ILogger<SettingsController> logger,
            AssetsDatabaseService assetsDatabaseService,
            OwnersDatabaseService ownersDatabaseService)
        {
            _logger = logger;

            _assetDatabaseService = assetsDatabaseService;
            _ownersDatabaseService = ownersDatabaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetCreator()
        {
            var model = new AssetCreatorModel(_assetDatabaseService.GetModels().ToList(),
                _assetDatabaseService.GetAssets().ToList(),
                _ownersDatabaseService.GetOwners().ToList());

            return View(model);
        }

        // https://stackoverflow.com/questions/21578814/how-to-receive-json-as-an-mvc-5-action-method-parameter
        [HttpPost]
        public JsonResult AssetCreator([FromBody] AssetCreatorReturn returned)
        {
            try
            {
                SaveReturnAssetCreator(returned);

                _logger.LogInformation("Saved new owners/assets to database.");
                return new JsonResult(new
                {
                    Successful = true,
                    Message = "Owners and assets successfully saved."
                });
            } catch(DuplicateAssetException e)
            {
                _logger.LogError(e, "Error while attempting to save new owners/assets.");
                return new JsonResult(new
                {
                    Successful = false,
                    Message = $"A duplicate asset was found: Serial number: \"{e.Asset.SerialNumber}\", " +
                    $"Model name: \"{e.Asset.GetModel(_assetDatabaseService.GetModels()).Name}\"."
                });
            } catch(ModelNotFoundException e)
            {
                _logger.LogError(e, "Error while attempting to save new owners/assets.");
                return new JsonResult(new
                {
                    Successful = false,
                    Message = e.Message
                });
            } catch(Exception e)
            {
                _logger.LogError(e, "Unexpected error while attempting to save new owners/assets.");
                return new JsonResult(new
                {
                    Successful = false,
                    Message = "An unexpected internal error ocurred while trying to save owners " +
                    "and assets."
                });
            }
        }

        public IActionResult AssetDefinitions()
        {
            var assetDefModel = new AssetDefinitionsModel(_assetDatabaseService.GetTypes().ToList(),
                _assetDatabaseService.GetManufacturers().ToList(),
                _assetDatabaseService.GetModels().ToList());

            return View(assetDefModel);
        }

        // https://stackoverflow.com/questions/21578814/how-to-receive-json-as-an-mvc-5-action-method-parameter
        [HttpPost]
        public JsonResult AssetDefinitions([FromBody] AssetDefReturn returned)
        {
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

        private void SaveReturnAssetCreator(AssetCreatorReturn returned)
        {
            List<AssetModel> assetModels = _assetDatabaseService.GetModels();
            AssetModel matchedModel = null;
            List<Owner> newOwners = new List<Owner>();
            Owner newOwner = null;
            List<Asset> newAssets = new List<Asset>();
            List<Asset> matchedAssets = null;
            foreach(var owner in returned.owners)
            {
                if(owner.Name != null && owner.Name != "")
                {
                    newOwner = new Owner
                    {
                        Name = owner.Name,
                        PreferredName = owner.PreferredName,
                        EmailAddresses = owner.EmailAddresses,
                        PhoneNumbers = owner.PhoneNumbers
                    };

                    newOwner = _ownersDatabaseService.CreateOwner(newOwner);

                    foreach(var asset in owner.Assets)
                    {
                        matchedAssets = _assetDatabaseService.GetAssets(a => 
                            a.SerialNumber.Equals(asset.SerialNumber));

                        if(matchedAssets.Count > 0)
                        {
                            bool matchFound = false;
                            foreach(var a in matchedAssets)
                            {
                                if(a.GetModel(assetModels).Name.Equals(asset))
                                {
                                    matchFound = true;
                                    break;
                                }
                            }

                            if(matchFound)
                            {
                                // If we've found an already existing asset with the exact same serial number
                                // and model, throw an exception (cannot create duplicate assets)
                                throw new DuplicateAssetException(matchedAssets.First(), $"A duplicate asset with serial " +
                                    $"number \"{matchedAssets.First().SerialNumber}\" and model name \"" +
                                    $"{matchedAssets.First().GetModel(assetModels).Name}\" was found in the given assets " +
                                    $"to create.");
                            }
                        } else
                        {
                            matchedModel = null;
                            foreach(var model in assetModels)
                            {
                                if(model.Name.Equals(asset.ModelName))
                                {
                                    matchedModel = model;
                                    break;
                                }
                            }

                            // If a matching model wasn't found
                            if(matchedModel == null)
                            {
                                throw new ModelNotFoundException($"A model with name \"{asset.ModelName}\" was not " +
                                    $"found.");
                            } else
                            {
                                newAssets.Add(new Asset
                                {
                                    SerialNumber = asset.SerialNumber,
                                    ModelId = matchedModel.Id,
                                    OwnerId = newOwner.Id
                                });
                            }
                        }
                    }
                }
            }

            foreach(var asset in newAssets)
            {
                _assetDatabaseService.Create(asset);
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

    public class AssetCreatorReturnAsset
    {
        public string SerialNumber { get; set; }
        public string ModelName { get; set; }
    }

    public class AssetCreatorReturnOwner
    {
        public string Name { get; set; }
        public PreferredName PreferredName { get; set; }
        public List<string> EmailAddresses { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public List<AssetCreatorReturnAsset> Assets { get; set; }
    }

    public class AssetCreatorReturn
    {
        public List<AssetCreatorReturnOwner> owners { get; set; }
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