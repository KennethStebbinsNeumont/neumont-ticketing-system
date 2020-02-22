using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
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
                    _appIdentityStorageService.GetRole(AppIdentityStorageService.Roles.Technicians));
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
            try
            {
                Owner matchedOwner = _ownersDatabaseService.GetOwnerById(request.OwnerId);
                Asset matchedAsset = _assetsDatabaseService.GetAssetById(request.AssetId);
                RepairDefinition matchedRepairDef = _ticketsDatabaseService.GetRepairById(request.RepairId);
                AppUser technician = _appIdentityStorageService.GetUserById(request.TechnicianId);
                
                var technicians = new List<string>();
                technicians.Add(technician.Id);

                List<string> loaners = new List<string>();
                LoanerAsset loaner;
                foreach(var id in request.LoanerIds)
                {
                    if(id != null && id.Length > 0)
                    {   // Ignore empty or null loaners
                        loaner = _assetsDatabaseService.GetLoanerById(id);
                        loaners.Add(loaner.Id);
                    }
                }

                List<TrackedString> comments = new List<TrackedString>();
                foreach(string comment in request.Comments)
                {
                    if(comment != null && comment.Length > 0)
                    {   // Ignore empty or null comments
                        comments.Add(new TrackedString
                        {
                            Value = comment,
                            AuthorId = request.TechnicianId
                        });
                    }
                }

                Ticket ticket = new Ticket
                {
                    Title = request.Title,
                    AssetId = matchedAsset.Id,
                    Repair = new Repair
                    {
                        DefinitionId = matchedRepairDef.Id
                    },
                    TechnicianIds = technicians,
                    LoanerIds = loaners,
                    Description = request.Description,
                    AdditionalFields = request.AdditionalFields,
                    Comments = comments
                };

                _ticketsDatabaseService.CreateTicket(ticket);

                return new JsonResult(new NewTicketResponse
                {
                    Successful = true,
                    Message = "Query completed normally.",
                    TicketId = ticket.TicketId
                });
            } 
            catch(NotFoundException<Owner>)
            {
                _logger.LogError($"Unable to find an owner with ID {request.OwnerId}");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given owner.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<Asset>)
            {
                _logger.LogError($"Unable to find an asset with ID {request.AssetId}");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given asset.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<RepairDefinition>)
            {
                _logger.LogError($"Unable to find an owner with ID {request.RepairId}");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given repair.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<AppUser>)
            {
                _logger.LogError($"Unable to find a user with ID {request.RepairId}");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given technician.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<LoanerAsset> e)
            {
                _logger.LogError($"Unable to find a loaner specified: {e.Message}");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find a loaner.",
                    TicketId = -1
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception while creating a new ticket.");
                return new JsonResult(new NewTicketResponse
                {
                    Successful = false,
                    Message = "An unexpected internal error occurred.",
                    TicketId = -1
                });
            }
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