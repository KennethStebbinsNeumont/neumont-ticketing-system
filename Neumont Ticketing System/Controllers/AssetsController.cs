using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [HttpPost]
        public JsonResult GetOwners([FromBody] GetOwnersRequest request)
        {

        }
    }

    public class GetOwnersRequest
    {
        public string Query { get; set; }
        public int MaxResults { get; set; } = 10;
    }
}