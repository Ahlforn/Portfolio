using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Roles
{
    public class EditModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        [BindProperty]
        public IdentityRole Role { get; set; }
        [BindProperty]
        public string ID { get; set; }

        public EditModel(PriceCalculator.Models.SiteContext context, RoleManager<IdentityRole> roleManager)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IdentityRole role = await _roleManager.FindByIdAsync(Role.Id);

            role.Name = Role.Name;

            IdentityResult result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                return RedirectToPage("./Index");
            else
                return BadRequest();
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.ID == id);
        }
    }
}
