using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Roles
{
    public class DeleteModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        [BindProperty]
        public IdentityRole Role { get; set; }
        
        public DeleteModel(PriceCalculator.Models.SiteContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Role = await _roleManager.FindByIdAsync(id);

            if (Role == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role == null)
                return NotFound();

            IdentityResult result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
                return RedirectToPage("./Index");
            else
                return BadRequest();
        }
    }
}
