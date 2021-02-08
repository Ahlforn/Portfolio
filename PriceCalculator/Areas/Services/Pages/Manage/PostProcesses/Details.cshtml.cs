using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.PostProcesses
{
    public class DetailsModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public DetailsModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        public PostProcess PostProcess { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PostProcess = await _context.PostProcesses.FirstOrDefaultAsync(m => m.ID == id);

            if (PostProcess == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
