using MongoDB.Driver;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Tickets;
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

        #region Read
        #region Tickets
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
            _repairs.InsertOne(repair);
            return repair;
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
            _repairs.ReplaceOne(u => u.Id == repair.Id, repair);
        }

        public void ReplaceRepair(string id, Repair repair)
        {
            _repairs.ReplaceOne(u => u.Id == id, repair);
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
