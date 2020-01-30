using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.DatabaseSettings
{
    public class AssetsDatabaseSettings : IAssetsDatabaseSettings
    {
        public string ManufacturersCollectionName { get; set; }
        public string TypesCollectionName { get; set; }
        public string ModelsCollectionName { get; set; }
        public string AssetsCollectionName { get; set; }
        public string LoanersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IAssetsDatabaseSettings
    {
        string ManufacturersCollectionName { get; set; }
        string TypesCollectionName { get; set; }
        string ModelsCollectionName { get; set; }
        string AssetsCollectionName { get; set; }
        string LoanersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
