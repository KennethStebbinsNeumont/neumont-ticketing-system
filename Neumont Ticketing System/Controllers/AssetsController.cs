using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Services.Exceptions;

namespace Neumont_Ticketing_System.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ILogger<AssetsController> _logger;

        private readonly AppIdentityStorageService _appIdentityStorageService;

        private readonly OwnersDatabaseService _ownersDatabaseService;

        private readonly AssetsDatabaseService _assetsDatabaseService;

        private readonly TicketsDatabaseService _ticketsDatabaseService;

        public AssetsController(ILogger<AssetsController> logger,
            AppIdentityStorageService appIdentityStorageService,
            OwnersDatabaseService ownersDatabaseService,
            AssetsDatabaseService assetsDatabaseService,
            TicketsDatabaseService ticketsDatabaseService)
        {
            _logger = logger;
            _appIdentityStorageService = appIdentityStorageService;
            _ownersDatabaseService = ownersDatabaseService;
            _assetsDatabaseService = assetsDatabaseService;
            _ticketsDatabaseService = ticketsDatabaseService;
        }

        private readonly string matchedOnOwnerNameString = "Name";
        private readonly string matchedOnOwnerPreferredNameString = "PreferredName";
        private readonly string matchedOnOwnerOwnerEmailString = "EmailAddress";

        [HttpPost]
        public JsonResult GetOwners([FromBody] GetOwnersRequest request)
        {
            if (request.Query == null || request.Query == "")
            {
                _logger.LogError("The given query was null or empty.");
                return new JsonResult(new GetOwnersResponse
                {
                    Successful = false,
                    Message = "The query string was null or empty.",
                    Query = request.Query,
                    Owners = new List<GetOwnersResponseOwner>(0)
                });
            }
            else
            {
                try
                {

                    List<GetOwnersResponseOwner> responseOwners = new List<GetOwnersResponseOwner>();
                    string[] split = request.Query.Split(' ');
                    // Normalize the individual words
                    List<string> words = new List<string>(split.Length);
                    string normalized;
                    for (int i = 0; i < split.Length; i++)
                    {
                        normalized = CommonFunctions.NormalizeString(split[i]);
                        if (normalized.Length > 0)
                            words.Add(normalized);
                    }
                    string normalizedQueryString = CommonFunctions.NormalizeString(request.Query);

                    // Start matchedOwners with all of the owners whose primary email address contains the query
                    // as-is
                    List<Owner> matchedOwners = _ownersDatabaseService.GetOwners(o => o.EmailAddresses.Count > 0 &&
                                                    o.EmailAddresses[0].Contains(request.Query));
                    List<Owner> tempOwners;
                    foreach (string normalizedWord in words)
                    {
                        tempOwners = _ownersDatabaseService.GetOwners(o => o.NormalizedName.Contains(normalizedWord));
                        tempOwners.AddRange(_ownersDatabaseService.GetOwners(o =>
                            o.PreferredName.NormalizedFirst.Contains(normalizedWord)));
                        tempOwners.AddRange(_ownersDatabaseService.GetOwners(o =>
                            o.PreferredName.NormalizedMiddle.Contains(normalizedWord)));
                        tempOwners.AddRange(_ownersDatabaseService.GetOwners(o =>
                            o.PreferredName.NormalizedLast.Contains(normalizedWord)));
                        tempOwners.ForEach(o =>
                        {
                            if (!matchedOwners.Contains(o))
                                matchedOwners.Add(o);
                        });
                    }

                    int score, nameScore = 0, prefNameScore = 0, emailScore = 0;
                    string matchedOn, primaryEmail, finalName;
                    List<string> possibleNames;
                    // A match can only be counted once per preferred name component
                    bool matchedFirst, matchedMiddle, matchedLast;
                    bool firstContains, middleContains, lastContains;
                    int firstLength, middleLength, lastLength;
                    foreach (var owner in matchedOwners)
                    {
                        score = 0;

                        #region Calculate match score
                        if (owner.NormalizedName.Equals(normalizedQueryString))
                        {
                            nameScore = 6;
                        }
                        else if (owner.NormalizedName.Contains(normalizedQueryString))
                        {
                            nameScore = 3;
                        }

                        if(owner.EmailAddresses.Count > 0)
                        {
                            primaryEmail = owner.EmailAddresses.First();
                            if(primaryEmail.Equals(request.Query) || 
                                primaryEmail.Split('@')[0].Equals(request.Query))
                            {
                                // If either the whole email or just the part before the domain
                                // exactly matches the query
                                emailScore = 6;
                            } else
                            {
                                emailScore = 3;
                            }
                        }

                        #region Preferred name score
                        possibleNames = words;
                        if (owner.PreferredName != null)
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
                                if (!matchedFirst && name.Equals(owner.PreferredName.NormalizedFirst))
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
                                    matchedFirst = true;
                                }
                                else if (!matchedMiddle && name.Equals(owner.PreferredName.NormalizedMiddle))
                                {
                                    prefNameScore += 2;
                                    matchedMiddle = true;
                                }
                                else if (!matchedLast && name.Equals(owner.PreferredName.NormalizedLast))
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
                                    if (firstContains)
                                    {
                                        firstLength = owner.PreferredName.NormalizedFirst?.Length ?? 0;
                                    }
                                    middleContains = !matchedMiddle &&
                                                        (owner.PreferredName.NormalizedMiddle?.Contains(name)
                                                            ?? false);
                                    if (middleContains)
                                    {
                                        middleLength = owner.PreferredName.NormalizedMiddle?.Length ?? 0;
                                    }
                                    lastContains = !matchedLast &&
                                                        (owner.PreferredName.NormalizedLast?.Contains(name)
                                                            ?? false);
                                    if (lastContains)
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
                                    }
                                    else if (middleContains && middleLength > firstLength &&
                                      middleLength > lastLength)
                                    {
                                        // If the middle name contains the possible name and is the shortest of
                                        // those that contain the possible name, it is the best match
                                        prefNameScore += 1;
                                        matchedMiddle = true;
                                    }
                                    else if (lastContains && lastLength > firstLength &&
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
                        #endregion Preferred name score

                        // Now, assign the highest score as the master score value && note
                        // the "winner" in matchedOn
                        if (nameScore >= prefNameScore && nameScore >= emailScore)
                        {
                            score = nameScore;
                            matchedOn = matchedOnOwnerNameString;
                            finalName = owner.Name;
                        }
                        else if(emailScore >= nameScore && emailScore >= prefNameScore)
                        {
                            score = emailScore;
                            matchedOn = matchedOnOwnerOwnerEmailString;
                            finalName = owner.Name;
                        } else
                        {
                            score = prefNameScore;
                            matchedOn = matchedOnOwnerPreferredNameString;
                            finalName = owner.PreferredName.ToString();
                        }
                        #endregion Calculate match score

                        responseOwners.Add(new GetOwnersResponseOwner
                        {
                            Id = owner.Id,
                            Name = finalName,
                            PrimaryEmail = owner.EmailAddresses.First(),
                            MatchedOn = matchedOn,
                            Score = score
                        });
                    }

                    // Sort by score
                    responseOwners.Sort((a, b) =>
                    {
                        if (a.Score < b.Score)
                            return -1;
                        else if (a.Score == b.Score)
                            return 0;
                        else
                            return 1;
                    });

                    // Trim to the requested result size, if necessary
                    if(responseOwners.Count > request.MaxResults)
                        responseOwners.RemoveRange(request.MaxResults, responseOwners.Count - 
                            request.MaxResults);

                    return new JsonResult(new GetOwnersResponse
                    {
                        Successful = true,
                        Message = "Query completed normally.",
                        Query = request.Query,
                        Owners = responseOwners
                    });
                } catch(Exception e)
                {
                    _logger.LogError(e, "Unexpected exception while trying to query database for owners.");
                    return new JsonResult(new GetOwnersResponse
                    {
                        Successful = false,
                        Message = "An unexpected internal error occurred.",
                        Query = request.Query,
                        Owners = new List<GetOwnersResponseOwner>(0)
                    });
                }
            }
        }

        [HttpPost]
        public JsonResult GetOwnedAssets([FromBody] GetOwnedAssetsRequest request)
        {
            if (request.OwnerId == null || request.OwnerId == "")
            {
                _logger.LogError("The given OwnerId was null or empty.");
                return new JsonResult(new GetOwnedAssetsResponse
                {
                    Successful = false,
                    Message = "The given OwnerId was null or empty.",
                    OwnerId = request.OwnerId,
                    Assets = new List<GetOwnedAssetsResponseAsset>(0)
                });
            } else
            {
                try
                {
                    var owner = _ownersDatabaseService.GetOwnerById(request.OwnerId);
                    var ownedAssets = _assetsDatabaseService.GetAssetsByOwnerId(owner.Id);
                    List<GetOwnedAssetsResponseAsset> responseAssets = 
                        new List<GetOwnedAssetsResponseAsset>(ownedAssets.Count);
                    string modelName;

                    foreach(var asset in ownedAssets)
                    {
                        modelName = _assetsDatabaseService.GetModelById(asset.ModelId).Name;
                        responseAssets.Add(new GetOwnedAssetsResponseAsset
                        {
                            Id = asset.Id,
                            SerialNumber = asset.SerialNumber,
                            ModelName = modelName
                        });
                    }

                    return new JsonResult(new GetOwnedAssetsResponse
                    {
                        Successful = true,
                        Message = "Query completed normally.",
                        OwnerId = request.OwnerId,
                        Assets = responseAssets
                    });

                } 
                catch(FormatException e)
                {
                    _logger.LogError($"The given owner id \"{request.OwnerId}\" is invalid.");
                    return new JsonResult(new GetOwnedAssetsResponse
                    {
                        Successful = false,
                        Message = "The given owner id is invalid.",
                        OwnerId = request.OwnerId,
                        Assets = new List<GetOwnedAssetsResponseAsset>(0)
                    });
                }
                catch(NotFoundException<Owner>)
                {
                    _logger.LogError($"No owner was found matching the id \"{request.OwnerId}\".");
                    return new JsonResult(new GetOwnedAssetsResponse
                    {
                        Successful = false,
                        Message = "No owner was found matching the given OwnerId.",
                        OwnerId = request.OwnerId,
                        Assets = new List<GetOwnedAssetsResponseAsset>(0)
                    });
                } catch(Exception e)
                {
                    _logger.LogError(e, $"Unexpected error while searching for assets beloning to " +
                        $"an owner with the id \"{request.OwnerId}\".");
                    return new JsonResult(new GetOwnedAssetsResponse
                    {
                        Successful = false,
                        Message = "An unexpected internal error occurred.",
                        OwnerId = request.OwnerId,
                        Assets = new List<GetOwnedAssetsResponseAsset>(0)
                    });
                }
            }
        }

        [HttpPost]
        public JsonResult GetApplicableRepairs([FromBody] GetApplicableRepairsRequest request)
        {
            if (request.AssetId == null || request.AssetId == "")
            {
                _logger.LogError("The given AssetId was null or empty.");
                return new JsonResult(new GetApplicableRepairsResponse
                {
                    Successful = false,
                    Message = "The given OwnerId was null or empty.",
                    AssetId = request.AssetId,
                    Repairs = new List<GetApplicableRepairsResponseRepair>(0)
                });
            } else
            {
                try
                {
                    var asset = _assetsDatabaseService.GetAssetById(request.AssetId);
                    var model = _assetsDatabaseService.GetModelById(asset.ModelId);
                    var allRepairs = _ticketsDatabaseService.GetRepairs();
                    var repairs = _ticketsDatabaseService.GetApplicableRepairs(model);
                    var responseRepairs = new List<GetApplicableRepairsResponseRepair>();
                    foreach(var repair in repairs)
                    {
                        responseRepairs.Add(new GetApplicableRepairsResponseRepair
                        {
                            Id = repair.Id,
                            Name = repair.Name,
                            Description = repair.Description,
                            AdditionalFieldNames = repair.AdditionalFieldNames
                        });
                    }

                    return new JsonResult(new GetApplicableRepairsResponse
                    {
                        Successful = true,
                        Message = "Query completed normally.",
                        AssetId = request.AssetId,
                        Repairs = responseRepairs
                    });

                } catch(NotFoundException<Asset>)
                {
                    _logger.LogError($"No asset with an ID of \"{request.AssetId}\" was found.");
                    return new JsonResult(new GetApplicableRepairsResponse
                    {
                        Successful = false,
                        Message = $"No asset with an ID of \"{request.AssetId}\" was found.",
                        AssetId = request.AssetId,
                        Repairs = new List<GetApplicableRepairsResponseRepair>(0)
                    });
                } catch(Exception e)
                {
                    _logger.LogError(e, $"Unexpected exception while attempting to find applicable repairs " +
                        $"for an asset with ID \"{request.AssetId}\"");
                    return new JsonResult(new GetApplicableRepairsResponse
                    {
                        Successful = false,
                        Message = "An unexpected internal error occurred.",
                        AssetId = request.AssetId,
                        Repairs = new List<GetApplicableRepairsResponseRepair>(0)
                    });
                }
            }
        }
    }

    #region GetOwners
    public class GetOwnersRequest
    {
        public string Query { get; set; }
        public int MaxResults { get; set; } = 10;
    }

    public class GetOwnersResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string Query { get; set; }
        public List<GetOwnersResponseOwner> Owners { get; set; }
    }

    public class GetOwnersResponseOwner
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PrimaryEmail { get; set; }
        public string MatchedOn { get; set; }
        public int Score { get; set; }
    }
    #endregion GetOwners

    #region GetOwnedAssets
    public class GetOwnedAssetsRequest
    {
        public string OwnerId { get; set; }
    }

    public class GetOwnedAssetsResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string OwnerId { get; set; }
        public List<GetOwnedAssetsResponseAsset> Assets { get; set; }
    }

    public class GetOwnedAssetsResponseAsset
    {
        public string Id { get; set; }
        public string SerialNumber { get; set; }
        public string ModelName { get; set; }
    }
    #endregion GetOwnedAssets

    #region GetApplicableRepairs
    public class GetApplicableRepairsRequest
    {
        public string AssetId { get; set; }
    }

    public class GetApplicableRepairsResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string AssetId { get; set; }
        public List<GetApplicableRepairsResponseRepair> Repairs { get; set; }
    }

    public class GetApplicableRepairsResponseRepair
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> AdditionalFieldNames { get; set; }
    }
    #endregion GetApplicableRepairs
}