using Neumont_Ticketing_System.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models
{
    public class TrackedString
    {
        public string Value { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public AppUser Author { get; set; }
    }
}
