using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Tickets
{
    public class IndexModel
    {
        public List<TicketEntry> Tickets { get; set; }
    }

    public class TicketEntry
    {
        public string TicketId { get; set; }

        public string OwnerName { get; set; }

        public string OwnerPreferredName { get; set; }

        public string AssetSerial { get; set; }

        public string PrimaryLoanerName { get; set; }

        public DateTime DateOpened { get; set; }

        public string RepairName { get; set; }
    }
}
