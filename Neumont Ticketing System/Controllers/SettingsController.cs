using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Controllers.Exceptions;
using Neumont_Ticketing_System.Models;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Models.Tickets;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Services.Exceptions;
using Neumont_Ticketing_System.Views.Settings;

namespace Neumont_Ticketing_System.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        private readonly AssetsDatabaseService _assetDatabaseService;
        private readonly OwnersDatabaseService _ownersDatabaseService;
        private readonly TicketsDatabaseService _ticketsDatabaseService;

        public SettingsController(ILogger<SettingsController> logger,
            AssetsDatabaseService assetsDatabaseService,
            OwnersDatabaseService ownersDatabaseService,
            TicketsDatabaseService ticketsDatabaseService)
        {
            _logger = logger;

            _assetDatabaseService = assetsDatabaseService;
            _ownersDatabaseService = ownersDatabaseService;
            _ticketsDatabaseService = ticketsDatabaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region AssetCreator
        public IActionResult AssetCreator()
        {
            var model = new AssetCreatorModel {
                AssetModels = _assetDatabaseService.GetModels().ToList()
            };

            return View(model);
        }

        public IActionResult AssetEditor(string ownerId)
        {
            try
            {
                var model = new AssetCreatorModel
                {
                    AssetModels = _assetDatabaseService.GetModels().ToList(),
                    Owner = _ownersDatabaseService.GetOwnerById(ownerId),
                    OwnedAssets = _assetDatabaseService.GetAssetsByOwnerId(ownerId)
                };

                return View("AssetCreator", model);
            } catch(NotFoundException e)
            {
                _logger.LogError(e, "NotFoundException while loading asset editor page. " +
                    "Redirecting to asset creator.");
                return RedirectToAction("AssetCreator");
            } 
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
            } catch(DuplicateException<Asset> e)
            {
                _logger.LogError(e, "Error while attempting to save new owners/assets.");
                return new JsonResult(new
                {
                    Successful = false,
                    Message = $"A duplicate asset was found: Serial number: \"{e.Duplicate.SerialNumber}\", " +
                    $"Model name: \"{e.Duplicate.GetModel(_assetDatabaseService.GetModels()).Name}\"."
                });
            } catch(NotFoundException e)
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
                                throw new DuplicateException<Asset>(matchedAssets.First(), $"A duplicate asset with serial " +
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
                                throw new NotFoundException<Asset>(
                                    $"A model with name \"{asset.ModelName}\" was not " +
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
        #endregion AssetCreator

        #region AssetManager
        public IActionResult AssetManager()
        {
            return View(new AssetManagerModel(_assetDatabaseService.GetModels()));
        }

        private readonly string matchedOnSerialNumberString = "SerialNumber";
        private readonly string matchedOnOwnerNameString = "Name";
        private readonly string matchedOnOwnerPreferredNameString = "PreferredName";
        /* Results are "scored": 
         *  * Each serial number that contains the query string is worth 4 points
         *  * Each serial number that exactly matches the querty string is worth 6 points
         *  * Each normal owner name that contaians the query string is worth 3 points
         *  * Each normal owner name that exactly matches the query string is worth 6 points
         *  * Each part of the preferred name that contains the query string is worth 1 points
         *  * Each part of the preferred name that exactly matches the query string is worth 2 points
         *      * If an owner record doesn't contain a middle name, exact matches on other preferred
         *          name components will be worth 3 points instead of 2.
         *  * If both the normal owner name and the owner's preferred name produce points, only
         *      the one with the most points (owner name score or aggregate preferred name score)
         *      will count. The other will be discarded.
         *  * If both the serial number and the owner's name (normal or preferred) generated points,
         *      only the one with the most points (serial number or name) will count. The other will
         *      be discarded.
         * 
         */
        [HttpPost]
        public async Task<JsonResult> AssetManager([FromBody] AssetManagerQuery queryObject)
        {
            var result = Task.Run<JsonResult>(() =>
            {
                if(queryObject.Query == null || queryObject.Query == "")
                {
                    _logger.LogError("The given query was null or empty.");
                    return new JsonResult(new AssetManagerQueryResponse
                    {
                        Successful = false,
                        Message = "The query string was null or empty.",
                        Query = queryObject.Query,
                        Assets = new List<AssetManagerQueryResponseAsset>(0)
                    });
                } else
                {
                    try
                    {
                        string[] split = queryObject.Query.Split(' ');
                        // Normalize the individual words
                        List<string> words = new List<string>(split.Length);
                        string normalized;
                        for(int i = 0; i < split.Length; i++)
                        {
                            normalized = CommonFunctions.NormalizeString(split[i]);
                            if(normalized.Length > 0)
                                words.Add(normalized);
                        }
                        string normalizedQueryString = CommonFunctions.NormalizeString(queryObject.Query);
                        List<Asset> matchedAssets = _assetDatabaseService.GetAssets(a => a.NormalizedSerialNumber
                            .Contains(normalizedQueryString));


                        List<Owner> matchedOwners = new List<Owner>();
                        List<Owner> tempOwners = null;
                        foreach (string normalizedWord in words)
                        {
                            tempOwners = _ownersDatabaseService.GetOwners(o => o.NormalizedName.Contains(normalizedWord));
                            tempOwners.AddRange(_ownersDatabaseService.GetOwners(o => 
                                o.PreferredName.NormalizedFirst.Contains(normalizedWord)));
                            tempOwners.AddRange(_ownersDatabaseService.GetOwners(o => 
                                o.PreferredName.NormalizedMiddle.Contains(normalizedWord)));
                            tempOwners.AddRange(_ownersDatabaseService.GetOwners(o => 
                                o.PreferredName.NormalizedLast.Contains(normalizedWord)));
                            tempOwners.ForEach(o => {
                                if (!matchedOwners.Contains(o))
                                    matchedOwners.Add(o);
                            });
                        }

                        List<AssetManagerQueryResponseAsset> responseAssets = new List<AssetManagerQueryResponseAsset>();
                        Owner matchedOwner = null;
                        AssetModel matchedModel = null;
                        AssetType matchedType = null;
                        int score;
                        foreach (var asset in matchedAssets)
                        {
                            matchedOwner = _ownersDatabaseService.GetOwnerById(asset.OwnerId);
                            matchedModel = _assetDatabaseService.GetModelById(asset.ModelId);
                            matchedType = _assetDatabaseService.GetTypeById(matchedModel.TypeId);
                            if (asset.NormalizedSerialNumber.Equals(normalizedQueryString))
                            {
                                score = 6;
                            }
                            else
                            {
                                // We know the possible serial number has to be at least
                                // a part of this asset's serial number, otherwise it
                                // wouldn't be in matchedAssets
                                score = 4;
                            }
                            responseAssets.Add(new AssetManagerQueryResponseAsset
                            {
                                OwnerId = asset.OwnerId,
                                OwnerName = matchedOwner.Name,
                                OwnerPreferredName = matchedOwner.PreferredName,
                                AssetId = asset.Id,
                                AssetSerial = asset.SerialNumber,
                                AssetModelName = matchedModel.Name,
                                AssetTypeName = matchedType.Name,
                                Score = score,
                                MatchedOn = matchedOnSerialNumberString
                            });
                        }

                        int nameScore = 0, prefNameScore = 0;
                        string matchedOn = null;
                        AssetManagerQueryResponseAsset matchedResponseAsset;
                        List<string> possibleNames = null;
                        // A match can only be counted once per preferred name component
                        bool matchedFirst = false, matchedMiddle = false, matchedLast = false;
                        bool firstContains = false, middleContains = false, lastContains = false;
                        int firstLength = 0, middleLength = 0, lastLength = 0;
                        foreach (var owner in matchedOwners)
                        {
                            score = 0;
                            nameScore = 0;
                            prefNameScore = 0;

                            #region Calculate name match score
                            if (owner.NormalizedName.Equals(normalizedQueryString))
                            {
                                nameScore = 6;
                            }
                            else if (owner.NormalizedName.Contains(normalizedQueryString))
                            {
                                nameScore = 3;
                            }

                            possibleNames = words;

                            if(owner.PreferredName != null)
                            {
                                matchedFirst = false;
                                matchedMiddle = false;
                                matchedLast = false;
                                firstContains = false;
                                middleContains = false;
                                lastContains = false;
                                firstLength = 0;
                                middleLength = 0;
                                lastLength = 0;
                                for (int i = 0; i < possibleNames.Count; i++)
                                {
                                    string name = possibleNames[i];

                                    // First, check to see if this possible name exactly matches any of the preferred
                                    // name components
                                    if(!matchedFirst && name.Equals(owner.PreferredName.NormalizedFirst))
                                    {
                                        if (owner.PreferredName.NormalizedMiddle == null ||
                                            owner.PreferredName.NormalizedMiddle.Length == 0)
                                        {   // If the user's middle name isn't set, a matching preferred first
                                            // name is worth more
                                            prefNameScore += 3;
                                        } else
                                        {
                                            prefNameScore += 2;
                                        }
                                        matchedFirst = true;
                                    } else if(!matchedMiddle && name.Equals(owner.PreferredName.NormalizedMiddle))
                                    {
                                        prefNameScore += 2;
                                        matchedMiddle = true;
                                    } else if (!matchedLast && name.Equals(owner.PreferredName.NormalizedLast))
                                    {
                                        if (owner.PreferredName.NormalizedMiddle == null ||
                                            owner.PreferredName.NormalizedMiddle.Length == 0)
                                        {   // If the user's middle name isn't set, a matching preferred first
                                            // name is worth more
                                            prefNameScore += 3;
                                        }
                                        else
                                        {
                                            prefNameScore += 2;
                                        }
                                        matchedLast = true;
                                    } 
                                    // Failing an exact match, see if any of the components contain the possible name
                                    // at all
                                    else
                                    {
                                        firstContains = !matchedFirst &&
                                                            (owner.PreferredName.NormalizedFirst?.Contains(name) 
                                                                ?? false);
                                        if(firstContains)
                                        {
                                            firstLength = owner.PreferredName.NormalizedFirst?.Length ?? 0;
                                        }
                                        middleContains = !matchedMiddle &&
                                                            (owner.PreferredName.NormalizedMiddle?.Contains(name) 
                                                                ?? false);
                                        if(middleContains)
                                        {
                                            middleLength = owner.PreferredName.NormalizedMiddle?.Length ?? 0;
                                        }
                                        lastContains = !matchedLast &&
                                                            (owner.PreferredName.NormalizedLast?.Contains(name) 
                                                                ?? false);
                                        if(lastContains)
                                        {
                                            lastLength = owner.PreferredName.NormalizedLast?.Length ?? 0;
                                        }

                                        if (firstContains && firstLength > middleLength && 
                                            firstLength > lastLength)
                                        {
                                            // If the first name contains the possible name and is the shortest of
                                            // those that contain the possible name, it is the best match
                                            prefNameScore += 1;
                                            matchedFirst = true;
                                        } else if (middleContains && middleLength > firstLength && 
                                            middleLength > lastLength)
                                        {
                                            // If the middle name contains the possible name and is the shortest of
                                            // those that contain the possible name, it is the best match
                                            prefNameScore += 1;
                                            matchedMiddle = true;
                                        }
                                        else if(lastContains && lastLength > firstLength &&
                                            lastLength > middleLength)
                                        {
                                            // If the last name contains the possible name and is the shortest of
                                            // those that contain the possible name, it is the best match
                                            prefNameScore += 1;
                                            matchedLast = true;
                                        }
                                    }
                                }
                            }

                            // Now, assign the highest score as the master score value && note
                            // the "winner" in matchedOn
                            if (nameScore >= prefNameScore)
                            {
                                score = nameScore;
                                matchedOn = matchedOnOwnerNameString;
                            }
                            else
                            {
                                score = prefNameScore;
                                matchedOn = matchedOnOwnerPreferredNameString;
                            }
                            #endregion Calculate name match score

                            foreach (var asset in _assetDatabaseService.GetAssets(a => a.OwnerId.Equals(owner.Id)))
                            {   // Go through all of this owner's assets and add them to the match list

                                // Search to see if this asset was already found
                                matchedResponseAsset = null;
                                foreach (var responseAsset in responseAssets)
                                {
                                    if(responseAsset.AssetId.Equals(asset.Id))
                                    {
                                        matchedResponseAsset = responseAsset;
                                        break;
                                    }
                                }

                                if(matchedResponseAsset != null)
                                {
                                    if(matchedResponseAsset.MatchedOn.Equals(matchedOnSerialNumberString) &&
                                        matchedResponseAsset.Score < score)
                                    {   // If the matched response asset was found via serial number and
                                        // its score is less than the final name score, update its
                                        // Score and MatchedOn properties
                                        matchedResponseAsset.Score = score;
                                        matchedResponseAsset.MatchedOn = matchedOn;
                                    }
                                } else
                                {   // If a matching response asset wasn't found, then make a new one
                                    matchedModel = _assetDatabaseService.GetModelById(asset.ModelId);
                                    matchedType = _assetDatabaseService.GetTypeById(matchedModel.TypeId);

                                    responseAssets.Add(new AssetManagerQueryResponseAsset
                                    {
                                        OwnerId = asset.OwnerId,
                                        OwnerName = owner.Name,
                                        OwnerPreferredName = owner.PreferredName,
                                        AssetId = asset.Id,
                                        AssetSerial = asset.SerialNumber,
                                        AssetModelName = matchedModel.Name,
                                        AssetTypeName = matchedType.Name,
                                        Score = score,
                                        MatchedOn = matchedOn
                                    });
                                }
                            }
                        }

                        // Remove all entires with a score of 0
                        responseAssets.RemoveAll(a => a.Score <= 0);

                        // Sort by score
                        responseAssets.Sort((a, b) =>
                        {
                            if (a.Score < b.Score)
                                return 1;
                            else if (a.Score == b.Score)
                                return 0;
                            else
                                return -1;
                        });

                        // Trim to the requested result size
                        if (responseAssets.Count > queryObject.MaxNumOfResults)
                            responseAssets.RemoveRange(queryObject.MaxNumOfResults, 
                            responseAssets.Count - queryObject.MaxNumOfResults);

                        return new JsonResult(new AssetManagerQueryResponse
                        {
                            Successful = true,
                            Message = "Query completed normally.",
                            Query = queryObject.Query,
                            Assets = responseAssets
                        });
                    } catch(Exception e)
                    {
                        _logger.LogError(e, "Unexpected exception while trying to query database for assets and owners.");
                        return new JsonResult(new AssetManagerQueryResponse
                        {
                            Successful = false,
                            Message = "An unexpected internal error ocurred.",
                            Query = queryObject.Query,
                            Assets = new List<AssetManagerQueryResponseAsset>(0)
                        });
                    }
                }
            });

            return await result;
        }
        #endregion AssetManager

        #region AssetDefinitions
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
                    Successful = false,
                    Message = $"Input error: {e.Message}"
                });
            } catch(Exception e)
            {
                _logger.LogError(e, "Unexpected error while attemping to save asset definitions to database.");
                return new JsonResult(new
                {
                    Successful = false,
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
        #endregion AssetDefinitions

        #region RepairManagement
        public IActionResult NewRepairDefinition()
        {
            var model = new NewRepairDefinitionModel(_assetDatabaseService.GetTypes(),
                _assetDatabaseService.GetManufacturers(),
                _assetDatabaseService.GetModels());

            return View(model);
        }

        [HttpPost]
        public JsonResult NewRepairDefinition([FromBody] NewRepairData proposedRepair)
        {
            try
            {
                List<AssetType> types = _assetDatabaseService.GetTypesByNormalizedName(
                    proposedRepair.AppliesTo.TypeNames);
                List<string> typeIds = new List<string>();
                types.ForEach(type => typeIds.Add(type.Id));
                List<AssetManufacturer> mfrs = _assetDatabaseService.GetManufacturersByNormalizedName(
                    proposedRepair.AppliesTo.ManufacturerNames);
                List<string> mfrIds = new List<string>();
                mfrs.ForEach(mfr => mfrIds.Add(mfr.Id));
                List<AssetModel> models = _assetDatabaseService.GetModelsByNormalizedName(
                    proposedRepair.AppliesTo.ModelNames);
                List<string> modelIds = new List<string>();
                models.ForEach(model => modelIds.Add(model.Id));

                _ticketsDatabaseService.CreateRepairDefinition(new RepairDefinition
                {
                    Name = proposedRepair.Name,
                    Description = proposedRepair.Description,
                    Steps = proposedRepair.Steps,
                    AdditionalFieldNames = proposedRepair.AdditionalFields,
                    AppliesTo = new AppliesTo
                    {
                        TypeIds = typeIds,
                        ManufacturerIds = mfrIds,
                        ModelIds = modelIds
                    }
                });

                return new JsonResult(new NewRepairDataResponse
                {
                    Successful = true
                });
            }
            catch (DuplicateException<RepairDefinition> e)
            {
                _logger.LogError($"A duplicate repair with the name \"{e.Duplicate.Name}\" was found while " +
                        $"trying to create a new repair.");
                return new JsonResult(new NewRepairDataResponse
                {
                    Successful = false,
                    Message = $"A duplicate repair with the name \"{e.Duplicate.Name}\" was found."
                });
            }
            catch(Exception e) {
                _logger.LogError(e, "Unexpected exception while trying to create a new repair definition from HTTP POST.");
                return new JsonResult(new NewRepairDataResponse
                {
                    Successful = false,
                    Message = "Unexpected internal error."
                });
            }
        }
        #endregion RepairManagement
    }

    #region AssetManager
    public class AssetManagerQuery
    {
        public string Query { get; set; }
        public int MaxNumOfResults { get; set; } = 50;
    }

    public class AssetManagerQueryResponseAsset
    {
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public PreferredName OwnerPreferredName { get; set; }
        public string AssetId { get; set; }
        public string AssetSerial { get; set; }
        public string AssetModelName { get; set; }
        public string AssetTypeName { get; set; }
        public int Score { get; set; }
        public string MatchedOn { get; set; }
    }

    public class AssetManagerQueryResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string Query { get; set; }
        public List<AssetManagerQueryResponseAsset> Assets { get; set; }
    }
    #endregion AssetManager

    #region AssetCreator
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
    #endregion AssetCreator

    #region AssetDefinitions
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
    #endregion AssetDefinitions

    #region RepairManagement
    public class NewRepairData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RepairAppliesTo AppliesTo { get; set; }
        public List<string> AdditionalFields { get; set; }
        public List<RepairStep> Steps { get; set; }
    }

    public class NewRepairDataResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
    }

    public class RepairAppliesTo
    {
        public List<string> TypeNames { get; set; }
        public List<string> ManufacturerNames { get; set; }
        public List<string> ModelNames { get; set; }
    }
    #endregion RepairManagement
}