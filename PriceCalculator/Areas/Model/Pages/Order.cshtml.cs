using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Model.Pages
{
    public class OrderModel : PageModel
    {
        private readonly SiteContext _context;
        private readonly UserManager<SiteUser> _userManager;
        public Quotation Quotation { get; set; }

        public OrderModel(SiteContext context, UserManager<SiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Quotation = await _context.Quotations.Include(q => q.PrintModels).FirstOrDefaultAsync(q => q.ID == id);
            SiteUser user = await _userManager.GetUserAsync(User);

            if(Quotation == null || Quotation.User.Id != user.Id)
            {
                return NotFound();
            }

            return Page();
        }
    }
}