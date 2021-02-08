using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;
using System.Security.Claims;
using System.IO;

namespace PriceCalculator.Areas.Model.Pages
{
    public class ListModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;

        public ListModel(SiteContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Quotation> Quotations { get; set; }

        public async Task OnGetAsync()
        {
            SiteUser user = await _userManager.GetUserAsync(User);

            Quotations = await _context.Quotations.Where(q => q.User == user).ToListAsync();
        }
    }
}