using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Tickets;
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
            } catch(NotFoundException<AppRole> e)
            {
                _logger.LogWarning(e, "The technicians role was not found.");
                model = new NewTicketModel(new List<AppUser>());
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult NewTicket([FromBody] NewTicketRequest request)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }

    public class NewTicketRequest
    {
        public string OwnerId { get; set; }
        public string AssetId { get; set; }
        public string RepairId { get; set; }
        public string TechnicianId { get; set; }
        public List<string> LoanerIds { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<AdditionalField> AdditionalFields { get; set; }
        public List<string> Comments { get; set; }
    }

    public class NewTicketResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public int TicketId { get; set; }
    }

}