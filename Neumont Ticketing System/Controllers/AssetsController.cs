using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Services;

namespace Neumont_Ticketing_System.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ILogger<AssetsController> _logger;

        private readonly AppIdentityStorageService _appIdentityStorageService;

        private readonly OwnersDatabaseService _ownersDatabaseService;

        private readonly AssetsDatabaseService _assetsDatabaseService;

        public AssetsController(ILogger<AssetsController> logger,
            AppIdentityStorageService appIdentityStorageService,
            OwnersDatabaseService ownersDatabaseService,
            AssetsDatabaseService assetsDatabaseService)
        {
            _logger = logger;
            _appIdentityStorageService = appIdentityStorageService;
            _ownersDatabaseService = ownersDatabaseService;
            _assetsDatabaseService = assetsDatabaseService;
        }

        private readonly string matchedOnOwnerNameString = "Name";
        private readonly string matchedOnOwnerPreferredNameString = "PreferredName";
        private readonly string matchedOnOwnerOwnerEmailString = "EmailAddress";

        [HttpPost]
        public async Task<JsonResult> GetOwners([FromBody] GetOwnersRequest request)
        {
            if (request.Query == null || request.Query == "")
            {
                _logger.LogError("The given query was null or empty.");
                return new JsonResult(new GetOwnersResponse
                {
                    Successful = false,
                    Message = "The query string was null or empty.",
                    Query = request.Query,
                    Owners = new List<GetOwnersResponseOwner>()
                });
            }
            else
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
                    tempOwners.ForEach(o =>
                    {
                        if (!matchedOwners.Contains(o))
                            matchedOwners.Add(o);
                    });
                }

                int score = 0, nameScore = 0, prefNameScore = 0;
                string matchedOn = null;
                List<string> possibleNames = null;
                // A match can only be counted once per preferred name component
                bool matchedFirst = false, matchedMiddle = false, matchedLast = false;
                bool firstContains = false, middleContains = false, lastContains = false;
                int firstLength = 0, middleLength = 0, lastLength = 0;
                foreach (var owner in matchedOwners)
                {
                    score = 0;

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

                    responseOwners.Add(new GetOwnersResponseOwner
                    {
                        Id = owner.Id,
                        Name = owner.Name,
                        PrimaryEmail = owner.EmailAddresses.First(),
                        MatchedOn = matchedOn,
                        Score = score
                    });
                }
            }
        }
    }

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
}