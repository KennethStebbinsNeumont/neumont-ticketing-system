﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppRoleStore : IRoleStore<AppRole>, IQueryableRoleStore<AppRole>
    {

    }
}
