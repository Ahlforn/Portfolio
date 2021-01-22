using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.Printers
{
    public class DeleteModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public DeleteModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Printer Printer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Printer = await _context.Printers.FirstOrDefaultAsync(p => p.ID == id);

            if (Printer == null)
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

            Printer = await _context.Printers.FindAsync(id);

            if (Printer != null)
            {
                _context.Printers.Remove(Printer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
