using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppRole
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public List<AppUser> Users { get; private set; }
    }
}
