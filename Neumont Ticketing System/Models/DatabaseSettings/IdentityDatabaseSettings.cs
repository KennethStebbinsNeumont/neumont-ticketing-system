using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.DatabaseSettings
{
    public class IdentityDatabaseSettings : IIdentityDatabaseSettings
    {
        public string UserCollectionName { get; set; }
        public string RoleCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IIdentityDatabaseSettings
    {
        string UserCollectionName { get; set; }
        string RoleCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
