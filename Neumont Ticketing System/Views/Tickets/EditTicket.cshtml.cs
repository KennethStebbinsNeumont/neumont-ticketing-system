using Neumont_Ticketing_System.Areas.Identity.Data;
using Neumont_Ticketing_System.Models.Assets;
using Neumont_Ticketing_System.Models.Owners;
using Neumont_Ticketing_System.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Tickets
{
    public class EditTicketModel
    {
        public List<AppUser> Technicians { get; set; }

        public AppUser AssignedTechnician { get; set; }

        public string TitleText { get; set; } = "Edit Ticket";

        public string SubtitleText { get; set; } = "Edit a Ticket";

        public Ticket Ticket { get; set; }

        public List<CombinedComment> Comments { get; set; }

        public bool HasTicket
        { get
            {
                return Ticket != null;
            }
        }

        public List<LoanerAsset> Loaners { get; set; }

        public Owner Owner { get; set; }

        public Asset Asset { get; set; }

        public AssetModel AssetModel { get; set; }

        public RepairDefinition RepairDefinition { get; set; }
    }

    public class CombinedComment
    {
        public string Value { get; set; }

        public DateTime Timestamp { get; set; }

        public string AuthorName { get; set; }
    }
}
