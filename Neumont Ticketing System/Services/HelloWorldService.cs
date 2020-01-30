using MongoDB.Driver;
using Neumont_Ticketing_System.Models;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class HelloWorldService
    {
        private readonly IMongoCollection<HelloWorld> _helloWorld;

        public HelloWorldService(IHelloWorldDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _helloWorld = database.GetCollection<HelloWorld>(settings.HelloWorldCollectionName);
        }

        public List<HelloWorld> Get() =>
            _helloWorld.Find(helloWorld => true).ToList();

        public HelloWorld Get(string id) =>
            _helloWorld.Find<HelloWorld>(helloWorld => helloWorld.Id == id).FirstOrDefault();

        public HelloWorld Create(HelloWorld helloWorld)
        {
            _helloWorld.InsertOne(helloWorld);
            return helloWorld;
        }

        public void Update(string id, HelloWorld helloWorldIn) =>
            _helloWorld.ReplaceOne(helloWorld => helloWorld.Id == id, helloWorldIn);

        public void Remove(HelloWorld helloWorldIn) =>
            _helloWorld.DeleteOne(helloWorld => helloWorld.Id == helloWorldIn.Id);

        public void Remove(string id) =>
            _helloWorld.DeleteOne(helloWorld => helloWorld.Id == id);
    }
}
