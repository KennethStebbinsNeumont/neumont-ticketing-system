using Microsoft.AspNetCore.Mvc.RazorPages;
using Neumont_Ticketing_System.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Views.Shared
{
    public partial class LoginPartialModel : PageModel
    {
        private readonly AppUserManager _userManager;
        private readonly AppSignInManager _signInManager;

        public LoginPartialModel(
            AppUserManager userManager,
            AppSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public AppUser LoggedInUser { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        private async Task LoadAsync()
        {
            LoggedInUser = await _userManager.FindByNameAsync(User.Identity.Name);

            Username = LoggedInUser.Username;
            FullName = LoggedInUser.FullName;
        }
    }
}
