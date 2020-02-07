using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Views.Settings;

namespace Neumont_Ticketing_System.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;

        private readonly AssetsDatabaseService _assetDatabaseService;

        public SettingsController(ILogger<SettingsController> logger,
            AssetsDatabaseService assetsDatabaseService)
        {
            _logger = logger;

            _assetDatabaseService = assetsDatabaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetDef()
        {
            var assetDefModel = new AssetDefModel(_assetDatabaseService.GetTypes().ToList(),
                _assetDatabaseService.GetManufacturers().ToList(),
                _assetDatabaseService.GetModels().ToList());

            return View(assetDefModel);
        }

        // https://stackoverflow.com/questions/21578814/how-to-receive-json-as-an-mvc-5-action-method-parameter
        [HttpPost]
        public IActionResult AssetDef([FromBody] AssetDefReturn returned)
        {
            Console.WriteLine("We're in, boys!");
            return AssetDef();
        }

        private void SaveReturnAssetDefinitions(AssetDefReturn returned)
        {
            // Prepare types
            var currentTypes = _assetDatabaseService.GetTypes();
        }
    }

    public class AssetDefReturnType
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Description { get; set; }
    }

    public class AssetDefReturnManufacturer
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public List<string> EmailAddresses { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }

    public class AssetDefReturnModel
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ModelNumber { get; set; }
        public string TypeName { get; set; }
        public string ManufacturerName { get; set; }
    }

    public class AssetDefReturn
    {
        public List<AssetDefReturnType> types { get; set; }
        public List<AssetDefReturnManufacturer> manufacturers { get; set; }
        public List<AssetDefReturnModel> models { get; set; }
    }
}