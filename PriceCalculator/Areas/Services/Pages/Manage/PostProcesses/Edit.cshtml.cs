using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCalculator.Models;

namespace PriceCalculator.Areas.Services.Pages.Manage.PostProcesses
{
    public class EditModel : PageModel
    {
        private readonly PriceCalculator.Models.SiteContext _context;

        public EditModel(PriceCalculator.Models.SiteContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(PostProcess).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostProcessExists(PostProcess.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PostProcessExists(int id)
        {
            return _context.PostProcesses.Any(e => e.ID == id);
        }
    }
}
