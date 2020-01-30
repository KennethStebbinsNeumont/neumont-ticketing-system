using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.DatabaseSettings
{
    public class HelloWorldDatabaseSettings : IHelloWorldDatabaseSettings
    {
        public string HelloWorldCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IHelloWorldDatabaseSettings
    {
        string HelloWorldCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
