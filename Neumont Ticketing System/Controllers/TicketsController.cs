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
            List<TicketEntry> ticketEntries = new List<TicketEntry>();

            List<Ticket> openTickets = _ticketsDatabaseService.GetTickets(t => t.Closed == null);

            Asset asset;
            Owner owner;
            RepairDefinition repairDefinition;
            TicketEntry ticketEntry;
            string preferredName;
            foreach(var ticket in openTickets)
            {
                asset = _assetsDatabaseService.GetAssetById(ticket.AssetId);
                owner = _ownersDatabaseService.GetOwnerById(asset.OwnerId);
                if(owner.PreferredName != null)
                {
                    preferredName = $"{owner.PreferredName.First} {$"{owner.PreferredName.Middle} " ?? ""}" +
                        $"{owner.PreferredName.Last}";
                } else
                {
                    preferredName = owner.Name;
                }
                repairDefinition = _ticketsDatabaseService.GetRepairDefinitionById(ticket.Repair.DefinitionId);

                ticketEntry = new TicketEntry
                {
                    TicketId = ticket.Id,
                    OwnerName = owner.Name,
                    AssetSerial = asset.SerialNumber,
                    DateOpened = ticket.Opened,
                    RepairName = repairDefinition.Name
                };

                if (ticket.LoanerIds.Count > 0)
                {
                    ticketEntry.PrimaryLoanerName = 
                        _assetsDatabaseService.GetLoanerById(ticket.LoanerIds[0]).Name;
                }

                ticketEntries.Add(ticketEntry);
            }

            ticketEntries.Sort((a, b) =>
            {
                if(a.DateOpened > b.DateOpened)
                {
                    return -1;
                } else if(a.DateOpened < b.DateOpened)
                {
                    return 1;
                } else
                {
                    return 0;
                }
            });

            IndexModel model = new IndexModel { Tickets = ticketEntries };

            return View(model);
        }

        public IActionResult NewTicket()
        {
            EditTicketModel model;
            try
            {
                var techs = _appIdentityStorageService.GetUsersByRole(
                    _appIdentityStorageService.GetRole(AppIdentityStorageService.Roles.Technicians));
                model = new EditTicketModel { 
                    Technicians = techs,
                    TitleText = "New Ticket",
                    SubtitleText = "Create a new ticket"
                };
            } catch(NotFoundException<AppRole> e)
            {
                _logger.LogWarning(e, "The technicians role was not found.");
                model = new EditTicketModel { 
                    Technicians = new List<AppUser>(),
                    TitleText = "New Ticket",
                    SubtitleText = "Create a new ticket"
                };
            }

            return View("EditTicket", model);
        }

        // When updating, only include the new comments
        [HttpPost]
        public JsonResult EditTicket([FromBody] EditTicketRequest request)
        {
            try
            {
                Owner matchedOwner = _ownersDatabaseService.GetOwnerById(request.OwnerId);
                Asset matchedAsset = _assetsDatabaseService.GetAssetById(request.AssetId);
                RepairDefinition matchedRepairDef = _ticketsDatabaseService.GetRepairDefinitionById(request.RepairId);
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

                if(request.TicketId == null)
                {
                    Ticket ticket = new Ticket
                    {
                        Title = request.Title,
                        AssetId = matchedAsset.Id,
                        Repair = new Repair
                        {
                            DefinitionId = matchedRepairDef.Id
                        },
                        AdditionalFields = request.AdditionalFields,
                        TechnicianIds = technicians,
                        LoanerIds = loaners,
                        Description = request.Description,
                        Comments = comments,
                        Opened = DateTime.Now
                    };

                    _ticketsDatabaseService.CreateTicket(ticket);

                    return new JsonResult(new EditTicketResponse
                    {
                        Successful = true,
                        Message = "Ticket created successfully.",
                        TicketId = ticket.TicketId
                    });
                } else
                {
                    Ticket matchedTicket = _ticketsDatabaseService
                                            .GetTicketById(request.TicketId);

                    matchedTicket.Title = request.Title;
                    if(!matchedRepairDef.Id.Equals(matchedTicket.Repair.DefinitionId))
                    {   // If the repair definiiton was changed
                        matchedTicket.Repair = new Repair { DefinitionId = matchedRepairDef.Id };
                    }
                    matchedTicket.AdditionalFields = request.AdditionalFields;
                    if(!matchedTicket.TechnicianIds.Contains(technician.Id))
                    {   // If the technician who updated this isn't already in the ticket's
                        // tech list
                        matchedTicket.TechnicianIds.Add(technician.Id);
                    }
                    matchedTicket.LoanerIds = loaners;
                    matchedTicket.Description = request.Description;
                    matchedTicket.Comments.AddRange(comments);

                    _ticketsDatabaseService.UpdateTicket(matchedTicket);

                    return new JsonResult(new EditTicketResponse
                    {
                        Successful = true,
                        Message = "Ticket updated successfully.",
                        TicketId = matchedTicket.TicketId
                    });
                }
            } 
            catch(NotFoundException<Owner>)
            {
                _logger.LogError($"Unable to find an owner with ID {request.OwnerId}");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given owner.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<Asset>)
            {
                _logger.LogError($"Unable to find an asset with ID {request.AssetId}");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given asset.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<RepairDefinition>)
            {
                _logger.LogError($"Unable to find an owner with ID {request.RepairId}");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given repair.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<AppUser>)
            {
                _logger.LogError($"Unable to find a user with ID {request.RepairId}");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find the given technician.",
                    TicketId = -1
                });
            }
            catch (NotFoundException<LoanerAsset> e)
            {
                _logger.LogError($"Unable to find a loaner specified: {e.Message}");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "Unable to find a loaner.",
                    TicketId = -1
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception while creating a new ticket.");
                return new JsonResult(new EditTicketResponse
                {
                    Successful = false,
                    Message = "An unexpected internal error occurred.",
                    TicketId = -1
                });
            }
        }
    }

    public class EditTicketRequest
    {
        public string TicketId { get; set; }
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

    public class EditTicketResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public int TicketId { get; set; }
    }

}