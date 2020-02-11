using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Models.Tickets
{
    public class RepairStep
    {
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public DateTime StartedDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public List<TrackedString> Comments { get; set; }

        public List<RepairStep> SubSteps { get; set; }

    }
}
