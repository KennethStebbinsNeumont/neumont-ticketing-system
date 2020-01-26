using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppUserStore : IUserPasswordStore<AppUser>, IUserSecurityStampStore<AppUser>, 
        IUserEmailStore<AppUser>, IQueryableUserStore<AppUser>, IUserLockoutStore<AppUser>
    {
        
    }
}
