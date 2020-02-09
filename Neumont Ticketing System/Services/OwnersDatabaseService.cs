using MongoDB.Driver;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Owners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class OwnersDatabaseService
    {
        private readonly IMongoCollection<Owner> _owners;

        public OwnersDatabaseService(IOwnersDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _owners = database.GetCollection<Owner>(settings.OwnersCollectionName);
        }

        #region Read
        public List<Owner> GetOwners()
        {
            return GetOwners(owner => true);
        }

        public List<Owner> GetOwners(System.Linq.Expressions.Expression<Func<Owner,
            bool>> expression,
            FindOptions options = null)
        {
            return _owners.Find(expression, options).ToList();
        }

        public Owner GetOwnerById(string id)
        {
            return _owners.Find(o => o.Id.Equals(id)).First();
        }
        #endregion Read

        #region Create
        public Owner CreateOwner(Owner owner)
        {
            _owners.InsertOne(owner);
            return owner;
        }
        #endregion Create

        #region Create
        public void UpdateOwner(Owner owner)
        {
            _owners.ReplaceOne(u => u.Id == owner.Id, owner);
        }

        public void ReplaceOwner(string id, Owner owner)
        {
            _owners.ReplaceOne(u => u.Id == id, owner);
        }
        #endregion Create

        #region Delete
        public void RemoveOwner(Owner owner)
        {
            _owners.DeleteOne(u => u.Id == owner.Id);
        }

        public void RemoveOwner(string id)
        {
            _owners.DeleteOne(u => u.Id == id);
        }
        #endregion Delete
    }
}
