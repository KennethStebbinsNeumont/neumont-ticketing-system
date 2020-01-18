﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neumont_Ticketing_System.Models;
using Neumont_Ticketing_System.Services;

namespace Neumont_Ticketing_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly HelloWorldService _helloWorldService;

        public HomeController(ILogger<HomeController> logger, HelloWorldService helloWorldService)
        {
            _logger = logger;
            _helloWorldService = helloWorldService;
        }

        public IActionResult Index()
        {
            var listOfStuff = _helloWorldService.Get();
            return View(listOfStuff[0]);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
