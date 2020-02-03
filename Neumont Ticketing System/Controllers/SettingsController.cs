using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Services;
using Neumont_Ticketing_System.Views.Settings;

namespace Neumont_Ticketing_System.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<HomeController> _logger;

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
    }
}