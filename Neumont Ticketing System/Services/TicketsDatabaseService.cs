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
        private readonly IMongoCollection<Repair> _repairs;

        public TicketsDatabaseService(ITicketsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _tickets = database.GetCollection<Ticket>(settings.TicketsCollectionName);
            _repairs = database.GetCollection<Repair>(settings.RepairsCollectionName);
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
        public Repair GetRepairByName(string name)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentException("Given name cannot be null nor empty.");
            string normalizedName = CommonFunctions.NormalizeString(name);
            var matches = GetRepairs(r => r.NormalizedName.Equals(normalizedName));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<Repair>($"No repair with a name matching \"{name}\" was found.");
        }

        public Repair GetRepairById(string id)
        {
            if (id == null || id.Length == 0)
                throw new ArgumentException("Given id cannot be null nor empty.");
            var matches = GetRepairs(r => r.Id.Equals(id));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<Repair>($"No repair with a matching ID of \"{id}\" was found.");
        }

        public List<Repair> GetRepairs()
        {
            return GetRepairs(repair => true);
        }

        public List<Repair> GetRepairs(System.Linq.Expressions.Expression<Func<Repair,
            bool>> expression,
            FindOptions options = null)
        {
            return _repairs.Find(expression, options).ToList();
        }

        public List<Repair> GetApplicableRepairs(AssetModel model)
        {
            return _repairs.Find(r => 
                            // Ensure the repair applies to this model's type
                            (r.AppliesTo.TypeIds.Count == 0 ||
                                r.AppliesTo.TypeIds.Contains(model.TypeId)) &&
                            // Next, ensure the repair applies to this model's manufacturer
                            (r.AppliesTo.ManufacturerIds.Count == 0 ||
                                r.AppliesTo.ManufacturerIds.Contains(model.ManufacturerId) &&
                            // Finally, ensure the repair applies to this model specifically
                            (r.AppliesTo.ModelIds.Count == 0 ||
                                r.AppliesTo.ModelIds.Contains(model.Id)))).ToList();
        }
        #endregion Repairs
        #endregion Read

        #region Create
        #region Tickets
        public Ticket CreateTicket(Ticket ticket)
        {
            _tickets.InsertOne(ticket);
            return ticket;
        }
        #endregion Tickets

        #region Repairs
        public Repair CreateRepair(Repair repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            var matchedRepairs = GetRepairs(r => r.NormalizedName.Equals(repair.NormalizedName));
            if (matchedRepairs.Count > 0)
            {   // If another repair with the same normalized name is found, throw an exception
                throw new DuplicateException<Repair>(matchedRepairs);
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
        public void UpdateRepair(Repair repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            SetAllStepsNormalizedNames(repair.Steps);
            _repairs.ReplaceOne(u => u.Id == repair.Id, repair);
        }

        public void ReplaceRepair(string id, Repair repair)
        {
            repair.NormalizedName = CommonFunctions.NormalizeString(repair.Name);
            var matchedRepairs = GetRepairs(r => r.NormalizedName.Equals(repair.NormalizedName) 
                                                && !r.Id.Equals(id));
            if (matchedRepairs.Count > 0)
            {   // If another repair with the same normalized name is found THAT IS 
                // NOT THE ONE WE'RE REPLACING, throw an exception
                throw new DuplicateException<Repair>(matchedRepairs);
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
        public void RemoveRepair(Repair repair)
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
