using MongoDB.Driver;
using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class AppIdentityStorageService
    {
        private readonly IMongoCollection<AppUser> _users;
        private readonly IMongoCollection<AppRole> _roles;

        public AppIdentityStorageService(IIdentityDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<AppUser>(settings.UserCollectionName);
            _roles = database.GetCollection<AppRole>(settings.RoleCollectionName);
        }

        public AppUser GetUserByUsername(string username)
        {
            var list = GetUsers(user => user.Username == username);
            if (list.Count > 0)
                return list[0];
            else
                throw NotFou
        }

        public List<AppUser> GetUsers()
        {
            return GetUsers(user => true);
        }

        public List<AppUser> GetUsers(System.Linq.Expressions.Expression<Func<AppUser, bool>> expression, 
            FindOptions options = null)
        {
            return _users.Find(expression, options).ToList();
        }

        public List<AppRole> GetRoles()
        {
            return GetRoles(role => true);
        }

        public List<AppRole> GetRoles(System.Linq.Expressions.Expression<Func<AppRole, bool>> expression,
            FindOptions options = null)
        {
            return _roles.Find(expression, options).ToList();
        }

        pu
    }
}
