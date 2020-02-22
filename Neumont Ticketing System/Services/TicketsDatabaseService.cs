using MongoDB.Driver;
using Neumont_Ticketing_System.Controllers.Exceptions;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Tickets;
using Neumont_Ticketing_System.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class TicketsDatabaseService
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IMongoCollection<RepairDefinition> _repairs;

        public TicketsDatabaseService(ITicketsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _tickets = database.GetCollection<Ticket>(settings.TicketsCollectionName);
            _repairs = database.GetCollection<RepairDefinition>(settings.RepairsCollectionName);
        }


        private void SetAllStepsNormalizedNames(List<RepairStep> steps)
        {
            foreach (var step in steps)
            {
                step.NormalizedName = step.Name.RemoveSpecialCharacters().ToUpper();
                SetAllStepsNormalizedNames(step.SubSteps);
            }
        }

        #region Read
        #region Tickets
        public Ticket GetTicketById(string id)
        {
            if (id == null || id.Length == 0)
                throw new ArgumentException("Given id cannot be null nor empty.");
            var matches = GetTickets(r => r.Id.Equals(id));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<Ticket>($"No ticket with a matching ID of \"{id}\" was found.");
        }
        public Ticket GetTicketByTicketId(int ticketId)
        {
            var matches = GetTickets(r => r.TicketId == ticketId);
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<Ticket>($"No ticket with a matching ticket id of " +
                    $"\"{ticketId}\" was found.");
        }

        public List<Ticket> GetTickets()
        {
            return GetTickets(ticket => true);
        }

        public List<Ticket> GetTickets(System.Linq.Expressions.Expression<Func<Ticket,
            bool>> expression,
            FindOptions options = null)
        {
            return _tickets.Find(expression, options).ToList();
        }
        #endregion Tickets

        #region Repairs
        public RepairDefinition GetRepairByName(string name)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentException("Given name cannot be null nor empty.");
            string normalizedName = CommonFunctions.NormalizeString(name);
            var matches = GetRepairs(r => r.NormalizedName.Equals(normalizedName));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<RepairDefinition>($"No repair with a name matching \"{name}\" was found.");
        }

        public RepairDefinition GetRepairById(string id)
        {
            if (id == null || id.Length == 0)
                throw new ArgumentException("Given id cannot be null nor empty.");
            var matches = GetRepairs(r => r.Id.Equals(id));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<RepairDefinition>($"No repair with a matching ID of \"{id}\" was found.");
        }

        public List<RepairDefinition> GetRepairs()
        {
            return GetRepairs(repair => true);
        }

        public List<RepairDefinition> GetRepairs(System.Linq.Expressions.Expression<Func<RepairDefinition,
            bool>> expression,
            FindOptions options = null)
        {
            return _repairs.Find(expression, options).ToList();
        }

        public List<RepairDefinition> GetApplicableRepairs(AssetModel model)
        {
            return _repairs.Find(r => 
                            // Ensure the repair applies to this model's type
                            (r.AppliesTo.TypeIds.Count == 0 ||
                                r.AppliesTo.TypeIds.Contains(model.TypeId)) &&
                            // Next, ensure the repair applies to this model's manufacturer
                            (r.AppliesTo.ManufacturerIds.Count == 0 ||
                                r.AppliesTo.ManufacturerIds.Contains(model.ManufacturerId)) &&
                            // Finally, ensure the repair applies to this model specifically
                            (r.AppliesTo.ModelIds.Count == 0 ||
                                r.AppliesTo.ModelIds.Contains(model.Id))).ToList();
        }
        #endregion Repairs
        #endregion Read

        #region Create
        #region Tickets
        public Ticket CreateTicket(Ticket ticket)
        {
            // https://stackoverflow.com/questions/32076382/mongodb-how-to-get-max-value-from-collections
            var queryResult = _tickets.Find(t => true, new FindOptions { BatchSize = 1 });
            if(queryResult.CountDocuments() > 0)
            {
                ticket.TicketId = queryResult.First().TicketId + 1;
            } else
            {   // If this is the first ticket in the database
                ticket.TicketId = 1;
            }

            if(ticket.Opened == null)
            {   // If this ticket's opened date hasn't been set
                ticket.Opened = DateTime.Now;
            }

            _tickets.InsertOne(ticket);
            return ticket;
        }
        #endregion Tickets

        #region Repairs
        public RepairDefinition CreateRepair(RepairDefinition repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            var matchedRepairs = GetRepairs(r => r.NormalizedName.Equals(repair.NormalizedName));
            if (matchedRepairs.Count > 0)
            {   // If another repair with the same normalized name is found, throw an exception
                throw new DuplicateException<RepairDefinition>(matchedRepairs);
            } else
            {
                SetAllStepsNormalizedNames(repair.Steps);
                _repairs.InsertOne(repair);
                return repair;
            }
        }
        #endregion Repairs
        #endregion Create

        #region Update
        #region Tickets
        public void UpdateTicket(Ticket ticket)
        {
            _tickets.ReplaceOne(u => u.Id == ticket.Id, ticket);
        }

        public void ReplaceTicket(string id, Ticket ticket)
        {
            _tickets.ReplaceOne(u => u.Id == id, ticket);
        }
        #endregion Tickets

        #region Repairs
        public void UpdateRepair(RepairDefinition repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            SetAllStepsNormalizedNames(repair.Steps);
            _repairs.ReplaceOne(u => u.Id == repair.Id, repair);
        }

        public void ReplaceRepair(string id, RepairDefinition repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            var matchedRepairs = GetRepairs(r => r.NormalizedName.Equals(repair.NormalizedName) 
                                                && !r.Id.Equals(id));
            if (matchedRepairs.Count > 0)
            {   // If another repair with the same normalized name is found THAT IS 
                // NOT THE ONE WE'RE REPLACING, throw an exception
                throw new DuplicateException<RepairDefinition>(matchedRepairs);
            }
            else
            {
                SetAllStepsNormalizedNames(repair.Steps);
                _repairs.ReplaceOne(u => u.Id == id, repair);
            }
        }
        #endregion Repairs
        #endregion Update

        #region Delete
        #region Tickets
        public void RemoveTicket(Ticket ticket)
        {
            _tickets.DeleteOne(u => u.Id == ticket.Id);
        }

        public void RemoveTicket(string id)
        {
            _tickets.DeleteOne(u => u.Id == id);
        }
        #endregion Tickets

        #region Repairs
        public void RemoveRepair(RepairDefinition repair)
        {
            _repairs.DeleteOne(u => u.Id == repair.Id);
        }

        public void RemoveRepair(string id)
        {
            _repairs.DeleteOne(u => u.Id == id);
        }
        #endregion Repairs
        #endregion Delete
    }
}
