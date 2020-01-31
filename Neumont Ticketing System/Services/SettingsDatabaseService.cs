using MongoDB.Driver;
using Neumont_Ticketing_System.Models.DatabaseSettings;
using Neumont_Ticketing_System.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Services
{
    public class SettingsDatabaseService
    {
        private readonly IMongoCollection<Setting> _applicationSettings;

        public SettingsDatabaseService(ISettingsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _applicationSettings = database.GetCollection<Setting>(settings.ApplicationCollectionName);
        }

        #region Read
        #region Application
        public List<Setting> GetApplicationSettings()
        {
            return GetApplicationSettings(setting => true);
        }

        public List<Setting> GetApplicationSettings(System.Linq.Expressions.Expression<Func<Setting,
            bool>> expression,
            FindOptions options = null)
        {
            return _applicationSettings.Find(expression, options).ToList();
        }
        #endregion Application
        #endregion Read

        #region Create
        #region Application
        public Setting CreateApplicationSetting(Setting setting)
        {
            _applicationSettings.InsertOne(setting);
            return setting;
        }
        #endregion Application
        #endregion Create

        #region Update
        #region Application
        public void UpdateApplicationSetting(Setting setting)
        {
            _applicationSettings.ReplaceOne(u => u.Id == setting.Id, setting);
        }

        public void ReplaceApplicationSetting(string id, Setting setting)
        {
            _applicationSettings.ReplaceOne(u => u.Id == id, setting);
        }
        #endregion Application
        #endregion Update

        #region Delete
        #region Application
        public void RemoveApplicationSetting(Setting setting)
        {
            _applicationSettings.DeleteOne(u => u.Id == setting.Id);
        }

        public void RemoveApplicationSetting(string id)
        {
            _applicationSettings.DeleteOne(u => u.Id == id);
        }
        #endregion Application
        #endregion Delete
    }
}
