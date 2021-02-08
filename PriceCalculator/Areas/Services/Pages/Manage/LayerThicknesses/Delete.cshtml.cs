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
    public class DeleteModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public DeleteModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LayerThickness = await _context.LayerThicknesses.FindAsync(id);

            if (LayerThickness != null)
            {
                _context.LayerThicknesses.Remove(LayerThickness);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
