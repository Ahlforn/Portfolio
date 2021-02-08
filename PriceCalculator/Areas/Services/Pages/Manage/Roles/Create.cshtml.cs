using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Roles
{
    public class CreateModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        [BindProperty]
        public IdentityRole Role { get; set; }

        public CreateModel(PriceCalculator.Models.SiteContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IdentityResult result = null;

            if(Role.Name.Length > 0)
                result = await _roleManager.CreateAsync(new IdentityRole(Role.Name));

            if (result != null && result.Succeeded)
                return RedirectToPage("./Index");
            else
                return BadRequest();
        }
    }
}