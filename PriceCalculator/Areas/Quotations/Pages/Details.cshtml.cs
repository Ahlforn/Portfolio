using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Quotations.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly SiteContext _context;
        public Quotation Quotation { get; set; }

        public DetailsModel(SiteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Quotation = await _context.Quotations.Include(q => q.PrintModels).FirstOrDefaultAsync();
            return Page();
        }
    }
}