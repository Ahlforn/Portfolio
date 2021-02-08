using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using PriceCalculator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;

        public bool ShowAll { get; set; }

        public IndexModel(SiteContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Quotation> Quotations { get; set; }

        public async Task<IActionResult> OnGetAsync(string handler)
        {
            SiteUser user = await _userManager.GetUserAsync(User);
            Quotations = await _context.Quotations.Include(q => q.User).OrderByDescending(q => q.Created).ToListAsync();

            if (handler == null || handler.Length == 0)
            {
                Quotations = Quotations.Where(q => q.User == user).ToList();
                ShowAll = false;
            }
            else
            {
                ShowAll = true;
            }

            return Page();
        }
    }
}