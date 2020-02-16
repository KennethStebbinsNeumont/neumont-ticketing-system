using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neumont_Ticketing_System.Models.Assets;

namespace Neumont_Ticketing_System.Controllers
{
    public class TicketsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewTicket()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetApplicableRepairs([FromBody] Asset asset)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }
}