using MongoDB.Driver;
using Neumont_Ticketing_System.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neumont_Ticketing_System.Services.Exceptions;
using Neumont_Ticketing_System.Models.DatabaseSettings;

namespace Neumont_Ticketing_System.Services
{
    public class AppIdentityStorageService
    {
        private readonly IMongoCollection<AppUser> _users;
        private readonly IMongoCollection<AppRole> _roles;

        public readonly string adminsRoleNormName = "ADMINISTRATORS";
        public readonly string techniciansRoleNormName = "TECHNICIANS";

        public AppIdentityStorageService(IIdentityDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<AppUser>(settings.UserCollectionName);
            _roles = database.GetCollection<AppRole>(settings.RoleCollectionName);
        }

        #region Read
        #region User operations
        public bool UsernameExists(string username)
        {
            var list = GetUsers(user => user.Username == username);
            return list.Count > 0;
        }

        public AppUser GetUserById(string id)
        {
            var matches = GetUsers(u => u.Id.Equals(id));
            if (matches.Count > 0)
                return matches[0];
            else
                throw new NotFoundException<AppUser>($"No app user with a matching ID of " +
                    $"\"{id}\" was found.");
        }

        public AppUser GetUserByUsername(string username)
        {
            var list = GetUsers(user => user.Username == username);
            if (list.Count > 0)
                return list[0];
            else
                throw new NotFoundException<AppUser>($"User with username {username} not found.");
        }

        public AppUser GetUserByEmail(string email)
        {
            var list = GetUsers(user => user.Email == email);
            if (list.Count > 0)
                return list[0];
            else
                throw new NotFoundException<AppUser>($"User with email address {email} not found.");
        }

        public IFindFluent<AppUser, AppUser> GetUsersQueryable()
        {
            return _users.Find(user => true);
        }

        public List<AppUser> GetUsersByRole(AppRole role)
        {
            return GetUsers(u => role.UserIds.Contains(u.Id));
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
        #endregion User operations

        #region Role operations
        public bool RoleNameExists(string name)
        {
            var list = GetRoles(role => role.Name == name);
            return list.Count > 0;
        }

        public AppRole GetRoleByName(string name)
        {
            var list = GetRoles(role => role.Name == name);
            if (list.Count > 0)
                return list[0];
            else
                throw new NotFoundException<AppRole>($"Role with name {name} not found.");
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

        public List<AppRole> GetUsersRoles(AppUser user)
        {
            return GetRoles(role => role.UserIds.Contains(user.Id));
        }
        #endregion Role operations
        #endregion Read


        #region Create
        #region User operations
        public AppUser CreateUser(AppUser user)
        {
            _users.InsertOne(user);
            return user;
        }
        #endregion User operations

        #region Role operations
        public AppRole CreateRole(AppRole role)
        {
            _roles.InsertOne(role);
            return role;
        }
        #endregion Role operations
        #endregion Create


        #region Update
        #region User operations
        public void UpdateUser(AppUser user)
        {
            _users.ReplaceOne(u => u.Id == user.Id, user);
        }

        public void ReplaceUser(string id, AppUser user)
        {
            _users.ReplaceOne(u => u.Id == id, user);
        }
        #endregion User operations

        #region Role operations
        public void UpdateRole(AppRole role)
        {
            _roles.ReplaceOne(r => r.Id == role.Id, role);
        }

        public void ReplaceRole(string id, AppRole role)
        {
            _roles.ReplaceOne(r => r.Id == id, role);
        }
        #endregion Role operations
        #endregion Update

        #region Delete
        #region User operations
        public void RemoveUser(AppUser user)
        {
            _users.DeleteOne(u => u.Id == user.Id);
        }

        public void RemoveUser(string id)
        {
            _users.DeleteOne(u => u.Id == id);
        }
        #endregion User operations

        #region Role operations
        public void RemoveRole(AppRole role)
        {
            _roles.DeleteOne(r => r.Id == role.Id);
        }

        public void RemoveRole(string id)
        {
            _roles.DeleteOne(r => r.Id == id);
        }
        #endregion Role operations
        #endregion Delete
    }
}
