using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.LayerThicknesses
{
    public class DetailsModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public DetailsModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public LayerThickness LayerThickness { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LayerThickness = await _context.LayerThicknesses
                .Include(l => l.Material).FirstOrDefaultAsync(m => m.ID == id);

            if (LayerThickness == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
