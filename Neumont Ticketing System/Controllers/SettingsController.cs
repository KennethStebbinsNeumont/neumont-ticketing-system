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

        [HttpPost]
        public IActionResult AssetDef(AssetDefReturn returned)
        {
            Console.WriteLine("We're in, boys!");
            return AssetDef();
        }
    }

    public class AssetDefReturnModel
    {
        public string Name { get; set; }
        public string ModelNumber { get; set; }
        public string TypeName { get; set; }
        public string ManufacturerName { get; set; }
    }

    public class AssetDefReturn
    {
        public List<AssetType> types { get; set; }
        public List<AssetManufacturer> manufacturers { get; set; }
        public List<AssetDefReturnModel> models { get; set; }
    }
}