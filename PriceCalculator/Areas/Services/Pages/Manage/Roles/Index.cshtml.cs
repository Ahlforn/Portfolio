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
    public class IndexModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<SiteUser> _userManager;

        public IList<IdentityRole> Roles { get; set; }
        public IList<SiteUser> Users { get; set; }

        [BindProperty]
        public string[] AssignedRoles { get; set; }

        public IndexModel(PriceCalculator.Models.SiteContext context, RoleManager<IdentityRole> roleManager, UserManager<SiteUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            Roles = await _roleManager.Roles.ToListAsync();
            Users = await _userManager.Users.ToListAsync();

            List<string> assignedRoles = new List<string>();
            foreach(var user in Users)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);

                if(roles.Count > 0)
                {
                    foreach(string roleName in roles)
                    {
                        IdentityRole role = await _roleManager.FindByNameAsync(roleName);
                        if(role != null)
                            assignedRoles.Add(user.Id + ":" + role.Id);
                    }
                }
            }

            AssignedRoles = assignedRoles.ToArray();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool error = false;

            if(AssignedRoles != null)
            {
                Dictionary<SiteUser, List<string>> assignments = new Dictionary<SiteUser, List<string>>();

                foreach(string str in AssignedRoles)
                {
                    string userId = str.Split(":")[0];
                    string roleId = str.Split(":")[1];

                    SiteUser user = await _userManager.FindByIdAsync(userId);
                    IdentityRole role = await _roleManager.FindByIdAsync(roleId);

                    if (!assignments.ContainsKey(user))
                        assignments.Add(user, new List<string>());

                    assignments[user].Add(role.Name);
                }

                IList<SiteUser> users = await _userManager.Users.ToListAsync();
                IList<IdentityRole> roles = await _roleManager.Roles.ToListAsync();

                foreach(var user in users)
                {
                    IList<string> existingRoles = await _userManager.GetRolesAsync(user);
                    List<string> assignedRoles = assignments.Keys.Contains(user) ? assignments[user] : new List<string>();
                    List<string> combined = existingRoles.Union(assignedRoles).ToList();

                    foreach(var role in roles)
                    {
                        if (existingRoles.Contains(role.Name) && !assignedRoles.Contains(role.Name))
                            await _userManager.RemoveFromRoleAsync(user, role.Name);

                        if (assignedRoles.Contains(role.Name) && !existingRoles.Contains(role.Name))
                            await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            // TODO: Handle errors better.

            if (!error)
                return RedirectToPage("./Index");
            else
                return BadRequest();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string id)
        {
            if (id == null | id.Length == 0)
                return BadRequest();

            SiteUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return BadRequest();

            IdentityResult result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToPage("./Index");
            else
                return BadRequest();
        }
    }
}
