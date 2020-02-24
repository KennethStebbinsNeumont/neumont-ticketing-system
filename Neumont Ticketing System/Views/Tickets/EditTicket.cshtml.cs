﻿using Neumont_Ticketing_System.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Tickets
{
    public class NewTicketModel
    {
        public List<AppUser> Technicians { get; set; }

        public string TitleText { get; set; }

        public string SubtitleText { get; set; }

        public string TicketId { get; set; }
    }
}
