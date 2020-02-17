using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Services.Exceptions;
using Neumont_Ticketing_System.Views.Tickets;

namespace Neumont_Ticketing_System.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;

        private readonly TicketsDatabaseService _ticketsDatabaseService;

        private readonly AppIdentityStorageService _appIdentityStorageService;

        private readonly OwnersDatabaseService _ownersDatabaseService;

        private readonly AssetsDatabaseService _assetsDatabaseService;

        public TicketsController(ILogger<TicketsController> logger,
            TicketsDatabaseService ticketsDatabaseService,
            AppIdentityStorageService appIdentityStorageService,
            OwnersDatabaseService ownersDatabaseService,
            AssetsDatabaseService assetsDatabaseService)
        {
            _logger = logger;
            _ticketsDatabaseService = ticketsDatabaseService;
            _appIdentityStorageService = appIdentityStorageService;
            _ownersDatabaseService = ownersDatabaseService;
            _assetsDatabaseService = assetsDatabaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewTicket()
        {
            NewTicketModel model;
            try
            {
                var techs = _appIdentityStorageService.GetUsersByRole(
                    _appIdentityStorageService.GetRoleByName(
                        _appIdentityStorageService.techniciansRoleNormName));
                model = new NewTicketModel(techs);
            } catch(RoleNotFoundException e)
            {
                _logger.LogWarning(e, "The technicians role was not found.");
                model = new NewTicketModel(new List<AppUser>());
            }

            return View(model);
        }

        [HttpPost] 
        public JsonResult GetOwners([FromBody] GetOwnersRequest request)
        {

        }

        [HttpPost]
        public JsonResult GetApplicableRepairs([FromBody] Asset asset)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class GetOwnersRequest
    {
        public string Query { get; set; }
        public int MaxResults { get; set; } = 10;
    }
}