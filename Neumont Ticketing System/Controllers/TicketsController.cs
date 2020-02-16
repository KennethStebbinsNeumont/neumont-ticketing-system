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
        private readonly ILogger<SettingsController> _logger;

        private AppIdentityStorageService _appIdentityStorageService;

        public TicketsController(ILogger<SettingsController> logger,
            AppIdentityStorageService appIdentityStorageService)
        {
            _logger = logger;
            _appIdentityStorageService = appIdentityStorageService;
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
        public JsonResult GetApplicableRepairs([FromBody] Asset asset)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }
}