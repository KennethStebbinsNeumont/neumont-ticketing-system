using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Tickets
{
    public class Repair
    {
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string Description { get; set; }

        public List<RepairStep> Steps { get; set; }
    }
}
