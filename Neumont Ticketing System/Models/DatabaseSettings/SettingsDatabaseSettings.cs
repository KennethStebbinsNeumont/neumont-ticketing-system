using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.DatabaseSettings
{
    public class SettingsDatabaseSettings : ISettingsDatabaseSettings
    {
        public string ApplicationCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ISettingsDatabaseSettings
    {
        string ApplicationCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
