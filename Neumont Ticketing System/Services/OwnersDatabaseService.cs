using MongoDB.Driver;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Services.Exceptions;
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
            var owners = _owners.Find(o => o.Id == id);
            if (owners.CountDocuments() > 0)
                return owners.First();
            else
                throw new NotFoundException<Owner>($"No owner with a matching ID of \"{id}\" was found.");
        }
        #endregion Read

        #region Create
        public Owner CreateOwner(Owner owner)
        {
            owner.NormalizedName = CommonFunctions.NormalizeString(owner.Name);
            if(owner.PreferredName != null)
            {
                if(owner.PreferredName.First != null)
                    owner.PreferredName.NormalizedFirst = 
                        CommonFunctions.NormalizeString(owner.PreferredName.First);
                if(owner.PreferredName.Middle != null)
                    owner.PreferredName.NormalizedMiddle = 
                        CommonFunctions.NormalizeString(owner.PreferredName.Middle);
                if(owner.PreferredName.Last != null)
                    owner.PreferredName.NormalizedLast = 
                        CommonFunctions.NormalizeString(owner.PreferredName.Last);
            }
            _owners.InsertOne(owner);
            return owner;
        }
        #endregion Create

        #region Create
        public void UpdateOwner(Owner owner)
        {
            owner.NormalizedName = CommonFunctions.NormalizeString(owner.Name);
            if (owner.PreferredName != null)
            {
                if (owner.PreferredName.First != null)
                    owner.PreferredName.NormalizedFirst =
                        CommonFunctions.NormalizeString(owner.PreferredName.First);
                if (owner.PreferredName.Middle != null)
                    owner.PreferredName.NormalizedMiddle =
                        CommonFunctions.NormalizeString(owner.PreferredName.Middle);
                if (owner.PreferredName.Last != null)
                    owner.PreferredName.NormalizedLast =
                        CommonFunctions.NormalizeString(owner.PreferredName.Last);
            }
            _owners.ReplaceOne(u => u.Id == owner.Id, owner);
        }

        public void ReplaceOwner(string id, Owner owner)
        {
            owner.NormalizedName = CommonFunctions.NormalizeString(owner.Name);
            if (owner.PreferredName != null)
            {
                if (owner.PreferredName.First != null)
                    owner.PreferredName.NormalizedFirst =
                        CommonFunctions.NormalizeString(owner.PreferredName.First);
                if (owner.PreferredName.Middle != null)
                    owner.PreferredName.NormalizedMiddle =
                        CommonFunctions.NormalizeString(owner.PreferredName.Middle);
                if (owner.PreferredName.Last != null)
                    owner.PreferredName.NormalizedLast =
                        CommonFunctions.NormalizeString(owner.PreferredName.Last);
            }
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
