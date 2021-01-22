using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Identity.Pages.Account.Manage
{
    public class TmpShowUserDetailsModel : PageModel
    {
        private readonly UserManager<SiteUser> _userManager;
        private readonly SignInManager<SiteUser> _signInManager;

        public SiteUser CurrentUser { get; set; }

        public TmpShowUserDetailsModel(UserManager<SiteUser> userManager, SignInManager<SiteUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            /*
            ViewData["FirstName"] = SiteUser.FirstName;
            ViewData["LastName"] = SiteUser.LastName;
            ViewData["Title"] = SiteUser.Title;
            ViewData["Email"] = SiteUser.Email;
            */
            return Page();
        }
    }
}