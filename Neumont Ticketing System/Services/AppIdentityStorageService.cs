using MongoDB.Driver;
using Neumont_Ticketing_System.Areas.Identity.Data;
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

        public AppIdentityStorageService()
    }
}
